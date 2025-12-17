namespace BillingSystem.SharedKernel
{
    public class ApiResponse<T>
    {
        public string Message { get; set; }
        public string StatusCode { get; set; }
        public T Data { get; set; }
        public bool IsSucceed { get; set; }
        public IEnumerable<ValidationError> ValidationErrors { get; set; }
        public static ApiResponse<T> IsSuccess(T data, string message, string status)
        {
            return new ApiResponse<T> 
            { 
                Message = message,
                StatusCode = status ,
                IsSucceed = true,
                Data = data,
                ValidationErrors = null
            };
        }
        public static ApiResponse<T> IsFailed(string message, string status, IEnumerable<ValidationError> validationErrors = null) 
        {
            return new ApiResponse<T>
            {
                Message = message,
                StatusCode = status,
                IsSucceed = false,
                ValidationErrors = validationErrors
            };
        }
    }

    public class ValidationError
    {
        public string Message { get; set; }
        public string Code { get; set; }
    }
}
