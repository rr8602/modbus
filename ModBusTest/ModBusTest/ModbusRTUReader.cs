using System;
using System.IO.Ports;

using Modbus.Device;

namespace ModbusServer
{
    public class ModbusRTUReader
    {
        private readonly string portName;
        private readonly int baudRate;
        private SerialPort serialPort;

        public ModbusRTUReader(string portName = "COM3", int baudRate = 115200)
        {
            this.portName = portName;
            this.baudRate = baudRate;

            serialPort = new SerialPort
            {
                PortName = portName,
                BaudRate = baudRate,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                ReadTimeout = 1000,
                WriteTimeout = 1000
            };

            serialPort.Open();
        }

        public void ReopenPort(string portName)
        {
            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                    serialPort.Close();

                serialPort.PortName = portName;
            }
            else
            {
                serialPort = new SerialPort
                {
                    PortName = portName,
                    BaudRate = baudRate,
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    ReadTimeout = 1000,
                    WriteTimeout = 1000
                };
            }
            serialPort.Open();
        }

        // Board에서 값 읽기
        public ushort[] ReadSensorValues()
        {
            try
            {
                EnsurePortOpen();
                IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(serialPort);

                byte slaveId = 1;
                ushort startAddress = 0;
                ushort numRegisters = 20;

                return master.ReadHoldingRegisters(slaveId, startAddress, numRegisters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Modbus 통신 오류: {ex.Message}");
                throw;
            }
        }

        // Board에 값 쓰기
        public void WriteSensorValues(ushort[] valuesToWrite)
        {
            try
            {
                EnsurePortOpen();
                IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(serialPort);

                byte slaveId = 1;
                ushort startAddress = 1;

                master.WriteMultipleRegisters(slaveId, startAddress, valuesToWrite);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Modbus 통신 오류: {ex.Message}");
                throw;
            }
        }

        private void EnsurePortOpen()
        {
            if (serialPort == null)
                throw new InvalidOperationException("SerialPort가 초기화되지 않았습니다.");

            if (!serialPort.IsOpen)
                serialPort.Open();
        }

        public void Dispose()
        {
            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                    serialPort.Close();
                serialPort.Dispose();
                serialPort = null;
            }
        }
    }
}