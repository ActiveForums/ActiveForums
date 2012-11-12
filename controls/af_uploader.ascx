<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_uploader.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_uploader" %>
<%@ Register TagPrefix="am" Namespace="DotNetNuke.Modules.ActiveForums.Controls" assembly="DotNetNuke.Modules.ActiveForums" %>
<script type="text/javascript">
	var isUploaded = false;
	var canPost = false;
	var canUpload = true;
	function af_uploadIt(){
	if (canUpload){
	var oIframe = document.getElementById('frmUploader');
	if (oIframe != null){
		 var oDoc = (oIframe.contentWindow || oIframe.contentDocument);
		if (oDoc.af_hasfile()!=''){
			af_preShowLoad();
			af_showLoad();
			isUploaded = false;
			oDoc.document.forms[0].submit();
		};
		};
	};
	};
	function disableUpload(){
		canUpload=false;
		var up = document.getElementById('btnUpload');
		up.style.display = 'none';
		var oIframe = document.getElementById('frmUploader');
		if (oIframe != null){
			var oDoc = (oIframe.contentWindow || oIframe.contentDocument);
			oDoc.af_disable();
		};  
	};
	function enableUpload(){
		canUpload = true;
		var up = document.getElementById('btnUpload');
		up.style.display = '';
		var oIframe = document.getElementById('frmUploader');
		if (oIframe != null){
			var oDoc = (oIframe.contentWindow || oIframe.contentDocument);
			//oDoc.af_enable();
		   // oDoc.af_disable();
		};  
	};
	function af_isUploaded(attachid){
		isUploaded = true;
		canPost = true;
		window.amaf_addAttachId(attachid);
		af_preClearLoad();
		af_clearLoad();

		var attachIds = window.amaf_getAttachIds();
		<%=cbAttach.ClientID%>.Callback('default',attachIds);
	};
	function af_setMessage(msg){
		af_clearLoad();
		isUploaded = true;
		canPost = false;
		var dsp = document.getElementById('afmessage');
		dsp.innerHTML = msg;
		isUploaded = false;
	};
	function af_delAttach(aid,uid){
		if (confirm('[RESX:Confirm:Delete]')){
			window.amaf_removeAttachId(aid);
			amaf_attachCount();
			var attachIds = window.amaf_getAttachIds();
			<%=cbMyFiles.ClientID%>.Callback('del',attachIds,aid,uid);
		};
	};
	function af_delContAttach(aid,uid){
		if (confirm('[RESX:Confirm:Delete]')){
			window.amaf_removeAttachId(aid);
			amaf_attachCount();
			var attachIds = window.amaf_getAttachIds();
			<%=cbAttach.ClientID%>.Callback('delcont',attachIds,aid,uid);
		};
	};

	function getIsUploaded(){
		return isUploaded;
	};
	function getCanPost(){
		return canPost;
	};
	function amaf_recalc(src){
	   var chk = document.getElementById("chkProp");
	   var h = document.getElementById("txtHeight");
	   var w = document.getElementById("txtWidth");
	   if (chk.checked && h.value != '' && w.value != ''){
			var tmp;
			var tmpH = parseInt(h.value);
			var tmpW = parseInt(w.value)
			if (tmpH < amaf_imgH || tmpW < amaf_imgW){
				if (src == 1){
					tmp = (tmpH / amaf_imgH);
					w.value = Math.round(amaf_imgW * tmp);
				};
				if (src == 2){
					var dif = (tmpW / amaf_imgW);
					h.value = Math.round(amaf_imgH * dif);
				};
			};
	   };
	};
	var amaf_imgH;
	var amaf_imgW;
	function amaf_insertThumbnail(aid,uid,w,h){
		amaf_imgH = h;
		amaf_imgW = w;
		window.amaf_MsgBox('[RESX:Actions:CreateThumbnail]','<table><tr><td>[RESX:Width]:</td><td><input type="text" id="txtWidth" name="txtWidth" onkeyup="amaf_recalc(2)" style="width:30px" class="aftextbox" value="' + w + '"></td></tr><tr><td>[RESX:Height]:</td><td><input type="text" id="txtHeight" name="txtHeight" onkeyup="amaf_recalc(1)" style="width:30px" class="aftextbox" value="' + h + '"></td></tr><tr><td colspan="4">[RESX:ThumbProportions]:<input type="checkbox" id="chkProp" checked></td></tr><tr><td colspan="4">[RESX:ThumbLink]:<input type="checkbox" id="chkOpenOrig" checked></td></tr><tr><td colspan="4" align="right"><input type="button" id="btnThumb" onclick="javascript:amaf_createThumbnail(' + aid + ',' + uid + ',150,150);" value="[RESX:Create]" /></td></tr></table>',175,250,'1');
	};
	function amaf_createThumbnail(aid,uid,w,h){
		var attachIds = window.amaf_getAttachIds();
		var h = document.getElementById("txtHeight").value;
		var w = document.getElementById("txtWidth").value;
		if (h != '' || w != ''){
		var linkOrig = document.getElementById("chkOpenOrig").checked;
		<%=cbAttach.ClientID%>.Callback('thumb',attachIds,aid,uid,w,h,linkOrig);
		window.amaf_MsgBoxClose();
		};

	};
	function amaf_toggleInline(aid,uid,opt){
		var attachIds = window.amaf_getAttachIds();
		<%=cbAttach.ClientID%>.Callback('inline',attachIds,aid,uid,opt);
	};
	function amaf_addAttach(aid,uid){
		window.amaf_addAttachId(aid);
		 var attachIds = window.amaf_getAttachIds();
		<%=cbAttach.ClientID%>.Callback('default',attachIds);
	};
	function amaf_attachComplete(){
		amaf_attachCount();

	};
	function amaf_attachCount(){
		var attachIds = window.amaf_getAttachIds().split(';');
		var fileText = document.getElementById("attachCount");
		fileText.innerHTML = ' (' + (parseInt(attachIds.length) - 1) + ')'
	};
	function amaf_myfileCount(i){
	   var fileText = document.getElementById("myFilesCount");
	   fileText.innerHTML = ' (' + i + ')'
	};
	function amaf_myfilesComplete(){

	};
</script>
<div id="divFiles" onclick="amaf_toggleTab(this);" class="amtabsel" style="margin-left:10px;">
	<div id="divFiles_text" class="amtabseltext">[RESX:AttachFiles]<span id="attachCount"></span></div>
</div>
<div id="divMyFiles" onclick="amaf_toggleTab(this);" class="amtab">
	<div id="divMyFiles_text" class="amtabtext">[RESX:MyFiles]<span id="myFilesCount"></span></div>
</div>
<div class="amtabcontent" id="amTabContent">
	<div id="divFiles_amafcontent" style="display:block;text-align:left;" class="amtabdisplay">
	<div class="afmessage" id="afmessage"></div>
	<div style="height:22px;clear:both;margin-bottom:5px;">
		<div style="height:22px;width:250px;display:inline;float:left;padding-top:3px;">
			<iframe marginheight="0" marginwidth="0" id="frmUploader" frameborder="0" scrolling="no" name="frmUploader" src="<%=Page.ResolveUrl("~/DesktopModules/ActiveForums/controls/uploader.aspx") + "?PortalId=" + PortalId + "&ForumId=" + ForumId%>" width="250" height="22"></iframe>
		</div>
		<div style="height:22px;padding-top:3px;">
			<am:ImageButton ID="btnUpload" PostBack="false" ClientSideScript="af_uploadIt();" CssClass="ambutton" runat="server" />
		</div>
	</div>
	<am:Callback ID="cbAttach" runat="server" OnCallbackComplete="amaf_attachComplete">
		<Content>
			<asp:PlaceHolder ID="plhAttach" runat="server" />
		</Content>
	</am:Callback>
	</div>
	<div id="divMyFiles_amafcontent" style="display:none;text-align:left;" class="amtabdisplay">
		<am:Callback ID="cbMyFiles" runat="server" OnCallbackComplete="amaf_myfilesComplete">
		<Content>
			<asp:PlaceHolder ID="plhMyFiles" runat="server" />
		</Content>
	</am:Callback>
	</div>
</div>