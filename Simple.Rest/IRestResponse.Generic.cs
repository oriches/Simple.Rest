namespace Simple.Rest
{
    public interface IRestResponse<out T>  : IRestResponse where T : class
    {
        T Resource { get; }
    }
}