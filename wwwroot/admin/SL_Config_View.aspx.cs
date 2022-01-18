using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;

public partial class admin_SL_Config_View : ExpPage
{
    public admin_SL_Config_View()
    {
        m_Role = ExpUserRole.Admin;
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            int configID = Int32.Parse(Request.QueryString["cid"]);
            InitWithConfig(configID);
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
        }
    }
    protected void cmdBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["AdminSLConfigSummaryPage"], false);
    }

    private void InitWithConfig(int configID)    
    {
        SL_Config config = DB_SL.GetConfigByID(configID);
        lblConfigName.Text = config.ConfigName;
        lblExpType.Text = config.ExpType.ToString();
        lblRandomizationType.Text = config.RandomizationType.ToString();
        lblNumberOfTriplets.Text = config.NumOfTriplets.ToString();
        lblPresetTriplets.Text = SL_Config.GetTripletsString(config.PresetTriplets);
        lblPresetFoils.Text = SL_Config.GetTripletsString(config.PresetFoils);
        lblPauseBetweenStimuli.Text = config.PauseBetweenStimuli.ToString();
        lblStimulusDuration.Text = config.StimulusDuration.ToString();
        lblTrainingTripletRepetitions.Text = config.TrainingNumOfTripletRepetitions.ToString();
        lblTestingPauseBetweenTriplets.Text = config.TestingPauseBetweenTriplets.ToString();
        lblInstructionsWelcome.Text = config.InstructionsWelcome;
        lblInstructionsTraining.Text = config.InstructionsTraining;
        lblInstructionsTesting.Text = config.InstructionsTesting;
        lblInstructionsTestingQuestion.Text = config.InstructionsTestingQuestion;
        lblInstructionsEnd.Text = config.InstructionsEnd;
    }
}
