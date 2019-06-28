using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using DevicesControlLibrary.Exchange;

namespace DevicesControlLibrary.Devices.PowerSupply
{
    /// <summary>
    ///     Work with Power Supply Agilent / Keysight N5746A
    /// </summary>
    public sealed class N5746A
    {
        #region Public Member Variables

        /// <summary>
        ///     Calibration level
        /// </summary>
        public enum CalibrationLevel
        {
            P1,
            P2
        }

        /// <summary>
        ///     Output Power-On State values
        /// </summary>
        public enum OutputPowerOnState
        {
            RST,
            AUTO
        }

        #endregion

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
        public N5746A(string ipAddress, int ipPort)
        {
            try
            {
                _lanExchanger = new LanExchanger(ipAddress, ipPort);
            }
            catch (Exception exception)
            {
                throw new Exception(
                    "Failed to initialize the exchange with the power supply over the LAN. Reason:  " +
                    exception.Message);
            }
        }

        #endregion

        #region Public methods

        #region Common commands

        /// <summary>
        ///     This command causes the following actions on the status system:
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///             Clears the Standard Event Status, Operation Status Event, 
        ///             and Questionable Status Event registers
        ///         </description>
        ///     </item>
        /// <item>
        ///         <description>
        ///             Clears the Status Byte and the Error Queue
        ///         </description>
        ///     </item>
        /// <item>
        ///         <description>
        ///             If *CLS immediately follows a program message terminator(NL), 
        ///             then the output queue and the MAV bit are also cleared.
        ///         </description>
        ///     </item>
        /// </list>
        /// </summary>
        public void SetClearStatus()
        {
            try
            {
                _lanExchanger.SendWithoutRequest("*CLS;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set clear status command. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     All of the enabled events of the Standard Event Status Event Register 
        ///     are logically OR-ed to cause the Event Summary Bit(ESB) of the Status
        ///     Byte Register to be set.The query reads the Standard Event The query
        ///     reads the Standard Event Status Enable register. The bit configuration of 
        ///     the Standard Event register is as <see cref="SetStandartEventStatusEnable(int)">follows</see>.
        /// </summary>
        /// <returns>Int value</returns>
        public int GetStandartEventStatus()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt("*ESE?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get result standart event status enabled command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command programs the Standard Event Status Enable register bits. 
        ///     The programming determines which events of the Standard Event Status 
        ///     Event register (see *ESR?) are allowed to set the ESB (Event Summary Bit)
        ///      of the Status Byte register. A "1" in the bit position enables the corresponding event. 
        /// </summary>
        /// <param name="nrf">register of activation of the standard event status</param>
        /// <remarks>
        /// <list type="bullet">
        ///     <item>
        ///         <description>Bit position 7. Bit value 128. Bit name - PON | Power-on has occured</description>
        ///     </item>
        ///     <item>
        ///         <description>Bit position 6. Undefined</description>
        ///     </item>
        ///     <item>
        ///         <description>Bit position 5. Bit value 32. Bit name - CME | Command error</description>
        ///     </item>
        ///     <item>
        ///         <description>Bit position 4. Bit value 16. Bit name - EXE | Execution error</description>
        ///     </item>
        ///     <item>
        ///         <description>Bit position 3. Bit value 8. Bit name - DDE | Device-dependent error</description>
        ///     </item>
        ///     <item>
        ///         <description>Bit position 1. Undefined</description>
        ///     </item>
        ///     <item>
        ///         <description>Bit position 0. Bit value 1. Bit name - OPC | Operation complete</description>
        ///     </item>
        /// </list>
        /// </remarks>
        public void SetStandartEventStatusEnable(int nrf)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("*ESE " + nrf + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set standart event status enabled command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This query reads the Standard Event Status Event register. 
        ///     Reading the register clears it. 
        ///     The bit configuration is the same as the Standard Event Status Enable register(see <see cref="SetStandartEventStatusEnable(int)">*ESE</see>).
        /// </summary>
        /// <returns>Return int value</returns>
        public int SetStandartEventStatusEnable()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt("*ESR?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get result event status register command. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     This query requests the power supply to identify itself.
        ///     It returns a string of four fields separated by commas.
        /// </summary>
        /// <returns>String, contains identification information</returns>
        public string GetIdentificationInformation()
        {
            try
            {
                return _lanExchanger.SendWithRequestString("*IDN?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get result identification device command. Reason:  " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command sets the OPC status bit when all pending operations have completed.
        ///     Since your program can read this status bit on an interrupt basis, *OPC allows
        ///      subsequent commands to be executed.
        /// </summary>
        public void SetOpcEnable()
        {
            try
            {
                _lanExchanger.SendWithoutRequest("*OPC;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set enable operation complete bit in ESR command. Reason:  " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command places a 1 in the Output Queue when all pending operations have completed.
        ///     Because it requires your program to read the returned value before executing the next 
        ///     program statement, *OPC? can be used to cause the controller to wait for commands to 
        ///     complete before proceeding with its program.
        /// </summary>
        /// <returns>Return true if all comands is complete. Otherwise - return false</returns>
        public bool GetOpc()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt("*OPC?;") == 1;
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get result operation complete command. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     This query requests the unit to identify any installed options. A 0 indicates no options are installed.
        /// </summary>
        /// <returns>Return value of option number</returns>
        /// <remarks>If return 0 - standart instrument, without option</remarks>
        public int GetOpt()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt("*OPT?;");
            }
            catch (Exception exception)
            {
                throw new Exception(
                    "Failed to get result option number installed command. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     This command restores the power supply to a state that was previously
        ///     stored in memory locations 0 through 15 with the <see cref="SetSaveState">*SAV</see> command.
        ///     Note that you can only recall a state from a location that contains a previously-stored state. 
        /// </summary>
        /// <param name="state">Value of memory location</param>
        /// <remarks>state parametr value of range 0 to 15. Attention! All saved instrument states are lost when the unit is turned off.</remarks>
        public void SetRecallSavedState(int state)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("*OPT?;");
            }
            catch (Exception exception)
            {
                throw new Exception(
                    "Failed to get result option number installed command. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     This command resets the power supply to a factory-defined state. 
        ///     This state is defined as follows.Note that *RST also forces an ABORt command. 
        ///     The *RST settings are as follows: 
        ///     <list type="bullet">
        ///     <item>
        ///         <description>CAL:STAT Off</description>
        ///     </item>
        ///     <item>
        ///         <description>INIT:CONT Off</description>
        ///     </item>
        ///     <item>
        ///         <description>OUTP Off</description>
        ///     </item>
        ///     <item>
        ///         <description>[SOUR:]CURR 0</description>
        ///     </item>
        ///     <item>
        ///         <description>[SOUR:]:CURR:TRIG 0</description>
        ///     </item>
        ///     <item>
        ///         <description>[SOUR:]CURR:PROT:STAT Off</description>
        ///     </item>
        ///     <item>
        ///         <description>[SOUR:]VOLT 0</description>
        ///     </item>
        ///     <item>
        ///         <description>[SOUR:]VOLT:LIM 0</description>
        ///     </item>
        ///     <item>
        ///         <description>[SOUR:]VOLT:TRIG 0</description>
        ///     </item>
        ///     <item>
        ///         <description>[SOUR:]VOLT:PROT MAX</description>
        ///     </item>
        ///     </list>
        /// </summary>
        public void Reset()
        {
            try
            {
                _lanExchanger.SendWithoutRequest("*RST;");
            }
            catch (Exception exception)
            {
                throw new Exception(
                    "Failed to reset command. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     This command stores the present state of the power supply to memory locations 0 through 15. 
        /// </summary>
        /// <param name="state">Value of state number</param>
        /// <remarks>All saved instrument states are lost when the unit is turned off.</remarks>
        public void SetSaveState(int state)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("*SAV " + state + ";");
            }
            catch (Exception exception)
            {
                throw new Exception(
                    "Failed to set saved state command. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     This command sets the condition of the Service Request Enable Register.
        ///     This register determines which bits from the Status Byte Register are
        ///     allowed to set the Master Status Summary(MSS) bit and the Request 
        ///     for Service(RQS) summary bit. 
        ///     A 1 in any Service Request Enable Register bit position enables the 
        ///     corresponding Status Byte Register bit and all such enabled bits then
        ///     are logically OR-ed to cause Bit 6 of the Status Byte Register to be set.
        /// </summary>
        /// <param name="nrf">Value of bit</param>
        public void SetServiceRequestEnableRegister(int nrf)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("*SRE " + nrf + ";");
            }
            catch (Exception exception)
            {
                throw new Exception(
                    "Failed to set service request enable register command. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     When the controller conducts a serial poll in response to SRQ, 
        ///     the RQS bit is cleared, but the MSS bit is not.
        ///     When *SRE is cleared (by programming it with 0), the power 
        ///     supply cannot generate an SRQ to the controller.
        ///     The query returns the current state of *SRE
        /// </summary>
        /// <returns>String value of SRE</returns>
        public string GetServiceRequestEnableRegister()
        {
            try
            {
                return _lanExchanger.SendWithRequestString("*SRE?;");
            }
            catch (Exception exception)
            {
                throw new Exception(
                    "Failed to get service request enable register command. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     This query reads the Status Byte register, which contains the status    
        ///     summary bits and the Output Queue MAV bit. 
        ///     Reading the Status Byte register does not clear it. 
        ///     The input summary bits are cleared when the appropriate event registers are read.
        ///     The MAV bit is cleared at poweron, by <see cref="SetClearStatus">*CLS</see> or
        ///     when there is no more response data available.
        ///     A serial poll also returns the value of the Status Byte register, 
        ///     except that bit 6 returns Request for Service(RQS) instead of Master Status Summary(MSS).
        ///     A serial poll clears RQS, but not MSS.When MSS is set, it indicates that the power supply 
        ///     has one or more reasons for requesting service.
        /// </summary>
        /// <returns>
        ///     String value of status byte:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>Bit position 7. Bit value 128. Bit name OPER | Operation status summary</description>
        ///         </item>
        ///         <item>
        ///             <description>Bit position 6. Bit value 64. Bit name MSS (RQS) | Master status summary (Request for service)</description>
        ///         </item>
        ///         <item>
        ///             <description>Bit position 5. Bit value 32. Bit name ESB | Event status byte summary</description>
        ///         </item>
        ///         <item>
        ///             <description>Bit position 4. Bit value 16. Bit name MAV | Message aviable</description>
        ///         </item>
        ///         <item>
        ///             <description>Bit position 3. Bit value 8. Bit name QUES | Questionable status summary</description>
        ///         </item>
        ///         <item>
        ///             <description>Bit position 2. Bit value 4 Bit name ERR | Error queue not empty</description>
        ///         </item>
        ///         <item>
        ///             <description>Bit position 1-0 undefined</description>
        ///         </item>
        ///     </list>
        /// </returns>
        public string GetStatusByte()
        {
            try
            {
                return _lanExchanger.SendWithRequestString("*STB?;");
            }
            catch (Exception exception)
            {
                throw new Exception(
                    "Failed to get status byte register value command. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     This command generates a trigger when the trigger source is set to BUS.
        ///     The command has the same affect as the Group Execute Trigger(GET) command.
        /// </summary>
        public void SetTrigger()
        {
            try
            {
                _lanExchanger.SendWithoutRequest("*TRG;");
            }
            catch (Exception exception)
            {
                throw new Exception(
                    "Failed to set trigger command. Reason: " + exception.Message);
            }
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
        ///     This command instructs the power supply not to process any further
        ///     commands until all pending operations are completed.
        ///     Pending operations are as defined under the *OPC command. 
        ///     *WAI can be aborted only by sending the power supply a Device Clear command.
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
        public void SetCalibrationLevel(CalibrationLevel calibrationLevel)
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

        #region Measure commands

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

        #endregion

        #region Output commands

        /// <summary>
        ///     This command enables or disables the specified output(s). 
        ///     The enabled state is On(1); the disabled state is Off(0). 
        ///     The state of a disabled output is a condition of zero output 
        ///     voltage and a zero source current(see <see cref="Reset">*RST</see>). 
        /// </summary>
        /// <param name="state">True - state is on, False - state is off</param>
        public void SetOutputState(bool state)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("OUTP: " + (state ? "ON" : "OFF") + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set output state of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     The query returns False if the output is off, and True if the output is on.
        /// </summary>
        /// <returns>True - state is on, False - state is off</returns>
        public bool GetOutputState()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt("OUTP:STAT?;") == 1;
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get output state of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command determines if the power-on state will be determined by
        ///     the reset state, or the settings the unit had when it was turned off.
        ///     RST programs the unit to the reset state; AUTO programs the unit to the
        ///     settings it had when it was turned off.The power-on state information is
        ///     saved on non-volatile memory.
        /// </summary>
        /// <param name="outputPonState">Value of output power-on state</param>
        public void SetOutputPowerOnState(OutputPowerOnState outputPonState)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("OUTP:PON:STAT " + outputPonState + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set output power on state of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     Returns output power-on state
        /// </summary>
        /// <returns><see cref="OutputPowerOnState"/></returns>
        public OutputPowerOnState GetOutputPowerOnState()
        {
            try
            {
                return (OutputPowerOnState) Enum.Parse(typeof(OutputPowerOnState),
                    _lanExchanger.SendWithRequestString("OUTP:PON:STAT?;"));
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get output power on state of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command clears the latched signals that have disabled the output.
        ///     The over-voltage and over-current conditions are always latching.
        ///     The over-temperature condition, AC-fail condition, Enable pins, and SO pins
        ///     are latching if OUTPut:PON:STATe is RST, and non-latching if OUTPut:PON:STATe is AUTO.
        ///     All conditions that generate the fault must be removed before the latch  can be cleared.
        ///     The output is then restored to the state it was in before the fault condition occurred.
        /// </summary>
        public void SetOutputProtectionClear()
        {
            try
            {
                _lanExchanger.SendWithRequestString("OUTP:PROT:CLE;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set output protection clear of command. Reason: " +
                                    exception.Message);
            }
        }

        #endregion

        #region Source commands

        #region Current commands

        /// <summary>
        ///     Setting the output current in current priority mode
        /// </summary>
        /// <param name="valueOfCurrent">Value of current</param>
        public void SetSourceCurrent(double valueOfCurrent)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("CURR " + valueOfCurrent + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set source current value in " + valueOfCurrent +
                                    "A of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     Getting the output currents
        /// </summary>
        public void GetSourceCurrent()
        {
            try
            {
                _lanExchanger.SendWithoutRequest("CURR?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get source current of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     Setting the power of the running output signal
        /// </summary>
        /// <param name="valueOfCurrTrig">Value of current</param>
        public void SetSourceTriggerCurrent(double valueOfCurrTrig)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("CURR:TRIG " + valueOfCurrTrig + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set source current trigger value in " + valueOfCurrTrig +
                                    " of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     Getting the power of the running output signal
        /// </summary>
        public void GetSourceTriggerCurrent()
        {
            try
            {
                _lanExchanger.SendWithoutRequest("CURR:TRIG?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get source current trigger of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     Allow or deny overcurrent protection.
        ///     Setting the state of over-current protection
        /// </summary>
        /// <param name="state">True - protection is on, False - protection is off</param>
        public void SetSourceProtectionCurrentState(bool state)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("CURR:PROT:STAT " + (state ? "ON" : "OFF") + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set state of over-current protection in value " + state +
                                    " of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     Getting the state of over-current protection - allow or deny
        /// </summary>
        /// <returns>True - is on, False - is off</returns>
        public bool GetSourceProtectionCurrentState()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt("CURR:PROT:STAT?;") == 1;
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get state of over-current protection of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     Set the output voltage when the voltage priority mode is active.
        /// </summary>
        /// <param name="valueOfVoltage">Value of voltage</param>
        public void SetSourceVoltage(double valueOfVoltage)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("VOLT " + valueOfVoltage + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set the output voltage of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     Get the output voltage when the voltage priority mode is active.
        /// </summary>
        /// <returns>Value of Volt</returns>
        public double GetSourceVoltage()
        {
            try
            {
                return _lanExchanger.SendWithRequestDouble("VOLT?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get the output voltage of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     Set the voltage of the running output signal.
        /// </summary>
        /// <param name="valueOfTriggeredVolt">Value of triggered volt</param>
        /// <returns>Value of triggered volt</returns>
        public void SetSourceVoltageTriggered(double valueOfTriggeredVolt)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("VOLT:TRIG " + valueOfTriggeredVolt + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set the voltage of the running output signal value of " +
                                    valueOfTriggeredVolt + " of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     Get the voltage of the running output signal.
        /// </summary>
        /// <returns>Value of triggered volt</returns>
        public double GetSourceVoltageTriggered()
        {
            try
            {
                return _lanExchanger.SendWithRequestDouble("VOLT:TRIG?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get the voltage of the running output signal of command. Reason: " +
                                    exception.Message);
            }
        }

        ///<summary>
        ///     This command sets the low voltage limit of the output.
        ///     When a low voltage limit has been set, the instrument will 
        ///     ignore any programming commands that attempt to set the
        ///     output voltage below the low voltage limit.
        /// </summary>
        /// <param name="lowValue">Value of low limit</param>
        public void SetSourceVoltageLimitLow(double lowValue)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("VOLT:LIM:LOW " + lowValue + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set the low voltage limit of the output value of command. Reason: " +
                                    exception.Message);
            }
        }

        ///<summary>
        ///     This command gets the low voltage limit of the output.
        /// </summary>
        /// <returns>Value of low limit</returns>
        public double GetSourceVoltageLimitLow()
        {
            try
            {
                return _lanExchanger.SendWithRequestDouble("VOLT:LIM:LOW?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get the low voltage limit of the output of command. Reason: " +
                                    exception.Message);
            }
        }
        
        ///<summary>
        ///     This command sets the over-voltage protection (OVP) level of the output. 
        ///     The values are programmed in volts.If the output voltage exceeds the OVP 
        ///     level, the output is disabled and OV is set in the Questionable Condition 
        ///     status register.
        /// </summary>
        /// <returns>Value of voltage protection</returns>
        public double GetSourceVoltageProtectionLevel()
        {
            try
            {
                return _lanExchanger.SendWithRequestDouble("VOLT:PROT:LEV?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get the value of voltage protection of command. Reason: " +
                                    exception.Message);
            }
        }

        ///<summary>
        ///     This command gets the low voltage limit of the output.
        /// </summary>
        /// <param name="valueOfVoltageProtection">Value of voltage protection</param>
        public void SetSourceVoltageProtectionLevel(double valueOfVoltageProtection)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("VOLT:PROT:LEV " + valueOfVoltageProtection + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set the value of voltage protection of command. Reason: " +
                                    exception.Message);
            }
        }

        #endregion

        #endregion

        #endregion
    }
}