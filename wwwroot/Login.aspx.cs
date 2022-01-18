using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Login : ExpBasePage
{
    // Members
    ////////////////////////////////////////


    // Protected Methods
    ////////////////////////////////////////
    protected void Page_Load(object sender, EventArgs e)
    {
        lblErrorMsg.Visible = false;

        bool loginClicked = Request[cmdLogin.UniqueID] != null;

        if (IsPostBack && loginClicked)
        {
            // Verify reCaptcha
            string EncodedResponse = Request.Form["g-recaptcha-response"];
            bool IsReCaptchaValid = ValidateReCaptchaResponse(EncodedResponse);

            if (!IsReCaptchaValid)
            {
                lblErrorMsg.Text = "Please complete the \"I'm not a robot\" task to login";
                lblErrorMsg.Visible = true;
                return;
            }

            // Verify login credentials
            ExpResult res = ExpUser.Login(txtUserID.Text, CryptLib.Hash(txtUserPW.Text));

            if (res.Code == ExpResultCode.OK)
            {
                if (lblBackTo.Text != string.Empty)
                {
                    Response.Redirect(Server.UrlDecode(lblBackTo.Text), false);
                }
                else
                {
                    if ((ExpUserRole)Common.SafeGetObject(Session["s_UserRole"], ExpUserRole.Guest) == ExpUserRole.Admin)
                        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["AdminPage"], false);

                    else
                        Response.Redirect(System.Configuration.ConfigurationManager.AppSettings["HomePage"], false);
                }
            }
            else
            {
                if (loginClicked)
                {
                    lblErrorMsg.Text = Common.ERRMSG_LOGIN_CREDENTIALS_INVALID;
                    if (res.Code == ExpResultCode.LoginFailed || res.Code == ExpResultCode.LoginLocked)
                        lblErrorMsg.Text = res.Message;
                    lblErrorMsg.Visible = true;
                }
            }
        }
        else
        {
            lblBackTo.Text = Request.QueryString["backto"];
            //txt_user_id.Focus();
        }
    }
    protected override void InitializeMe()
    {
        base.InitializeMe();
    }

    protected void cmdLogin_Click(object sender, EventArgs e)
    {
        // Not really necessary (everything is done in the Page_Load method).
    }


    private bool ValidateReCaptchaResponse(string _encodedResponse)
    {
        var client = new System.Net.WebClient();
        string PrivateKey = "6LdyixoUAAAAAHo71PA4PcCCx93BzPieSIEe5nKa";
        var GoogleReply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", PrivateKey, _encodedResponse));
        JavaScriptSerializer js = new JavaScriptSerializer();
        Dictionary<string, object> data = js.Deserialize<Dictionary<string, object>>(GoogleReply);
        return data["success"].ToString().ToLower() == "true";
    }
}
