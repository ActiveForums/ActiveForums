<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="uploader.aspx.cs" Inherits="DotNetNuke.Modules.ActiveForums.uploader" %>
<html>
<head>
	<title>Uploader</title>
	<script type="text/javascript">
	function af_hasfile(){
		var file = document.getElementById('<%=inpFile.ClientID%>');
		return file.value;
	};
	function af_disable() {
		var file = document.getElementById('<%=inpFile.ClientID%>');
		file.readOnly = true;
	};
	function af_enable() {
		var file = document.getElementById('<%=inpFile.ClientID%>');

		if (file.readOnly) {
			file.removeAttribute("readOnly");
		};

	};

	</script>
	<style type="text/css">
	.aftextbox{height:20px;font-family: Tahoma, Verdana, Arial;font-size: 11px; border:solid 1px #666666;}
	</style>
</head>
<body style="padding:0px;margin:0px;">
	<form id="frm1" name="frm1" enctype="multipart/form-data"  runat="server">
	<asp:FileUpload ID="inpFile" Width="100%" runat="server" CssClass="aftextbox" />
	<asp:HiddenField ID="isUploaded" runat="server" Value="0" />
	</form>
</body>
</html>