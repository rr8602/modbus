using System;
using System.IO.Ports;

using Modbus.Device;

namespace ModbusServer
{
    public class ModbusRTUReader
    {
        private readonly string portName;
        private readonly int baudRate;

        public ModbusRTUReader(string portName = "COM3", int baudRate = 115200)
        {
            this.portName = portName;
            this.baudRate = baudRate;
        }

        // Board에서 값 읽기
        public ushort[] ReadSensorValues()
        {
            try
            {
                using (SerialPort serialPort = new SerialPort(portName))
                {
                    serialPort.BaudRate = baudRate;
                    serialPort.DataBits = 8;
                    serialPort.Parity = Parity.None;
                    serialPort.StopBits = StopBits.One;

                    serialPort.Open();
                    IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(serialPort);

                    byte slaveId = 1; // Slave ID
                    ushort startAddress = 0; // 시작 주소
                    ushort numRegisters = 20; // 읽을 레지스터 개수

                    // Modbus RTU를 통해 Holding Registers 읽기
                    ushort[] sensorValues = master.ReadHoldingRegisters(slaveId, startAddress, numRegisters);

                    serialPort.Close();
                    return sensorValues;
                }
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
                using (SerialPort serialPort = new SerialPort(portName))
                {
                    serialPort.BaudRate = baudRate;
                    serialPort.DataBits = 8;
                    serialPort.Parity = Parity.None;
                    serialPort.StopBits = StopBits.One;

                    serialPort.Open();
                    IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(serialPort);

                    byte slaveId = 1; // Slave ID
                    ushort startAddress = 1; // 시작 주소 (Modbus 주소 1)

                    // Modbus RTU를 통해 Holding Registers 쓰기
                    master.WriteMultipleRegisters(slaveId, startAddress, valuesToWrite);

                    serialPort.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Modbus 통신 오류: {ex.Message}");
                throw;
            }
        }
    }
}