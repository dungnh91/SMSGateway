using NetworkMonitor.Models;
using NetworkMonitor.Models.HeThongEntity;
using SMSService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SMSService
{
    [ServiceContract]
    public interface ISMSManagement
    {

        [OperationContract]
        bool sendMessageToPhone(string phone, string message);

        [OperationContract]
        string sendMessageToPhones(string[] phones, string message);

        [OperationContract]
        string getBalance(int taiKhoan);

        [OperationContract]
        string GetNetworkName();
    }
}
