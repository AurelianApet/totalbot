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
using SocketIOClient.Newtonsoft.Json;
using SocketIOClient.WebSocketClient;
using Newtonsoft.Json.Linq;
using SocketIOClient;
using System.Text.Json;
using Newtonsoft.Json;
using SocketIOClient.ConnectInterval;

namespace TotalBot
{
    public partial class ChattingBox : Form
    {
        SocketIO socket;

        public string sesskey = "";
        public string roomId = "";
        public string managerId = "";
        public string bjId = "";
        public string userAgent = "";
        private int _bufferSize = 2147483600;
        List<string> bjFanList = new List<string>();
        public ChattingBox()
        {
            InitializeComponent();
        }
        //글로벌 변수
        Common comonClass = new Common();
        private async void Socket_OnConnected(object sender, EventArgs e)
        {
            Console.WriteLine("Socket_OnConnected");

            writeChattingText("소켓연결에 성공하였습니다.");
            var UserInfo = new JObject();
            await socket.EmitAsync("UserInfo", UserInfo);
            var init = new JObject();
            init.Add("ReturnParam", true);
            init.Add("deviceType", "webPc");
            init.Add("deviceVersion", "1.0.0");
            init.Add("sessKey", sesskey);
            init.Add("site", "pandatv");
            init.Add("userAgent", userAgent);
            await socket.EmitAsync("Init", init);

            Thread.Sleep(500);
        }
        //방 입장하기
        private async void CenterRoomBtn_Click(object sender, EventArgs e)
        {
            var uri = new Uri("https://chat1.neolive.kr");
            socket = new SocketIO(uri, new SocketIOOptions
            {
                EIO = 3
            });
            socket.JsonSerializer = new MyJsonSerializer(socket.Options.EIO);
            socket.GetConnectInterval = () => new MyConnectInterval();

            socket.OnConnected += Socket_OnConnected;

            await socket.ConnectAsync();

            socket.On("new_connection", (data) =>
            {

            });

            socket.On("Init", async (data) =>
            {
                var login = new JObject();
                login.Add("id", managerId);
                login.Add("roomid", null);
                login.Add("roompw", null);
                login.Add("sessKey", sesskey);

                await socket.EmitAsync("Login", login);
            });

            socket.On("UserInfo", (data) =>
            {

            });

            socket.On("Login", async (data) =>
            {
                var watch = new JObject();
                watch.Add("roomid", roomId);
                watch.Add("action", "watch");

                await socket.EmitAsync("WatchInfo", watch);
            });

            socket.On("WatchInfo", async (data) => {
                var enterRoom = new JObject();
                enterRoom.Add("roomid", roomId);
                enterRoom.Add("password", "");

                await socket.EmitAsync("EnterRoom", enterRoom);

                var memberlist = new JObject();
                memberlist.Add("roomid", roomId);
                memberlist.Add("onoff", true);
                memberlist.Add("get", false);

                await socket.EmitAsync("MemberList", memberlist);

                var missionInfo = new JObject();
                missionInfo.Add("roomid", roomId);

                await socket.EmitAsync("MissionInfo", missionInfo);

                var LiveRecom = new JObject();
                LiveRecom.Add("roomid", roomId);
                await socket.EmitAsync("LiveRecom", LiveRecom);
            });

            socket.On("EnterRoom", (data) =>
            {

            });
            socket.On("MemberList", (data) =>
            {

            });
            socket.On("MissionInfo", (data) =>
            {

            });

            socket.On("MemberJoin", (data) =>
            {
                if (data.ToString().Length > 0)
                {
                    try
                    {
                        JsonElement elem = data.GetValue();
                        JObject dataObject = JObject.Parse(elem.ToString());
                        string id = dataObject["userInfo"]["id"].ToString();
                        string nickname = dataObject["userInfo"]["nick"].ToString();
                        if (id == managerId)
                            writeChattingText("방입장에 성공하였습니다.");
                        Thread thread = new Thread(new ThreadStart(() => VfanAlarm(id, nickname)));
                        thread.Start();
                        //VfanAlarm(id, nickname);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            });

            string oldhumanCoin = "";
            string oldnickname = "";
            int combocount = 1;
            int dusancount = 1;
            var pKeyTime = DateTime.Now;
            var pComTime = DateTime.Now;
            var pKeyMessage = "";
            socket.On("ChatMessage", (data) =>
            {
                try
                {
                    JsonElement elem = data.GetValue();
                    JObject dataObject = JObject.Parse(elem.ToString());
                    string id = dataObject["userInfo"]["id"].ToString();
                    string message = dataObject["message"].ToString();
                    string nickname = dataObject["userInfo"]["nick"].ToString();
                    string type = dataObject["type"].ToString();
                    writeChattingText(id + ":\r\n" + message);
                    //키워드

                    if (((DateTime.Now - pKeyTime).Ticks > 300000000 || pKeyMessage != message) && id != managerId)
                    {
                        keywordResponse(message);
                        pKeyTime = DateTime.Now;
                        pKeyMessage = message;
                    }

                    //입장인사
                    greetFanResponse(message);
                    //선물인사 콤보인사 두산인사 보내기
                    if (type == "SponCoin")
                    {
                        if (nickname == oldnickname && (DateTime.Now - pComTime).Ticks < 300000000)
                        {
                            if (dataObject["humanCoin"].ToString() == oldhumanCoin)
                            {
                                combocount++;
                            }
                            else if (Convert.ToInt32(dataObject["humanCoin"]) == Convert.ToInt32(oldhumanCoin) + 1)
                            {
                                dusancount++;
                            }
                        }
                        else
                        {
                            if (oldhumanCoin != "" && oldnickname != "")
                            {
                                if (combocount > 1)
                                {
                                    comboFanResponse(oldnickname, combocount);
                                }
                                if (dusancount > 1)
                                {
                                    dusanFanResponse(oldnickname, dusancount);
                                }
                                if (combocount == 1 && dusancount == 1)
                                {
                                    presentFanResponse(oldnickname, oldhumanCoin);
                                }
                                oldhumanCoin = "";
                                oldnickname = "";
                            }
                        }
                        oldhumanCoin = dataObject["humanCoin"].ToString();
                        oldnickname = nickname;
                    }
                    else
                    {
                        if ((DateTime.Now - pComTime).Ticks > 300000000)
                        {
                            if (oldhumanCoin != "" && oldnickname != "")
                            {
                                if (combocount > 1)
                                {
                                    comboFanResponse(oldnickname, combocount);
                                }
                                if (dusancount > 1)
                                {
                                    dusanFanResponse(oldnickname, dusancount);
                                }
                                if (combocount == 1 && dusancount == 1)
                                {
                                    presentFanResponse(oldnickname, oldhumanCoin);
                                }
                                oldhumanCoin = "";
                                oldnickname = "";
                            }
                            pComTime = DateTime.Now;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            });
        }
        //2020-12-18 swh edited
        private string ReadFile(string filename)
        {
            //StringBuilder stringBuilder = new StringBuilder();
            //FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);

            //using (StreamReader streamReader = new StreamReader(fileStream))
            //{
            //    char[] fileContents = new char[_bufferSize];
            //    int charsRead = streamReader.Read(fileContents, 0, _bufferSize);

            //    // Can't do much with 0 bytes
            //    if (charsRead == 0)
            //        throw new Exception("File is 0 bytes");

            //    while (charsRead > 0)
            //    {
            //        stringBuilder.Append(fileContents);
            //        charsRead = streamReader.Read(fileContents, 0, _bufferSize);
            //    }
            //}
            //String str = stringBuilder.ToString();
            string text = "";
            foreach (var line in File.ReadLines(filename))
            {
                text += line;
            }
            return text;
        }
        //2020-12-18 swh edited

        //팬이 들어오면 알람뜨기
        private void VfanAlarm(string id, string nickname)
        {
            string fanData = "";
            if (File.Exists("bjFan.txt") && bjFanList.Count == 0)
            {
                try
                {
                    fanData = ReadFile("bjFan.txt");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eror!");
                }
                //if (fanData == "")
                //    writeLogText("추출된 비제이 팬 목록이 없습니다. VIP 추출및 체크 프로그램에서 팬목록을 추출하세요.");
                if (fanData != "")
                {
                    JObject fanJbject = new JObject();
                    try
                    {
                        fanJbject = JObject.Parse(fanData);
                        string fanId = "";
                        string fanNick = "";
                        string bjId = "";
                        string rankingNumber = "";
                        string rType = "";
                        for (int i = 0; i < fanJbject.Count; i++)
                        {
                            fanId = fanJbject[i.ToString()]["name"].ToString();
                            fanNick = fanJbject[i.ToString()]["nickname"].ToString();
                            bjId = fanJbject[i.ToString()]["bjId"].ToString();
                            rankingNumber = fanJbject[i.ToString()]["rankingNumber"].ToString();
                            rType = fanJbject[i.ToString()]["rType"].ToString();

                            JObject fanInfo = new JObject();
                            fanInfo.Add("id", fanId);
                            fanInfo.Add("nickname", fanNick);
                            fanInfo.Add("bjId", bjId);
                            fanInfo.Add("rankingNumber", rankingNumber);
                            fanInfo.Add("rType", rType);
                            bjFanList.Add(fanInfo.ToString());

                            if (fanId == id)
                            {
                                if (rType == "popular")
                                    writeLogText("팬아이디: " + fanId + " 닉네임: " + fanNick + " 비제이 아이디: " + bjId + " 인기랭킹: " + rankingNumber);
                                else writeLogText("팬아이디: " + fanId + " 닉네임: " + fanNick + " 비제이 아이디: " + bjId + " 월별랭킹: " + rankingNumber);
                            }

                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    //2020-12-18 8:47 swh edited
                    //string temp = "";
                    //if (fanData.Contains(id))
                    //{
                    //    int start = 0;

                    //    start = fanData.IndexOf(id, 0) + id.Length;
                    //    temp = fanData.Substring(start, 140);
                    //    int starttemp = temp.IndexOf("rankingNumber", 0) + 17;
                    //    writeLogText("비제이: " + id + " 전체랭킹: " + temp.Substring(starttemp, 1));
                    //}
                    //2020-12-18 8:47 swh edited
                }
            }
            else if (bjFanList.Count > 0)
            {
                for (int i = 0; i < bjFanList.Count; i++)
                {
                    JObject dataObject = JObject.Parse(bjFanList[i]);
                    string fanId = dataObject["id"].ToString();
                    string fanNick = dataObject["nickname"].ToString();
                    string bjId = dataObject["bjId"].ToString();
                    string rankingNumber = dataObject["rankingNumber"].ToString();
                    string rType = dataObject["rType"].ToString();
                    if (fanId == id)
                    {
                        if (rType == "popular")
                            writeLogText("팬아이디: " + fanId + " 닉네임: " + fanNick + " 비제이 아이디: " + bjId + " 인기랭킹: " + rankingNumber);
                        else writeLogText("팬아이디: " + fanId + " 닉네임: " + fanNick + " 비제이 아이디: " + bjId + " 월별랭킹: " + rankingNumber);
                    }
                }
            }
        }
        //실시간 메세지 보내기
        private void CchattingSendBtn_Click(object sender, EventArgs e)
        {
            if (sesskey == "" || roomId == "" || managerId == "")
            {
                MessageBox.Show("방입장이 실패하엇습니다.");
                return;
            }
            //socket.Connect(); //2020-12-18 10:55 swh edited
            Thread.Sleep(1000);
            sendMessage(roomId, this.CchattingText.Text);
            this.CchattingText.Text = "";
        }
        //일정한 시간 채팅 GridView 에 추가
        private void CconstantSaveBtn_Click(object sender, EventArgs e)
        {
            //추가부분
            int time = 0;
            int count = 0;
            try
            {
                time = Convert.ToInt32(this.CconstantTimeText.Text);
                count = Convert.ToInt32(this.CconstantCountText.Text);
            }
            catch (Exception ex)
            {
                this.CconstantTimeText.Text = "";
                this.CconstantCountText.Text = "";
                MessageBox.Show("시간 혹은 반복횟수를 바로 입력하세요.");
                return;
            }
            string chattingMecro = this.CconstantContentText.Text;
            this.CconstantGridView.Rows.Add(time, count, chattingMecro);
            this.CconstantGridView.Update();
            //초기화
            this.CconstantTimeText.Text = "";
            this.CconstantCountText.Text = "";
            this.CconstantContentText.Text = "";

            Thread thread = new Thread(new ThreadStart(() => CconstantChattingSend(time, count, chattingMecro)));
            thread.Start();
        }
        //일정한 시간 채팅 메세지 보내기
        private void CconstantChattingSend(int time, int count, string chattingMecro)
        {
            Thread.Sleep(1000);
            for (int i = 0; i < count; i++)
            {
                if (this.CconstantCheckbox.Checked)
                {
                    var length = this.CconstantGridView.Rows.Count;
                    var check = true;

                    check = Convert.ToBoolean(this.CconstantGridView.Rows[length - 2].Cells[0].Value);
                    sendMessage(roomId, chattingMecro);
                    Thread.Sleep(time);
                }
            }
        }
        //키워드 GridView 에 추가
        private void CkeywordSaveBtn_Click(object sender, EventArgs e)
        {
            string keyword = this.CkeywordText.Text;
            string chattingMecro = this.CkeywordContentText.Text;
            if (keyword == "" || chattingMecro == "")
            {
                MessageBox.Show("키워드 혹은 채팅메크로 입력란이 비었습니다.");
                return;
            }
            this.CkeywordGridView.Rows.Add(true, keyword, chattingMecro);
            this.CkeywordGridView.Update();
            this.CkeywordText.Text = "";
            this.CkeywordContentText.Text = "";
        }

        long timeStick = DateTime.Now.Ticks;
        string[] beforeKeyword;
        //키워드 메세지 보내기
        private void keywordResponse(string message)
        {
            long nowTimeStick = DateTime.Now.Ticks;
            if (!this.CkeywordCheckBox.Checked) return;
            var check = false;
            string[] keyword;
            string chattingMecro = "";
            for (int i = 0; i < this.CkeywordGridView.Rows.Count; i++)
            {
                try
                {
                    check = Convert.ToBoolean(this.CkeywordGridView.Rows[i].Cells[0].Value);
                    if (check)
                    {
                        keyword = this.CkeywordGridView.Rows[i].Cells[1].Value.ToString().Split(',');
                        chattingMecro = this.CkeywordGridView.Rows[i].Cells[2].Value.ToString();
                        for (int j = 0; j < keyword.Length; j++)
                        {
                            if (message.Contains(keyword[j]))
                            {
                                if (nowTimeStick - timeStick < 30000000)
                                {

                                    timeStick = nowTimeStick;
                                }
                                Thread.Sleep(1000);
                                sendMessage(roomId, chattingMecro);
                            }
                        }
                        beforeKeyword = keyword;
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
        //입장인사
        public void greetFanResponse(string message)
        {
            if (message.Contains("님께서 입장하셨습니다."))
            {
                if (CgreetCheckBox.Checked)
                {
                    int find = message.IndexOf("님께서");
                    string nickname = message.Substring(0, find);
                    Thread.Sleep(1000);
                    if (this.CgreetNickText.Text == "")
                    {
                        sendMessage(roomId, this.CgreetText.Text);
                    }
                    else
                    {
                        sendMessage(roomId, nickname + this.CgreetNickText.Text + " " + this.CgreetText.Text);
                    }
                }
            }
        }
        //선물인사
        private void presentFanResponse(string nickname, string humanCoin)
        {
            if (this.CpresentContentText.Text == "" || this.CpresentContentText.Text == null)
            {
                MessageBox.Show("선물인사 콘텐츠를 입력하세요.");
                return;
            }

            if (this.CpresentCheckBox.Checked)
            {
                string message = "";
                if (this.CpresentNickText.Text == "" || this.CpresentNickText == null)
                {
                    message = humanCoin + "개 " + this.CpresentContentText.Text;
                }
                else
                {
                    message = nickname + this.CpresentNickText.Text + " " + humanCoin + "개 " + this.CpresentContentText.Text;
                }
                Thread.Sleep(1000);
                sendMessage(roomId, message);
            }
        }
        //콤보인사
        private void comboFanResponse(string nickname, int combocount)
        {
            if (this.CcomboContentText.Text == "" || this.CcomboContentText.Text == null)
            {
                MessageBox.Show("콤보인사 콘텐츠를 입력하세요.");
                return;
            }

            if (this.CcomboCheckBox.Checked)
            {
                for (var i = 0; i < combocount; i++)
                {
                    string message = "";
                    if (this.CcomboNickText.Text == "" || this.CcomboNickText.Text == null)
                    {
                        message = this.CcomboContentText.Text;
                    }
                    else
                    {
                        message = nickname + this.CcomboNickText + " " + this.CcomboContentText.Text;
                    }
                    Thread.Sleep(1000);
                    sendMessage(roomId, message);
                }
            }
        }
        //두산인사
        private void dusanFanResponse(string nickname, int dusancount)
        {
            if (this.CdusanContentText.Text == "" || this.CdusanContentText.Text == null)
            {
                MessageBox.Show("두산인사 콘텐츠를 입력하세요.");
                return;
            }

            if (this.CdusanCheckBox.Checked)
            {
                string message = "";
                if (this.CdusanNickText.Text == "" || this.CdusanNickText.Text == null)
                {
                    message = this.CdusanContentText.Text;
                }
                else
                {
                    message = nickname + this.CdusanNickText.Text + " " + this.CdusanContentText.Text;
                }
                Thread.Sleep(1000);
                sendMessage(roomId, message);
            }
        }
        //메세지 보내기
        public async void sendMessage(string roomid, string chatMessage)
        {
            var ChatMessage = new JObject();
            ChatMessage.Add("roomid", roomid);
            ChatMessage.Add("message", chatMessage);
            await socket.EmitAsync("ChatMessage", ChatMessage);
            //writeChattingTexts"매니저: " + chatMessage);
        }
        //로그
        delegate void SetTextCallback(string text);
        private void writeChattingText(string message)
        {
            try
            {
                if (this.CchattingDetailText.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(writeChattingText);
                    this.Invoke(d, new object[] { message });
                }
                else
                {
                    this.CchattingDetailText.Text = message + "\r\n" + this.CchattingDetailText.Text;
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void writeLogText(string message)
        {
            try
            {
                if (this.ClogText.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(writeLogText);
                    this.Invoke(d, new object[] { message });
                }
                else
                {
                    this.ClogText.Text = message + "\r\n" + this.ClogText.Text;
                    this.ClogText.Update();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void CChattingContentBtn_Click(object sender, EventArgs e)
        {
            string text = "";
            if (File.Exists("log.log"))
            {
                //List<string> textArray = comonClass.ReadTxtFile("log.log");
                //for (int i = 0; i < textArray.Count; i++)
                //{
                //    if (textArray[i] == bjId)
                //    {
                //        textArray.RemoveRange(i, 7);
                //    }
                //}
                //for (int i = 0; i < textArray.Count; i++)
                //{
                //    text += textArray[i] + "\r\n";
                //}
                File.WriteAllText("log.log", String.Empty);
            }

            text += bjId + "\r\n";
            bool enterCheck = this.CgreetCheckBox.Checked;
            string enter = this.CgreetText.Text;
            string enterNickText = this.CgreetNickText.Text;
            text += enterCheck + "," + enterNickText + "," + enter + "\r\n";

            bool presentCheck = this.CpresentCheckBox.Checked;
            string presentNick = this.CpresentNickText.Text;
            string presentContent = this.CpresentContentText.Text;
            text += presentCheck + "," + presentNick + "," + presentContent + "\r\n";

            bool comboCheck = this.CcomboCheckBox.Checked;
            string comboNick = this.CcomboNickText.Text;
            string comboContent = this.CcomboContentText.Text;
            text += comboCheck + "," + comboNick + "," + comboContent + "\r\n";

            bool dusanCheck = this.CdusanCheckBox.Checked;
            string dusanNick = this.CdusanNickText.Text;
            string dusanContent = this.CdusanContentText.Text;
            text += dusanCheck + "," + dusanNick + "," + dusanContent + "\r\n";

            if (CconstantGridView.Rows.Count > 1)
            {
                text += this.CconstantCheckbox.Checked.ToString() + ",";
            }
            else
            {
                text += this.CconstantCheckbox.Checked.ToString() + "\r\n";
            }
            for (int i = 0; i < CconstantGridView.Rows.Count - 1; i++)
            {
                try
                {
                    if (i == CconstantGridView.Rows.Count - 2)
                    {
                        text += this.CconstantGridView.Rows[i].Cells[0].Value.ToString() + "," + this.CconstantGridView.Rows[i].Cells[1].Value.ToString() + "," + this.CconstantGridView.Rows[i].Cells[2].Value.ToString() + "\r\n";
                    }
                    else
                    {
                        text += this.CconstantGridView.Rows[i].Cells[0].Value.ToString() + "," + this.CconstantGridView.Rows[i].Cells[1].Value.ToString() + "," + this.CconstantGridView.Rows[i].Cells[2].Value.ToString() + ",";
                    }
                }
                catch (Exception ex)
                {

                }
            }
            if (CkeywordGridView.Rows.Count > 1)
            {
                text += this.CkeywordCheckBox.Checked.ToString() + ",";
            }
            else
            {
                text += this.CkeywordCheckBox.Checked.ToString();
            }
            for (int i = 0; i < CkeywordGridView.Rows.Count - 1; i++)
            {

                try
                {
                    if (i == CkeywordGridView.Rows.Count - 2)
                    {
                        text += this.CkeywordGridView.Rows[i].Cells[0].Value.ToString() + "," + this.CkeywordGridView.Rows[i].Cells[1].Value.ToString() + "," + this.CkeywordGridView.Rows[i].Cells[2].Value.ToString();
                    }
                    else
                    {
                        text += this.CkeywordGridView.Rows[i].Cells[0].Value.ToString() + "," + this.CkeywordGridView.Rows[i].Cells[1].Value.ToString() + "," + this.CkeywordGridView.Rows[i].Cells[2].Value.ToString() + ",";

                    }
                }
                catch (Exception ex)
                {

                }
            }

            comonClass.MakeTxtFile("log.log", text);
            MessageBox.Show("보관되였습니다.");
        }

        public void showHistory()
        {
            List<string> textArray = comonClass.ReadTxtFile("log.log");
            for (int i = 0; i < textArray.Count; i++)
            {
                try
                {
                    if (textArray[i] == bjId)
                    {
                        this.CgreetCheckBox.Checked = Convert.ToBoolean(textArray[i + 1].Split(',')[0]);
                        this.CgreetNickText.Text = Convert.ToString(textArray[i + 1].Split(',')[1]);
                        this.CgreetText.Text = Convert.ToString(textArray[i + 1].Split(',')[2]);
                        //2020-12-18 10:12 swh edited
                        this.CgreetText.Update();
                        this.CgreetNickText.Update();
                        this.CgreetCheckBox.Update();
                        //2020-12-18 10:12 swh edited

                        this.CpresentCheckBox.Checked = Convert.ToBoolean(textArray[i + 2].Split(',')[0]);
                        this.CpresentNickText.Text = Convert.ToString(textArray[i + 2].Split(',')[1]);
                        this.CpresentContentText.Text = Convert.ToString(textArray[i + 2].Split(',')[2]);

                        //2020-12-18 10:12 swh edited
                        this.CpresentContentText.Update();
                        this.CpresentCheckBox.Update();
                        this.CpresentNickText.Update();
                        //2020-12-18 10:12 swh edited

                        this.CcomboCheckBox.Checked = Convert.ToBoolean(textArray[i + 3].Split(',')[0]);
                        this.CcomboNickText.Text = Convert.ToString(textArray[i + 3].Split(',')[1]);
                        this.CcomboContentText.Text = Convert.ToString(textArray[i + 3].Split(',')[2]);

                        //2020-12-18 10:12 swh edited
                        this.CcomboContentText.Update();
                        this.CcomboCheckBox.Update();
                        this.CcomboNickText.Update();
                        //2020-12-18 10:12 swh edited

                        this.CdusanCheckBox.Checked = Convert.ToBoolean(textArray[i + 4].Split(',')[0]);
                        this.CdusanNickText.Text = Convert.ToString(textArray[i + 4].Split(',')[1]);
                        this.CdusanContentText.Text = Convert.ToString(textArray[i + 4].Split(',')[2]);
                        //2020-12-18 10:12 swh edited
                        this.CcomboContentText.Update();//2020-12-18 10:12 swh edited
                        this.CcomboNickText.Update();
                        this.CdusanCheckBox.Update();

                        string[] constant = textArray[i + 5].Split(',');
                        this.CconstantCheckbox.Checked = Convert.ToBoolean(constant[0]);
                        for (int j = 0; j < constant.Length; j++)
                        {
                            if (j % 4 == 1)
                            {
                                this.CconstantGridView.Rows.Add((constant[j]), constant[j + 1], constant[j + 2]);//2020-12-18 10:12 swh edited
                            }
                        }
                        this.CconstantGridView.Update();//2020-12-18 10:12 swh edited
                        string[] keyword = textArray[i + 6].Split(',');
                        this.CkeywordCheckBox.Checked = Convert.ToBoolean(keyword[0]);
                        for (int j = 0; j < keyword.Length; j++)
                        {
                            if (j % 3 == 1)
                            {
                                this.CkeywordGridView.Rows.Add((keyword[j]), keyword[j + 1], keyword[j + 2]);//2020-12-18 10:12 swh edited
                            }
                        }
                        this.CkeywordGridView.Update();//2020-12-18 10:12 swh edited
                    }

                }
                catch (Exception ex)
                {

                }
            }
        }

        private void CchattingText_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyASCII = Convert.ToInt32(e.KeyChar);
            if (keyASCII == 13)
            {
                if (sesskey == "" || roomId == "" || managerId == "")
                {
                    MessageBox.Show("방입장이 실패하엇습니다.");
                    return;
                }
                //socket.Connect(); //2020-12-18 10:55 swh edited
                Thread.Sleep(1000);
                sendMessage(roomId, this.CchattingText.Text);
                this.CchattingText.Text = "";
            }
        }
        //방 탈퇴하기
        private async void CcloseRoomBtn_Click(object sender, EventArgs e)
        {
            await socket.DisconnectAsync();
        }
    }

    class MyJsonSerializer : NewtonsoftJsonSerializer
    {
        public MyJsonSerializer(int eio) : base(eio) { }

        public override JsonSerializerSettings CreateOptions()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new global::Newtonsoft.Json.Serialization.DefaultContractResolver
                {
                    NamingStrategy = new global::Newtonsoft.Json.Serialization.CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            };
        }
    }

    class MyConnectInterval : IConnectInterval
    {
        public MyConnectInterval()
        {
            delay = 1000;
        }

        double delay;

        public int GetDelay()
        {
            Console.WriteLine("GetDelay: " + delay);
            return (int)delay;
        }

        public double NextDealy()
        {
            Console.WriteLine("NextDealy: " + (delay + 1000));
            return delay += 1000;
        }
    }
}