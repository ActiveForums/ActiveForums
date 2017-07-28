<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ForumSettings.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.Controls.ForumSettings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnnjs" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnnjs:DnnJsInclude runat="server" FilePath="~/DesktopModules/ActiveForums/scripts/afadmin.js" />
<style>
	.urlToggle, .fullTextNote {float: left; left: 450px; position: absolute; top: 10px; }
	.urlOptions, .pointOptions {width:400px;margin-left:185px;display:none;}
	.pointOptions input[type="text"]{min-width:inherit;}
</style>
	<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("DisplaySettings")%></a></h2>
	<fieldset>
		<div class="dnnFormItem">
			<dnn:Label ID="lblMode" resourcekey="Mode" runat="server" Suffix=":" /> 
			<asp:DropDownList ID="drpMode" runat="server">
				<asp:ListItem Value="Standard" resourcekey="Standard"></asp:ListItem>
				<asp:ListItem Value="SocialGroup" resourcekey="SocialGroups"></asp:ListItem>
			</asp:DropDownList>
		</div>
		<div class="dnnFormItem" id="socialGroupTemplate">
			<dnn:Label resourcekey="ForumGroupTemplate" runat="server" Suffix=":" />
			<asp:DropDownList ID="drpForumGroupTemplate" runat="server" />
			<div class="dnnClear afSecGrid">
				<asp:Literal ID="litForumSecurity" runat="server" />
			</div>
		</div>
	   <div class="dnnFormItem">
			<dnn:Label ID="lblForumTheme" resourcekey="ForumTheme" runat="server" Suffix=":" /> 
			<asp:DropDownList ID="drpThemes" runat="server">
				<asp:ListItem Text="_default" Value="_default" resourcekey="Default" />
			</asp:DropDownList>
	   </div>
		<div class="dnnFormItem">
			<dnn:Label ID="lblTemplate" resourcekey="ForumTemplate" runat="server" Suffix=":" /> 
			<asp:DropDownList ID="drpTemplates" runat="server" />

	   </div>
		<div class="dnnFormItem">
			<dnn:label ID="lblDefaultPageSize" runat="server" resourcekey="DefaultPageSize" Suffix=":" />
            <asp:DropDownList ID="drpPageSize" runat="server">
                <asp:ListItem>5</asp:ListItem>
                <asp:ListItem>10</asp:ListItem>
                <asp:ListItem>25</asp:ListItem>
                <asp:ListItem>50</asp:ListItem>
                <asp:ListItem>100</asp:ListItem>
                <asp:ListItem>200</asp:ListItem>
            </asp:DropDownList>
		</div>
        <div class="dnnFormItem">
			<dnn:label ID="lblUseSkinBreadCrumb" runat="server" resourcekey="UseSkinBreadCrumb" Suffix=":" />
			<asp:RadioButtonList ID="rdUseSkinBreadCrumb" runat="server" RepeatDirection="Horizontal">
				<asp:ListItem Value="True" resourcekey="Yes" />
				<asp:ListItem Value="False" resourcekey="No" />
			</asp:RadioButtonList>
        </div>
        <div class="dnnFormItem">
			<dnn:Label ID="lblTimeFormat" resourcekey="TimeFormat" runat="server" Suffix=":" /> 
			<asp:TextBox ID="txtTimeFormat" runat="server" Width="100" />
		</div>
        <div class="dnnFormItem">
			<dnn:Label ID="lblDateFormat" resourcekey="DateFormat" runat="server" Suffix=":" /> 
			<asp:TextBox ID="txtDateFormat" runat="server" Width="100" />
		</div>
	</fieldset>
	<h2 id="H1" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("ContentOptions")%></a></h2>
	<fieldset>
		<div class="dnnFormItem">
			<dnn:label ID="lblFloodInterval" runat="server" resourcekey="FloodInterval" Suffix=":" />
            <asp:DropDownList ID="drpFloodInterval" runat="server">
                <asp:ListItem>0</asp:ListItem>
                <asp:ListItem>100</asp:ListItem>
                <asp:ListItem>200</asp:ListItem>
                <asp:ListItem>300</asp:ListItem>
            </asp:DropDownList>
	    </div>
		<div class="dnnFormItem">
			<dnn:label ID="lblEditInterval" runat="server" resourcekey="EditInterval" Suffix=":" />
            <asp:DropDownList ID="drpEditInterval" runat="server">
                <asp:ListItem>0</asp:ListItem>
                <asp:ListItem>30</asp:ListItem>
                <asp:ListItem>60</asp:ListItem>
            </asp:DropDownList>
		</div>
		<div class="dnnFormItem">
			<dnn:label ID="lblAutoLinks" runat="server" resourcekey="AutoLink" Suffix=":" />
			<asp:RadioButtonList ID="rdAutoLinks" runat="server" RepeatDirection="Horizontal">
				<asp:ListItem Value="True" resourcekey="Yes" />
				<asp:ListItem Value="False" resourcekey="No" />
			</asp:RadioButtonList>
		</div>
		<div class="dnnFormItem">
			<dnn:label ID="lblDeleteBehavior" runat="server" resourcekey="DeleteBehavior" Suffix=":" />
			 <asp:DropDownList ID="drpDeleteBehavior" runat="server">
				<asp:ListItem Value="0" resourcekey="Remove" Selected="True"></asp:ListItem>
				<asp:ListItem Value="1" resourcekey="Recycle"></asp:ListItem>
			</asp:DropDownList>
		</div>
		<div class="dnnFormItem">
			<dnn:label ID="lblAddThis" runat="server" resourcekey="AddThisUsername" Suffix=":" />
			<asp:TextBox ID="txtAddThis" runat="server" Width="100" />
		</div>

	</fieldset>
	<h2 id="H2" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("CommunityOptions")%></a></h2>
	<fieldset>

		<div class="dnnFormItem">
			<dnn:label ID="lblProfileVisibility" runat="server" resourcekey="ProfileVisibility" Suffix=":" />
			<asp:DropDownList ID="drpProfileVisibility" runat="server">
			    <asp:ListItem Value="0" resourcekey="Disabled" />
				<asp:ListItem Value="1" resourcekey="Everyone" />
                <asp:ListItem Value="2" resourcekey="AuthenticatedUsers" />
                <asp:ListItem Value="3" resourcekey="Moderators" />
                <asp:ListItem Value="4" resourcekey="Admins" />
			</asp:DropDownList>
		</div>
        <div class="dnnFormItem">
			<dnn:label ID="lblMessagingType" runat="server" resourcekey="MessagingType" Suffix=":" />
			<asp:DropDownList ID="drpMessagingType" runat="server">
			    <asp:ListItem Value="0" resourcekey="MessagingDisabled" />
				<asp:ListItem Value="1" resourcekey="MessagingCore" />
                <asp:ListItem Value="2" resourcekey="MessagingVentrian" />
			</asp:DropDownList>
		</div>
        <div class="dnnFormItem" id="divMessagingTab">
			<dnn:label ID="lblMessagingTab" runat="server" resourcekey="MessagingTab" Suffix=":" />
            <asp:DropDownList ID="drpMessagingTab" runat="server" />			
		</div> 
		<div class="dnnFormItem">
			<dnn:label ID="lblAvatarSize" runat="server" resourcekey="AvatarSize" Suffix=":" />
			<ul class="afavatarform"><li><%=LocalizeString("Height")%>:<asp:TextBox ID="txtAvatarHeight" runat="server" Width="20" MaxLength="3" /></li><li><%=LocalizeString("Width")%>:<asp:TextBox ID="txtAvatarWidth" runat="server" Width="20" MaxLength="3" /></li></ul>
		</div>
		<div class="dnnFormItem">
			<dnn:label ID="lblSignatures" runat="server" resourcekey="UserSignatures" Suffix=":" />
			  <asp:DropDownList ID="drpSignatures" runat="server">
				<asp:ListItem Value="0" resourcekey="SignatureDisabled"></asp:ListItem>
				<asp:ListItem Value="1" resourcekey="SignaturePlainText"></asp:ListItem>
				<asp:ListItem Value="2" resourcekey="SignatureHTML" Selected="True"></asp:ListItem>
			</asp:DropDownList>
		</div>

		<div class="dnnFormItem">
			<dnn:label ID="lblUserDisplayMode" runat="server" resourcekey="UserDisplayMode" Suffix=":" />
			<asp:DropDownList ID="drpUserDisplayMode" runat="server">
									<asp:ListItem Value="Username" resourcekey="UserName" />
									<asp:ListItem Value="Firstname" resourcekey="FirstName" />
									<asp:ListItem Value="Lastname" resourcekey="LastName" />
									<asp:ListItem Value="Fullname" resourcekey="FullName" />
									<asp:ListItem Value="Displayname" resourcekey="DisplayName" />
									</asp:DropDownList>
		</div>
        <div class="dnnFormItem">
			<dnn:label ID="lblUsersOnline" runat="server" resourcekey="UsersOnline" Suffix=":" />
			<asp:RadioButtonList ID="rdUsersOnline" runat="server" RepeatDirection="Horizontal">
				<asp:ListItem Value="True" resourcekey="Yes" />
				<asp:ListItem Value="False" resourcekey="No" />
			</asp:RadioButtonList>
        </div>
		<div class="dnnFormItem">
			<dnn:label ID="lblPoints" runat="server" resourcekey="EnablePoints" Suffix=":" />
			<asp:RadioButtonList ID="rdPoints" runat="server" RepeatDirection="Horizontal">
				<asp:ListItem Value="True" resourcekey="Yes" />
				<asp:ListItem Value="False" resourcekey="No" />
			</asp:RadioButtonList>
			<div class="pointOptions">
				<div class="dnnFormItem">
					<dnn:label runat="server" resourcekey="TopicPointValue" Suffix=":" />
					<asp:TextBox ID="txtTopicPointValue" runat="server" MaxLength="3" Width="25" Text="1" />
				</div>
				<div class="dnnFormItem">
					<dnn:label runat="server" resourcekey="ReplyPointValue" Suffix=":" />
					<asp:TextBox ID="txtReplyPointValue" runat="server" MaxLength="3" Width="25" Text="1" />
				</div>
				 <div class="dnnFormItem">
					<dnn:label runat="server" resourcekey="AnswerPointValue" Suffix=":" />
					<asp:TextBox ID="txtAnswerPointValue" runat="server" MaxLength="3" Width="25" Text="1" />
				</div>
				<div class="dnnFormItem">
					<dnn:label runat="server" resourcekey="MarkAnswerPointValue" Suffix=":" />
					<asp:TextBox ID="txtMarkAnswerPointValue" runat="server" MaxLength="3" Width="25" Text="1" />
				</div>
				 <div class="dnnFormItem">
					<dnn:label runat="server" resourcekey="ModPointValue" Suffix=":" />
					<asp:TextBox ID="txtModPointValue" runat="server" MaxLength="3" Width="25" Text="1" />
				</div>
			</div>

		</div>
	</fieldset>
	<h2 id="H3" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("AdvancedOptions")%></a></h2>
	<fieldset>
		<div class="dnnFormItem">
			<dnn:label ID="lblEnableURLRewriter" runat="server" resourcekey="EnableURLRewriter" Suffix=":" />

			<asp:RadioButtonList ID="rdEnableURLRewriter" RepeatDirection="Horizontal" runat="server">
				<asp:ListItem Value="True" resourcekey="Yes" />
				<asp:ListItem Value="False" resourcekey="No" />
			</asp:RadioButtonList>
			  <span class="urlToggle"><asp:Literal ID="litToggleConfig" runat="server" /></span>
			  <div class="urlOptions">
					<div class="dnnFormItem">
						 <dnn:label ID="lblUrlPrefix" runat="server" resourcekey="URLPrefixBase" Suffix=":" />
						 <asp:TextBox ID="txtURLPrefixBase" runat="server" MaxLength="50" />
					</div>
					 <div class="dnnFormItem">
						 <dnn:label runat="server" resourcekey="URLPrefixCategory" Suffix=":" />
						 <asp:TextBox ID="txtURLPrefixCategory" runat="server" MaxLength="50" />
					</div>
					 <div class="dnnFormItem">
						 <dnn:label runat="server" resourcekey="URLPrefixTag" Suffix=":" />
						 <asp:TextBox ID="txtURLPrefixTags" runat="server" MaxLength="50" />
					</div>
					 <div class="dnnFormItem">
						 <dnn:label runat="server" resourcekey="URLPrefixOther" Suffix=":" />
						 <asp:TextBox ID="txtURLPrefixOther" runat="server" MaxLength="50" />
					</div>
			  </div>

		</div>
		<div class="dnnFormItem">
			<dnn:label ID="lblFullTextSearch" runat="server" resourcekey="FullTextSearch" Suffix=":" />
			<asp:RadioButtonList ID="rdFullTextSearch" RepeatDirection="Horizontal" runat="server">
                <asp:ListItem Value="True" resourcekey="Yes" />
				<asp:ListItem Value="False" resourcekey="No" />
			</asp:RadioButtonList>
            <span class="fullTextNote">
                <asp:Literal ID="ltrFullTextMessage" runat="server" />
            </span>
		</div>
		  <div class="dnnFormItem">
			<dnn:label ID="lblMailQueue" runat="server" resourcekey="MailQueue" Suffix=":" />
			<asp:RadioButtonList ID="rdMailQueue" RepeatDirection="Horizontal" runat="server">
				<asp:ListItem Value="True" resourcekey="Yes" />
				<asp:ListItem Value="False" resourcekey="No" />
			</asp:RadioButtonList>
		</div>
	</fieldset>
	<asp:HiddenField ID="txtGroupModSec" runat="server" />
	<asp:HiddenField ID="txtGroupMemSec" runat="server" />
	<asp:HiddenField ID="txtGroupRegSec" runat="server" />
	<asp:HiddenField ID="txtGroupAnonSec" runat="server" />

<script type="text/javascript">
	function amaf_toggleConfig(action, obj) {
		var data = {};
		data.ModuleId = <%=ModuleId%>;
		var sf = $.ServicesFramework(<%=ModuleId%>);
		//sf.getAntiForgeryProperty(data);
		$.ajax({
				type: "POST",
				url: sf.getServiceRoot('ActiveForums') + "AdminService/ToggleURLHandler",
				beforeSend: sf.setModuleHeaders,
				data: data,
				success: function (data) {
					if (data == 'enabled') {
						$('#<%=rdEnableURLRewriter.ClientID%>').removeAttr('disabled');
						$('#<%=rdEnableURLRewriter.ClientID%> span').removeAttr('disabled');
						$('#<%=rdEnableURLRewriter.ClientID%> span input').removeAttr('disabled');
						$('.urlToggle a').text('Uninstall Active Forums URL Handler');
						$('.urlOptions').show();
					}else{
						window.location.href = window.location.href;
					}
					if (typeof (callback) != "undefined") {
						callback(data,groupId);
					}
				},
				error: function (xhr, status, error) {
					alert(error);
				}
			});
	}
	jQuery(document).ready(function ($) {
		var status = $('#<%=rdEnableURLRewriter.ClientID%>').attr('disabled');
		if (typeof(status) == 'undefined') {
			$('.urlOptions').show();
		}

		$('.urlOptions input').keypress(function(event){
			return filterVanity(this, event);
		});

		var pointsSelected = $("#<%=rdPoints.ClientID%> input:checked").val();
		if (pointsSelected == 'True') {
			$('.pointOptions').show();
		}
		$('#<%=rdPoints.ClientID%> input').click(function() {
			var selected = $("#<%=rdPoints.ClientID%> input:checked").val();
			if (selected == 'True') {
				$('.pointOptions').show();
			}else{
				$('.pointOptions').hide();
			}
		});

		$('.pointOptions input').keypress(function(event) {
			return onlyNumbers(event);
		});
	    
		$('#<%=drpMessagingType.ClientID%>').change(function() {
		    toggleMessagingTab();
		});
		function toggleMessagingTab() {
		    var val = $('#<%=drpMessagingType.ClientID%>').val();

            if(val == '2') {
                $('#divMessagingTab').show();
            }else{
                $('#divMessagingTab').hide();
            };

        };
	    toggleMessagingTab();



		$('#<%=drpMode.ClientID%>').change(function() {
			toggleGroup();
		});
		function toggleGroup() {
			var val = $('#<%=drpMode.ClientID%>').val();
			if(val == 'SocialGroup') {
				$('#socialGroupTemplate').show();
			}else{
				$('#socialGroupTemplate').hide();
			};
		};
		toggleGroup();
		function getSecChecks(prefix){
			var chks = document.getElementsByTagName("input");
			var chckVal = '';
			for (i = 0; i <= chks.length -1; i++) {
				var inp = chks[i];
				if (inp.type == 'checkbox') {
					if (inp.id.indexOf(prefix) >= 0) {
						if (inp.checked == true){
							chckVal += inp.id.replace(prefix,'') + '=true,';
						}else{
							chckVal += inp.id.replace(prefix,'') + '=false,';
						};
					};
				};
			};
			return chckVal;
		};
		$('#<%=txtGroupModSec.ClientID%>').val(getSecChecks('ga'));
		$('#<%=txtGroupMemSec.ClientID%>').val(getSecChecks('gm'));
		$('#<%=txtGroupRegSec.ClientID%>').val(getSecChecks('gr'));
		$('#<%=txtGroupAnonSec.ClientID%>').val(getSecChecks('gn'));
		$('#row1 input').click(function(event) {
			$('#<%=txtGroupModSec.ClientID%>').val(getSecChecks('ga'));
		});
		$('#row2 input').click(function(event) {
			$('#<%=txtGroupMemSec.ClientID%>').val(getSecChecks('gm'));
		});
		$('#row3 input').click(function(event) {
			$('#<%=txtGroupRegSec.ClientID%>').val(getSecChecks('gr'));
		});
		$('#row4 input').click(function(event) {
			$('#<%=txtGroupAnonSec.ClientID%>').val(getSecChecks('gn'));
		});
	});
</script>