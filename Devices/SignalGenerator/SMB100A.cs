using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevicesControlLibrary.Exchange;

namespace DevicesControlLibrary.Devices.SignalGenerator
{
    /// <summary>
    ///     Work with Signal Generator R&S SMB100A
    /// </summary>
    public sealed class Smb100A
    {
        #region Private Members Variable

        private readonly LanExchanger _lanExchanger;

        #endregion

        #region Constructor

        /// <summary>
        ///     Constructor.
        ///     Creating LAN connected to device
        /// </summary>
        /// <param name="ipAddress">Device Ip-address</param>
        /// <param name="ipPort">Deivce Ip-port</param>
        public Smb100A(string ipAddress, int ipPort)
        {
            try
            {
                _lanExchanger = new LanExchanger(ipAddress, ipPort);
            }
            catch (Exception exception)
            {
                throw new Exception(
                    "Failed to initialize the exchange with the signal generator over the LAN. Reason: " +
                    exception.Message);
            }
        }

        #endregion

        #region Public Methods

        #region Common commands

        /// <summary>
        ///     Identification device
        /// </summary>
        /// <returns></returns>
        public string Identification()
        {
            try
            {
                return _lanExchanger.SendWithRequestString("*IDN?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Cloud not process identification device command: " + exception.Message);
            }
        }

        /// <summary>
        ///     Operation Complete
        /// </summary>
        /// <returns>Return true if all comands is complete. Otherwise - return false</returns>
        public bool Opc()
        {
            try
            {
                return _lanExchanger.SendWithRequestBool("*OPC?;") == 1;
            }
            catch (Exception exception)
            {
                throw new Exception("Could not process OPC command: " + exception.Message);
            }
        }

        /// <summary>
        ///     Reset device to default
        /// </summary>
        public void Reset()
        {
            _lanExchanger.SendWithoutRequest("*RST;");
        }

        /// <summary>
        ///     Self-Test query
        ///     TODO: ErrorCode translate
        /// </summary>
        public bool Test()
        {
            try
            {
                return _lanExchanger.SendWithRequestString("*TST?;").Equals("0");
            }
            catch (Exception exception)
            {
                throw new Exception("Could not process test command: " + exception.Message);
            }
        }

        /// <summary>
        ///     Wait to continue
        /// </summary>
        public void Wait()
        {
            try
            {
                _lanExchanger.SendWithoutRequest("*WAI;");
            }
            catch (Exception exception)
            {
                throw new Exception("Could not process wait command: " + exception.Message);
            }
        }

        #endregion

        #endregion
    }
}