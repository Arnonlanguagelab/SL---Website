using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;



public enum SL_ExpType
{
    VSL = 1,
    ASL = 2,
    MSL = 3,
    DSL = 4
}
public enum SL_RandomizationType
{
    Random  = 1,
    Fixed   = 2
}




public class SL_Parameters
{
    // Constants
    ////////////////////////////////////////


    // Members / Properties
    ////////////////////////////////////////
    public ExpName ExperimentName { get; set; }
    public SL_Config Config { get; set; }

    // Constructors
    ////////////////////////////////////////
    public SL_Parameters()
    {

    }

    // Public Methods
    ////////////////////////////////////////
    public static SL_Parameters ReadExpParametersFromURL(string _urlParamString)
    {
        SL_Parameters retVal = new SL_Parameters();

        try
        {
            // if _urlParamString is not provided, use default values, by running the code in the catch block.
            if (_urlParamString == null || _urlParamString == string.Empty)
                throw new Exception();

            string dec = CryptLib.Decrypt(_urlParamString);
            string[] paramArr = dec.Split(Common.PARAMS_URL_DELIMITER);

            retVal.ExperimentName = (ExpName)Int32.Parse(paramArr[0]);
            retVal.Config = DB_SL.GetConfigByID(Int32.Parse(paramArr[1]));
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            throw (ex);
        }

        return retVal;
    }
}




public class SL_Config
{
    // Constants
    ////////////////////////////////////////
    public const int DURATION_MIN = 0;
    public const int DURATION_MAX = 5000;
    public const char TRIPLETS_DELIMITER = ';';
    public const char TRIPLET_ELEMENT_DELIMITER = ',';

    // Members / Properties
    ////////////////////////////////////////
    public int                  ID                              { get; set; }
    public string               ConfigName                      { get; set; }
    public SL_ExpType           ExpType                         { get; set; }
    public SL_RandomizationType RandomizationType               { get; set; }
    public int                  NumOfTriplets                   { get; set; }
    public List<SL_TripletBase>     PresetTriplets                  { get; set; }
    public List<SL_TripletBase>     PresetFoils                     { get; set; }
    public int                  PauseBetweenStimuli             { get; set; }
    public int                  StimulusDuration                { get; set; }
    public int                  TrainingNumOfTripletRepetitions { get; set; }
    public int                  TestingPauseBetweenTriplets     { get; set; }
    public string               InstructionsWelcome             { get; set; }
    public string               InstructionsTraining            { get; set; }
    public string               InstructionsTesting             { get; set; }
    public string               InstructionsTestingQuestion     { get; set; }
    public string               InstructionsEnd                 { get; set; }

    // Constructors
    ////////////////////////////////////////
    public SL_Config()
    {

    }

    // Public Methods
    ////////////////////////////////////////
    public static void CreateConfigurationItem(Control container, SL_Config config)
    {
        HtmlGenericControl div = new HtmlGenericControl("div");
        div.ID = "divConfigItem" + config.ID;
        div.Attributes["class"] = "configItem";

        HtmlGenericControl header = new HtmlGenericControl("h5");
        header.Attributes["class"] = "configItemHeader";
        header.InnerText = config.ConfigName;
        div.Controls.Add(header);

        HtmlGenericControl subHeader = new HtmlGenericControl("h6");
        subHeader.Attributes["class"] = "configItemSubHeader";
        subHeader.InnerText = string.Format("{0} | {1}", config.ExpType.ToString(), config.RandomizationType.ToString());
        div.Controls.Add(subHeader);

        HyperLink linkView = new HyperLink();
        linkView.Attributes["class"] = "configItemButton";
        linkView.NavigateUrl = ConfigurationManager.AppSettings["AdminSLConfigViewPage"] + config.ID;
        //linkView.Target = "_blank";
        linkView.Text = "View";
        div.Controls.Add(linkView);

        HyperLink linkGoto = new HyperLink();
        linkGoto.Attributes["class"] = "configItemButton";
        string paramsStr = string.Format("{0}{1}{2}", (int)ExpName.SL, Common.PARAMS_URL_DELIMITER, config.ID);
        linkGoto.NavigateUrl = ConfigurationManager.AppSettings["ExpSLPage"] + CryptLib.Encrypt(paramsStr);
        linkGoto.Target = "_blank";
        linkGoto.Text = "Start Experiment";
        div.Controls.Add(linkGoto);

        container.Controls.Add(div);
    }
    public static List<SL_TripletBase> ParseTripletsString(string _tripletsString, string _tripletsType)
    {
        List<SL_TripletBase> retVal = new List<SL_TripletBase>();

        string[] triplets = _tripletsString.Split(TRIPLETS_DELIMITER);

        foreach (string triplet in triplets)
        {
            SL_TripletBase newTriplet = new SL_TripletBase();
            string strippedTriplet = triplet.Replace("[", "").Replace("]", "");

            string[] elements = strippedTriplet.Split(TRIPLET_ELEMENT_DELIMITER);

            if (elements.Length != 3)
                return null;

            newTriplet.A = elements[0];
            newTriplet.B = elements[1];
            newTriplet.C = elements[2];

            newTriplet.Type = _tripletsType;

            retVal.Add(newTriplet);
        }

        return retVal;
    }
    public static string GetTripletsString(List<SL_TripletBase> _triplets)
    {
        if (_triplets == null)
            return null;
        
        string retVal = string.Empty;

        foreach (SL_TripletBase t in _triplets)
        {
            retVal += string.Format("[{2}{0}{3}{0}{4}]{1}", TRIPLET_ELEMENT_DELIMITER, TRIPLETS_DELIMITER, t.A, t.B, t.C);
        }

        retVal = retVal.TrimEnd(TRIPLETS_DELIMITER);

        return retVal;
    }
}






public class SL_Session
{
    // Members / Properties
    ////////////////////////////////////////
    public int ID { get; set; }
    public string SessionID { get; set; }
    public string SubjectID { get; set; }
    public SL_Config Config { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan SessionDuration { get; set; }

    // Constructors
    ////////////////////////////////////////
    public SL_Session()
    {

    }

    // Public Methods
    ////////////////////////////////////////

}




public class SL_Stimulus
{
    // Members / Properties
    ////////////////////////////////////////
    public string       ID          { get; set; }
    public SL_ExpType   ExpType     { get; set; }
    public string       Type        { get; set; }
    public string       Filename    { get; set; }
    public string       Description { get; set; }


    // Constructors
    ////////////////////////////////////////
    public SL_Stimulus()
    {

    }
    public SL_Stimulus(string _id, SL_ExpType _expType, string _type, string _filename, string _description)
    {
        ID = _id;
        ExpType = _expType;
        Type = _type;
        Filename = _filename;
        Description = _description;
    }


    // Public Methods
    ////////////////////////////////////////

}





public class SL_TripletBase
{
    // Constants
    ////////////////////////////////////////
    public static string TRIPLET_TYPE_TRIPLET = "T";
    public static string TRIPLET_TYPE_FOIL    = "F";
    
    // Members / Properties
    ////////////////////////////////////////
    public int      ID          { get; set; }
    public string   SessionID   { get; set; }
    public string   A           { get; set; }
    public string   B           { get; set; }
    public string   C           { get; set; }
    public string   TripletID   { get; set; }
    public string   Type        { get; set; }

    // Constructors
    ////////////////////////////////////////
    public SL_TripletBase()
    {

    }

    // Public Methods
    ////////////////////////////////////////

}



public class SL_Triplet : SL_TripletBase
{
    // Constants
    ////////////////////////////////////////

    // Members / Properties
    ////////////////////////////////////////
    public string SubjectID { get; set; }

    // Constructors
    ////////////////////////////////////////
    public SL_Triplet()
    {

    }

    // Public Methods
    ////////////////////////////////////////

}



public class SL_TrainingTrialBase
{
    // Members / Properties
    ////////////////////////////////////////
    public int      ID          { get; set; }
    public string   SessionID   { get; set; }
    public string   StimulusID  { get; set; }
    public string   Element     { get; set; }
    public string   TripletID   { get; set; }
    public int      TrialNumber { get; set; }

    // Constructors
    ////////////////////////////////////////
    public SL_TrainingTrialBase()
    {

    }

    // Public Methods
    ////////////////////////////////////////

}

public class SL_TrainingTrial : SL_TrainingTrialBase
{
    // Members / Properties
    ////////////////////////////////////////
    public string       A               { get; set; }
    public string       B               { get; set; }
    public string       C               { get; set; }
    public string       SubjectID       { get; set; }
    public SL_Config    Config          { get; set; }
    public string       Sex             { get; set; }
    public int          AgeY            { get; set; }
    public int          AgeM            { get; set; }
    public string       NativeLanguage  { get; set; }
    public string       Location        { get; set; }
    public string       Remarks         { get; set; }

    // Constructors
    ////////////////////////////////////////
    public SL_TrainingTrial()
    {

    }

    // Public Methods
    ////////////////////////////////////////

}



public class SL_TestingTrialBase
{
    // Members / Properties
    ////////////////////////////////////////
    public int      ID          { get; set; }
    public string   SessionID   { get; set; }
    public string   Stimulus1A  { get; set; }
    public string   Stimulus1B  { get; set; }
    public string   Stimulus1C  { get; set; }
    public string   Stimulus2A  { get; set; }
    public string   Stimulus2B  { get; set; }
    public string   Stimulus2C  { get; set; }
    public int      Answer      { get; set; }
    public int      RT          { get; set; }
    public int      TrialNumber { get; set; }

    // Constructors
    ////////////////////////////////////////
    public SL_TestingTrialBase()
    {

    }

    // Public Methods
    ////////////////////////////////////////

}



public class SL_TestingTrial : SL_TestingTrialBase
{
    // Members / Properties
    ////////////////////////////////////////
    public string       SubjectID       { get; set; }
    public SL_Config    Config          { get; set; }
    public string       TripletID1      { get; set; }
    public string       TripletID2      { get; set; }
    public int          TrueTriplet     { get; set; }
    public TimeSpan     SessionDuration { get; set; }
    public string       Sex             { get; set; }
    public int          AgeY            { get; set; }
    public int          AgeM            { get; set; }
    public string       NativeLanguage  { get; set; }
    public string       Location        { get; set; }
    public string       Remarks         { get; set; }


    // Constructors
    ////////////////////////////////////////
    public SL_TestingTrial()
    {

    }

    // Public Methods
    ////////////////////////////////////////

}