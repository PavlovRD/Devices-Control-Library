using System;
using System.Collections.Generic;
using System.Linq;
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
                    "Failed to initialize the exchange with the power supply over the LAN. Reason: " +
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
                throw new Exception("Failed to set clear status command. Reason:" + exception.Message);
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
                throw new Exception("Failed to get result standart event status enabled command. Reason:" +
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
                throw new Exception("Failed to set standart event status enabled command. Reason:" + exception.Message);
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
                throw new Exception("Failed to get result event status register command. Reason:" + exception.Message);
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
                throw new Exception("Failed to get result identification device command. Reason: " + exception.Message);
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
                throw new Exception("Failed to set enable operation complete bit in ESR command. Reason: " +
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

        #endregion
    }
}