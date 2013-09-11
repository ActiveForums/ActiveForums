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

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums
{

    public partial class af_confirmaction_new : ForumBase
    {


        #region Event Handlers
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            try
            {
                //Put user code to initialize the page here
                if (Request.Params["afmsg"] != null)
                {
                    switch (Request.Params["afmsg"].ToUpper())
                    {
                        case "MOVE":
                            lblMessage.Text = Utilities.GetSharedResource("[RESX:Topic:Moved]", "Messages.ascx");
                            break;
                        case "MODALERT":
                            lblMessage.Text = Utilities.GetSharedResource("[RESX:Topic:Alert]", "Messages.ascx");
                            break;
                        case "PENDINGMOD":
                            lblMessage.Text = Utilities.GetSharedResource("[RESX:Topic:PendingMod]", "Messages.ascx");
                            hypPost.Visible = false;
                            break;
                        case "EMAILSENT":
                            lblMessage.Text = Utilities.GetSharedResource("[RESX:Email:Sent]", "Messages.ascx");
                            break;
                        case "POSTSUBMIT":
                            lblMessage.Text = Utilities.GetSharedResource("[RESX:Topic:Submit]", "Messages.ascx");
                            break;
                    }
                }
                else if (Request.QueryString[ParamKeys.ConfirmActionId] != null)
                {
                    ConfirmActions action = (ConfirmActions)Enum.Parse(typeof(ConfirmActions), Request.QueryString[ParamKeys.ConfirmActionId],true);
                    switch (action)
                    {
                        case ConfirmActions.AlertSent:
                            lblMessage.Text = Utilities.GetSharedResource("[RESX:Topic:Alert]", "Messages.ascx");
                            break;
                        case ConfirmActions.MessageDeleted:
                            lblMessage.Text = Utilities.GetSharedResource("[RESX:Topic:Deleted]", "Messages.ascx");
                            break;
                        case ConfirmActions.MessageMoved:
                            lblMessage.Text = Utilities.GetSharedResource("[RESX:Topic:Moved]", "Messages.ascx");
                            break;
                        case ConfirmActions.MessagePending:
                            lblMessage.Text = Utilities.GetSharedResource("[RESX:Topic:PendingMod]", "Messages.ascx");
                            break;
                        case ConfirmActions.TopicSaved:
                            lblMessage.Text = Utilities.GetSharedResource("[RESX:Topic:Saved]", "Messages.ascx");
                            break;
                        case ConfirmActions.TopicDeleted:
                            lblMessage.Text = Utilities.GetSharedResource("[RESX:Topic:Deleted]", "Messages.ascx");
                            break;
                        case ConfirmActions.ReplyDeleted:
                            lblMessage.Text = Utilities.GetSharedResource("[RESX:Reply:Deleted]", "Messages.ascx");
                            break;
                        case ConfirmActions.ReplySaved:
                            lblMessage.Text = Utilities.GetSharedResource("[RESX:Reply:Saved]", "Messages.ascx");
                            break;
                        case ConfirmActions.SendToComplete:
                            lblMessage.Text = Utilities.GetSharedResource("[RESX:Email:Sent]", "Messages.ascx");
                            break;
                        case ConfirmActions.SendToFailed:
                            lblMessage.Text = Utilities.GetSharedResource("[RESX:Email:Failed]", "Messages.ascx");




                            break;
                    }

                }

                hypForums.NavigateUrl = NavigateUrl(TabId, "", new string[] { ParamKeys.ViewType + "=" + Views.Topics, ParamKeys.ForumId + "=" + ForumId });
                hypPost.NavigateUrl = NavigateUrl(TabId, "", new string[] { ParamKeys.ViewType + "=" + Views.Topic, ParamKeys.ForumId + "=" + ForumId, ParamKeys.TopicId + "=" + TopicId });
                if (TopicId == -1)
                {
                    hypPost.Visible = false;
                }
                else
                {
                    hypPost.Visible = true;
                }
                hypHome.NavigateUrl = NavigateUrl(TabId);
            }
            catch (Exception exc)
            {
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        #endregion

        #region  Web Form Designer Generated Code

        //This call is required by the Web Form Designer.
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
        }
        protected System.Web.UI.WebControls.Panel pnlMessage;

        //NOTE: The following placeholder declaration is required by the Web Form Designer.
        //Do not delete or move it.
        private object designerPlaceholderDeclaration;

        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            this.LocalResourceFile = "~/DesktopModules/ActiveForums/App_LocalResources/sharedresources.resx";
            InitializeComponent();
        }

        #endregion

    }

}
