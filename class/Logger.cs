//© 2004 - 2005 ActiveModules, Inc. All Rights Reserved
//ORIGINAL LINE: Imports System.Web.HttpContext

using System.Web;

namespace DotNetNuke.Modules.ActiveForums
{
	public abstract class Logger
	{
		public static void Log(string Msg)
		{
			string sPath = HttpContext.Current.Request.MapPath("~/DesktopModules/ActiveForums/am.html");
			Msg = Msg + System.Environment.NewLine;
			System.IO.File.AppendAllText(sPath, Msg);
			//Dim w As StreamWriter = File.AppendText(sPath)
			//w.WriteLine(Now() & vbTab & Msg)
			//w.Flush()
			//w.Close()
			//Dim Email As New MailMessage
			//Email.To = "willm@ntweb.com"
			//Email.From = "willm@ntweb.com"
			//Email.Subject = "Error"
			//Email.Body = Msg
			//SmtpMail.Send(Email)
		}
	}
}

