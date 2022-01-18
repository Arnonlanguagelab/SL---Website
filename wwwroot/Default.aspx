<%@ Page Title="המעבדה לרכישת שפה ועיבוד שפה | מערכת ניסויים" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph_head" Runat="Server">  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ph_header_text" Runat="Server">
    <h1>המעבדה לרכישת שפה ועיבוד שפה</h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ph_body" Runat="Server">
    <div id="div_main" class="">
        <h3>מערכת ניסויים</h3>
        
        <asp:Button runat="server" ID="cmdLogin" CssClass="button block h-center" Text="כניסה למנהלים" OnClick="cmdLogin_Click" />
        
        <asp:Panel runat="server" ID="pnlConfigItems" CssClass="configItems" />
    </div>
</asp:Content>