namespace Osirisgate.CleanArchitectureCore.Response;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public enum StatusCode
{
    Continue = 100,
    SwitchingProtocols = 101,
    Processing = 102,                     // RFC2518
    EarlyHints = 103,                    // RFC8297

    Ok = 200,
    Created = 201,
    Accepted = 202,
    NonAuthoritativeInformation = 203,
    NoContent = 204,
    ResetContent = 205,
    PartialContent = 206,
    MultiStatus = 207,                   // RFC4918
    AlreadyReported = 208,               // RFC5842
    ImUsed = 226,                        // RFC3229

    MultipleChoices = 300,
    MovedPermanently = 301,
    Found = 302,
    SeeOther = 303,
    NotModified = 304,
    UseProxy = 305,
    Reserved = 306,
    TemporaryRedirect = 307,
    PermanentlyRedirect = 308,           // RFC7238

    BadRequest = 400,
    Unauthorized = 401,
    PaymentRequired = 402,
    Forbidden = 403,
    NotFound = 404,
    MethodNotAllowed = 405,
    NotAcceptable = 406,
    ProxyAuthenticationRequired = 407,
    RequestTimeout = 408,
    Conflict = 409,
    Gone = 410,
    LengthRequired = 411,
    PreconditionFailed = 412,
    RequestEntityTooLarge = 413,
    RequestUriTooLong = 414,
    UnsupportedMediaType = 415,
    RequestedRangeNotSatisfiable = 416,
    ExpectationFailed = 417,
    IamATeapot = 418,                    // RFC2324
    MisdirectedRequest = 421,            // RFC7540
    UnprocessableEntity = 422,           // RFC4918
    Locked = 423,                         // RFC4918
    FailedDependency = 424,               // RFC4918
    TooEarly = 425,                        // RFC-ietf-httpbis-replay-04
    UpgradeRequired = 426,                // RFC2817
    PreconditionRequired = 428,           // RFC6585
    TooManyRequests = 429,                // RFC6585
    RequestHeaderFieldsTooLarge = 431,    // RFC6585
    UnavailableForLegalReasons = 451,    // RFC7725

    InternalServerError = 500,
    NotImplemented = 501,
    BadGateway = 502,
    ServiceUnavailable = 503,
    GatewayTimeout = 504,
    HttpVersionNotSupported = 505,
    VariantAlsoNegotiatesExperimental = 506, // RFC2295
    InsufficientStorage = 507,                // RFC4918
    LoopDetected = 508,                       // RFC5842
    NotExtended = 510,                        // RFC2774
    NetworkAuthenticationRequired = 511       // RFC6585
}

/// <summary>
/// Extension methods for <see cref="StatusCode"/> enum.
/// </summary>
/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public static class StatusCodeExtensions
{
    /// <summary>
    /// Gets the integer value of the status code.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    /// <returns>The integer value of the status code.</returns>
    public static int GetValue(this StatusCode statusCode)
    {
        return (int)statusCode;
    }
}
