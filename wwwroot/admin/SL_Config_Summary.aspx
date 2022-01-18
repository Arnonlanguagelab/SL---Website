<%@ Page Title="Experiment | Admin Area | SL | Configuration Summary" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="SL_Config_Summary.aspx.cs" Inherits="admin_SL_Config_Summary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph_head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ph_header_text" Runat="Server">
    <h1>SL - Configuration Summary</h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ph_body" Runat="Server">
    <div id="div_admin" class="">
        <asp:Button runat="server" ID="cmdBack" CssClass="button block h-center narrow" Text="Back" OnClick="cmdBack_Click" CausesValidation="false" />
        <asp:Panel runat="server" ID="pnlConfigItems" CssClass="configItems" />
    </div>
</asp:Content>

