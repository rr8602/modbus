using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ModbusClient
{
    public partial class MainForm : Form
    {
        // 리스너 관련 필드
        private TcpListener tcpListener;
        private UdpClient udpClient;
        private bool isTcpListening = false;
        private bool isUdpListening = false;
        private CancellationTokenSource tcpCancellationTokenSource;
        private CancellationTokenSource udpCancellationTokenSource;

        // 데이터 표시용 컨트롤
        private Label[] speedLabels;
        private TextBox logTextBox;

        // WM_COPYDATA 메시지 처리를 위한 상수 및 구조체
        private const int WM_COPYDATA = 0x004A;

        [StructLayout(LayoutKind.Sequential)]
        private struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        public MainForm()
        {
            InitializeComponent();
            InitializeUI();

            // 창 제목 설정 (SendMessage 통신용)
            this.Text = "ModbusClient";

            // 폼 로드/종료 시 처리
            this.Load += MainForm_Load;
            this.FormClosing += MainForm_FormClosing;
        }

        private void InitializeUI()
        {
            // 로그 표시용 텍스트 박스
            logTextBox = new TextBox
            {
                Location = new System.Drawing.Point(20, 150),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Size = new System.Drawing.Size(400, 200)
            };
            this.Controls.Add(logTextBox);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // TCP 리스너 시작
            StartTcpListener();
            StartUdpListener();

            LogMessage("리스너가 시작되었습니다 (포트: 8888)");

            // 로그 메시지
            LogMessage("ModbusClient 시작됨");
            LogMessage("연결 대기 중 (포트: 8888)");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // TCP 리스너 종료
            StopTcpListener();
            StopUdpListener();
        }

        private void StartTcpListener()
        {
            if (isTcpListening) return;

            try
            {
                // TCP 리스너 생성 및 시작
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();

                isTcpListening = true;

                // 취소 토큰 생성
                tcpCancellationTokenSource = new CancellationTokenSource();
                // 비동기로 연결 수신 대기
                Task.Run(() => ListenForClientsAsync(tcpCancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                LogMessage($"TCP 리스너 시작 오류: {ex.Message}");
            }
        }

        private void StopTcpListener()
        {
            if (!isTcpListening) return;

            try
            {
                // 취소 토큰으로 리스너 작업 중단 신호
                tcpCancellationTokenSource?.Cancel();

                // 리스너 종료
                tcpListener?.Stop();
                isTcpListening = false;

                LogMessage("TCP 리스너가 종료되었습니다.");
            }
            catch (Exception ex)
            {
                LogMessage($"TCP 리스너 종료 오류: {ex.Message}");
            }
        }

        private async Task ListenForClientsAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // 연결 대기
                    TcpClient client = await tcpListener.AcceptTcpClientAsync();
                    LogMessage($"ModbusServer에서 TCP 연결 수신: {((IPEndPoint)client.Client.RemoteEndPoint).Address}, 크기: {client.ReceiveBufferSize}");

                    // 클라이언트 처리를 별도 태스크로 시작
                    _ = HandleClientAsync(client, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // 정상적인 취소
            }
            catch (Exception ex) when (!(ex is ObjectDisposedException))
            {
                LogMessage($"TCP 연결 수신 오류: {ex.Message}");
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
        {
            try
            {
                using (client)
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[1024];

                    // 읽기 시간 제한 5초 설정
                    var readTask = stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    if (await Task.WhenAny(readTask, Task.Delay(5000, cancellationToken)) == readTask)
                    {
                        // 성공적으로 데이터 수신
                        int bytesRead = await readTask;

                        if (bytesRead > 0)
                        {
                            // 필요한 크기로 버퍼 조정
                            byte[] receivedData = new byte[bytesRead];
                            Array.Copy(buffer, receivedData, bytesRead);

                            // UI 스레드에서 데이터 처리
                            this.BeginInvoke(new Action<byte[]>(ProcessReceivedData), receivedData);
                        }
                    }
                    else
                    {
                        // 시간 초과
                        LogMessage("데이터 수신 시간 초과");
                    }
                }
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                LogMessage($"클라이언트 처리 중 오류: {ex.Message}");
            }
        }

        // 수신된 데이터 처리 메서드 수정
        private void ProcessReceivedData(byte[] data)
        {
            try
            {
                // 데이터 유효성 검사 (STX, ETX, 길이)
                if (data == null || data.Length != 10 || data[0] != 0x02 || data[9] != 0x03)
                {
                    LogMessage($"잘못된 데이터 형식: 길이={data?.Length ?? 0}");
                    return;
                }

                // 속도값 추출 (모든 속도)
                ushort speed1 = BitConverter.ToUInt16(data, 1);
                ushort speed2 = BitConverter.ToUInt16(data, 3);
                ushort speed3 = BitConverter.ToUInt16(data, 5);
                ushort speed4 = BitConverter.ToUInt16(data, 7);

                // 모든 속도값 UI에 업데이트
                if (lbl_speed1 != null) lbl_speed1.Text = $"속도 1: {speed1}";
                if (lbl_speed2 != null) lbl_speed2.Text = $"속도 2: {speed2}";
                if (lbl_speed3 != null) lbl_speed3.Text = $"속도 3: {speed3}";
                if (lbl_speed4 != null) lbl_speed4.Text = $"속도 4: {speed4}";

                LogMessage($"수신 데이터: 속도1={speed1}, 속도2={speed2}, 속도3={speed3}, 속도4={speed4}");
            }
            catch (Exception ex)
            {
                LogMessage($"데이터 처리 오류: {ex.Message}");
            }
        }

        // WM_COPYDATA 메시지 처리 (SendMessage 방식의 통신을 위한 오버라이드)
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_COPYDATA)
            {
                try
                {
                    COPYDATASTRUCT cds = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT));

                    if (cds.cbData > 0)
                    {
                        byte[] data = new byte[cds.cbData];
                        Marshal.Copy(cds.lpData, data, 0, cds.cbData);

                        LogMessage($"ModbusServer에서 SendMessage 연결 수신: { cds.lpData }, 크기: { cds.cbData }");

                        // 수신된 데이터 처리
                        ProcessReceivedData(data);

                        // 데이터 처리 성공 표시
                        m.Result = (IntPtr)1;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"WM_COPYDATA 처리 오류: {ex.Message}");
                }
            }

            base.WndProc(ref m);
        }

        // UDP 수신을 위한 메서드 추가
        private void StartUdpListener()
        {
            if (isUdpListening) return;

            try
            {
                // UDP 리스너 생성
                udpClient = new UdpClient(8888);
                isUdpListening = true;

                // 취소 토큰 생성
                udpCancellationTokenSource = new CancellationTokenSource();

                // 비동기로 UDP 수신 대기
                Task.Run(() => ListenForUdpClientsAsync(udpCancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                LogMessage($"UDP 리스너 시작 오류: {ex.Message}");
            }
        }

        // UDP 리스너 중지 메서드 추가
        private void StopUdpListener()
        {
            if (!isUdpListening) return;

            try
            {
                // 취소 토큰으로 리스너 작업 중단 신호
                udpCancellationTokenSource?.Cancel();

                // UDP 클라이언트 종료
                udpClient?.Close();
                udpClient?.Dispose();
                udpClient = null;

                isUdpListening = false;
                LogMessage("UDP 리스너가 종료되었습니다.");
            }
            catch (Exception ex)
            {
                LogMessage($"UDP 리스너 종료 오류: {ex.Message}");
            }
        }

        // UDP 수신 대기 메서드 추가
        private async Task ListenForUdpClientsAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // UDP 데이터 수신 대기 (WithCancellation 대체)
                    var receiveTask = udpClient.ReceiveAsync();
                    var cancellationTask = Task.Delay(-1, cancellationToken); // 무한 대기 태스크

                    var completedTask = await Task.WhenAny(receiveTask, cancellationTask);

                    if (completedTask == cancellationTask)
                    {
                        // 취소됨
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    // 정상 수신 처리
                    UdpReceiveResult result = await receiveTask;

                    LogMessage($"ModbusServer에서 UDP 연결 수신: {result.RemoteEndPoint.Address}, 크기: {result.Buffer.Length}바이트");

                    // UI 스레드에서 데이터 처리
                    this.BeginInvoke(new Action<byte[]>(ProcessReceivedData), result.Buffer);
                }
            }
            catch (OperationCanceledException)
            {
                // 정상적인 취소
                LogMessage("UDP 리스너가 취소되었습니다.");
            }
            catch (ObjectDisposedException)
            {
                // 객체가 이미 해제됨
                LogMessage("UDP 리스너 객체가 이미 해제되었습니다.");
            }
            catch (Exception ex)
            {
                LogMessage($"UDP 데이터 수신 오류: {ex.Message}");
            }
        }

        // 로그 메시지 추가
        private void LogMessage(string message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(LogMessage), message);
                return;
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            logTextBox.AppendText($"[{timestamp}] {message}{Environment.NewLine}");

            // 스크롤을 맨 아래로 이동
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();
        }
    }
}