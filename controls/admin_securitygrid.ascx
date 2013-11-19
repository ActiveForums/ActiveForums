<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="admin_securitygrid.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.Controls.admin_securitygrid" %>
<%@ Register TagPrefix="am" Namespace="DotNetNuke.Modules.ActiveForums.Controls" Assembly="DotNetNuke.Modules.ActiveForums" %>
<script type="text/javascript">
    
    function addObject(type){
        currAction = 'addobject';
        if (type == 0){
            var ddlRole = document.getElementById("drpSecRoles");
            var objectName = ddlRole.options[ddlRole.selectedIndex].text;
            var objectId = ddlRole.options[ddlRole.selectedIndex].value;
            if (objectId !=''){
                ddlRole.selectedIndex = 0;
                securityAddObject(<%=PermissionsId%>,objectId,objectName,0);
            };

        }else if (type == 1){
            var objectName = document.getElementById("<%=txtUserName.ClientID%>").value;
            if (objectName != ''){
                securityAddObject(-1,objectName,objectName,1);

            };
        };
};

var rebuild = false;

function securityDelObject(obj,oid,otype,pid){
    if(confirm('[RESX:Actions:DeleteConfirm]')){
        rebuild = true;
        af_showLoad();
        securityCallback('delete', -1, pid, oid, '', otype, '', securityToggleComplete);
    };
};

function securityAddObject(pid,secId,secName,secType){
    af_showLoad();
    selectedTab = 'divSecurity';
    securityCallback('addobject', -1, pid, secId, secName, secType, '', securityToggleComplete);      
    rebuild = true;
};

function securityToggleComplete(data){
    if (data.length > 2){
        var cellId = document.getElementById(data.split('|')[1]);
        var action = data.split('|')[0];
        var img = cellId.firstChild;
        if (action == 'remove'){
            img.src = imgOff.src
            img.setAttribute('alt','Disabled');
        }else{
            img.src = imgOn.src
            img.setAttribute('alt','Enabled');
        };
    };
    if (rebuild){
        rebuild = false;
        var currview = getQueryString()["cpview"];
        var currparms = getQueryString()["params"];
        LoadView(currview,currparms,'divSecurity');
    };
};

function secGridComplete(){
    af_clearLoad();
};

function securityCallback(action, returnId, pid, secId,secName, secType, key, callback) {
    var data = {};
    data.ModuleId = <%=ModuleId%>;
    data.Action = action;
    data.PermissionsId = pid;
    data.SecurityId = secId;
    data.SecurityType = secType;
    data.SecurityKey = key;
    data.ReturnId = returnId;
    var sf = $.ServicesFramework(<%=ModuleId%>);
    //sf.getAntiForgeryProperty(data);
    
    $.ajax({
        type: "POST",
        url: sf.getServiceRoot('ActiveForums') + "AdminService/ToggleSecurity",
        beforeSend: sf.setModuleHeaders,
        data: data,
        success: function (data) {
            callback(data);
        },
        error: function (xhr, status, error) {
            alert(error);
        }
    });
};

function securityToggle(obj,pid,secId,secName,secType,key) {
    var returnId = obj.id;
    var img = obj.firstChild;
    if (img.src == imgOn.src){
        currAction = 'remove';
    }else{
        currAction = 'add';
    };
    img.src = imgSpin.src;
    img.setAttribute('alt','Please Wait');
    securityCallback(currAction, returnId, pid, secId, secName, secType, key, securityToggleComplete);           
};

</script>
<div id="gridActions" runat="server">
    <table cellpadding="0" cellspacing="0" width="100%" style="padding-bottom: 4px;">

        <tr>
            <td class="amroles">
                <table cellpadding="0" cellspacing="2">
                    <tr>
                        <td style="width: 12px;">
                            <img id="Img43" src="~/DesktopModules/ActiveForums/images/tooltip.png" runat="server" onmouseover="amShowTip(this, '[RESX:Tips:AddRoles]');" onmouseout="amHideTip(this);" /></td>
                        <td>[RESX:Roles]:</td>
                        <td style="width: 150px;">
                            <asp:Literal ID="litRoles" runat="server" /></td>
                        <td style="width: 16px;">
                            <div class="amcpimgbtn" style="width: 16px;" onclick="addObject(0);">
                                <img id="Img41" src="~/desktopmodules/activeforums/images/add.png" runat="server" alt="[RESX:AddRole]" />
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 5%"></td>
            <td class="amroles" align="right">
                <div style="display: none;">
                    <table cellpadding="0" cellspacing="2">
                        <tr>
                            <td style="width: 12px;">
                                <img id="Img44" src="~/DesktopModules/ActiveForums/images/tooltip.png" runat="server" onmouseover="amShowTip(this, '[RESX:Tips:AddUser]');" onmouseout="amHideTip(this);" /></td>
                            <td>[RESX:UserName]:</td>
                            <td style="width: 150px;">
                                <asp:TextBox ID="txtUserName" runat="server" CssClass="amcptxtbx" Width="150" /></td>
                            <td style="width: 16px;">
                                <div class="amcplnkbtn" style="width: 16px;" onclick="addObject(1);">
                                    <img id="Img42" src="~/desktopmodules/activeforums/images/add.png" runat="server" alt="[RESX:AddUserName]" />
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>

            </td>
        </tr>
    </table>
</div>
<div class="afsecgrid">

    <am:callback id="cbSecGrid" runat="server" oncallbackcomplete="secGridComplete">
					<Content>
						<asp:Literal ID="litSecGrid" runat="server" />
					</Content>
				</am:callback>

</div>

<div style="display: none;">
    <am:callback id="cbSecurityToggle" runat="server" oncallbackcomplete="securityToggleComplete">
				<Content></Content>
			</am:callback>
</div>

