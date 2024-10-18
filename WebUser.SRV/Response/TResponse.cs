namespace WebUser.SRV.Response
{
    public class TResponse<T>
    {
        public bool Success { get; }
        public T Data { get; }
        public string Message { get; }

        public TResponse(bool success, T data, string message)
        {
            Success = success;
            Data = data;
            Message = message;
        }

        public static TResponse<T> Create(bool success, T data, string message)
        {
            return new TResponse<T>(success, data, message);
        }
    }
}

