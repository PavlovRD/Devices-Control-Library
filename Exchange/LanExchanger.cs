using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevicesControlLibrary.Exchange
{
    public class LanExchanger
    {
        public LanExchanger(string ipAddress, int ipPort) {
            if (string.IsNullOrEmpty(ipAddress) || string.IsNullOrWhiteSpace(ipAddress))
            {
                throw new Exception("Ip-адрес не должен быть пустым");
            }
        }

        public int SendWithRequestInt(string command)
        {
            return 0;
        }

        public string SendWithRequestString(string command)
        {
            return "";
        }

        public void SendWithoutRequest(string command)
        {

        }

        public double SendWithRequestDouble(string command)
        {
            return 0.0;
        }
    }
}
