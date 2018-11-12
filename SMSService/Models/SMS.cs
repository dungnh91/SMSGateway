using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SMSService.Models
{
    public class SMS
    {
        #region Open and Close Ports
        //Open Port
        public SerialPort OpenPort(string p_strPortName, int p_uBaudRate, int p_uDataBits, int p_uReadTimeout, int p_uWriteTimeout)
        {
            receiveNow = new AutoResetEvent(false);
            SerialPort port = new SerialPort();

            try
            {
                port.PortName = p_strPortName;                 //COM1
                port.BaudRate = p_uBaudRate;                   //9600
                port.DataBits = p_uDataBits;                   //8
                port.StopBits = StopBits.One;                  //1
                port.Parity = Parity.None;                     //None
                port.ReadTimeout = p_uReadTimeout;             //300
                port.WriteTimeout = p_uWriteTimeout;           //300
                port.Encoding = Encoding.GetEncoding("iso-8859-1");
                port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                port.Open();
                port.DtrEnable = true;
                port.RtsEnable = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return port;
        }

        //Close Port
        public void ClosePort(SerialPort port)
        {
            try
            {
                port.Close();
                port.DataReceived -= new SerialDataReceivedEventHandler(port_DataReceived);
                port = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        //Execute AT Command
        public string ExecCommand(SerialPort port, string command, int responseTimeout, string errorMessage)
        {
            try
            {

                port.DiscardOutBuffer();
                port.DiscardInBuffer();
                receiveNow.Reset();
                port.Write(command + "\r");
                string input = "";
                if (command.Contains("AT+CUSD"))
                    input = ReadUSSD(port, responseTimeout);
                else
                    input = ReadResponse(port, responseTimeout);
                //if ((input.Length == 0) || ((!input.EndsWith("\r\n> ")) && (!input.Contains("\r\nOK\r\n"))))
                //  throw new ApplicationException("No success message was received.");
                return input;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(errorMessage, ex);
            }
        }

        //Receive data from port
        public void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (e.EventType == SerialData.Chars)
                    receiveNow.Set();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string ReadResponse(SerialPort port, int timeout)
        {
            string buffer = string.Empty;
            try
            {
                do
                {
                    if (receiveNow.WaitOne(timeout, false))
                    {
                        string t = port.ReadExisting();
                        buffer += t;
                    }
                    else
                    {
                        if (buffer.Length > 0)
                            throw new ApplicationException("Response received is incomplete.");
                        else
                            throw new ApplicationException("No data received from phone.");
                    }
                }
                while (!buffer.EndsWith("\r\nOK\r\n") && !buffer.EndsWith("\r\n> ") && !buffer.EndsWith("\r\nERROR\r\n"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return buffer;
        }

        //////////////////////////////////////////
        public string ReadUSSD(SerialPort port, int timeout)
        {
            string buffer = string.Empty;
            try
            {
                do
                {
                    if (receiveNow.WaitOne(timeout, false))
                    {
                        string t = port.ReadExisting();
                        Thread.Sleep(1000);
                        buffer += t;
                    }
                    else
                    {
                        if (buffer.Length > 0)
                            throw new ApplicationException("Response received is incomplete.");
                        else
                            throw new ApplicationException("No data received from phone.");
                    }
                }
                while (!buffer.EndsWith(",15\r\n"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return buffer;
        }
        ///
        #region Count SMS
        public int CountSMSmessages(SerialPort port)
        {
            int CountTotalMessages = 0;
            try
            {

                #region Execute Command

                string recievedData = ExecCommand(port, "AT", 300, "No phone connected at ");
                recievedData = ExecCommand(port, "AT+CMGF=1", 300, "Failed to set message format.");
                String command = "AT+CPMS?";
                recievedData = ExecCommand(port, command, 1000, "Failed to count SMS message");
                int uReceivedDataLength = recievedData.Length;

                #endregion

                #region If command is executed successfully
                if ((recievedData.Length >= 45) && (recievedData.StartsWith("AT+CPMS?")))
                {

                    #region Parsing SMS
                    string[] strSplit = recievedData.Split(',');
                    string strMessageStorageArea1 = strSplit[0];     //SM
                    string strMessageExist1 = strSplit[1];           //Msgs exist in SM
                    #endregion

                    #region Count Total Number of SMS In SIM
                    CountTotalMessages = Convert.ToInt32(strMessageExist1);
                    #endregion

                }
                #endregion

                #region If command is not executed successfully
                else if (recievedData.Contains("ERROR"))
                {

                    #region Error in Counting total number of SMS
                    string recievedError = recievedData;
                    recievedError = recievedError.Trim();
                    recievedData = "Following error occured while counting the message" + recievedError;
                    #endregion

                }
                #endregion

                return CountTotalMessages;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region Read SMS

        public AutoResetEvent receiveNow;

        public ShortMessageCollection ReadSMS(SerialPort port)
        {

            // Set up the phone and read the messages
            ShortMessageCollection messages = null;
            try
            {
                #region Execute Command
                // Check connection
                ExecCommand(port, "AT", 300, "No phone connected");
                // Use message format "Text mode"
                ExecCommand(port, "AT+CMGF=1", 300, "Failed to set message format.");
                // Use character set "PCCP437"
                ExecCommand(port, "AT+CSCS=\"PCCP437\"", 300, "Failed to set character set.");
                // Select SIM storage
                ExecCommand(port, "AT+CPMS=\"SM\"", 300, "Failed to select message storage.");
                // Read the messages
                string input = ExecCommand(port, "AT+CMGL=\"ALL\"", 5000, "Failed to read the messages."); //chờ 5 giây
                #endregion

                #region Parse messages
                messages = ParseMessages(input);
                #endregion

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            if (messages != null)
                return messages;
            else
                return null;

        }
        public ShortMessageCollection ParseMessages(string input)
        {
            ShortMessageCollection messages = new ShortMessageCollection();
            try
            {
                Regex r = new Regex(@"\+CMGL: (\d+),""(.+)"",""(.+)"",(.*),""(.+)""\r\n(.+)\r\n");
                Match m = r.Match(input);
                while (m.Success)
                {
                    ShortMessage msg = new ShortMessage();
                    //msg.Index = int.Parse(m.Groups[1].Value);
                    msg.Index = m.Groups[1].Value;
                    msg.Status = m.Groups[2].Value;
                    msg.Sender = m.Groups[3].Value;
                    msg.Alphabet = m.Groups[4].Value;
                    msg.Sent = m.Groups[5].Value;
                    msg.Message = m.Groups[6].Value;
                    messages.Add(msg);

                    m = m.NextMatch();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return messages;
        }

        #endregion

        #region Send SMS - Kiem tra qua trinh gui tin nhan

        static AutoResetEvent readNow = new AutoResetEvent(false);

        public bool sendMsg(SerialPort port, string PhoneNo, string Message)
        {
            bool isSend = false;

            try
            {

                string recievedData = ExecCommand(port, "AT", 300, "No phone connected");
                recievedData = ExecCommand(port, "AT+CMGF=1", 300, "Failed to set message format.");
                String command = "AT+CMGS=\"" + PhoneNo + "\"";
                recievedData = ExecCommand(port, command, 300, "Failed to accept phoneNo");
                command = Message + char.ConvertFromUtf32(26) + "\r";
                recievedData = ExecCommand(port, command, 5000, "Failed to send message"); //5 seconds
                if (recievedData.EndsWith("\r\nOK\r\n"))
                {
                    isSend = true;
                }
                else if (recievedData.Contains("ERROR"))
                {
                    isSend = false;
                }
                return isSend;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        static void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (e.EventType == SerialData.Chars)
                    readNow.Set();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Delete SMS
        public bool DeleteMsg(SerialPort port, string p_strCommand)
        {
            bool isDeleted = false;
            try
            {

                #region Execute Command
                string recievedData = ExecCommand(port, "AT", 300, "No phone connected");
                recievedData = ExecCommand(port, "AT+CMGF=1", 300, "Failed to set message format.");
                String command = p_strCommand;
                recievedData = ExecCommand(port, command, 1000, "Failed to delete message");
                #endregion

                if (recievedData.EndsWith("\r\nOK\r\n"))
                {
                    isDeleted = true;
                }
                if (recievedData.Contains("ERROR"))
                {
                    isDeleted = false;
                }
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        #endregion

        //thu kiem tra account //1: TK chinh 2: TK khuyen mai
        public string GetBalances(SerialPort port, int n)
        {
            try
            {
                string provider = GetNameServiceCenter(port).Replace("\"", "").Trim('\n').Trim(' ');
                string recievedData = ExecCommand(port, "AT", 300, "No phone connected");
                string command = "AT+CMGF=1";
                recievedData = ExecCommand(port, command, 300, "Failed to set message format.");
                string result = "";
                SIM sim = new SIM(provider);
                string ma = "";
                if (n == 1)
                {
                    ma = sim.maTKChinh;
                    switch (provider)
                    {
                        case "VN MOBIFONE":
                            result = GuiUSSD(command, recievedData, ma, port);
                            if (recievedData.Contains("+CUSD: 1"))
                            {
                                command = "AT+CUSD=1,\"" + ma + "\",15";
                                ExecCommand(port, command, 300, "Failed to set message format.");
                                Thread.Sleep(1000);
                            }
                            break;
                        default:
                            result = GuiUSSD(command, recievedData, ma, port);
                            break;
                    };
                }
                else if (n == 2)
                {
                    ma = sim.maTKPhu;
                    switch (provider)
                    {
                        case "VN MOBIFONE":
                            result = GuiUSSD(command, recievedData, ma, port);
                            if (recievedData.Contains("+CUSD: 1"))
                            {
                                command = "AT+CUSD=1,\"" + ma + "\",15";
                                ExecCommand(port, command, 300, "Failed to set message format.");
                                Thread.Sleep(1000);
                            }
                            break;
                        default:
                            result = GuiUSSD(command, recievedData, ma, port);
                            break;
                    };
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Xu ly service provider name

        #region Lấy tên nhà mạng
        public string GetNameServiceCenter(SerialPort port)
        {
            string recievedData = ExecCommand(port, "AT+COPS=3,0", 100, "Failed to get sms center");
            recievedData = ExecCommand(port, "AT+COPS?", 100, "Failed to get sms center");
            string[] split = recievedData.Replace("\r\n", "").Replace("OK", "").Split(',');

            string result = split[2].Replace("\"", "");
            return result;
        }
        #endregion

        public string GuiUSSD(string command, string recievedData, string provider, SerialPort port)
        {
            command = "AT+CUSD=1,\"" + provider + "\",15";
            recievedData = ExecCommand(port, command, 2000, "Failed to set message format.");
            string result = "";

            if (recievedData.Contains("+CUSD: "))
            {
                string[] USSD = recievedData.Split(':');
                for (int i = 1; i < USSD.Length; i++)
                {
                    result += USSD[i].ToString();
                }
                Thread.Sleep(500);
                result = result.Split('\"')[1].Trim(' ');
            }
            else
                result = "ERROR";
            return result;
        }

        public string LoadSIM(SerialPort port)
        {
            string ma = "";
            string provider = GetNameServiceCenter(port);
            SIM sim = new SIM(provider);
            ma = sim.maTimSoSIM;

            string command = "AT+CUSD=1,\"" + ma + "\",15";
            string recievedData = ExecCommand(port, command, 2000, "Failed to set message format.");
            string result = "";
            if (recievedData.Contains("+CUSD:"))
            {
                string[] chuoi = recievedData.Replace("\r\nOK\r\n", "").Split('\"');
                result = chuoi[3].Split('\r')[1];
                Thread.Sleep(500);
            }
            else
                result = "ERROR";
            return result;
        }
    }
}
