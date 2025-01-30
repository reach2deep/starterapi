namespace starterkit.Core.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public ErrorDetails? Error { get; set; }

        public static ApiResponse<T> CreateSuccess(T data)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data
            };
        }

        public static ApiResponse<T> CreateError(string message, string? code = null, object? details = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Error = new ErrorDetails
                {
                    Message = message,
                    Code = code,
                    Details = details
                }
            };
        }
    }

    public class ErrorDetails
    {
        public string Message { get; set; } = string.Empty;
        public string? Code { get; set; }
        public object? Details { get; set; }
    }
} 