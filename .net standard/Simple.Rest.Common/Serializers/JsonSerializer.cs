using System.IO;
using Newtonsoft.Json;

namespace Simple.Rest.Common.Serializers
{
    /// <summary>
    ///     JSON Serialization interface used by RestClient, serialization is done using JSON.NET.
    /// </summary>
    public sealed class JsonSerializer : ISerializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _serializer;

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public JsonSerializer()
        {
            _serializer = new Newtonsoft.Json.JsonSerializer();

            ContentType = "application/json";
        }

        /// <summary>
        ///     The content type is "application/json".
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
            var writer = new StreamWriter(new MemoryStream());
            _serializer.Serialize(writer, instance);

            writer.Flush();
            writer.BaseStream.Position = 0;

            return writer.BaseStream;
        }

        /// <summary>
        ///     Deserializes a resource stream to an instance of type T.
        /// </summary>
        /// <typeparam name="T">The type of the resource.</typeparam>
        /// <returns>The instance of T.</returns>
        public T Deserialize<T>(Stream stream)
        {
            var serializer = new Newtonsoft.Json.JsonSerializer();

            using (var reader = new StreamReader(stream))
            {
                return serializer.Deserialize<T>(new JsonTextReader(reader));
            }
        }
    }
}