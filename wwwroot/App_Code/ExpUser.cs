using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;



// Class: ExpUser
// (contains data from the [user] table)
////////////////////////////////////////////////////
public class ExpUser
{
    // Members / Properties
    ////////////////////////////////////////
    public string       ID          { get; set; }
    public ExpUserRole  Role        { get; set; }
    public string       Password    { get; set; }
    public string       FirstName   { get; set; }
    public string       LastName    { get; set; }
    public string       Email       { get; set; }
    public DateTime     RegTime     { get; set; }


    // Public Methods
    ////////////////////////////////////////
    public ExpUser()
    {
        
    }
    public ExpUser(string _id)
	{
        ID = _id;
    }


    // Public Static Methods
    ////////////////////////////////////////
    public static ExpResult Login(string _id, string _pw)
    {
        ExpUser user = DBUser.Login(_id, _pw);

        if (user != null)
        {
            SetupSession(user);
            return ExpResult.OK;
        }
        else
        {
            return new ExpResult(ExpResultCode.LoginFailed);
        }
    }
    public static ExpResult RegisterNewUser(ExpUserRole _role, string _id, string _pw, string _firstName, string _lastName, string _email)
    {
        if (DBUser.CheckIfUserExists(_id, _firstName, _lastName, _email))
            return new ExpResult(ExpResultCode.UserAlreadyExists);

        if (!DBUser.RegisterNewUser(_role, _id, _pw, _firstName, _lastName, _email))
            return new ExpResult(ExpResultCode.UnknownError, Common.ERRMSG_REGISTRATION_FAILED);

        return ExpResult.OK;
    }


    // Private Methods
    ////////////////////////////////////////
    private string GetStrippedEmail()
    {
        return Email.Replace("@", "").Replace(".", "");
    }

    // Private Static Methods
    ////////////////////////////////////////
    private static void SetupSession(ExpUser _userObj)
    {
        HttpContext.Current.Session["s_UserObj"] = _userObj;
        HttpContext.Current.Session["s_UserID"] = _userObj.ID;
        HttpContext.Current.Session["s_UserRole"] = (int)_userObj.Role;
    }
}
