<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="profile_mypreferences.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.profile_mypreferences" %>
<%@ Register  assembly="DotNetNuke.Modules.ActiveForums" namespace="DotNetNuke.Modules.ActiveForums.Controls" tagPrefix="am" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnnjs" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<dnnjs:DnnCssInclude runat="server" FilePath="~/DesktopModules/ActiveForums/userpref.css" />
<script type="text/javascript">
	function amaf_cbPrefComplete(){
		$('.profMsg').show().fadeOut(3000, 'easeInExpo');
	};
</script>
<div class="dnnFormMessage dnnFormSuccess profMsg" style="display:none;"><%=GetString("[RESX:Actions:PrefUpdated]")%></div>
<div class="dnnForm afpref">
	<div class="dnnFormItem">
		<dnn:label controlname="drpPrefDefaultSort" resourcekey="[RESX:PrefDefaultSort]" Text="Default Sort"  Suffix=":" runat="server" />
		<asp:DropDownList ID="drpPrefDefaultSort" runat="server">
									<asp:ListItem Value="ASC" resourcekey="[RESX:OldestFirst]"></asp:ListItem>
									<asp:ListItem Value="DESC" resourcekey="[RESX:NewestFirst]"></asp:ListItem>
									</asp:DropDownList>
	</div>
	<div class="dnnFormItem">
		<dnn:label controlname="drpPrefPageSize" resourcekey="[RESX:PrefPageSize]" Text="Page Size"  Suffix=":" runat="server" />
        <asp:DropDownList ID="drpPrefPageSize" runat="server">
              <asp:ListItem>5</asp:ListItem>
                <asp:ListItem>10</asp:ListItem>
                <asp:ListItem>25</asp:ListItem>
                <asp:ListItem>50</asp:ListItem>
                <asp:ListItem>100</asp:ListItem>
                <asp:ListItem>200</asp:ListItem>
        </asp:DropDownList>

	</div>
	<div class="dnnFormItem">
		<dnn:label controlname="chkPrefJumpToLastPost" resourcekey="[RESX:PrefJumpToLastPost]" Text="Jump to last post"  Suffix=":" runat="server" />
		<asp:CheckBox ID="chkPrefJumpToLastPost" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label controlname="chkPrefTopicSubscribe" resourcekey="[RESX:PrefTopicSubscribe]" Text="Subscribe"  Suffix=":" runat="server" />
		<asp:CheckBox ID="chkPrefTopicSubscribe" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label controlname="chkPrefBlockAvatars" resourcekey="[RESX:PrefBlockAvatars]" Text="Block Avatars"  Suffix=":" runat="server" />
		<asp:CheckBox ID="chkPrefBlockAvatars" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label controlname="chkPrefBlockSignatures" resourcekey="[RESX:PrefBlockSignatures]" Text="Block Signatures"  Suffix=":" runat="server" />
		<asp:CheckBox ID="chkPrefBlockSignatures" runat="server" />
	</div>
	<div class="dnnFormItem">
		<dnn:label controlname="txtSignature" resourcekey="[RESX:Signature]" Text="Signature"  Suffix=":" runat="server" />
		<asp:TextBox ID="txtSignature" runat="server" TextMode="MultiLine" />
	</div>
	<ul class="dnnActions dnnClear">
		<li><asp:LinkButton ID="btnSave" CssClass="dnnPrimaryAction" runat="server" Text="Save" resourcekey="[RESX:Save]" /></li>
	</ul>
</div>