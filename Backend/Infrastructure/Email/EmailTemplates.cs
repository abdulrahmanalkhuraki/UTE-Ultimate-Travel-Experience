namespace Infrastructure.Email;

internal static class EmailTemplates
{
    public static string OtpVerification(string firstName, string code, int expiresInMinutes)
    {
        return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
  <meta name=""color-scheme"" content=""dark"" />
  <meta name=""supported-color-schemes"" content=""dark"" />
  <meta name=""x-apple-disable-message-reformatting"" />
  <title>Verification code</title>
</head>
<body style=""margin:0;padding:0;background-color:#0f0f14;font-family:'Segoe UI',Roboto,Helvetica,Arial,sans-serif;color:#e2e8f0;"">

  <div style=""display:none;max-height:0;overflow:hidden;font-size:1px;line-height:1px;color:#0f0f14;opacity:0;"">
    Your UTE Tourism verification code. Expires in {expiresInMinutes} minutes.
  </div>

  <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" bgcolor=""#0f0f14"" style=""background-color:#0f0f14;padding:32px 16px;"">
    <tr>
      <td align=""center"">

        <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""560"" bgcolor=""#1a1a22"" style=""max-width:560px;width:100%;background-color:#1a1a22;border-radius:18px;border:1px solid #2a2a35;overflow:hidden;"">

          <tr>
            <td align=""center"" style=""padding:44px 32px 16px;"">
              <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                <tr>
                  <td align=""center"" bgcolor=""#7c3aed"" style=""width:84px;height:84px;background-color:#7c3aed;background-image:linear-gradient(135deg,#7c3aed 0%,#a78bfa 100%);border-radius:20px;text-align:center;vertical-align:middle;"">
                    <div style=""font-size:44px;line-height:84px;"">&#9992;&#65039;</div>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <tr>
            <td align=""center"" style=""padding:8px 32px 4px;"">
              <h1 style=""margin:0;color:#c4b5fd;font-size:34px;font-weight:700;letter-spacing:0.5px;"">Verification code</h1>
            </td>
          </tr>

          <tr>
            <td align=""center"" style=""padding:6px 32px 20px;"">
              <p style=""margin:0;font-size:30px;line-height:1;"">&#128274;</p>
            </td>
          </tr>

          <tr>
            <td style=""padding:0 32px;"">
              <hr style=""border:none;border-top:1px solid #4c1d95;margin:0;"" />
            </td>
          </tr>

          <tr>
            <td align=""center"" style=""padding:28px 32px 0;"">
              <p style=""margin:0;color:#e2e8f0;font-size:16px;line-height:1.6;"">
                Hi <strong style=""color:#ffffff;"">{firstName}</strong>,
              </p>
            </td>
          </tr>

          <tr>
            <td align=""center"" style=""padding:8px 32px 24px;"">
              <p style=""margin:0;color:#cbd5e1;font-size:15px;line-height:1.7;"">
                Copy the code below to verify your <strong style=""color:#ffffff;"">UTE Tourism</strong> account.
              </p>
            </td>
          </tr>

          <tr>
            <td align=""center"" style=""padding:0 32px 24px;"">
              <table role=""presentation"" cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"">
                <tr>
                  <td align=""center"" bgcolor=""#0d2818"" style=""background-color:#0d2818;border:1px solid #166534;border-radius:14px;padding:26px 16px;"">
                    <p style=""margin:0;color:#4ade80;font-size:42px;font-weight:700;letter-spacing:14px;font-family:'Courier New',Courier,monospace;text-shadow:0 0 12px rgba(74,222,128,0.35);"">{code}</p>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <tr>
            <td align=""center"" style=""padding:0 32px 18px;"">
              <p style=""margin:0;color:#e2e8f0;font-size:15px;line-height:1.7;"">
                The code can only be used once and expires in <strong style=""color:#ffffff;"">{expiresInMinutes} minutes</strong>.
              </p>
            </td>
          </tr>

          <tr>
            <td align=""center"" style=""padding:0 32px 28px;"">
              <p style=""margin:0;color:#f87171;font-size:14px;line-height:1.6;"">
                If you did not request this code, please ignore this email &#9888;&#65039;.
              </p>
            </td>
          </tr>

          <tr>
            <td style=""padding:0 32px;"">
              <hr style=""border:none;border-top:1px solid #2a2a35;margin:0;"" />
            </td>
          </tr>

          <tr>
            <td align=""center"" style=""padding:22px 32px 32px;"">
              <p style=""margin:0;color:#64748b;font-size:12px;line-height:1.6;"">
                &#169; 2026 UTE Tourism &mdash; Ultimate Travel Experience. All rights reserved.
              </p>
            </td>
          </tr>

        </table>

      </td>
    </tr>
  </table>

</body>
</html>";
    }
}
