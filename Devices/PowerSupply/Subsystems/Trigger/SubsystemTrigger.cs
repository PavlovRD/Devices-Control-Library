using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevicesControlLibrary.Exchange;

namespace DevicesControlLibrary.Devices.PowerSupply.Subsystems.Trigger
{
    public class SubsystemTrigger : ISubsystemTrigger
    {
        private readonly ILanExchanger _lanExchanger;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="lanExchanger">Interface lan exchanger</param>
        public SubsystemTrigger(ILanExchanger lanExchanger)
        {
            _lanExchanger = lanExchanger;
        }

        #region Abort commands

        /// <summary>
        ///     This command cancels any trigger actions in progress and returns the
        ///     trigger system to the IDLE state, unless INIT:CONT is enabled.
        ///     It also resets the WTG bit in the Status Operation Condition register.
        ///     ABORt is executed at power-on and upon execution of* RST.
        /// </summary>
        public void SetTriggerAbort()
        {
            try
            {
                _lanExchanger.SendWithoutRequest("ABOR;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set the abort of command of command. Reason: " +
                                    exception.Message);
            }
        }

        #endregion

        #region Initiate commands

        /// <summary>
        ///     This command controls the enabling of output triggers.
        ///     When a trigger is enabled, a trigger causes the specified triggering action to occur.
        ///     If the trigger system is not enabled, all triggers are ignored.
        /// </summary>
        public void SetInitiate()
        {
            try
            {
                _lanExchanger.SendWithoutRequest("INIT;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set the enabling of output triggers of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command controls the enabling of output triggers.
        ///     When a trigger is enabled, a trigger causes the specified triggering action to occur.
        ///     If the trigger system is not enabled, all triggers are ignored.
        /// </summary>
        /// <param name="state">True - is on, False - is off</param>
        public void SetInitiateContinuous(bool state)
        {
            try
            {
                _lanExchanger.SendWithoutRequest("INIT:CONT " + (state ? "ON" : "OFF") + ";");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set the enabling of output triggers of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command continuously initiates output triggers.
        ///     The enabled state is On(1); the disabled state is Off(0).
        ///     When disabled, the trigger system must be initiated for each 
        ///     trigger with the INITiate command.
        /// </summary>
        /// <returns>True - is on, False - is off</returns>
        public bool GetInitiateContinuous()
        {
            try
            {
                return _lanExchanger.SendWithRequestInt("INIT:CONT?;") == 1;
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get the enabling of output triggers of command. Reason: " +
                                    exception.Message);
            }
        }

        #endregion

        #region Trigger commands

        /// <summary>
        ///     If the trigger system has been initiated, this command generates an 
        ///     immediate output trigger.
        ///     When sent, the output trigger will:
        ///     - Initiate an output change as specified by the CURR:TRIG or VOLT:TRIG settings.
        ///     - Clear the WTG bits in the Status Operation Condition register after the trigger action has completed.
        /// </summary>
        public void SetTriggerImmediate()
        {
            try
            {
                _lanExchanger.SendWithoutRequest("TRIG;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set trigger immediate of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command selects the trigger source for the output trigger system.
        ///     Only BUS can be selected as the trigger source.
        /// </summary>
        public void SetTriggerSource()
        {
            try
            {
                _lanExchanger.SendWithoutRequest("TRIG:SOUR BUS;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set trigger source in BUS of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     Get trigger source
        /// </summary>
        /// <returns>Value of trigger source</returns>
        public string GetTriggerSource()
        {
            try
            {
                return _lanExchanger.SendWithRequestString("TRIG:SOUR?;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get trigger source of command. Reason: " +
                                    exception.Message);
            }
        }

        /// <summary>
        ///     This command generates a trigger when the trigger source is set to BUS.
        ///     The command has the same affect as the Group Execute Trigger(GET) command.
        /// </summary>
        public void SetTriggerGenerated()
        {
            try
            {
                _lanExchanger.SendWithoutRequest("TRG*;");
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to set generated trigger of command. Reason: " +
                                    exception.Message);
            }
        }

        #endregion
    }
}