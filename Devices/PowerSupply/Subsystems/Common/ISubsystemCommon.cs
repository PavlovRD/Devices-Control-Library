using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevicesControlLibrary.Devices.PowerSupply.Subsystems.Common
{
    internal interface ISubsystemCommon
    {
        void SetClearStatus();
        int GetStandartEventStatus();
        void SetStandartEventStatusEnable(int nrf);
        int SetStandartEventStatusEnable();
        string GetIdentificationInformation();
        void SetOpcEnable();
        bool GetOpc();
        int GetOpt();
        void SetRecallSavedState(int state);
        void Reset();
        void SetSaveState(int state);
        void SetServiceRequestEnableRegister(int nrf);
        string GetServiceRequestEnableRegister();
        string GetStatusByte();
        void SetTrigger();
        bool Test();
        void Wait();
    }
}