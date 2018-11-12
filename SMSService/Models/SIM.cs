using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSService.Models
{
    public class SIM
    {
        public string soSIM
        {
            get;
            set;
        }
        public string maTKChinh
        {
            get;
            set;
        }
        public string maTKPhu
        {
            get;
            set;
        }
        public string maTimSoSIM
        {
            get;
            set;
        }
        public SIM(string provider)
        {
            switch (provider)
            {
                case "VIETTEL":
                    this.maTimSoSIM = "*1#";
                    this.maTKChinh = "*101#";
                    this.maTKPhu = "*102#";
                    break;
                case "VN MOBIFONE":
                    this.maTimSoSIM = "*0#";
                    this.maTKChinh = "*101#";
                    this.maTKPhu = "*102#";
                    break;
                case "VN VINAPHONE":
                    this.maTimSoSIM = "*0#";
                    this.maTKChinh = "*110#";
                    this.maTKPhu = "*110#";
                    break;
                default:
                    this.maTimSoSIM = "*1#";
                    this.maTKChinh = "*101#";
                    break;
            };
        }
    }
}
