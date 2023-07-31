using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Http;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json.Linq;

namespace TotalBot
{
    public class Common
    {
        Constant Constant = new Constant();
        public string commonFirstSesskey = "";
        public string commonSecondSesskey = "";
        public string commonThirdSesskey = "";
        public string commonForthSesskey = "";
        public string commonUserAgent = "";

        public List<string> pandaLogin(string pandaId, string pandaPass, string proxyIp, string proxyPort)
        {
            //세션과 유저 아이디 목록 되돌림 값
            List<string> result = new List<string>();
            // 서버로부터 쿠키로딩하는 부분
            string cookieresponseMessageLoginString = "";
            HttpClientHandler cookiehandler = new HttpClientHandler();
            if(proxyIp != "" && proxyPort != "")
            {
                try
                {
                    cookiehandler.Proxy = new WebProxy(proxyIp, Convert.ToInt32(proxyPort));
                }
                catch(Exception ex)
                {
                    MessageBox.Show("프록시 정보를 올바로 입력하세요.");
                    return result;
                }
            }
            

            CookieContainer container = new CookieContainer();
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.DefaultConnectionLimit = 2;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            HttpClientHandler handler = new HttpClientHandler();

            handler.CookieContainer = container;
            HttpClient httpClient = new HttpClient(handler);

            //팬더TV에 요청보내서 쿠키 얻어오는 부분
            var beforeLoginSesskey = "";
            try
            {
                HttpResponseMessage responseMessageMain = httpClient.GetAsync("https://www.pandalive.co.kr/").Result;
                var loginSesskey = Convert.ToString(responseMessageMain.EnsureSuccessStatusCode().Headers);
                string[] words = loginSesskey.Split(' ');
                for (var i = 0; i < words.Length; i++)
                {
                    if (words[i].Contains("sessKey"))
                    {
                        var start = words[i].IndexOf("=") + 1;
                        var end = words[i].IndexOf(";");
                        var length = end - start;
                        beforeLoginSesskey = words[i].Substring(start, length);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("팬더TV 쿠키 로딩중 오류가 발생하엇습니다.");
                return result;
            }

            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate,br");
            //httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.150 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("User-Agent", commonUserAgent);
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            httpClient.DefaultRequestHeaders.Host = "api.pandalive.co.kr";

            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = DateTime.Now.ToUniversalTime() - origin;
            string Date = Convert.ToString(Math.Floor(diff.TotalSeconds));

            List<KeyValuePair<string, string>> inputs = new List<KeyValuePair<string, string>>();
            inputs.Add(new KeyValuePair<string, string>("29X__Q4yowex4Bzo", pandaId));
            inputs.Add(new KeyValuePair<string, string>("2wqZ_Q4yowex4Bzo", pandaPass));
            inputs.Add(new KeyValuePair<string, string>("AYUcv2W9f-yi4dNLpZWlOU", "off"));
            inputs.Add(new KeyValuePair<string, string>("o36w9w_N3d85AZ_Q4yowex4Bzo", ""));
            inputs.Add(new KeyValuePair<string, string>("_i3nY6v7ot9WIAugPuZ6Pj3WlOU", commonFirstSesskey));
            inputs.Add(new KeyValuePair<string, string>("Yt40SkS4eFP1sNLe4Dna", Date));
            inputs.Add(new KeyValuePair<string, string>("AYohuyc98ugPuZ6Pj3WlOU", commonSecondSesskey));
            inputs.Add(new KeyValuePair<string, string>("yQv1sB81otLfILAZU_XYXoaA", beforeLoginSesskey));  // cookie
            inputs.Add(new KeyValuePair<string, string>("o3GVexFtLZADj4Idn1w0S34Bzo", commonThirdSesskey));
            inputs.Add(new KeyValuePair<string, string>("YtK1kkS4aBf-Yxfc4Dna", "webPc"));
            inputs.Add(new KeyValuePair<string, string>("y6RycBO1oZLfADX_9tKZXoaA", "1.0.0"));

            try
            {
                HttpResponseMessage responseMessageLogin = httpClient.PostAsync("https://www.pandalive.co.kr/member/login", (HttpContent)new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)inputs)).Result;
                responseMessageLogin.EnsureSuccessStatusCode();
                string responseMessageLoginString = responseMessageLogin.Content.ReadAsStringAsync().Result;
                var sesskeyStart = responseMessageLoginString.IndexOf("sessKey");
                var loginstring = responseMessageLoginString.Substring(sesskeyStart);
                string[] loginwords = loginstring.Split('"');
                string SessKeys = loginwords[2];

                MessageBox.Show(pandaId + "님이 팬더TV 로그인에 성공하었습니다.");

                //if (responseMessageLoginString.Contains("true"))
                //{
                //    MessageBox.Show(pandaId + "님이 팬더TV 로그인에 성공하었습니다.");
                //}
                //else
                //{
                //    MessageBox.Show(pandaId + "님이 팬더TV 로그인에 실패하었습니다.");
                //    return result;
                //}
                //세션키 추가 
                result.Add(SessKeys);
                string responseMessageLoginHeader = responseMessageLogin.Headers.GetValues("Set-Cookie").FirstOrDefault();
                string process = responseMessageLoginHeader.Split(';')[0];
                string userIdLoginIdx = process.Substring(process.IndexOf("=") + 1, process.Length - process.IndexOf("=") - 1);
                //유저 아이디 추가
                result.Add(userIdLoginIdx);
                System.Threading.Thread.Sleep(500);
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("팬더TV 로그인 요청시 오류가 발생하었습니다.");
                return result;
            }
        }
        public List<string> ReadTxtFile(string path)
        {
            string line = "";
            List<string> textArray = new List<string>();
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        textArray.Add(line);
                    }
                }
            }
            return textArray;
        }
        public string ReadRealTxtFile(string path)
        {
            string text = "";
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    text = sr.ReadToEnd();
                }
            }
            return text;
        }
        public void MakeTxtFile(string path, string text)
        {
            try
            {
                if (File.Exists(path))
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(path + "파일이 켜져 있습니다. 종료하고 다시 시도해주세요.");
                    }
                }

                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.Write(text);
                }
                if (path == "log.log")
                {
                    FileInfo file = new FileInfo(path);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
