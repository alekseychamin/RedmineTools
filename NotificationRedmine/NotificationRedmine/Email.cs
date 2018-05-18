using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace NotificationRedmine
{
    class Email
    {
        public string RedmineEmailAddress = "redminestarsyst@gmail.com";
        public string host = "smtp.gmail.com";
        public string password = "yhXRp7CTw80g";
        public int port = 587;

        public void SendMail(UserRedmine userRedmine, string subject)
        {
            try
            {
                SmtpClient c = new SmtpClient("smtp.gmail.com", 587);

                MailAddress to = new MailAddress(userRedmine.Value.Email);
                MailAddress from = new MailAddress(RedmineEmailAddress);
                MailMessage msg = new MailMessage();

                msg.To.Add(to);
                msg.From = from;
                msg.BodyEncoding = Encoding.UTF8;
                msg.IsBodyHtml = false;
                msg.Subject = subject;
                msg.Body = userRedmine.message;
                c.Credentials = new System.Net.NetworkCredential(RedmineEmailAddress, password);
                c.EnableSsl = true;
                c.Send(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in sending email! " + ex.Message);
            }
        }
    }
}
