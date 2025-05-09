using System;
using System.IO;
using System.Windows.Forms;
using IniParser.Model;
using IniParser;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace ModbusServer
{
    public class CommunicationHelper
    {
        // SendMessage API 선언
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // 통신 방식
        public enum CommType
        {
            TCP,
            UDP,
            SendMessage
        }

        // 통신 설정 클래스
        public class CommSettings
        {
            public CommType Type { get; set; }
            public string ServerIP { get; set; } = "127.0.0.1";
            public int ServerPort { get; set; } = 8888;
            public string TargetWindowName { get; set; } = "ModbusClient";
        }

        // INI 파일에서 설정 로드
        public static CommSettings LoadSettings(string iniFilePath = "C:\\Modbus\\ModBusTest\\ModBusTest\\agent_config.ini")
        {
            CommSettings settings = new CommSettings();

            try
            {
                if (File.Exists(iniFilePath))
                {
                    var parser = new FileIniDataParser();
                    IniData data = parser.ReadFile(iniFilePath);

                    string commTypeSetting = data["Comm"]["CommunicationType"] ?? "TCP";

                    // 통신 타입 설정
                    switch (commTypeSetting.ToUpper())
                    {
                        case "TCP":
                            settings.Type = CommType.TCP;
                            break;
                        case "UDP":
                            settings.Type = CommType.UDP;
                            break;
                        case "SENDMESSAGE":
                            settings.Type = CommType.SendMessage;
                            break;
                        default:
                            settings.Type = CommType.TCP;
                            break;
                    }

                    // 추가 설정 로드
                    settings.ServerIP = data["Comm"]["ServerIP"] ?? settings.ServerIP;
                    settings.ServerPort = int.TryParse(data["Comm"]["ServerPort"], out int port) ? port : settings.ServerPort;
                    settings.TargetWindowName = data["Comm"]["TargetWindowName"] ?? settings.TargetWindowName;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"설정 로드 실패: {ex.Message}");
                // 기본 설정 유지
            }

            return settings;
        }

        // 데이터 전송 메서드 (설정 객체 사용)
        public static void SendData(byte[] data, CommSettings settings)
        {
            try
            {
                switch (settings.Type)
                {
                    case CommType.TCP:
                        SendViaTCP(settings.ServerIP, settings.ServerPort, data);
                        break;

                    case CommType.UDP:
                        SendViaUDP(settings.ServerIP, settings.ServerPort, data);
                        break;

                    case CommType.SendMessage:
                        SendViaWindowsMessage(settings.TargetWindowName, data);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"데이터 전송 중 오류 발생: {ex.Message}");
                throw;
            }
        }

        // 간편하게 INI 파일에서 설정을 로드하고 데이터 전송
        public static void SendDataWithIniSettings(byte[] data, string iniFilePath = "C:\\Modbus\\ModBusTest\\ModBusTest\\agent_config.ini")
        {
            CommSettings settings = LoadSettings(iniFilePath);
            SendData(data, settings);
        }

        // TCP 전송 메서드
        private static void SendViaTCP(string ipAddress, int port, byte[] data)
        {
            using (TcpClient client = new TcpClient())
            {
                client.Connect(ipAddress, port);

                using (NetworkStream stream = client.GetStream())
                {
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                }
            }
        }

        // UDP 전송 메서드
        private static void SendViaUDP(string ipAddress, int port, byte[] data)
        {
            using (UdpClient client = new UdpClient())
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
                client.Send(data, data.Length, endPoint);
            }
        }

        // SendMessage 전송 메서드
        private static void SendViaWindowsMessage(string targetWindowName, byte[] data)
        {
            IntPtr hwnd = FindWindow(null, targetWindowName);

            if (hwnd == IntPtr.Zero)
            {
                throw new Exception($"'{targetWindowName}' 창을 찾을 수 없습니다.");
            }

            const uint WM_COPYDATA = 0x004A;

            // COPYDATASTRUCT 구조체를 생성하고 데이터 복사
            GCHandle dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                COPYDATASTRUCT cds = new COPYDATASTRUCT
                {
                    dwData = (IntPtr)1, // 임의의 식별자
                    cbData = data.Length,
                    lpData = dataHandle.AddrOfPinnedObject()
                };

                IntPtr cdsBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(cds));
                Marshal.StructureToPtr(cds, cdsBuffer, false);

                SendMessage(hwnd, WM_COPYDATA, IntPtr.Zero, cdsBuffer);

                Marshal.FreeHGlobal(cdsBuffer);
            }
            finally
            {
                if (dataHandle.IsAllocated)
                {
                    dataHandle.Free();
                }
            }
        }
    }

    // COPYDATASTRUCT 구조체 정의
    [StructLayout(LayoutKind.Sequential)]
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        public IntPtr lpData;
    }
}