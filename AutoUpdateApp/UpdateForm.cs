using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Windows.Data;
using System.Collections;
using System.Diagnostics;

namespace AutoUpdate
{
  
    public partial class UpdateForm : Form
    {
        public delegate void UpdateState();
        SoftUpDate app = new SoftUpDate(Application.ExecutablePath, "WindowsFormsApp1");
        bool IsUpdate;
        public UpdateForm()
        {
            InitializeComponent();
            CheckUpdate(UpdateStatus);
            

        }
        /// <summary>
        /// 调用cmd命令行执行杀死进程的目录，且当进程被杀死之后回调更新方法
        /// </summary>
        /// <param name="command"></param>
        /// <param name="onExit"></param>
        /// <returns></returns>
        private static Process CmdProcessStart(string command, EventHandler onExit)
        {
            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + command);
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.UseShellExecute = false;           
            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.EnableRaisingEvents = true;
            proc.Exited += onExit;
            proc.Start();
            return proc;
        }

        /// <summary>
        /// 更新完成后的事件
        /// </summary>
        public event UpdateState UpdateFinish;

        Version older;
        /// <summary>
        /// 检测版本是否需要更新
        /// </summary>
        /// <returns></returns>
        public  bool CheckUpdateLoad(bool IsUpdate,Version version)
        {
            
            bool result = false;
            
            try
            {
                if (IsUpdate)
                {
                    if (MessageBox.Show("StatusTool检测到新版本，是否更新？", "StatusTool版本检查", MessageBoxButtons.YesNo) ==
                        DialogResult.Yes)
                    {
                        //如果版本需要更新就去下载压缩包
                        Update();

                        //获取压缩包名
                        app.FileZipName = "StatusTool";

                        //杀死要更新的进程
                        CmdProcessStart($"taskkill /F /T /IM WindowsFormsApp1.exe", App_UpdateFinish);
                        older = version;
                        //如果路径有这个文件就删除
                        if (File.Exists(Application.StartupPath + "\\StatusTool.rar"))
                        {
                            try
                            {
                                File.Delete(Application.StartupPath + "\\StatusTool.rar");
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message);
                            }
                        }

                        //如果改路径有这个目录就删除
                        if (Directory.Exists(Application.StartupPath + "\\StatusTool"))
                        {
                            try
                            {
                                Directory.Delete(Application.StartupPath + "\\StatusTool", true);
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message);
                            }
                        }

                        result = true;
                    }
                }
                else
                {
                    result = false;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                result = false;
            }

            return result;
        }
     


        /// <summary>
        /// 复制更新文件，然后删除下载包
        /// </summary>
        public void App_UpdateFinish(object sender, EventArgs e) 
        {
            
           
                //获取压缩包的路径
                string dirEcgPath = Application.StartupPath+"\\StatusTool" ;
                
                //如果文件里没有这个路径就创建它
                if (!Directory.Exists(dirEcgPath))
                {
                    Directory.CreateDirectory(dirEcgPath);
                }

                if (File.Exists(Application.StartupPath + "\\StatusTool.zip"))
                {
                    //如果这个路径有这个压缩包,就解压到指定的路径
                    ZipFile.ExtractToDirectory(Application.StartupPath + "\\StatusTool.zip", dirEcgPath);
                    //解压完成删除
                    File.Delete(Application.StartupPath + "\\StatusTool.zip");

                }

                try
                {
                    //获取新的文件
                    DirectoryInfo directoryInfo = new DirectoryInfo(dirEcgPath);
                    foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                    {
                        //复制解压的新的文件，复制到你当前的工作目录下
                        File.Copy(fileInfo.FullName, Application.StartupPath + "\\" + fileInfo.Name, true);
                    }

                    //删除原有的文件
                    Directory.Delete(dirEcgPath, true);
                    //File.Delete(path);
                    ////覆盖完成

                    ////控制进程的类


                    ////用进程对象启动.exe文件
                    System.Diagnostics.FileVersionInfo fv =
                        System.Diagnostics.FileVersionInfo.GetVersionInfo(
                            Application.StartupPath + "\\WindowsFormsApp1.exe");
                    int num = older.CompareTo(new Version(fv.FileVersion));
                    if (num < 0)
                    {
                        older = null;
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        process.StartInfo.FileName = "WindowsFormsApp1.exe";

                        process.StartInfo.WorkingDirectory = Application.StartupPath;
                        process.StartInfo.CreateNoWindow = true;
                        process.Start();
                        MessageBox.Show("更新成功，已重启程序");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    MessageBox.Show("请关闭系统在执行更新的操作");
                    Application.Exit();

                }

        }

        //隐藏窗体
        private void Form1_Load(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                this.Hide();
                this.Opacity = 1;
            }));
        }


        /// <summary>
        /// 下载压缩包
        /// </summary>
        private void Update()
        {

            try
            {

                if (!IsUpdate)
                {
                    return;
                }

                WebClient wc = new WebClient();

                //目标文件路径
                string filename = Application.StartupPath + "\\";
                FileInfo newfile = new FileInfo(filename + "\\StatusTool.zip");
                //目标文件
                string newPath = filename + newfile.Name;
                string newUpdateUrl = updateUrl + "StatusTool.zip";
                //从指定的url获取文件，然后传输到目标路径
                //异步获取安装包
                wc.DownloadFileAsync(new Uri(newUpdateUrl), newPath);
                //为下载文件添加事件
                
                wc.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(wc_DownLoadFileCompleted);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        /// <summary>
        /// 文件下载成功后触发的方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void wc_DownLoadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

            (sender as WebClient).Dispose();
            UpdateFinish?.Invoke();


        }


        string downLoad;
        //文件远程地址
        string updateUrl = "http://192.168.5.101:8080/files/";


        /// <summary>
        /// 获取状态，传递状态信息，然后回调，是否更新的方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateStatus(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                ArrayList rs = (ArrayList)sender;
                CheckUpdateLoad((bool)rs[0],(Version)rs[1]);

            }));

        }
        /// <summary>
        /// 检查是否需要更新，实时获取程序版本，判断是否更新
        /// </summary>
        private void CheckUpdate(EventHandler<DataTransferEventArgs> SourceUpdatedCallback)
        {
            try
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        //（提供用于将数据发送到由 URI 标识的资源及从这样的资源接收数据的常用方法）
                        WebClient wc = new WebClient();
                        //OpenRead返回读取资源的流对象Stream

                        Stream stream = wc.OpenRead(updateUrl + "Config.xml");
                        //以xml文档的格式去解析

                        XmlDocument xmldoc = new XmlDocument();

                        //加载这个流资源
                        xmldoc.Load(updateUrl + "Config.xml");
                        //获取xml文档中的update节点数据
                        XmlNode list = xmldoc.SelectSingleNode("Update");

                        foreach (XmlNode node in list)
                        {
                            //从xml文档中获取Version的值，然后于程序集的版本号比较
                            if (node.Name == "Soft" && node.Attributes["Name"].Value.ToLower() == app.SoftName.ToLower())
                            {
                                foreach (XmlNode xml in node)
                                {
                                    if (xml.Name == "Version")
                                         app.NewVerson = xml.InnerText;
                                    else
                                        downLoad = xml.InnerText;
                                }
                            }
                        }
                        System.Diagnostics.FileVersionInfo fv = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.StartupPath + "\\WindowsFormsApp1.exe");
                        Version ver = new Version(app.NewVerson);
                        Version version =new Version(fv.FileVersion);

                        int tm = version.CompareTo(ver);
                        if (tm >= 0)
                        {
                            IsUpdate = false;
                        }
                        else
                        {
                            IsUpdate = true;
                        }
                        ArrayList arrayList = new ArrayList();
                        arrayList.Add(IsUpdate);
                        arrayList.Add(version);
                        SourceUpdatedCallback(arrayList,null);

                        Thread.Sleep(3600000 * 24);
                    }

                });
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

    }
}
