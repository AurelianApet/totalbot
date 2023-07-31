using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Fiddler;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Threading;

namespace TotalBot
{
    public partial class LoginForm : Form
    {

        Constant Constant = new Constant();

        private string m_fiddleHost = "api.pandalive.co.kr";
        private int m_fiddlePort = 443;
        private ChromeDriver chrome;
        private ChromeOptions options;

        private string token = "";
        private string commonUserAgent = "";
        private string commonFirstSesskey = "";
        private string commonSecondSesskey = "";
        private string commonThirdSesskey = "";
        private string commonForthSesskey = "";

        private string m_logFilePath = ".\\log\\";
        private string m_logFileName = "";
        private FileStream m_fileStream = null;
        private object m_objFileLocker = new object();

        public LoginForm()
        {
            InitializeComponent();

            options = new ChromeOptions();
            options.AddUserProfilePreference("profile.default_content_setting_values.plugins", "1");
            options.AddUserProfilePreference("profile.content_settings.plugin_whitelist.adobe-flash-player", "1");
            options.AddUserProfilePreference("profile.content_settings.exceptions.plugins.*,*.per_resource.adobe-flash-player", "1");
            options.AddUserProfilePreference("PluginsAllowedForUrls", "BEST URL EVER");

            startFiddler();

            this.FormClosing += new FormClosingEventHandler(this.closing);
        }


        private void closing(object sender, FormClosingEventArgs e)
        {
            stopFiddler();
        }

        private void startFiddler()
        {
            if (FiddlerApplication.IsStarted())
            {
                Console.WriteLine("<--Fiddler is already started-->");
                return;
            }
            try
            {
                InstallCertificate();
                FiddlerApplication.BeforeRequest += onAfterSessionComplete;
                FiddlerApplication.Startup(8888, true, true, true);
            }
            catch (Exception e)
            {
                LogToFile("Exception::" + e.Message);
            }
        }

        private void stopFiddler()
        {
            if (!FiddlerApplication.IsStarted())
                return;
            FiddlerApplication.BeforeRequest -= onAfterSessionComplete;
            if (FiddlerApplication.IsStarted())
                FiddlerApplication.Shutdown();
            UninstallCertificate();
        }

        private void showTotalBot()
        {
            stopFiddler();
            chrome.Dispose();

            Thread enterRoomThread = new Thread(new ThreadStart(() => enterTotalBot()));
            enterRoomThread.Start();

            this.Close();
        }

        private void enterTotalBot()
        {
            Totalbot totalBot = new Totalbot(commonFirstSesskey, commonSecondSesskey, commonThirdSesskey, commonForthSesskey, commonUserAgent);
            totalBot.token = token;
            Application.Run(totalBot);
        }

        private void onAfterSessionComplete(Session sess)
        {
            try
            {
                if (sess.RequestMethod != "POST")
                    return;
                if (sess == null || sess.oRequest == null || sess.oRequest.headers == null)
                    return;

                if (sess.hostname.ToLower() != m_fiddleHost || sess.port != m_fiddlePort)
                    return;

                LogToFile(sess.oRequest.headers.ToString() + "\r\n" + sess.GetRequestBodyAsString());
                // saveBaqryxx
                if (!sess.url.Contains("/live/cache"))
                    return;

                string headers = sess.oRequest.headers.ToString();
                var reqBody = sess.GetRequestBodyAsString();

                string firstLine = sess.RequestMethod + " " + sess.fullUrl + " " + sess.oRequest.headers.HTTPVersion;
                int at = headers.IndexOf("\r\n");
                if (at < 0)
                    return;
                headers = firstLine + "\r\n" + headers.Substring(at + 1);
                string output = reqBody;

                LogToFile(firstLine + "\r\n" + reqBody);

                char[] spliter = { '\r', '\n' };
                string[] headerPart = sess.oRequest.headers.ToString().Split(spliter);
                for (int i = 0; i < headerPart.Length; i++)
                {
                    if (headerPart[i].Contains("User-Agent"))
                    {
                        at = headerPart[i].IndexOf(":");
                        commonUserAgent = headerPart[i].Substring(at + 1).Trim();
                    }
                }


                string[] arr_param = output.Split('&');
                for (int i = 0; i < arr_param.Length; i++)
                {
                    string[] par_split = arr_param[i].Split('=');
                    if (par_split.Length > 1)
                    {
                        string par0 = System.Web.HttpUtility.UrlDecode(par_split[0]);
                        string par1 = System.Web.HttpUtility.UrlDecode(par_split[1]);

                        if (par0 == "_token")
                        {
                            token = par1;
                        }

                        if (par0 == "_i3nY6v7ot9WIAugPuZ6Pj3WlOU")
                        {
                            commonFirstSesskey = par1;
                        }

                        if (par0 == "AYohuyc98ugPuZ6Pj3WlOU")
                        {
                            commonSecondSesskey = par1;
                        }

                        if (par0 == "o3GVexFtLZADj4Idn1w0S34Bzo")
                        {
                            commonThirdSesskey = par1;
                        }

                        if (par0 == "H5XAgw1OdK1zAu2GFv1uthl")
                        {
                            commonForthSesskey = par1;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception::" + e.Message);
                LogToFile("Exception::" + e.Message);
            }

        }



        public void LogToFile(string strMsg)
        {
            try
            {
                DateTime now = DateTime.Now;
                if (m_logFileName.Length == 0 || m_fileStream == null)
                {
                    m_logFileName = string.Format("{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}.log",
                        now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
                }
                lock (m_objFileLocker)
                {
                    string strFilePath = m_logFilePath;
                    Directory.CreateDirectory(strFilePath);
                    strFilePath += m_logFileName;
                    m_fileStream = File.Open(strFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                    if (m_fileStream.Length > 204800)
                    {
                        m_logFileName = "";
                    }
                    StreamWriter writer = new StreamWriter(m_fileStream, Encoding.UTF8);

                    string strNow = string.Format("{0:D4}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}",
                            now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
                    writer.WriteLine(strNow + " : " + strMsg);
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("LogException::" + e.Message);
            }
            finally
            {
                if (m_fileStream != null)
                {
                    m_fileStream.Close();
                }
            }
        }

        private void SerialkeyBtn_Click(object sender, EventArgs e)
        {
            try
            {
                HttpClientHandler cookiehandler = new HttpClientHandler();
                HttpClient cookiehttpClient = new HttpClient(cookiehandler);
                List<KeyValuePair<string, string>> cookieinputs = new List<KeyValuePair<string, string>>();
                //cookieinputs.Add(new KeyValuePair<string, string>("id", "5"));
                cookieinputs.Add(new KeyValuePair<string, string>("password", this.SerialkeyText.Text));
                HttpResponseMessage cookieresponseMessageMain = cookiehttpClient.PostAsync(Constant.LOGIN_URL, (HttpContent)new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)cookieinputs)).Result;
                cookieresponseMessageMain.EnsureSuccessStatusCode();
                string cookieresponseMessageLoginString = cookieresponseMessageMain.Content.ReadAsStringAsync().Result;
                if (cookieresponseMessageLoginString.Contains("true"))
                {
                    this.MpandaLoginGroup.Enabled = true;
                    MessageBox.Show("비밀번호가 확인되엿습니다.");
                }
                else
                {
                    MessageBox.Show("비밀번호가 올바르지 않습니다.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("비밀번호 확인이 실패하였습니다.");
            }
        }

        private void pandaLoginBtn_Click(object sender, EventArgs e)
        {
            doProdLogin();
        }

        private void doProdLogin()
        {
            chrome = new ChromeDriver(options);
            chrome.Url = "https://www.pandalive.co.kr/member/login";

            var idInput = chrome.FindElementByXPath("//*[@id=\"formLogin\"]/div/div/fieldset/input[1]");
            var pwInput = chrome.FindElementByXPath("//*[@id=\"formLogin\"]/div/div/fieldset/input[2]");
            var loginButton = chrome.FindElementByXPath("//*[@id=\"formLogin\"]/div/div/fieldset/div[1]");

            idInput.SendKeys(pandaIdText.Text);
            pwInput.SendKeys(pandaPassText.Text);

            loginButton.Click();

            IJavaScriptExecutor js = (IJavaScriptExecutor)chrome;

            var idTemp = js.ExecuteScript("return userInfo.userId");
            var sessKeyTemp = js.ExecuteScript("return jQuery.cookie(\"sessKey\")");

            while (idTemp == null)
            {
                idTemp = js.ExecuteScript("return userInfo.userId");
            }

            while (sessKeyTemp == null)
            {
                sessKeyTemp = js.ExecuteScript("return jQuery.cookie(\"sessKey\")");
            }

            string id = idTemp.ToString();
            string sessKey = sessKeyTemp.ToString();

            Thread.Sleep(1000);
            if (!chrome.Url.Contains("/member/login"))
            {
                //var aLink = chrome.FindElementByXPath("//*[@id=\"aside\"]/div/div/ul/li[4]/a");
                //aLink.Click();
                //Thread.Sleep(1000);

                showTotalBot();
            }
            else
            {
                MessageBox.Show("로그인에 실패하였습니다.");
            }
        }

        private void doDevLogin()
        {
            chrome = new ChromeDriver(options);
            chrome.Url = "http://laravel-theme.loc/login";


            var idInput = chrome.FindElementByXPath("//input[@name='email']");
            var pwInput = chrome.FindElementByXPath("//input[@name='password']");
            var loginButton = chrome.FindElementByXPath("//input[@type='submit']");

            idInput.SendKeys(pandaIdText.Text);
            pwInput.SendKeys(pandaPassText.Text);

            loginButton.Click();

            Thread.Sleep(1000);
            if (chrome.Url.Contains("my-account"))
            {
                showTotalBot();
            }
            else
            {
                MessageBox.Show("로그인에 실패하였습니다.");
            }

        }

        public bool InstallCertificate()
        {
            if (!CertMaker.rootCertExists())
            {
                if (!CertMaker.createRootCert())
                    return false;
                if (!CertMaker.trustRootCert())
                    return false;
            }
            return true;
        }

        public bool UninstallCertificate()
        {
            if (CertMaker.rootCertExists())
            {
                if (!CertMaker.removeFiddlerGeneratedCerts(true))
                    return false;
            }
            return true;
        }
    }
}
