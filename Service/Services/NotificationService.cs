using System.Net;
using System.Net.Mail;

namespace MyGarage
{
    public class NotificationService
    {
        // ── Email ─────────────────────────────────────────────────────────
        public async Task SendEmailAsync(
            string smtpHost,
            int smtpPort,
            string smtpUser,
            string smtpPassword,
            string to,
            string subject,
            string body,
            string? attachmentPath = null)
        {
            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPassword),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false  // ✅ obligatoire pour Gmail
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(smtpUser, "MyGarage"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mail.To.Add(to);

            if (attachmentPath != null && File.Exists(attachmentPath))
                mail.Attachments.Add(new Attachment(attachmentPath));

            await client.SendMailAsync(mail);
        }
        // ── SMS Free Mobile ───────────────────────────────────────────────
        public async Task SendSmsAsync(string userId, string apiKey, string message)
        {
            string encodedMsg = Uri.EscapeDataString(message);
            string url = $"https://smsapi.free-mobile.fr/sendmsg?user={userId}&pass={apiKey}&msg={encodedMsg}";

            using var http = new HttpClient();
            var response = await http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Erreur SMS Free Mobile : {(int)response.StatusCode} — {response.ReasonPhrase}");
        }
    }
}