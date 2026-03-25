namespace DaimyoDataSolutions.Application.Services.Base
{
    public static class ServiceConstants
    {
        private const string _recordNotFound = "Record not found";
        private const string _requestIsInvalid = "Request is invalid";
        private const string _requestProcessingError = "An error occurred while processing your request.";

        public static string RecordNotFound => _recordNotFound;
        public static string RequestIsInvalid => _requestIsInvalid;
        public static string RequestProcessingError => _requestProcessingError;
    }
}
