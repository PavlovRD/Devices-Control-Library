using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevicesControlLibrary.Exchange;

namespace DevicesControlLibrary.Devices.PowerSupply.Subsystems.Measure
{
    public class SubsystemMeasure : ISubsystemMeasure
    {
        private readonly ILanExchanger _lanExchanger;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="lanExchanger">Interface lan exchanger</param>
        public SubsystemMeasure(ILanExchanger lanExchanger)
        {
            _lanExchanger = lanExchanger;
        }

        /// <summary>
        ///     Returns current consumption in amperes
        /// </summary>
        /// <return>Value of current in ampers</return>
        public double GetMeasureCurrent()
        {
            try
            {
                return _lanExchanger.SendWithRequestDouble("MEAS:CURR?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get measurement current in amperes value of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     Returns output voltage in volts
        /// </summary>
        /// <return>Value of output voltage in volts</return>
        public double GetMeasureVolt()
        {
            try
            {
                return _lanExchanger.SendWithRequestDouble("MEAS:VOLT?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get measurement output voltage in volt value of command. Reason: " +
                                    exception.Message);
            }
        }
    }
}