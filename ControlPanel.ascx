<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ControlPanel.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.ControlPanel" %>
<%@ Register  assembly="DotNetNuke.Modules.ActiveForums" namespace="DotNetNuke.Modules.ActiveForums.Controls" tagPrefix="am" %>
<script type="text/javascript">
	var imgPath = '<%=Page.ResolveUrl("~/DesktopModules/ActiveForums/images/")%>';
	var dlgHeight;
	var dlgWidth;
	var dlgTitle;
	var selectedTab;
	function getSelectedTab(){
		return selectedTab;
	};
	function setSelectedTab(id){
		selectedTab = id;
	}

	function LoadView(param,opt,tab){
		closeAllProp();
		af_clearLoad();
		af_showLoad();
		selectedTab = '';
		if (opt==undefined){
			opt = '';
		};
		if (typeof(tab)!='undefined') {
			History.pushState(null, null, '?cpview=' + param + '&params=' + opt + '&tab=' + tab);
		}else{
			History.pushState(null, null, '?cpview=' + param + '&params=' + opt);
		}

		<%=cbShell.ClientID%>.Callback(param,opt); 
	}
	function getTop(obj,height){
		var dlg=document.getElementById(obj);
		var winH=document.body.clientHeight;
		var top=((winH/2)-(height/2));
		dlg.style.top=top+'px';
	};
	function getLeft(obj,width){
		var dlg=document.getElementById(obj);
		var winW=document.body.offsetWidth;
		dlg.style.left=((winW/2)-(width/2))+'px';
	};

 function afam_showDialog(title,key,height,width,optional){
		dlgHeight=height;
		dlgWidth=width;
		dlgTitle=title;
		getTop("amModal",height);
		getLeft("amModal",width);
			   var modal = document.getElementById("amModal");
		var modalFrameDiv = document.getElementById("amModalFrameDiv");
		modal.style.zIndex=200001;
		modal.style.height=dlgHeight+'px';
		modal.style.width=dlgWidth+'px';  
		modal.style.display='block';
		modalFrameDiv.style.height=height-22+'px';
		amcp.UI.LoadMask();
		var amModalHeader=document.getElementById("amModalHeaderText");
		amModalHeader.innerHTML=dlgTitle;
		if(optional=='undefined'){optional=''};
		 var modFrame = document.getElementById("amModalFrame");
			if (modFrame != undefined){
				modFrame.height = '0';
			};
		<%=cbModal.ClientID%>.Callback('load',key,optional)

	};

	function amaf_closeDialog(){
		var modFrame = document.getElementById("amModalFrame");
		if (modFrame != undefined){
			modFrame.height = '0';
		   modFrame.parentNode.removeChild(modFrame);
		}
		var dlg=document.getElementById("amModal");
		dlg.style.display='none';
		var cModal = document.getElementById("<%=cbModal.ClientID%>");
		cModal.removeChild(cModal.firstChild);
	   amcp.UI.ClearMask();


	};
	function af_getShell(){
		return '<%=cbShell.ClientID%>';
	};
	function amClearTab(){
		selectedTab = '';
	};
	function af_shellComplete(){
		af_clearLoad(500);
		//toggleTab(selectedTab);
		var tab = getQueryString()["tab"];
		if (typeof(tab) != 'undefined') {
			toggleTab(document.getElementById(tab));
		}
	};
	function af_shellInit(){

		if (window.location.toString().indexOf('#') >= 0) {

			var sHash = window.location.hash.substring(1) + '|';
			var params = sHash.split('|');
			var view = params[0].split("=")[1];
			var param = params[1];
			if (param.indexOf('!') >= 0) {
				param = param.replace('params=', '');
			} else {
				param = param.split("=")[1];
			};
			LoadView(view, param);
		}else {
			var view = getQueryString()["cpview"];
			var parms = getQueryString()["params"];
			LoadView(view, parms);
		};

	};
	function af_setSession(){
	 if (window.location.toString().indexOf('#') >= 0) {
			var sHash = window.location.hash.substring(1) + '|';
			var params = sHash.split('|');
			var view = params[0].split("=")[1];
			var param = params[1];
			if (param.indexOf('!') >= 0) {
				param = param.replace('params=', '');
			} else {
				param = param.split("=")[1];
			};
		   <%=cbShell.ClientID%>.Callback(view,param,1);
		};

	};

</script>



<div>
<div class="amcpcontainer" id="amcpcontainer">
	<div class="amcploader" id="amcploader">
		<span>[RESX:PleaseWait]</span>
		<img src="~/desktopmodules/activeforums/images/spinner-lg.gif" runat="server" />
	</div>
	<div id="amcpnotify" class="amcpnotify">
		<div>
			<i></i>
			<span id="amcpnotify-message"></span>
		</div>      
	</div>

	<div class="amnavbar">
		<div class="amcpmdtoolbar">
			<am:imagebutton id="btnHome" runat="server" Height="50" Width="55" PostBack="False" ClientSideScript="LoadView('home');" ImageLocation="TOP" text="[RESX:Dashboard]" ImageUrl="~/DesktopModules/ActiveForums/images/home32.png" />
			<am:imagebutton id="btnForums" runat="server" Height="50" Width="50" PostBack="False" ClientSideScript="LoadView('manageforums');" ImageLocation="TOP" text="[RESX:Forums]" ImageUrl="~/DesktopModules/ActiveForums/images/forums32.png" />
			<am:imagebutton id="btnTemplates" runat="server" Height="50" Width="50" PostBack="False" ClientSideScript="LoadView('templates');" ImageLocation="TOP" text="[RESX:Templates]" ImageUrl="~/DesktopModules/ActiveForums/images/templates32.png" />
			<am:imagebutton id="btnFilters" runat="server" Height="50" Width="50" PostBack="False" ClientSideScript="LoadView('filters');" ImageLocation="TOP" text="[RESX:Filters]" ImageUrl="~/DesktopModules/ActiveForums/images/filters32.png" />
			<am:imagebutton id="btnRanks" runat="server" Height="50" Width="50" PostBack="False" ClientSideScript="LoadView('ranks');" ImageLocation="TOP" text="[RESX:Ranks]" ImageUrl="~/DesktopModules/ActiveForums/images/ranks32.png" />
			<am:imagebutton id="btnTags" runat="server" Height="50" Width="50" PostBack="False" ClientSideScript="LoadView('tags');" ImageLocation="TOP" text="[RESX:Tags]" ImageUrl="~/DesktopModules/ActiveForums/images/tags32.png" />
			<am:imagebutton id="btnCategories" runat="server" Height="50" Width="50" PostBack="False" ClientSideScript="LoadView('categories');" ImageLocation="TOP" text="[RESX:Categories]" ImageUrl="~/DesktopModules/ActiveForums/images/categories32.png" />
			<am:imagebutton id="btnReturn" runat="server" Height="50" Width="50" PostBack="False" ImageLocation="TOP" text="[RESX:Exit]" ImageUrl="~/DesktopModules/ActiveForums/images/return32.png" />
		</div>
	</div>
	<am:CallBack id="cbShell" runat="server" Debug="false" CssClass="amcpshell" OnCallbackComplete="af_shellComplete">
		<Content>
			<asp:PlaceHolder id="plhControlPanel" runat="server" />
		</Content>

	  </am:CallBack>
</div>
<div class="amcpfooter">
	<table width="100%" cellpadding="0" cellspacing="0"><tr><td style="text-align:left;padding-top:2px;"><asp:Label ID="lblProd" runat="server" CssClass="amcpfootertext" /></td><td style="text-align:right;padding-top:2px;"><asp:label ID="lblCopy" runat="server" CssClass="amcpfootertext" /></td></tr></table>
</div>

<div id="amModal" class="amModal" style="display:none;position:absolute;">
<div class="amModalHeader">
	<div class="amModalHeaderText" id="amModalHeaderText"></div>
	<div class="amModalCloseImg" onclick="amaf_closeDialog();"><img src="<%=Page.ResolveUrl("~/DesktopModules/activeforums/images/close.gif")%>" alt="[RESX:Close]" /></div>
</div>
<div class="amModalFrame" id="amModalFrameDiv">
	<am:Callback ID="cbModal" runat="server" Debug="false">
		<Content>
			<asp:PlaceHolder ID="plhModal" runat="server" />
		</Content>
	</am:Callback>

</div>

</div>
<am:scriptloader id="amScriptLoader" TextSuggest="true" Callback="true" runat="server" ActiveGrid="true" RequiredFieldValidator="true" />
 <am:imagebutton ID="btnDummy" runat="server" Visible="false" />
<div id="amtip" class="amtip" style="display:none;">
	 <div id="amtipbubble" class="amtipbubble">
		<div id="amtiptext" class="amtiptext"></div>
	</div>
</div>
</div>
<div id="amProp">
</div>
<script type="text/javascript">
	(function (window, undefined) {
		var History = window.History;
		if (!History.enabled) {
			return false;
		}
		History.Adapter.bind(window, 'statechange', function () { // Note: We are using statechange instead of popstate
			var State = History.getState(); // Note: We are using History.getState() instead of event.state
			if (State != undefined) {
				afHistoryChange(State.url, State.data);
			}
		});
	})(window);
	af_shellInit();

</script>