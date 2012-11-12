<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Classic.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.Classic" %>
<%@ Register TagPrefix="am" Namespace="DotNetNuke.Modules.ActiveForums.Controls" assembly="DotNetNuke.Modules.ActiveForums" %>

<asp:placeholder ID="plhToolbar" runat="server" />
<div class="afcontainer" id="afcontainer">
	<div id="amnotify" class="amnotify"><div><i></i><span id="amnotify-message"></span></div></div>
	<asp:PlaceHolder ID="plhLoader" runat="server" />
</div>

<asp:Literal ID="litOutput" runat="server" />