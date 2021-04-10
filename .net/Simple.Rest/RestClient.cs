using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using Simple.Rest.Common;
using Simple.Rest.Common.Serializers;

namespace Simple.Rest
{
    /// <summary>
    ///     Class for resource orientated RESTful interface, supports verbs GET, POST, PUT &amp; DELETE.
    /// </summary>
    public sealed class RestClient : BaseRestClient
    {
        /// <summary>
        ///     Creates an instance with the JSON serializer.
        /// </summary>
        public RestClient()
            : base(new JsonSerializer(), new JsonSerializer())
        {
        }

        /// <summary>
        ///     Creates an instance with a customer serializer, e.g. XML or JSON.
        /// </summary>
        /// <param name="serializer"></param>
        public RestClient(ISerializer serializer)
            : base(serializer, serializer)
        {
        }

        protected override async Task WriteToRequestStream(HttpWebRequest request, byte[] body)
        {
            await using var requestStream = new BinaryWriter(await request.GetRequestStreamAsync());
            if (ShouldCompressWithGzip(request))
            {
                byte[] compressedBody;
                await using (var outStream = new MemoryStream())
                {
                    await using (var gzipStream = new GZipStream(outStream, CompressionMode.Compress))
                    {
                        await using var bodyStream = new MemoryStream(body);
                        bodyStream.CopyTo(gzipStream);
                    }

                    compressedBody = outStream.ToArray();
                }

                requestStream.Write(compressedBody, 0, compressedBody.Length);
            }
            else if (ShouldCompressWithDeflate(request))
            {
                byte[] compressedBody;
                await using (var outStream = new MemoryStream())
                {
                    await using (var gzipStream = new DeflateStream(outStream, CompressionMode.Compress))
                    {
                        await using var bodyStream = new MemoryStream(body);
                        bodyStream.CopyTo(gzipStream);
                    }

                    compressedBody = outStream.ToArray();
                }

                requestStream.Write(compressedBody, 0, compressedBody.Length);
            }
            else if (ShouldCompressWithBrotli(request))
            {
                byte[] compressedBody;
                await using (var outStream = new MemoryStream())
                {
                    await using (var brotliStream = new BrotliStream(outStream, CompressionMode.Compress))
                    {
                        await using var bodyStream = new MemoryStream(body);
                        bodyStream.CopyTo(brotliStream);
                    }

                    compressedBody = outStream.ToArray();
                }

                requestStream.Write(compressedBody, 0, compressedBody.Length);
            }
            else
            {
                requestStream.Write(body, 0, body.Length);
            }
        }

        protected override RestResponse<T> ProcessResponse<T>(HttpWebResponse response)
        {
            try
            {
                if (IsGzipCompressed(response))
                {
                    using var stream = response.GetResponseStream();
                    using var gZipStream = new GZipStream(stream, CompressionMode.Decompress);
                    var result = Deserialize<T>(gZipStream);
                    return new RestResponse<T>(response, result);
                }

                if (IsBrotliCompressed(response))
                {
                    using var stream = response.GetResponseStream();
                    using var brotliStream = new BrotliStream(stream, CompressionMode.Decompress);
                    var result = Deserialize<T>(brotliStream);
                    return new RestResponse<T>(response, result);
                }

                if (IsDeflateCompressed(response))
                {
                    using var stream = response.GetResponseStream();
                    using var deflateStream = new DeflateStream(stream, CompressionMode.Decompress);
                    var result = Deserialize<T>(deflateStream);
                    return new RestResponse<T>(response, result);
                }

                using (var stream = response.GetResponseStream())
                {
                    var result = Deserialize<T>(stream);
                    return new RestResponse<T>(response, result);
                }
            }
            catch (Exception ex)
            {
                return new RestResponse<T>(response, ex, default(T));
            }
        }

        private static bool IsBrotliCompressed(WebResponse response)
        {
            var encoding = response.Headers["Content-Encoding"];
            return !string.IsNullOrEmpty(encoding) && encoding.ToLower().Contains("br");
        }

        private static bool IsDeflateCompressed(WebResponse response)
        {
            var encoding = response.Headers["Content-Encoding"];
            return !string.IsNullOrEmpty(encoding) && encoding.ToLower().Contains("deflate");
        }

        private static bool ShouldCompressWithDeflate(WebRequest request)
        {
            var encoding = request.Headers["Accept-Encoding"];
            return !string.IsNullOrEmpty(encoding) && encoding.ToLower().Contains("deflate");
        }

        private static bool ShouldCompressWithBrotli(WebRequest request)
        {
            var encoding = request.Headers["Accept-Encoding"];
            return !string.IsNullOrEmpty(encoding) && encoding.ToLower().Contains("br");
        }
    }
}