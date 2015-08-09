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
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using DotNetNuke.Security.Roles;

using AFSettings = DotNetNuke.Modules.ActiveForums.Settings;

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

            var sepChar = '|';
            if (Params != null && !(string.IsNullOrEmpty(Params)))
            {
                if (Params.Contains("!"))
                    sepChar = '!';

                editorType = Params.Split(sepChar)[1]; // Params.Split(CChar(sepChar))(1).Split(CChar("="))(1)
                recordId = Utilities.SafeConvertInt(Params.Split(sepChar)[0]);
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
            var propImage = "<img src=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/images/properties16.png") + "\" alt=\"[RESX:ConfigureProperties]\" />";

            rdAttachOn.Attributes.Add("onclick", "toggleAttach(this);");
            rdAttachOn.Attributes.Add("value", "1");
            rdAttachOff.Attributes.Add("onclick", "toggleAttach(this);");
            rdAttachOff.Attributes.Add("value", "0");
            cfgAttach.InnerHtml = propImage;

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
                lblMaintWarn.Text = string.Format(GetSharedResource("[RESX:MaintenanceWarning]"), GetSharedResource("[RESX:MaintenanceWarning:Recycle]"), GetSharedResource("[RESX:MaintenanceWarning:Recycle:Desc]"));
            else
                lblMaintWarn.Text = string.Format(GetSharedResource("[RESX:MaintenanceWarning]"), GetSharedResource("[RESX:MaintenanceWarning:Remove]"), GetSharedResource("[RESX:MaintenanceWarning:Remove:Desc]"));

            drpEditorTypes.Attributes.Add("onchange", "toggleEditorFields();");
            
            if (cbEditorAction.IsCallback) 
                return;
            
            BindGroups();
            BindTemplates();
            
            if (recordId > 0)
            {
                switch (editorType)
                {
                    case "F":
                        LoadForum(recordId);
                        break;
                    case "G":
                        LoadGroup(recordId);
                        break;
                }

                cfgHTML.Attributes.Add("onclick", "showProp(this,'edProp')");

                cfgMod.Attributes.Add("onclick", "showProp(this,'modProp')");

                cfgAutoSub.Attributes.Add("onclick", "showProp(this,'subProp')");

                cfgAttach.Attributes.Add("onclick", "showProp(this,'attachProp')");

                var sb = new StringBuilder();
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

        private void cbEditorAction_Callback(object sender, Controls.CallBackEventArgs e)
        {
            switch (e.Parameters[0].ToLowerInvariant())
            {
                case "forumsave":
                {
                    var fi = new Forum();
                    var fc = new ForumController();
                    var bIsNew = false;
                    int forumGroupId;
                    var forumSettingsKey = string.Empty;

                    if (Utilities.SafeConvertInt(e.Parameters[1]) <= 0)
                    {
                        bIsNew = true;
                        fi.ForumID = -1;
                    }
                    else
                    {
                        fi = fc.Forums_Get(PortalId, ModuleId, Utilities.SafeConvertInt(e.Parameters[1]), UserId, false, false, -1);
                        forumSettingsKey = fi.ForumSettingsKey;
                    }

                    fi.ModuleId = ModuleId;
                    fi.PortalId = PortalId;
                    var sParentValue = e.Parameters[2];
                    var parentForumId = 0;

                    if (sParentValue.Contains("GROUP"))
                    {
                        sParentValue = sParentValue.Replace("GROUP", string.Empty);
                        forumGroupId = Utilities.SafeConvertInt(sParentValue);
                    }
                    else
                    {
                        parentForumId = Utilities.SafeConvertInt(sParentValue.Replace("FORUM", string.Empty));
                        forumGroupId = fc.Forums_GetGroupId(parentForumId);
                    }

                    fi.ForumGroupId = forumGroupId;
                    fi.ParentForumId = parentForumId;
                    fi.ForumName = e.Parameters[3];
                    fi.ForumDesc = e.Parameters[4];
                    fi.Active = Utilities.SafeConvertBool(e.Parameters[5]);
                    fi.Hidden = Utilities.SafeConvertBool(e.Parameters[6]);

                    fi.SortOrder = string.IsNullOrWhiteSpace(e.Parameters[7]) ? 0 : Utilities.SafeConvertInt(e.Parameters[7]);

                    var fkey = string.IsNullOrEmpty(fi.ForumSettingsKey) ? string.Empty : fi.ForumSettingsKey;

                    if (Utilities.SafeConvertBool(e.Parameters[8]))
                    {
                        var fgc = new ForumGroupController();
                        var fgi = fgc.GetForumGroup(ModuleId, forumGroupId);
                            
                        if (bIsNew)
                            fi.PermissionsId = fgi.PermissionsId;
                        else if (fi.ForumSettingsKey != "G:" + forumGroupId)
                            fi.PermissionsId = fgi.PermissionsId;

                        fi.ForumSettingsKey = "G:" + forumGroupId;

                    }
                    else if (bIsNew || fkey.Contains("G:"))
                    {
                        fi.ForumSettingsKey = string.Empty;
                        if (fi.InheritSecurity)
                            fi.PermissionsId = -1;
                    }
                    else
                    {
                        fi.ForumSettingsKey = "F:" + fi.ForumID;
                    }

                    if (forumSettingsKey != fkey && fkey.Contains("F:"))
                        bIsNew = true;

                    fi.PrefixURL = e.Parameters[9];
                    if (!(string.IsNullOrEmpty(fi.PrefixURL)))
                    {
                        var db = new Data.Common();
                        if (!(db.CheckForumURL(PortalId, ModuleId, fi.PrefixURL, fi.ForumID, fi.ForumGroupId)))
                            fi.PrefixURL = string.Empty;
                    }

                    var forumId = fc.Forums_Save(PortalId, fi, bIsNew, Utilities.SafeConvertBool(e.Parameters[8]));
                    recordId = forumId;
                    var securityKey = string.Empty;
                        
                    DataCache.ClearForumGroupsCache(ModuleId);
                        
                    var cachekey = string.Format("AF-FI-{0}-{1}-{2}", PortalId, ModuleId, forumId);
                    DataCache.CacheClear(cachekey);
                        
                    cachekey = string.Format("AF-FV-{0}-{1}", PortalId, ModuleId);
                    DataCache.CacheClearPrefix(cachekey);

                    hidEditorResult.Value = forumId.ToString();
                    break;
                }

                case "groupsave":
                {
                    var bIsNew = false;
                    var groupId = Utilities.SafeConvertInt(e.Parameters[1]);        
                    var fgc = new ForumGroupController();
                    var gi = (groupId > 0) ? fgc.Groups_Get(ModuleId, groupId) : new ForumGroupInfo();

                    var securityKey = string.Empty;
                    if (groupId == 0)
                        bIsNew = true;
                    else
                        securityKey = "G:" + groupId;

                    gi.ModuleId = ModuleId;
                    gi.ForumGroupId = groupId;
                    gi.GroupName = e.Parameters[3];
                    gi.Active = Utilities.SafeConvertBool(e.Parameters[5]);
                    gi.Hidden = Utilities.SafeConvertBool(e.Parameters[6]);
                    
                    gi.SortOrder = string.IsNullOrWhiteSpace(e.Parameters[7]) ? 0 : Utilities.SafeConvertInt(e.Parameters[7]);

                    gi.PrefixURL = e.Parameters[9];
                    if (!(string.IsNullOrEmpty(gi.PrefixURL)))
                    {
                        var db = new Data.Common();
                        if (!(db.CheckGroupURL(PortalId, ModuleId, gi.PrefixURL, gi.ForumGroupId)))
                            gi.PrefixURL = string.Empty;
                    }

                    gi.GroupSettingsKey = securityKey;
                    var gc = new ForumGroupController();
                    groupId = gc.Groups_Save(PortalId, gi, bIsNew);
                    recordId = groupId;

                    DataCache.ClearForumGroupsCache(ModuleId);
                    var cachekey = string.Format("AF-FV-{0}-{1}", PortalId, ModuleId);
                    DataCache.CacheClearPrefix(cachekey);
                    hidEditorResult.Value = groupId.ToString();

                    break;
                }

                case "forumsettingssave":
                {
                    var forumId = Utilities.SafeConvertInt(e.Parameters[1]);
                    var sKey = "F:" + forumId;
                    SaveSettings(sKey, e.Parameters);

                    hidEditorResult.Value = forumId.ToString();
                    DataCache.CacheClear(forumId.ToString() + "ForumSettings");
                    DataCache.CacheClear(string.Format(CacheKeys.ForumInfo, forumId));
                    DataCache.CacheClear(string.Format(CacheKeys.ForumInfo, forumId) + "st");
                    var cachekey = string.Format("AF-FI-{0}-{1}-{2}", PortalId, ModuleId, forumId);
                    DataCache.CacheClear(cachekey);
                    break;
                }

                case "groupsettingssave":
                {
                    var forumId = Utilities.SafeConvertInt(e.Parameters[1]);
                    var sKey = "G:" + forumId;
                    SaveSettings(sKey, e.Parameters);

                    hidEditorResult.Value = forumId.ToString();
                    DataCache.CacheClear(forumId.ToString() + "GroupSettings");
                    DataCache.CacheClear(string.Format(CacheKeys.GroupInfo, forumId));
                    DataCache.CacheClear(string.Format(CacheKeys.GroupInfo, forumId) + "st");

                    break;
                }

                case "deleteforum":
                {
                    var forumId = Utilities.SafeConvertInt(e.Parameters[1]);
                    DataProvider.Instance().Forums_Delete(PortalId, ModuleId, forumId);
                    var cachekey = string.Format("AF-FV-{0}-{1}", PortalId, ModuleId);
                    DataCache.CacheClearPrefix(cachekey);
                    break;
                }

                case "deletegroup":
                {
                    var groupId = Utilities.SafeConvertInt(e.Parameters[1]);
                    DataProvider.Instance().Groups_Delete(ModuleId, groupId);
                    var cachekey = string.Format("AF-FV-{0}-{1}", PortalId, ModuleId);
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

        private void SaveSettings(string sKey, string[] parameters)
        {
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.TopicsTemplateId, parameters[2]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.TopicTemplateId, parameters[3]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EmailAddress, parameters[4]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.UseFilter, parameters[5]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AllowPostIcon, parameters[6]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AllowEmoticons, parameters[7]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AllowScript, parameters[8]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.IndexContent, parameters[9]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AllowRSS, parameters[10]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AllowAttach, parameters[11]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachCount, parameters[12]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachMaxSize, parameters[13]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachTypeAllowed, parameters[14]);
            //AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachStore, parameters[15]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachMaxHeight, parameters[16]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachMaxWidth, parameters[17]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachAllowBrowseSite, parameters[18]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachInsertAllowed, parameters[19]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.MaxAttachWidth, parameters[20]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.MaxAttachHeight, parameters[21]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ConvertingToJpegAllowed, parameters[22]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AllowHTML, parameters[23]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EditorType, parameters[24]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EditorHeight, parameters[25]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EditorWidth, parameters[26]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EditorToolbar, parameters[27]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EditorStyle, parameters[28]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EditorPermittedUsers, parameters[29]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.TopicFormId, parameters[30]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ReplyFormId, parameters[31]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.QuickReplyFormId, parameters[32]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ProfileTemplateId, parameters[33]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.IsModerated, parameters[34]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.DefaultTrustValue, parameters[35]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AutoTrustLevel, parameters[36]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ModApproveTemplateId, parameters[37]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ModRejectTemplateId, parameters[38]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ModMoveTemplateId, parameters[39]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ModDeleteTemplateId, parameters[40]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ModNotifyTemplateId, parameters[41]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AutoSubscribeEnabled, parameters[42]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AutoSubscribeRoles, parameters[43]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AutoSubscribeNewTopicsOnly, parameters[61]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ActiveSocialEnabled, parameters[62]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ActiveSocialTopicsOnly, parameters[63]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ActiveSocialSecurityOption, parameters[64]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.CreatePostCount, parameters[65]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ReplyPostCount, parameters[66]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AllowLikes, parameters[67]);
            AFSettings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EditorMobile, parameters[68]);
        }

        private void LoadForum(int forumId)
        {
            var fc = new ForumController();
            var fi = fc.Forums_Get(forumId, -1, false, false);

            if (fi == null) 
                return;

            var newForum = fc.GetForum(PortalId, ModuleId, forumId, true);
            
            ctlSecurityGrid = LoadControl("~/DesktopModules/activeforums/controls/admin_securitygrid.ascx") as Controls.admin_securitygrid;
            if(ctlSecurityGrid != null)
            {
                ctlSecurityGrid.Perms = newForum.Security;
                ctlSecurityGrid.PermissionsId = newForum.PermissionsId;
                ctlSecurityGrid.ModuleConfiguration = ModuleConfiguration;
                ctlSecurityGrid.ReadOnly = newForum.InheritSecurity;

                plhGrid.Controls.Clear();
                plhGrid.Controls.Add(ctlSecurityGrid); 
            }
      
            txtForumName.Text = fi.ForumName;
            txtForumDesc.Text = fi.ForumDesc;
            chkActive.Checked = fi.Active;
            chkHidden.Checked = fi.Hidden;
            hidForumId.Value = fi.ForumID.ToString();
            txtPrefixURL.Text = fi.PrefixURL;

            var groupValue = (fi.ParentForumId > 0) ? "FORUM" + fi.ParentForumId : "GROUP" + fi.ForumGroupId;


            Utilities.SelectListItemByValue(drpGroups, groupValue);

            if (fi.ForumSettingsKey == "G:" + fi.ForumGroupId)
            {
                chkInheritGroup.Checked = true;
                trTemplates.Attributes.Add("style", "display:none;");
            }

            Utilities.SelectListItemByValue(drpTopicsTemplate, fi.TopicsTemplateId);
            Utilities.SelectListItemByValue(drpTopicTemplate, fi.TopicTemplateId);
            Utilities.SelectListItemByValue(drpTopicForm, fi.TopicFormId);
            Utilities.SelectListItemByValue(drpReplyForm, fi.ReplyFormId);
            //Utilities.SelectListItemByValue(drpQuickReplyForm, fi.QuickReplyFormId);
            Utilities.SelectListItemByValue(drpProfileDisplay, fi.ProfileTemplateId);
            Utilities.SelectListItemByValue(drpModApprovedTemplateId, fi.ModApproveTemplateId);
            Utilities.SelectListItemByValue(drpModRejectTemplateId, fi.ModRejectTemplateId);
            Utilities.SelectListItemByValue(drpModDeleteTemplateId, fi.ModDeleteTemplateId);
            Utilities.SelectListItemByValue(drpModMoveTemplateId, fi.ModMoveTemplateId);
            Utilities.SelectListItemByValue(drpModNotifyTemplateId, fi.ModNotifyTemplateId);
            Utilities.SelectListItemByValue(drpDefaultTrust, fi.DefaultTrustValue);
            Utilities.SelectListItemByValue(drpEditorTypes, fi.EditorType);
            Utilities.SelectListItemByValue(drpEditorMobile, fi.EditorMobile);
            Utilities.SelectListItemByValue(drpPermittedRoles, (int)fi.EditorPermittedUsers);

            txtAutoTrustLevel.Text = fi.AutoTrustLevel.ToString();
            txtEmailAddress.Text = fi.EmailAddress;
            txtCreatePostCount.Text = fi.CreatePostCount.ToString();
            txtReplyPostCount.Text = fi.ReplyPostCount.ToString();

            rdFilterOn.Checked = fi.UseFilter;
            rdFilterOff.Checked = !fi.UseFilter;

            rdPostIconOn.Checked = fi.AllowPostIcon;
            rdPostIconOff.Checked = !fi.AllowPostIcon;

            rdEmotOn.Checked = fi.AllowEmoticons;
            rdEmotOff.Checked = !fi.AllowEmoticons;

            rdScriptsOn.Checked = fi.AllowScript;
            rdScriptsOff.Checked = !fi.AllowScript;

            rdIndexOn.Checked = fi.IndexContent;
            rdIndexOff.Checked = !fi.IndexContent;

            rdRSSOn.Checked = fi.AllowRSS;
            rdRSSOff.Checked = !fi.AllowRSS;

            rdAttachOn.Checked = fi.AllowAttach;
            rdAttachOff.Checked = !fi.AllowAttach;

            if (fi.AllowAttach)
                cfgAttach.Attributes.Remove("style");
            else
                cfgAttach.Attributes.Add("style", "display:none;");

            txtMaxAttach.Text = fi.AttachCount.ToString();
            txtMaxAttachSize.Text = fi.AttachMaxSize.ToString();
            txtAllowedTypes.Text = fi.AttachTypeAllowed;
            ckAllowBrowseSite.Checked = fi.AttachAllowBrowseSite;
            txtMaxAttachWidth.Text = fi.MaxAttachWidth.ToString();
            txtMaxAttachHeight.Text = fi.MaxAttachHeight.ToString();
            ckAttachInsertAllowed.Checked = fi.AttachInsertAllowed;
            ckConvertingToJpegAllowed.Checked = fi.ConvertingToJpegAllowed;

            rdHTMLOn.Checked = fi.AllowHTML;
            rdHTMLOff.Checked = !fi.AllowHTML;

            if (fi.AllowHTML)
                cfgHTML.Attributes.Remove("style");
            else
                cfgHTML.Attributes.Add("style", "display:none;");

            rdModOn.Checked = fi.IsModerated;
            rdModOff.Checked = !fi.IsModerated;

            if (fi.IsModerated)
                cfgMod.Attributes.Remove("style");
            else
                cfgMod.Attributes.Add("style", "display:none;");

            rdAutoSubOn.Checked = fi.AutoSubscribeEnabled;
            rdAutoSubOff.Checked = !fi.AutoSubscribeEnabled;

            if (fi.AutoSubscribeEnabled)
                cfgAutoSub.Attributes.Remove("style");
            else
                cfgAutoSub.Attributes.Add("style", "display:none;");

            rdLikesOn.Checked = fi.AllowLikes;
            rdLikesOff.Checked = !fi.AllowLikes;

            chkTopicsOnly.Checked = fi.AutoSubscribeNewTopicsOnly;

            txtEditorHeight.Text = (fi.EditorHeight == string.Empty) ? "400" : fi.EditorHeight;
            txtEditorWidth.Text = (fi.EditorWidth == string.Empty) ? "99%" : fi.EditorWidth;
            
            hidRoles.Value = fi.AutoSubscribeRoles;
            BindAutoSubRoles(fi.AutoSubscribeRoles);
        }

        private void LoadGroup(int groupId)
        {
            var gc = new ForumGroupController();
            var gi = gc.Groups_Get(ModuleId, groupId);

            if (gi == null)
                return;

            var newGroup = gc.GetForumGroup(ModuleId, groupId);

            ctlSecurityGrid = LoadControl("~/DesktopModules/activeforums/controls/admin_securitygrid.ascx") as Controls.admin_securitygrid;
            if(ctlSecurityGrid != null)
            {
                ctlSecurityGrid.Perms = newGroup.Security;
                ctlSecurityGrid.PermissionsId = newGroup.PermissionsId;
                ctlSecurityGrid.ModuleConfiguration = ModuleConfiguration;

                plhGrid.Controls.Clear();
                plhGrid.Controls.Add(ctlSecurityGrid);    
            }
            
            trGroups.Visible = false;
            reqGroups.Enabled = false;
            txtForumName.Text = gi.GroupName;
            chkActive.Checked = gi.Active;
            chkHidden.Checked = gi.Hidden;
            hidForumId.Value = gi.ForumGroupId.ToString();
            hidSortOrder.Value = gi.SortOrder.ToString();
            txtPrefixURL.Text = gi.PrefixURL;

            Utilities.SelectListItemByValue(drpTopicsTemplate, gi.TopicsTemplateId);
            Utilities.SelectListItemByValue(drpTopicTemplate, gi.TopicTemplateId);
            Utilities.SelectListItemByValue(drpTopicForm, gi.TopicFormId);
            Utilities.SelectListItemByValue(drpReplyForm, gi.ReplyFormId);
            Utilities.SelectListItemByValue(drpProfileDisplay, gi.ProfileTemplateId);
            Utilities.SelectListItemByValue(drpModApprovedTemplateId, gi.ModApproveTemplateId);
            Utilities.SelectListItemByValue(drpModRejectTemplateId, gi.ModRejectTemplateId);
            Utilities.SelectListItemByValue(drpModDeleteTemplateId, gi.ModDeleteTemplateId);
            Utilities.SelectListItemByValue(drpModMoveTemplateId, gi.ModMoveTemplateId);
            Utilities.SelectListItemByValue(drpModNotifyTemplateId, gi.ModNotifyTemplateId);
            Utilities.SelectListItemByValue(drpDefaultTrust, gi.DefaultTrustValue);
            Utilities.SelectListItemByValue(drpEditorTypes, (int)gi.EditorType);
            Utilities.SelectListItemByValue(drpEditorMobile, (int)gi.EditorMobile);
            Utilities.SelectListItemByValue(drpPermittedRoles, gi.EditorPermittedUsers);
            
            txtAutoTrustLevel.Text = gi.AutoTrustLevel.ToString();
            txtEmailAddress.Text = gi.EmailAddress;
            txtCreatePostCount.Text = gi.CreatePostCount.ToString();
            txtReplyPostCount.Text = gi.ReplyPostCount.ToString();
              
            rdFilterOn.Checked = gi.UseFilter;
            rdFilterOff.Checked = !gi.UseFilter;

            rdPostIconOn.Checked = gi.AllowPostIcon;
            rdPostIconOff.Checked = !gi.AllowPostIcon;

            rdEmotOn.Checked = gi.AllowEmoticons;
            rdEmotOff.Checked = !gi.AllowEmoticons;

            rdScriptsOn.Checked = gi.AllowScript;
            rdScriptsOff.Checked = !gi.AllowScript;

            rdIndexOn.Checked = gi.IndexContent;
            rdIndexOff.Checked = !gi.IndexContent;

            rdRSSOn.Checked = gi.AllowRSS;
            rdRSSOff.Checked = !gi.AllowRSS;

            rdAttachOn.Checked = gi.AllowAttach;
            rdAttachOff.Checked = !gi.AllowAttach;

            if (gi.AllowAttach)
                cfgAttach.Attributes.Remove("style");
            else
                cfgAttach.Attributes.Add("style", "display:none;");

            txtMaxAttach.Text = gi.AttachCount.ToString();
            txtMaxAttachSize.Text = gi.AttachMaxSize.ToString();
            txtAllowedTypes.Text = gi.AttachTypeAllowed;
            ckAllowBrowseSite.Checked = gi.AttachAllowBrowseSite;
            txtMaxAttachWidth.Text = gi.MaxAttachWidth.ToString();
            txtMaxAttachHeight.Text = gi.MaxAttachHeight.ToString();
            ckAttachInsertAllowed.Checked = gi.AttachInsertAllowed;
            ckConvertingToJpegAllowed.Checked = gi.ConvertingToJpegAllowed;

            rdHTMLOn.Checked = gi.AllowHTML;
            rdHTMLOff.Checked = !gi.AllowHTML;

            if (gi.AllowHTML)
                cfgHTML.Attributes.Remove("style");
            else
                cfgHTML.Attributes.Add("style", "display:none;");

            rdModOn.Checked = gi.IsModerated;
            rdModOff.Checked = !gi.IsModerated;

            if (gi.IsModerated)
                cfgMod.Attributes.Remove("style");
            else
                cfgMod.Attributes.Add("style", "display:none;");

            rdAutoSubOn.Checked = gi.AutoSubscribeEnabled;
            rdAutoSubOff.Checked = !gi.AutoSubscribeEnabled;

            rdLikesOn.Checked = gi.AllowLikes;
            rdLikesOff.Checked = !gi.AllowLikes;

            if (gi.AutoSubscribeEnabled)
                cfgAutoSub.Attributes.Remove("style");
            else
                cfgAutoSub.Attributes.Add("style", "display:none;");

            var x = gi.EditorType;

            txtEditorHeight.Text = (gi.EditorHeight == string.Empty) ? "400" : gi.EditorHeight;
            txtEditorWidth.Text = (gi.EditorWidth == string.Empty) ? "99%" : gi.EditorWidth;
            chkTopicsOnly.Checked = gi.AutoSubscribeNewTopicsOnly;
            hidRoles.Value = gi.AutoSubscribeRoles;
            BindAutoSubRoles(gi.AutoSubscribeRoles);
        }
        
        private void BindAutoSubRoles(string roles)
        {
            var sb = new StringBuilder();
            sb.Append("<table id=\"tblRoles\" cellpadding=\"0\" cellspacing=\"0\" width=\"99%\">");
            sb.Append("<tr><td width=\"99%\"></td><td></td></tr>");
            if (roles != null)
            {
                var rc = new RoleController();
                var arr = rc.GetPortalRoles(PortalId);
                foreach (RoleInfo ri in from RoleInfo ri in arr let sRoles = roles.Split(';') from role in sRoles.Where(role => role == ri.RoleID.ToString()) select ri)
                {
                    sb.Append("<tr><td class=\"amcpnormal\">" + ri.RoleName + "</td><td><img src=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/images/delete16.png") + "\" onclick=\"removeRole(this," + ri.RoleID + ");\" /></td></tr>");
                }
            }
            sb.Append("</table>");
            tbRoles.Text = sb.ToString();
        }

        private void BindGroups()
        {
            var dr = DataProvider.Instance().Forums_List(PortalId, ModuleId, -1, -1, false);
            drpGroups.Items.Add(new ListItem(Utilities.GetSharedResource("DropDownSelect"), "-1"));
            
            var tmpGroupId = -1;
            while (dr.Read())
            {
                var groupId = dr.GetInt("ForumGroupId");
                if (tmpGroupId != groupId)
                {
                    drpGroups.Items.Add(new ListItem(dr.GetString("GroupName"), "GROUP" + groupId));
                    tmpGroupId = groupId;
                }

                var forumId = dr.GetInt("ForumId");
                if (forumId == 0) 
                    continue;
                
                if (dr.GetInt("ParentForumID") == 0)
                    drpGroups.Items.Add(new ListItem(" - " + dr.GetString("ForumName"), "FORUM" + forumId));
            }

            dr.Close();
        }

        private void BindTabs()
        {
            var sb = new StringBuilder();
            var sLabel = (editorType == "F") ? "[RESX:Forum]" : "[RESX:Group]";
            
            sb.Append("<div id=\"divForum\" onclick=\"toggleTab(this);\" class=\"amtabsel\" style=\"margin-left:10px;\"><div id=\"divForum_text\" class=\"amtabseltext\">" + sLabel + "</div></div>");
            
            if (recordId > 0)
            {
                sb.Append("<div id=\"divSecurity\" onclick=\"toggleTab(this);\" class=\"amtab\"><div id=\"divSecurity_text\" class=\"amtabtext\">[RESX:Security]</div></div>");
                
                if (editorType == "F" && chkInheritGroup.Checked)
                    sb.Append("<div id=\"divSettings\" onclick=\"toggleTab(this);\" class=\"amtab\" style=\"display:none;\"><div id=\"divSettings_text\" class=\"amtabtext\">[RESX:Features]</div></div>");
                else
                    sb.Append("<div id=\"divSettings\" onclick=\"toggleTab(this);\" class=\"amtab\"><div id=\"divSettings_text\" class=\"amtabtext\">[RESX:Features]</div></div>");

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
            var rc = new RoleController();
            drpRoles.DataTextField = "RoleName";
            drpRoles.DataValueField = "RoleId";
            drpRoles.DataSource = rc.GetPortalRoles(PortalId);
            drpRoles.DataBind();
            drpRoles.Items.Insert(0, new ListItem("[RESX:DropDownDefault]", string.Empty));
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


    }
}
