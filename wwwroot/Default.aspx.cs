using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


public partial class _Default : ExpBasePage 
{
    // Members
    ////////////////////////////////////////


    // Protected Methods
    ////////////////////////////////////////
    protected void Page_Load(object sender, EventArgs e)
    {
        InitSLConfigurationItems(pnlConfigItems);
    }
    protected override void InitializeMe()
    {
        base.InitializeMe();
    }
    protected void cmdLogin_Click(object sender, EventArgs e)
    {
        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["LoginPage"], false);
    }

    // Private Methods
    ////////////////////////////////////////
    private void InitSLConfigurationItems(Control container)
    {
        container.Controls.Clear();

        HtmlGenericControl header = new HtmlGenericControl("h2");
        header.Attributes["class"] = "configItemsExpName";
        header.InnerText = "SL";
        container.Controls.Add(header);

        foreach (SL_Config config in DB_SL.GetConfigs(null).Values)
            SL_Config.CreateConfigurationItem(container, config);
    }

    // Public Methods
    ////////////////////////////////////////

}
