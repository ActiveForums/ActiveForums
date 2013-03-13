//INSTANT C# TODO TASK: C# compiler constants cannot be set to explicit values:


//#if SKU_ENTERPRISE

using System;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class ActiveForumViewer : ForumBase
    {
        #region Event Handlers
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                if (Settings["AFForumModuleID"] != null)
                {
                    string viewType = Convert.ToString(Settings["AFViewType"]);
                    int tmpModuleId = Convert.ToInt32(Settings["AFForumModuleID"]);
                    int tmpForumId = Convert.ToInt32(Settings["AFForumGroupID"]);
                    if (viewType.ToLowerInvariant() == "topics")
                    {
                        viewType = Views.Topics;
                        ctlForumLoader.ForumId = tmpForumId;
                    }
                    else
                    {
                        viewType = Views.ForumView;
                        ctlForumLoader.ForumGroupId = tmpForumId;
                    }
                    ctlForumLoader.DefaultView = viewType;
                    ctlForumLoader.ForumModuleId = tmpModuleId;
                    ctlForumLoader.ForumTabId = TabId;
                    ctlForumLoader.ModuleConfiguration = this.ModuleConfiguration;
                    ctlForumLoader.InheritModuleCSS = false;
                    if (!(Convert.ToString(Settings["AFTopicsTemplate"]) == null))
                    {
                        ctlForumLoader.DefaultTopicsViewTemplateId = Convert.ToInt32(Settings["AFTopicsTemplate"]);
                    }
                    if (!(Convert.ToString(Settings["AFForumViewTemplate"]) == null))
                    {
                        ctlForumLoader.DefaultForumViewTemplateId = Convert.ToInt32(Settings["AFForumViewTemplate"]);
                    }
                    if (!(Convert.ToString(Settings["AFTopicTemplate"]) == null))
                    {
                        ctlForumLoader.DefaultTopicViewTemplateId = Convert.ToInt32(Settings["AFTopicTemplate"]);
                    }
                    //Dim objModule As ForumBase = CType(LoadControl("~/desktopmodules/ActiveForums/ActiveForums.ascx"), ForumBase)
                    //If Not objModule Is Nothing Then
                    //    objModule.ModuleConfiguration = Me.ModuleConfiguration
                    //    objModule.ID = Path.GetFileNameWithoutExtension("~/desktopmodules/ActiveForums/ActiveForums.ascx")
                    //    objModule.AFModID = CType(Settings["AFForumModuleID"], Integer)
                    //    objModule.LoadGroupForumID = CType(Settings["AFForumGroupID"], Integer)
                    //    objModule.LoadView = CType(Settings["AFViewType"], String)
                    //    plhMod.Controls.Add(objModule)
                    //End If
                    System.Web.UI.HtmlControls.HtmlGenericControl oLink = new System.Web.UI.HtmlControls.HtmlGenericControl("link");
                    oLink.Attributes["rel"] = "stylesheet";
                    oLink.Attributes["type"] = "text/css";
                    oLink.Attributes["href"] = Page.ResolveUrl("~/DesktopModules/ActiveForums/module.css");
                    System.Web.UI.Control oCSS = this.Page.FindControl("CSS");
                    if (oCSS != null)
                    {
                        int iControlIndex = 0;
                        iControlIndex = oCSS.Controls.Count;
                        oCSS.Controls.AddAt(0, oLink);
                    }
                }
                else
                {
                    Label lblMessage = new Label();
                    lblMessage.Text = "Please access the Module Settings page to configure this module.";
                    lblMessage.CssClass = "NormalRed";
                    this.Controls.Add(lblMessage);
                }
            }
            catch (Exception exc)
            {
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        #endregion
    }
}
//#endif
