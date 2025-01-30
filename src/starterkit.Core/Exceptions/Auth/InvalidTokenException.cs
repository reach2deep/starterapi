namespace starterkit.Core.Exceptions.Auth
{
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException() : base("Invalid token") { }
        public InvalidTokenException(string message) : base(message) { }
        public InvalidTokenException(string message, Exception inner) : base(message, inner) { }
    }
} 