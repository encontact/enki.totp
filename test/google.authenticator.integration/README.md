# Google Authenticator Integration Test

This test project validates interoperability between `enki.token` and Google Authenticator (via Otp.NET), and prints a terminal QR code using QRCoder.

## Run

```bash
cd /home/reinaldo/code/enki.totp

dotnet test test/google.authenticator.integration/google.authenticator.integration.csproj -l "console;verbosity=detailed"
```

## Optional Environment Variables

Use a fixed secret to keep the same account on your phone:

```bash
export GOOGLE_AUTH_SECRET="YOUR_BASE32_SECRET"
export GOOGLE_AUTH_ISSUER="enki.totp"
export GOOGLE_AUTH_ACCOUNT="you@example.com"

dotnet test test/google.authenticator.integration/google.authenticator.integration.csproj -l "console;verbosity=detailed"
```

## What to expect

- Test `ShouldPrintTerminalQrCodeForGoogleAuthenticatorEnrollment` prints:
  - Secret
  - `otpauth://` provisioning URL
  - ASCII QR code in terminal
  - Current code from `enki.token`
  - Current code from Otp.NET
- The test asserts both codes are equal.

## .NET 10

Current repository SDK is pinned to .NET 8 in `global.json`.
If you want to run with .NET 10 SDK, you can update `global.json` and run the same command with your .NET 10 environment.
