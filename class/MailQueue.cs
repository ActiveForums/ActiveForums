//
// Active Forums - http://www.dnnsoftware.com
// Copyright (c) 2013
// by DNN Corp.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DotNetNuke.Entities.Host;

namespace DotNetNuke.Modules.ActiveForums.Queue
{
	public class Controller
	{
		public static void Add(string EmailFrom, string EmailTo, string EmailSubject, string EmailBody, string EmailBodyPlainText, string EmailCC, string EmailBCC)
		{
			try
			{
				DataProvider.Instance().Queue_Add(EmailFrom, EmailTo, EmailSubject, EmailBody, EmailBodyPlainText, EmailCC, EmailBCC);
			}
			catch (Exception ex)
			{
				Services.Exceptions.Exceptions.LogException(ex);
			}

		}
	}
	public class Scheduler : Services.Scheduling.SchedulerClient
	{

		private string SMTPServer;
		private string SmtpUserName;
		private string SMTPPassword;
		private string SMTPAuthentication;

		public Scheduler(Services.Scheduling.ScheduleHistoryItem objScheduleHistoryItem) : base()
		{
			ScheduleHistoryItem = objScheduleHistoryItem;
		}
		public override void DoWork()
		{
			try
			{
				int intQueueCount;
				intQueueCount = ProcessQueue();
				ScheduleHistoryItem.Succeeded = true;
				ScheduleHistoryItem.AddLogNote("Processed " + intQueueCount.ToString() + " messages");
			}
			catch (Exception ex)
			{
				ScheduleHistoryItem.Succeeded = false;
				ScheduleHistoryItem.AddLogNote("Process Queue Failed. " + ex);
				Errored(ref ex);
				Services.Exceptions.Exceptions.LogException(ex);
			}
		}
		private int ProcessQueue()
		{
			int intQueueCount = 0;
			try
			{
				//Get Host SMTP Settings
				Hashtable objHost = Entities.Portals.PortalSettings.GetHostSettings();
				//Get Queue

				IDataReader dr = DataProvider.Instance().Queue_List();
				while (dr.Read())
				{
					intQueueCount += 1;
					var objEmail = new Message
					                   {
					                       Subject = dr["EmailSubject"].ToString(),
					                       SendFrom = dr["EmailFrom"].ToString(),
					                       SendTo = dr["EmailTo"].ToString(),
					                       Body = dr["EmailBody"].ToString(),
					                       BodyText = dr["EmailBodyPlainText"].ToString(),
					                       SmtpServer = Host.SMTPServer, // objHost["SMTPServer"].ToString(),
					                       SmtpUserName = Host.SMTPUsername, // objHost["SMTPUsername"].ToString(),
					                       SmtpPassword = Host.SMTPPassword, // objHost["SMTPPassword"].ToString(),
					                       SmtpAuthentication = Host.SMTPAuthentication, // objHost["SMTPAuthentication"].ToString(),
					                       SmtpSSL = Host.EnableSMTPSSL.ToString() // objHost["SMTPEnableSSL"].ToString()
					                   };

				    bool canDelete = objEmail.SendMail();
					if (canDelete)
					{
						try
						{
							DataProvider.Instance().Queue_Delete(Convert.ToInt32(dr["Id"]));
						}
						catch (Exception ex)
						{
							Services.Exceptions.Exceptions.LogException(ex);
						}
					}
					else
					{
						intQueueCount = intQueueCount - 1;
					}
				}
				dr.Close();
				dr.Dispose();

				return intQueueCount;
			}
			catch (Exception ex)
			{
				Services.Exceptions.Exceptions.LogException(ex);
				return -1;
			}

		}

	}
	public class Message
	{
		public string Subject;
		public string SendFrom;
		public string SendTo;
		public string Body;
		public string BodyText;
		//Public Recipients As ArrayList
		public string SmtpServer;
		public string SmtpUserName;
		public string SmtpPassword;
		public string SmtpAuthentication;
		public string SmtpSSL;
		//Public Subs As List(Of SubscriptionInfo)


		public bool SendMail()
		{
			try
			{
				Hashtable objHost;
				if (SmtpServer == "")
				{
					objHost = Entities.Portals.PortalSettings.GetHostSettings();
					SmtpServer = Host.SMTPServer; // Convert.ToString(objHost["SMTPServer"]);
					SmtpUserName = Host.SMTPUsername; // Convert.ToString(objHost["SMTPUsername"]);
					SmtpPassword = Host.SMTPPassword;// Convert.ToString(objHost["SMTPPassword"]);
					SmtpAuthentication = Host.SMTPAuthentication;// Convert.ToString(objHost["SMTPAuthentication"]);
					SmtpSSL = Host.EnableSMTPSSL.ToString();// Convert.ToString(objHost["SMTPEnableSSL"]);
				}
				//Dim Email As New System.Net.Mail.MailMessage

				//Email.From = New System.Net.Mail.MailAddress(SendFrom)
				//Email.To.Add(New System.Net.Mail.MailAddress(SendTo))
				//Email.Subject = Subject
				//Email.IsBodyHtml = True
				//Email.Body = Body
				var subs = new List<SubscriptionInfo>();
				var si = new SubscriptionInfo
				             {Email = SendTo, DisplayName = string.Empty, LastName = string.Empty, FirstName = string.Empty};
			    subs.Add(si);
				var oEmail = new Email
				                 {
				                     UseQueue = false,
				                     Recipients = subs,
				                     Subject = Subject,
				                     From = SendFrom,
				                     BodyText = BodyText,
				                     BodyHTML = Body,
				                     SmtpServer = SmtpServer,
				                     SmtpUserName = SmtpUserName,
				                     SmtpPassword = SmtpPassword,
				                     SmtpAuthentication = SmtpAuthentication,
				                     SmtpSSL = SmtpSSL
				                 };
			    try
				{
					var objThread = new System.Threading.Thread(oEmail.Send);
					objThread.Start();
					return true;
				}
				catch (Exception ex)
				{
					Services.Exceptions.Exceptions.LogException(ex);
					return false;
				}
			}
			catch (Exception ex)
			{
				Services.Exceptions.Exceptions.LogException(ex);
				return false;
			}
		}
	}

}
