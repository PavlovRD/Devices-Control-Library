using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DevicesControlLibrary.Devices.PowerSupply;

namespace DevicesControlLibrary
{
    public class DevicesManager
    {
        #region Public Properties

        /// <summary>
        ///     Instance of Power Supply
        /// </summary>
        public APowerSupply PowerSupply { get; private set; }

        #endregion

        #region Public Enums

        /// <summary>
        ///     Enum for power supply devices
        /// </summary>
        public enum PowerSupplyCollection
        {
            N5746A
        }

        /// <summary>
        ///     Enum for signal generator devices
        /// </summary>
        public enum SignalGeneratorCollection
        {
            SMB100A
        }

        #endregion
        
        #region Constructors

        /// <summary>
        ///     Creating power supply
        /// </summary>
        /// <param name="powerSupply">Device from <see cref="PowerSupplyCollection"/> enum</param>
        /// <param name="ipAddress">Ip address for power supply</param>
        /// <param name="ipPort">Ip port for power supply</param>
        public DevicesManager(PowerSupplyCollection powerSupply, string ipAddress, int ipPort)
        {
            switch (powerSupply)
            {
                case PowerSupplyCollection.N5746A:
                {
                    //TODO: Set power supply
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        /// <summary>
        ///     Creating signal generator
        /// </summary>
        /// <param name="signalGenerator">Device from <see cref="SignalGeneratorCollection"/> enum</param>
        public DevicesManager(SignalGeneratorCollection signalGenerator)
        {
            switch (signalGenerator)
            {
                case SignalGeneratorCollection.SMB100A:
                {
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        #endregion
    }
}