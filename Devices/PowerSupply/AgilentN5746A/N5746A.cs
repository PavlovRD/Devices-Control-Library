using System;
using DevicesControlLibrary.Devices.PowerSupply.Subsystems.Calibration;
using DevicesControlLibrary.Devices.PowerSupply.Subsystems.Common;
using DevicesControlLibrary.Exchange;

namespace DevicesControlLibrary.Devices.PowerSupply.AgilentN5746A
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
            Rst,
            Auto
        }

        public enum CommunicateRlstate
        {
            /// <summary>
            ///     The instrument is set to front panel control (front panel keys are active).
            /// </summary>
            Loc,

            /// <summary>
            ///     The instrument is set to remote interface control (front panel keys are active).
            /// </summary>
            Rem,

            /// <summary>
            ///     The front panel keys are disabled (the instrument can only be controlled via the remote interface).
            /// </summary>
            Rwl
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

        #endregion

        #region Voltage commands

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

        #region Status commands

        /// <summary>
        ///     This command sets all defined bits in the Operation and Questionable PTR registers. 
        ///     The command clears all defined bits in the Operation and Questionable NTR and Enable registers.
        /// </summary>
        public void SetStatusPreset()
        {
            try
            {
                _lanExchanger.SendWithoutRequest("STAT:PRES;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set status preset of command. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     This query returns the value of the Operation Event register.
        ///     The Event register is a read-only register, which stores (latches) 
        ///     all events that are passed by the Operation NTR and/or PTR filter. 
        ///     Reading the Operation Event register clears it. The bit configuration
        ///      of the Operation status registers is as follows:
        ///     <list type="bullet">
        ///         <item>
        ///         <description>Bit position: 15-11. Bit value: undefined. Bit name: undefined.</description>
        ///         </item>
        ///         <item>
        ///         <description>Bit position: 10. Bit value: 1024. Bit name: CC | The output is in constant current.</description>
        ///         </item>
        ///         <item>
        ///         <description>Bit position: 9. Bit value: undefined. Bit name: undefined.</description>
        ///         </item>
        ///         <item>
        ///         <description>Bit position: 8. Bit value: 256. Bit name: CV | The output is in constant voltage.</description>
        ///         </item>
        ///         <item>
        ///         <description>Bit position: 7-6. Bit value: undefined. Bit name: undefined.</description>
        ///         </item>
        ///         <item>
        ///         <description>Bit position: 5. Bit value: 32. Bit name: WTG | The unit is waiting for a transinet trigger.</description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <returns>String contains value</returns>
        public string GetStatusOperationEvent()
        {
            try
            {
                return _lanExchanger.SendWithRequestString("STAT:OPER?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get status operation event of command. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     This query returns the value of the Operation Condition register. 
        ///     That is a read-only register, which holds the live(unlatched)
        ///     operational status of the power supply.
        /// </summary>
        /// <returns>String contains of value</returns>
        public string GetStatusOperationCondition()
        {
            try
            {
                return _lanExchanger.SendWithRequestString("STAT:OPER:COND?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get status operation condition of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     Sets the value of the Operational Enable register.
        ///     This register is a mask for enabling specific bits from the Operation Event register 
        ///     to set the operation summary bit (OPER) of the Status Byte register. 
        ///     This bit (bit 7) is the logical OR of all the Operational Event register bits that are
        ///     enabled by the Status  operation Enable register. The Preset value = 0.
        /// </summary>
        /// <param name="valueOfOperationEnabled">Value of operation enabled</param>
        public void SetStatusOperationEnabled(int valueOfOperationEnabled)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("STAT:OPER:ENAB " + valueOfOperationEnabled + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set status operation enabled of value " + valueOfOperationEnabled +
                                    " of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     Gets read the valiw of the Operational Event register
        /// </summary>
        /// <returns>String value</returns>
        public string GetStatusOperationEnabled()
        {
            try
            {
                return _lanExchanger.SendWithRequestString("STAT:OPER:ENAB?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get status operation enabled of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command set the value of the Operation NTR (NegativeTransition) register.
        ///     When a bit in the Operation NTR register is set to 1, then a 1-to-0 
        ///     transition of the corresponding bit in the Operation Condition 
        ///     register causes that bit in the Operation Event register to be set.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///             If the same bits in both NTR and PTR registers are set to 1, then any
        ///             transition of that bit at the Operation Condition register sets the
        ///             corresponding bit in the Operation Event register.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             If the same bits in both NTR and PTR registers are set to 0, then no 
        ///             transition of that bit at the Operation Condition register can set the
        ///             corresponding bit in the Operation Event register.
        ///         </description>
        ///     </item>
        /// </list>
        ///     The Preset value are: NTR = 0; PTR = 32767
        /// </remarks>
        /// <param name="ntrValue">The contains the number of all registers in the registers.</param>
        public void SetStatusOperationNtr(int ntrValue)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("STAT:OPER:NTR " + ntrValue + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set status operation NTR of command. Reason :" + exception.Message);
            }
        }

        /// <summary>
        ///     Get value of the Operation NTR (NegativeTransition) value.
        /// </summary>
        /// <returns>The key message contains the binary sum of all bits set in the registers.</returns>
        public int GetStatusOperationNtr()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt("STAT:OPER:NTR?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get status operation NTR of command. Reason :" + exception.Message);
            }
        }

        /// <summary>
        ///     This command set the value of the Operation PTR (PositiveTransition) register.
        ///     When a bit of the Operation PTR register is set to 1, then a 0-to-1
        ///     transition of the corresponding bit in the Operation Condition
        ///     register causes that bit in the Operation Event register to be set.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///             If the same bits in both NTR and PTR registers are set to 1, then any
        ///             transition of that bit at the Operation Condition register sets the
        ///             corresponding bit in the Operation Event register.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             If the same bits in both NTR and PTR registers are set to 0, then no 
        ///             transition of that bit at the Operation Condition register can set the
        ///             corresponding bit in the Operation Event register.
        ///         </description>
        ///     </item>
        /// </list>
        ///     The Preset value are: NTR = 0; PTR = 32767
        /// </remarks>
        /// <param name="ptrValue">The contains the number of all registers in the registers.</param>
        public void SetStatusOperationPtr(int ptrValue)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("STAT:OPER:PTR " + ptrValue + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set status operation PTR of command. Reason :" + exception.Message);
            }
        }

        /// <summary>
        ///     Get value of the Operation PTR (PositiveTransition) value.
        /// </summary>
        /// <returns>The key message contains the number of all registers in the registers.</returns>
        public int GetStatusOperationPtr()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt("STAT:OPER:PTR?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get status operation PTR of command. Reason :" + exception.Message);
            }
        }

        /// <summary>
        ///     This query returns the value of the Questionable Event register. 
        ///     The event register is a read-only register, which stores (latches) 
        ///     all events that are passed by the Questionable NTR and / or PTR filter.
        ///     Reading the Questionable Event register clears it.
        ///     The bit configuration of the Questionable status registers is as follows:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 Bit position: 15-11. Bit Value: undefined. Bit Name: undefined.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Bit position: 10. Bit Value: 1024. Bit Name: UNR | The output is unregulated.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Bit position: 9. Bit Value: 512. Bit Name: INH | The output is turned off by one of the external J1 inhibit signals.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Bit position: 8-5. Bit Value: Undefined. Bit Name: Undefined.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Bit position: 4. Bit Value: 16. Bit Name: OT | The output is disabled by the over-temperature protection.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Bit position: 3. Bit Value: Undefined. Bit Name: Undefined.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Bit position: 2. Bit Value: 4. Bit Name: PF | The output is disabled because AC power has failed.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Bit position: 1. Bit Value: 2. Bit Name: OC | The output is disabled by the over-current protection
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 Bit position: 0. Bit Value: 1. Bit Name: OV | The output is disabled by the over-voltage protection.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <returns>The responsible message contains the binary sum of all bits set in the recorder.</returns>
        public int GetStatusQuestionable()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt("STAT:QUES?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get status questionable of command. Reason :" + exception.Message);
            }
        }

        /// <summary>
        ///     This query returns the value of the Questionable Condition register.
        ///     That is a read-only register, which holds the real-time (unlatched)
        ///     questionable status of the power supply.
        /// </summary>
        /// <returns>The responsible message contains the binary sum of all bits set in the recorder.</returns>
        public int GetStatusQuestionableCondition()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt("STAT:QUES:COND?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get status questionable condition of command. Reason :" +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command read the value of the Questionable Enable register. 
        ///     This register is a mask for enabling specific bits from the Questionable Event register 
        ///     to set the questionable summary bit (QUES) of the Status Byte register. 
        ///     This bit (bit 3) is the logical OR of all the Questionable Event register bits that are
        ///     enabled by the Questionable Status Enable register.
        ///     The Preset value = 0.
        /// </summary>
        /// <returns>The responsible message contains the binary sum of all bits set in the recorder.</returns>
        public int GetStatusQuestionableEnable()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt("STAT:QUES:ENAB?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get status questionable enable of command. Reason :" +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command set the value of the Questionable Enable register.
        ///     This register is a mask for enabling specific bits from the Questionable Event register 
        ///     to set the questionable summary bit (QUES) of the Status Byte register. 
        ///     This bit (bit 3) is the logical OR of all the Questionable Event register bits that are
        ///     enabled by the Questionable Status Enable register.
        ///     The Preset value = 0.
        /// </summary>
        /// <param name="valueOfEnable">The responsible message contains the binary sum of all bits set in the recorder</param>
        public void SetStatusQuestionableEnable(int valueOfEnable)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("STAT:QUES:ENAB " + valueOfEnable + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set status questionable enable in value " + valueOfEnable +
                                    " of command. Reason :" +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command set the value of the Questionable NTR (Negative-Transition) register.
        ///     These registers serve as polarity filters between the Questionable Condition 
        ///     and Questionable Event registers to cause the following action:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 When a bit of the Questionable NTR register is set to 1, then a 1-to-0 transition
        ///                 of the corresponding bit of the Questionable Condition register causes that bit 
        ///                 in the Questionable Event register to be set.
        ///             </description>
        ///         </item>       
        ///         <item>
        ///             <description>
        ///                 If the same bits in both NTR and PTR registers are set to 1, then any transition 
        ///                 of that bit at the Questionable Condition register sets the corresponding bit in the 
        ///                 Questionable Event register.
        ///             </description>
        ///         </item>        
        ///         <item>
        ///             <description>
        ///                 If the same bits in both NTR and PTR registers are set to 0, then no
        ///                 transition of that bit at the Questionable Condition register can set
        ///                 the corresponding bit in the Questionable Event register.
        ///             </description>
        ///         </item>
        ///     </list>
        ///     The Preset values are: NTR = 0.
        /// </summary>
        /// <param name="valueOfNtr">Decimal value corresponding to the binary sum of the register bits.</param>
        public void SetStatusQuestionableNtr(int valueOfNtr)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("STAT:QUES:NTR " + valueOfNtr + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set status questionable NTR in value " + valueOfNtr +
                                    " of command. Reason :" +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command set the value of the Questionable PTR (Positive-Transition) register.
        ///     These registers serve as polarity filters between the Questionable Condition 
        ///     and Questionable Event registers to cause the following action:
        ///     <list type = "bullet" >
        ///         <item>
        ///             <description>
        ///                 When a bit of the Questionable PTR register is set to 1, then a 0-to-1 transition
        ///                 of the corresponding bit in the Questionable Condition register causes that bit 
        ///                 in the Questionable Event register to be set.
        ///             </description>
        ///         </item>       
        ///         <item>
        ///             <description>
        ///                 If the same bits in both NTR and PTR registers are set to 1, then any transition 
        ///                 of that bit at the Questionable Condition register sets the corresponding bit in the 
        ///                 Questionable Event register.
        ///             </description>
        ///         </item>        
        ///         <item>
        ///             <description>
        ///                 If the same bits in both NTR and PTR registers are set to 0, then no
        ///                 transition of that bit at the Questionable Condition register can set
        ///                 the corresponding bit in the Questionable Event register.
        ///             </description>
        ///         </item>
        ///     </list>
        ///     The Preset values are: PTR = 32767.
        /// </summary>
        /// <param name="valueOfPtr">Decimal value corresponding to the binary sum of the register bits.</param>
        public void SetStatusQuestionablePtr(int valueOfPtr)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("STAT:QUES:PTR  " + valueOfPtr + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set status questionable ptr in value " + valueOfPtr +
                                    " of command. Reason :" +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command get the value of the Questionable NTR (Negative-Transition) register.
        /// </summary>
        /// <returns>Decimal value corresponding to the binary sum of the register bits.</returns>
        public int GetStatusQuestionableNtr()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt("STAT:QUES:NTR?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get status questionable ntr of command. Reason :" +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command get the value of the Questionable PTR (Positive-Transition) register.
        /// </summary>
        /// <returns>Decimal value corresponding to the binary sum of the register bits.</returns>
        public int GetStatusQuestionablePtr()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt("STAT:QUES:PTR?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set status questionable ptr of command. Reason :" +
                                    exception.Message);
            }
        }

        #endregion

        #region System commands

        /// <summary>
        ///     This command configures the remote/local state of the instrument
        ///     according to the following settings in <see cref="CommunicateRlstate"/>.
        /// </summary>
        public void SetSystemCommunicateRlstate(CommunicateRlstate communicateRlstate)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("SYST:COMM:RLST " + communicateRlstate + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set system communicate rlstate to value " + communicateRlstate +
                                    " of command. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     This command return remote/local state of the instrument
        /// </summary>
        /// <returns></returns>
        public CommunicateRlstate GetSystemCommunicateRlstate()
        {
            try
            {
                var response = _lanExchanger.SendWithRequestString("SYST:COMM:RLST?;");
                switch (response)
                {
                    case "LOC":
                    {
                        return CommunicateRlstate.Loc;
                    }
                    case "REM":
                    {
                        return CommunicateRlstate.Rem;
                    }
                    case "RWL":
                    {
                        return CommunicateRlstate.Rwl;
                    }
                    default:
                    {
                        throw new Exception("Undefined returned state: " + response);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get remote/local state of the instrument of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This query returns the control connection port number. 
        ///     This is used to open a control socket connection to the instrument.
        /// </summary>
        /// <returns>Integer value of port number</returns>
        public int GetSystemCommunicateControlConnectionPortNumber()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt("SYST:COMM:TCP:CONT?");
            }
            catch (Exception exception)
            {
                throw new Exception(
                    "Failed to get system communicate control connection port number of command. Reason: " +
                    exception.Message);
            }
        }

        /// <summary>
        ///     This query returns the next error number and its corresponding message 
        ///     string from the error queue.
        ///     The queue is a FIFO(first-in, first-out) buffer that stores errors as they occur.
        ///     As it is read, each error is removed from the queue. 
        ///     When all errors have been read, the query returns 0, NO ERROR. 
        ///     If more errors are accumulated than the queue can hold, the last error in the queue 
        ///     will be -350, TOO MANY ERRORS (see Appendix C for error codes)
        /// </summary>
        /// <returns>String, contains error code in format {code error},{error string}</returns>
        public string GetSystemError()
        {
            try
            {
                return _lanExchanger.SendWithRequestString("SYST:ERR?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get system error of command. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     This query returns the SCPI version number to which the instrument complies.
        ///     The returned value is of the form YYYY.V, where YYYY represents the year 
        ///     and V is the revision number for that year.
        /// </summary>
        /// <returns>String contains version information</returns>
        public string GetSystemVersion()
        {
            try
            {
                return _lanExchanger.SendWithRequestString("SYST:VERS?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get system version of command. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     This command restores the power supply to a state that was previously
        ///     stored in memory locations 0 through 15 with the *SAV command.
        ///     Note that you can only recall a state from a location that contains a previously-stored state. 
        /// </summary>
        /// <param name="stateNumber">Value of range 0-15</param>
        public void SetSystemRclState(int stateNumber)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("*RCL " + stateNumber + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set system rcl in value " + stateNumber + " of command. Reason: " +
                                    exception.Message);
            }
        }

        #endregion

        #endregion
    }
}