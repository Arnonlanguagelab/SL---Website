using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using Microsoft.Practices.EnterpriseLibrary.Data;



public static class DB_SL
{
    public static string CreateSession(int _configID)
    {
        try
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("sl_CreateSession");
            db.AddInParameter(theCommand, "ConfigID", DbType.Int32, _configID);

            object obj = db.ExecuteScalar(theCommand);
            string sessionID = obj == null ? null : obj.ToString();

            Common.LogMessage(string.Format("sl_CreateSession completed Successfully. SessionID:{0}", sessionID));
            return sessionID;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            return null;
        }
    }
    public static bool EndSession(string _sessionID)
    {
        try
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("sl_EndSession");
            db.AddInParameter(theCommand, "SessionID", DbType.String, _sessionID);

            object countObj = db.ExecuteNonQuery(theCommand);
            int count;
            if (Int32.TryParse(countObj.ToString(), out count))
                if (count != 1)
                    return false;

            Common.LogMessage(string.Format("sl_EndSession completed Successfully. SessionID:{0}", _sessionID));
            return true;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            return false;
        }
    }
    public static Dictionary<int, SL_Session> GetSessions()
    {
        try
        {
            Dictionary<int, SL_Session> retVal = new Dictionary<int, SL_Session>();
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("sl_GetSessions");

            DataSet ds = db.ExecuteDataSet(theCommand);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            SL_Session newSession = new SL_Session();

                            newSession.ID = Common.SafeGetInt(dr["ID"]);
                            newSession.SessionID = Common.SafeGetString(dr["SessionID"]);
                            newSession.SubjectID = Common.SafeGetString(dr["SubjectID"]);
                            newSession.Config = GetConfigByID(Common.SafeGetInt(dr["ConfigID"]));
                            newSession.StartTime = Common.SafeGetDateTime(dr["StartTime"]);
                            newSession.EndTime = Common.SafeGetDateTime(dr["EndTime"]);
                            newSession.SessionDuration = TimeSpan.Parse(Common.SafeGetObject(dr["SessionDuration"], "00:00:00").ToString());

                            retVal.Add(newSession.ID, newSession);
                        }
                    }
                    return retVal;
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            return null;
        }
    }

    public static int CreateConfig(SL_Config _config)
    {
        try
        {
            string pt = SL_Config.GetTripletsString(_config.PresetTriplets);
            string pf = SL_Config.GetTripletsString(_config.PresetFoils);
            
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("sl_CreateConfig");
            db.AddInParameter(theCommand, "ConfigName", DbType.String, _config.ConfigName);
            db.AddInParameter(theCommand, "ExpType", DbType.Int32, (int)_config.ExpType);
            db.AddInParameter(theCommand, "RandomizationType", DbType.Int32, (int)_config.RandomizationType);
            db.AddInParameter(theCommand, "NumOfTriplets", DbType.Int32, _config.NumOfTriplets);
            db.AddInParameter(theCommand, "PresetTriplets", DbType.String, pt);
            db.AddInParameter(theCommand, "PresetFoils", DbType.String, pf);
            db.AddInParameter(theCommand, "PauseBetweenStimuli", DbType.Int32, _config.PauseBetweenStimuli);
            db.AddInParameter(theCommand, "StimulusDuration", DbType.Int32, _config.StimulusDuration);
            db.AddInParameter(theCommand, "TrainingNumOfTripletRepetitions", DbType.Int32, _config.TrainingNumOfTripletRepetitions);
            db.AddInParameter(theCommand, "TestingPauseBetweenTriplets", DbType.Int32, _config.TestingPauseBetweenTriplets);
            db.AddInParameter(theCommand, "InstructionsWelcome", DbType.String, _config.InstructionsWelcome);
            db.AddInParameter(theCommand, "InstructionsTraining", DbType.String, _config.InstructionsTraining);
            db.AddInParameter(theCommand, "InstructionsTesting", DbType.String, _config.InstructionsTesting);
            db.AddInParameter(theCommand, "InstructionsTestingQuestion", DbType.String, _config.InstructionsTestingQuestion);
            db.AddInParameter(theCommand, "InstructionsEnd", DbType.String, _config.InstructionsEnd);

            object obj = db.ExecuteScalar(theCommand);
            int configID = obj == null ? -1 : Int32.Parse(obj.ToString());

            Common.LogMessage(string.Format("sl_CreateConfig completed Successfully. ConfigID:{0}", configID));
            return configID;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            return -1;
        }
    }
    public static Dictionary<int, SL_Config> GetConfigs(string _configName)
    {
        try
        {
            Dictionary<int, SL_Config> retVal = new Dictionary<int, SL_Config>();
            int id;
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("sl_GetConfigs");
            if (_configName != null && _configName != string.Empty)
                db.AddInParameter(theCommand, "ConfigName", DbType.Int32, _configName);

            DataSet ds = db.ExecuteDataSet(theCommand);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            id = Common.SafeGetInt(dr["ID"]);

                            retVal.Add(id, GetConfigByID(id));
                        }
                    }
                    return retVal;
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            return null;
        }
    }
    public static SL_Config GetConfigByID(int _configID)
    {
        try
        {
            SL_Config config = null;
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("sl_GetConfigs");
            db.AddInParameter(theCommand, "ConfigID", DbType.Int32, _configID);

            DataSet ds = db.ExecuteDataSet(theCommand);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            config = new SL_Config();

                            config.ID = Common.SafeGetInt(dr["ID"]);
                            config.ConfigName = Common.SafeGetString(dr["ConfigName"]);
                            config.ExpType = (SL_ExpType)Common.SafeGetInt(dr["ExpType"]);
                            config.RandomizationType = (SL_RandomizationType)Common.SafeGetInt(dr["RandomizationType"]);
                            config.NumOfTriplets = Common.SafeGetInt(dr["NumOfTriplets"]);
                            config.PresetTriplets = SL_Config.ParseTripletsString(Common.SafeGetString(dr["PresetTriplets"]), SL_TripletBase.TRIPLET_TYPE_TRIPLET);
                            config.PresetFoils = SL_Config.ParseTripletsString(Common.SafeGetString(dr["PresetFoils"]), SL_TripletBase.TRIPLET_TYPE_FOIL);
                            config.PauseBetweenStimuli = Common.SafeGetInt(dr["PauseBetweenStimuli"]);
                            config.StimulusDuration = Common.SafeGetInt(dr["StimulusDuration"]);
                            config.TrainingNumOfTripletRepetitions = Common.SafeGetInt(dr["TrainingNumOfTripletRepetitions"]);
                            config.TestingPauseBetweenTriplets = Common.SafeGetInt(dr["TestingPauseBetweenTriplets"]);
                            config.InstructionsWelcome = Common.SafeGetString(dr["InstructionsWelcome"]);
                            config.InstructionsTraining = Common.SafeGetString(dr["InstructionsTraining"]);
                            config.InstructionsTesting = Common.SafeGetString(dr["InstructionsTesting"]);
                            config.InstructionsTestingQuestion = Common.SafeGetString(dr["InstructionsTestingQuestion"]);
                            config.InstructionsEnd = Common.SafeGetString(dr["InstructionsEnd"]);
                        }
                    }
                    return config;
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            return null;
        }
    }

    public static List<SL_Stimulus> GetStimuliByExpType(SL_ExpType _expType)
    {
        try
        {
            SL_ExpType expType;
            string id, type, filename, description;
            List<SL_Stimulus> retVal = new List<SL_Stimulus>();
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("sl_GetStimuli");
            db.AddInParameter(theCommand, "ExpType", DbType.Int32, (int)_expType);

            DataSet ds = db.ExecuteDataSet(theCommand);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            id = Common.SafeGetString(dr["ID"]);
                            expType = (SL_ExpType)Common.SafeGetInt(dr["ExpType"]);
                            type = Common.SafeGetString(dr["StimulusType"]);
                            filename = Common.SafeGetString(dr["Filename"]);
                            description = Common.SafeGetString(dr["Description"]);

                            retVal.Add(new SL_Stimulus(id, expType, type, filename, description));
                        }
                    }
                    return retVal;
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            return null;
        }
    }
    public static Dictionary<int, SL_Triplet> GetSessionTriplets(string _sessionID, int _expType, int _configID)
    {
        try
        {
            Dictionary<int, SL_Triplet> retVal = new Dictionary<int, SL_Triplet>();
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("sl_GetSessionTriplets");

            if (_sessionID != null && _sessionID != string.Empty)
                db.AddInParameter(theCommand, "SessionID", DbType.String, _sessionID);
            if (_expType != -1)
                db.AddInParameter(theCommand, "ExpType", DbType.Int32, _expType);
            if (_configID != -1)
                db.AddInParameter(theCommand, "ConfigID", DbType.Int32, _configID);

            DataSet ds = db.ExecuteDataSet(theCommand);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            SL_Triplet newPair = new SL_Triplet();

                            newPair.ID = Common.SafeGetInt(dr["ID"]);
                            newPair.SessionID = Common.SafeGetString(dr["SessionID"]);
                            newPair.SubjectID = Common.SafeGetString(dr["SubjectID"]);
                            newPair.A = Common.SafeGetString(dr["A"]);
                            newPair.B = Common.SafeGetString(dr["B"]);
                            newPair.C = Common.SafeGetString(dr["C"]);
                            newPair.TripletID = Common.SafeGetString(dr["TripletID"]);
                            newPair.Type = Common.SafeGetString(dr["Type"]);

                            retVal.Add(newPair.ID, newPair);
                        }
                    }
                    return retVal;
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            return null;
        }
    }
    public static Dictionary<int, SL_TrainingTrial> GetSessionTrainingTrials(string _sessionID, int _expType, int _configID)
    {
        try
        {
            Dictionary<int, SL_TrainingTrial> retVal = new Dictionary<int, SL_TrainingTrial>();
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("sl_GetSessionTrainingTrials");

            if (_sessionID != null && _sessionID != string.Empty)
                db.AddInParameter(theCommand, "SessionID", DbType.String, _sessionID);
            if (_expType != -1)
                db.AddInParameter(theCommand, "ExpType", DbType.Int32, _expType);
            if (_configID != -1)
                db.AddInParameter(theCommand, "ConfigID", DbType.Int32, _configID);

            DataSet ds = db.ExecuteDataSet(theCommand);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            SL_TrainingTrial newTrainingTrial = new SL_TrainingTrial();

                            newTrainingTrial.ID = Common.SafeGetInt(dr["ID"]);
                            newTrainingTrial.SessionID = Common.SafeGetString(dr["SessionID"]);
                            newTrainingTrial.SubjectID = Common.SafeGetString(dr["SubjectID"]);
                            newTrainingTrial.Config = GetConfigByID(Common.SafeGetInt(dr["ConfigID"]));
                            newTrainingTrial.TrialNumber = Common.SafeGetInt(dr["TrialNumber"]);
                            newTrainingTrial.A = Common.SafeGetString(dr["A"]);
                            newTrainingTrial.B = Common.SafeGetString(dr["B"]);
                            newTrainingTrial.C = Common.SafeGetString(dr["C"]);
                            newTrainingTrial.TripletID = Common.SafeGetString(dr["TripletID"]);
                            newTrainingTrial.Sex = Common.SafeGetString(dr["Sex"]);
                            newTrainingTrial.AgeY = Common.SafeGetInt(dr["AgeY"]);
                            newTrainingTrial.AgeM = Common.SafeGetInt(dr["AgeM"]);
                            newTrainingTrial.NativeLanguage = Common.SafeGetString(dr["NativeLanguage"]);
                            newTrainingTrial.Location = Common.SafeGetString(dr["Location"]);
                            newTrainingTrial.Remarks = Common.SafeGetString(dr["Remarks"]);

                            retVal.Add(newTrainingTrial.ID, newTrainingTrial);
                        }
                    }
                    return retVal;
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            return null;
        }
    }
    public static Dictionary<int, SL_TestingTrial> GetSessionTestingTrials(string _sessionID, int _expType, int _configID)
    {
        try
        {
            Dictionary<int, SL_TestingTrial> retVal = new Dictionary<int, SL_TestingTrial>();
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("sl_GetSessionTestingTrials");

            if (_sessionID != null && _sessionID != string.Empty)
                db.AddInParameter(theCommand, "SessionID", DbType.String, _sessionID);
            if (_expType != -1)
                db.AddInParameter(theCommand, "ExpType", DbType.Int32, _expType);
            if (_configID != -1)
                db.AddInParameter(theCommand, "ConfigID", DbType.Int32, _configID);

            DataSet ds = db.ExecuteDataSet(theCommand);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            SL_TestingTrial newTestingTrial = new SL_TestingTrial();

                            newTestingTrial.ID = Common.SafeGetInt(dr["ID"]);
                            newTestingTrial.SessionID = Common.SafeGetString(dr["SessionID"]);
                            newTestingTrial.SubjectID = Common.SafeGetString(dr["SubjectID"]);
                            newTestingTrial.TrialNumber = Common.SafeGetInt(dr["TrialNumber"]);
                            newTestingTrial.Config = GetConfigByID(Common.SafeGetInt(dr["ConfigID"]));
                            newTestingTrial.Stimulus1A = Common.SafeGetString(dr["Stimulus1A"]);
                            newTestingTrial.Stimulus1B = Common.SafeGetString(dr["Stimulus1B"]);
                            newTestingTrial.Stimulus1C = Common.SafeGetString(dr["Stimulus1C"]);
                            newTestingTrial.Stimulus2A = Common.SafeGetString(dr["Stimulus2A"]);
                            newTestingTrial.Stimulus2B = Common.SafeGetString(dr["Stimulus2B"]);
                            newTestingTrial.Stimulus2C = Common.SafeGetString(dr["Stimulus2C"]);
                            newTestingTrial.TripletID1 = Common.SafeGetString(dr["TripletID1"]);
                            newTestingTrial.TripletID2 = Common.SafeGetString(dr["TripletID2"]);
                            newTestingTrial.TrueTriplet = Common.SafeGetInt(dr["TrueTriplet"]);
                            newTestingTrial.Answer = Common.SafeGetInt(dr["Answer"]);
                            newTestingTrial.RT = Common.SafeGetInt(dr["RT"]);
                            newTestingTrial.SessionDuration = TimeSpan.Parse(Common.SafeGetObject(dr["SessionDuration"], "00:00:00").ToString());
                            newTestingTrial.Sex = Common.SafeGetString(dr["Sex"]);
                            newTestingTrial.AgeY = Common.SafeGetInt(dr["AgeY"]);
                            newTestingTrial.AgeM = Common.SafeGetInt(dr["AgeM"]);
                            newTestingTrial.NativeLanguage = Common.SafeGetString(dr["NativeLanguage"]);
                            newTestingTrial.Location = Common.SafeGetString(dr["Location"]);
                            newTestingTrial.Remarks = Common.SafeGetString(dr["Remarks"]);

                            retVal.Add(newTestingTrial.ID, newTestingTrial);
                        }
                    }
                    return retVal;
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            return null;
        }
    }

    public static bool SaveDemographics(string _sessionID, string _subjectID, string _sex, int _ageY, int _ageM, string _nativeLang, string _location, string _remarks)
    {
        try
        {
            long ageCombined = (_ageY * 1000) + _ageM;
            
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("sl_SaveDemographics");
            db.AddInParameter(theCommand, "SessionID", DbType.String, _sessionID);
            db.AddInParameter(theCommand, "SubjectID", DbType.String, _subjectID);
            db.AddInParameter(theCommand, "Sex", DbType.String, _sex);
            db.AddInParameter(theCommand, "Age", DbType.Int32, ageCombined);
            db.AddInParameter(theCommand, "NativeLanguage", DbType.String, _nativeLang);
            db.AddInParameter(theCommand, "Location", DbType.String, _location);
            db.AddInParameter(theCommand, "Remarks", DbType.String, _remarks);

            object countObj = db.ExecuteNonQuery(theCommand);
            int count;
            if (Int32.TryParse(countObj.ToString(), out count))
                if (count != 1)
                    return false;

            Common.LogMessage(string.Format("sl_SaveDemographics completed Successfully. SessionID:{0}", _sessionID));
            return true;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            return false;
        }
    }
}
