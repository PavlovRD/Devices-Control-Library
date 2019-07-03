using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DevicesControlLibrary.Exchange
{
    public class LanExchanger
    {
        #region Private Properties

        /// <summary>
        ///     Ip address device
        /// </summary>
        private IPAddress IpAddress { get; set; }

        /// <summary>
        ///     Ip port device
        /// </summary>
        private int IpPort { get; set; }

        /// <summary>
        ///     Socket for exchange with device
        /// </summary>
        private Socket Socket { get; set; }

        #endregion

        public LanExchanger(string ipAddress, int ipPort)
        {
            IpAddress = IPAddress.Parse(ipAddress);
            IpPort = ipPort; // 1 -65 535
        }

        /// <summary>
        ///     Sending a command to the device and returning the result as an integer value
        /// </summary>
        /// <param name="command">String, contains command</param>
        /// <returns>Integer value of response</returns>
        public int SendWithRequestInt(string command)
        {
            return Convert.ToInt32(QueryControl(command));
        }

        /// <summary>
        ///     Sending a command to the device and returning the result as an string value
        /// </summary>
        /// <param name="command">String, contains command</param>
        /// <returns>String value of response</returns>
        public string SendWithRequestString(string command)
        {
            return QueryControl(command);
        }

        /// <summary>
        ///     Sending a command to the device without returning
        /// </summary>
        /// <param name="command">String, contains command</param>
        public void SendWithoutRequest(string command)
        {
            QueryControl(command);
        }

        /// <summary>
        ///     Sending a command to the device and returning the result as an double value
        /// </summary>
        /// <param name="command">String, contains command</param>
        /// <returns>Double value of response</returns>
        public double SendWithRequestDouble(string command)
        {
            var currentDoubleSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var result = QueryControl(command);

            return Convert.ToDouble(!result.Contains(currentDoubleSeparator)
                ? result.Replace(result.Contains(".") ? "." : ",", currentDoubleSeparator)
                : QueryControl(command));
        }

        public void InitConnection()
        {
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string QueryControl(string command)
        {
            try
            {
                RequestSending(command);
                return command.IndexOf("?", StringComparison.CurrentCulture) >= 0 ? ResponseReceive() : "";
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool RequestSending(string command)
        {
            var ping = new Ping();
            var pingReply = ping.Send(IpAddress, 5000);

            if (pingReply != null && pingReply.Status != IPStatus.Success)
            {
                throw new Exception("Device not responding");
            }
            else
            {
                Socket.Send(Encoding.ASCII.GetBytes(command + Environment.NewLine));
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ResponseReceive()
        {
            var buffer = new byte[256];
            buffer = new byte[buffer.Length];

            try
            {
                int readBytes;
                do
                {
                    readBytes = Socket.Receive(buffer);
                } while (Socket.Available < 0);

                return Encoding.ASCII.GetString(buffer, 0, readBytes);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}