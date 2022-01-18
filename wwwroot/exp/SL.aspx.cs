using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class exp_SL : ExpBasePage 
{
    // Members
    ////////////////////////////////////////


    // Protected Methods
    ////////////////////////////////////////
    protected void Page_Load(object sender, EventArgs e)
    {
        string expParamsStr = Request.QueryString["pd"];

        SL_Parameters expParams = SL_Parameters.ReadExpParametersFromURL(expParamsStr);

        if (expParams != null)
        {
            divWelcome.Visible = true;
            divError.Visible = false;

            lblInstructionsWelcome.Text = expParams.Config.InstructionsWelcome;
            lblInstructionsTraining.Text = expParams.Config.InstructionsTraining;
            lblInstructionsTesting.Text = expParams.Config.InstructionsTesting;
            lblInstructionsTestingQuestion.Text = expParams.Config.InstructionsTestingQuestion;
            lblInstructionsEnd.Text = expParams.Config.InstructionsEnd;

            //InitDemographicsControls();
        }
        else 
        {
            divWelcome.Visible = false;
            divError.Visible = true;
        }
    }
    protected override void InitializeMe()
    {
        base.InitializeMe();
    }


    // Private Methods
    ////////////////////////////////////////
    //private void InitDemographicsControls()
    //{
    //    drpDemAgeY.Items.Clear();
    //    drpDemAgeY.Items.Add(new ListItem("", ""));
    //    for (int i = 0; i < 16; i++)
    //        drpDemAgeY.Items.Add(new ListItem(i.ToString(), i.ToString()));

    //    drpDemAgeM.Items.Clear();
    //    drpDemAgeM.Items.Add(new ListItem("", ""));
    //    for (int i = 0; i <= 11; i++)
    //        drpDemAgeM.Items.Add(new ListItem(i.ToString(), i.ToString()));
    //}


    // Public Methods
    ////////////////////////////////////////


    // Web Methods
    ////////////////////////////////////////
    [WebMethod]
    public static SL_Parameters LoadExpParameters(string _pd)
    {
        return SL_Parameters.ReadExpParametersFromURL(_pd);
    }
    [WebMethod]
    public static List<SL_Stimulus> GetStimuli(int _expType)
    {
        return DB_SL.GetStimuliByExpType((SL_ExpType)_expType);
    }
    [WebMethod]
    public static string CreateSession(int _configID)
    {
        return DB_SL.CreateSession(_configID);
    }
    [WebMethod]
    public static bool EndSession(string _sessionID)
    {
        return DB_SL.EndSession(_sessionID);
    }

    [WebMethod]
    public static bool SaveDemographics(string _sessionID, string _subjectID, string _sex, int _ageY, int _ageM, string _nativeLang, string _location, string _remarks)
    {
        return DB_SL.SaveDemographics(_sessionID, _subjectID, _sex, _ageY, _ageM, _nativeLang, _location, _remarks);
    }
    [WebMethod]
    public static bool SaveSessionTriplets(SL_TripletBase[] _triplets)
    {
        return DBCommon.BulkInsert<SL_TripletBase>(_triplets.ToList(), "sl_triplet");
    }
    [WebMethod]
    public static bool SaveSessionTrainingTrials(SL_TrainingTrialBase[] _trials)
    {
        return DBCommon.BulkInsert<SL_TrainingTrialBase>(_trials.ToList(), "sl_training_trial");
    }
    [WebMethod]
    public static bool SaveSessionTestingTrials(SL_TestingTrialBase[] _trials)
    {
        return DBCommon.BulkInsert<SL_TestingTrialBase>(_trials.ToList(), "sl_testing_trial");
    }
}
