using System;
using System.IO;
using System.Windows.Forms;
using IniParser.Model;
using IniParser;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Linq;

namespace ModbusServer
{
    public partial class MainForm : Form
    {
        private ModbusRTUReader modbusReader;
        private Timer updateTimer;

        private CommunicationHelper.CommSettings cachedSettings = null;
        private DateTime lastSettingsLoadTime = DateTime.MinValue;
        private ushort[] lastBoardValues = null;

        private bool modbusCommunicationEnabled = true;

        public MainForm()
        {
            InitializeComponent();
            UpdatePortList();

            updateTimer = new Timer();
            updateTimer.Interval = 100;
            updateTimer.Tick += UpdateSpeedValue;

            cmbCommType.SelectedIndexChanged += cmbCommType_SelectedIndexChanged;
            cmb_port.SelectedIndexChanged += async (s, e) => await cmb_port_SelectedIndexChanged(s, e);

            cmbCommType.SelectedIndex = 0;

            // 모드버스 활성화 상태 초기화 (TCP가 기본값)
            string initialCommType = cmbCommType.SelectedItem.ToString();
            modbusCommunicationEnabled = (initialCommType.ToUpper() != "SENDMESSAGE");
            Console.WriteLine($"초기 통신 방식: {initialCommType}, Modbus RTU 활성화: {modbusCommunicationEnabled}");

            
        }

        private async Task cmb_port_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedPort = cmb_port.SelectedItem?.ToString();

            // 기존 인스턴스가 있으면 반드시 Dispose
            if (modbusReader != null)
            {
                try { modbusReader.Dispose(); } catch { }

                modbusReader = null;
            }

            if (updateTimer != null && updateTimer.Enabled)
                updateTimer.Stop();

            try
            {
                modbusReader = new ModbusRTUReader(selectedPort);
                lastBoardValues = null;

                // 실제 Board와 통신 시도
                ushort[] boardValues = null;

                try
                {
                    boardValues = modbusReader.ReadSensorValues();
                }
                catch
                {
                    // 통신 실패 시 비정상 포트로 간주
                    boardValues = null;
                }

                if (boardValues == null)
                {
                    ClearTextBoxes();
                    modbusReader = null;
                    lastBoardValues = null;

                    return;
                }

                await UpdateBoardValues(this, EventArgs.Empty);

                if (updateTimer != null && !updateTimer.Enabled)
                    updateTimer.Start();
            }
            catch
            {
                ClearTextBoxes();
                modbusReader = null;
                lastBoardValues = null;

                if (updateTimer != null && updateTimer.Enabled)
                    updateTimer.Stop();
            }
        }

        private void ClearTextBoxes()
        {
            this.SuspendLayout();

            for (int i = 1; i <= 4; i++)
            {
                var roller = Controls["txt_roller" + i] as TextBox;
                var encoder = Controls["txt_encoder" + i] as TextBox;
                var speed = Controls["txt_speed" + i] as TextBox;

                if (roller != null)
                {
                    roller.KeyPress -= NumericTextBox_KeyPress;
                    roller.Enter -= TextBox_Enter;
                    roller.Leave -= TextBox_Leave;
                    roller.Text = "";
                    roller.KeyPress += NumericTextBox_KeyPress;
                    roller.Enter += TextBox_Enter;
                    roller.Leave += TextBox_Leave;
                }
                if (encoder != null)
                {
                    encoder.KeyPress -= NumericTextBox_KeyPress;
                    encoder.Enter -= TextBox_Enter;
                    encoder.Leave -= TextBox_Leave;
                    encoder.Text = "";
                    encoder.KeyPress += NumericTextBox_KeyPress;
                    encoder.Enter += TextBox_Enter;
                    encoder.Leave += TextBox_Leave;
                }
                if (speed != null)
                {
                    speed.Text = "";
                }
            }

            this.ResumeLayout();
        }

        // 활성화된(연결 가능한) 포트 목록 Get
        private void UpdatePortList()
        {
            string[] ports = SerialPort.GetPortNames();

            cmb_port.Items.Clear();

            if (ports.Length == 0)
            {
                cmb_port.Items.Add("사용 가능한 포트 없음");
                cmb_port.SelectedIndex = 0;
            }
            else
            {
                cmb_port.Items.AddRange(ports);
                cmb_port.SelectedIndex = 0;
            }
        }

        // Board에서 값 읽어와 UI 업데이트
        private async Task UpdateBoardValues(object sender, EventArgs e)
        {
            if (modbusReader == null)
                return;

            try
            {
                ushort[] boardValues = await Task.Run(() => modbusReader.ReadSensorValues());

                for (int i = 0; i < 4; i++)
                {
                    Controls["txt_roller" + (i + 1)].Text = boardValues[i + 1].ToString();
                    Controls["txt_encoder" + (i + 1)].Text = boardValues[i + 5].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Modbus 통신 오류: {ex.Message}");
                ClearTextBoxes();
            }
        }

        // 속도 값 실시간 갱신
        private async void UpdateSpeedValue(object sender, EventArgs e)
        {
            updateTimer.Stop();

            // modbusReader가 null이면 바로 리턴
            if (modbusReader == null)
            {
                return;
            }

            try
            {
                // 속도 값 읽기 (재시도 로직은 ModbusRTUReader 내부에 추가됨)
                ushort[] boardValues = await Task.Run(() => {
                    try
                    {
                        return modbusReader.ReadSensorValues();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"속도값 읽기 실패: {ex.Message}");
                        // 더미 값 반환
                        return new ushort[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    }
                });

                // 값이 변경되었는지 확인
                bool hasChanged = false;
                if (lastBoardValues == null || lastBoardValues.Length != boardValues.Length)
                {
                    hasChanged = true;
                }
                else
                {
                    // 속도 값들만 비교(13~16 인덱스)
                    for (int i = 13; i <= 16 && i < boardValues.Length && i < lastBoardValues.Length; i++)
                    {
                        if (boardValues[i] != lastBoardValues[i])
                        {
                            hasChanged = true;
                            break;
                        }
                    }
                }

                if (hasChanged)
                {
                    // UI 업데이트
                    for (int i = 0; i < 4 && i + 13 < boardValues.Length; i++)
                    {
                        Controls["txt_speed" + (i + 1)].Text = boardValues[i + 13].ToString();
                    }

                    // 값을 캐시
                    lastBoardValues = (ushort[])boardValues.Clone();

                    // 데이터 전송 처리 (별도 스레드에서)
                    await Task.Run(() => {
                        try
                        {
                            // 10바이트 데이터 생성
                            byte[] dataToSend = new byte[10];

                            // Byte 0 = STX (0x02)
                            dataToSend[0] = 0x02;

                            // Byte 1-8 = Speed 값들
                            BitConverter.GetBytes(boardValues[13]).CopyTo(dataToSend, 1);
                            BitConverter.GetBytes(boardValues[14]).CopyTo(dataToSend, 3);
                            BitConverter.GetBytes(boardValues[15]).CopyTo(dataToSend, 5);
                            BitConverter.GetBytes(boardValues[16]).CopyTo(dataToSend, 7);

                            // Byte 9 = ETX (0x03)
                            dataToSend[9] = 0x03;

                            // 캐시된 설정 사용 또는 새로 로드
                            if (cachedSettings == null || (DateTime.Now - lastSettingsLoadTime).TotalSeconds > 5)
                            {
                                cachedSettings = CommunicationHelper.LoadSettings();
                                lastSettingsLoadTime = DateTime.Now;
                            }

                            // 설정을 사용하여 데이터 전송
                            CommunicationHelper.SendData(dataToSend, cachedSettings);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"데이터 전송 실패: {ex.Message}");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"속도값 업데이트 오류: {ex.Message}");
            }
            finally
            {
                // 타이머 재시작 (폼이 아직 유효한 경우에만)
                if (!IsDisposed && updateTimer != null)
                    updateTimer.Start();
            }
        }

        // 사용자 입력값을 Board에 update
        private void btn_writeToBoard_Click(object sender, EventArgs e)
        {
            string selectedPort = cmb_port.SelectedItem.ToString();

            if (modbusReader == null)
            {
                MessageBox.Show("Board와 연결된 포트를 먼저 선택하세요. 현재 선택한 포트는 Board와 연결되지 않은 포트 입니다.", "포트 연결 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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

                if (modbusCommunicationEnabled)
                {
                    try
                    {
                        modbusReader.ReopenPort(selectedPort);
                        Console.WriteLine($"{selectedPort} 포트에 즉시 접근 시도 완료 (값 적용)");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{selectedPort} 포트 재오픈 실패(값 적용): {ex.Message}");
                    }
                }

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
                string iniFilePath = "E:\\modbus\\ModBusTest\\ModbusTest\\agent_config.ini";

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

        // cmbCommType의 SelectedIndexChanged 이벤트 핸들러 추가
        private void cmbCommType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (modbusReader == null || cmb_port.SelectedItem == null)
                return;

            string selectedType = cmbCommType.SelectedItem.ToString().ToUpper();
            string selectedPort = cmb_port.SelectedItem.ToString();

            if (modbusCommunicationEnabled)
            {
                try
                {
                    modbusReader.ReopenPort(selectedPort);
                    Console.WriteLine($"{selectedPort} 포트에 즉시 접근 시도 완료");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{selectedPort} 포트 재오픈 실패: {ex.Message}");
                }
            }

            // SendMessage일 때만 Modbus RTU 통신 비활성화
            modbusCommunicationEnabled = (selectedType != "SENDMESSAGE");

            // 설정 업데이트
            UpdateCommSettings(cmbCommType.SelectedItem.ToString());

            // 로그 메시지
            string message = modbusCommunicationEnabled
                ? $"Modbus RTU 통신이 활성화되었습니다. (통신 방식: {selectedType})"
                : $"Modbus RTU 통신이 비활성화되었습니다. (통신 방식: {selectedType})";

            Console.WriteLine(message);

            // 캐시 설정 초기화 - 통신 방식 변경 시 항상 새 설정 사용
            cachedSettings = null;

            // 테스트 데이터 생성 (Modbus RTU 비활성화 시)
            if (!modbusCommunicationEnabled && lastBoardValues != null)
            {
                // 기존 값 유지하면서 캐시 초기화
                ushort[] testValues = (ushort[])lastBoardValues.Clone();
                Random random = new Random();

                // 약간의 변화를 주어 실제 데이터처럼 보이도록 함
                for (int i = 13; i <= 16 && i < testValues.Length; i++)
                {
                    int currentValue = testValues[i];
                    // 약간의 변동폭을 더함 (-50 ~ +50)
                    int newValue = currentValue + random.Next(-50, 51);
                    newValue = Math.Max(0, Math.Min(65535, newValue)); // 범위 제한
                    testValues[i] = (ushort)newValue;
                }

                lastBoardValues = testValues;
            }
        }

        // 통신 설정 업데이트
        private void UpdateCommSettings(string commType)
        {
            try
            {
                string iniFilePath = "E:\\modbus\\ModBusTest\\ModbusTest\\agent_config.ini";

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

                // 통신 유형 설정
                data["Comm"]["CommunicationType"] = commType;

                // 설정 캐시 초기화
                cachedSettings = null;

                // INI 파일 저장
                parser.WriteFile(iniFilePath, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"통신 설정 업데이트 중 오류: {ex.Message}");
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