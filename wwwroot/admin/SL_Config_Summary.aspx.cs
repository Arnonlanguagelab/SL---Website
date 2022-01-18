using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Xsl;

public partial class admin_SL_Config_Summary : ExpPage
{
    public admin_SL_Config_Summary()
    {
        m_Role = ExpUserRole.Admin;
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        InitConfigurationItems(pnlConfigItems);
    }

    protected void cmdBack_Click(object sender, EventArgs e)
    {
        Response.Redirect(ConfigurationManager.AppSettings["AdminSLConfigPage"], false);
    }
    protected override void InitializeMe()
    {
        base.InitializeMe();
    }

    private void InitConfigurationItems(Control container)
    {
        container.Controls.Clear();

        foreach (SL_Config config in DB_SL.GetConfigs(null).Values)
            SL_Config.CreateConfigurationItem(container, config);
    }
}
