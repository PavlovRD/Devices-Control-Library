using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevicesControlLibrary.Devices.PowerSupply.Subsystems.Trigger
{
    public interface ISubsystemTrigger
    {
        void SetTriggerAbort();
        void SetInitiate();
        void SetInitiateContinuous(bool state);
        bool GetInitiateContinuous();
        void SetTriggerImmediate();
        void SetTriggerSource();
        string GetTriggerSource();
        void SetTriggerGenerated();
    }
}