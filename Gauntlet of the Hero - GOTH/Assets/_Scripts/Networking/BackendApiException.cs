using GauntletOfTheHero.Backend.Models;

namespace GauntletOfTheHero.Backend.Networking
{
    public sealed class BackendApiException : System.Exception
    {
        public long StatusCode { get; }
        public string ResponseBody { get; }
        public BackendErrorResponseDto BackendError { get; }

        public BackendApiException(long statusCode, string responseBody, BackendErrorResponseDto backendError)
            : base(BuildMessage(statusCode, backendError, responseBody))
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
            BackendError = backendError;
        }

        private static string BuildMessage(long statusCode, BackendErrorResponseDto backendError, string responseBody)
        {
            if (backendError != null)
            {
                return $"Backend request failed with status {statusCode} ({backendError.code}): {backendError.message}";
            }

            return $"Backend request failed with status {statusCode}: {responseBody}";
        }
    }
}