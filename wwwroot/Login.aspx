<%@ Page Title="המעבדה לרכישת שפה ועיבוד שפה | כניסה למנהלים" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph_head" Runat="Server">
    <script src='https://www.google.com/recaptcha/api.js'></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ph_header_text" Runat="Server">
    <h1>כניסה למנהלים</h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ph_body" Runat="Server">
    <div id="div_login" class="div_login">
        <div runat="server" id="divLogin">
            <p>
                <asp:TextBox runat="server" ID="txtUserID" CssClass="textbox block h-center txt-center ltr" CausesValidation="true" TextMode="SingleLine" MaxLength="20" placeholder="שם משתמש" />
                <asp:TextBox runat="server" ID="txtUserPW" CssClass="textbox block h-center txt-center ltr" AutoCompleteType="None" CausesValidation="true" TextMode="Password" MaxLength="20" placeholder="סיסמה" />
                <asp:Button runat="server" ID="cmdLogin" CssClass="button block h-center" Text="כניסה" OnClick="cmdLogin_Click" />
            </p>
            <p>
                <asp:Label runat="server" ID="lblErrorMsg" CssClass="errMessage ltr" />
            </p>
            <div class="g-recaptcha" data-sitekey="6LdyixoUAAAAAG8PYwtXFCO7C2Xr11afZwDZ906w"></div>
        </div>
    </div>
    
    <asp:Label runat="server" ID="lblBackTo" Text="" Visible="false" />
</asp:Content>

