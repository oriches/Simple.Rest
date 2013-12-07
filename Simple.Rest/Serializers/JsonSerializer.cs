namespace Simple.Rest.Serializers
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    public sealed class JsonSerializer : ISerializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _serializer;

        public string ContentType { get; private set; }

        public JsonSerializer()
        {
            _serializer = new Newtonsoft.Json.JsonSerializer();

            ContentType = "application/json";
        }

        public Stream Serialize<T>(T instance) where  T : class
        {
            var writer = new StreamWriter(new MemoryStream(), Encoding.UTF8);
            _serializer.Serialize(writer, instance);
            
            writer.Flush();
            writer.BaseStream.Seek(0, SeekOrigin.Begin);

            return writer.BaseStream;
        }

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