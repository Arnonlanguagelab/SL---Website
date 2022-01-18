using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Summary description for DBUser
/// </summary>
public class DBUser
{
	public DBUser()
	{

	}

    public static ExpUser Login(string _id, string _pw)
    {
        try
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("Login");
            db.AddInParameter(theCommand, "user_id", DbType.String, _id);
            db.AddInParameter(theCommand, "user_pw", DbType.String, _pw);

            object obj = db.ExecuteScalar(theCommand);
            int count;
            if (Int32.TryParse(obj.ToString(), out count))
                if (count != 1)
                    return null;

            Common.LogMessage(string.Format("Login completed Successfully. ID:{0}", _id));
            return GetUserByID(_id);
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            return null;
        }
    }
    public static ExpUser GetUserByID(string _id)
    {
        try
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("GetUserByID");
            db.AddInParameter(theCommand, "user_id", DbType.String, _id);
            
            DataSet ds = db.ExecuteDataSet(theCommand);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        ExpUser usobj = new ExpUser(_id);
                        int roleid;
                        ExpUserRole role = Int32.TryParse(dr["user_role"].ToString(), out roleid) ? (ExpUserRole)roleid : ExpUserRole.Guest;
                        usobj.Role = role;
                        usobj.Password = Common.SafeGetString(dr["user_pw"]);
                        usobj.FirstName = Common.SafeGetString(dr["first_name"]);
                        usobj.LastName = Common.SafeGetString(dr["last_name"]);
                        usobj.Email = Common.SafeGetString(dr["email"]);
                        usobj.RegTime = Common.SafeGetDateTime(dr["reg_time"]);

                        return usobj;
                    }
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
    public static bool CheckIfUserExists(string _id, string _firstName, string _lastName, string _email)
    {
        try
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("CheckIfUserExists");
            db.AddInParameter(theCommand, "user_id", DbType.String, _id);
            db.AddInParameter(theCommand, "first_name", DbType.String, _firstName);
            db.AddInParameter(theCommand, "last_name", DbType.String, _lastName);
            db.AddInParameter(theCommand, "email", DbType.String, _email);

            object countObj = db.ExecuteScalar(theCommand);
            int count;
            if (Int32.TryParse(countObj.ToString(), out count))
                if (count > 0)
                    return true;

            return false;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            throw (ex);
        }
    }
    public static bool RegisterNewUser(ExpUserRole _role, string _id, string _pw, string _firstName, string _lastName, string _email)
    {
        try
        {
            DateTime regDate = Common.GetIsraelTimeNow();
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("RegisterNewUser");
            db.AddInParameter(theCommand, "user_role", DbType.Int32, (int)_role);
            db.AddInParameter(theCommand, "user_id", DbType.String, _id);
            db.AddInParameter(theCommand, "user_pw", DbType.String, _pw);
            db.AddInParameter(theCommand, "first_name", DbType.String, _firstName);
            db.AddInParameter(theCommand, "last_name", DbType.String, _lastName);
            db.AddInParameter(theCommand, "email", DbType.String, _email);
            db.AddInParameter(theCommand, "reg_time", DbType.DateTime, regDate);
            
            object countObj = db.ExecuteScalar(theCommand);
            int count;
            if (Int32.TryParse(countObj.ToString(), out count))
                if (count != 1)
                    return false;

            Common.LogMessage(string.Format("RegisterNewUser completed Successfully. ID:{0} | Email:{1}", _id, _email));
            return true;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            return false;
        }
    }
    public static Dictionary<string, ExpUser> GetAllUsers() { return GetAllUsers(string.Empty, string.Empty); }
    public static Dictionary<string, ExpUser> GetAllUsers(string _filterExp, string _sortExp)
    {
        try
        {
            Dictionary<string, ExpUser> retVal = new Dictionary<string, ExpUser>();
            string userID;
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("GetRegisteredUsers");
            
            DataSet ds = db.ExecuteDataSet(theCommand);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Select(_filterExp, _sortExp))
                        {
                            userID = Common.SafeGetString(dr["user_id"]);

                            ExpUser user = DBUser.GetUserByID(userID);

                            retVal.Add(userID, user);
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
    public static bool UpdateUserDetails(string _id, string _firstName, string _lastName, string _email)
    {
        try
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("UpdateUserDetails");
            db.AddInParameter(theCommand, "user_id", DbType.String, _id);
            db.AddInParameter(theCommand, "first_name", DbType.String, _firstName);
            db.AddInParameter(theCommand, "last_name", DbType.String, _lastName);
            db.AddInParameter(theCommand, "email", DbType.String, _email);

            object countObj = db.ExecuteNonQuery(theCommand);
            int count;
            if (Int32.TryParse(countObj.ToString(), out count))
                if (count != 1)
                    return false;

            Common.LogMessage(string.Format("UpdateUserDetails completed Successfully. ID:{0}", _id));
            return true;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            return false;
        }
    }
    public static bool UpdateUserPassword(string _id, string _oldPass, string _newPass)
    {
        try
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("UpdateUserPassword");
            db.AddInParameter(theCommand, "user_id", DbType.String, _id);
            db.AddInParameter(theCommand, "old_pass", DbType.String, _oldPass);
            db.AddInParameter(theCommand, "new_pass", DbType.String, _newPass);

            object countObj = db.ExecuteNonQuery(theCommand);
            int count;
            if (Int32.TryParse(countObj.ToString(), out count))
                if (count != 1)
                    return false;

            Common.LogMessage(string.Format("UpdateUserPassword completed Successfully. ID:{0}", _id));
            return true;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            return false;
        }
    }

    public static Dictionary<string, ExpUser> SearchUsers(int _userRole, string _userID, string _userName, string _email)
    {
        try
        {
            Dictionary<string, ExpUser> retVal = new Dictionary<string, ExpUser>();
            string userID, newID;
            int year;
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("SearchUsers");
            db.AddInParameter(theCommand, "user_role", DbType.Int32, _userRole);
            db.AddInParameter(theCommand, "user_id", DbType.String, _userID);
            db.AddInParameter(theCommand, "user_name", DbType.String, _userName);
            db.AddInParameter(theCommand, "email", DbType.String, _email);

            DataSet ds = db.ExecuteDataSet(theCommand);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            userID = Common.SafeGetString(dr["user_id"]);
                            year = Common.SafeGetInt(dr["year"]);

                            ExpUser user = DBUser.GetUserByID(userID);

                            newID = string.Format("{0}@{1}", userID, year);

                            retVal.Add(newID, user);
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
    public static bool DeleteUser(string _userID, int _year)
    {
        try
        {
            ExpUser user = GetUserByID(_userID);

            Database db = DatabaseFactory.CreateDatabase();
            DbCommand theCommand = db.GetStoredProcCommand("DeleteUser");
            db.AddInParameter(theCommand, "user_id", DbType.String, _userID);
            db.AddInParameter(theCommand, "year", DbType.Int32, _year);

            object resObj = db.ExecuteScalar(theCommand);
            int res;
            if (Int32.TryParse(resObj.ToString(), out res))
            {
                if (res == 1)
                {
                    Common.LogMessage(string.Format("DeleteUser completed successfully. ID:{0}|Role:{1}|Name:{2}|Email:{3}",
                                                    _userID, user.Role, user.FirstName + " " + user.LastName, user.Email));
                    return true;
                }
            }

            Common.LogMessage(string.Format("DeleteUser FAILED. ID:{0}", _userID), ExpLogType.Warning);
            return false;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            return false;
        }
    }    
}
