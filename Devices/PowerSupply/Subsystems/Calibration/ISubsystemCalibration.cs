using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevicesControlLibrary.Devices.PowerSupply.AgilentN5746A;

namespace DevicesControlLibrary.Devices.PowerSupply.Subsystems.Calibration
{
    public interface ISubsystemCalibration
    {
        void SetCalibrationVoltage(double voltageValueOfCalibration);
        bool GetCalibrationState();
        void SetCalibrationState(bool state, string password = null);
        void SetCalibrationPassword(string password);
        string GetCalibrationLevel();
        void SetCalibrationLevel(N5746A.CalibrationLevel calibrationLevel);
        string GetCalibrationDate();
        void SetCalibrationDate(string date);
        void SetCalibrationData(double dataValue);

        void SetCalibrationOutputCurrent(double maximumValue);
    }
}