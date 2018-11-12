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
using System.Threading;
using System.Threading.Tasks;

namespace SMSService
{
    [ServiceContract(Namespace = "http://SMSService")]
    public class SMSManagement
    {
        private static SerialPort serialPort = null;
        private static SMS sms = new SMS();


        public SMSManagement()
        {
            KetNoiThietBi();
        }

        ~SMSManagement()
        {
            sms.ClosePort(serialPort);
        }

        public void Reconnect()
        {
            KetNoiThietBi();
        }

        private static bool KetNoiThietBi()
        {
            try
            {
                if (serialPort != null && serialPort.IsOpen)
                    return true;

                HeThong devicePort = HeThongService.getHeThongByKhoa(KhoaHeThong.SMS_DEVICE_PORT).FirstOrDefault();
                HeThong timeout = HeThongService.getHeThongByKhoa(KhoaHeThong.SMS_TIMEOUT).FirstOrDefault();
                //Mở kết nối với port
                
                serialPort = sms.OpenPort(devicePort.GiaTri, Convert.ToInt32(115200), Convert.ToInt32(8), Convert.ToInt32(timeout.GiaTri), Convert.ToInt32(timeout.GiaTri));

                if (serialPort != null && serialPort.IsOpen)
                {
                    Console.WriteLine("Modem đã được kết nối tại Port " + devicePort.GiaTri);
                    //File.WriteAllText(@"C:\netmd.log", "Modem da duoc ket noi tai port " + devicePort.GiaTri);
                }
                else
                {
                    File.AppendAllText(@"C:\netmd.log", "Thông số cấu hình port không chính xác !");
                    return false;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\netmd.log", "Message:" + ex.Message);
                File.AppendAllText(@"C:\netmd.log", "StackTrace:" + ex.StackTrace);
                return false;
            }
            return true;
        }

        [OperationContract]
        public bool sendMessageToPhone(string phone, string message)
        {
            //SMS sms = new SMS();
            //if (KetNoiThietBi())
            //{
            //    if (serialPort == null)
            //    {
            //        Console.WriteLine("Không thể kết nối tới thiết bị. Xin hãy kiểm tra lại cấu hình.");
            //    }
            //    else
            //        sms.sendMsg(serialPort, phone, message);
            //}
            //return "";
            try
            {
                if (KetNoiThietBi())
                {
                    bool result = false;
                    while (!result)
                    {
                        result = sms.sendMsg(serialPort, phone, message);
                        Thread.Sleep(1000);
                    }
                    return result;
                }
                else
                {
                    //closeConnect();
                    return sendMessageToPhone(phone, message);
                }
            }
            catch(Exception ex)
            {
                closeConnect();
                File.AppendAllText(@"C:\netmd.log", "Message: " + ex.Message + " - sendMessageToPhone \r\n");
                File.AppendAllText(@"C:\netmd.log", "StackTrace: " + ex.StackTrace + "\r\n");
                File.AppendAllText(@"C:\netmd.log", "========================================================\r\n");
                return false;
            }
        }

        [OperationContract]
        public string sendMessageToPhones(string[] phones, string message)
        {
            try
            {
                if (!KetNoiThietBi())
                {
                    Console.WriteLine("Không thể kết nối tới thiết bị. Xin hãy kiểm tra lại cấu hình.");
                }
                foreach (string phone in phones)
                {
                    sms.sendMsg(serialPort, phone, message);
                }
            }
            catch (Exception ex)
            {
                closeConnect();
                File.AppendAllText(@"C:\netmd.log", "Message:" + ex.Message);
                File.AppendAllText(@"C:\netmd.log", "StackTrace:" + ex.StackTrace);
                File.AppendAllText(@"C:\netmd.log", "========================================================");
            }
            return "";
        }

        [OperationContract]
        public string getBalance(int taiKhoan)
        {
            try
            {
                if (KetNoiThietBi())
                {
                    return sms.GetBalances(serialPort, taiKhoan);
                }
            }
            catch (Exception ex)
            {
                closeConnect();
                File.AppendAllText(@"C:\netmd.log", "Message:" + ex.Message + "\r\n");
                File.AppendAllText(@"C:\netmd.log", "StackTrace:" + ex.StackTrace + "\r\n");
                File.AppendAllText(@"C:\netmd.log", "========================================================\r\n");
            }
            return "";
        }

        [OperationContract]
        public string GetNetworkName()
        {
            try
            {
                if (KetNoiThietBi())
                {
                    string result = sms.GetNameServiceCenter(serialPort);
                    return result;
                }
            }
            catch (Exception ex)
            {
                //closeConnect();
                File.AppendAllText(@"C:\netmd.log", "Message: " + ex.Message + " - GetNetworkName\r\n");
                File.AppendAllText(@"C:\netmd.log", "StackTrace: " + ex.StackTrace + "\r\n");
                File.AppendAllText(@"C:\netmd.log", "========================================================\r\n");
                return GetNetworkName();
            }
            return "";
        }

        private void closeConnect()
        {
            try
            {
                sms.ClosePort(serialPort);
                serialPort = null;
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\netmd.log", "Message: " + ex.Message + " - closeConnect\r\n");
                File.AppendAllText(@"C:\netmd.log", "StackTrace: " + ex.StackTrace + "\r\n");
                File.AppendAllText(@"C:\netmd.log", "========================================================\r\n");
            }
            finally
            {
                serialPort = null;
            }
        }
    }
}
