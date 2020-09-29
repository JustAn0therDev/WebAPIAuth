namespace WebAPIAuth.Responses
{
    public class ErrorResponse {
        public string ErrorMessage { get; set; }

        public ErrorResponse(string errorMessage) {
            ErrorMessage = errorMessage;
        }
    }
}