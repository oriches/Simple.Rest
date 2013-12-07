namespace Simple.Rest.Serializers
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;

    [ContractClassFor(typeof(ISerializer))]
    internal abstract class SerializerContract : ISerializer
    {
        public string ContentType { get; private set; }

        public Stream Serialize<T>(T instance) where T : class
        {
            Contract.Requires<ArgumentNullException>(instance != null);

            return null;
        }

        public T Deserialize<T>(Stream stream)
        {
            Contract.Requires<ArgumentNullException>(stream != null);

            return default(T);
        }
    }
}