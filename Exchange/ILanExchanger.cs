using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DevicesControlLibrary.Exchange
{
    public interface ILanExchanger
    {
        int SendWithRequestInt(string command);
        string SendWithRequestString(string command);
        void SendWithoutRequest(string command);
        double SendWithRequestDouble(string command);
    }
}