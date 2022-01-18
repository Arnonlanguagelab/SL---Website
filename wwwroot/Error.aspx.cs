using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Error : ExpBasePage
{
    protected void Page_PreRender(object sender, EventArgs e)
    {
        try
        {
            //object userIDObj = Session["s_UserID"];
            //if (userIDObj == null) // no session is running.
            //    return;

            //if (Request.Params[0].Contains("404")) // if a 404 error, do not log.
            //    return;

            //string errMsg = string.Empty;
            //foreach (string key in Request.Params.AllKeys)
            //{
            //    errMsg += string.Format("{0}={1}; ", key, Request.Params[key]);
            //}
            //errMsg = errMsg.Trim();
            //if (errMsg.EndsWith(";"))
            //    errMsg = errMsg.TrimEnd(';');

            //Common.LogMessage(string.Format("ERROR! ID:{0} PARAMS:{1}", userIDObj.ToString(), errMsg), ExpLogType.Error);
            
            //Session.Abandon();
            //Common.ClearApplicationCache(Cache);
        }
        catch
        { 

        }
    }
}
