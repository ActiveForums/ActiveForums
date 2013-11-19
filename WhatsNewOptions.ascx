<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="WhatsNewOptions.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.WhatsNewOptions" %>
<style>
    * .aftree td { font-size: 9px; }
    .aftree { font-size: 9px; }
    .aftagstxt { font-size: 11px; color: #000000; font-family: Tahoma, Arial, Verdana; border: solid 1px #cdcdcd; }
    .aftsresults { z-index: 50000; position: absolute; border: solid 1px #999; font-size: 11px; background-color: #f5f5f5; width: 150px; }
    .aftsresultsitems { z-index: 50000; border-bottom: solid 0px #999; font-size: 11px; }
    .aftsresultsel { z-index: 50000; background-color: Yellow; font-size: 11px; }
    .tokenbold { font-size: 9px; font-weight: bold; font-family: Tahoma, Arial; }
    .tokenhelp { font-size: 9px; font-family: Tahoma, Arial; }
    .validationError { color: #f00; }
</style>
<table width="99%">
    <tr>
        <td valign="top" style="font-family: Tahoma; font-size: 9px;" class="aftree">
            <asp:TreeView ID="trForums" runat="server" onclick="af_OnTreeClick(event);" CssClass="aftree" ShowCheckBoxes="All" />
        </td>
        <td valign="top">
            <table>
                <tr>
                    <td class="NormalBold" style="width:100px;">[RESX:TopicsOnly]:</td>
                    <td><asp:CheckBox ID="chkTopicsOnly" runat="server" /></td>
                </tr>
                <tr>
                    <td class="NormalBold">[RESX:FilterByTag]:</td>
                    <td><asp:TextBox ID="txtTags" runat="server" CssClass="NormalTextBox" Width="150" /></td>
                </tr>
                <tr>
                    <td class="NormalBold">[RESX:RandomOrder]:</td>
                    <td><asp:CheckBox ID="chkRandomOrder" runat="server" /></td>
                </tr>
                <tr>
                    <td class="NormalBold">[RESX:NumberOfItems]:</td>
                    <td><asp:TextBox ID="txtNumItems" runat="server" CssClass="NormalTextBox" Width="50" Text="10" MaxLength="3" /></td>
                </tr>
                <tr>
                    <td class="NormalBold" colspan="2">[RESX:HTMLHeader]:</td>
                </tr>
                <tr>
                    <td colspan="2"><asp:TextBox ID="txtHeader" runat="server" TextMode="MultiLine" Rows="3" cols="300" Width="300" /></td>
                </tr>
                <tr>
                    <td class="NormalBold" colspan="2">[RESX:HTMLTemplate]:</td>
                </tr>
                <tr>
                    <td colspan="2"><asp:TextBox ID="txtTemplate" runat="server" TextMode="MultiLine" Rows="6" cols="300" Width="300" /></td>
                </tr>
                <tr>
                    <td class="NormalBold" colspan="2">[RESX:HTMLFooter]:</td>
                </tr>
                <tr>
                    <td colspan="2"><asp:TextBox ID="txtFooter" runat="server" TextMode="MultiLine" Rows="3" cols="300" Width="300" /></td>
                </tr>
                <tr>
                    <td class="NormalBold" valign="top">[RESX:EnableRSS]:</td>
                    <td class="Normal" valign="top"><asp:CheckBox ID="chkRSS" runat="server" AutoPostBack="True" /></td>
                </tr>
                <tr id="trRSS" runat="server">
                    <td class="Normal" valign="top"></td>
                    <td class="NormalBold" valign="top">
                        <asp:PlaceHolder runat="server" ID="pnlRSS">
                            <asp:CheckBox ID="chkIgnoreSecurity" runat="server" /> [RESX:IgnoreSecurity]<br />
                            <asp:CheckBox ID="chkIncludeBody" runat="server" /> [RESX:IncludeBody]<br />
                            <div style="margin-top:5px;">
                                [RESX:CacheTimeOut]:<asp:TextBox ID="txtCache" runat="server" CssClass="NormalTextBox" Width="44px" Text="30" /> ([RESX:Minutes])
                                <asp:RequiredFieldValidator ID="rqvTxtCache" runat="server" ControlToValidate="txtCache" ErrorMessage="*" CssClass="validationError" ValidationGroup="WhatsNewOptions" />
                            </div>
						</asp:PlaceHolder>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div style="margin-top:10px; padding-top:10px; border-top:solid 1px #000;">
                            <asp:LinkButton ID="lnkUpdate" CssClass="dnnPrimaryAction" runat="server" Text="[RESX:Update]" ValidationGroup="WhatsNewOptions" />
                            <asp:LinkButton ID="lnkCancel" CssClass="dnnSecondaryAction" runat="server" Text="[RESX:Cancel]" CausesValidation="False"/>
                        </div>
                    </td>
                </tr>
            </table>
            <table style="margin-top:20px;">
                <tr>
                    <td colspan="2" class="NormalBold" style="padding-bottom:10px;">[RESX:Token:TemplateOnly]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[FORUMGROUPNAME]</td>
                    <td class="tokenhelp">[RESX:Token:ForumGroupName]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[FORUMGROUPID]</td>
                    <td class="tokenhelp">[RESX:Token:ForumGroupId]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[TOPICTABID]</td>
                    <td class="tokenhelp">[RESX:Token:TopicTabId]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[TOPICMODULEID]</td>
                    <td class="tokenhelp">[RESX:Token:TopicModuleId]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[FORUMNAME]</td>
                    <td class="tokenhelp">[RESX:Token:ForumName]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[FORUMID]</td>
                    <td class="tokenhelp">[RESX:Token:ForumId]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[SUBJECT]</td>
                    <td class="tokenhelp">[RESX:Token:Subject]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[AUTHORUSERNAME]</td>
                    <td class="tokenhelp">[RESX:Token:AuthorUserName]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[AUTHORFIRSTNAME]</td>
                    <td class="tokenhelp">[RESX:Token:AuthorFirstName]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[AUTHORLASTNAME]</td>
                    <td class="tokenhelp">[RESX:Token:AuthorLastName]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[AUTHORID]</td>
                    <td class="tokenhelp">[RESX:Token:AuthorId]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[AUTHORDISPLAYNAME]</td>
                    <td class="tokenhelp">[RESX:Token:AuthorDisplayName]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[DATE]</td>
                    <td class="tokenhelp">[RESX:Token:Date]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[BODY:XX]</td>
                    <td class="tokenhelp">[RESX:Token:BodyXX]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[BODYHTML]</td>
                    <td class="tokenhelp">[RESX:Token:BodyHTML]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[BODYTEXT]</td>
                    <td class="tokenhelp">[RESX:Token:BodyText]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[TOPICID]</td>
                    <td class="tokenhelp">[RESX:Token:TopicId]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[REPLYID]</td>
                    <td class="tokenhelp">[RESX:Token:ReplyId]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[REPLYCOUNT]</td>
                    <td class="tokenhelp">[RESX:Token:ReplyCount]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[POSTURL]</td>
                    <td class="tokenhelp">[RESX:Token:PostURL]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[SUBJECTLINK]</td>
                    <td class="tokenhelp">[RESX:Token:SubjectLink]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[TOPICSURL]</td>
                    <td class="tokenhelp">[RESX:Token:TopicsURL]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[FORUMURL]</td>
                    <td class="tokenhelp">[RESX:Token:ForumURL]</td>
                </tr>
                <tr>
                    <td colspan="2" class="NormalBold" style="padding:10px 0px;">[RESX:Token:FooterOnly]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[RSSICON]</td>
                    <td class="tokenhelp">[RESX:Token:RSSICON]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[RSSURL]</td>
                    <td class="tokenhelp">[RESX:Token:RSSURL]</td>
                </tr>
                <tr>
                    <td class="tokenbold">[RSSICONLINK]</td>
                    <td class="tokenhelp">[RESX:Token:RSSIconLink]</td>
                </tr>
            </table>
        </td>
    </tr>
</table>
