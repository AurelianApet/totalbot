using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;


namespace TotalBot
{
    delegate void myDelegate();
    public partial class Totalbot : Form
    {
        Common commonClass = new Common();
        //팬더 TV 공통 글로벌 변수
        string commonSesskey = "";
        string commonLoginUserId = "";
        public string commonFirstSesskey = "";
        public string commonSecondSesskey = "";
        public string commonThirdSesskey = "";
        public string commonForthSesskey = "";
        public string commonUserAgent = "";
        Constant Constant = new Constant();

        public string token = "";

        public Totalbot(string cfk, string csk, string ctk, string crk, string cua)
        {
            InitializeComponent();
            //disablePanel();

            commonFirstSesskey = cfk;
            commonSecondSesskey = csk;
            commonThirdSesskey = ctk;
            commonForthSesskey = crk;
            commonUserAgent = cua;

            commonClass.commonFirstSesskey = commonFirstSesskey;
            commonClass.commonSecondSesskey = commonSecondSesskey;
            commonClass.commonThirdSesskey = commonThirdSesskey;
            commonClass.commonForthSesskey = commonForthSesskey;
            commonClass.commonUserAgent = commonUserAgent;
        }


        //비밀번호 확인전
        private void disablePanel()
        {
            MpandaLoginGroup.Enabled = false;
            mainTabControl.Enabled = false;
        }

        //팬더 TV 공통 로그인 부분 완료
        private void pandaLoginBtn_Click(object sender, EventArgs e)
        {
            string pandaId = this.pandaIdText.Text;
            string pandaPass = this.pandaPassText.Text;
            string pandaProxyIp = this.pandaProxyIpText.Text;
            string pandaProxyPort = this.pandaProxyPortText.Text;
            if (pandaId == "" || pandaId == null || pandaPass == "" || pandaPass == null)
            {
                MessageBox.Show("팬더TV 아이디 혹은 비번을 바로 입력하세요.");
            }
            else
            {
                List<string> sesskeyAndId = new List<string>();
                sesskeyAndId = commonClass.pandaLogin(pandaId, pandaPass, pandaProxyIp, pandaProxyPort);
                if (sesskeyAndId.Count != 2)
                    return;
                commonSesskey = sesskeyAndId[0];
                commonLoginUserId = sesskeyAndId[1];
            }
        }
        //채팅 프로그램
        //채팅 프로그램 로그인 완료
        private void CbjAddbtn_Click(object sender, EventArgs e)
        {
            //입력부분 정확성 검사
            var bjId = this.CbjIdText.Text;
            var bjRoomId = this.CbjRoomidText.Text;
            var managerId = this.CmanagerIdText.Text;
            var managerPass = this.CmanagerPassText.Text;
            var proxyIp = this.CbjProxyIpText.Text;
            var proxyPort = this.CbjProxyPortText.Text;

            if (bjId == "" || bjRoomId == "" || managerId == "" || managerPass == "")
            {
                MessageBox.Show("비제이와 매니저 정보를 올바로 입력하세요.");
                return;
            }

            //로그인하여 sessKey 얻는 부분
            List<string> sesskeyAndId = new List<string>();
            sesskeyAndId = commonClass.pandaLogin(managerId, managerPass, proxyIp, proxyPort);
            if (sesskeyAndId.Count != 2)
                return;

            String sesskey = sesskeyAndId[0];
            String loginUserId = sesskeyAndId[1];
            //GridView에 현시 부분
            int bjNumber = this.BjGridView.Rows.Count;
            this.BjGridView.Rows.Add(bjNumber, bjId, bjRoomId, managerId, sesskey);
            this.CbjIdText.Text = "";
            this.CbjRoomidText.Text = "";
            this.CmanagerIdText.Text = "";
            this.CmanagerPassText.Text = "";
        }
        //패널 옮길때 호출되는 함수 완료
        private void CchattingSetting_Click(object sender, EventArgs e)
        {
            string no = "";
            string bjId = "";
            string roomId = "";
            string managerId = "";
            string sesskey = "";
            //채팅 패널 초기화
            this.CchattingListPanel.Controls.Clear();

            if (this.BjGridView.Rows.Count <= 1)
            {
                //MessageBox.Show("등록된 비제이 유저가 없습니다.");
                return;
            }
            for (int i = 0; i < this.BjGridView.Rows.Count - 1; i++)
            {
                try
                {
                    no = this.BjGridView.Rows[i].Cells[0].Value.ToString();
                    bjId = this.BjGridView.Rows[i].Cells[1].Value.ToString();
                    roomId = this.BjGridView.Rows[i].Cells[2].Value.ToString();
                    managerId = this.BjGridView.Rows[i].Cells[3].Value.ToString();
                    sesskey = this.BjGridView.Rows[i].Cells[4].Value.ToString();
                    CchattingListShow(no, bjId, roomId, managerId, sesskey);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("채팅패널에 입력중 오류가 발생하였습니다.");
                }
            }
        }
        //채팅 리스트에 노출 완료
        private void CchattingListShow(string no, string bjId, string roomId, string managerId, string sesskey)
        {
            for (int i = 0; i < 4; i++)
            {
                Label label = new Label();
                label.AutoSize = true;
                switch (i)
                {
                    case 0:
                        label.Text = no;
                        label.Location = new System.Drawing.Point(10, 30 * Convert.ToInt32(no));
                        break;
                    case 1:
                        label.Text = bjId;
                        label.Location = new System.Drawing.Point(50, 30 * Convert.ToInt32(no));
                        break;
                    case 2:
                        label.Text = roomId;
                        label.Location = new System.Drawing.Point(220, 30 * Convert.ToInt32(no));
                        break;
                    case 3:
                        label.Text = managerId;
                        label.Location = new System.Drawing.Point(480, 30 * Convert.ToInt32(no));
                        break;
                }
                label.Font = new System.Drawing.Font("BatangChe", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.CchattingListPanel.Controls.Add(label);
            }

            Button btn = new Button();
            btn.Text = "상세보기";
            btn.Name = sesskey + "," + bjId + "," + managerId + "," + roomId;
            btn.Location = new System.Drawing.Point(650, 30 * Convert.ToInt32(no));
            btn.Font = new System.Drawing.Font("BatangChe", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btn.Click += new EventHandler(CchattingDetailView_Click);
            btn.Size = new System.Drawing.Size(100, 22);
            this.CchattingListPanel.Controls.Add(btn);
        }
        //기본채팅 패널 노출 완료
        public void CchattingDetailView_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string[] sesskeyAndId = btn.Name.Split(',');
            Thread chattingThread = new Thread(new ThreadStart(() => CchattingThread(sesskeyAndId[0], sesskeyAndId[1], sesskeyAndId[2], sesskeyAndId[3])));
            chattingThread.Start();
            Thread.Sleep(500);
        }
        //채팅 실현
        public void CchattingThread(string sesskey, string bjId, string managerId, string roomId)
        {
            ChattingBox chattingBox = new ChattingBox();
            chattingBox.Text = bjId + "비제이님 채팅관리";
            chattingBox.sesskey = sesskey;
            chattingBox.managerId = managerId;
            chattingBox.roomId = roomId;
            chattingBox.bjId = bjId;
            chattingBox.userAgent = commonUserAgent;

            chattingBox.showHistory();
            chattingBox.ShowDialog();
        }




        //VIP 추출 및 체크 프로그램
        //글로벌 변수
        List<string> bjRankingList = new List<string>();
        List<string> bjMonthRankingList = new List<string>();
        List<string> bjList = new List<string>();
        List<string> bjFanList = new List<string>();
        //BJ 랭킹 추출
        private void VbjRankingBtn_Click(object sender, EventArgs e)
        {
            this.bjRankingList.Clear();
            getRanking("rankingPopular", "bjPopularRanking.txt");
        }
        //월별 랭킹 추출
        private void VmonthRankingBtn_Click(object sender, EventArgs e)
        {
            this.bjMonthRankingList.Clear();
            getRanking("rankingPopularMonth", "bjMonthRanking.txt");
        }
        //랭킹 추출 함수
        private void getRanking(string type, string path)
        {
            if (commonSesskey == "" || commonLoginUserId == "" || commonFirstSesskey == "" || commonSecondSesskey == "" || commonThirdSesskey == "" || commonForthSesskey == "")
            {
                MessageBox.Show("팬더TV에 로그인 하세요.");
                return;
            }

            int count = 200;
            var baseUrl = new Uri("https://api.pandalive.co.kr/live/cache");
            CookieContainer container = new CookieContainer();
            container.Add(baseUrl, new Cookie("sessKey", commonSesskey));
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.DefaultConnectionLimit = 2;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            HttpClientHandler handler = new HttpClientHandler();

            handler.CookieContainer = container;
            HttpClient httpClient = new HttpClient(handler);

            //팬더TV에 요청보내서 쿠키 얻어오는 부분


            httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate,br");
            httpClient.DefaultRequestHeaders.Add("User-Agent", commonUserAgent);
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            httpClient.DefaultRequestHeaders.Host = "api.pandalive.co.kr";

            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = DateTime.Now.ToUniversalTime() - origin;
            string Date = Convert.ToString(Math.Floor(diff.TotalSeconds));

            List<KeyValuePair<string, string>> inputs = new List<KeyValuePair<string, string>>();
            inputs.Add(new KeyValuePair<string, string>("NgxdaEY0ZX2DdK1XoaA", type));
            inputs.Add(new KeyValuePair<string, string>("Y6K2VQRw0MfwbPfZ4Dna", "112"));
            inputs.Add(new KeyValuePair<string, string>("AYNUR1c5v-yi4dNLpZWlOU", "74"));
            inputs.Add(new KeyValuePair<string, string>("PL2SAuzDQgv_a4_EcI4Nln1wgRx4Bzo", Convert.ToString(count)));
            inputs.Add(new KeyValuePair<string, string>("H5XAgw1OdK1zAu2GFv1uthl", commonForthSesskey));
            inputs.Add(new KeyValuePair<string, string>("AYPgdwhUv0yi4dNLpZWlOU", "N"));
            inputs.Add(new KeyValuePair<string, string>("_i3nY6v7ot9WIAugPuZ6Pj3WlOU", commonFirstSesskey));
            inputs.Add(new KeyValuePair<string, string>("Yt40SkS4eFP1sNLe4Dna", Date));
            inputs.Add(new KeyValuePair<string, string>("AYohuyc98ugPuZ6Pj3WlOU", commonSecondSesskey));
            inputs.Add(new KeyValuePair<string, string>("yQv1sB81otLfILAZU_XYXoaA", commonSesskey));  // cookie
            inputs.Add(new KeyValuePair<string, string>("o3GVexFtLZADj4Idn1w0S34Bzo", commonThirdSesskey));
            inputs.Add(new KeyValuePair<string, string>("YtK1kkS4aBf-Yxfc4Dna", "webPc"));
            inputs.Add(new KeyValuePair<string, string>("y6RycBO1oZLfADX_9tKZXoaA", "1.0.0"));

            try
            {
                HttpResponseMessage responseMessageLogin = httpClient.PostAsync("https://api.pandalive.co.kr/live/cache", (HttpContent)new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)inputs)).Result;
                responseMessageLogin.EnsureSuccessStatusCode();
                string responseMessageLoginString = responseMessageLogin.Content.ReadAsStringAsync().Result;
                JObject responseObject = JObject.Parse(responseMessageLoginString);
                if (type == "rankingPopular")
                {
                    for (int i = 0; i < responseObject["list"].ToArray().Length; i++)
                    {
                        responseObject["list"][i]["rType"] = "popular";
                        bjRankingList.Add(responseObject["list"][i].ToString());
                    }
                }
                else
                {
                    for (int i = 0; i < responseObject["list"].ToArray().Length; i++)
                    {
                        responseObject["list"][i]["rType"] = "month";
                        bjMonthRankingList.Add(responseObject["list"][i].ToString());
                    }
                }
                //파일로 보관
                VbjRankingSaveFile(path, responseObject["list"].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("팬더TV 랭킹 추출시 오류가 발생하었습니다.");
            }

        }
        //인기랭킹과 월별랭킹 합치기
        private void VmergeBtn_Click(object sender, EventArgs e)
        {
            if (bjRankingList.Count == 0)
            {
                MessageBox.Show("인기BJ랭킹을 추출하세요.");
            }
            else if (bjMonthRankingList.Count == 0)
            {
                MessageBox.Show("월별BJ랭킹을 추출하세요.");
            }
            else
            {
                for (int i = 0; i < bjRankingList.Count; i++)
                {
                    bjList.Add(bjRankingList[i]);
                }
                for (int i = 0; i < bjMonthRankingList.Count; i++)
                {
                    bjList.Add(bjMonthRankingList[i]);
                }
                string text = "[";
                for (int i = 0; i < bjList.Count; i++)
                {
                    if (i == bjList.Count - 1)
                    {
                        text += bjList[i];
                    }
                    else
                    {
                        text += bjList[i] + ",";
                    }
                }
                text += "]";
                VbjRankingSaveFile("bjAllRanking.txt", text);
            }
        }
        //VIP 추출
        private void VgetvipBtn_Click(object sender, EventArgs e)
        {
            if (bjList.Count == 0)
            {
                MessageBox.Show("추출된 비제이 목록이 없습니다.");
                return;
            }
            this.progressBar1.Maximum = bjList.Count / 5;
            this.progressBar1.Value = 0;
            Thread[] threadArray = new Thread[bjList.Count];
            int count = 0;
            for (int i = 0; i < bjList.Count; i += 5)
            {
                count = 0;
                for (int j = 0; j < 5; j++)
                {
                    if (i + j < bjList.Count)
                    {
                        count++;
                        threadArray[i + j] = new Thread(new ThreadStart(() => getVipList(JObject.Parse(bjList[i])["userIdx"].ToString(), JObject.Parse(bjList[i])["userId"].ToString(), JObject.Parse(bjList[i])["rankingNumber"].ToString(), JObject.Parse(bjList[i])["rType"].ToString())));
                        threadArray[i + j].Start();
                        Thread.Sleep(100);
                    }
                }
                for (int j = 0; j < count; j++)
                {
                    threadArray[i + j].Join();
                }
                this.progressBar1.Value++;
                Thread.Sleep(100);
            }
            //for (int i = 0; i < bjList.Count; i++)
            //{
            //    getVipList(JObject.Parse(bjList[i])["userIdx"].ToString(), JObject.Parse(bjList[i])["userId"].ToString(), JObject.Parse(bjList[i])["rankingNumber"].ToString(), JObject.Parse(bjList[i])["rType"].ToString());
            //}
            string text = "{";
            this.progressBar1.Maximum = bjFanList.Count / 100;
            this.progressBar1.Value = 0;
            for (int i = 0; i < bjFanList.Count; i++)
            {
                if (i == bjFanList.Count - 1)
                {
                    text += Convert.ToString(i) + ":" + bjFanList[i];
                }
                else
                {
                    text += Convert.ToString(i) + ":" + bjFanList[i] + ",";
                }
                if (i % 100 == 0)
                {
                    this.progressBar1.Value = i / 100;
                }
            }
            text += "}";
            VbjRankingSaveFile("bjFan.txt", text);
        }

        private void updateProgressBar()
        {
            this.progressBar1.Value++;
        }
        private void getVipList(string userIdx, string userId, string rankingNumber, string rType)
        {
            var baseUrl = new Uri("https://www.pandalive.co.kr/channel/" + userIdx + "/fanRanking");
            CookieContainer container = new CookieContainer();
            container.Add(baseUrl, new Cookie("sessKey", commonSesskey));
            container.Add(baseUrl, new Cookie("userLoginIdx", commonLoginUserId));
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = container;
            HttpClient httpClient = new HttpClient(handler);

            List<KeyValuePair<string, string>> inputs = new List<KeyValuePair<string, string>>();
            inputs.Add(new KeyValuePair<string, string>("wV3cO1A1GI5YNQuycO9ultuZ65IZWlOU", Convert.ToString(0)));
            inputs.Add(new KeyValuePair<string, string>("o3K59-yiFYH5Y2PtKZzAdvfDx-qR3dADj4Nln1wgRx4Bzo", "fromUserId"));
            inputs.Add(new KeyValuePair<string, string>("NgxmPfZ0ZX2DdK1XoaA", "all"));
            inputs.Add(new KeyValuePair<string, string>("o3K59-yiFYH5Y2PtKZzAdvfDx-qR3dADj4Nln1wgRx4Bzo", "fromUserId"));
            inputs.Add(new KeyValuePair<string, string>("o3DFP2yiFYH5Y2PtKZzAdvfDx-qR3dADj4Nln1wgRx4Bzo", ""));

            List<string> Result = new List<string>();
            try
            {
                HttpResponseMessage responseMessageMain = httpClient.PostAsync(baseUrl, (HttpContent)new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)inputs)).Result;
                string content = responseMessageMain.Content.ReadAsStringAsync().Result.ToString();

                var startNumber = content.IndexOf("<tbody>");
                var endNumber = content.IndexOf("</tbody>");
                string formcontent = content.Substring(startNumber, endNumber - startNumber);
                string[] formArray = formcontent.Split('>');

                for (int j = 0; j < formArray.Length; j++)
                {
                    if (formArray[j].Contains("</td"))
                    {
                        Result.Add(formArray[j].Substring(0, formArray[j].Length - 4));
                    }
                }
                for (int k = 0; k < Result.Count; k++)
                {
                    if (k % 4 == 0)
                    {
                        if (Result[k] != "" && Result[k] != null)
                        {
                            JObject fanInfo = new JObject();
                            fanInfo.Add("id", Result[k]);
                            fanInfo.Add("name", Result[k + 1]);
                            fanInfo.Add("nickname", Result[k + 2]);
                            fanInfo.Add("heart", Result[k + 3]);
                            fanInfo.Add("bjId", userId);
                            fanInfo.Add("rankingNumber", rankingNumber);
                            fanInfo.Add("rType", rType);
                            bjFanList.Add(fanInfo.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("팬정보 전체랭킹 로드시 오류가 발생하엇습니다.");
            }
            List<string> Result_1 = new List<string>();
            for (int j = 0; j < 3; j++)
            {
                try
                {
                    DateTime now = DateTime.Now;
                    string month = now.Year + "-" + (Convert.ToInt32(now.Month) - j).ToString();
                    List<KeyValuePair<string, string>> inputs_1 = new List<KeyValuePair<string, string>>();
                    inputs_1.Add(new KeyValuePair<string, string>("wV3cO1A1GI5YNQuycO9ultuZ65IZWlOU", Convert.ToString(0)));
                    inputs_1.Add(new KeyValuePair<string, string>("NgxmPfZ0ZX2DdK1XoaA", "month"));
                    inputs_1.Add(new KeyValuePair<string, string>("_x1BdKVNsv1hkf-yi4dNLpZWlOU", month));
                    HttpResponseMessage responseMessageMain = httpClient.PostAsync(baseUrl, (HttpContent)new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)inputs_1)).Result;
                    string content = responseMessageMain.Content.ReadAsStringAsync().Result.ToString();
                    var startNumber = content.IndexOf("<tbody>");
                    var endNumber = content.IndexOf("</tbody>");
                    string formcontent = content.Substring(startNumber, endNumber - startNumber);
                    string[] formArray = formcontent.Split('>');

                    for (int k = 0; k < formArray.Length; k++)
                    {
                        if (formArray[k].Contains("</td"))
                        {
                            Result_1.Add(formArray[k].Substring(0, formArray[k].Length - 4));
                        }
                    }
                    for (int k = 0; k < Result_1.Count; k++)
                    {
                        if (k % 4 == 0)
                        {
                            if (Result_1[k] != "" && Result_1[k] != null)
                            {
                                JObject fanInfo = new JObject();
                                fanInfo.Add("id", Result_1[k]);
                                fanInfo.Add("name", Result_1[k + 1]);
                                fanInfo.Add("nickname", Result_1[k + 2]);
                                fanInfo.Add("heart", Result_1[k + 3]);
                                fanInfo.Add("bjId", userId);
                                fanInfo.Add("rankingNumber", rankingNumber);
                                fanInfo.Add("rType", rType);
                                bjFanList.Add(fanInfo.ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("팬정보 월랭킹 로드시 오류가 발생하엇습니다.");
                }
            }
        }
        //BJ 랭킹 파일로 보관
        private void VbjRankingSaveFile(string path, string data)
        {
            commonClass.MakeTxtFile(path, data);
            MessageBox.Show("추출이 완료되었습니다.");
        }





        //메세지 프로그램
        //글로벌 변수
        List<string> MfanList = new List<string>();
        int MpanResultCount = 1;
        int MpanPanelPosition = 0;
        List<JObject> MbronzList = new List<JObject>();
        List<JObject> MsilverList = new List<JObject>();
        List<JObject> MdaiaList = new List<JObject>();
        List<JObject> MgoldList = new List<JObject>();
        List<JObject> MvipList = new List<JObject>();
        List<JObject> MtotalFanList = new List<JObject>();
        //메세지 프로그램에서 팬정보 로드
        private void MgetPanLoadBtn_Click(object sender, EventArgs e)
        {
            if (commonSesskey == "" || commonLoginUserId == "")
            {
                MessageBox.Show("먼저 팬더 TV에 로그인하세요.");
                return;
            }
            int siteNumber = 0;
            while (true)
            {
                if (MpanResultCount == 0)
                {
                    MessageBox.Show("팬정보 로드가 완료되엇습니다.");
                    MpanResultCount = 1;
                    return;
                }
                MgetPanInfo(siteNumber);
                System.Threading.Thread.Sleep(100);
                siteNumber += 10;
            }

        }
        //팬정보 로드
        private void MgetPanInfo(int siteNumber)
        {
            var baseUrl = new Uri("https://www.pandalive.co.kr/channel/" + commonLoginUserId + "/fanRanking");
            CookieContainer container = new CookieContainer();
            container.Add(baseUrl, new Cookie("sessKey", commonSesskey));
            container.Add(baseUrl, new Cookie("userLoginIdx", commonLoginUserId));
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = container;
            HttpClient httpClient = new HttpClient(handler);

            List<KeyValuePair<string, string>> inputs = new List<KeyValuePair<string, string>>();
            inputs.Add(new KeyValuePair<string, string>("wV3cO1A1GI5YNQuycO9ultuZ65IZWlOU", Convert.ToString(siteNumber)));
            inputs.Add(new KeyValuePair<string, string>("o3K59-yiFYH5Y2PtKZzAdvfDx-qR3dADj4Nln1wgRx4Bzo", "fromUserId"));
            inputs.Add(new KeyValuePair<string, string>("NgxmPfZ0ZX2DdK1XoaA", "all"));
            inputs.Add(new KeyValuePair<string, string>("o3K59-yiFYH5Y2PtKZzAdvfDx-qR3dADj4Nln1wgRx4Bzo", "fromUserId"));
            inputs.Add(new KeyValuePair<string, string>("o3DFP2yiFYH5Y2PtKZzAdvfDx-qR3dADj4Nln1wgRx4Bzo", ""));

            List<string> Result = new List<string>();
            try
            {
                HttpResponseMessage responseMessageMain = httpClient.PostAsync(baseUrl, (HttpContent)new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)inputs)).Result;
                string content = responseMessageMain.Content.ReadAsStringAsync().Result.ToString();

                var startNumber = content.IndexOf("<tbody>");
                var endNumber = content.IndexOf("</tbody>");
                string formcontent = content.Substring(startNumber, endNumber - startNumber);
                string[] formArray = formcontent.Split('>');

                for (var i = 0; i < formArray.Length; i++)
                {
                    if (formArray[i].Contains("</td"))
                    {
                        MfanList.Add(formArray[i].Substring(0, formArray[i].Length - 4));
                        Result.Add(formArray[i].Substring(0, formArray[i].Length - 4));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("팬정보 " + siteNumber + "페지 로드시 오류가 발생하엇습니다.");
            }

            if (Result.Count == 0)
                MpanResultCount = 0;

            for (var j = 0; j < Result.Count - 1; j++)
            {
                if (j % 7 == 0)
                {
                    try
                    {
                        if (Result[j + 5] != "" && Result[j + 5] != null)
                        {
                            MpanPanelPosition++;
                            CheckBox checkbox = new CheckBox();
                            checkbox.Name = Convert.ToString(MpanPanelPosition);
                            checkbox.Text = Result[j + 1] + "   " + Result[j + 2] + "   " + Result[j + 3] + "   " + Result[j + 4] + "   " + Result[j + 5] + "   " + Result[j + 6];
                            checkbox.AutoSize = true;
                            checkbox.Location = new Point(25, 25 * MpanPanelPosition);
                            this.MpanInfoPanel.Controls.Add(checkbox);
                        }
                    }
                    catch (Exception ex) { }
                }
            }
        }
        //쪽기 보내기 단추 클릭
        private void MsendMessageBtn_Click(object sender, EventArgs e)
        {
            if (MfanList.Count == 0)
            {
                MessageBox.Show("먼저 팬정보를 로드하세요.");
                return;
            }
            else if (this.MmessageText.Text == "" || this.MmessageText.Text == null)
            {
                MessageBox.Show("쪽지를 입력하세요.");
                return;
            }

            //목록에 보관
            for (int k = 0; k < MfanList.Count; k++)
            {
                if (k % 7 == 4)
                {
                    if (MfanList[k + 1] != "" && MfanList[k + 1] != null)
                    {
                        JObject fanInfo = new JObject();
                        fanInfo.Add("id", MfanList[k - 3]);
                        fanInfo.Add("name", MfanList[k - 2]);
                        fanInfo.Add("nickname", MfanList[k - 1]);
                        fanInfo.Add("heart", MfanList[k]);
                        MtotalFanList.Add(fanInfo);
                        switch (MfanList[k + 1])
                        {
                            case "브론즈":
                                MbronzList.Add(fanInfo);
                                break;
                            case "실버":
                                MsilverList.Add(fanInfo);
                                break;
                            case "다이아":
                                MdaiaList.Add(fanInfo);
                                break;
                            case "골드":
                                MgoldList.Add(fanInfo);
                                break;
                            case "VIP":
                                MvipList.Add(fanInfo);
                                break;
                        }
                    }
                }
            }

            //쪽지 보내기
            bool childCheck = false;
            int childnumber = 0;
            if (this.MallCheck.Checked)
            {
                for (int v = 0; v < MtotalFanList.Count; v++)
                {
                    sendMessage((string)MtotalFanList[v]["name"], (string)MtotalFanList[v]["nickname"]);
                }
                MessageBox.Show("전체 쪽지 보내기 완료!");
                return;
            }
            foreach (CheckBox child in this.MpanInfoPanel.Controls)
            {
                if (child.Checked)
                {
                    sendMessage((string)MtotalFanList[childnumber]["name"], (string)MtotalFanList[childnumber]["nickname"]);
                    childCheck = true;
                }
                childnumber++;
            }
            if (this.MbronzCheck.Checked)
            {
                if (MbronzList.Count == 0)
                {
                    MessageBox.Show("등록된 브론즈님이 없습니다.");
                }
                else
                {
                    for (int b = 0; b < MbronzList.Count; b++)
                    {
                        sendMessage((string)MbronzList[b]["name"], (string)MbronzList[b]["nickname"]);
                    }
                    MessageBox.Show("브론즈님들께 쪽지보내기 완료!");
                }
            }
            if (this.MsilverCheck.Checked)
            {
                if (MsilverList.Count == 0)
                {
                    MessageBox.Show("등록된 실버님 없습니다.");
                }
                else
                {
                    for (int s = 0; s < MsilverList.Count; s++)
                    {
                        sendMessage((string)MsilverList[s]["name"], (string)MsilverList[s]["nickname"]);
                    }
                    MessageBox.Show("실버님들께 쪽지보내기 완료!");
                }

            }
            if (this.MdiaCheck.Checked)
            {
                if (MdaiaList.Count == 0)
                {
                    MessageBox.Show("등록된 다이아님 없습니다.");
                }
                else
                {
                    for (int d = 0; d < MdaiaList.Count; d++)
                    {
                        sendMessage((string)MdaiaList[d]["name"], (string)MdaiaList[d]["nickname"]);
                    }
                    MessageBox.Show("다이아님들께 쪽지보내기 완료!");
                }
            }
            if (this.MgoldCheck.Checked)
            {
                if (MgoldList.Count == 0)
                {
                    MessageBox.Show("등록된 다이아님 없습니다.");
                }
                else
                {
                    for (int g = 0; g < MgoldList.Count; g++)
                    {
                        sendMessage((string)MgoldList[g]["name"], (string)MgoldList[g]["nickname"]);
                    }
                    MessageBox.Show("골드님들께 쪽지보내기 완료!");
                }
            }
            if (this.MvipCheck.Checked)
            {
                if (MvipList.Count == 0)
                {
                    MessageBox.Show("등록된 다이아님 없습니다.");
                }
                else
                {
                    for (int v = 0; v < MvipList.Count; v++)
                    {
                        sendMessage((string)MvipList[v]["name"], (string)MvipList[v]["nickname"]);
                    }
                    MessageBox.Show("VIP님들께 쪽지보내기 완료!");
                }
            }
            if (this.MpandIdText.Text != "" && this.MpanNicknameText.Text != "")
            {
                sendMessage(this.MpandIdText.Text, this.MpanNicknameText.Text);
                MessageBox.Show(this.MpanNicknameText.Text + "께 쪽지 보내기 완료!");
            }
            if (!childCheck && !this.MbronzCheck.Checked && !this.MvipCheck.Checked && !this.MsilverCheck.Checked && !this.MdiaCheck.Checked && !this.MgoldCheck.Checked && this.MpandIdText.Text == "" && this.MpanNicknameText.Text == "")
            {
                MessageBox.Show("팬을 선팩하세요!");
                return;
            }
        }
        //쪽지 보내기
        private void sendMessage(string fanid, string fannickname)
        {
            try
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                TimeSpan diff = DateTime.Now.ToUniversalTime() - origin;
                string Date = Convert.ToString(Math.Floor(diff.TotalSeconds));

                ServicePointManager.Expect100Continue = false;
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.DefaultConnectionLimit = 2;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };


                var baseUrl = new Uri("https://api.pandalive.co.kr/post/sendMsg");
                CookieContainer container = new CookieContainer();
                container.Add(baseUrl, new Cookie("sessKey", commonSesskey));
                container.Add(baseUrl, new Cookie("userLoginIdx", commonLoginUserId));
                container.Add(baseUrl, new Cookie("userLoginYN", "Y"));
                container.Add(baseUrl, new Cookie("partner", "googleads03"));
                HttpClientHandler handler = new HttpClientHandler();
                handler.CookieContainer = container;
                HttpClient httpClient = new HttpClient(handler);

                httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
                httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate,br");
                httpClient.DefaultRequestHeaders.Add("User-Agent", commonUserAgent);
                httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
                httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                httpClient.DefaultRequestHeaders.Host = "api.pandalive.co.kr";

                List<KeyValuePair<string, string>> inputs = new List<KeyValuePair<string, string>>();
                inputs.Add(new KeyValuePair<string, string>("paVZADYyWlOU", fanid));
                if (this.MbrotherText.Text != "" || this.MbrotherText.Text != null)
                {
                    inputs.Add(new KeyValuePair<string, string>("acCLIZCt40XoaA", fannickname + this.MbrotherText.Text + " " + this.MmessageText.Text));
                }
                else
                {
                    inputs.Add(new KeyValuePair<string, string>("acCLIZCt40XoaA", this.MbrotherText.Text + " " + this.MmessageText.Text));
                }
                inputs.Add(new KeyValuePair<string, string>("_i3nY6v7ot9WIAugPuZ6Pj3WlOU", commonFirstSesskey));
                inputs.Add(new KeyValuePair<string, string>("Yt40SkS4eFP1sNLe4Dna", Date));
                inputs.Add(new KeyValuePair<string, string>("AYohuyc98ugPuZ6Pj3WlOU", commonSecondSesskey));
                inputs.Add(new KeyValuePair<string, string>("yQv1sB81otLfILAZU_XYXoaA", commonSesskey));
                inputs.Add(new KeyValuePair<string, string>("o3GVexFtLZADj4Idn1w0S34Bzo", commonThirdSesskey));
                inputs.Add(new KeyValuePair<string, string>("YtK1kkS4aBf-Yxfc4Dna", "webPc"));
                inputs.Add(new KeyValuePair<string, string>("y6RycBO1oZLfADX_9tKZXoaA", "1.0.0"));

                HttpResponseMessage responseMessageMain = httpClient.PostAsync(baseUrl, (HttpContent)new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)inputs)).Result;
                string content = responseMessageMain.Content.ReadAsStringAsync().Result.ToString();
                Console.WriteLine(content);
                if (content.Contains("true"))
                {
                    //MessageBox.Show(fannickname + "께 쪽지보내기 성공 하었습니다.");
                }
                else
                {
                    MessageBox.Show(fannickname + ": BJ님께서 VIP 등급 아래인 경우 쪽지 수신을 제한하여 발송이 불가능합니다.");
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}