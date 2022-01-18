using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Xml;
using System.Xml.Xsl;

public partial class admin_SL_Config_Create : ExpPage
{
    public admin_SL_Config_Create()
    {
        m_Role = ExpUserRole.Admin;
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        lblErrorMessage.Text = "";
    }
    protected void cmdGenerateConfig_Click(object sender, EventArgs e)
    {
        try
        {
            if (!ValidateConfig())
            {
                lblErrorMessage.Text = "Some parameters contain invalid values!"; // TODO : display a better messge.
                return;
            }

            SL_Config config = new SL_Config();

            config.ConfigName = txtConfigName.Text;
            config.ExpType = (SL_ExpType)Int32.Parse(drpExpType.SelectedValue);
            config.RandomizationType = (SL_RandomizationType)Int32.Parse(drpTripletsRandType.SelectedValue);
            
            if (config.RandomizationType == SL_RandomizationType.Random)
            {
                config.NumOfTriplets = Int32.Parse(txtNumberOfTriplets.Text);
            }
            else if (config.RandomizationType == SL_RandomizationType.Fixed)
            {
                config.PresetTriplets = SL_Config.ParseTripletsString(txtPresetTriplets.Text, SL_TripletBase.TRIPLET_TYPE_TRIPLET);
                config.PresetFoils = SL_Config.ParseTripletsString(txtPresetFoils.Text, SL_TripletBase.TRIPLET_TYPE_FOIL);

                config.NumOfTriplets = config.PresetFoils.Count;
            }

            config.PauseBetweenStimuli = Int32.Parse(txtPauseBetweenStimuli.Text);
            config.StimulusDuration = Int32.Parse(txtStimulusDuration.Text);
            config.TrainingNumOfTripletRepetitions = Int32.Parse(txtTrainingTripletRepetitions.Text);
            config.TestingPauseBetweenTriplets = Int32.Parse(txtTestingPauseBetweenTriplets.Text);
            
            config.InstructionsWelcome = txtInstructionsWelcome.ViewStateText;
            config.InstructionsTraining = txtInstructionsTraining.ViewStateText;
            config.InstructionsTesting = txtInstructionsTesting.ViewStateText;
            config.InstructionsTestingQuestion = txtInstructionsTestingQuestion.ViewStateText;
            config.InstructionsEnd = txtInstructionsEndScreen.ViewStateText;

            int configID = DB_SL.CreateConfig(config);

            if (configID == -1)
                throw (new Exception());

            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["AdminSLConfigPage"], false);
        }
        catch
        {
            lblErrorMessage.Text = "Some parameters contain invalid values!"; // TODO : display a better messge.
        }        
    }
    protected void cmdBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["AdminSLConfigPage"], false);
    }
    protected override void InitializeMe()
    {
        base.InitializeMe();
    }

    private bool ValidateConfig()
    {
        try
        {
            bool ok = true;

            SL_ExpType expType = (SL_ExpType)Int32.Parse(drpExpType.SelectedValue);
            
            if ((SL_RandomizationType)Int32.Parse(drpTripletsRandType.SelectedValue) == SL_RandomizationType.Fixed)
            {
                // Validate Preset Triplets (that all stimuli exist in db, and all belong to the same exp type).
                List<SL_TripletBase> triplets = SL_Config.ParseTripletsString(txtPresetTriplets.Text, SL_TripletBase.TRIPLET_TYPE_TRIPLET);
                List<string> tStimuliIDs = DB_SL.GetStimuliByExpType(expType).Select(s => s.ID).ToList<string>();

                foreach (SL_TripletBase triplet in triplets)
                {
                    ok = tStimuliIDs.Contains(triplet.A) && ok;
                    ok = tStimuliIDs.Contains(triplet.B) && ok;
                    ok = tStimuliIDs.Contains(triplet.C) && ok;
                }

                // Validate Preset Foils (that all stimuli exist in db, and all belong to the same exp type).
                List<SL_TripletBase> foils = SL_Config.ParseTripletsString(txtPresetFoils.Text, SL_TripletBase.TRIPLET_TYPE_FOIL);
                List<string> fStimuliIDs = DB_SL.GetStimuliByExpType(expType).Select(s => s.ID).ToList<string>();

                foreach (SL_TripletBase foil in foils)
                {
                    ok = fStimuliIDs.Contains(foil.A) && ok;
                    ok = fStimuliIDs.Contains(foil.B) && ok;
                    ok = fStimuliIDs.Contains(foil.C) && ok;
                }

                // Validate number of triplets and foils.
                ok = triplets.Count == foils.Count && ok;
            }
            else if ((SL_RandomizationType)Int32.Parse(drpTripletsRandType.SelectedValue) == SL_RandomizationType.Random)
            { 
                // Validate Number of Triplets (there should be at least #Triplets*3 stimuli in db)
                List<SL_Stimulus> stimuli = DB_SL.GetStimuliByExpType(expType);
                int numOfTriplets = Int32.Parse(txtNumberOfTriplets.Text);

                ok = (stimuli.Count >= numOfTriplets * 3) && ok;
            }

            int val;
            val = Int32.Parse(txtPauseBetweenStimuli.Text);
            ok = val >= SL_Config.DURATION_MIN && val <= SL_Config.DURATION_MAX && ok;

            val = Int32.Parse(txtStimulusDuration.Text);
            ok = val >= SL_Config.DURATION_MIN && val <= SL_Config.DURATION_MAX && ok;

            val = Int32.Parse(txtTestingPauseBetweenTriplets.Text);
            ok = val >= SL_Config.DURATION_MIN && val <= SL_Config.DURATION_MAX && ok;

            // TODO : TESTING#QUESTIONS SHOULD BE A MULTIPLY OF #TRIPLETS??

            return ok;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            return false;
        }
    }

    [WebMethod]
    public static List<SL_Stimulus> GetStimuli(int _expType)
    {
        return DB_SL.GetStimuliByExpType((SL_ExpType)_expType);
    }
}
