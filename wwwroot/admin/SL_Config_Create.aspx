<%@ Page Title="Experiment | Admin Area | SL | Create Experiment Configuration" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="SL_Config_Create.aspx.cs" Inherits="admin_SL_Config_Create" ValidateRequest="false" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph_head" Runat="Server">
    <script type="text/javascript" src="/js/SL_Config.js" ></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ph_header_text" Runat="Server">
    <h1>SL - Create Experiment Configuration</h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ph_body" Runat="Server">
    <div id="div_admin" class="">
        <asp:Button runat="server" ID="cmdBack" CssClass="button block narrow" Text="Back" OnClick="cmdBack_Click" CausesValidation="false" />
        <asp:Label runat="server" ID="lblErrorMessage" CssClass="errMessage" Text="" />
        <table class="tblConfig">
            <tr>
                <td class="tdConfigHeader">Configuration Name:</td>
                <td class="tdConfig">
                    <asp:TextBox runat="server" ID="txtConfigName" CssClass="wider txt-left configInput" MaxLength="50" />
                    <asp:RequiredFieldValidator runat="server" ID="val_ConfigName_req" CssClass="configErrMessage" ControlToValidate="txtConfigName" 
                        Display="Dynamic" ErrorMessage="<% $appSettings:ErrMsgRequired %>" />
                    <asp:RegularExpressionValidator runat="server" ID="val_ConfigName_rex" CssClass="configErrMessage" ControlToValidate="txtConfigName" 
                        Display="Dynamic" ValidationExpression="<% $appSettings:RegExConfigName %>" ErrorMessage="<% $appSettings:ErrMsgConfigName %>" />
                </td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Experiment Type:</td>
                <td class="tdConfig">
                    <asp:DropDownList runat="server" ID="drpExpType" CssClass="narrow configInput">
                        <asp:ListItem Value="" Text="" Selected="True" />
                        <asp:ListItem Value="1" Text="VSL" />
                        <asp:ListItem Value="2" Text="ASL" />
                        <asp:ListItem Value="3" Text="MSL" />
                        <asp:ListItem Value="4" Text="DSL" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="val_ExpType_req" CssClass="configErrMessage" ControlToValidate="drpExpType" 
                        Display="Dynamic" ErrorMessage="<% $appSettings:ErrMsgRequired %>" />
                </td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Triplets Creation Type:</td>
                <td class="tdConfig">
                    <asp:DropDownList runat="server" ID="drpTripletsRandType" CssClass="narrow configInput">
                        <asp:ListItem Value="" Text="" Selected="True" />
                        <asp:ListItem Value="1" Text="Random" />
                        <asp:ListItem Value="2" Text="Preset" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="val_TripletsRandType_req" CssClass="configErrMessage" ControlToValidate="drpTripletsRandType" 
                        Display="Dynamic" ErrorMessage="<% $appSettings:ErrMsgRequired %>" />
                </td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Number of Triplets:</td>
                <td class="tdConfig">
                    <asp:TextBox runat="server" ID="txtNumberOfTriplets" CssClass="narrow txt-center configInput" MaxLength="2" />
                    <asp:RequiredFieldValidator runat="server" ID="val_NumberOfTriplets_req" CssClass="configErrMessage" ControlToValidate="txtNumberOfTriplets" 
                        Display="Dynamic" ErrorMessage="<% $appSettings:ErrMsgRequired %>" />
                    <asp:RegularExpressionValidator runat="server" ID="val_NumberOfTriplets_rex" CssClass="configErrMessage" ControlToValidate="txtNumberOfTriplets" 
                        Display="Dynamic" ValidationExpression="<% $appSettings:RegExNumeric %>" ErrorMessage="<% $appSettings:ErrMsgNumeric %>" />
                </td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Preset Triplets:</td>
                <td class="tdConfig">
                    <asp:TextBox runat="server" ID="txtPresetTriplets" CssClass="widest txt-left configInput" MaxLength="255" />
                    <asp:RequiredFieldValidator runat="server" ID="val_PresetTriplets_req" CssClass="configErrMessage" ControlToValidate="txtPresetTriplets" 
                        Display="Dynamic" ErrorMessage="<% $appSettings:ErrMsgRequired %>" />
                    <asp:RegularExpressionValidator runat="server" ID="val_PresetTriplets_rex" CssClass="configErrMessage" ControlToValidate="txtPresetTriplets" 
                        Display="Dynamic" ValidationExpression="<% $appSettings:RegExPresetTriplets %>" ErrorMessage="<% $appSettings:ErrMsgPresetTripletsFormat %>" />
                </td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Preset Foils:</td>
                <td class="tdConfig">
                    <asp:TextBox runat="server" ID="txtPresetFoils" CssClass="widest txt-left configInput" MaxLength="255" />
                    <asp:RequiredFieldValidator runat="server" ID="val_PresetFoils_req" CssClass="configErrMessage" ControlToValidate="txtPresetFoils" 
                        Display="Dynamic" ErrorMessage="<% $appSettings:ErrMsgRequired %>" />
                    <asp:RegularExpressionValidator runat="server" ID="val_PresetFoils_rex" CssClass="configErrMessage" ControlToValidate="txtPresetFoils" 
                        Display="Dynamic" ValidationExpression="<% $appSettings:RegExPresetTriplets %>" ErrorMessage="<% $appSettings:ErrMsgPresetTripletsFormat %>" />
                </td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Training Triplet Repetitions:</td>
                <td class="tdConfig">
                    <asp:TextBox runat="server" ID="txtTrainingTripletRepetitions" CssClass="narrow txt-center configInput" MaxLength="2" />
                    <asp:RequiredFieldValidator runat="server" ID="val_TrainingTripletRepetitions_req" CssClass="configErrMessage" ControlToValidate="txtTrainingTripletRepetitions" 
                        Display="Dynamic" ErrorMessage="<% $appSettings:ErrMsgRequired %>" />
                    <asp:RegularExpressionValidator runat="server" ID="val_TrainingTripletRepetitions_rex" CssClass="configErrMessage" ControlToValidate="txtTrainingTripletRepetitions" 
                        Display="Dynamic" ValidationExpression="<% $appSettings:RegExNumeric %>" ErrorMessage="<% $appSettings:ErrMsgNumeric %>" />
                </td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Pause between Stimuli:</td>
                <td class="tdConfig">
                    <asp:TextBox runat="server" ID="txtPauseBetweenStimuli" CssClass="narrow txt-center configInput" MaxLength="4" />
                    <asp:RequiredFieldValidator runat="server" ID="val_PauseBetweenStimuli_req" CssClass="configErrMessage" ControlToValidate="txtPauseBetweenStimuli" 
                        Display="Dynamic" ErrorMessage="<% $appSettings:ErrMsgRequired %>" />
                    <asp:RegularExpressionValidator runat="server" ID="val_PauseBetweenStimuli_rex" CssClass="configErrMessage" ControlToValidate="txtPauseBetweenStimuli" 
                        Display="Dynamic" ValidationExpression="<% $appSettings:RegExNumeric %>" ErrorMessage="<% $appSettings:ErrMsgNumeric %>" />
                </td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Stimulus Duration:</td>
                <td class="tdConfig">
                    <asp:TextBox runat="server" ID="txtStimulusDuration" CssClass="narrow txt-center configInput" MaxLength="4" />
                    <asp:RequiredFieldValidator runat="server" ID="val_StimulusDuration_req" CssClass="configErrMessage" ControlToValidate="txtStimulusDuration" 
                        Display="Dynamic" ErrorMessage="<% $appSettings:ErrMsgRequired %>" />
                    <asp:RegularExpressionValidator runat="server" ID="val_StimulusDuration_rex" CssClass="configErrMessage" ControlToValidate="txtStimulusDuration" 
                        Display="Dynamic" ValidationExpression="<% $appSettings:RegExNumeric %>" ErrorMessage="<% $appSettings:ErrMsgNumeric %>" />
                </td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Testing Pause between Triplets:</td>
                <td class="tdConfig">
                    <asp:TextBox runat="server" ID="txtTestingPauseBetweenTriplets" CssClass="narrow txt-center configInput" MaxLength="4" />
                    <asp:RequiredFieldValidator runat="server" ID="val_TestingPauseBetweenTriplets_req" CssClass="configErrMessage" ControlToValidate="txtTestingPauseBetweenTriplets" 
                        Display="Dynamic" ErrorMessage="<% $appSettings:ErrMsgRequired %>" />
                    <asp:RegularExpressionValidator runat="server" ID="val_TestingPauseBetweenTriplets_rex" CssClass="configErrMessage" ControlToValidate="txtTestingPauseBetweenTriplets" 
                        Display="Dynamic" ValidationExpression="<% $appSettings:RegExNumeric %>" ErrorMessage="<% $appSettings:ErrMsgNumeric %>" />
                </td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Instructions - Welcome:</td>
                <td class="tdConfig">
                    <%--<asp:TextBox runat="server" ID="txtInstructionsWelcome" CssClass="widest txt-right rtl" TextMode="MultiLine" />--%>
                    <FTB:FreeTextBox runat="server" id="txtInstructionsWelcome" SupportFolder="/aspnet_client/FreeTextBox/" 
                        JavaScriptLocation="ExternalFile" ToolbarImagesLocation="ExternalFile" ButtonImagesLocation="ExternalFile" 
                        TextDirection="LeftToRight" Width="100%" Height="250" AutoGenerateToolbarsFromString="true"
                        ToolbarLayout="FontFacesMenu,FontSizesMenu,FontForeColorsMenu|Bold,Italic,Underline,Strikethrough;Superscript,Subscript,RemoveFormat|JustifyLeft,JustifyRight,JustifyCenter,JustifyFull;BulletedList,NumberedList,Indent,Outdent;CreateLink,Unlink,InsertImage|Cut,Copy,Paste;Undo,Redo">
                    </FTB:FreeTextBox>
                    <asp:RequiredFieldValidator runat="server" ID="val_InstructionsWelcome_req" CssClass="configErrMessage" ControlToValidate="txtInstructionsWelcome" 
                        Display="Dynamic" ErrorMessage="<% $appSettings:ErrMsgRequired %>" />
                </td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Instructions - Training:</td>
                <td class="tdConfig">
                    <%--<asp:TextBox runat="server" ID="txtInstructionsTraining" CssClass="widest txt-right rtl" TextMode="MultiLine" />--%>
                    <FTB:FreeTextBox runat="server" id="txtInstructionsTraining" SupportFolder="/aspnet_client/FreeTextBox/" 
                        JavaScriptLocation="ExternalFile" ToolbarImagesLocation="ExternalFile" ButtonImagesLocation="ExternalFile" 
                        TextDirection="LeftToRight" Width="100%" Height="250" AutoGenerateToolbarsFromString="true"
                        ToolbarLayout="FontFacesMenu,FontSizesMenu,FontForeColorsMenu|Bold,Italic,Underline,Strikethrough;Superscript,Subscript,RemoveFormat|JustifyLeft,JustifyRight,JustifyCenter,JustifyFull;BulletedList,NumberedList,Indent,Outdent;CreateLink,Unlink,InsertImage|Cut,Copy,Paste;Undo,Redo">
                    </FTB:FreeTextBox>
                    <asp:RequiredFieldValidator runat="server" ID="val_InstructionsTraining_req" CssClass="configErrMessage" ControlToValidate="txtInstructionsTraining" 
                        Display="Dynamic" ErrorMessage="<% $appSettings:ErrMsgRequired %>" />
                </td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Instructions - Testing:</td>
                <td class="tdConfig">
                    <%--<asp:TextBox runat="server" ID="txtInstructionsTesting" CssClass="widest txt-right rtl" TextMode="MultiLine" />--%>
                    <FTB:FreeTextBox runat="server" id="txtInstructionsTesting" SupportFolder="/aspnet_client/FreeTextBox/" 
                        JavaScriptLocation="ExternalFile" ToolbarImagesLocation="ExternalFile" ButtonImagesLocation="ExternalFile" 
                        TextDirection="LeftToRight" Width="100%" Height="250" AutoGenerateToolbarsFromString="true"
                        ToolbarLayout="FontFacesMenu,FontSizesMenu,FontForeColorsMenu|Bold,Italic,Underline,Strikethrough;Superscript,Subscript,RemoveFormat|JustifyLeft,JustifyRight,JustifyCenter,JustifyFull;BulletedList,NumberedList,Indent,Outdent;CreateLink,Unlink,InsertImage|Cut,Copy,Paste;Undo,Redo">
                    </FTB:FreeTextBox>
                    <asp:RequiredFieldValidator runat="server" ID="val_InstructionsTesting_req" CssClass="configErrMessage" ControlToValidate="txtInstructionsTesting" 
                        Display="Dynamic" ErrorMessage="<% $appSettings:ErrMsgRequired %>" />
                </td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Instructions - Testing Question:</td>
                <td class="tdConfig">
                    <%--<asp:TextBox runat="server" ID="txtInstructionsTestingQuestion" CssClass="widest txt-right rtl" TextMode="MultiLine" />--%>
                    <FTB:FreeTextBox runat="server" id="txtInstructionsTestingQuestion" SupportFolder="/aspnet_client/FreeTextBox/" 
                        JavaScriptLocation="ExternalFile" ToolbarImagesLocation="ExternalFile" ButtonImagesLocation="ExternalFile" 
                        TextDirection="LeftToRight" Width="100%" Height="250" AutoGenerateToolbarsFromString="true"
                        ToolbarLayout="FontFacesMenu,FontSizesMenu,FontForeColorsMenu|Bold,Italic,Underline,Strikethrough;Superscript,Subscript,RemoveFormat|JustifyLeft,JustifyRight,JustifyCenter,JustifyFull;BulletedList,NumberedList,Indent,Outdent;CreateLink,Unlink,InsertImage|Cut,Copy,Paste;Undo,Redo">
                    </FTB:FreeTextBox>
                    <asp:RequiredFieldValidator runat="server" ID="val_InstructionsTestingQuestion_req" CssClass="configErrMessage" ControlToValidate="txtInstructionsTestingQuestion" 
                        Display="Dynamic" ErrorMessage="<% $appSettings:ErrMsgRequired %>" />
                </td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Instructions - End Screen:</td>
                <td class="tdConfig">
                    <%--<asp:TextBox runat="server" ID="txtInstructionsEndScreen" CssClass="widest txt-right rtl" TextMode="MultiLine" />--%>
                    <FTB:FreeTextBox runat="server" id="txtInstructionsEndScreen" SupportFolder="/aspnet_client/FreeTextBox/" 
                        JavaScriptLocation="ExternalFile" ToolbarImagesLocation="ExternalFile" ButtonImagesLocation="ExternalFile" 
                        TextDirection="LeftToRight" Width="100%" Height="250" AutoGenerateToolbarsFromString="true"
                        ToolbarLayout="FontFacesMenu,FontSizesMenu,FontForeColorsMenu|Bold,Italic,Underline,Strikethrough;Superscript,Subscript,RemoveFormat|JustifyLeft,JustifyRight,JustifyCenter,JustifyFull;BulletedList,NumberedList,Indent,Outdent;CreateLink,Unlink,InsertImage|Cut,Copy,Paste;Undo,Redo">
                    </FTB:FreeTextBox>
                    <asp:RequiredFieldValidator runat="server" ID="val_InstructionsEndScreen_req" CssClass="configErrMessage" ControlToValidate="txtInstructionsEndScreen" 
                        Display="Dynamic" ErrorMessage="<% $appSettings:ErrMsgRequired %>" />
                </td>
            </tr>
        </table>
        
        <asp:Button runat="server" ID="cmdGenerateConfig" CssClass="button block h-center" Text="Create Configuration" OnClick="cmdGenerateConfig_Click" />
    </div>
</asp:Content>

