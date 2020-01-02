namespace Simple.Rest.Common
{
    /// <summary>
    ///     Strongly typed response from the RestClient, contains all the HTTP related statuses, cookies &amp; headers.
    /// </summary>
    /// <typeparam name="T">The type of the resource.</typeparam>
    public interface IRestResponse<out T> : IRestResponse where T : class
    {
        /// <summary>
        ///     The response resource.
        /// </summary>
        T Resource { get; }
    }
}