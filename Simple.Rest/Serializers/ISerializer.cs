namespace Simple.Rest.Serializers
{
    using System.IO;

    public interface ISerializer
    {
        string ContentType { get; }

        Stream Serialize<T>(T instance) where T : class;

        T Deserialize<T>(Stream stream);
    }
}