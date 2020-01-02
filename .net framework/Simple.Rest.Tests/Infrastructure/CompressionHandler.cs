using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Rest.Tests.Infrastructure
{
    public sealed class CompressionHandler : DelegatingHandler
    {
        public Collection<ICompressor> Compressors;

        public CompressionHandler()
        {
            Compressors = new Collection<ICompressor> {new GZipCompressor(), new DeflateCompressor()};
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var contentEncoding = request.Content.Headers.ContentEncoding;
            if (contentEncoding != null && contentEncoding.Any() && request.Content != null)
            {
                var encoding = request.Content.Headers.ContentEncoding.First();
                var compressor = Compressors.FirstOrDefault(c =>
                    c.EncodingType.Equals(encoding, StringComparison.InvariantCultureIgnoreCase));

                if (compressor != null)
                    request.Content = await DecompressContentAsync(request.Content, compressor).ConfigureAwait(true);
            }

            var compressedResponse = await base.SendAsync(request, cancellationToken)
                .ContinueWith(async t =>
                {
                    var response = t.Result;

                    if (response.Content != null)
                    {
                        var acceptEncoding = response.RequestMessage.Headers.AcceptEncoding;
                        if (acceptEncoding != null && acceptEncoding.Any())
                        {
                            var encoding = response.RequestMessage.Headers.AcceptEncoding.First().Value;
                            var compressor = Compressors.FirstOrDefault(c =>
                                c.EncodingType.Equals(encoding, StringComparison.InvariantCultureIgnoreCase));

                            if (compressor != null)
                                response.Content = await CompressContentAsync(response.Content, compressor)
                                    .ConfigureAwait(true);
                        }
                    }

                    return response;
                }, TaskContinuationOptions.OnlyOnRanToCompletion).ConfigureAwait(true);

            return compressedResponse.Result;
        }

        private static async Task<HttpContent> DecompressContentAsync(HttpContent compressedContent,
            ICompressor compressor)
        {
            using (compressedContent)
            {
                var decompressed = new MemoryStream();
                await compressor.Decompress(await compressedContent.ReadAsStreamAsync(), decompressed)
                    .ConfigureAwait(true);

                decompressed.Position = 0;
                var newContent = new StreamContent(decompressed);

                newContent.Headers.ContentType = compressedContent.Headers.ContentType;
                return newContent;
            }
        }

        private static async Task<HttpContent> CompressContentAsync(HttpContent content, ICompressor compressor)
        {
            using (content)
            {
                var compressed = new MemoryStream();
                await compressor.Compress(await content.ReadAsStreamAsync(), compressed).ConfigureAwait(true);

                compressed.Position = 0;
                var newContent = new StreamContent(compressed);

                newContent.Headers.ContentEncoding.Add(compressor.EncodingType);
                newContent.Headers.ContentType = content.Headers.ContentType;
                return newContent;
            }
        }
    }
}