using System;
using System.IO.Ports;
using System.Windows.Forms;

using Modbus.Device;

namespace ModbusServer
{
    public class ModbusRTUReader
    {
        private readonly string portName;
        private readonly int baudRate;
        private SerialPort serialPort;

        public ModbusRTUReader(string portName, int baudRate = 115200)
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
                ReadTimeout = 100,
                WriteTimeout = 100
            };
        }

        public void ReopenPort(string portName)
        {

            try
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
                        ReadTimeout = 100,
                        WriteTimeout = 100
                    };
                }
                serialPort.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"시리얼 포트({portName}) 오픈 실패: {ex.Message}");
            }
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

                return null;
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
            try
            {
                if (serialPort == null)
                    throw new InvalidOperationException("SerialPort가 초기화되지 않았습니다.");

                if (!serialPort.IsOpen)
                    serialPort.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"시리얼 포트({portName}) 오픈 실패: {ex.Message}");
            }
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