using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

/// Session Members (just a list to remember them)
/// "s_UserObj"
/// "s_UserID" 
/// "s_UserRole"
/// 


// Enumerations
////////////////////////////////////////
public enum ExpUserRole
{
    Guest       = 0,
    Admin       = 1
}
public enum ExpResultCode
{
    OK                  = 0,
    UnknownError        = 1,
    UserAlreadyExists   = 2,
    LoginFailed         = 3,
    LoginLocked         = 4
}
public enum ExpLogType
{
    Info    = 0,
    Warning = 1,
    Error   = 2
}
public enum ExpName
{ 
    SL = 1
}



// Class: ExpResult 
// (a class to be returned by methods)
////////////////////////////////////////
public class ExpResult
{
    // Members
    ////////////////////////////////////////
    ExpResultCode   m_Code;
    string          m_Message;
    Exception       m_Exception;

    // Constructors
    ////////////////////////////////////////
    public ExpResult(Exception ex)
    {
        Code = ExpResultCode.UnknownError;
        Exception = ex;
        m_Message = ex.Message;
    }
    public ExpResult(ExpResultCode _code)
    {
        Code = _code;
        Message = null;
        Exception = null;
    }
    public ExpResult(ExpResultCode _code, string _message)
    {
        Code = _code;
        Message = _message;
        Exception = null;
    }

    // Properties
    ////////////////////////////////////////
    public static ExpResult OK
    {
        get { return new ExpResult(ExpResultCode.OK); }
    }

    public ExpResultCode Code
    {
        get { return m_Code; }
        set { m_Code = value; }
    }
    public string Message
    {
        get 
        { 
            if (m_Message != null)
                return m_Message;

            return CodeMessage(m_Code);
        }
        set { m_Message = value; }
    }
    public Exception Exception
    {
        get { return m_Exception != null ? m_Exception : new Exception(Message); }
        set { m_Exception = value; }
    }

    // Private Methods
    ////////////////////////////////////////
    private string CodeMessage(ExpResultCode _code)
    {
        switch (_code)
        { 
            case ExpResultCode.OK:
                return string.Empty;
            case ExpResultCode.UnknownError:
                return "An unknown error has occurred.";
            case ExpResultCode.UserAlreadyExists:
                return "User already exists.";
            case ExpResultCode.LoginFailed:
                return Common.ERRMSG_LOGIN_CREDENTIALS_INVALID;

            default:
                return string.Empty;
        }
    }
}



// Class: ExpCSVObject 
// (a class to contain csv reports' data)
//////////////////////////////////////////
public class ExpCSVObject
{
    // Members / Properties
    ////////////////////////////////////////
    public List<string>        Headers { get; set; }
    public List<List<string>>  Data    { get; set; }

    // Constructors
    ////////////////////////////////////////
    public ExpCSVObject(int _numOfColumns)
    {
        Headers = new List<string>(_numOfColumns);
        Data = new List<List<string>>(_numOfColumns);
    }

    // Public Methods
    ////////////////////////////////////////
    public string GetDataAt(int _row, int _col)
    {
        string retVal = string.Empty;

        if (Data != null)
            if (_row >= 0 && _row < Data.Count)
                if (Data[_row] != null)
                    if (_col >= 0 && _col < Data[_row].Count)
                        retVal = Data[_row][_col];

        return retVal;
    }
    public string GetDelimitedHeaders(string _delimiter)
    {
        string retVal = string.Empty;

        if (Headers == null)
            return string.Empty;

        foreach (string str in Headers)
            retVal = retVal + str + _delimiter;

        // exclude the last delimiter.
        int index = retVal.LastIndexOf(_delimiter);
        index = index == -1 ? 0 : index;
        
        return retVal.Substring(0, index); 
    }
    public string GetDelimitedDataRow(string _delimiter, int _row)
    {
        string retVal = string.Empty;

        if (Data == null || _row >= Data.Count || Data[_row] == null)
            return string.Empty;

        foreach (string str in Data[_row])
            retVal = retVal + str + _delimiter;

        // exclude the last delimiter.
        int index = retVal.LastIndexOf(_delimiter);
        index = index == -1 ? 0 : index;
        
        return retVal.Substring(0, index); 
    }
    public int AddHeader(string _header)
    {
        if (Headers == null)
            return -1;

        Headers.Add(_header);
        return (Headers.Count - 1);
    }
    public int AddDataRow(List<string> _dataRow)
    {
        if (Data == null)
            return -1;

        Data.Add(_dataRow);
        return (Data.Count - 1);
    }
    public int AddDataToRow(int _row, string _data)
    {
        if (Data == null)
            return -1;

        if (Data[_row] == null)
            return -1;

        Data[_row].Add(_data);
        return (Data[_row].Count - 1);
    }
}



// Class: Common
// (contains common data and methods)
////////////////////////////////////////
public class Common
{   
    // Public Static Members & Consts
    ////////////////////////////////////////
    public const int ADMIN_TIMEOUT_MINUTES      = 45;
    public const int USER_TIMEOUT_MINUTES       = 15;
    public const int LOGIN_ATTEMPTS_BEFORE_LOCK = 5;
    public const int LOGIN_LOCK_TIME_MINUTES    = 30;
    public const int SQL_LOG_MESSAGE_MAX_LENGTH = 2500;

    public const char PARAMS_URL_DELIMITER = '|';
    public const char CSV_RESULTS_DELIMITER = ';';
    public const string CSV_REPORT_DELIMITER = ",";
    public static string[] CSV_WHITESPACE_DELIMITERS = { ",", " ", "\t", "\r", "\r\n", "\n\r", "n", Environment.NewLine };

    public const string FORMAT_TIME = "HH:mm";
    public const string FORMAT_DATE = "dd/MM/yyyy";
    public const string FORMAT_DATE_TIME = "dd/MM/yyyy HH:mm";
    public const string FORMAT_DATE_TIME_SECONDS = "dd/MM/yyyy HH:mm:ss";
    public const string FORMAT_DATE_TIME_DB = "yyyy-MM-dd HH:mm:ss";

    public const string ERRMSG_LOGIN_CREDENTIALS_INVALID = "Username and password do not match any user in the system.";
    public const string ERRMSG_REGISTRATION_FAILED = "User registration failed for an unknown reason.";

    public const string STR_DEFAULT_LOG_SOURCE = "ExpWeb";


    // Public Static Methods
    ////////////////////////////////////////
    public static void LogMessage(string _message, ExpLogType _type, string _source)
    {
        DBCommon.DBLog(_message, _type, _source);
    }
    public static void LogMessage(string _message, ExpLogType _type)
    {
        LogMessage(_message, _type, STR_DEFAULT_LOG_SOURCE);
    }
    public static void LogMessage(string _message)
    {
        LogMessage(_message, ExpLogType.Info);
    }
    public static void LogMessage(Exception _exception)
    {
        if (_exception is HttpUnhandledException || _exception is System.Threading.ThreadAbortException) 
            return;

        StringBuilder message = new StringBuilder();
        message.Append("Exception: " + _exception.GetType() + " ");
        message.Append(_exception.Message);
        message.Append("\n");
        if (_exception.InnerException != null)
        {
            message.Append("InnerException: ");
            message.Append(_exception.InnerException.ToString());
            message.Append("\n");
        }
        message.Append("Source: ");
        message.Append(_exception.Source);
        message.Append("\n");
        message.Append("\nStack: ");
        message.Append(_exception.StackTrace);
        LogMessage(message.ToString(), ExpLogType.Error);

        if (_exception is WebException)
        {
            if (((WebException)_exception).Response is FtpWebResponse)
            {
                message = new StringBuilder();
                message.Append("FtpWebResponse: \n");
                message.Append("StatusCode: " + ((FtpWebResponse)((WebException)_exception).Response).StatusCode + " \n");
                message.Append("StatusDescription: " + ((FtpWebResponse)((WebException)_exception).Response).StatusDescription + " \n");
                message.Append("WelcomeMessage: " + ((FtpWebResponse)((WebException)_exception).Response).WelcomeMessage + " \n");
                message.Append("ExitMessage: " + ((FtpWebResponse)((WebException)_exception).Response).ExitMessage + " \n");
                LogMessage(message.ToString(), ExpLogType.Error);
            }
        }
    }
    public static void ShowClientSideAlert(string _message, Page _executingPage)
    {
        _executingPage.Response.Write("<script>alert('" + _message.Replace("\'","\'\'").Replace("\"","\'") + "')</script>");
    }
    public static void ShowClientSideAlertAndRedirect(string _message, Page _executingPage, string _url)
    {
        _executingPage.Response.Write("<script>alert('" + _message.Replace("\'", "\'\'").Replace("\"", "\'") + "'); window.location = '" + _url + "'; </script>");
    }
    public static void ClearApplicationCache(System.Web.Caching.Cache _cache)
    {
        List<string> keys = new List<string>();
        IDictionaryEnumerator enumerator = _cache.GetEnumerator();

        while (enumerator.MoveNext())
            keys.Add(enumerator.Key.ToString());

        for (int i = 0; i < keys.Count; i++)
            _cache.Remove(keys[i]);
    }
    public static bool AcceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }    
    public static object SafeGetObject(object _obj, object _defaultValue)
    {
        return ((_obj == null || _obj == System.DBNull.Value) ? _defaultValue : _obj);
    }
    public static string SafeGetString(object _obj)
    {
        return SafeGetObject(_obj, string.Empty).ToString();
    }
    public static bool SafeGetBit(object _obj)
    {
        bool retVal = false;
        bool.TryParse(SafeGetString(_obj), out retVal);
        return retVal;
    }
    public static DateTime SafeGetDateTime(object _obj)
    {
        DateTime retVal = DateTime.MinValue;
        DateTime.TryParse(SafeGetString(_obj), out retVal);
        return retVal;
    }
    public static char SafeGetChar(object _obj, object _defaultValue)
    {
        return SafeGetObject(_obj, _defaultValue).ToString()[0];
    }
    public static int SafeGetInt(object _obj)
    {
        int retVal;
        int.TryParse(SafeGetObject(_obj, -1).ToString(), out retVal);
        return retVal;
    }
    public static double SafeGetDouble(object _obj)
    {
        double retVal;
        double.TryParse(SafeGetObject(_obj, -1).ToString(), out retVal);
        return retVal;
    }
    public static List<string> SplitStringByLength(string _str, int _maxLength)
    {
        List<string> retVal = new List<string>();

        while (_str.Length > 0)
        {
            int len = _str.Length > _maxLength ? _maxLength : _str.Length;
            retVal.Add(_str.Substring(0, len));
            _str = _str.Substring(len);
        }

        return retVal;
    }
    public static DateTime GetIsraelTimeNow()
    {
        return GetIsraelTime(DateTime.Now);
    }
    public static DateTime GetIsraelTime(DateTime _dt)
    {
        return TimeZoneInfo.ConvertTime(_dt, TimeZoneInfo.Local, TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time"));
    }
    public static DateTime ConvertJSTime(long _milliseconds)
    {
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return epoch.AddMilliseconds(_milliseconds);
    }
    public static void GenerateCSVReport(Page _callingPage, string _fullFilename, ExpCSVObject _csvObject, params ExpCSVObject[] _legends)
    {
        _callingPage.Response.Clear();
        _callingPage.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", _fullFilename));
        _callingPage.Response.AddHeader("content-type", "application/csv");

        using (StreamWriter writer = new StreamWriter(_callingPage.Response.OutputStream, Encoding.UTF8))
        {
            foreach (ExpCSVObject legend in _legends)
            {
                if (legend.Headers.Count > 0)
                    writer.WriteLine(legend.GetDelimitedHeaders(CSV_REPORT_DELIMITER));

                for (int i = 0; i < legend.Data.Count; i++)
                    writer.WriteLine(legend.GetDelimitedDataRow(CSV_REPORT_DELIMITER, i));

                //writer.WriteLine(string.Empty); // leave blank line spcae
            }

            if (_csvObject.Headers.Count > 0)
                writer.WriteLine(_csvObject.GetDelimitedHeaders(CSV_REPORT_DELIMITER));

            for (int i = 0; i < _csvObject.Data.Count; i++)
                writer.WriteLine(_csvObject.GetDelimitedDataRow(CSV_REPORT_DELIMITER, i));
        }
        _callingPage.Response.End();
    }
    public static string GenerateRandomPassword(int _length)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        string retVal = string.Empty;
        Random random = new Random();

        for (int i = 0; i < _length; i++)
            retVal += chars[random.Next(0, chars.Length)];

        return retVal;
    }
    public static string GenerateUniqueUnsubscriptionLink(string _email)
    {
        string url, url_enc, appRoot, supportEmail;
        url = string.Format("{0}", _email);
        url_enc = CryptLib.Encrypt(url);

        appRoot = System.Configuration.ConfigurationManager.AppSettings["AppRoot"];
        supportEmail = System.Configuration.ConfigurationManager.AppSettings["EmailSupport"];

        return string.Format("<br/><br/><div style='font-family:Arial;font-size:11px;direction:ltr;text-align:left;'>In the past you have provided the automated matching system with your email address. If you no longer want to receive these reminders, please click <a href='{0}/unsubscribe.aspx?ue={1}'>unsubscribe</a> to be removed from the mailing list.<br/>If you need our assistance please contact us at this address <a href='mailto:{2}'>{2}</a>.</div>", appRoot, url_enc, supportEmail);
    }
    public static bool IsIDNumberValid(string _id)
    {
        if (_id.Length != 9)
            return false;

        int intID;
        if (!Int32.TryParse(_id, out intID))
            return false;

        string t8 = _id.Substring(0, _id.Length - 1);

        return (t8 + CalculateCheckDigit(t8) == _id);
    }
    

    // Private Static Methods
    ////////////////////////////////////////
    private static string CalculateCheckDigit(string _id)
    {
        if (_id.Length != 8)
            return string.Empty;

        int sum = 0;
        for (int i = 0; i < _id.Length; i++)
        {
            int x = Convert.ToInt32(_id[i].ToString());
            if ((i + 1) % 2 == 0) // if the digit is in even position, multiply it by 2.
                x = x * 2;

            if (x > 9) // if digit value is higher than 9, mod 10 and add 1.
                x = (x % 10) + 1;

            sum += x; // sum up all manipulated digits.
        }
        int retVal = (10 - sum % 10) % 10;
        return retVal.ToString();
    }
}