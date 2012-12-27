<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_post.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_post" %>
<%@ Register TagPrefix="am" Namespace="DotNetNuke.Modules.ActiveForums.Controls" assembly="DotNetNuke.Modules.ActiveForums" %>

<script type="text/javascript">
<!--
	var img1 = new Image();
	img1.src = '<%=spinner%>';
var animate;
var animatecount;
function togglePreview(obj) {
	var grid = document.getElementById("afgrid");
	if (grid.style.position == 'relative' || grid.style.position == ''){
		grid.style.position = 'static';
	}else{
		grid.style.position = '';
	};
	var position = getPreviewPosition(obj);
	var sBody = amaf_getBody();
	if (sBody == '') {
		return;
	}
	var prevInfo = document.getElementById('divPreviewWindow');
	var prevWidth = prevInfo.parentNode.offsetWidth - 4;
	var growX = prevWidth / 10;
	var growY = 302 / 10;
	var pt = document.getElementById("divPreviewText");
	var afPos = getPreviewPosition(prevInfo.parentNode);
	if (pt.innerHTML != '') {
		animatecount = 10;
		var prevInfo = document.getElementById('divPreviewWindow');
		var preview = document.createElement('div');
		preview.id = 'afPreview';
		preview.style.border = 'solid 1px #666';
		preview.style.width = prevInfo.offsetWidth + 'px';
		preview.style.height = '302px';
		preview.style.position = 'absolute';
		preview.style.top = prevInfo.offsetTop;
		preview.style.left = prevInfo.offsetLeft;
		document.body.appendChild(preview);
		animate = setInterval("showPreview(false," + prevWidth + "," + growX + ',' + growY + "," + afPos.x + "," + (position.y - 302) + "," + (position.x + (obj.offsetWidth / 2)) + "," + position.y + ")",1);
	}else{
		animatecount = 0;
		var preview = document.createElement('div');
		preview.id = 'afPreview';
		preview.style.border = 'solid 1px #666';
		preview.style.width = '0px';
		preview.style.height = '0px';
		preview.style.position = 'absolute';
		preview.style.top = position.y;
		preview.style.left = position.x + (obj.offsetWidth / 2);
		document.body.appendChild(preview);
		// state, preview width, grow width, grow height, end x, end y, start x, start y
		animate = setInterval("showPreview(true," + prevWidth + "," + growX + ',' + growY + "," + afPos.x + "," + (position.y - 302) + "," + (position.x + (obj.offsetWidth / 2)) + "," + position.y + ")",1);
	};
};
function showPreview(bool,prevWidth,growX,growY,endX,endY,startX,startY) {

	var preview = document.getElementById('afPreview');
	if (bool) {
		animatecount += 1;
		prevObj = document.getElementById('ancPreview');
		showLoading(prevObj);
		preview.style.height = (growY * animatecount) + 'px';
		preview.style.width = (growX * animatecount) + 'px';
		preview.style.top = (startY - ((302 / 10) * animatecount));
		preview.style.left = (startX - (((startX - endX) / 10) * animatecount));
		if (animatecount == 10) {
			clearInterval(animate);
			animate = null;
			var editor = document.getElementById("<%=EditorClientId%>")
			var sBody = amaf_getBody();
			<%=cbPreview.ClientID%>.Callback('preview',sBody)
			var out = document.getElementById('divPreviewWindow');
			out.style.display = 'block';
			preview.parentNode.removeChild(preview);
			preview = null;
		}
	}else{
		animatecount -= 1;
		var out = document.getElementById('divPreviewWindow');
		if (out.style.display == 'block') {
			out.style.display = 'none';
			var pt = document.getElementById("divPreviewText");
			pt.innerHTML = '';
		}
		preview.style.height = (growY * animatecount) + 'px';
		preview.style.width = (growX * animatecount) + 'px';
		preview.style.top = (startY - ((302 / 10) * animatecount));
		preview.style.left = (startX - (((startX - endX) / 10) * animatecount));
		if (animatecount == 0) {
			clearInterval(animate);
			animate = null;
			preview.parentNode.removeChild(preview);
			preview = null;
		};
	};
};
function cbPreview_render(){
	var sPreview = document.getElementById("<%=hidPreviewText.ClientID%>").value;
	var out = document.getElementById("divPreviewText");
	out.innerHTML = '';
	out.innerHTML = sPreview;
};


function getPreviewPosition(e){
	var left = 0;
	var top  = 0;
	while (e.offsetParent){
		left += e.offsetLeft;
		top  += e.offsetTop;
		e     = e.offsetParent;
	};
	left += e.offsetLeft;
	top  += e.offsetTop;

	return {x:left, y:top};
};
function closePreview(){
	var obj = document.getElementById('ancPreview');
	togglePreview(obj);
};
function showLoading(button){
	var pos = getPreviewPosition(button);
	var out = document.getElementById("divPreviewWindow");
	var pt = document.getElementById("divPreviewText");
	var afPos = getPreviewPosition(out.parentNode);
	pt.innerHTML = '';
	out.style.height = '300px';
	out.style.width = out.parentNode.offsetWidth + 'px';
	pt.style.overflow = 'auto';
	pt.style.height = '280px';
	out.style.top = (pos.y - 302) + 'px';
	out.style.left = afPos.x;
	pt.appendChild(img1);
}
var upl;
function af_preShowLoad(){
	var grid = document.getElementById("afgrid");
	if (grid.style.position == 'static' || grid.style.position == ''){
		grid.style.position = 'relative';
	}else{
		grid.style.position = '';
	};
};
function af_preClearLoad(){
var grid = document.getElementById("afgrid");
grid.style.position = 'relative';
};
function af_checkupload(){
	if ($('#attachments') != null) {
		var attachIds = '';
		$('#attachments tbody tr').each(function() {
			var fid = $(this).attr('id');
			attachIds += fid + ';';
		});
		$('#<%=hidAttachIds.ClientID%>').val(attachIds);
	}
	amPostback();
};

function amaf_addAttachId(attachid){
  var hidAttachIds = document.getElementById("<%=hidAttachIds.ClientID%>");

  if (attachid != ''){
  hidAttachIds.value = hidAttachIds.value + attachid + ';';
  };
};
function amaf_getAttachIds(){
  var hidAttachIds = document.getElementById("<%=hidAttachIds.ClientID%>");
  return hidAttachIds.value;
};
function amaf_setAttachIds(attachids){
	var hidAttachIds = document.getElementById("<%=hidAttachIds.ClientID%>");
	hidAttachIds.value = attachids;
};
function amaf_removeAttachId(attachid){
	var attachids = amaf_getAttachIds().split(';');
	var hidAttachIds = document.getElementById("<%=hidAttachIds.ClientID%>");
	hidAttachIds.value = '';
	var tmp = 0;
	while (tmp < attachids.length){
		if (attachids[tmp] != attachid){
			amaf_addAttachId(attachids[tmp]);
		};
		tmp+=1;
	};
};


//-->
</script>
<style type="text/css">
	blockquote{border:solid 1px #666;}
</style>
<div id="afgrid">
<asp:HiddenField ID="hidAttachIds" runat="server" />
<asp:PlaceHolder ID="plhMessage" runat="server" />
<asp:placeholder id="plhContent" runat="server" />

<div id="divPreviewWindow" class="afpreview"><div class="afpreviewbar"><img src="<%=ImagePath%>/images/delete_new.gif" align="right" onclick="closePreview();" /><%=PreviewText%></div><div id="divPreviewText" class="afpreviewtext"></div></div>
<am:callback ID="cbPreview" runat="server" OnCallbackComplete="cbPreview_render">
	<Content>
		<asp:HiddenField ID="hidPreviewText" runat="server" />

	</Content>
</am:callback>
</div>

<div id="amMsgBox" class="amMsgBox" style="display:none;position:absolute;">
	<div class="amMsgBoxHeader" id="amMsgBoxHeader">
		<div id="amMsgBoxHeaderText" style="float:left"></div>
		<div onclick="amaf_MsgBoxClose();" style="text-align:right;padding-right:2px;cursor:pointer;"><img src="<%=Page.ResolveUrl("~/DesktopModules/ActiveForums/images/close.gif")%>" alt="Close" /></div>
	</div>
	<div class="amMsgBoxText" id="amMsgBoxText"></div>
</div>