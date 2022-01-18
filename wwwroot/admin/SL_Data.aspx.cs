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

public partial class admin_SL_Data : ExpPage
{
    public admin_SL_Data()
    {
        m_Role = ExpUserRole.Admin;
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            InitDropDownLists();
        }
        else
        { 
            
        }
    }
    protected override void InitializeMe()
    {
        base.InitializeMe();
    }
    protected void cmdSessionsReport_Click(object sender, EventArgs e)
    {
        GenerateSessionsReport();
    }
    protected void cmdTripletsReport_Click(object sender, EventArgs e)
    {
        GenerateTripletsReport(txtSessionID.Text, drpExpType.SelectedValue, drpConfigID.SelectedValue);
    }
    protected void cmdTrainingTrialsReport_Click(object sender, EventArgs e) 
    {
        GenerateTrainingTrialsReport(txtSessionID.Text, drpExpType.SelectedValue, drpConfigID.SelectedValue);
    }
    protected void cmdTestingTrialsReport_Click(object sender, EventArgs e)
    {
        GenerateTestingTrialsReport(txtSessionID.Text, drpExpType.SelectedValue, drpConfigID.SelectedValue);
    }
    protected void cmdTrainingAndTestingTrialsReport_Click(object sender, EventArgs e)
    {
        GenerateTrainingAndTestingTrialsReport(txtSessionID.Text, drpExpType.SelectedValue, drpConfigID.SelectedValue);
    }
    protected void cmdBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["AdminPage"], false);
    }

    protected void GenerateSessionsReport()
    {
        ExpCSVObject csvObj = new ExpCSVObject(8);

        List<string> headers = new List<string>();
        headers.Add("SeqID");
        headers.Add("SessionID");
        headers.Add("SubjectID");
        headers.Add("ExpType");
        headers.Add("ConfigName");
        headers.Add("StartTime");
        headers.Add("EndTime");
        headers.Add("SessionDuration");
        csvObj.Headers = headers;

        Dictionary<int, SL_Session> sessions = DB_SL.GetSessions();

        foreach (SL_Session session in sessions.Values)
        {
            List<string> dataRow = new List<string>();
            dataRow.Add(session.ID.ToString());
            dataRow.Add(session.SessionID);
            dataRow.Add(session.SubjectID);
            dataRow.Add(session.Config.ExpType.ToString());
            dataRow.Add(session.Config.ConfigName);
            dataRow.Add(session.StartTime.ToString(Common.FORMAT_DATE_TIME_DB));
            dataRow.Add(session.EndTime.ToString(Common.FORMAT_DATE_TIME_DB));
            dataRow.Add(session.SessionDuration.ToString());

            csvObj.AddDataRow(dataRow);
        }

        string filename = string.Format("SL Sessions.csv");

        Common.GenerateCSVReport(this, filename, csvObj);
    }
    protected void GenerateTripletsReport(string _sessionID, string _expType, string _configID)
    {
        ExpCSVObject csvObj = new ExpCSVObject(8);

        List<string> headers = new List<string>();
        headers.Add("SeqID");
        headers.Add("SessionID");
        headers.Add("SubjectID");
        headers.Add("A");
        headers.Add("B");
        headers.Add("C");
        headers.Add("TripletID");
        headers.Add("Type");
        csvObj.Headers = headers;

        int expType = _expType == string.Empty ? -1 : Int32.Parse(_expType);
        int configID = _configID == string.Empty ? -1 : Int32.Parse(_configID);
        Dictionary<int, SL_Triplet> trainingTrials = DB_SL.GetSessionTriplets(_sessionID.Trim(), expType, configID);

        foreach (SL_Triplet triplet in trainingTrials.Values)
        {
            List<string> dataRow = new List<string>();
            dataRow.Add(triplet.ID.ToString());
            dataRow.Add(triplet.SessionID);
            dataRow.Add(triplet.SubjectID);
            dataRow.Add(triplet.A);
            dataRow.Add(triplet.B);
            dataRow.Add(triplet.C);
            dataRow.Add(triplet.TripletID);
            dataRow.Add(triplet.Type);

            csvObj.AddDataRow(dataRow);
        }

        string filename = string.Format("SL Triplets - {0}.csv", txtSessionID.Text.Trim() == string.Empty ? "All" : txtSessionID.Text.Trim());

        Common.GenerateCSVReport(this, filename, csvObj);
    }
    protected void GenerateTrainingTrialsReport(string _sessionID, string _expType, string _configID)
    {
        string filename = string.Format("SL Training Trials - {0}.csv", txtSessionID.Text.Trim() == string.Empty ? "All" : txtSessionID.Text.Trim());

        ExpCSVObject trainingHeaders = GetTrainingTrialsHeadersCSVObject(_sessionID, _expType, _configID);
        ExpCSVObject trainingData = GetTrainingTrialsDataCSVObject(_sessionID, _expType, _configID);

        Common.GenerateCSVReport(this, filename, trainingData, trainingHeaders);
    }
    protected void GenerateTestingTrialsReport(string _sessionID, string _expType, string _configID)
    {
        string filename = string.Format("SL Testing Trials - {0}.csv", txtSessionID.Text.Trim() == string.Empty ? "All" : txtSessionID.Text.Trim());

        ExpCSVObject testingHeaders = GetTestingTrialsHeadersCSVObject(_sessionID, _expType, _configID);
        ExpCSVObject testingData = GetTestingTrialsDataCSVObject(_sessionID, _expType, _configID);

        Common.GenerateCSVReport(this, filename, testingData, testingHeaders);
    }
    protected void GenerateTrainingAndTestingTrialsReport(string _sessionID, string _expType, string _configID)
    {
        string filename = string.Format("SL Training & Testing Trials - {0}.csv", txtSessionID.Text.Trim() == string.Empty ? "All" : txtSessionID.Text.Trim());

        ExpCSVObject trainingHeaders = GetTrainingTrialsHeadersCSVObject(_sessionID, _expType, _configID);
        ExpCSVObject testingHeaders = GetTestingTrialsHeadersCSVObject(_sessionID, _expType, _configID);
        ExpCSVObject trainingData = GetTrainingTrialsDataCSVObject(_sessionID, _expType, _configID);
        ExpCSVObject testingData = GetTestingTrialsDataCSVObject(_sessionID, _expType, _configID);

        Common.GenerateCSVReport(this, filename, testingData, trainingHeaders, testingHeaders, trainingData);
    }

    private void InitDropDownLists()
    {
        drpExpType.Items.Clear();
        drpExpType.Items.Add(new ListItem("All Experiment Types", ""));
        drpExpType.Items.Add(new ListItem(SL_ExpType.VSL.ToString(), ((int)SL_ExpType.VSL).ToString()));
        drpExpType.Items.Add(new ListItem(SL_ExpType.ASL.ToString(), ((int)SL_ExpType.ASL).ToString()));
        drpExpType.Items.Add(new ListItem(SL_ExpType.MSL.ToString(), ((int)SL_ExpType.MSL).ToString()));
        drpExpType.Items.Add(new ListItem(SL_ExpType.DSL.ToString(), ((int)SL_ExpType.DSL).ToString()));


        drpConfigID.Items.Clear();
        drpConfigID.Items.Add(new ListItem("All Configurations", ""));
        foreach (SL_Config config in DB_SL.GetConfigs(null).Values)
            drpConfigID.Items.Add(new ListItem(config.ConfigName, config.ID.ToString()));
    }
    private ExpCSVObject GetTrainingTrialsHeadersCSVObject(string _sessionID, string _expType, string _configID)
    {
        ExpCSVObject csvObj = new ExpCSVObject(21);

        List<string> headers = new List<string>();
        headers.Add("SeqID");
        headers.Add("SessionID");
        headers.Add("SubjectID");
        headers.Add("ExperimentPhase");
        headers.Add("TrialNumber");
        headers.Add("A");
        headers.Add("B");
        headers.Add("C");
        headers.Add("TripletID");
        headers.Add("ExpType");
        headers.Add("ConfigName");
        headers.Add("TripletCreationType");
        headers.Add("PauseBetweenStimuli");
        headers.Add("StimulusDuration");
        headers.Add("TrainingTripletRepetition");
        headers.Add("TestingPauseBetweenTriplets");
        headers.Add("Sex");
        headers.Add("AgeY");
        headers.Add("AgeM");
        headers.Add("NativeLanguage");
        headers.Add("Location");
        headers.Add("Remarks");
        csvObj.Headers = headers;

        return csvObj;
    }
    private ExpCSVObject GetTestingTrialsHeadersCSVObject(string _sessionID, string _expType, string _configID)
    {
        ExpCSVObject csvObj = new ExpCSVObject(22);

        List<string> headers = new List<string>();
        headers.Add("SeqID");
        headers.Add("SessionID");
        headers.Add("SubjectID");
        headers.Add("ExperimentPhase");
        headers.Add("TrialNumber");
        headers.Add("Stimulus1A");
        headers.Add("Stimulus1B");
        headers.Add("Stimulus1C");
        headers.Add("Stimulus2A");
        headers.Add("Stimulus2B");
        headers.Add("Stimulus2C");
        headers.Add("TripletID1");
        headers.Add("TripletID2");
        headers.Add("TrueTriplet");
        headers.Add("Answer");
        headers.Add("RT");
        headers.Add("SessionDuration");
        headers.Add("ExpType");
        headers.Add("ConfigName");
        headers.Add("TripletCreationType");
        headers.Add("PauseBetweenStimuli");
        headers.Add("StimulusDuration");
        headers.Add("TrainingTripletRepetition");
        headers.Add("TestingPauseBetweenTriplets");
        headers.Add("Sex");
        headers.Add("AgeY");
        headers.Add("AgeM");
        headers.Add("NativeLanguage");
        headers.Add("Location");
        headers.Add("Remarks");
        csvObj.Headers = headers;

        return csvObj;
    }
    private ExpCSVObject GetTrainingTrialsDataCSVObject(string _sessionID, string _expType, string _configID)
    {
        ExpCSVObject csvObj = new ExpCSVObject(21);

        int expType = _expType == string.Empty ? -1 : Int32.Parse(_expType);
        int configID = _configID == string.Empty ? -1 : Int32.Parse(_configID);
        Dictionary<int, SL_TrainingTrial> trainingTrials = DB_SL.GetSessionTrainingTrials(_sessionID.Trim(), expType, configID);

        foreach (SL_TrainingTrial trial in trainingTrials.Values)
        {
            List<string> dataRow = new List<string>();
            dataRow.Add(trial.ID.ToString());
            dataRow.Add(trial.SessionID);
            dataRow.Add(trial.SubjectID);
            dataRow.Add("Training");
            dataRow.Add(trial.TrialNumber.ToString());
            dataRow.Add(trial.A);
            dataRow.Add(trial.B);
            dataRow.Add(trial.C);
            dataRow.Add(trial.TripletID);
            dataRow.Add(trial.Config.ExpType.ToString());
            dataRow.Add(trial.Config.ConfigName);
            dataRow.Add(trial.Config.RandomizationType.ToString());
            dataRow.Add(trial.Config.PauseBetweenStimuli.ToString());
            dataRow.Add(trial.Config.StimulusDuration.ToString());
            dataRow.Add(trial.Config.TrainingNumOfTripletRepetitions.ToString());
            dataRow.Add(trial.Config.TestingPauseBetweenTriplets.ToString());
            dataRow.Add(trial.Sex);
            dataRow.Add(trial.AgeY.ToString());
            dataRow.Add(trial.AgeM.ToString());
            dataRow.Add(trial.NativeLanguage);
            dataRow.Add(trial.Location);
            dataRow.Add(trial.Remarks);

            csvObj.AddDataRow(dataRow);
        }

        return csvObj;
    }
    private ExpCSVObject GetTestingTrialsDataCSVObject(string _sessionID, string _expType, string _configID)
    {
        ExpCSVObject csvObj = new ExpCSVObject(22);

        int expType = _expType == string.Empty ? -1 : Int32.Parse(_expType);
        int configID = _configID == string.Empty ? -1 : Int32.Parse(_configID);
        Dictionary<int, SL_TestingTrial> testingTrials = DB_SL.GetSessionTestingTrials(_sessionID.Trim(), expType, configID);

        foreach (SL_TestingTrial trial in testingTrials.Values)
        {
            List<string> dataRow = new List<string>();
            dataRow.Add(trial.ID.ToString());
            dataRow.Add(trial.SessionID);
            dataRow.Add(trial.SubjectID);
            dataRow.Add("Testing");
            dataRow.Add(trial.TrialNumber.ToString());
            dataRow.Add(trial.Stimulus1A);
            dataRow.Add(trial.Stimulus1B);
            dataRow.Add(trial.Stimulus1C);
            dataRow.Add(trial.Stimulus2A);
            dataRow.Add(trial.Stimulus2B);
            dataRow.Add(trial.Stimulus2C);
            dataRow.Add(trial.TripletID1);
            dataRow.Add(trial.TripletID2);
            dataRow.Add(trial.TrueTriplet.ToString());
            dataRow.Add(trial.Answer.ToString());
            dataRow.Add(trial.RT.ToString());
            dataRow.Add(trial.SessionDuration.ToString());
            dataRow.Add(trial.Config.ExpType.ToString());
            dataRow.Add(trial.Config.ConfigName);
            dataRow.Add(trial.Config.RandomizationType.ToString());
            dataRow.Add(trial.Config.PauseBetweenStimuli.ToString());
            dataRow.Add(trial.Config.StimulusDuration.ToString());
            dataRow.Add(trial.Config.TrainingNumOfTripletRepetitions.ToString());
            dataRow.Add(trial.Config.TestingPauseBetweenTriplets.ToString());
            dataRow.Add(trial.Sex);
            dataRow.Add(trial.AgeY.ToString());
            dataRow.Add(trial.AgeM.ToString());
            dataRow.Add(trial.NativeLanguage);
            dataRow.Add(trial.Location);
            dataRow.Add(trial.Remarks);

            csvObj.AddDataRow(dataRow);
        }

        return csvObj;
    }
}
