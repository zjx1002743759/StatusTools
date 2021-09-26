using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WindowsFormsApp1
{


   
    public partial class StatusMainForm : Form
    {
        


        List<Member> listmember = new List<Member>();
        static  List<Member> listAllMember = new List<Member>();

        public StatusMainForm()
        {
            InitializeComponent();
            

        }




        /// <summary>
        /// 托盘控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
           
        }


        /// <summary>
        /// 右键菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {


        }



        /// <summary>
        /// 隐藏窗体事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
           
            //开机自启
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            registryKey.SetValue("BaichuiMonitor", Application.ExecutablePath);//"BaichuiMonitor"可以自定义

            //隐藏窗口
            //当窗口加载，回调方法设置窗口状态
            this.BeginInvoke(new Action(() =>
            {
                this.Hide();
                this.Opacity = 1;
            }));


            List<Member> list = new List<Member>();
            
            list = ReadProcess(UserConfPath);
            if (list == null)
            {
                SaveProcess(list,UserConfPath);
               
            }
            
            AddAllToolStripMenuItem(list);
            foreach (Member l in list)
            {               
                listAllMember.Add(l);
                ShowTableAdaptiveWidthColumn(l);
                if (l.Current_Status==true) {
                    SwitchClick(l, e);
                }
            }

            string url = Application.StartupPath + "AutoUpdate.exe";
            CmdProcessStart($"start " + url);

        }

        /// <summary>
        /// 通过url获取资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private static Member HttpGet(string Job_name)
        {
            Member m = new Member();

            try
            {
                if (Job_name == null || Job_name == string.Empty) return null;
                string url = $"http://192.168.5.100:9200/jobstatus/_doc/{Job_name}".ToLower();

                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                Encoding encoding = Encoding.UTF8;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                //请求方式为Get
                request.Method = "GET";
                // 接受数据格式为json，字符集为UTF-8
                request.ContentType = "application/json; charset=UTF-8";
                //获取响应状态
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //获取字节流
                Stream myResponseStream = response.GetResponseStream();
                //创建获取读取字节流的实例
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                //从头到尾读取字符串
                string retString = myStreamReader.ReadToEnd();
                JObject obj = (JObject)JsonConvert.DeserializeObject(retString);
                JToken s = obj["_source"];
                //创建成员对象，将获取的数据都封装进对象

                m.Job_name = s["job_name"].ToString();
                m.Build_no = s["build_no"].ToString();
                m.Result = s["result"].ToString();
                m.Timestamp = s["@timestamp"].ToString();


                //关闭流
                myStreamReader.Close();
                myResponseStream.Close();
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }

                //返回这个对象
                return m;
            }
            catch (Exception e)
            {
                for (int i = 0; i < listAllMember.Count; i++) {
                    if (listAllMember[i].Job_name.Equals(Job_name)) {
                        listAllMember.Remove(listAllMember[i]);
                    }
                }
                return null;
            }

        }

        private static Process CmdProcessStart(string command)
        {
            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + command);
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.UseShellExecute = false;
            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.EnableRaisingEvents = true;
            proc.Start();
            return proc;
        }


        /// <summary>
        /// 退出事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Member> list = new List<Member>();
            foreach (Member s in listAllMember)
            {              
                list.Add(s);
            }
            SaveProcess(list, UserConfPath);
            Icon.Dispose();
            CmdProcessStart($"taskkill /F /T /IM AutoUpdate.exe");
            //关闭程序
            Application.Exit();

        }



        /// <summary>
        /// 菜单设置项点击击事件，点击触发图标的闪烁事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ShowTableAdaptiveWidthColumn();

            //显示窗口
            this.Show();

        }


        //ToolStripMenuItem job = new System.Windows.Forms.ToolStripMenuItem();
        /// <summary>
        /// 开机时读取文件缓存，获取全部的项目名
        /// 动态添加控件
        /// 然后将一个对象传给，定时获取数据的方法，获值，然后显示
        /// </summary>
        private void AddAllToolStripMenuItem(List<Member> s)
        {
            if (s.Count == 0)
            {
                return;
            }
            else
            {
                foreach (Member m in s)
                {

                    ToolStripMenuItem job = new System.Windows.Forms.ToolStripMenuItem();
                    //验证项目状态

                    //Twinkle_Tick(m.Job_name);
                    //设置名字
                    job.Name = m.Job_name;
                    job.Size = new System.Drawing.Size(180, 22);
                    job.Text = m.Job_name;
                    job.Click += SwitchClick;
                    this.contextMenuStrip1.Items.Insert(0, job);
                    //定时获取数据
                    //回调
                    GetTimingData(job, BuildBoard_SourceUpdated);
                    //验证项目状态，进行显示

                }
            }
        }


        /// <summary>
        /// Add项目名，创建一个新控件
        /// </summary>
        /// <param name="mem"></param>
        /// <returns></returns>
        private Member AddToolStripMember(Member m)
        {
            if (listmember.Count == 0)
            {
                return null;
            }
            else
            {
                ToolStripMenuItem job = new System.Windows.Forms.ToolStripMenuItem();
                //验证项目状态

                //Twinkle_Tick(m.Job_name);
                //设置名字
                job.Name = m.Job_name;
                job.Size = new System.Drawing.Size(180, 22);
                job.Text = m.Job_name;
                job.Click += SwitchClick;
                this.contextMenuStrip1.Items.Insert(0, job);
                //定时获取数据
                //回调

                GetTimingData(m, job, BuildBoard_SourceUpdated);

                //验证项目状态，进行显示
            }
            return m;

        }



        /// <summary>
        /// 点击菜单项中的一项，触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwitchClick(object sender, EventArgs e)
        {
            ToolStripMenuItem job = sender as ToolStripMenuItem;
            Member m = sender as Member;
            if (m != null) {
                m_jobName = m.Job_name;
                GetNotifyIconSelected(Build_Jobname);
                this.notifyIcon1.Text = m.Job_name;
            }
            if (job != null)
            {
                m_jobName = job.Text;

                GetNotifyIconSelected(Build_Jobname);

                this.notifyIcon1.Text = job.Text;
            }
            for (int i = 0; i < listAllMember.Count; i++)
            {
                
                    listAllMember[i].Current_Status = false;
                
            }
            for (int i = 0; i < listAllMember.Count; i++)
            {
                if (listAllMember[i].Job_name == m_jobName)
                {
                    listAllMember[i].Current_Status = true;
                }
            }
        }

        //缓存初始值，用于修改点击之后获取的Job_name
        string m_jobName = null;


        /// <summary>
        /// 判断项目状态，控制图标闪烁
        /// </summary>
        /// <param name="m"></param>
        /// <param name="job"></param>
        private void GetNotifyIconSelected(EventHandler<DataTransferEventArgs> SourceUpdatedCallback)
        {
           
            Task.Run(() =>
            {
                while (true)
                {
                    Member m = HttpGet(m_jobName);
                    if (m == null) continue;
                    
                    Console.WriteLine(m.Job_name);
                    Console.WriteLine(m.Result);
                    ArrayList rs = new ArrayList();
                    rs.Add(m);
                    //存储这个集合，回调传给Build_Jobname函数
                    SourceUpdatedCallback(rs, null);
                    Thread.Sleep(20000);
                }

            });
           


        }

        /// <summary>
        /// 点击菜单项中的一项后，传入的函数，并回调一个闪烁功能的函数ChangedIcon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        ///     
        private void Build_Jobname(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                ArrayList rs = sender as ArrayList;
                //传递点击对象的更新数据，实现颜色修改
                
                ChangedIcon((Member)rs[0]);

            }));

        }

       









        //用于缓存同一个build_no下项目失败的状态
        List<Member> newMem = new List<Member>();
        private static object _lock = new object();
        /// <summary>
        /// 图标的闪烁状态和当前获得的项目的图标切换
        /// </summary>
        /// <param name="m"></param>
        private void ChangedIcon(Member m) {
            bool _status = true;
            //获取两张切换的图片
            //图标
            Icon normal = Properties.Resources.favicon;
            //
            Icon blank = Properties.Resources.favicon1;
            //string result = m.Result;

            string result = m.Result;
            
            //根据状态来设置图标
            if (result != null)
            {
                if (result.Equals("SUCCESS"))
                {
                    this.notifyIcon1.Icon = Icon.FromHandle(((Bitmap)Properties.Resources.Successs).GetHicon());

                    //job.BackColor = Color.FromArgb(56, 194, 33);
                }
                else if (result.Equals("FAILURE"))
                {
                    this.notifyIcon1.Icon = Icon.FromHandle(((Bitmap)Properties.Resources.Failure).GetHicon());

                }
                else if (result.Equals("ABORTED "))
                {
                    this.notifyIcon1.Icon = Icon.FromHandle(((Bitmap)Properties.Resources.Successs).GetHicon());
                }
                else if (result.Equals("UNSTABLE "))
                {
                    this.notifyIcon1.Icon = Icon.FromHandle(((Bitmap)Properties.Resources.Unstable).GetHicon());

                }
                else if (result.Equals("RUNNING"))
                {
                    this.notifyIcon1.Icon = Icon.FromHandle(((Bitmap)Properties.Resources.Building).GetHicon());
                    //job.BackColor = Color.FromArgb(56, 240, 33);
                }
            }
            
                if (newMem.Count == 0)
                {
                    if (m.Result.Equals("FAILURE"))
                    {
                        newMem.Add(m);
                    }

                    //开启一个线程，来执行闪烁功能
                    Task.Run(() =>
                    {
                        int index = 10;
                        while (true)
                        {

                            if (m.Result.Equals("FAILURE"))
                            {

                                if (_status)
                                {
                                    notifyIcon1.Icon = Icon.FromHandle(((Bitmap)Properties.Resources.Failure).GetHicon());
                                }
                                else
                                {
                                    notifyIcon1.Icon = blank;
                                }
                                _status = !_status;

                                index--;
                                if (index == 0)
                                {
                                    notifyIcon1.Icon = Icon.FromHandle(((Bitmap)Properties.Resources.Failure).GetHicon());
                                    
                                    
                                    
                                    Console.WriteLine(Thread.CurrentThread.Name);
                                    break;


                                }

                            }
                            else
                            {
                                notifyIcon1.Icon = this.notifyIcon1.Icon;

                            }

                            Thread.Sleep(500);

                        }

                    });
                }
                else
                {
                    if (!m.Build_no.Equals(newMem[0].Build_no)&&!m.Job_name.Equals(newMem[0].Build_no))
                    {
                        if (m.Result.Equals("FAILURE"))
                        {
                            newMem.Add(m);
                            newMem.RemoveAt(0);

                            Task.Run(() =>
                            {
                                int index = 10;

                                while (true)
                                {


                                    if (m.Result.Equals("FAILURE"))
                                    {

                                        if (_status)
                                        {
                                            notifyIcon1.Icon = Icon.FromHandle(((Bitmap)Properties.Resources.Failure).GetHicon());
                                        }
                                        else
                                        {
                                            notifyIcon1.Icon = blank;
                                        }
                                        _status = !_status;
                                        index--;
                                        if (index == 0)
                                        {
                                            notifyIcon1.Icon = Icon.FromHandle(((Bitmap)Properties.Resources.Failure).GetHicon());
                                            
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        notifyIcon1.Icon = this.notifyIcon1.Icon;
                                    }
                                    Thread.Sleep(500);

                                }

                            });

                        }
                    }

                }
            
        }



        /// <summary>
        /// 每个项目的状态显示，每个项目的定时数据传出，为菜单项的每一项设置图片
        /// </summary>
        /// <param name="m"></param>
        /// <param name="job"></param>
        private void ChangedColor(Member m, ToolStripMenuItem job)
        {
            if (m == null) {
                return;
            }
            //设置每个项目的状态图片
            string result = m.Result;
            if (result != null)
            {
                if (result.Equals("SUCCESS"))
                {
                    job.Image = Properties.Resources.Successs;

                    //job.BackColor = Color.FromArgb(56, 194, 33);
                }
                else if (result.Equals("FAILURE"))
                {
                    job.Image = Properties.Resources.Failure;
                    //job.BackColor = Color.FromArgb(255, 0, 0);
                }
                else if (result.Equals("ABORTED"))
                {
                    job.Image = Properties.Resources.Unstable;
                    //job.BackColor = Color.FromArgb(247, 231, 0);
                }
                else if (result.Equals("UNSTABLE"))
                {
                    job.Image = Properties.Resources.Successs;
                    //job.BackColor = Color.FromArgb(112, 105, 0);
                }
                else if (result.Equals("RUNNING"))
                {
                    job.Image = Properties.Resources.Building;
                    //job.BackColor = Color.FromArgb(56, 240, 33);
                }
            }

        }

        /// <summary>
        ///当数据更新就回调这个方法，这个方法被调用时，在进行回调表示主界面对控件的颜色进行操作。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void BuildBoard_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                ArrayList rs = sender as ArrayList;
                //接受项目的更新数据，实现颜色修改
                ChangedColor(((Member)rs[0]), ((ToolStripMenuItem)rs[1]));
            }));

        }

        /// <summary>
        ///定时更新数据，
        /// </summary>
        /// <param name="m"></param>
        /// <param name="job"></param>
        /// <param name="SourceUpdatedCallback"></param>
        private void GetTimingData(Member m, ToolStripMenuItem job, EventHandler<DataTransferEventArgs> SourceUpdatedCallback)
        {
            //开启一个线程
            Task.Run(() =>
            {
               //保证这个线程一直运行
               while (true)
                {   //获取数据

                   Member mem = HttpGet(m.Job_name);
                    ArrayList rs = new ArrayList();
                    rs.Add(mem);
                    rs.Add(job);
                    SourceUpdatedCallback(rs, null);
                   //线程休眠
                   Thread.Sleep(20000);
                }
            });

        }
        /// <summary>
        /// 重载一个定时获取数据的方法
        /// </summary>
        /// <param name="job"></param>
        /// <param name="SourceUpdatedCallback"></param>
        private void GetTimingData(ToolStripMenuItem job, EventHandler<DataTransferEventArgs> SourceUpdatedCallback)
        {
            //开启一个线程
            Task.Run(() =>
            {
                //保证这个线程一直运行
                while (true)
                {   //获取数据        
                    Member mem = HttpGet(job.Text);
                    if (mem != null)
                    {
                        ArrayList rs = new ArrayList();
                        rs.Add(mem);
                        rs.Add(job);
                        SourceUpdatedCallback(rs, null);
                    }

                    //线程休眠
                    Thread.Sleep(20000);
                }
            });

        }


        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Addbutton_Click(object sender, EventArgs e)
        {
            //如果这个TextBox不为null
            if (this.NameComboBox.Text != "")
            {
                if (listAllMember.Count == 0)
                {
                    

                  Member m =HttpGet(this.NameComboBox.Text);
                    if (m == null) {
                        MessageBox.Show("请输入正确的项目名");
                        this.NameComboBox.Text = "";
                        return;
                    }
                    listmember.Add(m);
                    listAllMember.Add(m);
                    Console.WriteLine();
                    MessageBox.Show("添加成功");

                    this.NameComboBox.Text = "";

                    Member NewMem = AddToolStripMember(m);
                    ShowTableAdaptiveWidthColumn(NewMem);

                    return;

                }
                else
                {
                    for (int i = 0; i < listAllMember.Count; i++)
                    {
                        if (!listAllMember[i].Job_name.Equals(this.NameComboBox.Text))
                        {
                            if (i == listAllMember.Count - 1)
                            {
                                Member m = HttpGet(this.NameComboBox.Text);
                                if (m == null)
                                {
                                    MessageBox.Show("请输入正确的项目名");
                                    this.NameComboBox.Text = "";
                                    return;
                                }
                                listmember.Add(m);

                                Console.WriteLine();
                                MessageBox.Show("添加成功");


                                this.NameComboBox.Text = "";
                                Member NewMem = AddToolStripMember(m);
                                listAllMember.Add(NewMem);
                                ShowTableAdaptiveWidthColumn(NewMem);

                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("请勿重复添加");

                            this.NameComboBox.Text = "";
                            return;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("输入不能为空");
            }

        }

        /// <summary>
        /// 关闭设置窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColseButton_Click(object sender, EventArgs e)
        {
            //隐藏窗体
            this.BeginInvoke(new Action(() =>
            {
                this.Hide();
                this.Opacity = 1;
            }));
        }


        /// <summary>
        /// 显示表格数据
        /// </summary>
        private void ShowTableAdaptiveWidthColumn(Member m)
        {


            listView1.GridLines = true;//表格是否显示网格线
            listView1.FullRowSelect = true;//是否选中整行

            listView1.View = View.Details;//设置显示方式
            listView1.Scrollable = true;//是否自动显示滚动条
            listView1.MultiSelect = false;//是否可以选择多行

            //添加表头（列）
            if (listView1.Columns.Count == 0)
            {
                listView1.Columns.Add("Job_name", "项目名");
                listView1.Columns["Job_name"].Width = (int)(listView1.ClientRectangle.Width);//设置宽度

            }
            //添加表格内容

            ListViewItem item = new ListViewItem();
            listView1.Items.Add(m.Job_name);

            //item.SubItems.Add(m.Job_name);          
            //listView1.Items.Add(m.Job_name);
            //listView1.Controls.Add(btn);
            item.SubItems.Clear();

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Deletebutton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认删除？", "此删除不可恢复", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (this.listView1.SelectedItems.Count== 0) {
                    MessageBox.Show("请选定一个项");
                    return;
                }
                //获取被选定的项
                ListViewItem item = this.listView1.SelectedItems[0];
                //把这个元素从listView中移除
                listView1.Items.Remove(item);
                //从全局的list列表移除这个选项
               
                for (int i = 0; i < listAllMember.Count; i++)
                {
                    if (listAllMember[i].Job_name.Equals(item.Text))
                    {
                        listAllMember.Remove(listAllMember[i]);
                    }
                }

                ToolStripItemCollection items = this.contextMenuStrip1.Items;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Text.Equals(item.Text))
                    {
                        this.contextMenuStrip1.Items.Remove(items[i]);
                    }
                }
                List<Member> list = new List<Member>();

                if (File.Exists(UserConfPath))
                {
                    File.Delete(UserConfPath);
                    foreach (Member m in listAllMember)
                    {
                        list.Add(m);
                    }
                    SaveProcess(list, UserConfPath);
                }
                MessageBox.Show("删除成功");
            }

        }




        private static object _locker = new object();
        public static string CurrentPath = Application.StartupPath;
        private static readonly string UserConfPath = $@"{CurrentPath}\WindowsFormsApp1.txt";
        /// <summary>
        /// 读取文件的方法
        /// </summary>
        /// <param name="savedataFullName"></param>
        /// <returns></returns>
        public static List<Member> ReadProcess(string savedataFullName)
        {
            List<Member> JobNameString = new List<Member>();
            lock (_locker)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                if (File.Exists(savedataFullName))
                {
                    using (FileStream fs = File.Open(savedataFullName, FileMode.Open))
                    {
                        JobNameString = (List<Member>)formatter.Deserialize(fs);
                    }
                }
            }
            return JobNameString;
        }
        /// <summary>
        /// 保存数据到本地
        /// </summary>
        /// <param name="list"></param>
        /// <param name="savedataFullName"></param>
        public static void SaveProcess(List<Member> list, string savedataFullName)
        {
            lock (_locker)
            {
                using (Stream ms = File.OpenWrite(savedataFullName))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(ms, list);
                }
            }

        }





        
        /// <summary>
        /// 点击下拉框的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameComboBox_DropDown(object sender, EventArgs e)
        {
            
            this.NameComboBox.Items.Clear();
            //获取当前的时间
            DateTime currentTime = DateTime.UtcNow;
            //获取前天前的时间
            DateTime olderTime = currentTime.AddDays(-7);
            //获取查询条件
            var searchJosn = JObject.Parse(GetAppConf().Element("ELK_SEARCH_STRING").Value);
            //设置查询的时间
            searchJosn["query"]["bool"]["filter"][2]["range"]["@timestamp"]["lte"]=currentTime;
            searchJosn["query"]["bool"]["filter"][2]["range"]["@timestamp"]["gte"] = olderTime;
            //获取查询到的值
            var temp = SearchELK($"http://192.168.5.100:9200/jobstatus/_search", searchJosn);
            JArray array = temp["aggregations"]["42fb8758-98ff-48f9-ae60-0e498058adfe"]["buckets"];
            for (int i = 0; i < array.Count; i++) {
                this.NameComboBox.Items.Add(array[i]["key"]);
            }           
            
                
            
        }



       
      
        /// <summary>
        /// 根据Json数据格式的字符串去查询
        /// </summary>
        /// <param name="url"></param>
        /// <param name="jsonObj"></param>
        /// <returns></returns>
        public static dynamic SearchELK(string url, dynamic jsonObj)
        {
            
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = new StringContent(JsonConvert.SerializeObject(jsonObj), Encoding.UTF8, "application/json");
                HttpResponseMessage rs = HttpClient.SendAsync(request).Result;
            
            return JObject.Parse(rs.Content.ReadAsStringAsync().Result.ToString());
        }


        private static HttpClient _httpClient = null;
        /// <summary>
        /// 返回一个非空的HttpClient对象
        /// </summary>
        public static HttpClient HttpClient
        {
          
                get
                {
                    if (_httpClient == null)
                    {
                    
                        _httpClient = new HttpClient();
                  
                    }
                    return _httpClient;
                }
            
        }
        
       

        private static XElement AppConf;
        /// <summary>
        /// 读取配置文件，返回一个XMLement类型对象
        /// </summary>
        /// <returns></returns>
        public static XElement GetAppConf()
        {
            if (AppConf == null)
            {
                AppConf = XElement.Parse(File.ReadAllText("Config.xml"));
            }
            return AppConf;

        }

     
    }
}


