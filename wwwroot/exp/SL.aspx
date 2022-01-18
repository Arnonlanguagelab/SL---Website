<%@ Page Title="המעבדה לרכישת שפה ועיבוד שפה | ניסוי" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="SL.aspx.cs" Inherits="exp_SL" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ph_head" Runat="Server">      
    <script type="text/javascript" src="/js/SL_v2.js" ></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ph_header_text" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ph_body" Runat="Server">
    <div id="div_experiment_main">
        <div id="div_startup" class="">
            <div runat="server" id="divWelcome" class="">
                <p><asp:Label runat="server" ID="lblInstructionsWelcome" CssClass="" /></p>
                <h4>לחצו על מקש הרווח כדי להמשיך</h4>
            </div>
            <div runat="server" id="divError" class="" visible="false">
                <h2 class="errMessage">אופס...</h2>
                <p>נראה שארעה שגיאה. אנא נסו לטעון את העמוד שנית.</p>
            </div>
        </div>

        <div id="div_demographics" class=" hidden">
            <p><h3>אנא מלאו את הפרטים הבאים:</h3></p>
            <table class="tblDemographics">
                <tr>
                    <td class="tdDemHeader">קוד נבדק:</td>
                    <td class="tdDem">
                        <input type="text" id="txt_dem_subject_id" class="demInput txt-center narrow ltr" maxlength="9" autocomplete="off" />
                    </td>
                </tr>
                <tr>
                    <td class="tdDemHeader">מין:</td>
                    <td class="tdDem">
                        <select id="drp_dem_sex" class="demInput">
                            <option value="" selected="selected"></option>
                            <option value="F">נקבה</option>
                            <option value="M">זכר</option>
                        </select>
                        <span id="lbl_dem_sex_err_msg" class="configErrMessage txt-center"></span>
                    </td>
                </tr>
                <tr>
                    <td class="tdDemHeader">גיל:</td>
                    <td class="tdDem">
                        <span class="tdDemHeader">שנים</span>
                        <select id="drp_dem_age_y" class="demInput">
                            <option value="" selected="selected"></option>
                            <option value="0">0</option>
                            <option value="1">1</option>
                            <option value="2">2</option>
                            <option value="3">3</option>
                            <option value="4">4</option>
                            <option value="5">5</option>
                            <option value="6">6</option>
                            <option value="7">7</option>
                            <option value="8">8</option>
                            <option value="9">9</option>
                            <option value="10">10</option>
                            <option value="11">11</option>
                            <option value="12">12</option>
                            <option value="13">13</option>
                            <option value="14">14</option>
                            <option value="15">15</option>
                        </select>
                        <span id="lbl_dem_age_y_err_msg" class="configErrMessage txt-center"></span>
                        <span class="tdDemHeader">חודשים</span>
                        <select id="drp_dem_age_m" class="demInput">
                            <option value="" selected="selected"></option>
                            <option value="0">0</option>
                            <option value="1">1</option>
                            <option value="2">2</option>
                            <option value="3">3</option>
                            <option value="4">4</option>
                            <option value="5">5</option>
                            <option value="6">6</option>
                            <option value="7">7</option>
                            <option value="8">8</option>
                            <option value="9">9</option>
                            <option value="10">10</option>
                            <option value="11">11</option>
                        </select>
                        <span id="lbl_dem_age_m_err_msg" class="configErrMessage txt-center"></span>
                    </td>
                </tr>
                <tr>
                    <td class="tdDemHeader">שפת אם:</td>
                    <td class="tdDem">
                        <select id="drp_dem_native_lang" class="demInput">
                            <option value="" selected="selected"></option>
                            <option value="עברית">עברית</option>
                            <option value="ערבית">ערבית</option>
                            <option value="רוסית">רוסית</option>
                            <option value="אמהרית">אמהרית</option>
                            <option value="אנגלית">אנגלית</option>
                            <option value="אחר">אחר</option>
                        </select>
                        <span id="lbl_dem_native_lang_err_msg" class="configErrMessage txt-center"></span>
                    </td>
                </tr>
                <tr>
                    <td class="tdDemHeader">מקום הניסוי:</td>
                    <td class="tdDem">
                        <select id="drp_dem_location" class="demInput">
                            <option value="" selected="selected"></option>
                            <option value="במעבדה">במעבדה</option>
                            <option value="בבית">בבית</option>
                            <option value="בספריה">בספריה</option>
                            <option value="בבית ספר">בבית ספר</option>
                            <option value="בבית קפה">בבית קפה</option>
                            <option value="אחר">אחר</option>
                        </select>
                        <span id="lbl_dem_location_err_msg" class="configErrMessage txt-center"></span>
                    </td>
                </tr>
                <tr>
                    <td class="tdDemHeader">הערות:</td>
                    <td class="tdDem">
                        <textarea id="txt_dem_remarks" class="demInput txt-right wide rtl" maxlength="100" autocomplete="off"></textarea>
                    </td>
                </tr>
            </table>
            <input type="button" id="cmd_demographics_ok" class="button block h-center" value="אישור" />
        </div>
        
        <div id="div_training_instructions" class=" hidden">
            <p><asp:Label runat="server" ID="lblInstructionsTraining" CssClass="" /></p>
            <h4>לחצו על מקש הרווח כדי להמשיך</h4>
        </div>
        
        <div id="div_training_trials" class=" hidden">
            <div id="div_training_panel" class=""></div>
        </div>
        
        <div id="div_testing_instructions" class=" hidden">
            <p><asp:Label runat="server" ID="lblInstructionsTesting" CssClass="" /></p>
            <h4>לחצו על מקש הרווח כדי להמשיך</h4>
        </div>
        
        <div id="div_testing_trials" class=" hidden">
            <div id="div_testing_question_panel" class=""></div>
            <div id="div_testing_answer_panel" class="">
                <p><asp:Label runat="server" ID="lblInstructionsTestingQuestion" CssClass="" /></p>
            </div>
        </div>
        
        <div id="div_finish" class=" hidden">
            <p><asp:Label runat="server" ID="lblInstructionsEnd" CssClass="" /></p>
            <%--<p>                
                <div id="div_finish_spid_val" class="spidVal"></div>
            </p>--%>
        </div>
    </div>
</asp:Content>