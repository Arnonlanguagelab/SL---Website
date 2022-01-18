using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;


/// <summary>
/// Summary description for Cryptography
/// </summary>
public class CryptLib
{
    // Private Static Members & Consts
    ////////////////////////////////////////    
    private const string ENCRYPTION_KEY = "s9vQZ24"; // Key should be up to 7 characters.

    // Public Methods
    ////////////////////////////////////////
    public CryptLib()
	{

	}
    public static string Encrypt(string _url)
    {
        try
        {
            Page page = new ExpPage();

            string encrypted_url = Crypto.EncryptStringAES(_url, ENCRYPTION_KEY);
            string encoded_url = page.Server.UrlEncode(encrypted_url);
            return encoded_url;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            throw (ex);
        }
    }
    public static string Decrypt(string _url)
    {
        bool flag = false;
        Page callingPage = new ExpPage();
        string decrypted_url = string.Empty;

        // decode the url.
        string decoded_url = callingPage.Server.UrlDecode(_url);

        // try decrypting the decoded url.
        try { decrypted_url = Crypto.DecryptStringAES(decoded_url, ENCRYPTION_KEY); flag = true; }
        catch { flag = false; }

        // if that failed - try decrypting the original url (before decoding)
        if (!flag)
        {
            try { decrypted_url = Crypto.DecryptStringAES(_url, ENCRYPTION_KEY); flag = true; }
            catch (Exception ex) { Common.LogMessage(ex); }
        }

        // return decrypted string. if both decryption method failed, an empty string is returned.
        return decrypted_url;
    }
    public static string Hash(string _str)
    {
        try
        {
            string retVal = Hashing.Hash(_str, ENCRYPTION_KEY);
            return retVal;
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex);
            throw (ex);
        }
    }
    public static string TryDecodeHash(string _str)
    {
        string retVal = string.Empty;
        try { retVal = Hashing.TryDecodeHash(_str, ENCRYPTION_KEY); }
        catch (Exception ex) { Common.LogMessage(ex); }
        return retVal;
    }
}

public class Hashing
{
    private static SymmetricAlgorithm m_ObjCryptoService = new DESCryptoServiceProvider();
    
    public static string Hash(string _source, string _key)
    {
        byte[] bytIn = System.Text.ASCIIEncoding.ASCII.GetBytes(_source);
        System.IO.MemoryStream ms = new System.IO.MemoryStream();

        byte[] bytKey = GetLegalKey(_key);

        m_ObjCryptoService.Key = bytKey;
        m_ObjCryptoService.IV = bytKey;

        ICryptoTransform encrypto = m_ObjCryptoService.CreateEncryptor();

        CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);

        cs.Write(bytIn, 0, bytIn.Length);
        cs.FlushFinalBlock();

        byte[] bytOut = ms.GetBuffer();
        int i = 0;
        for (i = 0; i < bytOut.Length; i++)
            if (bytOut[i] == 0)
                break;

        return System.Convert.ToBase64String(bytOut, 0, i);
    }
    public static string TryDecodeHash(string _source, string _key)
    {
        byte[] bytIn = System.Convert.FromBase64String(_source);

        System.IO.MemoryStream ms = new System.IO.MemoryStream(bytIn, 0, bytIn.Length);
        ms.Position = 0;

        byte[] bytKey = GetLegalKey(_key);

        m_ObjCryptoService.Key = bytKey;
        m_ObjCryptoService.IV = bytKey;

        ICryptoTransform encrypto = m_ObjCryptoService.CreateDecryptor();

        CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);

        System.IO.StreamReader sr = new System.IO.StreamReader(cs);
        return sr.ReadToEnd();
    }
    private static byte[] GetLegalKey(string _key)
    {
        string sTemp;
        if (m_ObjCryptoService.LegalKeySizes.Length > 0)
        {
            int lessSize = 0, moreSize = m_ObjCryptoService.LegalKeySizes[0].MinSize;

            while (_key.Length * 8 > moreSize)
            {
                lessSize = moreSize;
                moreSize += m_ObjCryptoService.LegalKeySizes[0].SkipSize;
            }
            sTemp = _key.PadRight(moreSize / 8, ' ');
        }
        else
            sTemp = _key;


        return ASCIIEncoding.ASCII.GetBytes(sTemp);
    }
}

public class Crypto
{
    private static byte[] _salt = Encoding.ASCII.GetBytes("o6806642kbM7c5");

    /// <summary>
    /// Encrypt the given string using AES.  The string can be decrypted using 
    /// DecryptStringAES().  The sharedSecret parameters must match.
    /// </summary>
    /// <param name="plainText">The text to encrypt.</param>
    /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
    public static string EncryptStringAES(string plainText, string sharedSecret)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentNullException("plainText");
        if (string.IsNullOrEmpty(sharedSecret))
            throw new ArgumentNullException("sharedSecret");

        string outStr = null;                       // Encrypted string to return
        RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.

        try
        {
            // generate the key from the shared secret and the salt
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

            // Create a RijndaelManaged object
            aesAlg = new RijndaelManaged();
            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

            // Create a decryptor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                // prepend the IV
                msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                }
                outStr = Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
        finally
        {
            // Clear the RijndaelManaged object.
            if (aesAlg != null)
                aesAlg.Clear();
        }

        // Return the encrypted bytes from the memory stream.
        return outStr;
    }
    /// <summary>
    /// Decrypt the given string.  Assumes the string was encrypted using 
    /// EncryptStringAES(), using an identical sharedSecret.
    /// </summary>
    /// <param name="cipherText">The text to decrypt.</param>
    /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
    public static string DecryptStringAES(string cipherText, string sharedSecret)
    {
        if (string.IsNullOrEmpty(cipherText))
            throw new ArgumentNullException("cipherText");
        if (string.IsNullOrEmpty(sharedSecret))
            throw new ArgumentNullException("sharedSecret");

        // Declare the RijndaelManaged object
        // used to decrypt the data.
        RijndaelManaged aesAlg = null;

        // Declare the string used to hold
        // the decrypted text.
        string plaintext = null;

        try
        {
            // generate the key from the shared secret and the salt
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

            // Create the streams used for decryption.                
            byte[] bytes = Convert.FromBase64String(cipherText);
            using (MemoryStream msDecrypt = new MemoryStream(bytes))
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                // Get the initialization vector from the encrypted stream
                aesAlg.IV = ReadByteArray(msDecrypt);
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))

                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                }
            }
        }
        finally
        {
            // Clear the RijndaelManaged object.
            if (aesAlg != null)
                aesAlg.Clear();
        }

        return plaintext;
    }
    private static byte[] ReadByteArray(Stream s)
    {
        byte[] rawLength = new byte[sizeof(int)];
        if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
        {
            throw new SystemException("Stream did not contain properly formatted byte array");
        }

        byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
        if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
        {
            throw new SystemException("Did not read byte array properly");
        }

        return buffer;
    }
}