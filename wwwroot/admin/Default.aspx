<%@ Page Title="Experiment | Admin Area" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="admin_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph_head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ph_header_text" Runat="Server">
    <h1>Admin Area</h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ph_body" Runat="Server">
    <div id="div_admin" class="">        
        <h3>SL Experiment</h3>
                
        <div class="adminSessionActions">
            <asp:Button runat="server" ID="cmdSLData" CssClass="button block h-center wide" Text="Data Reports" OnClick="cmdSLData_Click" />
            <asp:Button runat="server" ID="cmdSLConfig" CssClass="button block h-center wide" Text="Configuration Manager" OnClick="cmdSLConfig_Click" />
        </div>        
                
        <asp:Button runat="server" ID="cmdLogout" CssClass="button block h-center wide" Text="Logout" OnClick="cmdLogout_Click" />
    </div>
</asp:Content>

