using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace AutoUpdate
{
    public delegate void UpdateState();
    public class SoftUpDate
    {

        public event UpdateState UpdateFinish;


        //加载的文件
        private string loadFile;
        //新的版本号
        private string newVerson;
        //项目名
        private string softName;
        //是否需要更新状态
        private bool isUpdate;
        //压缩包名字
        private string _fileZipName = string.Empty;

        public bool IsUpdate
        {
            set { this.isUpdate = value; }
            get
            { 
                return isUpdate;
            }
        }

       
        public string LoadFile
        {
            get { return loadFile; }
            set { this.loadFile = value; }
        }
        public string SoftName
        {
            get { return softName; }
            set { this.softName = value; }
        }
        public string NewVerson
        {
            set { this.newVerson = value; }
            get { return newVerson; }
        }

        public string FileZipName
        {
            get { return _fileZipName; }
            set { this._fileZipName = value; }
        }
        public SoftUpDate()
        {
        }
        public SoftUpDate(string file, string softNmae)
        {
            this.LoadFile = file;
            this.SoftName = softNmae;
        }

    }
}
