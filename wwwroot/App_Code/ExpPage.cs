using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Threading;
using System.Globalization;
using System.Data;
using System.Text;
using System.IO;

/// <summary>
/// Summary description for ExpPage
/// </summary>
public class ExpPage : ExpBasePage
{
    // Members
    ////////////////////////////////////////
    public ExpUser      m_User;
    public string       m_UserID                    = string.Empty;
    public bool         m_AllowAnononymousAccess    = false;
    public ExpUserRole  m_Role                      = ExpUserRole.Guest;


    // Public Methods
    ////////////////////////////////////////
	public ExpPage()
	{
        this.Init += new EventHandler(ExpPage_Init);
        this.Unload += new EventHandler(ExpPage_Unload);
	}


    // Private Methods
    ////////////////////////////////////////
    void ExpPage_Unload(object sender, EventArgs e)
    {
        Session["s_UserObj"] = m_User;
    }
    void ExpPage_Init(object sender, EventArgs e)
    {
        //// overriden by the specific .aspx page to set the m_Role variable according to the user logged in.
        //InitializeMe();

        // set the Session.Timeout according to the user logged in (longer timeout for admins).
        if (m_Role == ExpUserRole.Admin)
            Session.Timeout = Common.ADMIN_TIMEOUT_MINUTES;
        else
            Session.Timeout = Common.USER_TIMEOUT_MINUTES;

        // if this page requires login and no user is logged in - redirect to login page
        // (with a parameter of the url of the page to redirect to after login is done).
        if (!m_AllowAnononymousAccess && (Session["s_UserObj"] == null))
        {
            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["LoginPage"] + Server.UrlEncode(Request.Url.ToString()), true);
            return;
        }

        m_User = (ExpUser)Session["s_UserObj"];
        ExpUserRole currole = ExpUserRole.Guest;
        if (m_User != null)
        {
            m_UserID = m_User.ID;
            try { currole = m_User.Role; }
            catch (Exception) { currole = 0; }
        }

        // in case there's a mismatch between the page's m_Role and the currently logged-in user's role - redirect to default role page.
        // this means that every ExpPage object needs to override the m_Role member with the correct value.
        if (!m_AllowAnononymousAccess && currole != m_Role)
        {
            if (currole == ExpUserRole.Admin)
                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["AdminPage"], true);
            else
                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["DefaultPage"], true);
        }
    }
}
