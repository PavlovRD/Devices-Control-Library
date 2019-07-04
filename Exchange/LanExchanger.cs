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

        /// <summary>
        ///     End point
        /// </summary>
        private EndPoint EndPoint { get; set; }

        /// <summary>
        ///     Object for locking
        /// </summary>
        private object LockObject { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        ///     Constructor.
        ///     Set ip address and port
        /// </summary>
        /// <param name="ipAddress">Ip address device</param>
        /// <param name="ipPort">Ip port device</param>
        public LanExchanger(string ipAddress, int ipPort)
        {
            IpAddress = IPAddress.Parse(ipAddress);
            IpPort = ipPort; // 1 -65 535
            LockObject = new object();
        }

        #endregion

        #region Public Methods

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

        #endregion

        /// <summary>
        ///     Initial connection
        /// </summary>
        public void InitConnection()
        {
            if (Socket == null)
            {
                Socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
                if (EndPoint == null)
                {
                    EndPoint = new IPEndPoint(IpAddress,IpPort);
                }

                var ping = new Ping();
                var pingReply = ping.Send(IpAddress, 5000);

                if (pingReply != null && pingReply.Status != IPStatus.Success)
                {
                    throw new Exception("Device not responding");
                }
                Socket.Connect(EndPoint);
            }
        }

        #region Private Methods

        /// <summary>
        ///     Processing of query send / request    
        /// </summary>
        /// <param name="command">String, contains command</param>
        /// <returns>
        ///     String contains the result in the case of 
        ///     a request with a response and null in the 
        ///     case of a request withput an answer
        /// </returns>
        private string QueryControl(string command)
        {
            try
            {
                RequestSending(command);
                return command.IndexOf("?", StringComparison.CurrentCulture) >= 0 ? ResponseReceive() : null;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                SocketClose();
            }
        }

        /// <summary>
        ///     Закрывает сокет
        /// </summary>
        private void SocketClose()
        {
            lock (LockObject)
            {
                if (Socket == null) return;
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close(1);
                Socket = null;
            }
        }

        /// <summary>
        ///     Send request of command
        /// </summary>
        /// <param name="command">String contains command</param>
        private void RequestSending(string command)
        {
            lock (LockObject)
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
            }
        }

        /// <summary>
        ///     Receive response after request
        /// </summary>
        /// <returns>String, contains result</returns>
        private string ResponseReceive()
        {
            lock (LockObject)
            {
                var buffer = new byte[1024];
                buffer = new byte[buffer.Length];

                int readBytes;
                do
                {
                    readBytes = Socket.Receive(buffer);
                } while (Socket.Available < 0);

                return Encoding.ASCII.GetString(buffer, 0, readBytes);
            }
        }

        #endregion
    }
}