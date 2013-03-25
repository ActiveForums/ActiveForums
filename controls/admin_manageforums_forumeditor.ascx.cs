using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Text;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class admin_manageforums_forumeditor : ActiveAdminBase
    {
        public string imgOn;
        public string imgOff;
        public string editorType = "G"; //"F"
        public int recordId = 0;
        protected Controls.admin_securitygrid ctlSecurityGrid = new Controls.admin_securitygrid();

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cbEditorAction.CallbackEvent += cbEditorAction_Callback;

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            litTopicPropButton.Text = "<div><a href=\"\" onclick=\"afadmin_LoadPropForm();return false;\" class=\"btnadd afroundall\">[RESX:AddProperty]</a></div>";
            litPropLoad.Text = Utilities.GetFileResource("DotNetNuke.Modules.ActiveForums.scripts.afadmin.properties.js");


            BindRoles();
            string sepChar = "|";
            if (Params != null && !(string.IsNullOrEmpty(Params)))
            {
                if (Params.Contains("!"))
                {
                    sepChar = "!";
                }
                editorType = Params.Split(Convert.ToChar(sepChar))[1]; // Params.Split(CChar(sepChar))(1).Split(CChar("="))(1)
                recordId = Convert.ToInt32(Params.Split(Convert.ToChar(sepChar))[0]);
            }
            if (editorType == "G")
            {
                trGroups.Visible = false;
                reqGroups.Enabled = false;
                trDesc.Visible = false;
                trInherit.Visible = false;
                lblForumGroupName.Text = GetSharedResource("[RESX:GroupName]");
                btnDelete.ClientSideScript = "deleteGroup();";

            }
            else
            {
                lblForumGroupName.Text = GetSharedResource("[RESX:ForumName]");
                trInherit.Visible = true;
                chkInheritGroup.Attributes.Add("onclick", "amaf_toggleInherit();");
                btnDelete.ClientSideScript = "deleteForum();";
            }
            if (recordId == 0)
            {
                btnDelete.Visible = false;
            }
            imgOn = Page.ResolveUrl("~/DesktopModules/ActiveForums/images/admin_check.png");
            imgOff = Page.ResolveUrl("~/DesktopModules/ActiveForums/images/admin_stop.png");
            reqForumName.Text = "<img src=\"" + Page.ResolveUrl(RequiredImage) + "\" />";
            reqGroups.Text = "<img src=\"" + Page.ResolveUrl(RequiredImage) + "\" />";
            string propImage = "<img src=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/images/properties16.png") + "\" alt=\"[RESX:ConfigureProperties]\" />";

            //rdAttachOn.Attributes.Add("onclick", "toggleAttach(this);");
            //rdAttachOn.Attributes.Add("value", "1");
            //rdAttachOff.Attributes.Add("onclick", "toggleAttach(this);");
            //rdAttachOff.Attributes.Add("value", "0");

            rdHTMLOn.Attributes.Add("onclick", "toggleEditor(this);");
            rdHTMLOff.Attributes.Add("onclick", "toggleEditor(this);");
            rdHTMLOn.Attributes.Add("value", "1");
            rdHTMLOff.Attributes.Add("value", "0");
            cfgHTML.Attributes.Add("style", "display:none;");
            cfgHTML.InnerHtml = propImage;
            rdModOn.Attributes.Add("onclick", "toggleMod(this);");
            rdModOff.Attributes.Add("onclick", "toggleMod(this);");
            rdModOn.Attributes.Add("value", "1");
            rdModOff.Attributes.Add("value", "0");
            cfgMod.Attributes.Add("style", "display:none;");
            cfgMod.InnerHtml = propImage;



            trAutoSub.Visible = true;
            rdAutoSubOn.Attributes.Add("onclick", "toggleAutoSub(this);");
            rdAutoSubOff.Attributes.Add("onclick", "toggleAutoSub(this);");
            rdAutoSubOn.Attributes.Add("value", "1");
            rdAutoSubOff.Attributes.Add("value", "0");
            cfgAutoSub.Attributes.Add("style", "display:none;");
            cfgAutoSub.InnerHtml = propImage;
            txtOlderThan.Attributes.Add("onkeypress", "return onlyNumbers(event)");
            txtReplyOlderThan.Attributes.Add("onkeypress", "return onlyNumbers(event)");
            txtUserId.Attributes.Add("onkeypress", "return onlyNumbers(event)");
            if (MainSettings.DeleteBehavior == 1)
            {
                lblMaintWarn.Text = string.Format(GetSharedResource("[RESX:MaintenanceWarning]"), GetSharedResource("[RESX:MaintenanceWarning:Recycle]"), GetSharedResource("[RESX:MaintenanceWarning:Recycle:Desc]"));
            }
            else
            {
                lblMaintWarn.Text = string.Format(GetSharedResource("[RESX:MaintenanceWarning]"), GetSharedResource("[RESX:MaintenanceWarning:Remove]"), GetSharedResource("[RESX:MaintenanceWarning:Remove:Desc]"));
            }



            drpEditorTypes.Attributes.Add("onchange", "toggleEditorFields();");
            if (!cbEditorAction.IsCallback)
            {
                BindGroups();
                BindTemplates();
                if (recordId > 0)
                {
                    if (editorType == "F")
                    {
                        LoadForum(recordId);
                    }
                    else if (editorType == "G")
                    {
                        LoadGroup(recordId);
                    }
                    cfgHTML.Attributes.Add("onclick", "showProp(this,'edProp')");

                    cfgMod.Attributes.Add("onclick", "showProp(this,'modProp')");

                    cfgAutoSub.Attributes.Add("onclick", "showProp(this,'subProp')");



                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("<script type=\"text/javascript\">");
                    sb.Append("function toggleAutoSub(obj){");
                    sb.Append("    closeAllProp();");
                    sb.Append("    var mod = document.getElementById('" + cfgAutoSub.ClientID + "');");
                    sb.Append("    if (obj.value == '1'){");
                    sb.Append("        mod.style.display = '';");
                    sb.Append("    }else{");
                    sb.Append("        mod.style.display = 'none';");
                    sb.Append("        var winDiv = document.getElementById('subProp');");
                    sb.Append("        mod.style.display = 'none';");
                    sb.Append("    };");
                    sb.Append("};");

                    sb.Append("</script>");
                    litScripts.Text = sb.ToString();


                }
                BindTabs();
            }



        }
        private void cbEditorAction_Callback(object sender, Modules.ActiveForums.Controls.CallBackEventArgs e)
        {

            switch (e.Parameters[0].ToLowerInvariant())
            {
                case "forumsave":
                    {
                        Forum fi = new Forum();
                        ForumController fc = new ForumController();
                        bool bIsNew = false;
                        int forumId = 0;
                        int ForumGroupId = 0;
                        string ForumSettingsKey = string.Empty;

                        if (Convert.ToInt32(e.Parameters[1]) <= 0)
                        {
                            bIsNew = true;
                            fi.ForumID = -1;
                        }
                        else
                        {
                            fi = new Forum();
                            fi = fc.Forums_Get(PortalId, ModuleId, Convert.ToInt32(e.Parameters[1]), this.UserId, false, false, -1);
                            ForumSettingsKey = fi.ForumSettingsKey;
                        }
                        //.ForumID = CInt(e.Parameters(1))
                        //If .ForumID = 0 Then
                        //    bIsNew = True
                        //End If
                        fi.ModuleId = ModuleId;
                        fi.PortalId = PortalId;
                        string sParentValue = "";
                        sParentValue = e.Parameters[2].ToString();
                        int ParentForumId = 0;

                        if ((sParentValue.IndexOf("GROUP", 0) + 1) > 0)
                        {
                            sParentValue = sParentValue.Replace("GROUP", "");
                            ForumGroupId = Convert.ToInt32(sParentValue);
                        }
                        else
                        {
                            ParentForumId = Convert.ToInt32(sParentValue.Replace("FORUM", ""));
                            ForumGroupId = fc.Forums_GetGroupId(ParentForumId);
                        }
                        fi.ForumGroupId = ForumGroupId;
                        fi.ParentForumId = ParentForumId;
                        fi.ForumName = e.Parameters[3].ToString();
                        fi.ForumDesc = e.Parameters[4].ToString();
                        fi.Active = Convert.ToBoolean(e.Parameters[5]);
                        fi.Hidden = Convert.ToBoolean(e.Parameters[6]);
                        if (e.Parameters[7].ToString() == "")
                        {
                            fi.SortOrder = 0;
                        }
                        else
                        {
                            fi.SortOrder = Convert.ToInt32(e.Parameters[7]);
                        }
                        string fkey = string.Empty;
                        if (!(string.IsNullOrEmpty(fi.ForumSettingsKey)))
                        {
                            fkey = fi.ForumSettingsKey;
                        }
                        if (Convert.ToBoolean(e.Parameters[8]))
                        {
                            ForumGroupController fgc = new ForumGroupController();
                            ForumGroupInfo fgi = fgc.GetForumGroup(ModuleId, ForumGroupId);
                            if (bIsNew)
                            {
                                fi.PermissionsId = fgi.PermissionsId;

                            }
                            else
                            {
                                if (fi.ForumSettingsKey != "G) :" + ForumGroupId.ToString())
                                {
                                    fi.PermissionsId = fgi.PermissionsId;
                                }
                            }
                            fi.ForumSettingsKey = "G:" + ForumGroupId.ToString();

                        }
                        else if (bIsNew || fkey.Contains("G:"))
                        {
                            fi.ForumSettingsKey = "";
                            if (fi.InheritSecurity)
                            {
                                fi.PermissionsId = -1;
                            }
                        }
                        else
                        {
                            fi.ForumSettingsKey = "F:" + fi.ForumID.ToString();
                        }
                        if (ForumSettingsKey != fkey && fkey.Contains("F:"))
                        {
                            bIsNew = true;
                        }
                        fi.PrefixURL = e.Parameters[9];
                        if (!(string.IsNullOrEmpty(fi.PrefixURL)))
                        {
                            Data.Common db = new Data.Common();
                            if (!(db.CheckForumURL(PortalId, ModuleId, fi.PrefixURL, fi.ForumID, fi.ForumGroupId)))
                            {
                                fi.PrefixURL = string.Empty;
                            }
                        }

                        forumId = fc.Forums_Save(PortalId, fi, bIsNew, Convert.ToBoolean(e.Parameters[8]));
                        recordId = forumId;
                        string securityKey = string.Empty;
                        DataCache.ClearForumGroupsCache(ModuleId);
                        string cachekey = string.Format("AF-FI-{0}-{1}-{2}", PortalId, ModuleId, forumId);
                        DataCache.CacheClear(cachekey);
                         cachekey = string.Format("AF-FV-{0}-{1}", PortalId, ModuleId);
                        DataCache.CacheClearPrefix(cachekey);
                        hidEditorResult.Value = forumId.ToString();
                        break;
                    }
                case "groupsave":
                    {
                        bool bIsNew = false;
                        ForumGroupInfo gi = new ForumGroupInfo();
                        ForumGroupController fgc = new ForumGroupController();
                        int groupId = Convert.ToInt32(e.Parameters[1]);
                        if (groupId > 0)
                        {
                            gi = fgc.Groups_Get(ModuleId, groupId);
                        }
                        string securityKey = string.Empty;
                        if (groupId == 0)
                        {
                            bIsNew = true;
                        }
                        else
                        {
                            securityKey = "G:" + groupId;
                        }
                        gi.ModuleId = ModuleId;
                        gi.ForumGroupId = groupId;
                        gi.GroupName = e.Parameters[3].ToString();
                        gi.Active = Convert.ToBoolean(e.Parameters[5]);
                        gi.Hidden = Convert.ToBoolean(e.Parameters[6]);
                        if (e.Parameters[7].ToString() == "")
                        {
                            gi.SortOrder = 0;
                        }
                        else
                        {
                            gi.SortOrder = Convert.ToInt32(e.Parameters[7]);
                        }
                        gi.PrefixURL = e.Parameters[9];
                        if (!(string.IsNullOrEmpty(gi.PrefixURL)))
                        {
                            Data.Common db = new Data.Common();
                            if (!(db.CheckGroupURL(PortalId, ModuleId, gi.PrefixURL, gi.ForumGroupId)))
                            {
                                gi.PrefixURL = string.Empty;
                            }
                        }
                        gi.GroupSettingsKey = securityKey;
                        ForumGroupController gc = new ForumGroupController();
                        groupId = gc.Groups_Save(PortalId, gi, bIsNew);
                        recordId = groupId;

                        DataCache.ClearForumGroupsCache(ModuleId);
                        string cachekey = string.Format("AF-FV-{0}-{1}", PortalId, ModuleId);
                        DataCache.CacheClearPrefix(cachekey);
                        hidEditorResult.Value = groupId.ToString();


                        break;
                    }
                case "forumsettingssave":
                    {
                        int ForumId = Convert.ToInt32(e.Parameters[1]);
                        string sKey = "F:" + ForumId.ToString();
                        SaveSettings(sKey, e.Parameters);

                        hidEditorResult.Value = ForumId.ToString();
                        DataCache.CacheClear(ForumId.ToString() + "ForumSettings");
                        DataCache.CacheClear(string.Format(CacheKeys.ForumInfo, ForumId));
                        DataCache.CacheClear(string.Format(CacheKeys.ForumInfo, ForumId) + "st");
                        string cachekey = string.Format("AF-FI-{0}-{1}-{2}", PortalId, ModuleId, ForumId);
                        DataCache.CacheClear(cachekey);
                        break;
                    }
                case "groupsettingssave":
                    {
                        int ForumId = Convert.ToInt32(e.Parameters[1]);
                        string sKey = "G:" + ForumId.ToString();
                        SaveSettings(sKey, e.Parameters);

                        hidEditorResult.Value = ForumId.ToString();
                        DataCache.CacheClear(ForumId.ToString() + "GroupSettings");
                        DataCache.CacheClear(string.Format(CacheKeys.GroupInfo, ForumId));
                        DataCache.CacheClear(string.Format(CacheKeys.GroupInfo, ForumId) + "st");

                        break;
                    }
                case "deleteforum":
                    {
                        int ForumId = Convert.ToInt32(e.Parameters[1]);
                        DataProvider.Instance().Forums_Delete(PortalId, ModuleId, ForumId);
                        string cachekey = string.Format("AF-FV-{0}-{1}", PortalId, ModuleId);
                        DataCache.CacheClearPrefix(cachekey);
                        break;
                    }
                case "deletegroup":
                    {
                        int GroupId = Convert.ToInt32(e.Parameters[1]);
                        DataProvider.Instance().Groups_Delete(ModuleId, GroupId);
                        string cachekey = string.Format("AF-FV-{0}-{1}", PortalId, ModuleId);
                        DataCache.CacheClearPrefix(cachekey);
                        break;
                    }
            }
            DataCache.CacheClear(string.Format(CacheKeys.ForumList, ModuleId));
            DataCache.ClearAllForumSettingsCache(ModuleId);
            DataCache.CacheClear(ModuleId + "fv");

            hidEditorResult.RenderControl(e.Output);
        }


        #endregion

        #region Private Methods
        private void SaveSettings(string sKey, string[] Parameters)
        {
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.TopicsTemplateId, Parameters[2].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.TopicTemplateId, Parameters[3].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EmailAddress, Parameters[4].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.UseFilter, Parameters[5].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AllowPostIcon, Parameters[6].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AllowEmoticons, Parameters[7].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AllowScript, Parameters[8].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.IndexContent, Parameters[9].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AllowRSS, Parameters[10].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AllowAttach, Parameters[11].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachCount, Parameters[12].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachMaxSize, Parameters[13].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachTypeAllowed, Parameters[14].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachStore, Parameters[15].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachMaxHeight, Parameters[16].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachMaxWidth, Parameters[17].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachUniqueFileNames, Parameters[18].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AllowHTML, Parameters[19].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EditorType, Convert.ToString(((Convert.ToBoolean(Parameters[19]) == false) ? "0" : Parameters[20])));
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EditorHeight, Parameters[21].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EditorWidth, Parameters[22].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EditorToolbar, Parameters[23].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EditorStyle, Parameters[24].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EditorPermittedUsers, Parameters[25].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.TopicFormId, Parameters[26].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ReplyFormId, Parameters[27].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.QuickReplyFormId, Parameters[28].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ProfileTemplateId, Parameters[29].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.IsModerated, Parameters[30].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.DefaultTrustValue, Parameters[31].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AutoTrustLevel, Parameters[32].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ModApproveTemplateId, Parameters[33].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ModRejectTemplateId, Parameters[34].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ModMoveTemplateId, Parameters[35].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ModDeleteTemplateId, Parameters[36].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ModNotifyTemplateId, Parameters[37].ToString());


            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AutoSubscribeEnabled, Parameters[38].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AutoSubscribeRoles, Parameters[39].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AutoSubscribeNewTopicsOnly, Parameters[57].ToString());


            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ActiveSocialEnabled, Parameters[58].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ActiveSocialTopicsOnly, Parameters[59].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ActiveSocialSecurityOption, Parameters[60].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.CreatePostCount, Parameters[61].ToString());
            DotNetNuke.Modules.ActiveForums.Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ReplyPostCount, Parameters[62].ToString());
        }
        private void LoadForum(int ForumId)
        {
            ForumController fc = new ForumController();
            Forum fi = fc.Forums_Get(ForumId, -1, false, false);
            Forum NewForum = fc.GetForum(PortalId, ModuleId, ForumId, true);
            ctlSecurityGrid = (Controls.admin_securitygrid)(LoadControl("~/DesktopModules/activeforums/controls/admin_securitygrid.ascx"));
            ctlSecurityGrid.Perms = NewForum.Security;
            ctlSecurityGrid.PermissionsId = NewForum.PermissionsId;
            ctlSecurityGrid.ModuleConfiguration = this.ModuleConfiguration;
            ctlSecurityGrid.ReadOnly = NewForum.InheritSecurity;

            plhGrid.Controls.Clear();
            plhGrid.Controls.Add(ctlSecurityGrid);
            if (fi != null)
            {
                txtForumName.Text = fi.ForumName;
                txtForumDesc.Text = fi.ForumDesc;
                chkActive.Checked = fi.Active;
                chkHidden.Checked = fi.Hidden;
                hidForumId.Value = fi.ForumID.ToString();
                txtPrefixURL.Text = fi.PrefixURL;
                int iIndex = 0;
                if (fi.ParentForumId > 0)
                {
                    iIndex = drpGroups.Items.IndexOf(drpGroups.Items.FindByValue("FORUM" + Convert.ToString(fi.ParentForumId)));
                }
                else
                {
                    iIndex = drpGroups.Items.IndexOf(drpGroups.Items.FindByValue("GROUP" + Convert.ToString(fi.ForumGroupId)));
                }
                drpGroups.SelectedIndex = iIndex;
                if (fi.ForumSettingsKey == "G:" + fi.ForumGroupId.ToString())
                {
                    chkInheritGroup.Checked = true;
                    trTemplates.Attributes.Add("style", "display:none;");
                }
                drpTopicsTemplate.SelectedIndex = drpTopicsTemplate.Items.IndexOf(drpTopicsTemplate.Items.FindByValue(fi.TopicsTemplateId.ToString()));
                drpTopicTemplate.SelectedIndex = drpTopicTemplate.Items.IndexOf(drpTopicTemplate.Items.FindByValue(fi.TopicTemplateId.ToString()));
                drpTopicForm.SelectedIndex = drpTopicForm.Items.IndexOf(drpTopicForm.Items.FindByValue(fi.TopicFormId.ToString()));
                drpReplyForm.SelectedIndex = drpReplyForm.Items.IndexOf(drpReplyForm.Items.FindByValue(fi.ReplyFormId.ToString()));
                //drpQuickReplyForm.SelectedIndex = drpQuickReplyForm.Items.IndexOf(drpQuickReplyForm.Items.FindByValue(fi.QuickReplyFormId.ToString))
                drpProfileDisplay.SelectedIndex = drpProfileDisplay.Items.IndexOf(drpProfileDisplay.Items.FindByValue(fi.ProfileTemplateId.ToString()));
                drpModApprovedTemplateId.SelectedIndex = drpModApprovedTemplateId.Items.IndexOf(drpModApprovedTemplateId.Items.FindByValue(fi.ModApproveTemplateId.ToString()));
                drpModRejectTemplateId.SelectedIndex = drpModRejectTemplateId.Items.IndexOf(drpModRejectTemplateId.Items.FindByValue(fi.ModRejectTemplateId.ToString()));
                drpModDeleteTemplateId.SelectedIndex = drpModDeleteTemplateId.Items.IndexOf(drpModDeleteTemplateId.Items.FindByValue(fi.ModDeleteTemplateId.ToString()));
                drpModMoveTemplateId.SelectedIndex = drpModMoveTemplateId.Items.IndexOf(drpModMoveTemplateId.Items.FindByValue(fi.ModMoveTemplateId.ToString()));
                drpModNotifyTemplateId.SelectedIndex = drpModNotifyTemplateId.Items.IndexOf(drpModNotifyTemplateId.Items.FindByValue(fi.ModNotifyTemplateId.ToString()));
                drpDefaultTrust.SelectedIndex = drpDefaultTrust.Items.IndexOf(drpDefaultTrust.Items.FindByValue(Convert.ToInt32(fi.DefaultTrustValue).ToString()));
                txtAutoTrustLevel.Text = fi.AutoTrustLevel.ToString();
                txtEmailAddress.Text = fi.EmailAddress;
                txtCreatePostCount.Text = fi.CreatePostCount.ToString();
                txtReplyPostCount.Text = fi.ReplyPostCount.ToString();
                if (fi.UseFilter)
                {
                    rdFilterOn.Checked = true;
                    rdFilterOff.Checked = false;
                }
                else
                {
                    rdFilterOn.Checked = false;
                    rdFilterOff.Checked = true;
                }
                if (fi.AllowPostIcon)
                {
                    rdPostIconOn.Checked = true;
                    rdPostIconOff.Checked = false;
                }
                else
                {
                    rdPostIconOn.Checked = false;
                    rdPostIconOff.Checked = true;
                }
                if (fi.AllowEmoticons)
                {
                    rdEmotOn.Checked = true;
                    rdEmotOff.Checked = false;
                }
                else
                {
                    rdEmotOn.Checked = false;
                    rdEmotOff.Checked = true;
                }
                if (fi.AllowScript)
                {
                    rdScriptsOn.Checked = true;
                    rdScriptsOff.Checked = false;
                }
                else
                {
                    rdScriptsOn.Checked = false;
                    rdScriptsOff.Checked = true;
                }




                if (fi.IndexContent)
                {
                    rdIndexOn.Checked = true;
                    rdIndexOff.Checked = false;
                }
                else
                {
                    rdIndexOn.Checked = false;
                    rdIndexOff.Checked = true;
                }
                if (fi.AllowRSS)
                {
                    rdRSSOn.Checked = true;
                    rdRSSOff.Checked = false;
                }
                else
                {
                    rdRSSOn.Checked = false;
                    rdRSSOff.Checked = true;
                }
                if (fi.AllowAttach)
                {
                    rdAttachOn.Checked = true;
                    rdAttachOff.Checked = false;
                }
                else
                {
                    rdAttachOn.Checked = false;
                    rdAttachOff.Checked = true;
                }
                if (fi.AllowHTML)
                {
                    rdHTMLOn.Checked = true;
                    rdHTMLOff.Checked = false;
                    cfgHTML.Attributes.Remove("style");
                }
                else
                {
                    rdHTMLOn.Checked = false;
                    rdHTMLOff.Checked = true;
                    cfgHTML.Attributes.Add("style", "display:none;");
                }
                if (fi.IsModerated)
                {
                    rdModOn.Checked = true;
                    rdModOff.Checked = false;
                    cfgMod.Attributes.Remove("style");
                }
                else
                {
                    rdModOn.Checked = false;
                    rdModOff.Checked = true;
                    cfgMod.Attributes.Add("style", "display:none;");
                }

                if (fi.AutoSubscribeEnabled)
                {
                    rdAutoSubOn.Checked = true;
                    rdAutoSubOff.Checked = false;
                    cfgAutoSub.Attributes.Remove("style");
                }
                else
                {
                    rdAutoSubOn.Checked = false;
                    rdAutoSubOff.Checked = true;
                    cfgAutoSub.Attributes.Add("style", "display:none;");
                }
                chkTopicsOnly.Checked = fi.AutoSubscribeNewTopicsOnly;

                drpEditorTypes.SelectedIndex = drpEditorTypes.Items.IndexOf(drpEditorTypes.Items.FindByValue(Convert.ToInt32(fi.EditorType).ToString()));
                if (Convert.ToInt32(fi.EditorType).ToString() != "1")
                {
                    txtEditorToolBar.Enabled = false;
                    drpEditorStyle.Enabled = false;
                }


                drpEditorStyle.SelectedIndex = drpEditorStyle.Items.IndexOf(drpEditorStyle.Items.FindByValue(Convert.ToString(fi.EditorStyle)));
                drpPermittedRoles.SelectedIndex = drpPermittedRoles.Items.IndexOf(drpPermittedRoles.Items.FindByValue(Convert.ToInt32(fi.EditorPermittedUsers).ToString()));


                txtEditorHeight.Text = Convert.ToString(((fi.EditorHeight == string.Empty) ? "400" : fi.EditorHeight));
                txtEditorWidth.Text = Convert.ToString(((fi.EditorWidth == string.Empty) ? "99%" : fi.EditorWidth));
                txtEditorToolBar.Text = Convert.ToString(((fi.EditorToolBar == string.Empty) ? "bold,italic,underline|bulletedlist,numberedlist" : fi.EditorToolBar));

                hidRoles.Value = fi.AutoSubscribeRoles;
                BindAutoSubRoles(fi.AutoSubscribeRoles);



            }
        }
        private void LoadGroup(int GroupId)
        {
            ForumGroupController gc = new ForumGroupController();
            ForumGroupInfo gi = gc.Groups_Get(ModuleId, GroupId);
            ForumGroupInfo newGroup = gc.GetForumGroup(ModuleId, GroupId);
            ctlSecurityGrid = (Controls.admin_securitygrid)(LoadControl("~/DesktopModules/activeforums/controls/admin_securitygrid.ascx"));
            ctlSecurityGrid.Perms = newGroup.Security;
            ctlSecurityGrid.PermissionsId = newGroup.PermissionsId;
            ctlSecurityGrid.ModuleConfiguration = this.ModuleConfiguration;
            plhGrid.Controls.Clear();
            plhGrid.Controls.Add(ctlSecurityGrid);
            if (gi != null)
            {
                trGroups.Visible = false;
                reqGroups.Enabled = false;
                txtForumName.Text = gi.GroupName;
                chkActive.Checked = gi.Active;
                chkHidden.Checked = gi.Hidden;
                hidForumId.Value = gi.ForumGroupId.ToString();
                hidSortOrder.Value = gi.SortOrder.ToString();
                txtPrefixURL.Text = gi.PrefixURL;

                drpTopicsTemplate.SelectedIndex = drpTopicsTemplate.Items.IndexOf(drpTopicsTemplate.Items.FindByValue(gi.TopicsTemplateId.ToString()));
                drpTopicTemplate.SelectedIndex = drpTopicTemplate.Items.IndexOf(drpTopicTemplate.Items.FindByValue(gi.TopicTemplateId.ToString()));
                drpTopicForm.SelectedIndex = drpTopicForm.Items.IndexOf(drpTopicForm.Items.FindByValue(gi.TopicFormId.ToString()));
                drpReplyForm.SelectedIndex = drpReplyForm.Items.IndexOf(drpReplyForm.Items.FindByValue(gi.ReplyFormId.ToString()));
                //drpQuickReplyForm.SelectedIndex = drpQuickReplyForm.Items.IndexOf(drpQuickReplyForm.Items.FindByValue(gi.QuickReplyFormId.ToString))
                drpProfileDisplay.SelectedIndex = drpProfileDisplay.Items.IndexOf(drpProfileDisplay.Items.FindByValue(gi.ProfileTemplateId.ToString()));
                drpModApprovedTemplateId.SelectedIndex = drpModApprovedTemplateId.Items.IndexOf(drpModApprovedTemplateId.Items.FindByValue(gi.ModApproveTemplateId.ToString()));
                drpModRejectTemplateId.SelectedIndex = drpModRejectTemplateId.Items.IndexOf(drpModRejectTemplateId.Items.FindByValue(gi.ModRejectTemplateId.ToString()));
                drpModDeleteTemplateId.SelectedIndex = drpModDeleteTemplateId.Items.IndexOf(drpModDeleteTemplateId.Items.FindByValue(gi.ModDeleteTemplateId.ToString()));
                drpModMoveTemplateId.SelectedIndex = drpModMoveTemplateId.Items.IndexOf(drpModMoveTemplateId.Items.FindByValue(gi.ModMoveTemplateId.ToString()));
                drpModNotifyTemplateId.SelectedIndex = drpModNotifyTemplateId.Items.IndexOf(drpModNotifyTemplateId.Items.FindByValue(gi.ModNotifyTemplateId.ToString()));
                drpDefaultTrust.SelectedIndex = drpDefaultTrust.Items.IndexOf(drpDefaultTrust.Items.FindByValue(gi.DefaultTrustValue.ToString()));
                txtAutoTrustLevel.Text = gi.AutoTrustLevel.ToString();
                txtEmailAddress.Text = gi.EmailAddress;
                txtCreatePostCount.Text = gi.CreatePostCount.ToString();
                txtReplyPostCount.Text = gi.ReplyPostCount.ToString();
                if (gi.UseFilter)
                {
                    rdFilterOn.Checked = true;
                    rdFilterOff.Checked = false;
                }
                else
                {
                    rdFilterOn.Checked = false;
                    rdFilterOff.Checked = true;
                }
                if (gi.AllowPostIcon)
                {
                    rdPostIconOn.Checked = true;
                    rdPostIconOff.Checked = false;
                }
                else
                {
                    rdPostIconOn.Checked = false;
                    rdPostIconOff.Checked = true;
                }
                if (gi.AllowEmoticons)
                {
                    rdEmotOn.Checked = true;
                    rdEmotOff.Checked = false;
                }
                else
                {
                    rdEmotOn.Checked = false;
                    rdEmotOff.Checked = true;
                }
                if (gi.AllowScript)
                {
                    rdScriptsOn.Checked = true;
                    rdScriptsOff.Checked = false;
                }
                else
                {
                    rdScriptsOn.Checked = false;
                    rdScriptsOff.Checked = true;
                }
                if (gi.IndexContent)
                {
                    rdIndexOn.Checked = true;
                    rdIndexOff.Checked = false;
                }
                else
                {
                    rdIndexOn.Checked = false;
                    rdIndexOff.Checked = true;
                }
                if (gi.AllowRSS)
                {
                    rdRSSOn.Checked = true;
                    rdRSSOff.Checked = false;
                }
                else
                {
                    rdRSSOn.Checked = false;
                    rdRSSOff.Checked = true;
                }

                if (gi.AllowAttach)
                {
                    rdAttachOn.Checked = true;
                    rdAttachOff.Checked = false;
                }
                else
                {
                    rdAttachOn.Checked = false;
                    rdAttachOff.Checked = true;
                }
                if (gi.AllowHTML)
                {
                    rdHTMLOn.Checked = true;
                    rdHTMLOff.Checked = false;
                    cfgHTML.Attributes.Remove("style");
                }
                else
                {
                    rdHTMLOn.Checked = false;
                    rdHTMLOff.Checked = true;
                    cfgHTML.Attributes.Add("style", "display:none;");
                }
                if (gi.IsModerated)
                {
                    rdModOn.Checked = true;
                    rdModOff.Checked = false;
                    cfgMod.Attributes.Remove("style");
                }
                else
                {
                    rdModOn.Checked = false;
                    rdModOff.Checked = true;
                    cfgMod.Attributes.Add("style", "display:none;");
                }
                if (gi.AutoSubscribeEnabled)
                {
                    rdAutoSubOn.Checked = true;
                    rdAutoSubOff.Checked = false;
                    cfgAutoSub.Attributes.Remove("style");
                }
                else
                {
                    rdAutoSubOn.Checked = false;
                    rdAutoSubOff.Checked = true;
                    cfgAutoSub.Attributes.Add("style", "display:none;");
                }
                drpEditorTypes.SelectedIndex = drpEditorTypes.Items.IndexOf(drpEditorTypes.Items.FindByValue(Convert.ToInt32(gi.EditorType).ToString()));
                drpEditorStyle.SelectedIndex = drpEditorStyle.Items.IndexOf(drpEditorStyle.Items.FindByValue(Convert.ToString(gi.EditorStyle)));
                drpPermittedRoles.SelectedIndex = drpPermittedRoles.Items.IndexOf(drpPermittedRoles.Items.FindByValue(Convert.ToInt32(gi.EditorPermittedUsers).ToString()));


                txtEditorHeight.Text = Convert.ToString(((gi.EditorHeight == string.Empty) ? "400" : gi.EditorHeight));
                txtEditorWidth.Text = Convert.ToString(((gi.EditorWidth == string.Empty) ? "99%" : gi.EditorWidth));
                txtEditorToolBar.Text = Convert.ToString(((gi.EditorToolBar == string.Empty) ? "bold,italic,underline|bulletedlist,numberedlist" : gi.EditorToolBar));
                chkTopicsOnly.Checked = gi.AutoSubscribeNewTopicsOnly;
                hidRoles.Value = gi.AutoSubscribeRoles;
                BindAutoSubRoles(gi.AutoSubscribeRoles);
            }
        }
        private void BindAutoSubRoles(string Roles)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<table id=\"tblRoles\" cellpadding=\"0\" cellspacing=\"0\" width=\"99%\">");
            sb.Append("<tr><td width=\"99%\"></td><td></td></tr>");
            if (Roles != null)
            {
                DotNetNuke.Security.Roles.RoleController rc = new DotNetNuke.Security.Roles.RoleController();
                ArrayList arr = rc.GetPortalRoles(PortalId);
                foreach (DotNetNuke.Security.Roles.RoleInfo ri in arr)
                {
                    string[] sRoles = Roles.Split(';');
                    foreach (string role in sRoles)
                    {
                        if (role == ri.RoleID.ToString())
                        {
                            sb.Append("<tr><td class=\"amcpnormal\">" + ri.RoleName + "</td><td><img src=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/images/delete16.png") + "\" onclick=\"removeRole(this," + ri.RoleID + ");\" /></td></tr>");
                        }
                    }
                }
            }
            sb.Append("</table>");
            tbRoles.Text = sb.ToString();
        }
        private void BindGroups()
        {
            IDataReader dr = DataProvider.Instance().Forums_List(PortalId, ModuleId, -1, -1, false);
            drpGroups.Items.Add(new ListItem(Utilities.GetSharedResource("DropDownSelect"), "-1"));
            int tmpGroupId = -1;
            while (dr.Read())
            {
                if (!(tmpGroupId == Convert.ToInt32(dr["ForumGroupId"])))
                {
                    drpGroups.Items.Add(new ListItem(dr["GroupName"].ToString(), "GROUP" + dr["ForumGroupId"].ToString()));
                    tmpGroupId = Convert.ToInt32(dr["ForumGroupId"]);
                }
                if (!(Convert.ToInt32(dr["ForumId"]) == 0))
                {
                    if (Convert.ToInt32(dr["ParentForumID"]) == 0)
                    {
                        drpGroups.Items.Add(new ListItem(" - " + dr["ForumName"].ToString(), "FORUM" + dr["ForumId"].ToString()));
                    }
                }
            }
            dr.Close();
        }
        private void BindTabs()
        {
            StringBuilder sb = new StringBuilder();
            string sLabel = "[RESX:Group]";
            if (editorType == "F")
            {
                sLabel = "[RESX:Forum]";
            }
            sb.Append("<div id=\"divForum\" onclick=\"toggleTab(this);\" class=\"amtabsel\" style=\"margin-left:10px;\"><div id=\"divForum_text\" class=\"amtabseltext\">" + sLabel + "</div></div>");
            if (recordId > 0)
            {
                sb.Append("<div id=\"divSecurity\" onclick=\"toggleTab(this);\" class=\"amtab\"><div id=\"divSecurity_text\" class=\"amtabtext\">[RESX:Security]</div></div>");
                if (editorType == "F" && chkInheritGroup.Checked)
                {
                    sb.Append("<div id=\"divSettings\" onclick=\"toggleTab(this);\" class=\"amtab\" style=\"display:none;\"><div id=\"divSettings_text\" class=\"amtabtext\">[RESX:Features]</div></div>");
                }
                else
                {
                    sb.Append("<div id=\"divSettings\" onclick=\"toggleTab(this);\" class=\"amtab\"><div id=\"divSettings_text\" class=\"amtabtext\">[RESX:Features]</div></div>");
                }
                if (editorType == "F")
                {
                    sb.Append("<div id=\"divClean\" onclick=\"toggleTab(this);\" class=\"amtab\"><div id=\"divClean_text\" class=\"amtabtext\">[RESX:Maintenance]</div></div>");
                    sb.Append("<div id=\"divProperties\" onclick=\"toggleTab(this);\" class=\"amtab\"><div id=\"divProperties_text\" class=\"amtabtext\">[RESX:TopicProperties]</div></div>");
                }

            }
            litTabs.Text = sb.ToString();
        }
        private void BindRoles()
        {
            DotNetNuke.Security.Roles.RoleController rc = new DotNetNuke.Security.Roles.RoleController();
            drpRoles.DataTextField = "RoleName";
            drpRoles.DataValueField = "RoleId";
            drpRoles.DataSource = rc.GetPortalRoles(PortalId);
            drpRoles.DataBind();
            drpRoles.Items.Insert(0, new ListItem("[RESX:DropDownDefault]", ""));
        }



        private void BindTemplates()
        {
            BindTemplateDropDown(drpTopicsTemplate, Templates.TemplateTypes.TopicsView, "[RESX:Default]", "0");
            BindTemplateDropDown(drpTopicTemplate, Templates.TemplateTypes.TopicView, "[RESX:Default]", "0");
            BindTemplateDropDown(drpTopicForm, Templates.TemplateTypes.TopicForm, "[RESX:Default]", "0");
            BindTemplateDropDown(drpReplyForm, Templates.TemplateTypes.ReplyForm, "[RESX:Default]", "0");
            //BindTemplateDropDown(drpQuickReplyForm, Templates.TemplateTypes.QuickReplyForm, "[RESX:Default]", "0")
            BindTemplateDropDown(drpProfileDisplay, Templates.TemplateTypes.PostInfo, "[RESX:Default]", "0");

            BindTemplateDropDown(drpModApprovedTemplateId, Templates.TemplateTypes.ModEmail, "[RESX:DropDownDisabled]", "-1");
            BindTemplateDropDown(drpModDeleteTemplateId, Templates.TemplateTypes.ModEmail, "[RESX:DropDownDisabled]", "-1");
            BindTemplateDropDown(drpModMoveTemplateId, Templates.TemplateTypes.ModEmail, "[RESX:DropDownDisabled]", "-1");
            BindTemplateDropDown(drpModRejectTemplateId, Templates.TemplateTypes.ModEmail, "[RESX:DropDownDisabled]", "-1");
            BindTemplateDropDown(drpModNotifyTemplateId, Templates.TemplateTypes.ModEmail, "[RESX:DropDownDisabled]", "-1");


        }


        #endregion


        //private void cbMaint_Callback(object sender, Controls.CallBackEventArgs e)
        //{
        //    try
        //    {
        //        int fid = Convert.ToInt32(e.Parameters[1]);
        //        int olderThan = Convert.ToInt32(e.Parameters[2]);
        //        int byUserId = Convert.ToInt32(e.Parameters[3]);
        //        int lastActive = Convert.ToInt32(e.Parameters[4]);
        //        bool withNotReplies = Convert.ToBoolean(e.Parameters[5]);
        //        bool dryRun = Convert.ToBoolean(e.Parameters[6]);
        //        int rows = DataProvider.Instance().Forum_Maintenance(fid, olderThan, lastActive, byUserId, withNotReplies, dryRun, MainSettings.DeleteBehavior);
        //        if (dryRun)
        //        {
        //            litMaintStatus.Text = string.Format(GetSharedResource("[RESX:Maint:DryRunResults]"), rows.ToString());
        //        }
        //        else
        //        {
        //            litMaintStatus.Text = "<div class=\"afsuccess\">" + GetSharedResource("[RESX:ProcessComplete]") + "</div>";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        litMaintStatus.Text = "<div class=\"aferror\">" + ex.Message + "</div>";
        //    }

        //    litMaintStatus.RenderControl(e.Output);
        //}
    }
}
