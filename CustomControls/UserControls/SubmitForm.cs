//
// Active Forums - http://activeforums.org/
// Copyright (c) 2019
// by Active Forums Community
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [DefaultProperty("Text"), ToolboxData("<{0}:SubmitForm runat=server></{0}:SubmitForm>")]
    public class SubmitForm : TopicBase
    {
        #region EditorModes
        public enum EditorModes : int
        {
            NewTopic,
            Reply,
            ReplyWithBody,
            Quote,
            EditTopic,
            EditReply
        }
        #endregion
        #region Private Members
        private string _Subject = string.Empty;
        private string _topicSubject = string.Empty;
        private string _Summary = string.Empty;
        private string _Body = string.Empty;
        private string _Template = string.Empty;
        private ImageButton _PostButton = new ImageButton();
        private ImageButton _CancelButton = new ImageButton();
        private string _clientId;
        private string _topicIcon;
        private bool _locked;
        private bool _checked;
        private bool _pinned;
        private bool _subscribed;
        private DateTime _announceStart = DateTime.MinValue;
        private DateTime _announceEnd = DateTime.MinValue;
        private string _categories = string.Empty;
        private string _pollQuestion;
        private string _pollType;
        private string _pollOptions;
        private int _statusId = -1;
        private string _AuthorName = string.Empty;
        private string _postbackScript = string.Empty;
        private string _topicReviewTemplate = string.Empty;
        private int _contentId = -1;
        private int _authorId = -1;
        private bool _requireCaptcha = true;
        private int _topicPriority;
        private bool canModEdit;
        private bool canModApprove;
        private bool canEdit;
        private bool canReply;
        private bool canCreate;
        private bool canAttach;
        private bool canTrust;
        private bool canLock;
        private bool canPin;
        private bool canAnnounce;
        private bool canSubscribe;

        #endregion
        #region Public Properties
        public string Template
        {
            get
            {
                return _Template;
            }
            set
            {
                _Template = value;
            }
        }
        public string AuthorName
        {
            get
            {
                return txtUsername.Text;
            }
            set
            {
                _AuthorName = value;
                txtUsername.Text = value;
            }
        }
        public string Subject
        {
            get
            {
                return txtSubject.Text;
            }
            set
            {
                _Subject = value;
                txtSubject.Text = value;
            }
        }
        public string TopicSubject
        {
            get
            {
                return _topicSubject;
            }
            set
            {
                _topicSubject = value;
            }
        }
        public string Summary
        {
            get
            {
                return txtSummary.Text;
            }
            set
            {
                _Summary = value;
                txtSummary.Text = value;
            }
        }
        public string Body
        {
            get
            {
                string tempBody = null;
                if (plhEditor.Controls.Count > 0)
                {
                    switch (EditorType)
                    {
                        //Case EditorTypes.ACTIVEEDITOR
                        //    Dim txtEditor As New ActiveEditorControls.ActiveEditor
                        //    txtEditor = CType(plhEditor.Controls.Item(0), ActiveEditorControls.ActiveEditor)
                        //    Body = txtEditor.Text
                        case EditorTypes.HTMLEDITORPROVIDER:
                            tempBody = ((UI.UserControls.TextEditor)(plhEditor.FindControl("txtBody"))).Text;
                            break;
                        case EditorTypes.TEXTBOX:
                            tempBody = ((TextBox)txtEditor).Text;
                            break;
                    }
                }
                else
                {
                    return _Body;
                }
                //Return String.Empty
                return tempBody;
            }
            set
            {
                _Body = value;
                //txtBody.Text = value
            }
        }
        public ImageButton PostButton
        {
            get
            {
                return _PostButton;
            }
            set
            {
                _PostButton = value;
            }
        }
        public ImageButton CancelButton
        {
            get
            {
                return _CancelButton;
            }
            set
            {
                _CancelButton = value;
            }
        }

        public EditorTypes EditorType { get; set; }

        public Forum ForumInfo { get; set; }

        //Public Property TopicInfo() As TopicInfo
        //    Get
        //        Return _topicInfo
        //    End Get
        //    Set(ByVal value As TopicInfo)
        //        _topicInfo = value
        //    End Set
        //End Property
        public string TopicIcon
        {
            get
            {
                string tempTopicIcon = string.IsNullOrEmpty(afposticons.PostIcon) ? string.Empty : afposticons.PostIcon;

                //Return _topicIcon
                return tempTopicIcon;
            }
            set
            {
                _topicIcon = value;
                afposticons.PostIcon = value;
            }
        }
        public bool Locked
        {
            get
            {
                return chkLocked.Checked;
            }
            set
            {
                _locked = value;
                chkLocked.Checked = value;
            }
        }
        public bool Pinned
        {
            get
            {
                return chkPinned.Checked;
            }
            set
            {
                _pinned = value;
                chkPinned.Checked = value;
            }
        }
        public bool Subscribe
        {
            get
            {
                return chkSubscribe.Checked;
            }
            set
            {
                _subscribed = value;
                chkSubscribe.Checked = value;
            }
        }
        public bool IsApproved
        {
            get
            {
                return chkApproved.Checked;
            }
            set
            {
                _checked = value;
                chkApproved.Checked = value;
            }
        }
        public bool IsAnnounce
        {
            get
            {
                return chkAnnounce.Checked;
            }
            set
            {
                chkAnnounce.Checked = value;
            }
        }
        public int TopicPriority
        {
            get
            {
                return int.Parse(txtTopicPriority.Text);
            }
            set
            {
                txtTopicPriority.Text = value.ToString();
                _topicPriority = value;

            }
        }
        public DateTime AnnounceStart
        {
            get
            {
                if (calStartDate.SelectedDate == "" && _announceStart == new DateTime())
                {
                    return Utilities.NullDate();
                }
                if (!(string.IsNullOrEmpty(calStartDate.SelectedDate)))
                {
                    return Convert.ToDateTime(calStartDate.SelectedDate);
                }
                return _announceStart;
            }
            set
            {
                _announceStart = value;
                calStartDate.SelectedDate = value.ToString();
            }
        }
        public DateTime AnnounceEnd
        {
            get
            {
                if (calEndDate.SelectedDate == "" && _announceEnd == new DateTime())
                {
                    return Utilities.NullDate();
                }
                if (!(string.IsNullOrEmpty(calEndDate.SelectedDate)))
                {
                    return Convert.ToDateTime(calEndDate.SelectedDate);
                }
                return _announceEnd;
            }
            set
            {
                _announceEnd = value;
                calEndDate.SelectedDate = Convert.ToString(value);
            }
        }
        public string EditorClientId
        {
            get
            {
                return _clientId;
            }
        }

        public string AttachmentsClientId { get; set; }

        public EditorModes EditorMode { get; set; }

        public string Tags { get; set; }

        public string Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                _categories = value;
            }
        }

        public string PollQuestion
        {
            get
            {
                return afpolledit.PollQuestion;
            }
            set
            {
                _pollQuestion = value;
                afpolledit.PollQuestion = value;
            }
        }
        public string PollType
        {
            get
            {
                return afpolledit.PollType;
            }
            set
            {
                _pollType = value;
                afpolledit.PollType = value;
            }
        }
        public string PollOptions
        {
            get
            {
                return afpolledit.PollOptions;
            }
            set
            {
                _pollOptions = value;
                afpolledit.PollOptions = value;
            }
        }

        public bool ShowModOptions { get; set; }

        public int StatusId
        {
            get
            {
                return aftopicstatus.Status;
            }
            set
            {
                _statusId = value;
                aftopicstatus.Status = value;
            }
        }
        public string PostBackScript
        {
            get
            {
                return _postbackScript;
            }
        }
        public int ContentId
        {
            get
            {
                return _contentId;
            }
            set
            {
                _contentId = value;
            }
        }
        public int AuthorId
        {
            get
            {
                return _authorId;
            }
            set
            {
                _authorId = value;
            }
        }

        public bool AllowHTML { get; set; }

        public bool RequireCaptcha
        {
            get
            {
                return _requireCaptcha;
            }
            set
            {
                _requireCaptcha = value;
            }
        }
        public List<PropertiesInfo> TopicProperties { get; set; }
        #endregion
        #region Protected Controls
        protected TextBox txtSubject = new TextBox();
        protected TextBox txtSummary = new TextBox();
        protected Label lblSubject = new Label();
        //Protected editorActiveEditor As ActiveEditorControls.ActiveEditor
        protected TextBox editorTextBox;
        protected UI.UserControls.TextEditor editorDNN;
        protected PlaceHolder plhEditor = new PlaceHolder();
        protected ImageButton btnPost = new ImageButton();
        protected Button btnSubmit = new Button();
        protected ImageButton btnCancel = new ImageButton();
        protected ImageButton btnPreview = new ImageButton();
        protected PlaceHolder plhControl = new PlaceHolder();
        protected Control txtEditor = null;
        protected af_posticonlist afposticons = new af_posticonlist();
        protected af_polledit afpolledit = new af_polledit();
        protected af_topicstatus aftopicstatus = new af_topicstatus();
        protected CheckBox chkLocked = new CheckBox();
        protected CheckBox chkPinned = new CheckBox();
        protected CheckBox chkSubscribe = new CheckBox();
        protected CheckBox chkAnnounce = new CheckBox();
        protected CheckBox chkApproved = new CheckBox();
        protected System.Web.UI.WebControls.RequiredFieldValidator reqSubject = new System.Web.UI.WebControls.RequiredFieldValidator();
        protected Label reqCustomBody = new Label();
        protected DatePicker calStartDate = new DatePicker();
        protected DatePicker calEndDate = new DatePicker();
        protected af_attach ctlAttach = new af_attach();

        protected PlaceHolder plhUpload;
        //Protected WithEvents tsTags As DotNetNuke.Modules.ActiveForums.Controls.TextSuggest
        protected PlaceHolder plhTopicReview = new PlaceHolder();
        protected TextBox txtTopicPriority = new TextBox();
        //Support for Anonymous
        protected TextBox txtUsername = new TextBox();
        protected System.Web.UI.WebControls.RequiredFieldValidator reqUsername = new System.Web.UI.WebControls.RequiredFieldValidator();
        protected UI.WebControls.CaptchaControl ctlCaptcha = new UI.WebControls.CaptchaControl();
        protected System.Web.UI.WebControls.RequiredFieldValidator reqCaptcha = new System.Web.UI.WebControls.RequiredFieldValidator();

        private string BodyTemplate = string.Empty;

        #endregion

        // Defines the Click event.
        //
        public event EventHandler BubbleClick;
        protected void OnBubbleClick(EventArgs e)
        {
            if (BubbleClick != null)
                BubbleClick(this, e);
        }

        private string ParseForm(string template)
        {
            bool hasOptions = false;
            template = "<%@ Register TagPrefix=\"am\" Namespace=\"DotNetNuke.Modules.ActiveForums.Controls\" Assembly=\"DotNetNuke.Modules.ActiveForums\" %>" + template;
            template = "<%@ register src=\"~/DesktopModules/ActiveForums/controls/af_posticonlist.ascx\" tagprefix=\"af\" tagname=\"posticons\" %>" + template;
            template = template.Replace("[AF:INPUT:SUBJECT]", "<asp:textbox id=\"txtSubject\" cssclass=\"aftextbox\" runat=\"server\" />");
            template = template.Replace("[AF:REQ:SUBJECT]", "<asp:requiredfieldvalidator id=\"reqSubject\" validationgroup=\"afform\" ControlToValidate=\"txtSubject\" runat=\"server\" />");
            template = template.Replace("[AF:REQ:BODY]", "<asp:label id=\"reqCustomBody\" visible=\"false\" runat=\"server\" />");
            if (template.Contains("[AF:BODY:TEMPLATE]"))
            {
                BodyTemplate = TemplateUtils.GetTemplateSection(template, "[AF:BODY:TEMPLATE]", "[/AF:BODY:TEMPLATE]");
                BodyTemplate = BodyTemplate.Replace("[AF:BODY:TEMPLATE]", string.Empty);
                BodyTemplate = BodyTemplate.Replace("[/AF:BODY:TEMPLATE]", string.Empty);
                template = TemplateUtils.ReplaceSubSection(template, string.Empty, "[AF:BODY:TEMPLATE]", "[/AF:BODY:TEMPLATE]");
            }
            if (template.Contains("[TOOLBAR"))
            {
                var lit = new LiteralControl();
                object sToolbar = DataCache.CacheRetrieve("aftb" + ModuleId);
                if (sToolbar == null)
                {
                    sToolbar = Utilities.GetFileContent(SettingKeys.TemplatePath + "ToolBar.txt");
                    sToolbar = Utilities.ParseToolBar(sToolbar.ToString(), TabId, ModuleId, UserId, CurrentUserType);
                    DataCache.CacheStore("aftb" + ModuleId, sToolbar);
                }
                lit.Text = sToolbar.ToString();
                template = template.Replace("[TOOLBAR]", sToolbar.ToString());
            }

            template = template.Replace("[AF:INPUT:SUMMARY]", "<asp:textbox id=\"txtSummary\" runat=\"server\" />");
            template = template.Replace("[AF:INPUT:BODY]", "<asp:placeholder id=\"plhEditor\" runat=\"server\" />");
            template = template.Replace("[AF:LABEL:SUBJECT]", "<asp:label id=\"lblSubject\" runat=\"server\" />");
            if (!Request.IsAuthenticated & (canCreate || canReply))
            {
                if (template.Contains("[AF:UI:ANON]"))
                {
                    template = template.Replace("[AF:INPUT:USERNAME]", "<asp:textbox id=\"txtUsername\" cssclass=\"aftextbox\" runat=\"server\" />");
                    template = template.Replace("[AF:REQ:USERNAME]", "<asp:requiredfieldvalidator id=\"reqUsername\" validationgroup=\"afform\" ControlToValidate=\"txtUsername\" runat=\"server\" />");
                    template = "<%@ Register TagPrefix=\"dnn\" Assembly=\"DotNetNuke\" Namespace=\"DotNetNuke.UI.WebControls\"%>" + template;
                    template = template.Replace("[AF:INPUT:CAPTCHA]", "<dnn:captchacontrol  id=\"ctlCaptcha\" captchawidth=\"130\" captchaheight=\"40\" cssclass=\"Normal\" runat=\"server\" errorstyle-cssclass=\"NormalRed\"  />");
                    if (!RequireCaptcha)
                    {
                        template = template.Replace("[RESX:SecurityCode]:[AF:REQ:SECURITYCODE]", string.Empty);
                    }
                    template = template.Replace("[AF:UI:ANON]", string.Empty);
                    template = template.Replace("[/AF:UI:ANON]", string.Empty);


                }
            }
            else
            {
                template = TemplateUtils.ReplaceSubSection(template, string.Empty, "[AF:UI:ANON]", "[/AF:UI:ANON]");
            }

            if (EditorMode != EditorModes.NewTopic || EditorMode != EditorModes.EditTopic)
            {
                template = template.Replace("[AF:UI:SECTION:TOPICREVIEW]", "<table class=\"afsection\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"afsectionhd\" style=\"border-left:solid 1px #b3b3b3;\" onclick=\"aftoggleSection('TopicReview');\">[RESX:TopicReview]</td><td class=\"afsectionhd\" align=\"right\" style=\"border-right:solid 1px #b3b3b3;\"><img id=\"imgSectionTopicReview\" src=\"" + ImagePath + "/images/arrows_left.png\" border=\"0\" class=\"afarrow\" /></td></tr><tr><td colspan=\"2\" class=\"afsectiondsp\" id=\"sectionTopicReview\" style=\"display:none;\"><div class=\"affieldsetnote\">[RESX:TopicReview:Note]</div>");
                _topicReviewTemplate = TemplateUtils.GetTemplateSection(template, "[AF:CONTROL:TOPICREVIEW]", "[/AF:CONTROL:TOPICREVIEW]");
                template = TemplateUtils.ReplaceSubSection(template, "<asp:placeholder id=\"plhTopicReview\" runat=\"server\" />", "[AF:CONTROL:TOPICREVIEW]", "[/AF:CONTROL:TOPICREVIEW]");
                template = template.Replace("[/AF:UI:SECTION:TOPICREVIEW]", "</td></tr></table>");
            }
            if (Permissions.HasPerm(ForumInfo.Security.Tag, ForumUser.UserRoles))
            {
                template = template.Replace("[AF:UI:SECTION:TAGS]", "<table class=\"afsection\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"afsectionhd\" style=\"border-left:solid 1px #b3b3b3;\" onclick=\"aftoggleSection('Tags');\">[RESX:Tags]</td><td class=\"afsectionhd\" align=\"right\" style=\"border-right:solid 1px #b3b3b3;\"><img id=\"imgSectionTags\" src=\"" + ImagePath + "/images/arrows_left.png\" border=\"0\" class=\"afarrow\" /></td></tr><tr><td colspan=\"2\" class=\"afsectiondsp\" id=\"sectionTags\" style=\"display:none;\"><div class=\"affieldsetnote\">[RESX:Tags:Note]</div>");
                template = template.Replace("[AF:UI:FIELDSET:TAGS]", "<fieldset class=\"affieldset\"><legend>[RESX:Tags]</legend><div class=\"affieldsetnote\">[RESX:Tags:Note]</div>");
                var tc = new TokensController();
                string sTagOut = tc.TokenGet("editor", "[AF:CONTROL:TAGS]");
                if (string.IsNullOrEmpty(sTagOut))
                {
                    //sTagOut = "<am:textsuggest id=""tsTags"" runat=""server"" DataTextField=""TagName"" DataValueField=""TagName"" CssResults=""aftsresults"" CssResultItems=""aftsresultsitems"" CssResultItemsSelected=""aftsresultsel""  CssClass=""aftagstxt"" Width=""99%"" />"
                    sTagOut = "<input type=\"text\" id=\"txtTags\" style=\"width:98%;\" class=\"NormalTextBox\"  />";
                    //sTagOut &= "<script type=""text/javascript"">amaf_loadSuggest('txtTags', null, -1);</script>"
                }
                sTagOut = sTagOut.Replace("[TAGS]", Tags);
                template = template.Replace("[AF:CONTROL:TAGS]", sTagOut);
                template = template.Replace("[/AF:UI:FIELDSET:TAGS]", "</fieldset>");
                template = template.Replace("[/AF:UI:SECTION:TAGS]", "</td></tr></table>");
            }
            //Properties
            if ((EditorMode == EditorModes.EditTopic || EditorMode == EditorModes.NewTopic) & ForumInfo.Properties != null)
            {
                string pTemplate = TemplateUtils.GetTemplateSection(template, "[AF:PROPERTIES]", "[/AF:PROPERTIES]");
                string propList = string.Empty;
                foreach (PropertiesInfo p in ForumInfo.Properties)
                {
                    string pValue = string.Empty;
                    if (TopicProperties != null && TopicProperties.Count > 0)
                    {
                        foreach (PropertiesInfo tp in TopicProperties)
                        {
                            if (tp.PropertyId == p.PropertyId)
                            {
                                pValue = tp.DefaultValue;
                            }
                        }
                    }
                    string tmp = pTemplate;

                    if (p.IsRequired)
                    {
                        tmp = tmp.Replace("[AF:PROPERTY:LABEL]", "<span class=\"afprop-required\">[RESX:" + p.Name + "]</span>");
                        tmp = tmp.Replace("[AF:PROPERTY:REQUIRED]", "<span class=\"afrequired\"></span>");
                    }
                    else
                    {
                        tmp = tmp.Replace("[AF:PROPERTY:LABEL]", "<span class=\"afprop-normal\">[RESX:" + p.Name + "]</span>");
                        tmp = tmp.Replace("[AF:PROPERTY:REQUIRED]", string.Empty);
                    }
                    if (p.DataType == "text")
                    {
                        tmp = tmp.Replace("[AF:PROPERTY:CONTROL]", "<input type=\"text\" id=\"afprop-" + p.PropertyId.ToString() + "\" class=\"NormalTextBox afprop-input\" name=\"afprop-" + p.PropertyId + "\" value=\"" + pValue + "\" />");
                    }
                    else if (p.DataType == "yesno")
                    {
                        string sYesSelected = string.Empty;
                        string sNoSelected = " checked=\"checked\"";
                        if (pValue.ToLowerInvariant() == "yes")
                        {
                            sYesSelected = " checked=\"checked\"";
                            sNoSelected = string.Empty;
                        }
                        tmp = tmp.Replace("[AF:PROPERTY:CONTROL]", "[RESX:Yes]:<input type=\"radio\" id=\"afprop-" + p.PropertyId.ToString() + "-yes\" groupname=\"afprop-" + p.PropertyId.ToString() + "\" class=\"NormalTextBox afprop-radio\" name=\"afprop-" + p.PropertyId + "\" value=\"Yes\" " + sYesSelected + " /> [RESX:No]:<input type=\"radio\" id=\"afprop-" + p.PropertyId.ToString() + "-no\" groupname=\"afprop-" + p.PropertyId.ToString() + "\" class=\"NormalTextBox afprop-radio\" name=\"afprop-" + p.PropertyId + "\" value=\"No\" " + sNoSelected + " />");
                    }
                    else if (p.DataType.Contains("list"))
                    {
                        string sList = string.Empty;
                        var lists = new Common.Lists.ListController();
                        if (p.DataType.Contains("list|"))
                        {
                            sList = "<select id=\"afprop-" + p.PropertyId.ToString() + "\" class=\"NormalTextBox afprop-select\" name=\"afprop-" + p.PropertyId.ToString() + "\">";

                            string lName = p.DataType.Substring(p.DataType.IndexOf("|") + 1);
                            var lc = lists.GetListEntryInfoCollection(lName, string.Empty);
                            foreach (Common.Lists.ListEntryInfo l in lc)
                            {
                                if (pValue == l.Value)
                                {
                                    sList += "<option value=\"" + l.Value + "\" selected=\"selected\">" + l.Text + "</option>";
                                }
                                else
                                {
                                    sList += "<option value=\"" + l.Value + "\">" + l.Text + "</option>";
                                }

                            }
                            sList += "</select>";
                        }
                        else if (p.DataType.Contains("list-multi|"))
                        {
                            sList = "<div class=\"afprop-chklist\">";
                            sList += "<ul>";
                            string lName = p.DataType.Substring(p.DataType.IndexOf("|") + 1);
                            var lc = lists.GetListEntryInfoCollection(lName, string.Empty);
                            string[] pValues = null;
                            if (!(string.IsNullOrEmpty(pValue)))
                            {
                                pValues = pValue.Split(',');
                            }
                            foreach (Common.Lists.ListEntryInfo l in lc)
                            {
                                bool isSelected = false;
                                if (pValues != null)
                                {
                                    foreach (string pv in pValues)
                                    {
                                        if (pv == l.Value)
                                        {
                                            isSelected = true;

                                        }
                                    }
                                }
                                sList += "<li>";
                                if (isSelected)
                                {
                                    sList += "<input type=\"checkbox\"  name=\"afprop-" + p.PropertyId.ToString() + "\" value=\"" + l.Value + "\" checked=\"checked\" />";
                                }
                                else
                                {
                                    sList += "<input type=\"checkbox\"  name=\"afprop-" + p.PropertyId.ToString() + "\" value=\"" + l.Value + "\" />";
                                }
                                sList += "<span>" + l.Text + "</span></li>";
                            }
                            sList += "</ul></div>";
                        }

                        tmp = tmp.Replace("[AF:PROPERTY:CONTROL]", sList);
                    }
                    propList += tmp;
                }
                template = TemplateUtils.ReplaceSubSection(template, propList, "[AF:PROPERTIES]", "[/AF:PROPERTIES]");
            }
            else
            {
                template = TemplateUtils.ReplaceSubSection(template, string.Empty, "[AF:PROPERTIES]", "[/AF:PROPERTIES]");
            }
            if ((EditorMode == EditorModes.EditTopic || EditorMode == EditorModes.NewTopic) && Permissions.HasPerm(ForumInfo.Security.Categorize, ForumUser.UserRoles))
            {
                template = template.Replace("[AF:UI:SECTION:CATEGORIES]", "<table class=\"afsection\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"afsectionhd\" style=\"border-left:solid 1px #b3b3b3;\">[RESX:Categories]</td><td class=\"afsectionhd\" align=\"right\" style=\"border-right:solid 1px #b3b3b3;\" onclick=\"aftoggleSection('Categories');\"><img id=\"imgSectionCategories\" src=\"" + ImagePath + "/images/arrows_left.png\" border=\"0\" class=\"afarrow\" /></td></tr><tr><td colspan=\"2\" class=\"afsectiondsp\" id=\"sectionCategories\" style=\"display:none;\"><div class=\"affieldsetnote\">[RESX:Categories:Note]</div>");
                template = template.Replace("[AF:UI:FIELDSET:CATEGORIES]", "<fieldset class=\"affieldset\"><legend>[RESX:Categories]</legend><div class=\"affieldsetnote\">[RESX:Categories:Note]</div>");
                string sCatOut;
                var cc = new CategoriesList(PortalId, ModuleId, ForumInfo.ForumID, ForumInfo.ForumGroupId);
                if (TopicId > 0)
                {
                    cc.SelectedValues = Categories;
                }
                sCatOut = cc.RenderEdit();
                template = template.Replace("[AF:CONTROL:CATEGORIES]", sCatOut);
                template = template.Replace("[/AF:UI:FIELDSET:CATEGORIES]", "</fieldset>");
                template = template.Replace("[/AF:UI:SECTION:CATEGORIES]", "</td></tr></table>");
            }
            if ((EditorMode == EditorModes.EditTopic || EditorMode == EditorModes.NewTopic) && Permissions.HasPerm(ForumInfo.Security.Poll, ForumUser.UserRoles))
            {
                template = "<%@ register src=\"~/DesktopModules/ActiveForums/controls/af_polledit.ascx\" tagprefix=\"af\" tagname=\"polledit\" %>" + template;
                template = template.Replace("[AF:UI:SECTION:POLL]", "<table class=\"afsection\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"afsectionhd\" style=\"border-left:solid 1px #b3b3b3;\">[RESX:Polls]</td><td class=\"afsectionhd\" align=\"right\" style=\"border-right:solid 1px #b3b3b3;\" class=\"afarrow\" onclick=\"aftoggleSection('Poll');\"><img id=\"imgSectionPoll\" src=\"" + ImagePath + "/images/arrows_left.png\" border=\"0\" /></td></tr><tr><td colspan=\"2\" class=\"afsectiondsp\" id=\"sectionPoll\" style=\"display:none;\"><div class=\"affieldsetnote\">[RESX:Poll:Note]</div>");
                template = template.Replace("[/AF:UI:SECTION:POLL]", "</td></tr></table>");
                template = template.Replace("[AF:UI:FIELDSET:POLL]", "<fieldset class=\"affieldset\"><legend>[RESX:Polls]</legend><div class=\"affieldsetnote\">[RESX:Poll:Note]</div>");
                template = template.Replace("[AF:CONTROL:POLL]", "<af:polledit id=\"afpolledit\" runat=\"server\" />");
                template = template.Replace("[/AF:UI:FIELDSET:POLL]", "</fieldset>");
                template = template.Replace("[AF:CONTROLS:SECTIONTOGGLE]", "<img id=\"imgSectionPoll\" src=\"" + ImagePath + "/images/arrows_left.png\" border=\"0\" class=\"afarrow\" onclick=\"aftoggleSection('Poll');\" />");
            }
            else
            {
                template = TemplateUtils.ReplaceSubSection(template, string.Empty, "[AF:UI:FIELDSET:POLL]", "[/AF:UI:FIELDSET:POLL]");
                template = template.Replace("[AF:CONTROL:POLL]", string.Empty);
            }
            if (EditorMode == EditorModes.ReplyWithBody)
            {
                template = template.Replace("[AF:UI:MESSAGEREPLY]", string.Empty);
                template = template.Replace("[/AF:UI:MESSAGEREPLY]", string.Empty);
                template = template.Replace("[AF:LABEL:BODYREPLY]", Body);
            }
            else
            {
                template = TemplateUtils.ReplaceSubSection(template, string.Empty, "[AF:UI:MESSAGEREPLY]", "[/AF:UI:MESSAGEREPLY]");
            }

            if (template.Contains("[AF:UI:FIELDSET:OPTIONS]"))
            {
                template = template.Replace("[AF:UI:FIELDSET:OPTIONS]", "<fieldset class=\"affieldset\"><legend>[RESX:AdditionalOptions]</legend>");
                template = template.Replace("[/AF:UI:FIELDSET:OPTIONS]", "</fieldset>");
            }

            string sOptions = GetOptions();
            template = template.Replace("[AF:CONTROL:OPTIONS]", sOptions);
            if (template.Contains("[AF:UI:SECTION:OPTIONS]"))
            {
                if (sOptions == string.Empty)
                {
                    template = template.Replace("[AF:UI:SECTION:OPTIONS]", string.Empty);
                    template = template.Replace("[/AF:UI:SECTION:OPTIONS]", string.Empty);
                }
                else
                {
                    template = template.Replace("[AF:UI:SECTION:OPTIONS]", "<table class=\"afsection\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"afsectionhd\" style=\"border-left:solid 1px #b3b3b3;\">[RESX:AdditionalOptions]</td><td class=\"afsectionhd\" align=\"right\" style=\"border-right:solid 1px #b3b3b3;\" class=\"afarrow\" onclick=\"aftoggleSection('Options');\"><img id=\"imgSectionOptions\" src=\"" + ImagePath + "/images/arrows_left.png\" border=\"0\" /></td></tr><tr><td colspan=\"2\" class=\"afsectiondsp\" id=\"sectionOptions\" style=\"display:none;\"><div class=\"affieldsetnote\">[RESX:Options:Note]</div>");
                    template = template.Replace("[/AF:UI:SECTION:OPTIONS]", "</td></tr></table>");
                }

            }
            if (template.Contains("[AF:CONTROL:STATUS]"))
            {
                if (EditorMode == EditorModes.EditTopic || EditorMode == EditorModes.NewTopic)
                {
                    template = "<%@ register src=\"~/DesktopModules/ActiveForums/controls/af_topicstatus.ascx\" tagprefix=\"af\" tagname=\"topicstatus\" %>" + template;
                    template = template.Replace("[AF:CONTROL:STATUS]", "<af:topicstatus id=\"aftopicstatus\" AutoPostBack=\"False\" ForumId=\"" + ForumInfo.ForumID + "\" runat=\"server\" />");
                }

            }

            template = template.Replace("[AF:LINK:FORUMNAME]", "<a href=\"" + NavigateUrl(TabId, "", ParamKeys.ViewType + "=" + Views.Topics + "&" + ParamKeys.ForumId + "=" + ForumInfo.ForumID.ToString()) + "\">" + ForumInfo.ForumName + "</a>");
            template = template.Replace("[AF:LINK:FORUMGROUP]", "<a href=\"" + NavigateUrl(TabId, "", ParamKeys.GroupId + "=" + ForumInfo.ForumGroupId.ToString()) + "\">" + ForumInfo.GroupName + "</a>");
            template = template.Replace("[AF:LINK:FORUMMAIN]", "<a href=\"" + NavigateUrl(TabId) + "\">[RESX:FORUMS]</a>");
            template = !(TopicId == -1) ? template.Replace("[AF:LINK:TOPICNAME]", "<a href=\"" + NavigateUrl(TabId, "", ParamKeys.TopicId + "=" + TopicId + "&" + ParamKeys.ViewType + "=" + Views.Topic + "&" + ParamKeys.ForumId + "=" + ForumInfo.ForumID.ToString()) + "\">" + TopicSubject + "</a>") : template.Replace("[AF:LINK:TOPICNAME]", string.Empty);
            template = template.Replace("[AF:UI:FIELDSET:ACTIONS]", "<fieldset class=\"affieldset\"><legend>[RESX:Actions]</legend>");
            template = template.Replace("[/AF:UI:FIELDSET:ACTIONS]", "</fieldset>");
            template = template.Replace("[AF:BUTTON:SUBMIT]", "<am:imagebutton id=\"btnPost\" Text=\"[RESX:Submit]\" runat=\"server\" />");
            template = template.Replace("[AF:BUTTON:CANCEL]", "<am:imagebutton id=\"btnCancel\" Text=\"[RESX:Cancel]\" runat=\"server\" />");
            template = template.Replace("[AF:BUTTON:PREVIEW]", Request.IsAuthenticated ? "<am:imagebutton id=\"btnPreview\" PostBack=\"False\"  Text=\"[RESX:Preview]\" runat=\"server\" />" : string.Empty);

            if (template.Contains("[AF:CONTROL:POSTICONS]") && ForumInfo.AllowPostIcon)
            {
                template = template.Replace("[AF:UI:FIELDSET:POSTICONS]", "<fieldset class=\"affieldset\"><legend>[RESX:PostIcons]</legend><div class=\"affieldsetnote\">[RESX:PostIcons:Note]</div>");
                template = template.Replace("[AF:CONTROL:POSTICONS]", "<af:posticons id=\"afposticons\" runat=\"server\" Theme=\"" + MainSettings.Theme + "\" />");
                template = template.Replace("[/AF:UI:FIELDSET:POSTICONS]", "</fieldset>");

            }
            else
            {
                template = template.Replace("[AF:UI:FIELDSET:POSTICONS]", string.Empty);
                template = template.Replace("[AF:CONTROL:POSTICONS]", string.Empty);
                template = template.Replace("[/AF:UI:FIELDSET:POSTICONS]", string.Empty);
            }
            if (template.Contains("[AF:CONTROL:EMOTICONS]") && ForumInfo.AllowEmoticons)
            {
                var objUtils = new emoticons();
                //Dim obj As Object = DataCache.CacheRetrieve(ForumModuleId & "Emoticons" & EditorType.ToString)
                //If obj Is Nothing Then
                //    DataCache.CacheStore(ForumModuleId & "Emoticons" & EditorType.ToString) = objUtils.LoadEmoticons(EditorType, ForumModuleId, ImagePath)
                //End If
                template = template.Replace("[AF:CONTROL:EMOTICONS]", "<fieldset class=\"affieldset\"><legend>[RESX:Smilies]</legend>" + objUtils.LoadEmoticons(EditorType, ModuleId, ImagePath) + "</fieldset>");
            }
            else
            {
                template = template.Replace("[AF:CONTROL:EMOTICONS]", string.Empty);
            }
            if (template.Contains("[AF:CONTROL:UPLOAD]"))
            {
                if (canAttach && ForumInfo.AllowAttach)
                {
                    template = "<%@ register src=\"~/DesktopModules/ActiveForums/controls/af_attach.ascx\" tagprefix=\"af\" tagname=\"attach\" %>" + template;
                    template = template.Replace("[AF:UI:FIELDSET:ATTACH]", "<fieldset class=\"affieldset\"><legend>[RESX:Attachments]</legend><div class=\"affieldsetnote\">[RESX:Attacments:Note]</div>");
                    template = template.Replace("[AF:UI:SECTION:ATTACH]", "<table class=\"afsection\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"afsectionhd\" style=\"border-left:solid 1px #b3b3b3;\">[RESX:Attachments]</td><td class=\"afsectionhd\" align=\"right\" style=\"border-right:solid 1px #b3b3b3;\" onclick=\"aftoggleSection('Attach');\"><img id=\"imgSectionAttach\" src=\"" + ImagePath + "/images/arrows_left.png\" border=\"0\" class=\"afarrow\"/></td></tr><tr><td colspan=\"2\" class=\"afsectiondsp\" id=\"sectionAttach\" style=\"display:none;\"><div class=\"affieldsetnote\">[RESX:Attachments:Note]</div>");
                    template = template.Replace("[AF:CONTROL:UPLOAD]", "<af:attach id=\"ctlAttach\" runat=\"server\" />");
                    template = template.Replace("[/AF:UI:FIELDSET:ATTACH]", "</fieldset>");
                    template = template.Replace("[/AF:UI:SECTION:ATTACH]", "</td></tr></table>");
                }
                else
                {
                    template = template.Replace("[AF:UI:FIELDSET:ATTACH]", string.Empty);
                    template = template.Replace("[AF:CONTROL:UPLOAD]", string.Empty);
                    template = template.Replace("[/AF:UI:FIELDSET:ATTACH]", string.Empty);
                    template = template.Replace("[AF:UI:SECTION:ATTACH]", string.Empty);
                    template = template.Replace("[/AF:UI:SECTION:ATTACH]", string.Empty);
                }

            }
            //If str.Contains("[AF:CONTROL:FORUMTREE]") Then
            //    str = str.Replace("[AF:CONTROL:FORUMTREE]", "<af:forumtree id=""ctlForumTree"" runat=""server"" showcheckboxes=""true"" ModuleId=""" & ModuleId & """ />")
            //    sHeader &= "<%@ register src=""~/DesktopModules/ActiveForums/controls/af_forumtree.ascx"" tagprefix=""af"" tagname=""forumtree"" %>"
            //End If
            template = Utilities.LocalizeControl(template);
            template = Utilities.StripTokens(template);
            return template;
        }
        private string GetOptions()
        {
            var sb = new StringBuilder();
            bool bHasOptions = false;
            sb.Append("<table cellpadding=\"2\" cellspacing=\"0\">");
            if (ForumInfo.IsModerated && canModApprove && ShowModOptions)
            {
                sb.Append("<tr><td>[RESX:Approved]:</td>");
                sb.Append("<td><asp:checkbox id=\"chkApproved\" Text=\"[RESX:Approved:Note]\" TextAlign=\"right\" cssclass=\"afcheckbox\" runat=\"server\" /></td></tr>");
                bHasOptions = true;
            }
            if ((canLock || canModEdit) & (EditorMode == EditorModes.NewTopic || EditorMode == EditorModes.EditTopic))
            {
                sb.Append("<tr><td>[RESX:Locked]:</td>");
                sb.Append("<td><asp:checkbox id=\"chkLocked\" Text=\"[RESX:Locked:Note]\" TextAlign=\"right\" cssclass=\"afcheckbox\" runat=\"server\" /></td></tr>");
                bHasOptions = true;
            }
            if ((canPin || canModEdit) & (EditorMode == EditorModes.NewTopic || EditorMode == EditorModes.EditTopic))
            {
                sb.Append("<tr><td>[RESX:Pinned]:</td>");
                sb.Append("<td><asp:checkbox id=\"chkPinned\" Text=\"[RESX:Pinned:Note]\" TextAlign=\"right\" cssclass=\"afcheckbox\" runat=\"server\" /></td></tr>");
                bHasOptions = true;
            }
            if (canSubscribe)
            {
                sb.Append("<tr><td>[RESX:Subscribe]:</td>");
                sb.Append("<td><asp:checkbox id=\"chkSubscribe\" Text=\"[RESX:Subscribe:Note]\" TextAlign=\"right\" cssclass=\"afcheckbox\" runat=\"server\" /></td></tr>");
                bHasOptions = true;
            }
            if ((EditorMode == EditorModes.NewTopic || EditorMode == EditorModes.EditTopic) && Permissions.HasPerm(ForumInfo.Security.Prioritize, ForumUser.UserRoles))
            {
                sb.Append("<tr><td>[RESX:TopicPriority]:</td>");
                sb.Append("<td><asp:textbox id=\"txtTopicPriority\" cssclass=\"aftextbox\" width=\"75\" onkeypress=\"return onlyNumbers(event);\" runat=\"server\" /></td></tr>");
                bHasOptions = true;
            }
            sb.Append("</table>");
            if (bHasOptions == false)
            {
                sb = new StringBuilder();
            }
            if (canAnnounce && (EditorMode == EditorModes.NewTopic || EditorMode == EditorModes.EditTopic))
            {
                sb.Append("<table cellpadding=\"2\" cellspacing=\"0\">");
                sb.Append("<tr><td valign=\"top\" colspan=\"3\">[RESX:Announce]:</td></tr><tr><td></td>");
                sb.Append("<td>[RESX:Announce:StartDate]:</td><td><am:datepicker id=\"calStartDate\" runat=\"server\" /></td></tr><tr><td></td><td>[RESX:Announce:EndDate]:</td><td><am:datepicker id=\"calEndDate\" runat=\"server\" /></td></tr>");
                sb.Append("</table>");
                bHasOptions = true;
            }
            if (bHasOptions)
            {
                return sb.ToString();
            }
            return string.Empty;
        }
        private void LoadForm()
        {
            AppRelativeVirtualPath = "~/submitform.ascx";
            var ctl = new Control {AppRelativeTemplateSourceDirectory = "~/", ID = "ctlTemp"};
            ctl = ParseControl(ParseForm(Template));
            Controls.Add(ctl);
            while (!(ctl.Controls.Count == 0))
            {
                Controls.Add(ctl.Controls[0]);
            }

            calStartDate.NullDate = Utilities.NullDate().ToString();
            calEndDate.NullDate = Utilities.NullDate().ToString();
            //calStartDate.DateFormat = MainSettings.DateFormatString
            //calEndDate.DateFormat = MainSettings.DateFormatString
            if (!(AnnounceStart == Utilities.NullDate()))
            {
                calStartDate.SelectedDate = AnnounceStart.ToString();
            }
            if (!(AnnounceEnd == Utilities.NullDate()))
            {
                calEndDate.SelectedDate = AnnounceEnd.ToString();
            }

            plhEditor = new PlaceHolder();
            plhEditor = (PlaceHolder)(FindControl("plhEditor"));
            //'plhEditor.Controls.Clear()
            Unit editorWidth;
            Unit editorHeight;
            if (Convert.ToString(ForumInfo.EditorWidth) != null)
            {
                editorWidth = Convert.ToString(ForumInfo.EditorWidth).IndexOf("%", 0, StringComparison.Ordinal) + 1 > 0 ? Unit.Percentage(Convert.ToDouble(Convert.ToString(ForumInfo.EditorWidth).TrimEnd('%'))) : Unit.Parse(ForumInfo.EditorWidth);
            }
            else
            {
                editorWidth = Unit.Pixel(600);
            }
            editorHeight = Convert.ToString(ForumInfo.EditorHeight) != null ? Unit.Parse(ForumInfo.EditorHeight) : Unit.Pixel(400);
            switch (EditorType)
            {
                case EditorTypes.TEXTBOX:
                    {
                        var editor = new TextBox();
                        txtEditor = editor;
                        editor.ID = "txtBody";
                        editor.Width = editorWidth;
                        editor.Height = editorHeight;
                        editor.TextMode = TextBoxMode.MultiLine;
                        editor.Rows = 4;
                        plhEditor.Controls.Add(editor);
                        _clientId = editor.ClientID;
                        break;
                    }
                case EditorTypes.HTMLEDITORPROVIDER:
                    {
                        //TODO: figure out why the editor no longer has any WYSIWYG functionality
                        var editor = new UI.UserControls.TextEditor();
                        txtEditor = editor;
                        editor = (UI.UserControls.TextEditor)(LoadControl("~/controls/TextEditor.ascx"));
                        editor.ID = "txtBody";
                        editor.ChooseMode = false;
                        editor.DefaultMode = "R";
                        editor.ChooseRender = false;
                        editor.Mode = "RICH";
                        editor.Width = editorWidth;
                        editor.Height = editorHeight;
                        plhEditor.Controls.Add(editor);
                        _clientId = editor.ClientID;
                        break;
                    }
            }
            plhTopicReview = new PlaceHolder();
            plhTopicReview = (PlaceHolder)(FindControl("plhTopicReview"));
            if (plhTopicReview != null)
            {
                var ctlTopicView = new TopicView
                                       {
                                           ModuleConfiguration = ModuleConfiguration,
                                           TopicTemplate = _topicReviewTemplate,
                                           OptPageSize = int.MaxValue,
                                           OptDefaultSort = "DESC",
                                           ForumInfo = ForumInfo
                                       };
                plhTopicReview.Controls.Add(ctlTopicView);
            }

            

        }
        public void btnPost_Click(object sender, EventArgs e)
        {

            var captcha = (UI.WebControls.CaptchaControl)(FindControl("ctlCaptcha"));
            if (captcha != null)
            {
                if (!captcha.IsValid && RequireCaptcha && !Request.IsAuthenticated)
                {
                    return;
                }
            }


            var rd = (RadioButtonList)(afposticons.FindControl("rblMessageIcons1"));
            var stat = (DropDownList)(aftopicstatus.FindControl("drpStatus"));

            var pollQuest = (TextBox)(afpolledit.FindControl("txtPollQuestion"));
            var pType = (RadioButtonList)(afpolledit.FindControl("rdPollType"));
            var pOpt = (TextBox)(afpolledit.FindControl("txtPollOptions"));
            if (rd != null)
            {
                TopicIcon = rd.SelectedItem.Value;
            }
            if (stat != null)
            {
                StatusId = Convert.ToInt32(stat.SelectedItem.Value);
            }
            if (pollQuest != null)
            {
                PollQuestion = pollQuest.Text;
                PollType = pType.SelectedItem.Value;
                PollOptions = pOpt.Text;
            }
            if (!(string.IsNullOrEmpty(Body)))
            {
                Body = Body.Replace("&nbsp;", " ");
                string tmpBody = Utilities.StripHTMLTag(Body);
                if (string.IsNullOrEmpty(tmpBody))
                {
                    reqCustomBody.Visible = true;
                }
                if (Body.Trim() == string.Empty)
                {
                    reqCustomBody.Visible = true;
                }
            }
            else
            {
                reqCustomBody.Visible = true;
            }
            if (!Request.IsAuthenticated && Utilities.InputIsValid(txtUsername.Text.Trim()) == false)
            {
                reqUsername.Visible = true;
            }

            OnBubbleClick(e);
        }
        protected override void Render(HtmlTextWriter writer)
        {
            var stringWriter = new System.IO.StringWriter();
            var htmlWriter = new HtmlTextWriter(stringWriter);
            base.Render(htmlWriter);
            string html = stringWriter.ToString();
            html = Utilities.LocalizeControl(html);
            writer.Write(html);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            canModEdit = Permissions.HasPerm(ForumInfo.Security.ModEdit, ForumUser.UserRoles);
            canModApprove = Permissions.HasPerm(ForumInfo.Security.ModApprove, ForumUser.UserRoles);
            canEdit = Permissions.HasPerm(ForumInfo.Security.Edit, ForumUser.UserRoles);
            canReply = Permissions.HasPerm(ForumInfo.Security.Reply, ForumUser.UserRoles);
            canCreate = Permissions.HasPerm(ForumInfo.Security.Create, ForumUser.UserRoles);
            canAttach = Permissions.HasPerm(ForumInfo.Security.Attach, ForumUser.UserRoles);
            canTrust = Permissions.HasPerm(ForumInfo.Security.Trust, ForumUser.UserRoles);
            canLock = Permissions.HasPerm(ForumInfo.Security.Lock, ForumUser.UserRoles);
            canPin = Permissions.HasPerm(ForumInfo.Security.Pin, ForumUser.UserRoles);
            canAnnounce = Permissions.HasPerm(ForumInfo.Security.Announce, ForumUser.UserRoles);
            canSubscribe = Permissions.HasPerm(ForumInfo.Security.Subscribe, ForumUser.UserRoles);

        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LoadForm();
            LinkControls(Controls);
            //If Not tsTags Is Nothing Then
            //    tsTags.Delimiter = ","
            //    tsTags.CharsTillLoad = 2
            //    tsTags.SelectedText = _tags
            //    tsTags.SelectedValue = _tags
            //End If

            txtSubject.CssClass = "aftextbox";
            string MyTheme = MainSettings.Theme;
            string MyThemePath = Page.ResolveUrl("~/DesktopModules/ActiveForums/themes/" + MyTheme);
            txtSubject.MaxLength = 255;
            txtSummary.MaxLength = 2000;
            txtSubject.Text = _Subject;
            txtSummary.Text = _Summary;
            txtUsername.Text = _AuthorName;
            lblSubject.Text = _Subject;
            lblSubject.CssClass = "aftextbox";
            txtSummary.CssClass = "aftextbox";
            chkLocked.Checked = _locked;
            chkPinned.Checked = _pinned;
            chkApproved.Checked = _checked;
            chkSubscribe.Checked = _subscribed;
            txtTopicPriority.Text = _topicPriority.ToString();
            if (AnnounceEnd > Utilities.NullDate())
            {
                calEndDate.SelectedDate = _announceEnd.ToString();
            }
            if (AnnounceStart > Utilities.NullDate())
            {
                calStartDate.SelectedDate = _announceStart.ToString();
            }
            btnPost.ImageLocation = PostButton.ImageLocation;
            btnPost.ImageUrl = PostButton.ImageUrl;
            btnPost.EnableClientValidation = false;
            btnPost.ValidationGroup = "afform";
            btnPost.ClientSideScript = PostButton.ClientSideScript;
            btnPost.Height = PostButton.Height;
            btnPost.Width = PostButton.Width;
            btnPost.PostBack = PostButton.PostBack;
            btnPost.CssClass = "amimagebutton";

            btnCancel.ImageLocation = CancelButton.ImageLocation;
            btnCancel.ImageUrl = CancelButton.ImageUrl;
            btnCancel.PostBack = CancelButton.PostBack;
            btnCancel.ClientSideScript = CancelButton.ClientSideScript;
            btnCancel.Confirm = CancelButton.Confirm;
            btnCancel.ConfirmMessage = CancelButton.ConfirmMessage;
            btnCancel.Height = CancelButton.Height;
            btnCancel.Width = CancelButton.Width;
            btnCancel.CssClass = "amimagebutton";

            btnPreview.Height = PostButton.Height;
            btnPreview.Width = PostButton.Width;
            btnPreview.CssClass = "amimagebutton";

            afposticons.PostIcon = _topicIcon;

            if (aftopicstatus != null)
            {
                aftopicstatus.Status = _statusId;
            }

            afpolledit.PollQuestion = _pollQuestion;
            afpolledit.PollType = _pollType;
            afpolledit.PollOptions = _pollOptions;
            afpolledit.ModuleConfiguration = ModuleConfiguration;
            afpolledit.ForumInfo = ForumInfo;

            reqSubject.ErrorMessage = string.Format("<img src=\"{0}/images/warning.png\" align=\"absmiddle\" />", MyThemePath);
            reqSubject.EnableClientScript = false;
            reqUsername.ErrorMessage = string.Format("<img src=\"{0}/images/warning.png\" align=\"absmiddle\" />", MyThemePath);
            reqUsername.EnableClientScript = false;
            reqCustomBody.Text = string.Format("<img src=\"{0}/images/warning.png\" align=\"absmiddle\" />", MyThemePath);
            btnPreview.ImageUrl = MyThemePath + "/images/preview32.png";
            btnPreview.ClientSideScript = "togglePreview(this);";
            btnPreview.ObjectId = "ancPreview";
            btnPreview.ImageLocation = PostButton.ImageLocation;
            btnPreview.Height = PostButton.Height;
            btnPreview.CssClass = "amimagebutton";

            if (EditorMode == EditorModes.NewTopic || EditorMode == EditorModes.Reply || EditorMode == EditorModes.ReplyWithBody)
            {
                _Body = BodyTemplate;
            }
            switch (EditorType)
            {
                //Case EditorTypes.ACTIVEEDITOR
                //    Dim txtEditor As New ActiveEditorControls.ActiveEditor
                //    txtEditor = CType(plhEditor.Controls.Item(0), ActiveEditorControls.ActiveEditor)
                //    txtEditor.Text = _Body
                case EditorTypes.HTMLEDITORPROVIDER:
                    ((UI.UserControls.TextEditor)(plhEditor.FindControl("txtBody"))).Text = _Body;
                    break;
                case EditorTypes.TEXTBOX:
                    ((TextBox)txtEditor).Text = _Body;
                    break;
            }


            EmotScript();
            insertHTMLScript();
            var sb = new StringBuilder();
            sb.Append("function CheckBody(sender, args){");
            sb.Append("if (args.Value == ''){");
            sb.Append(" args.IsValid = false;");
            sb.Append(" return;};");
            sb.Append(" args.IsValid = true;};");
            Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "bodyval", sb.ToString(), true);
            Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "ampost", "<script type=\"text/javascript\">function amPostback(){" + Page.ClientScript.GetPostBackEventReference(btnPost, string.Empty) + "};</script>", false);

            btnPost.Click += btnPost_Click;
            btnSubmit.Click += btnSubmit_Click;

            // This field is used to communicate attachment data between af_attach and af_post, etc.
            ctlAttach.AttachmentsClientId = AttachmentsClientId;
        }
        private void LinkControls(ControlCollection ctrls)
        {
            foreach (Control ctrl in ctrls)
            {
                switch (ctrl.ID)
                {
                    case "txtSubject":
                        txtSubject = (TextBox)ctrl;
                        break;
                    case "txtUsername":
                        txtUsername = (TextBox)ctrl;
                        break;
                    case "lblSubject":
                        lblSubject = (Label)ctrl;
                        break;
                    case "txtSummary":
                        txtSummary = (TextBox)ctrl;
                        break;
                    case "btnPost":
                        btnPost = (ImageButton)ctrl;
                        break;
                    case "btnCancel":
                        btnCancel = (ImageButton)ctrl;
                        break;
                    case "plhEditor":
                        switch (EditorType)
                        {
                            //Case EditorTypes.ACTIVEEDITOR
                            //    Dim txtEditor As New ActiveEditorControls.ActiveEditor
                            //    txtEditor = CType(plhEditor.Controls.Item(0), ActiveEditorControls.ActiveEditor)
                            case EditorTypes.HTMLEDITORPROVIDER:
                                var txtEditor = new UI.UserControls.TextEditor();
                                break;
                            case EditorTypes.TEXTBOX:
                                var txtEditor1 = (TextBox)(plhEditor.Controls[0]);
                                break;
                        }
                        break;
                    case "afposticons":
                        afposticons = (DotNetNuke.Modules.ActiveForums.af_posticonlist)ctrl;
                        break;
                    case "aftopicstatus":
                        aftopicstatus = (DotNetNuke.Modules.ActiveForums.af_topicstatus)ctrl;
                        break;
                    case "chkLocked":
                        chkLocked = (CheckBox)ctrl;
                        break;
                    case "chkPinned":
                        chkPinned = (CheckBox)ctrl;
                        break;
                    case "chkSubscribe":
                        chkSubscribe = (CheckBox)ctrl;
                        break;
                    case "txtTopicPriority":
                        txtTopicPriority = (TextBox)ctrl;
                        break;
                    case "chkApproved":
                        chkApproved = (CheckBox)ctrl;
                        break;
                    case "reqSubject":
                        reqSubject = (System.Web.UI.WebControls.RequiredFieldValidator)ctrl;
                        break;
                    case "reqCustomBody":
                        reqCustomBody = (Label)ctrl;
                        break;
                    case "calStartDate":
                        calStartDate = (DotNetNuke.Modules.ActiveForums.Controls.DatePicker)ctrl;
                        break;
                    case "calEndDate":
                        calEndDate = (DotNetNuke.Modules.ActiveForums.Controls.DatePicker)ctrl;
                        break;
                    case "btnPreview":
                        btnPreview = (DotNetNuke.Modules.ActiveForums.Controls.ImageButton)ctrl;
                        //Case "tsTags"
                        //    tsTags = CType(ctrl, DotNetNuke.Modules.ActiveForums.Controls.TextSuggest)
                        break;
                    case "afpolledit":
                        afpolledit = (DotNetNuke.Modules.ActiveForums.af_polledit)ctrl;
                        break;
                    case "ctlAttach":
                        ctlAttach = (DotNetNuke.Modules.ActiveForums.Controls.af_attach)ctrl;
                        ctlAttach.ModuleConfiguration = this.ModuleConfiguration;
                        ctlAttach.ModuleId = ModuleId;
                        ctlAttach.ForumInfo = ForumInfo;
                        break;
                    case "ctlCaptcha":
                        ctlCaptcha = (DotNetNuke.UI.WebControls.CaptchaControl)ctrl;
                        if (RequireCaptcha && !Request.IsAuthenticated)
                        {
                            ctlCaptcha.Visible = true;
                        }
                        else
                        {
                            ctlCaptcha.Visible = false;
                        }
                        break;
                }
                if (ctrl.Controls.Count > 0)
                {
                    LinkControls(ctrl.Controls);
                }
            }
        }
        private void btnSubmit_Click(object sender, System.EventArgs e)
        {

        }
        private void insertHTMLScript()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("var afeditor = '" + _clientId + "';");
            Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "editorHTML", sb.ToString(), true);
        }

        private void EmotScript()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("var afeditor = '" + _clientId + "';");
            Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "emot", sb.ToString(), true);
        }
    }
}