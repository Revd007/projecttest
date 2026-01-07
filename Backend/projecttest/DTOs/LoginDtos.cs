namespace projecttest.Dtos
{
    public record LoginRequest(
        string Identifier,
        string Password
    );

    public record LoginResponse (
        string Token,
        string Username,
        string Email,
        string Phone
     );

    public record VerifyRequest (
        string Identifier,
        string Type);

    public record RegisterRequest(
        string Username,
        string Email,
        string PhoneNumber,
        string Password
    );

    public record VerifyOtpRequest(string Identifier, string OtpCode);
}
