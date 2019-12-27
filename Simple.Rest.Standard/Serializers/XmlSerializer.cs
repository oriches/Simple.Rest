using System.IO;
using System.Runtime.Serialization;

namespace Simple.Rest.Standard.Serializers
{
    /// <summary>
    ///     XML Serialization interface used by RestClient, serialization is done using .Net DataContractSerializer.
    /// </summary>
    public sealed class XmlSerializer : ISerializer
    {
        /// <summary>
        ///     Default constructor.
        /// </summary>
        public XmlSerializer()
        {
            ContentType = "application/xml";
        }

        /// <summary>
        ///     The content type is "application/xml".
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        ///     Serializes a resource of type T to a stream.
        /// </summary>
        /// <typeparam name="T">The type of the resource.</typeparam>
        /// <param name="instance">The instance to be serialized</param>
        /// <returns>The serialization stream.</returns>
        public Stream Serialize<T>(T instance) where T : class
        {
            var serializer = new DataContractSerializer(typeof(T));
            var stream = new MemoryStream();

            serializer.WriteObject(stream, instance);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        /// <summary>
        ///     Deserializes a resource stream to an instance of type T.
        /// </summary>
        /// <typeparam name="T">The type of the resource.</typeparam>
        /// <returns>The instance of T.</returns>
        public T Deserialize<T>(Stream stream)
        {
            var serializer = new DataContractSerializer(typeof(T));
            return (T) serializer.ReadObject(stream);
        }
    }
}