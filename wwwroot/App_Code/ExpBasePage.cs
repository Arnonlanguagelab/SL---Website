using System;
using System.Linq;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;

/// <summary>
/// Summary description for ExpBasePage
/// </summary>
public class ExpBasePage : Page
{
    public ExpBasePage()
	{
        this.Init += new EventHandler(ExpBasePage_Init);
	}

    protected void ExpBasePage_Init(object sender, EventArgs e)
    {
        InitializeMe();
    }
    
    protected virtual void InitializeMe()
    {

    }
    protected override void InitializeCulture()
    {

        string selectedLanguage;
        if (Session["MyCulture"] != null)
        {
            selectedLanguage = Session["MyCulture"].ToString();
        }
        else if (Request.Cookies["lang"] != null)
        {
            selectedLanguage = Request.Cookies["lang"].Value;
            Session["MyCulture"] = selectedLanguage;
        }
        else
        {
            selectedLanguage = "en-US";
            Session["MyCulture"] = selectedLanguage;
        }
        UICulture = selectedLanguage;
        Culture = selectedLanguage;

        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(selectedLanguage);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(selectedLanguage);
        
        base.InitializeCulture();
        
        Response.Cookies["lang"].Value = selectedLanguage;
        Response.Cookies["lang"].Path = "/";
        Response.Cookies["lang"].Expires = DateTime.Today.AddDays(14);
    }
}
