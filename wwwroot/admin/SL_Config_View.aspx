<%@ Page Title="Experiment | Admin Area | SL | View Experiment Configuration" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="SL_Config_View.aspx.cs" Inherits="admin_SL_Config_View" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph_head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ph_header_text" Runat="Server">
    <h1>SL - View Experiment Configuration</h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ph_body" Runat="Server">
    <div id="div_admin" class="">
        <asp:Button runat="server" ID="cmdBack" CssClass="button block narrow" Text="Back" OnClick="cmdBack_Click" CausesValidation="false" />
        <table class="tblConfig tblConfigView">
            <tr>
                <td class="tdConfigHeader">Configuration Name:</td>
                <td class="tdConfig"><asp:Label runat="server" ID="lblConfigName" CssClass="" /></td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Experiment Type:</td>
                <td class="tdConfig"><asp:Label runat="server" ID="lblExpType" CssClass="" /></td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Triplets Creation Type:</td>
                <td class="tdConfig"><asp:Label runat="server" ID="lblRandomizationType" CssClass="" /></td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Number of Triplets:</td>
                <td class="tdConfig"><asp:Label runat="server" ID="lblNumberOfTriplets" CssClass="" /></td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Preset Triplets:</td>
                <td class="tdConfig"><asp:Label runat="server" ID="lblPresetTriplets" CssClass="" /></td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Preset Foils:</td>
                <td class="tdConfig"><asp:Label runat="server" ID="lblPresetFoils" CssClass="" /></td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Training Triplet Repetitions:</td>
                <td class="tdConfig"><asp:Label runat="server" ID="lblTrainingTripletRepetitions" CssClass="" /></td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Pause between Stimuli:</td>
                <td class="tdConfig"><asp:Label runat="server" ID="lblPauseBetweenStimuli" CssClass="" /></td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Stimulus Duration:</td>
                <td class="tdConfig"><asp:Label runat="server" ID="lblStimulusDuration" CssClass="" /></td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Testing Pause between Triplets:</td>
                <td class="tdConfig"><asp:Label runat="server" ID="lblTestingPauseBetweenTriplets" CssClass="" /></td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Instructions - Welcome:</td>
                <td class="tdConfig txt-center"><asp:Label runat="server" ID="lblInstructionsWelcome" CssClass="" /></td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Instructions - Training:</td>
                <td class="tdConfig txt-center"><asp:Label runat="server" ID="lblInstructionsTraining" CssClass="" /></td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Instructions - Testing:</td>
                <td class="tdConfig txt-center"><asp:Label runat="server" ID="lblInstructionsTesting" CssClass="" /></td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Instructions - Testing Question:</td>
                <td class="tdConfig txt-center"><asp:Label runat="server" ID="lblInstructionsTestingQuestion" CssClass="" /></td>
            </tr>
            <tr>
                <td class="tdConfigHeader">Instructions - End Screen:</td>
                <td class="tdConfig txt-center"><asp:Label runat="server" ID="lblInstructionsEnd" CssClass="" /></td>
            </tr>
        </table>
    </div>
</asp:Content>

