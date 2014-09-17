<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="af_searchadvanced.ascx.cs" Inherits="DotNetNuke.Modules.ActiveForums.af_searchadvanced" %>
<div class="af-adv-search">
    <div class="af-adv-search-box">
        <div class='af-adv-search-header'><asp:Literal runat="server" ID="litOptions" /></div>
        <div class="af-adv-search-row">
            <asp:Label runat="server" ID="lblSearch" AssociatedControlID="txtSearch" />
            <asp:TextBox runat="server" ID="txtSearch" />
            <asp:DropDownList runat="server" ID="drpSearchColumns" >
                <asp:ListItem Text="Subject & Topic" Value="0" />
                <asp:ListItem Text="Subject Only" Value="1" />
                <asp:ListItem Text="Topic Only" Value="2" />
            </asp:DropDownList>
            <asp:DropDownList runat="server" ID="drpSearchType">
                <asp:ListItem Text="ANY Keyword" Value="0" />
                <asp:ListItem Text="ALL Keywords" Value="1" />
                <asp:ListItem Text="Exact Match" Value="2" />
            </asp:DropDownList>
        </div>
        <div class="af-adv-search-row">
            <asp:Label runat="server" ID="lblUserName" AssociatedControlID="txtUserName" />
            <asp:TextBox ID="txtUserName" runat="server" />
        </div>
        <div class="af-adv-search-row">
            <asp:Label runat="server" ID="lblTags" AssociatedControlID="txtTags" />
            <asp:TextBox runat="server" ID="txtTags" />
        </div>
        <div class="af-adv-search-footer">
            <span class="af-search-input-error"><asp:Literal runat="server" ID="litInputError" /></span>
            <asp:Button runat="server" ID="btnSearch" Text="Search" />
            <button runat="server" id="btnReset" type="reset" />
        </div>
    </div>
    <div class="af-adv-search-box">
        <div class='af-adv-search-header af-adv-search-header-collapse'><asp:Literal runat="server" ID="litAdditionalOptions" />&nbsp;<span class="ui-icon ui-icon-triangle-1-n"></span></div>
        <div class="af-adv-search-row">
            <asp:Label runat="server" ID="lblForums" AssociatedControlID="lbForums" />
            <asp:ListBox runat="server" ID="lbForums" CssClass="af-adv-search-list" SelectionMode="Multiple" Rows="6" />
        </div>
        <div class="af-adv-search-row">
            <asp:Label runat="server" ID="lblSearchDays" AssociatedControlID="drpSearchDays" />
            <asp:DropDownList ID="drpSearchDays" runat="server" />
        </div>
        <div class="af-adv-search-row">
            <asp:Label runat="server" ID="lblResultType" AssociatedControlID="drpResultType" />
            <asp:DropDownList runat="server" ID="drpResultType">
                <asp:ListItem Text="Topics" Value="0" />
                <asp:ListItem Text="Posts" Value="1" />
            </asp:DropDownList>
        </div>
        <div class="af-adv-search-row">
            <asp:Label runat="server" ID="lblSortType" AssociatedControlID="drpSort" />
            <asp:DropDownList runat="server" ID="drpSort">
                <asp:ListItem Text="Relevance" Value="0" />
                <asp:ListItem Text="Post Date" Value="1" />
            </asp:DropDownList>
        </div>
        <div class="af-adv-search-footer">
            <asp:Button runat="server" CssClass="afbtn-b" ID="btnSearch2" Text="Search" />
            <button runat="server" class="afbtn" id="btnReset2" type="reset" />
        </div>
    </div>
</div>

<script type="text/javascript">

    $(document).ready(function() {
        $('#<%= lbForums.ClientID %>').afForumSelector();
        $('.af-adv-search-footer').find('input:submit').each(function() { $(this).replaceWith('<button type="submit" name="' + $(this).attr('name') + '" class="' + $(this).attr('class') + '" id="' + $(this).attr('id') + '" >' + $(this).val() + '</button>');});
        $('.af-adv-search-footer :submit').button({ icons: { primary: "ui-icon-search" } }).click(function (e) {
            if (!$('#<%=txtSearch.ClientID%>').val() && !$('#<%=txtUserName.ClientID%>').val() && !$('#<%=txtTags.ClientID%>').val()) {
                $('.af-search-input-error').show().delay(1500).fadeOut('slow');
                return false;
            } 
        });
        $('.af-adv-search :text').keypress(function (event) { if (event.keyCode == 13) { $('#<%= btnSearch.ClientID%>').click(); } });
        $('.af-adv-search-footer :reset').button({ icons: { primary: "ui-icon-refresh" } });
        $('.af-adv-search-header-collapse').click(function() { $(this).siblings().toggle();  $(this).find(".ui-icon").toggleClass("ui-icon-triangle-1-n ui-icon-triangle-1-s"); });
    });

</script>