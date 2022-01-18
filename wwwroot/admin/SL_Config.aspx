<%@ Page Title="Experiment | Admin Area | SL | Configuration Manager" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="SL_Config.aspx.cs" Inherits="admin_SL_Config" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph_head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ph_header_text" Runat="Server">
    <h1>SL - Configuration Manager</h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ph_body" Runat="Server">
    <div id="div_admin" class="">        
        <h3>SL Configuration</h3>
                
        <div class="adminSessionActions">
            <asp:Button runat="server" ID="cmdSLConfigSummary" CssClass="button block h-center wide" Text="Configuration Summary" OnClick="cmdSLConfigSummary_Click" />
            <asp:Button runat="server" ID="cmdSLConfigCreate" CssClass="button block h-center wide" Text="Create Configuration" OnClick="cmdSLConfigCreate_Click" />
        </div>        
                
        <asp:Button runat="server" ID="cmdBack" CssClass="button block h-center wide" Text="Back" OnClick="cmdBack_Click" />
    </div>
</asp:Content>