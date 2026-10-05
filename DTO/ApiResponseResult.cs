namespace Phase_07_Poc_01.DTO
{
    public class ApiResponseResult<T>
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public T? Result { get; set; }

        public static ApiResponseResult<T> SuccessResponse(T result, string message = "Success")
        {
            return new ApiResponseResult<T>
            {
                Success = true,
                Message = message,
                Result = result
            };
        }

        public static ApiResponseResult<T> FailureResponse(string message, T? result = default)
        {
            return new ApiResponseResult<T>
            {
                Success = false,
                Message = message,
                Result = result
            };
        }
  } 
}