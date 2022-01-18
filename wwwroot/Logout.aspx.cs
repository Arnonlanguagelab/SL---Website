using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Logout : ExpBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Common.LogMessage(string.Format("Logout completed Successfully. ID:{0}", Session["s_UserID"]));
        Session.Abandon();
        Common.ClearApplicationCache(Cache);
        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["HomePage"], false);
    }
}
