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

public partial class admin_Default : ExpPage
{
    public admin_Default()
    {
        m_Role = ExpUserRole.Admin;
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected override void InitializeMe()
    {
        base.InitializeMe();
    }
    protected void cmdSLData_Click(object sender, EventArgs e)
    {
        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["AdminSLDataPage"], false);
    }
    protected void cmdSLConfig_Click(object sender, EventArgs e)
    {
        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["AdminSLConfigPage"], false);
    }
    protected void cmdLogout_Click(object sender, EventArgs e)
    {
        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["LogoutPage"], false);
    }
}
