using System;
using System.Windows.Forms;
using ModbusServer;

namespace ModBusTest
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm()); // MainForm이 올바른 클래스 이름인지 확인
        }
    }
}