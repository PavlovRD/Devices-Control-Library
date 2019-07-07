using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevicesControlLibrary.Exchange;

namespace DevicesControlLibrary.Devices.PowerSupply.Subsystems.Output
{
    public class SubsystemOutput : ISubsystemOutput
    {
        private readonly ILanExchanger _lanExchanger;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="lanExchanger">Interface lan exchanger</param>
        public SubsystemOutput(ILanExchanger lanExchanger)
        {
            _lanExchanger = lanExchanger;
        }
    }
}