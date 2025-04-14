namespace vm_api_backend_appservice.Exceptions
{
    public abstract class BaseException : Exception
    {
        public BaseException(string message) : base(message)
        {
        }
    }
} 