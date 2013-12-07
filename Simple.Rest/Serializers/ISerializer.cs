namespace Simple.Rest.Serializers
{
    using System.Diagnostics.Contracts;
    using System.IO;

    [ContractClass(typeof(SerializerContract))]
    public interface ISerializer
    {
        string ContentType { get; }

        Stream Serialize<T>(T instance) where T : class;

        T Deserialize<T>(Stream stream);
    }
}