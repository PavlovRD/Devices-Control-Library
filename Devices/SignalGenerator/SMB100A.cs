using System;
using System.Collections.Generic;
using System.Globalization;
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
        #region Public Variables

        /// <summary>
        ///     Enum for frequncy modulation deviation
        /// </summary>
        public enum FrequencyDeviationType
        {
            Hz = 0,
            kHz = 1,
            mHz = 2
        }

        public enum FrequencyMode
        {
            NORM,
            LNO,
            HDEV
        }

        public enum SourceType
        {
            INT,
            EXT,
            INT_EXT
        }

        public enum Frequency
        {
            HZ,
            KHZ,
            MHZ,
            GHZ
        }

        public enum FREQuencyMode
        {
            CW,
            FIX,
            SWE
        }

        public enum FrequencyStepMode
        {
            DEC,
            USER
        }

        public enum PowMode
        {
            DBM,
            DBuV,
            nv,
            uv,
            mv,
            V
        }

        public enum PulsePolarity
        {
            NORM,
            INV
        }

        public enum PulseModulationTriggerMode
        {
            /// <summary>
            ///     The pulse modulation is generated continuously.
            /// </summary>
            AUTO,

            /// <summary>
            ///     The pulse modulation is triggered by an external trigger event.
            ///     The trigger signal is supplied via the PULSE EXT connector.
            /// </summary>
            EXT,

            /// <summary>
            ///     The pulse modulation is gated by an external gate signal. The
            ///     signal is supplied via the PULSE EXT connector.
            /// </summary>
            EGAT,

            /// <summary>
            ///     Pulse modulation is generated once.
            /// </summary>
            SINGl
        }

        public enum PulseModulationTriggerExternalImpedance
        {
            G50,
            G10K
        }

        public enum PulseModulationTriggerExternalSlope
        {
            NEG,
            POS
        }

        public enum PulseModulationTriggerExternalGatePolarity
        {
            NORM,
            INV
        }

        public enum PulseModulationSource
        {
            /// <summary>
            ///     The internally generated rectangular signal is used for the pulse
            ///     modulation. The frequency of the internal signal can be set in the SOURce:LFOutput subsystem.
            /// </summary>
            INT,

            /// <summary>
            /// The signal applied externally via the EXT MOD connector is used for the pulse modulation.
            /// </summary>
            EXT
        }

        public enum PulseModulationMode
        {
            /// <summary>
            ///     Enables single pulse generation.
            /// </summary>
            SING,

            /// <summary>
            ///     Enables double pulse generation. The two pulses are generated in one pulse period.
            /// </summary>
            DOUB,

            /// <summary>
            ///     A user-defined pulse train is generated The pulse train is defined
            ///     by value pairs of on and off times that can be entered in a pulsetrain list.
            /// </summary>
            PTR
        }

        public enum PulsePeriodType
        {
            s,
            ns,
            ms,
            us
        }

        public enum PulseWidthType
        {
            s,
            ns,
            ms,
            us
        }

        public enum PulseDelayType
        {
            s,
            ns,
            ms,
            us
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
                return _lanExchanger.SendWithRequestInt("*OPC?;") == 1;
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

        #region Source commands

        #region Base commands

        /// <summary>
        ///     Sets control state for all modulation in devices
        /// </summary>
        /// <param name="state">True - modulation is on, false - modulation is off</param>
        public void ModulationStateControl(bool state)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":SOUR:MOD:ALL:STAT " + (state ? "ON" : "OFF") + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set state control all modulation in " + (state ? "ON" : "OFF") +
                                    ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Gets control state for modulation
        /// </summary>
        /// <returns>True - modulation is on, False - modulation is off</returns>
        public bool ModulationGetStateControl()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt(":SOUR:MOD:STAT?;") == 1;
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get state modulation" + ". Reason: " + exception.Message);
            }
        }

        #endregion

        #region Amplitude Modulation

        /// <summary>
        ///     Control state for aplitude modulation in devices
        /// </summary>
        /// <param name="state">True - modulation is on, false - modulation is off</param>
        public void AmplitudeModulationStateControl(bool state)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":SOUR:AM:STAT " + (state ? "ON" : "OFF") + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set amplitude modulation state in value of " + (state ? "ON" : "OFF") +
                                    ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Get amplitude modulation state value
        /// </summary>
        public bool AmplitudeModulationGetState()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt(":AM:STAT?;") == 1;
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get amplitude modulation state value. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Selects the coupling mode for the external amplitude modulation signal.
        ///     Default AC mode
        /// </summary>
        /// <param name="type">True - AC mode, False - DC mode</param>
        public void AmplitudeModulationCouplingMode(bool type)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":AM:EXT:COUP " + (type ? "AC" : "DC") + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set amplitude modulation coupling " + (type ? "AC" : "DC") +
                                    ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Selects the modulation signal source for amplitude modulation.
        /// </summary>
        /// <param name="type">True - internal source, False - external source</param>
        public void AmplitudeModulationSelectSource(SourceType sourceType)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":AM:SOUR " + (sourceType == SourceType.INT_EXT
                                                     ? sourceType.ToString().Replace("_", ",")
                                                     : sourceType.ToString()) + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set amplitude modulation signal source type " +
                                    (sourceType == SourceType.INT_EXT
                                        ? sourceType.ToString().Replace("_", ",")
                                        : sourceType.ToString()) +
                                    ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Selects exponential or linear amplitude modulation.
        /// </summary>
        /// <param name="type">True - linear amplitude modulation, False - exponential amplitude modulation</param>
        public void AmplitudeModulationSelectAmType(bool type)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":AM:TYPE " + (type ? "LIN" : "EXP") + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set amplitude modulation select type " + (type ? "LIN" : "EXP") +
                                    ". Reason: " + exception.Message);
            }
        }

        #endregion

        #region Frequency Modulation

        /// <summary>
        ///     Control state for frequency modulation in devices
        /// </summary>
        /// <param name="state">True - modulation is on, false - modulation is off</param>
        public void FrequencyModulationStateControl(bool state)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":SOUR:FM:STAT " + (state ? "ON" : "OFF") + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set state frequency modulation in value of " + (state ? "ON" : "OFF") +
                                    ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Get frequncy modulation state value
        /// </summary>
        /// <returns>True - modulation is on, False - modulation is off</returns>
        public bool FrequencyModulationGetState()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt(":FM:STAT?;") == 1;
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get frequncy modulation state value. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Sets the deviation value in frequency modulation
        /// </summary>
        /// <param name="deviationValue">Value of deviation</param>
        /// <param name="type">Type of value - Hz, kHz or mHz</param>
        public void FrequencyModulationSetDeviation(double deviationValue, FrequencyDeviationType type)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":FM:DEV " + deviationValue + " " + type + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set signal deviation value to " + deviationValue + " " +
                                    type + ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Gets the set deviation value in frequency modulation
        /// </summary>
        /// <returns>Value of frequency modulation deviation</returns>
        public double FrequencyModulationGetDeviation()
        {
            try
            {
                //TODO May be FM:SENSE?
                return _lanExchanger.SendWithRequestDouble(":FM:DEV?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get signal deviation value. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Selects the modulation signal source for frequency modulation.
        /// </summary>
        /// <param name="type">True - internal source, False - external source</param>
        public void FrequencyModulationSelectSource(SourceType sourceType)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":FM:SOUR " + (sourceType == SourceType.INT_EXT
                                                     ? sourceType.ToString().Replace("_", ",")
                                                     : sourceType.ToString()) + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set frequency modulation signal source type " +
                                    (sourceType == SourceType.INT_EXT
                                        ? sourceType.ToString().Replace("_", ",")
                                        : sourceType.ToString()) +
                                    ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Selects the mode for the frequency modulation
        /// </summary>
        /// <param name="type">Mode - Norm, Lno or Hdev</param>
        public void FrequencyModulationCouplingMode(FrequencyMode type)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("FM:MODE " + type + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed set mode frequency modulation " + type + ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Selects the coupling mode for the external frequency modulation signal.
        ///     Default AC mode
        /// </summary>
        /// <param name="type">True - AC mode, False - DC mode</param>
        public void FrequencyModulationCouplingMode(bool type)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":FM:EXT:COUP " + (type ? "AC" : "DC") + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set frequency modulation set coupling " + (type ? "AC" : "DC") +
                                    ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Get the coupling mode for the external frequency modulation signal.
        /// </summary>
        /// <returns>String, contains value mode</returns>
        public string FrequencyModulationGetCouplingMode()
        {
            try
            {
                return _lanExchanger.SendWithRequestString(":FM:EXT:COUP?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get frequency modulation coupling. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Sets the deviation of the external frequency modulation signal in Hz. 
        ///     The maximum deviation depends on the set RF frequency and the selected modulation mode(see data sheet).
        ///     The sum of the deviations of all active frequency modulation signals may not exceed
        ///     the total value set with command[:SOURce<hw>]:FM[:DEViation] - FrequencyMOdulationSetDeviation method.
        ///     Increment 0.01, default value 1000
        ///     TODO: Required test
        /// </summary>
        /// <param name="valueExtDeviation">Vallue of deviation external frequency modulation in kHz</param>
        /// <param name="frequencyDeviationType">Type of Hz, kHz, mHz</param>
        public void FrequencyModulationSetExternalDeviation(double valueExtDeviation,
            FrequencyDeviationType frequencyDeviationType)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":FM:EXT:DEV " + valueExtDeviation + frequencyDeviationType);
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set deviation of the external frequency modulatino signal:  " +
                                    valueExtDeviation + frequencyDeviationType + ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Sets the deviation of the internal frequency modulation signal in Hz.
        ///     The sum of the deviations of all active frequency modulation signals may not exceed
        ///      the total value set with command[:SOURce<hw>]:FM[:DEViation].
        ///     Range: 0 to dynamic, increment 0.01, default value 1000
        ///     TODO: Required test
        /// </summary>
        /// <param name="valueIntDeviation">Vallue of deviation external frequency modulation in kHz</param>
        /// <param name="frequencyDeviationType">Type of Hz, kHz, mHz</param>
        public void FrequencyModulationSetInternalDeviation(double valueIntDeviation,
            FrequencyDeviationType frequencyDeviationType)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":FM:INT:DEV " + valueIntDeviation + frequencyDeviationType);
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set deviation of the internal frequency modulatino signal:  " +
                                    valueIntDeviation + frequencyDeviationType + ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Queries the input sensitivity of the externally applied signal for frequency modulation.
        ///     The returned value reports the sensitivity in Hz/V.It is assigned to the voltage value for full modulation of the input signal.
        ///     The sensitivity depends on the set [:SOURce<hw>]:FM[:DEViation].
        ///     TODO: Required test
        /// </summary>
        /// <returns>Value of input sens</returns>
        public double FrequencyModulationGetSens()
        {
            try
            {
                return _lanExchanger.SendWithRequestDouble(":FM:SENS?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get of input sens value. Reason: " + exception.Message);
            }
        }

        #endregion

        #region Pulse Modulation

        /// <summary>
        ///     Activates the pulse modulation.
        /// </summary>
        /// <param name="state">True - modulation is on, false - modulation is off</param>
        public void PulseModulationControlState(bool state)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":PULM:STAT " + (state ? "ON" : "OFF") + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set state pulse modulation in value of" + (state ? "ON" : "OFF") +
                                    ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Get pulse modulation state value
        /// </summary>
        /// <returns>True - modulation is on, False - modulation is off</returns>
        public bool PulseModulationGetState()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt(":PULM:STAT?;") == 1;
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get pulse modulation state value. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Selects the source for the pulse modulation signal.
        /// </summary>
        /// <param name="pulseModulationSource">Value of pulse modilation source</param>
        public void PulseModulationSelectSource(PulseModulationSource pulseModulationSource)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":PULM:SOUR " + pulseModulationSource + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set source pulse modulation in value of" + pulseModulationSource +
                                    ". Reason: " + exception.Message);
            }
        }


        /// <summary>
        ///     Sets the polarity between modulating and modulated signal. 
        ///     This command is effective only for an external modulation signal
        /// </summary>
        /// <param name="pulsePolarity">Value of polarity</param>
        public void PulseModulationSetPolarity(PulsePolarity pulsePolarity)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":PULM:POL " + pulsePolarity + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set the polarity in value of " + pulsePolarity + ". Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     Selects the trigger mode for pulse modulation.
        /// </summary>
        /// <param name="pulseModulationTriggerMode">Value of trigger for pulse modulation</param>
        public void PulseModulationSetTriggerMode(PulseModulationTriggerMode pulseModulationTriggerMode)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":PULM:TRIG:MODE " + pulseModulationTriggerMode);
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set the trigger mode in pulse modilation in value of " +
                                    pulseModulationTriggerMode + ". Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     Sets the mode of the pulse generator.
        /// </summary>
        /// <param name="pulseModulationMode">Value of mode for pulse generator</param>
        public void PulseModulationSetMode(PulseModulationMode pulseModulationMode)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":PULM:MODE " + pulseModulationMode + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set mode in pulse generator in value of " +
                                    pulseModulationMode + ". Reason: " +
                                    exception.Message);
            }
        }


        /// <summary>
        ///     Sets the width of the generated pulse. The width determines the pulse length. 
        ///     The pulse width must be at least 20ns less than the set pulse period.
        /// </summary>
        /// <param name="pulseWidth">Value of pulse width</param>
        /// <param name="pulseWidthType">Value of type pulse width</param>
        public void PulseModulationSetWidth(double pulseWidth, PulseWidthType pulseWidthType)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":PULM:WIDT " + pulseWidth + " " + pulseWidthType + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set the width of the generated pulse in value of " + pulseWidth +
                                    pulseWidthType + ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Gets the width of the generated pulse.
        /// </summary>
        public double PulseModulationGetWidth()
        {
            try
            {
                return _lanExchanger.SendWithRequestDouble(":PULM:WIDT?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get the width of the generated pulse. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Sets the pulse delay.
        /// </summary>
        /// <param name="pulseDelay">Value of pulse delay</param>
        /// <param name="pulseDelayType">Value of pulse delay type</param>
        public void PulseModulationSetDelay(double pulseDelay, PulseDelayType pulseDelayType)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":PULM:DEL " + pulseDelay + " " + pulseDelayType + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set the pulse delay of the generated pulse in value of " + pulseDelay +
                                    pulseDelayType + ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Gets the pulse delay.
        /// </summary>
        public double PulseModulationGetDelay()
        {
            try
            {
                return _lanExchanger.SendWithRequestDouble(":PULM:DEL?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get the pulse delay. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Sets the delay from the start of the first pulse to the start of the second pulse.
        ///     Range: 10 ns to 100 s, Increment: 5 ns, Default value: 3 us
        /// </summary>
        /// <param name="pulseDelay">Value of pulse double delay</param>
        /// <param name="pulseDelayType">Value of pulse delay type</param>
        public void PulseModulationSetDoubleDelay(double pulseDelay, PulseDelayType pulseDelayType)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":PULM:DOUB:DEL " + pulseDelay + " " + pulseDelayType + ";");
            }
            catch (Exception exception)
            {
                throw new Exception(
                    "Failed to set the the delay from the start of the first pulse to the start of the second pulse in value of " +
                    pulseDelay +
                    pulseDelayType + ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        /// Enables/disables double pulse generation.The two pulses are generated in one pulse
        /// period.
        /// </summary>
        ///<param name="state">True - Double pulse generation is on, False - Double pulse generation is off</param>
        public void PulseModulationStateControlDouble(bool state)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":PULM:DOUB:STAT " + (state ? "ON" : "OFF") + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set state pulm double pulse generation in value of" +
                                    (state ? "ON" : "OFF") +
                                    ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Sets the width of the second pulse in case of double pulse generation.
        /// </summary>
        /// <param name="valueOfWidth">Value of width pulse</param>
        /// <param name="pulseWidthType">Value of type width pulse</param>
        public void PulseModulationDoubleWidth(double valueOfWidth, PulseWidthType pulseWidthType)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":PULM:DOUB:WIDT " + valueOfWidth + " " + pulseWidthType + ";");
            }
            catch (Exception exception)
            {
                throw new Exception(
                    "Failed to set the width of the second pulse in case of double pulse generation in value of" +
                    valueOfWidth + pulseWidthType +
                    ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Sets the period of the generated pulse. 
        ///     The period determines the repetition frequency of the internal signal.
        /// </summary>
        /// <param name="valueOfPertiod">Value of pulm period</param>
        /// <param name="pulmPeriodType">Value of type pulm period</param>
        public void PulseModulationSetPeriod(double valueOfPertiod, PulsePeriodType pulmPeriodType)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":PULM:PER " + valueOfPertiod + " " + pulmPeriodType + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set pulm period in value of " + valueOfPertiod + pulmPeriodType +
                                    ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Gets the period of the generated pulse. 
        /// </summary>
        public double PulseModulationGetPeriod()
        {
            try
            {
                return _lanExchanger.SendWithRequestDouble(":PULM:PER?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get pulm period. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Selects the impedance for external pulse trigger.
        /// </summary>
        /// <param name="pulseModulationTriggerExternalImpedance">Value of external impedance</param>
        public void PulseModulationSetExternalImpedance(
            PulseModulationTriggerExternalImpedance pulseModulationTriggerExternalImpedance)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("SOUR:PULM:TRIG:EXT:IMP " + pulseModulationTriggerExternalImpedance);
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set the trigger mode external impedance in value of " +
                                    pulseModulationTriggerExternalImpedance + ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Sets the polarity of the active slope of an applied trigger at the PULSE EXT connector
        /// </summary>
        /// <param name="pulseModulationTriggerExternalSlope">Value of external impedance</param>
        public void PulseModulationSetExternalSlope(
            PulseModulationTriggerExternalSlope pulseModulationTriggerExternalSlope)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("SOUR:PULM:TRIG:EXT:SLOP " + pulseModulationTriggerExternalSlope);
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set the trigger mode external slope in value of " +
                                    pulseModulationTriggerExternalSlope + ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Selects the polarity of the Gate signal. The signal is supplied via the PULSE EXT connector.
        /// </summary>
        /// <param name="triggerExternalGatePolarity">Value of trigger external gate polarity</param>
        public void PulseModulationSetPolarityGateSignal(
            PulseModulationTriggerExternalGatePolarity triggerExternalGatePolarity)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("PULM:TRIG:EXT:GATE:POL " + triggerExternalGatePolarity);
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set the trigger mode external gate polarity in value of " +
                                    triggerExternalGatePolarity + ". Reason: " + exception.Message);
            }
        }

        #endregion

        #region Frequency 

        /// <summary>
        ///     Sets the frequency of the RF output signal.
        /// </summary>
        /// <param name="valueOfFreq">Value frequency</param>
        /// <param name="frequency">Value of Hz, kHz, mHz, GHz</param>
        public void FrequencySetValueOutputSignal(double valueOfFreq, Frequency frequency)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(
                    ":FREQ " + Convert.ToString(valueOfFreq, CultureInfo.InvariantCulture) + " " + frequency + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set frequency in value of " + valueOfFreq + frequency +
                                    ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Gets to the frequency value
        /// </summary>
        /// <returns></returns>
        public double FrequencyGetValueOutputSignal()
        {
            try
            {
                return _lanExchanger.SendWithRequestDouble(":FREQ?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get frequency value. Reason: " + exception.Message);
            }
        }

        //TODO: NOT USING! REQUIRED TEST

        /// <summary>
        ///     Sets the center frequency of the RF sweep range.
        /// </summary>
        /// <param name="valueOfCenterFreq">Value center frequency in MHz</param>
        public void FrequencySetCenterRfSweepRange(double valueOfCenterFreq)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":FREQ:CENT " + valueOfCenterFreq + " MHz;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set center frequency in value of " + valueOfCenterFreq +
                                    " MHz. Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Selects the frequency mode for the generating the RF output signal. The selected
        ///     mode determines the parameters to be used for further frequency settings.
        /// </summary>
        /// <param name="frequencyMode">Frequency mode: 
        ///     CW_FIX - Sets the fixed frequency mode, 
        ///     SWE - Sets the sweep mode. The instrument processes the frequency settings in defined sweep steps.</param>
        public void FrequencySetMode(FrequencyMode frequencyMode)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":FREQ:MODE " + frequencyMode + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set frequency mode in value of " + frequencyMode +
                                    ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Sets the value for the multiplication factor of a subsequent downstream instrument.
        /// </summary>
        /// <param name="multiplier">Value of multiplication</param>
        public void FrequencySetMultiplier(double multiplier)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":FREQ:MULT " + multiplier + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set frequency multiplier in value of " + multiplier +
                                    ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Sets the frequency offset of a downstream instrument, for example a mixer.
        /// </summary>
        /// <param name="offset">Value of offset in kHz</param>
        public void FrequencySetOffset(double offset)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":FREQ:OFFS " + offset + "kHz;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set frequency offset in value of " + offset +
                                    ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Determines the extent of the frequency sweep range. 
        /// </summary>
        /// <param name="span">Value of span in MHz</param>
        public void FrequencySetSpan(double span)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":FREQ:SPAN " + span + "MHz;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set frequency span in value of " + span +
                                    ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Sets the start frequency for the RF sweep.
        ///     This parameter relates to the center frequency and span.If you change the frequency, these parameters change accordingly.
        /// </summary>
        /// <param name="startValue">Value of start frequency</param>
        /// <param name="frequency">Frequency type</param>
        public void FrequencySetStart(double startValue, Frequency frequency)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":FREQ:SPAN " + startValue + " " + frequency + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set frequency start in value of " + startValue + frequency +
                                    ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Sets the stop frequency for the RF sweep.
        ///     This parameter is related to the center frequency and span.If you change the frequency, these parameters change accordingly
        /// </summary>
        /// <param name="stopValue">Value of stop frequency</param>
        /// <param name="frequency">Frequency type</param>
        public void FrequencySetStop(double stopValue, Frequency frequency)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":FREQ:STOP " + stopValue + " " + frequency + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set frequency stop in value of " + stopValue + frequency + ". Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     Sets the step width for FREQ:STEP:MODE USER.
        ///     To adjust the frequency step by step with this step size, use the FREQ:UP and
        ///     FREQ:DOWN commands.
        /// </summary>
        /// <param name="stepValue">Value of step in kHz</param>
        public void FrequencySetStep(double stepValue)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":FREQ:STEP " + stepValue + " kHz;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set frequency step in value of " + stepValue + " kHz. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     Activates(USER) or deactivates(DECimal) the user-defined step width used when
        ///     varying the frequency value with the frequency values UP/DOWN.The command is
        ///     linked to the command "Variation Active" for manual control, i.e.the command also
        ///     activates/deactivates the user-defined step width used when varying the frequency
        ///     value with the rotary knob.
        /// </summary>
        /// <param name="frequencyStepMode">Mode for Step frequency</param>
        public void FrequencySetStepMode(FrequencyStepMode frequencyStepMode)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":FREQ:STEP:MODE " + frequencyStepMode + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set frequency step mode in value of " + frequencyStepMode +
                                    ". Reason: " +
                                    exception.Message);
            }
        }

        #endregion

        #region Power

        /// <summary>
        ///     Sets the output signal in pow value.
        /// </summary>
        /// <param name="valueOfPow">Value of pow</param>
        /// <param name="powMode">Value of pow mode</param>
        public void PowSetValue(double valueOfPow, PowMode powMode)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(
                    ":POW " + Convert.ToString(valueOfPow, CultureInfo.InvariantCulture).Replace(',', '.') + " " +
                    powMode + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set pow in value of " + valueOfPow + powMode +
                                    ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Gets to the pow value
        /// </summary>
        /// <returns>Value of Pow</returns>
        public double PowGetValue()
        {
            try
            {
                return _lanExchanger.SendWithRequestDouble(":POW?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get pow value. Reason: " + exception.Message);
            }
        }

        #endregion

        #region Output Subsystem

        /// <summary>
        ///     Control RF output
        /// </summary>
        /// <param name="rfState">True - Rf otput state on, False - Rf output state off</param>
        public void OutputRfControlState(bool rfState)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":OUTPut:STAT " + (rfState ? "ON" : "OFF") + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set RF-Output state in " + (rfState ? "ON" : "OFF") +
                                    ". Reason: " + exception.Message);
            }
        }

        /// <summary>
        ///     Gets control state for RF
        /// </summary>
        /// <returns>True - RF is on, False - RF is off</returns>
        public bool OutputRfControlState()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt(":OUTP:STAT?;") == 1;
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get state RF. Reason: " + exception.Message);
            }
        }

        #endregion

        #endregion

        #endregion
    }
}