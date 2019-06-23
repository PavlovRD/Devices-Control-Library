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

        public enum FREQuencySTEPMode
        {
            DEC,
            USER
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

        #region Source commands

        #region Base commands

        /// <summary>
        ///     Control state for all modulation in devices
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
                throw new Exception("Cloud not process source all modulation " + (state ? "ON" : "OFF") +
                                    " device command: " + exception.Message);
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
                throw new Exception("Failed to set amplitude modulation state value to " + (state ? "ON" : "OFF") +
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
                return _lanExchanger.SendWithRequestBool(":AM:STAT?;") == 1;
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
                throw new Exception("Failed to set state frequency modulation " + (state ? "ON" : "OFF") +
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
                return _lanExchanger.SendWithRequestBool(":FM:STAT?;") == 1;
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

        #region Frequency 

        /// <summary>
        ///     Sets the frequency of the RF output signal.
        /// </summary>
        /// <param name="valueOfFreq">Value frequency in MHz</param>
        /// <param name="frequency">Value of Hz, kHz, mHz, GHz</param>
        public void FrequencySetValueOutputSignal(double valueOfFreq, Frequency frequency)
        {
            try
            {
                _lanExchanger.SendWithoutRequest(":FREQ " + valueOfFreq + " " + frequency + ";");
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
        public void FrequencySetStepMode(FREQuencySTEPMode frequencyStepMode)
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

        #endregion

        #endregion
    }
}