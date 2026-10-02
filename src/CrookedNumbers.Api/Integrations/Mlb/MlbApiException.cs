namespace CrookedNumbers.Api.Integrations.Mlb;

public class MlbApiException(string message, Exception? innerException = null)
    : Exception(message, innerException);

public sealed class MlbNotFoundException(string resource)
    : MlbApiException($"The requested MLB resource '{resource}' was not found.");

public sealed class MlbResponseException(string message, Exception? innerException = null)
    : MlbApiException(message, innerException);
