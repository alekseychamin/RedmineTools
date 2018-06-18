using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationRedmine
{
    class Manage
    {
        List<UserRedmine> listUserRedmine = new List<UserRedmine>();
        RedmineManager redmineManager = null;
        Email email;
        TimeSpan dt;
        string[] noEmailSend;

        struct DayTimeStart
        {
            public DayOfWeek dayOfWeek;
            public string timeStart;
        }

        DayTimeStart[] dayTimeStart = new DayTimeStart[3];

        public Manage(string host, string key)
        {
            dayTimeStart[0].dayOfWeek = DayOfWeek.Monday;
            dayTimeStart[0].timeStart = "9:00";

            dayTimeStart[1].dayOfWeek = DayOfWeek.Wednesday;
            dayTimeStart[1].timeStart = "9:00";

            dayTimeStart[2].dayOfWeek = DayOfWeek.Friday;
            dayTimeStart[2].timeStart = "9:00";

            noEmailSend = new[] {"Арбузов Владимир Леонидович"};

            try
            {
                email = new Email();
                redmineManager = new RedmineManager(host, key);
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString() + " Could not connect to redmine host! " + ex);
                SaveError.SaveMessage(ex.ToString());
            }
        }
        
        private bool isDayOfWeek(DayOfWeek dayOfWeek, out string timeStart)
        {
            bool result = false;
            timeStart = "";

            foreach (DayTimeStart dayTime in dayTimeStart)
            {
                if (dayTime.dayOfWeek == dayOfWeek)
                {
                    result = true;
                    timeStart = dayTime.timeStart;
                    break;
                }
            }

            return result;
        }

        private DateTime GetDate(DateTime stepDateTime, string timeStart)
        {                    
            int year = stepDateTime.Year;
            int mounth = stepDateTime.Month;
            int day = stepDateTime.Day;

            string[] timeParts = timeStart.Split(new char[1] { ':' });
            int hour = int.Parse(timeParts[0]);
            int minute = int.Parse(timeParts[1]);

            return new DateTime(year, mounth, day, hour, minute, 0);            
        }

        public TimeSpan GetTimeStart()
        {
            bool done = false;
            string timeStart = "";

            TimeSpan dt = new TimeSpan();

            DateTime stepDateTime = DateTime.Now;
            DateTime currentDateTime = DateTime.Now;

            while (!done)
            {                
                if (isDayOfWeek(stepDateTime.DayOfWeek, out timeStart) && ( (dt = ( GetDate(stepDateTime, timeStart) - currentDateTime)).TotalSeconds > 0) )
                {                    
                    done = true;                    
                }
                else
                    stepDateTime = stepDateTime.AddDays(1);
            }

            Console.WriteLine("near date to start = {0}, dt = {1} hours", GetDate(stepDateTime, timeStart).ToString(), dt.TotalHours);

            return dt;
        }

        private void Sort()
        {
            foreach (UserRedmine userRedmine in listUserRedmine)
            {
                userRedmine.ListUserIssue.Sort();
            }
        }

        public void MakeNotificationToTime()
        {            
            GetUserOpenIssue();
            SetUserIssueNotification();
            SetEmailMessage();
            //ShowNotification();
            SendEmail(noEmailSend);            
        }

        public void StartMakeNotification()
        {
            while (true)
            {
                dt = GetTimeStart();
                Task.Delay(dt).ContinueWith((x) => this.MakeNotificationToTime());
                Thread.Sleep(dt);                            
            }
        }

        public void GetUserOpenIssue()
        {
            listUserRedmine.Clear();

            try
            {
                NameValueCollection param = new NameValueCollection { { "Id", "*" } };
                foreach (var user in redmineManager.GetObjects<User>(param))
                {
                    UserRedmine userRedmine = new UserRedmine();
                    userRedmine.Value = user;
                    listUserRedmine.Add(userRedmine);

                    if (userRedmine.Value.LastName.Equals("Чамин"))
                    {
                        userRedmine.EmailAddress = "ale-san2006@mail.ru";
                    }
                }

                param = new NameValueCollection { { "status_id", "open" } };
                foreach (var issue in redmineManager.GetObjects<Issue>(param))
                {
                    UserRedmine userRedmine = null;

                    if (issue.AssignedTo == null)
                    {
                        userRedmine = listUserRedmine.Find(x => x.Value.Id == issue.Author.Id);
                    }
                    else
                    {
                        userRedmine = listUserRedmine.Find(x => x.Value.Id == issue.AssignedTo.Id);
                    }

                    UserIssue userIssue = new UserIssue();
                    userIssue.issue = issue;

                    userRedmine.ListUserIssue.Add(userIssue);
                }

                this.Sort();
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString() + " Error in GetUserOpenIssue: {0}", ex);
                SaveError.SaveMessage(ex.ToString());
            }
            
        }

        public void SendEmail(params string[] noNameSendMail)
        {
            bool isNameSendMail = true;            

            foreach (var userRedmine in listUserRedmine)
            {
                isNameSendMail = true;
                foreach (string name in noNameSendMail)
                {
                    if (userRedmine.FullName.Contains(name))
                    {
                        isNameSendMail = false;
                        break;
                    }
                }

                if (isNameSendMail & userRedmine.isNeedToSend) // & userRedmine.FullName.Contains("Чамин"))
                {
                    email.SendMail(userRedmine.messageSend, "Redmine просроченные задачи и проценты выполнения задач", new[] {userRedmine.Value.Email, userRedmine.EmailAddress});
                }                                   
            }
        }

        private bool IsOutDate(DateTime issueDate, int days = 0)
        {
            bool isMore = false;

            DateTime now = DateTime.Now;
            TimeSpan dt = (now - issueDate);

            if ((dt.TotalDays > days) & (days != 0))
                isMore = true;            

            return isMore;
        }
        
        private bool IsOutPercent(UserIssue userIssue, out float planPercent)
        {
            bool result = false;
            planPercent = 0;
            float curPercent = (float)userIssue.issue.DoneRatio;
            DateTime curDate = DateTime.Now;
            TimeSpan dt1, dt2;

            if ((userIssue.issue.DueDate != null) && ((curDate < userIssue.issue.DueDate) & (curDate > userIssue.issue.StartDate)))
            {
                dt1 = curDate - (DateTime)userIssue.issue.StartDate;
                dt2 = (DateTime)userIssue.issue.DueDate - (DateTime)userIssue.issue.StartDate;

                if (dt2.TotalSeconds != 0)
                    planPercent = 100 * (float)(dt1.TotalSeconds / dt2.TotalSeconds);
                else
                    planPercent = 0;
            }
            else if (curDate >= userIssue.issue.DueDate) planPercent = 100;
                else planPercent = 0;
            

            if ((planPercent - curPercent) > 10)
            {
                result = true;
            }            

            return result;
        }

        private void SetEmailMessage()
        {
            foreach (var userRedmine in listUserRedmine)
            {
                if (userRedmine.isNeedToSend)
                {
                    if (userRedmine.messageSend == "")
                    {
                        userRedmine.messageSend = "Здравствуйте уважаемый(ая) " + userRedmine.FullName + "." + "\n"; ;
                        userRedmine.messageSend += "У вас есть открытые задания по которым вы являетесь ответственным и по которым есть следующие замечания: " + "\n" +
                            "1. в части задач просрочены сроки выполнения " +
                            " для которых в случае их выполнения необходимо закрыть (поменять статус на - закрыта) или согласовать новую дату завершения;" + "\n" +
                            "2. в части задач необходимо указать актуальные проценты выполнения." + "\n";
                        userRedmine.messageSend += "\n";
                    }

                    foreach (var userIssue in userRedmine.ListUserIssue)
                    {
                        if (userIssue.message != "")
                        {
                            userIssue.message += "ссылка на задание в Redmine -> " + "http://188.242.201.77/issues/" + userIssue.issue.Id.ToString() + "\n";
                            userRedmine.messageSend += userIssue.message + "\n";
                        }
                    }
                }
            }            
        }

        private void SetMessage(UserIssue userIssue, int type, float planPercent = 0)
        {
            if (userIssue.message == "")
            {
                if (userIssue.issue.DueDate != null)
                    userIssue.message += "Проект: " + userIssue.issue.Project.Name + "-> задание № " + userIssue.issue.Id + ": " + userIssue.issue.Subject.Trim() + "\n" + "->" +
                                        " дата начала: " + userIssue.issue.StartDate.Value.ToShortDateString() + " -> дата завершения: " + userIssue.issue.DueDate.Value.ToShortDateString() + "\n";
                else
                    if (userIssue.issue.StartDate != null)
                    userIssue.message += "Проект: " + userIssue.issue.Project.Name + "-> задание № " + userIssue.issue.Id + ": " + userIssue.issue.Subject.Trim() + "\n" + "->" + 
                                        " дата начала: " + userIssue.issue.StartDate.Value.ToShortDateString() + " -> дата завершения: -" + "\n";
            }


            switch (type)
            {
                case 1:
                    userIssue.message += "Дата завершения задачи просрочена на текущую дату." + "\n";
                    break;

                case 2:
                    userIssue.message += "Текущий процент выполнения по задаче не соответствует плановому " + "\n" + "-> плановый % выполнения = " + planPercent.ToString("0") + "%" + " -> текущий % выполнения = " + userIssue.issue.DoneRatio + "%" + "\n";                 
                    break;                
            }

            
        }

        public void SetUserIssueNotification()
        {
            float planPercent;

            foreach (var userRedmine in listUserRedmine)
            {                
                foreach (var userIssue in userRedmine.ListUserIssue)
                {
                    try
                    {
                        if ((userIssue.issue.DueDate == null) || (IsOutDate(userIssue.issue.DueDate.Value, 1)))
                        {
                            userRedmine.isNeedToSend = true;
                            SetMessage(userIssue, 1);
                        }

                        if (IsOutPercent(userIssue, out planPercent))
                        {
                            userRedmine.isNeedToSend = true;
                            SetMessage(userIssue, 2, planPercent);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(DateTime.Now.ToString() + " Error in " + ex);
                        SaveError.SaveMessage(ex.ToString());
                    }
                    
                    //foreach (var customField in userIssue.issue.CustomFields)
                    //{
                    //    Console.WriteLine("customField = {0}", customField.Values[0].Info);
                    //    if (customField.Values[0].Info != "")
                    //    {
                    //        customField.Values[0].Info = "-";
                    //        redmineManager.UpdateObject(userIssue.issue.Id.ToString(), userIssue.issue);
                    //    }
                    //    Console.WriteLine("id = {0}, name = {1}", userIssue.issue.Id.ToString(), userIssue.issue.Author.Name.ToString());
                    //}
                    
                }
            }
        }

        public void ShowNotification()
        {
            foreach (var userRedmine in listUserRedmine)
            {
                Console.WriteLine(userRedmine.messageSend);
            }
        }        
    }
}
