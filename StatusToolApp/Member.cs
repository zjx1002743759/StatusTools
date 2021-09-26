using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    /// <summary>
    /// 接收json格式的数据，将需要的成员信息封装
    /// </summary>
    [Serializable]
   public class Member
    {
        private string job_name;
        private string build_no;
        private string result;
        private string timestamp;
        private string build_url;
        private Boolean current_status=false;

        public string Job_name
        {
            get { return job_name; }
            set { this.job_name = value; }
        }
        public string Build_no
        {
            get { return build_no; }
            set { this.build_no = value; }
        }
        public string Result
        {
            get { return result; }
            set { this.result = value; }
        }
        public string Timestamp
        {
            get { return timestamp; }
           set { this.timestamp = value; }
        }
        public string Build_Url
        {
            get { return build_url; }
            set { this.build_url = value; }
        }
        public Boolean Current_Status
        {
            get { return current_status; }
            set { this.current_status = value;}
        }

    }
}
