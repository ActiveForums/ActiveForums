<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_modtopics.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_modtopics_new" %>
<%@ Register TagPrefix="am" Namespace="DotNetNuke.Modules.ActiveForums.Controls" assembly="DotNetNuke.Modules.ActiveForums" %>
<script type="text/javascript">
var currAction = '';
function afmodEdit(url){
	window.location.href = url;
};
function afmodDelete(fid,tid,pid){
	currAction = 'moddel';
	af_showLoad();
	<%=cbMod.ClientID%>.Callback('modDel',fid,tid,pid);
};
function afmodApprove(fid,tid,pid){
	currAction = 'modappr';
	af_showLoad();
	<%=cbMod.ClientID%>.Callback('modAppr',fid,tid,pid);
};
function afmodReject(fid,tid,pid,uid){
	currAction = 'modreject';
	af_showLoad();
	<%=cbMod.ClientID%>.Callback('modReject',fid,tid,pid,uid);
};
function cbModComplete(){

};
</script>
<asp:Label ID="lblHeader" CssClass="aftitlelg" runat="server" />
<am:Callback ID="cbMod" runat="server" OnCallbackComplete="cbModComplete">
	<Content><asp:Literal ID="litTopics" runat="server" /></Content>
</am:Callback>