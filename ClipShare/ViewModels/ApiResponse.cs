namespace ClipShare.ViewModels
{
    public class ApiResponse
    {
        public ApiResponse()
        {
            
        }
        public ApiResponse(int statusCode, string title = null, string message = null, object result = null)
        {
            StatusCode = statusCode;
            Title = title ?? GetDefaultTitle(statusCode);
            Message = message ?? GetDefaultMessage(statusCode);
            Result = result;

            if (statusCode == 200 || statusCode == 201)
            {
                IsSuccess = true;
            }
            else
            {
                IsSuccess = false;
            }
        }

        public int StatusCode { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }
        public bool IsSuccess { get; set; }

        private string GetDefaultTitle(int statusCode)
        {
            return statusCode switch
            {
                200 => "Success",
                201 => "Success",
                400 => "Bad Request",
                401 => "Unauthorized",
                404 => "Not Found",
                500 => "Internal Server Error",
                _ => null
            };
        }

        private string GetDefaultMessage(int statusCode)
        {
            return statusCode switch
            {
                200 => "Success",
                201 => "Success",
                400 => "Bad Request",
                401 => "Unauthorized",
                404 => "Not Found",
                500 => "Internal Server Error",
                _ => null
            };
        }
    }
}
