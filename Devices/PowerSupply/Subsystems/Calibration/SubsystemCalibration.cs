using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevicesControlLibrary.Devices.PowerSupply.AgilentN5746A;
using DevicesControlLibrary.Exchange;

namespace DevicesControlLibrary.Devices.PowerSupply.Subsystems.Calibration
{
    public class SubsystemCalibration : ISubsystemCalibration
    {
        private readonly ILanExchanger _lanExchanger;

        public SubsystemCalibration(ILanExchanger lanExchanger)
        {
            _lanExchanger = lanExchanger;
        }

        #region Calibration comands

        /// <summary>
        ///     This command initiates the calibration of the output current. 
        /// </summary>
        /// <param name="maximumValue">The maximum current in the range of values for the output signal specified during calibration</param>
        public void SetCalibrationOutputCurrent(double maximumValue)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("CAL:CURR " + maximumValue + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set calibration in value of " + maximumValue + "A command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command enters a calibration value that you obtain by reading an external meter.
        ///     You must first select a calibration level (with CALibrate:LEVel) for the value being entered.
        ///     Data values are entered in either volts or amperes, depending on which function is being calibrated.
        /// </summary>
        /// <param name="dataValue">Value of calibration</param>
        public void SetCalibrationData(double dataValue)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("CAL:DATA " + dataValue + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set calibration data in value of " + dataValue + " command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command stores the date the unit was last calibrated. 
        ///     The data must be of the numeric format “yyyy/mm/dd” 
        ///     where yyyy indicates the year, mm indicates the month,
        ///     and dd indicates the day.    
        /// </summary>
        /// <param name="date">Value of date in format yyyy/mm/dd</param>
        public void SetCalibrationDate(string date)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("CAL:DATE " + date + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set calibration date in value of " + date + " command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     The query returns the date.
        /// </summary>
        /// <returns>String, contains date in format yyyy/mm/dd</returns>
        public string GetCalibrationDate()
        {
            try
            {
                return _lanExchanger.SendWithRequestString("CAL:DATE?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get calibration date value of command. Reason: " +
                                    exception.Message);
            }
        }


        /// <summary>
        ///     This command selects the next point in the calibration sequence.
        /// </summary>
        /// <param name="calibrationLevel">Value of calibration level:
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///             P1 is the first calibration point
        ///     </description>
        ///     </item>
        /// 
        ///     <item>
        ///         <description>
        ///             P2 is the second calibration point
        ///     </description>
        ///     </item>
        /// </list>
        /// </param>
        public void SetCalibrationLevel(N5746A.CalibrationLevel calibrationLevel)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("CAL:LEV " + calibrationLevel + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set calibration level value of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///      This command returns the next point in the calibration sequence.
        /// </summary>
        /// <returns>Value of calibration level:
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///             P1 is the first calibration point
        ///     </description>
        ///     </item>
        /// 
        ///     <item>
        ///         <description>
        ///             P2 is the second calibration point
        ///     </description>
        ///     </item>
        /// </list></returns>
        public string GetCalibrationLevel()
        {
            try
            {
                return _lanExchanger.SendWithRequestString("CAL:LEV?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get calibration level value of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command lets you change the calibration password.
        ///     A new password is automatically stored in nonvolatile memory.
        ///     If the password is set to 0, password protection is removed and 
        ///     the ability to enter calibration mode is unrestricted.The default
        ///     password is 0 (zero).
        /// </summary>
        /// <param name="password">Password can contains only integer value</param>
        public void SetCalibrationPassword(string password)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("CAL:PASS " + password + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set calibration password value of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command enables/disables calibration mode. 
        ///     Calibration mode must be enabled for the power supply 
        ///     to accept any other calibration commands.
        ///     The first parameter specifies the enabled or disabled state On(1) or Off(0).
        ///     The second parameter is the password.
        /// </summary>
        /// <param name="state">True - state is on, False - state is off</param>
        /// <param name="password">Password value can contains only int values. Optional</param>
        public void SetCalibrationState(bool state, string password = null)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("CAL:STAT " +
                                                 (state
                                                     ? (password != null ? "ON [," + password + "]" : "ON")
                                                     : "OFF") + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set state calibration in value of " +
                                    (state ? (password != null ? "ON [," + password + "]" : "ON") : "OFF") +
                                    " command. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     The query returns only the state, not the password.
        /// </summary>
        /// <returns>True - state is on, False - state is off</returns>
        public bool GetCalibrationState()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt("CAL:STAT?;") == 1;
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get state calibration value of command. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     This command initiates the calibration of the output voltage. 
        /// </summary>
        /// <param name="voltageValueOfCalibration">Value of voltage</param>
        public void SetCalibrationVoltage(double voltageValueOfCalibration)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("CAL:VOLT " + voltageValueOfCalibration + ";");
            }
            catch (Exception exception)
            {
                throw new Exception(
                    "Failed to set initiates the calibration of the output voltage value of command. Reason: " +
                    exception.Message);
            }
        }

        #endregion
    }
}