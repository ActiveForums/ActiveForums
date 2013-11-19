<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="admin_properties.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.admin_properties" %>
<script type="text/javascript">
	var asprop = {};
	var asproplist = {};
	function asadmin_saveProperty() {
		var tmp = asadmin_getPropertyInput();
		if (typeof (tmp) != 'undefined') {
			tmp.action = 2;
			asadmin_callback(JSON.stringify(tmp), asadmin_savePropertyComplete);
		};
	};
	function asadmin_savePropertyComplete(sender, result) {
		asClient.DetachEvent('OnAdminHelperComplete', 'asadmin_savePropertyComplete');
		asadmin_cancelPropForm();
		alert(window.location.hash);
		asadmin_getProperties();
	};
	function asadmin_getPropertyInput() {
		var pname = asGet('txtPropertyName');
		if (pname.trim() == '') {
			return;
		};
		var dtype = asGet('drpDataType').value;
		var dacl = asGet('drpDefaultACL').value;
		var phid = false;
		if (asGet('propHiddenYes').checked) {
			phid = true;
		};
		var pro = false;
		if (asGet('propReadOnlyYes').checked) {
			pro = true;
		};
		var preq = false;
		if (asGet('propReqYes').checked) {
			preq = true;
		};
		var propid = -1;
		var psort = -1;
		if (asGet('hidPropertyId') != '') {
			propid = asGet('hidPropertyId');
		};
		var lbl = asGet('txtLabel');
		var prex = asGet('txtValExp');
		var petemp = asGet('txtEditTemplate');
		var pvtemp = asGet('txtViewTemplate');
		psort = asGet('hidSortOrder');
		var pdv = asGet('txtDefaultValue');
		asprop.Name = pname;
		asprop.DataType = dtype;
		asprop.IsHidden = phid;
		asprop.IsReadOnly = pro;
		asprop.IsRequired = preq;
		asprop.ValidationExpression = encodeURI(escape(prex));
		asprop.EditTemplate = petemp;
		asprop.ViewTemplate = pvtemp;
		asprop.DefaultAccessControl = dacl;
		asprop.ObjectType = 1;
		asprop.ObjectOwnerId = -1;
		asprop.PropertyId = propid;
		asprop.SortOrder = psort;
		asprop.DefaultValue = pdv;
		asprop.Label = lbl;
		return asprop;

	};
	function asadmin_getProperties() {
		asadmin_cancelPropForm()
		asClient.DetachEvent('OnAdminHelperComplete', 'asadmin_getProperties');
		var req = {};
		req.action = 4;
		req.ObjectType = 1;
		req.ObjectOwnerId = -1;
		asadmin_callback(JSON.stringify(req), asadmin_buildProperties);
	};
	function asadmin_buildProperties(sender, result) {
		asClient.DetachEvent('OnAdminHelperComplete', 'asadmin_buildProperties');
		asproplist = result;
		var ul = document.getElementById('proplist');
		var licur = as_getElementsByClassName('candrag');
		var cn = licur.length;
		for (var x = 0; x < cn; x++) {
			var el = licur[x];
			var p = el.parentNode;
			if (typeof (p) != 'undefined') {
				p.removeChild(el);
			};
		};
		var p = {};
		for (var i = 0; i < asproplist.length; i++) {
			p = asproplist[i];
			var l = createLI(p);
			ul.appendChild(l);
			selectSetSelected('acl-' + p.PropertyId, p.DefaultAccessControl);
		};
		var options = {
			itemHoverClass: 'myItemHover',
			dragTargetClass: 'myDragTarget',
			dropTargetClass: 'myDropTarget',
			useDefaultDragHandle: true
		};
		var lists = jQuery('#proplist').ListReorder(options);
		lists.bind('listorderchanged', function (evt, jqList, listOrder) {
			var props = '';
			for (var i = 0; i < listOrder.length; i++) {
				var idx = listOrder[i];
				var asprop = asproplist[idx];
				if (asprop.SortOrder != i) {
					asproplist[idx]['SortOrder'] = i;
					props += asprop.PropertyId + '|' + i + '^';
				};
			};
			if (props.length > 0) {
				var tmp = {};
				tmp.action = 5;
				tmp.props = props;
				asadmin_callback(JSON.stringify(tmp), asadmin_sortSaveComplete);
			};

		});

	};
	function asadmin_sortSaveComplete(sender, result) {
		asClient.DetachEvent('OnAdminHelperComplete', 'asadmin_sortSaveComplete');
		if (typeof (result.message) != 'undefined') {
			alert(result.message);
		};

	};

	function asadmin_cancelPropForm() {
		asprop = {};
		asadmin_resetForm('ampropform');
		if (document.getElementById('propeditor').style.display == 'block') {
			asCloseDiv('propeditor');
		};

	};
	function asadmin_resetForm(id) {
		var container = document.getElementById(id);
		if (typeof (container) != 'undefined') {
			var elements = container.getElementsByTagName('input');
			for (var i = 0; i < elements.length; i++) {
				if (elements[i].type == 'text') {
					elements[i].value = '';
				} else if (elements[i].type == 'radio') {
					if (elements[i].getAttribute('isdefault')) {
						elements[i].checked = true;
					} else {
						elements[i].checked = false;
					};
				} else if (elements[i].type == 'checkbox') {
					if (elements[i].getAttribute('isdefault')) {
						elements[i].checked = true;
					} else {
						elements[i].checked = false;
					};
				};
			};
			var elements = container.getElementsByTagName('select');
			for (var i = 0; i < elements.length; i++) {
				elements[i].selectedIndex = 0;
			};


		};
	};
	function createLI(prop) {
		var l = document.createElement('li');
		l.setAttribute('id', prop.PropertyId);
		l.setAttribute('class', 'asclear candrag');
		var dr = document.createElement('div');
		dr.setAttribute('class', 'propname asclear');
		var a = document.createElement('a');
		a.appendChild(document.createTextNode(prop.Name));
		a.onclick = function () { asadmin_loadPropForm(this); };
		dr.appendChild(a);
		var ul = document.createElement('ul');
		ul.setAttribute('class', 'aslistflat asclear');
		var la = document.createElement('li');
		la.appendChild(document.createTextNode('[RESX:IsHidden]'));
		if (prop.IsHidden == true) {
			la.setAttribute('class', 'checked');
		};
		ul.appendChild(la);
		la = document.createElement('li');
		la.appendChild(document.createTextNode('[RESX:IsRequired]'));
		if (prop.IsRequired == true) {
			la.setAttribute('class', 'checked');
		};
		ul.appendChild(la);
		la = document.createElement('li');
		la.appendChild(document.createTextNode('[RESX:IsReadOnly]'));
		if (prop.IsReadOnly == true) {
			la.setAttribute('class', 'checked');
		};
		ul.appendChild(la);
		dr.appendChild(ul);
		l.appendChild(dr);
		dr = document.createElement('div');
		dr.setAttribute('class', 'acl');
		var select = document.createElement('select');
		select.setAttribute('id', 'acl-' + prop.PropertyId);
		l.appendChild(select);
		as_addOption('', '[RESX:Everyone]', 0, select);
		as_addOption('', '[RESX:SiteMembers]', 1, select);
		as_addOption('', '[RESX:GroupMembers]', 2, select);
		as_addOption('', '[RESX:GroupAdmins]', 3, select);
		as_addOption('', '[RESX:SiteAdmins]', 4, select);
		return l;
	};
	function asadmin_deleteProp() {
		if (confirm('[RESX:WARN:DeleteProperty]')) {
			var tmp = {};
			tmp.action = 3;
			tmp.propertyid = asGet('hidPropertyId');
			asadmin_callback(JSON.stringify(tmp), asadmin_getProperties);
		};
	};
	function asadmin_loadPropForm(obj) {
		var objNode = obj.parentNode;
		var count = 1;
		while (objNode.tagName != 'LI' && count < 100) {
			objNode = objNode.parentNode;
			count++;
		};
		for (var prop in asproplist) {
			if (asproplist[prop].PropertyId == objNode.id) {
				asprop = asproplist[prop];

			};
		};

		if (typeof (asprop) != 'undefined') {
			asV('hidPropertyId').value = asprop.PropertyId;
			asV('txtPropertyName').value = asprop.Name;
			asV('txtEditTemplate').value = asprop.EditTemplate;
			asV('txtViewTemplate').value = asprop.ViewTemplate;
			asV('txtValExp').value = decodeURI(unescape(asprop.ValidationExpression));
			asV('txtDefaultValue').value = asprop.DefaultValue;
			asV('hidSortOrder').value = asprop.SortOrder;
			asV('txtLabel').value = asprop.Label;
			selectSetSelected('drpDefaultACL', asprop.DefaultAccessControl);
			selectSetSelected('drpDataType', asprop.DataType);

			if (asprop.IsHidden == true) {
				asV('propHiddenNo').checked = false;
				asV('propHiddenYes').checked = true;
			};
			if (asprop.IsReadOnly == true) {
				asV('propReadOnlyNo').checked = false;
				asV('propReadOnlyYes').checked = true;
			};
			if (asprop.IsRequired == true) {
				asV('propReqNo').checked = false;
				asV('propReqYes').checked = true;
			};

			asLoadDiv('propeditor', '[RESX:AddProperty]');
		};
	};
</script>
<div class="amcpsubnav">
	<div onclick="LoadView('groupslist','');" class="amcplnkbtn"></div>
</div>
<div class="amcpcontrols" style="background-color: #fff;">
	<div class="amcphd"><div class="ashelp" onclick="return asLoadHelp(this,'KBGRPPRLST','[RESX:GroupProperties]');" title="[RESX:Help]"></div>[RESX:GroupProperties]</div>
	<div class="amcpintro">[RESX:GroupsProperties:Intro]</div>
	<div><a href="" onclick="asLoadDiv('propeditor','[RESX:AddProperty]');return false;" class="btnadd asroundall">[RESX:AddProperty]</a></div>
	<ul id="proplist">
		<li class="asclear prophead"><div style="float:left;">[RESX:Properties]</div><div style="width:150px;float:right;">[RESX:Visibility]</div></li>

	</ul>

</div>
<div style="display:none;width:500px;height:450px;position:absolute;padding:0;margin:0;" id="propeditor" class="asmodalform">
<table width="100%" cellpadding="0" cellspacing="0">
		<tr>
			<td class="afdlg-ul"></td>
			<td class="afdlg-um"><img id="Img3" src="~/desktopmodules/activesocial/admin/images/close.gif" onclick="asadmin_cancelPropForm();" runat="server" border="0" style="padding-top:5px;float:right;cursor:pointer;" /><div id="propeditor_header" style="float:left;padding:3px;padding-top:8px;padding-left:10px;font-weight:bold;">[RESX:PropertyEditor]</div></td>
			<td class="afdlg-ur"></td>
		</tr>
		<tr>
			<td class="afdlg-ml"></td>
			<td style="background-color:#f5f5f5;">
				<ul class="amcpformlist asclear ampropeditor" id="ampropform">
					<li><label>[RESX:PropertyName]:</label><input type="text" id="txtPropertyName" title="[RESX:PropertyName]" required="true" value="" onkeypress="return filterInput(this,event,'txtLabel');" /></li>
					<li><label>[RESX:Label]:</label><input type="text" id="txtLabel" title="[RESX:Label]" required="true" value="" /></li>
					<li><label>[RESX:DataType]:</label><select id="drpDataType"><option value="text">[RESX:Text]</option></select></li>
					<li><label>[RESX:PropertyDefaultAccessControl]:</label><select id="drpDefaultACL"><option value="0">[RESX:Everyone]</option><option value="1">[RESX:SiteMembers]</option><option value="2">[RESX:GroupMembers]</option><option value="3">[RESX:GroupAdmins]</option><option value="4">[RESX:SiteAdmins]</option></select></li>
					<li><label>[RESX:IsHidden]:</label><div class="amcpradiorow"><span>[RESX:No]</span><input type="radio" name="prophidden" isdefault="true" value="false" id="propHiddenNo" checked="checked" /><span>[RESX:Yes]</span><input type="radio" name="prophidden" value="true" id="propHiddenYes" /></div></li>
					<li><label>[RESX:IsReadOnly]:</label><div class="amcpradiorow"><span>[RESX:No]</span><input type="radio" name="propreadonly" isdefault="true" value="false" id="propReadOnlyNo" checked="checked" /><span>[RESX:Yes]</span><input type="radio" name="propreadonly" value="true" id="propReadOnlyYes" /></div></li>
					<li><label>[RESX:IsRequired]:</label><div class="amcpradiorow"><span>[RESX:No]</span><input type="radio" name="propreq" isdefault="true" value="false" id="propReqNo" checked="checked" /><span>[RESX:Yes]</span><input type="radio" name="propreq" value="true" id="propReqYes" /></div></li>
					<li><label>[RESX:ValidationExpression]:</label><input type="text" id="txtValExp" value="" /></li>
					<li style="display:none;"><label>[RESX:DefaultValue]:</label><input type="text" id="txtDefaultValue" value="" /></li>
					<li style="display:none;"><label>[RESX:EditTemplate]:</label><input type="text" id="txtEditTemplate" value="" /></li>
					<li style="display:none;"><label>[RESX:ViewTemplate]:</label><input type="text" id="txtViewTemplate" value="" /></li>
				</ul>
				<div class="asbuttonarea">
					<input type="button" title="[RESX:Save]" value="[RESX:Save]" id="btnSave" class="asbtn act" onclick="asadmin_saveProperty();" />
					<input type="button" title="[RESX:Delete]" value="[RESX:Delete]" onclick="asadmin_deleteProp();" id="btnDelete" class="asbtn dim" />
					<input type="button" title="[RESX:Cancel]" value="[RESX:Cancel]" onclick="asadmin_cancelPropForm();" id="btnCancel" class="asbtn dim" />
				</div>
				<input type="hidden" id="hidPropertyId" value="" />
				<input type="hidden" id="hidSortOrder" value="" />
			 </td>
			<td class="afdlg-mr"></td>
		</tr>
		<tr>
			<td class="afdlg-bl"></td>
			<td class="afdlg-bm"></td>
			<td class="afdlg-br"></td>
		</tr>
	</table>
</div>
<script type="text/javascript">

	jQuery(document).ready(function () {
		asadmin_getProperties();


		jQuery(function () {
			$.extend($.fn.disableTextSelect = function () {
				return this.each(function () {
					if ($.browser.mozilla) {//Firefox
						$(this).css('MozUserSelect', 'none');
					} else if ($.browser.msie) {//IE
						$(this).bind('selectstart', function () { return false; });
					} else {//Opera, etc.
						$(this).mousedown(function () { return false; });
					}
				});
			});
			$('.noSelect').disableTextSelect(); //No text selection on elements with a class of 'noSelect'
		});
	});
</script>