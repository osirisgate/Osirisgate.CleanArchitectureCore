using Osirisgate.CleanArchitectureCore.Response;

namespace Osirisgate.CleanArchitectureCore.Tests.Enums;

/// <remarks>
/// Organization: Osirisgate
/// Author: Ulrich Geraud A. | Software Engineer | developer@osirisgate.com
/// </remarks>
public sealed class StatusCodeTest
{
    [Fact]
    public void TestStatusCodeGetValue()
    {
        Assert.Equal(100, StatusCode.Continue.GetValue());
        Assert.Equal(101, StatusCode.SwitchingProtocols.GetValue());
        Assert.Equal(102, StatusCode.Processing.GetValue());
        Assert.Equal(103, StatusCode.EarlyHints.GetValue());

        Assert.Equal(200, StatusCode.Ok.GetValue());
        Assert.Equal(201, StatusCode.Created.GetValue());
        Assert.Equal(202, StatusCode.Accepted.GetValue());
        Assert.Equal(203, StatusCode.NonAuthoritativeInformation.GetValue());
        Assert.Equal(204, StatusCode.NoContent.GetValue());
        Assert.Equal(205, StatusCode.ResetContent.GetValue());
        Assert.Equal(206, StatusCode.PartialContent.GetValue());
        Assert.Equal(207, StatusCode.MultiStatus.GetValue());
        Assert.Equal(208, StatusCode.AlreadyReported.GetValue());
        Assert.Equal(226, StatusCode.ImUsed.GetValue());

        Assert.Equal(300, StatusCode.MultipleChoices.GetValue());
        Assert.Equal(301, StatusCode.MovedPermanently.GetValue());
        Assert.Equal(302, StatusCode.Found.GetValue());
        Assert.Equal(303, StatusCode.SeeOther.GetValue());
        Assert.Equal(304, StatusCode.NotModified.GetValue());
        Assert.Equal(305, StatusCode.UseProxy.GetValue());
        Assert.Equal(306, StatusCode.Reserved.GetValue());
        Assert.Equal(307, StatusCode.TemporaryRedirect.GetValue());
        Assert.Equal(308, StatusCode.PermanentlyRedirect.GetValue());

        Assert.Equal(400, StatusCode.BadRequest.GetValue());
        Assert.Equal(401, StatusCode.Unauthorized.GetValue());
        Assert.Equal(402, StatusCode.PaymentRequired.GetValue());
        Assert.Equal(403, StatusCode.Forbidden.GetValue());
        Assert.Equal(404, StatusCode.NotFound.GetValue());
        Assert.Equal(405, StatusCode.MethodNotAllowed.GetValue());
        Assert.Equal(406, StatusCode.NotAcceptable.GetValue());
        Assert.Equal(407, StatusCode.ProxyAuthenticationRequired.GetValue());
        Assert.Equal(408, StatusCode.RequestTimeout.GetValue());
        Assert.Equal(409, StatusCode.Conflict.GetValue());
        Assert.Equal(410, StatusCode.Gone.GetValue());
        Assert.Equal(411, StatusCode.LengthRequired.GetValue());
        Assert.Equal(412, StatusCode.PreconditionFailed.GetValue());
        Assert.Equal(413, StatusCode.RequestEntityTooLarge.GetValue());
        Assert.Equal(414, StatusCode.RequestUriTooLong.GetValue());
        Assert.Equal(415, StatusCode.UnsupportedMediaType.GetValue());
        Assert.Equal(416, StatusCode.RequestedRangeNotSatisfiable.GetValue());
        Assert.Equal(417, StatusCode.ExpectationFailed.GetValue());
        Assert.Equal(418, StatusCode.IamATeapot.GetValue());
        Assert.Equal(421, StatusCode.MisdirectedRequest.GetValue());
        Assert.Equal(422, StatusCode.UnprocessableEntity.GetValue());
        Assert.Equal(423, StatusCode.Locked.GetValue());
        Assert.Equal(424, StatusCode.FailedDependency.GetValue());
        Assert.Equal(425, StatusCode.TooEarly.GetValue());
        Assert.Equal(426, StatusCode.UpgradeRequired.GetValue());
        Assert.Equal(428, StatusCode.PreconditionRequired.GetValue());
        Assert.Equal(429, StatusCode.TooManyRequests.GetValue());
        Assert.Equal(431, StatusCode.RequestHeaderFieldsTooLarge.GetValue());
        Assert.Equal(451, StatusCode.UnavailableForLegalReasons.GetValue());

        Assert.Equal(500, StatusCode.InternalServerError.GetValue());
        Assert.Equal(501, StatusCode.NotImplemented.GetValue());
        Assert.Equal(502, StatusCode.BadGateway.GetValue());
        Assert.Equal(503, StatusCode.ServiceUnavailable.GetValue());
        Assert.Equal(504, StatusCode.GatewayTimeout.GetValue());
        Assert.Equal(505, StatusCode.HttpVersionNotSupported.GetValue());
        Assert.Equal(506, StatusCode.VariantAlsoNegotiatesExperimental.GetValue());
        Assert.Equal(507, StatusCode.InsufficientStorage.GetValue());
        Assert.Equal(508, StatusCode.LoopDetected.GetValue());
        Assert.Equal(510, StatusCode.NotExtended.GetValue());
        Assert.Equal(511, StatusCode.NetworkAuthenticationRequired.GetValue());
    }
}
