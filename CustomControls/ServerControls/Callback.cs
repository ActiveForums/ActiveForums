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

using System.IO;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using System.Text.RegularExpressions;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [SupportsEventValidation(), DefaultProperty("Text"), Designer("DotNetNuke.Modules.ActiveForums.Controls.ActiveCallbackDesigner"), ParseChildren(true, ""), ToolboxData("<{0}:Callback runat=server></{0}:Callback>")]
    public class Callback : WebControl
    {

        #region Declarations

        private CallBackContent _content;
        private ClientTemplate _loadingtemplate;
        private bool _debug = false;
        private bool _loaded = false;
        private bool _isCallback = false;
        private CallBackContent _previouscontent;
        private int _refreshInterval = 0;
        private string _parameter = "";
        private string _resourceFile = "";
        public event CallbackEventHandler CallbackEvent;
        public delegate void CallbackEventHandler(object sender, CallBackEventArgs e);
        private string _PostURL = "";
        private string _token = string.Empty;
        private int _validState = 0; //-1 = don't process, 0 = initial load, 1 = process

        #endregion

        #region Properties

        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public bool Debug
        {
            get
            {

                return _debug;
            }

            set
            {
                _debug = value;
            }
        }
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public int RefreshInterval
        {
            get
            {
                return _refreshInterval;
            }

            set
            {
                _refreshInterval = value;
            }
        }
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string Parameter
        {
            get
            {
                return _parameter;
            }

            set
            {
                _parameter = value;
            }
        }
        /// <summary>
        /// Javascript function to fire on Callback Complete
        /// Example without params: OnCallbackComplete="foo"
        /// Example with params: OnCallbackComplete="function(){foo(params);}"
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string OnCallbackComplete
        {
            get
            {
                string s = Convert.ToString(ViewState["OnCallbackComplete"]);
                if (s == null)
                {
                    return "null";
                }
                else
                {
                    return s;
                }
            }

            set
            {
                ViewState["OnCallbackComplete"] = value;
            }
        }
        [Description("Initial content to render."), DefaultValue(null, ""), Browsable(false), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty)]
        public CallBackContent Content
        {
            get
            {
                EnsureChildControls();
                return _content;
            }
            set
            {
                _content = value;
            }
        }
        [Description("Loading Template during Callback"), PersistenceMode(PersistenceMode.InnerProperty)]
        public ClientTemplate LoadingTemplate
        {
            get
            {
                return _loadingtemplate;
            }
            set
            {
                _loadingtemplate = value;
            }
        }
        public override System.Web.UI.ControlCollection Controls
        {
            get
            {
                EnsureChildControls();
                return base.Controls;
            }
        }
        [Description("Whether we are currently in a callback request."), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public bool IsCallback
        {
            get
            {
                if (Context != null && Context.Request != null)
                {
                    string sId = ClientID;
                    if (!(string.IsNullOrEmpty(ForceId)))
                    {
                        sId = ForceId;
                    }
                    if (!(Context.Request.Params[string.Format("amCB_{0}", sId)] == null))
                    {
                        _isCallback = true;
                    }
                    else
                    {
                        _isCallback = false;
                    }
                }
                return _isCallback;
            }
        }
        public string ResourceFile
        {
            get
            {
                return _resourceFile;
            }
            set
            {
                _resourceFile = value;
            }
        }
        public string PostURL
        {
            get
            {
                return _PostURL;
            }
            set
            {
                _PostURL = value;
            }
        }
        private string _forceId = string.Empty;
        public string ForceId
        {
            get
            {
                return _forceId;
            }
            set
            {
                _forceId = value;
            }
        }

        #endregion

        #region Events/Subs

        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            if (Context == null || Page == null)
            {
                return;
            }
            if (Content != null)
            {
                //Try
                this.Controls.Add(Content);
                //Catch ex As Exception

                //End Try

            }
        }
        protected override void Render(HtmlTextWriter output)
        {
            if (Enabled)
            {
                //Dim sID As String = Me.ClientID
                //Dim args() As String = GetArgs()
                //If Not args Is Nothing Then
                //    If args.Length > 0 Then
                //        Me.handleCB(args)
                //        Return
                //    End If
                //End If
                //output = New HtmlTextWriter(output, String.Empty)
                //AddAttributesToRender(output)
                //output.RenderBeginTag(HtmlTextWriterTag.Div)
                //If Not Content Is Nothing Then
                //    Content.RenderControl(output)
                //End If
                //output.RenderEndTag()
                string sID = this.ClientID;
                if (!(string.IsNullOrEmpty(ForceId)))
                {
                    sID = ForceId;
                }
                string[] args = GetArgs();
                if (args != null)
                {
                    if (args.Length > 0)
                    {
                        this.handleCB(args);
                        return;
                    }
                }
                output = new HtmlTextWriter(output, string.Empty);
                if (string.IsNullOrEmpty(ForceId))
                {
                    AddAttributesToRender(output);
                }
                else
                {
                    output.AddAttribute("id", sID);
                }

                // Me.ID = sID

                output.RenderBeginTag(HtmlTextWriterTag.Div);
                if (Content != null)
                {
                    Content.RenderControl(output);
                }
                output.RenderEndTag();
                if (Page != null & Context != null & Context.Request != null & !(Context.Request.Form["amCB_" + sID] == null) & !(Context.Request.Form["amCB_" + sID] == ""))
                {
                    this.OnCallback(new CallBackEventArgs(output, Context.Request.Form["amCB_" + sID]));
                }
                if (HttpContext.Current.Request.Params["amcbdebug"] == "true" || HttpContext.Current.Request.Params["amdebug"] == "true")
                {
                    this.Debug = true;
                }

                StringBuilder str = new StringBuilder();
                str.Append("<script type=\"text/javascript\">");
                if (!string.IsNullOrEmpty(OnCallbackComplete))
                {
                    str.Append("window.objCB.prototype.CBC_" + sID + "=" + OnCallbackComplete + ";");
                }
                if (LoadingTemplate != null)
                {
                    str.Append("window.objCB.prototype.LT_" + sID + "='" + LoadingTemplate.Text.Replace("\\r", "").Replace("'", "\\\\'").Replace("\\n", "").Replace("\n", "").Replace("\r", "") + "';");
                }
                str.AppendFormat("window.{0}=new objCB('{0}');", sID);
                str.Append(sID + ".Debug=" + Debug.ToString().ToLower() + ";");
                string URL = PostURL;
                if (URL == "")
                {
                    //  URL = GetResponseUrl(Context)
                }
                //If URL.Contains("/404.aspx?404;") Then
                //    URL = URL.Replace("/404.aspx?404;", String.Empty)
                //End If
                if (!(string.IsNullOrEmpty(URL)))
                {
                    URL = XSSFilter(URL);
                    str.Append(sID + ".Location='" + URL.Replace("'", "\\\\'").Replace(";", string.Empty) + "';");
                }
                str.Append(sID + ".Parameter='" + Parameter + "';");
                if (RefreshInterval > 0)
                {
                    str.AppendFormat("setInterval('{0}.Callback({0}.Parameter)',{1});", sID, RefreshInterval);
                }
                str.Append("</script>");
                output.Write(str.ToString());
            }


        }
        public string XSSFilter(string sText)
        {
            sText = HttpContext.Current.Server.UrlDecode(sText);
            string pattern = "<script.*/*>|</script>|<[a-zA-Z][^>]*=['\"]+javascript:\\w+.*['\"]+>|<\\w+[^>]*\\son\\w+=.*[ /]*>";
            sText = Regex.Replace(sText, pattern, string.Empty, RegexOptions.IgnoreCase);
            sText = sText.Replace("-->", string.Empty);
            sText = sText.Replace("<!--", string.Empty);
            return sText;
        }
        protected override void CreateChildControls()
        {
            if (Content != null)
            {
                //Try
                Controls.Clear();
                this.Controls.Add(Content);
                //Catch ex As Exception

                //End Try

            }
            //ChildControlsCreated = True
        }
        private void handleCB(string[] args)
        {
            try
            {
                if (_validState == 1)
                {
                    using (StringWriter strWriter = new StringWriter())
                    {
                        using (HtmlTextWriter oWriter = new HtmlTextWriter(strWriter, string.Empty))
                        {
                            CallBackEventArgs oArgs = new CallBackEventArgs(oWriter) { Parameter = args[0], Parameters = args };
                            this.OnCallback(oArgs);
                            oWriter.Close();
                        }
                        Context.Response.Clear();
                        Context.Response.ContentType = "text/xml";
                        Context.Response.Write("<CallbackData><![CDATA[");
                        string sTemp = strWriter.ToString();
                        sTemp = sTemp.Replace("//<![CDATA[", string.Empty);
                        sTemp = sTemp.Replace("//]]>", string.Empty);
                        Context.Response.Write(sTemp);
                    }
                    Context.Response.Write("]]></CallbackData>");
                    //Context.Response.Flush()

                    Context.Response.End();


                    //HttpContext.Current.ApplicationInstance.CompleteRequest()
                }
                else if (_validState == -1)
                {
                    throw new Exception("Incomplete Request");
                }
                //Dim strWriter As New StringWriter()
                //Dim oWriter As New HtmlTextWriter(strWriter, String.Empty)
                //Dim oArgs As New CallBackEventArgs(oWriter)
                //oArgs.Parameter = args(0)
                //oArgs.Parameters = args
                //Me.OnCallback(oArgs)
                //oWriter.Close()
                //Context.Response.Clear()
                //Context.Response.ContentType = "text/xml"
                //Context.Response.Write("<CallbackData><![CDATA[")
                //Dim sTemp As String = strWriter.ToString
                //sTemp = sTemp.Replace("//<![CDATA[", String.Empty)
                //sTemp = sTemp.Replace("//]]>", String.Empty)
                //Context.Response.Write(sTemp)
                //Context.Response.Write("]]></CallbackData>")
                //'Context.Response.Flush()
                //Context.Response.End()
                //'HttpContext.Current.ApplicationInstance.CompleteRequest()
            }
            catch (Exception ex)
            {
                if (!((ex) is System.Threading.ThreadAbortException))
                {
                    Context.Response.Clear();
                    Context.Response.ContentType = "text/xml";
                    Context.Response.Write("<CallbackError><![CDATA[" + ex.ToString() + "]]></CallbackError>");
                    Context.Response.End();
                    //HttpContext.Current.ApplicationInstance.CompleteRequest()
                }
            }
        }
        public static string GetResponseUrl(HttpContext oContext)
        {
            return oContext.Request.Url.Scheme + Uri.SchemeDelimiter + oContext.Request.Url.Host + (Convert.ToString((oContext.Request.Url.IsDefaultPort ? "" : (":" + oContext.Request.Url.Port)))) + oContext.Response.ApplyAppPathModifier(oContext.Request.RawUrl);
        }
        //Private Function GetArgs() As String()
        //    Dim idname As String = "amCB_" & Me.ClientID
        //    Dim args() As String = Nothing
        //    If Not Context Is Nothing Then
        //        args = Context.Request.Params.GetValues(idname)
        //    End If
        //    If Not args Is Nothing Then
        //        For x As Integer = 0 To (args.Length - 1)
        //            args(x) = args(x).Replace("!AM#", "&").Replace("#AM!", "=").Replace("#MA!", "+")
        //        Next
        //    End If
        //    Return args
        //End Function
        private string[] GetArgs()
        {
            string sId = ClientID;
            if (!(string.IsNullOrEmpty(ForceId)))
            {
                sId = ForceId;
            }
            string idname = string.Format("amCB_{0}", sId);
            string[] args = null;
            if (Context != null)
            {
                string cName = "amcit";

                if (HttpContext.Current.Request.Cookies[cName] != null)
                {

                    string token = HttpContext.Current.Request.Cookies[cName].Value;
                    string tokenname = "hidreq";
                    string tokenvalue = string.Empty;
                    if (Context.Request.Params[tokenname] != null)
                    {
                        tokenvalue = Context.Request.Params[tokenname];
                    }
                    args = Context.Request.Params.GetValues(idname);
                    if (args != null)
                    {
                        if (string.IsNullOrEmpty(tokenvalue))
                        {
                            _validState = -1;
                            //Throw New Exception("Error validating request.")
                        }
                        else
                        {
                            tokenvalue = tokenvalue.Replace("!AM#", "&").Replace("#AM!", "=").Replace("#MA!", "+");
                        }
                        if (!(token == tokenvalue))
                        {
                            _validState = -1;
                        }
                        else
                        {
                            _validState = 1;
                            for (int x = 0; x <= (args.Length - 1); x++)
                            {
                                args[x] = args[x].Replace("!AM#", "&").Replace("#AM!", "=").Replace("#MA!", "+");
                            }
                        }




                    }
                }
            }




            return args;
        }
        private void OnCallback(CallBackEventArgs e)
        {
            if (CallbackEvent != null)
                CallbackEvent(this, e);
        }
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            if (!(Page.ClientScript.IsClientScriptIncludeRegistered("AMCallback")))
            {
                bool cbloaded = false;
                if (HttpContext.Current.Items["cbld"] != null)
                {
                    cbloaded = bool.Parse(HttpContext.Current.Items["cbld"].ToString());
                }
                if (!this.IsCallback && !Page.IsPostBack)
                {
                    if (cbloaded == false && HttpContext.Current.Request.Params["hidreq"] == null)
                    {
                        DotNetNuke.Security.PortalSecurity dnn = new DotNetNuke.Security.PortalSecurity();
                        string ticket = dnn.Encrypt("afexp41", HttpContext.Current.Session.SessionID);

                        HttpCookie myCookie = new HttpCookie("amcit");
                        if (HttpContext.Current.Request.Cookies["amcit"] != null)
                        {
                            HttpContext.Current.Response.Cookies["amcit"].Value = "";
                            HttpContext.Current.Response.Cookies["amcit"].Expires = DateTime.Now.AddYears(-1);
                            HttpContext.Current.Response.Cookies["amcit"].Domain = "";
                            HttpContext.Current.Response.Cookies["amcit"].HttpOnly = true;
                        }
                        myCookie.HttpOnly = true;
                        //myCookie.Path = "/"
                        //myCookie.Domain = HttpContext.Current.Request.Url.Host
                        myCookie.Expires = DateTime.Now.AddHours(2);
                        myCookie.Value = ticket;
                        HttpContext.Current.Response.Cookies.Add(myCookie);
                        Page.ClientScript.RegisterHiddenField("amcbid", ticket);
                        HttpContext.Current.Items.Add("cbld", "True");
                    }
                }
                Page.ClientScript.RegisterClientScriptInclude("AMCallback", Page.ClientScript.GetWebResourceUrl(this.GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.cb.js"));
            }
        }
        #endregion


    }
    public class CallBackEventArgs : EventArgs
    {

        public string Parameter;
        public string[] Parameters;

        public HtmlTextWriter Output;
        internal CallBackEventArgs(HtmlTextWriter _writer)
        {
            Output = _writer;
        }
        internal CallBackEventArgs(HtmlTextWriter _writer, string sParam)
        {
            Output = _writer;
            Parameter = sParam;
        }
    }
    [ToolboxItem(false)]
    public class CallBackContent : System.Web.UI.Control
    {
    }
    [ToolboxItem(false)]
    public class ClientTemplate : System.Web.UI.Control
    {
        [DefaultValue(""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Text
        {
            get
            {
                if (this.Controls.Count > 0 && this.Controls[0] is LiteralControl)
                {
                    return ((LiteralControl)(this.Controls[0])).Text;
                }
                return "";
            }
            set
            {
                this.Controls.Clear();
                this.Controls.Add(new LiteralControl(value));
            }
        }
    }
    public class ActiveCallbackDesigner : ControlDesigner
    {

        public override string GetDesignTimeHtml()
        {
            return "<div style=\"font-family:tahoma;font-size:11px;\">Active Forums Callback Control</div>";
        }

    }
    public interface ICallback
    {
        void Callback(object sender, Controls.CallBackEventArgs e);


    }
}
