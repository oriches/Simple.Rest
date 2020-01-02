using System.IO;

namespace Simple.Rest.Common.Serializers
{
    /// <summary>
    ///     Serialization interface used by RestClient.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        ///     The serializer type.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        ///     Serializes a resource of type T to a stream.
        /// </summary>
        /// <typeparam name="T">The type of the resource.</typeparam>
        /// <param name="instance">The instance to be serialized</param>
        /// <returns>The serialization stream.</returns>
        Stream Serialize<T>(T instance) where T : class;

        /// <summary>
        ///     Deserializes a resource stream to an instance of type T.
        /// </summary>
        /// <typeparam name="T">The type of the resource.</typeparam>
        /// <returns>The instance of T.</returns>
        T Deserialize<T>(Stream stream);
    }
}