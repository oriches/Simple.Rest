namespace Simple.Rest.Tests.Infrastructure
{
    using System.IO;
    using System.Threading.Tasks;

    public interface ICompressor
    {
        string EncodingType { get; }

        Task Compress(Stream source, Stream destination);

        Task Decompress(Stream source, Stream destination);
    }
}