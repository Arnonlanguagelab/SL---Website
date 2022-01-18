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

public partial class admin_SL_Config : ExpPage
{
    public admin_SL_Config()
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
    protected void cmdSLConfigSummary_Click(object sender, EventArgs e)
    {
        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["AdminSLConfigSummaryPage"], false);
    }
    protected void cmdSLConfigCreate_Click(object sender, EventArgs e)
    {
        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["AdminSLConfigCreatePage"], false);
    }
    protected void cmdBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["AdminPage"], false);
    }
}
