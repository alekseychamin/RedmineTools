using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinRedminePlaning
{
    class Email
    {
        public string RedmineEmailAddress = "redminestarsyst@gmail.com";
        public string host = "smtp.gmail.com";
        public string password = "yhXRp7CTw80g";
        public int port = 587;

        private void Send(string toEmailAddress, string ccEmailAddress, string body, string subject)
        {
            try
            {
                SmtpClient c = new SmtpClient("smtp.gmail.com", 587);

                //MailAddress to = new MailAddress(toEmailAddress);
                MailAddress from = new MailAddress(RedmineEmailAddress);
                MailMessage msg = new MailMessage();

                if (!toEmailAddress.Equals(""))
                    msg.To.Add(toEmailAddress);

                if (!ccEmailAddress.Equals(""))
                    msg.CC.Add(ccEmailAddress);

                msg.From = from;
                msg.BodyEncoding = Encoding.UTF8;
                msg.IsBodyHtml = false;
                msg.Subject = subject;
                msg.Body = body;
                c.Credentials = new System.Net.NetworkCredential(RedmineEmailAddress, password);
                c.EnableSsl = true;

                if (!toEmailAddress.Equals("") || !ccEmailAddress.Equals(""))
                    c.Send(msg);
                //Console.WriteLine("email send to " + toEmailAddress);
                //MessageBox.Show("email send to " + toEmailAddress + "\n" + "email send to cc " + ccEmailAddress);
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error in sending email! " + ex);
                MessageBox.Show("Error in sending email! " + ex);
            }
        }

        private string MakeEmailFromList(List<string> listEmail)
        {
            string res = "";

            foreach (string email in listEmail)
            {
                if (!res.Equals(""))
                    res += ",";

                res += email;
            }

            return res;
        }
        public void SendMail(string body, string subject, List<string> toEmailAddress, List<string> ccEmailAddress)
        {
            string toAddress, ccAddress;

            toAddress = MakeEmailFromList(toEmailAddress);
            ccAddress = MakeEmailFromList(ccEmailAddress);

            Send(toAddress, ccAddress, body, subject);

        }
    }
}
