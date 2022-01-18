<%@ Page Title="Experiment | Admin Area | SL | Data" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="SL_Data.aspx.cs" Inherits="admin_SL_Data" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph_head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ph_header_text" Runat="Server">
    <h1>SL - Data</h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ph_body" Runat="Server">
    <div id="div_admin" class="">        
        <h3>Experiment Data</h3>
        <asp:Button runat="server" ID="cmdSessionsReport" CssClass="button block h-center wide" Text="Sessions" OnClick="cmdSessionsReport_Click" />
        
        <div class="adminSessionActions">
            <asp:TextBox runat="server" ID="txtSessionID" CssClass="textbox block h-center txt-center wide" MaxLength="20" placeholder="Session ID" />
            <asp:DropDownList runat="server" ID="drpExpType" CssClass="textbox block h-center txt-center wide" />
            <asp:DropDownList runat="server" ID="drpConfigID" CssClass="textbox block h-center txt-center wide" />
            
            <asp:Button runat="server" ID="cmdTripletsReport" CssClass="button block h-center wide" Text="Triplets" OnClick="cmdTripletsReport_Click" />
            <asp:Button runat="server" ID="cmdTrainingTrialsReport" CssClass="button block h-center wide" Text="Training Trials" OnClick="cmdTrainingTrialsReport_Click" />
            <asp:Button runat="server" ID="cmdTestingTrialsReport" CssClass="button block h-center wide" Text="Testing Trials" OnClick="cmdTestingTrialsReport_Click" />
            <asp:Button runat="server" ID="cmdTrainingAndTestingTrialsReport" CssClass="button block h-center wide" Text="Training & Testing Trials" OnClick="cmdTrainingAndTestingTrialsReport_Click" />
        </div>
                
        <asp:Button runat="server" ID="cmdBack" CssClass="button block h-center wide" Text="Back" OnClick="cmdBack_Click" />
    </div>
</asp:Content>

