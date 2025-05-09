using System;
using System.IO;
using System.Windows.Forms;
using IniParser.Model;
using IniParser;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace ModBusTest
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

        // 데이터 전송 메서드
        public static void SendData(CommType commType, byte[] data, string ipAddress = "127.0.0.1", int port = 8888, string targetWindow = "MainProgram")
        {
            try
            {
                switch (commType)
                {
                    case CommType.TCP:
                        SendViaTCP(ipAddress, port, data);
                        break;

                    case CommType.UDP:
                        SendViaUDP(ipAddress, port, data);
                        break;

                    case CommType.SendMessage:
                        SendViaWindowsMessage(targetWindow, data);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"데이터 전송 중 오류 발생: {ex.Message}");
                throw;
            }
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