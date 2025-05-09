using System;
using System.IO;
using System.Windows.Forms;
using IniParser.Model;
using IniParser;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Linq;

namespace ModBusTest
{
    public partial class main_form : Form
    {
        private readonly ModbusRTUReader modbusReader;
        private readonly Timer updateTimer;

        public main_form()
        {
            InitializeComponent();
            cmbCommType.SelectedIndex = 0;

            modbusReader = new ModbusRTUReader();

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is TextBox &&
                   (ctrl.Name.StartsWith("txt_roller") || ctrl.Name.StartsWith("txt_encoder")))
                {
                    ((TextBox)ctrl).KeyPress += NumericTextBox_KeyPress;
                    ((TextBox)ctrl).Enter += TextBox_Enter;
                    ((TextBox)ctrl).Leave += TextBox_Leave;
                }
            }

            UpdateBoardValues(this, EventArgs.Empty);

            updateTimer = new Timer();
            updateTimer.Interval = 100;
            updateTimer.Tick += UpdateSpeedValue;
            updateTimer.Start();
        }

        // Board에서 값 읽어와 UI 업데이트
        private void UpdateBoardValues(object sender, EventArgs e)
        {
            try
            {
                ushort[] boardValues = modbusReader.ReadSensorValues();

                if (boardValues.Length >= 12)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Controls["txt_roller" + (i + 1)].Text = boardValues[i + 1].ToString();
                        Controls["txt_encoder" + (i + 1)].Text = boardValues[i + 5].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Modbus 통신 오류: {ex.Message}");
                throw;
            }
        }

        // 속도 값 실시간 갱신
        private void UpdateSpeedValue(object sender, EventArgs e)
        {
            try
            {
                ushort[] boardValues = modbusReader.ReadSensorValues();

                for (int i = 0; i < 4; i++)
                {
                    Controls["txt_speed" + (i + 1)].Text = boardValues[i + 13].ToString();
                }
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"Modbus 응답 시간 초과: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Modbus 통신 오류: {ex.Message}");
            }
        }

        // 사용자 입력값을 Board에 update
        private void btn_writeToBoard_Click(object sender, EventArgs e)
        {
            try
            {
                ushort[] valuesToWrite = new ushort[8]; // 롤러 값 4개 + 엔코더 값 4개

                for (int i = 0; i < 4; i++)
                {
                    valuesToWrite[i] = ushort.Parse(Controls["txt_roller" + (i + 1)].Text); // 롤러 값
                    valuesToWrite[i + 4] = ushort.Parse(Controls["txt_encoder" + (i + 1)].Text); // 엔코더 값
                }

                modbusReader.WriteSensorValues(valuesToWrite);

                UpdateIniFile(valuesToWrite);

                MessageBox.Show("Board에 값을 성공적으로 반영했습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Board에 값을 쓰는 중 오류 발생: {ex.Message}");
            }
        }

        // ini 파일 업데이트
        private void UpdateIniFile(ushort[] valuesToWrite)
        {
            try
            {
                string iniFilePath = "C:\\Modbus\\ModBusTest\\ModBusTest\\agent_config.ini";

                var parser = new FileIniDataParser();
                IniData data;

                // INI 파일 읽기 (없으면 새로 생성)
                if (File.Exists(iniFilePath))
                {
                    data = parser.ReadFile(iniFilePath);
                }
                else
                {
                    data = new IniData();
                }

                // 롤러 값 업데이트
                for (int i = 0; i < 4; i++)
                {
                    data["Roller"]["RollerDiameter" + (i + 1)] = valuesToWrite[i].ToString();
                }

                // 엔코더 값 업데이트
                for (int i = 0; i < 4; i++)
                {
                    data["Encoder"]["EncoderPulse" + (i + 1)] = valuesToWrite[i + 4].ToString();
                }

                // CommunicationType 값 업데이트
                string selectedType = cmbCommType.SelectedItem.ToString();
                data["Comm"]["CommunicationType"] = selectedType;

                // INI 파일 저장
                parser.WriteFile(iniFilePath, data);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"INI 파일 업데이트 중 오류 발생: {ex.Message}");
            }
        }

        // 숫자만 입력받도록 하는 이벤트
        private void NumericTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 제어키(예: 백스페이스)는 무조건 허용
            if (char.IsControl(e.KeyChar))
            {
                return;
            }

            // 숫자가 아닌 경우 입력 무시
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                // 처음 숫자가 0일 경우, 새로 입력된 숫자로 대체
                if (textBox.Text == "0")
                {
                    textBox.Text = e.KeyChar.ToString();
                    textBox.SelectionStart = textBox.Text.Length;
                    e.Handled = true;
                    return;
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (updateTimer != null)
            {
                updateTimer.Stop();
                updateTimer.Dispose();
            }
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            // 텍스트박스에 포커스가 들어오면 타이머 멈춤
            updateTimer.Stop();
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            // 텍스트박스에서 포커스가 벗어나면 타이머 다시 시작
            updateTimer.Start();
        }
    }
}