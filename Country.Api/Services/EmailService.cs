using Country.Api.Dtos;
using Country.Api.Interfaces.Services;
using System.Net.Mail;

namespace Country.Api.Services
{
    public class EmailService : IEmailService
    {
        public bool SendEmail(MailBodyDto body)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("adrenalinhospital@gmail.com");
            mailMessage.To.Add(new MailAddress(body.Email));

            mailMessage.Subject = body.Subject;
            mailMessage.IsBodyHtml = true;

            if (body.File != null)
            {
                mailMessage.Attachments.Add(new Attachment(new MemoryStream(body.File), body.Filename));
            }

            if (body.Code != null)
            {
                var imageData = Convert.FromBase64String(body.Code.Split(',')[1]);
                AlternateView imgview = AlternateView.CreateAlternateViewFromString(body.Content + "<img src=cid:imgpath width=200 />", null, "text/html");
                LinkedResource lr = new LinkedResource(new MemoryStream(imageData), "image/jpeg");
                lr.ContentId = "imgpath";
                imgview.LinkedResources.Add(lr);
                mailMessage.AlternateViews.Add(imgview);
                mailMessage.Body = lr.ContentId;
            }


            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("adrenalinhospital@gmail.com", "odioogqpwsquwcii");
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Port = 587;

            try
            {
                client.Send(mailMessage);
                return true;
            }
            catch (System.Exception)
            {


            }

            return false;
        }
    }
}
