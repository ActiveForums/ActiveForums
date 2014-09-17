<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Classic.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.Classic" %>

<!-- 
// We do not need to use BS3 for now,
// for the time being, we will do all
// of our responsive w/o a framework.

<link href="//maxcdn.bootstrapcdn.com/bootstrap/3.2.0/css/bootstrap.min.css" rel="stylesheet"> -->
<link href="//maxcdn.bootstrapcdn.com/font-awesome/4.1.0/css/font-awesome.min.css" rel="stylesheet">

<asp:placeholder ID="plhToolbar" runat="server" />
<div class="afcontainer" id="afcontainer">
	<div id="amnotify" class="amnotify"><div><i></i><span id="amnotify-message"></span></div></div>
	<asp:PlaceHolder ID="plhLoader" runat="server" />
</div>
<asp:Literal ID="litOutput" runat="server" />
