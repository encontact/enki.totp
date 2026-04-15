using System;
using System.Security.Cryptography;
using enki.token;
using OtpNet;
using QRCoder;
using Xunit;
using EnkiTotp = enki.totp.Totp;
using OtpNetTotp = OtpNet.Totp;

namespace google.authenticator.integration;

public class GoogleAuthenticatorIntegrationTests
{
    [Fact]
    public void EnkiTokenAndOtpNetShouldGenerateEquivalentCodeForSameTimeStep()
    {
        var base32Secret = ResolveOrCreateSecret();
        var unixNow = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var enkiTotp = new EnkiTotp(base32Secret, timeoutSeconds: 30, digits: 6);
        var enkiCode = enkiTotp.getCodeString(unixNow);

        var otpNetSecretBytes = Base32Encoding.ToBytes(base32Secret);
        var otpNetTotp = new OtpNetTotp(otpNetSecretBytes, step: 30, mode: OtpHashMode.Sha1, totpSize: 6);
        var otpNetCode = otpNetTotp.ComputeTotp(DateTimeOffset.FromUnixTimeSeconds((long)unixNow).UtcDateTime);

        Assert.Equal(enkiCode, otpNetCode);
    }

    [Fact]
    public void ShouldPrintTerminalQrCodeForGoogleAuthenticatorEnrollment()
    {
        var issuer = Environment.GetEnvironmentVariable("GOOGLE_AUTH_ISSUER") ?? "enki.totp";
        var account = Environment.GetEnvironmentVariable("GOOGLE_AUTH_ACCOUNT") ?? "local-user@example.com";
        var base32Secret = ResolveOrCreateSecret();

        var provisioningUri = BuildProvisioningUri(issuer, account, base32Secret);
        var capturedTime = DateTimeOffset.UtcNow;
        var unixNow = (ulong)capturedTime.ToUnixTimeSeconds();

        var enkiTotp = new EnkiTotp(base32Secret, timeoutSeconds: 30, digits: 6);
        var enkiToken = enkiTotp.getCodeString(unixNow);

        var otpNetSecretBytes = Base32Encoding.ToBytes(base32Secret);
        var otpNetTotp = new OtpNetTotp(otpNetSecretBytes, step: 30, mode: OtpHashMode.Sha1, totpSize: 6);
        var otpNetCode = otpNetTotp.ComputeTotp(capturedTime.UtcDateTime);

        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(provisioningUri, QRCodeGenerator.ECCLevel.Q);
        var asciiQrCode = new AsciiQRCode(qrData);
        var terminalQr = asciiQrCode.GetGraphic(1);

        Console.WriteLine("============================================");
        Console.WriteLine("Google Authenticator Enrollment (Manual)");
        Console.WriteLine($"Issuer     : {issuer}");
        Console.WriteLine($"Account    : {account}");
        Console.WriteLine($"Secret     : {base32Secret}");
        Console.WriteLine($"otpauth URI: {provisioningUri}");
        Console.WriteLine("--------------------------------------------");
        Console.WriteLine(terminalQr);
        Console.WriteLine("--------------------------------------------");
        Console.WriteLine($"enki.token code : {enkiToken}");
        Console.WriteLine($"Otp.NET code    : {otpNetCode}");
        Console.WriteLine("============================================");

        Assert.Equal(enkiToken, otpNetCode);
    }

    private static string ResolveOrCreateSecret()
    {
        var envSecret = Environment.GetEnvironmentVariable("GOOGLE_AUTH_SECRET");
        if (!string.IsNullOrWhiteSpace(envSecret))
        {
            return SanitizeBase32Secret(envSecret);
        }

        var generated = RandomNumberGenerator.GetBytes(20);
        return EnkiTotp.ToBase32String(generated);
    }

    private static string BuildProvisioningUri(string issuer, string account, string base32Secret)
    {
        var escapedIssuer = Uri.EscapeDataString(issuer);
        var escapedAccount = Uri.EscapeDataString(account);
        return $"otpauth://totp/{escapedIssuer}:{escapedAccount}?secret={base32Secret}&issuer={escapedIssuer}";
    }

    private static string SanitizeBase32Secret(string rawSecret)
    {
        return rawSecret
            .Trim()
            .Replace(" ", string.Empty)
            .ToUpperInvariant();
    }
}
