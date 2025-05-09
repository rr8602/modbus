namespace ModbusServer
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbl_roller1 = new System.Windows.Forms.Label();
            this.lbl_roller2 = new System.Windows.Forms.Label();
            this.lbl_roller3 = new System.Windows.Forms.Label();
            this.lbl_roller4 = new System.Windows.Forms.Label();
            this.lbl_encoder4 = new System.Windows.Forms.Label();
            this.lbl_encoder3 = new System.Windows.Forms.Label();
            this.lbl_encoder2 = new System.Windows.Forms.Label();
            this.lbl_encoder1 = new System.Windows.Forms.Label();
            this.lbl_type = new System.Windows.Forms.Label();
            this.cmbCommType = new System.Windows.Forms.ComboBox();
            this.txt_roller2 = new System.Windows.Forms.TextBox();
            this.txt_roller3 = new System.Windows.Forms.TextBox();
            this.txt_roller4 = new System.Windows.Forms.TextBox();
            this.txt_encoder1 = new System.Windows.Forms.TextBox();
            this.txt_encoder2 = new System.Windows.Forms.TextBox();
            this.txt_encoder3 = new System.Windows.Forms.TextBox();
            this.txt_encoder4 = new System.Windows.Forms.TextBox();
            this.txt_roller1 = new System.Windows.Forms.TextBox();
            this.btn_writeToBoard = new System.Windows.Forms.Button();
            this.lbl_speed2 = new System.Windows.Forms.Label();
            this.lbl_speed1 = new System.Windows.Forms.Label();
            this.lbl_speed4 = new System.Windows.Forms.Label();
            this.lbl_speed3 = new System.Windows.Forms.Label();
            this.txt_speed1 = new System.Windows.Forms.TextBox();
            this.txt_speed4 = new System.Windows.Forms.TextBox();
            this.txt_speed3 = new System.Windows.Forms.TextBox();
            this.txt_speed2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lbl_roller1
            // 
            this.lbl_roller1.AutoSize = true;
            this.lbl_roller1.Location = new System.Drawing.Point(26, 21);
            this.lbl_roller1.Name = "lbl_roller1";
            this.lbl_roller1.Size = new System.Drawing.Size(75, 15);
            this.lbl_roller1.TabIndex = 0;
            this.lbl_roller1.Text = "롤러직경1";
            // 
            // lbl_roller2
            // 
            this.lbl_roller2.AutoSize = true;
            this.lbl_roller2.Location = new System.Drawing.Point(26, 54);
            this.lbl_roller2.Name = "lbl_roller2";
            this.lbl_roller2.Size = new System.Drawing.Size(75, 15);
            this.lbl_roller2.TabIndex = 1;
            this.lbl_roller2.Text = "롤러직경2";
            // 
            // lbl_roller3
            // 
            this.lbl_roller3.AutoSize = true;
            this.lbl_roller3.Location = new System.Drawing.Point(26, 89);
            this.lbl_roller3.Name = "lbl_roller3";
            this.lbl_roller3.Size = new System.Drawing.Size(75, 15);
            this.lbl_roller3.TabIndex = 2;
            this.lbl_roller3.Text = "롤러직경3";
            // 
            // lbl_roller4
            // 
            this.lbl_roller4.AutoSize = true;
            this.lbl_roller4.Location = new System.Drawing.Point(26, 119);
            this.lbl_roller4.Name = "lbl_roller4";
            this.lbl_roller4.Size = new System.Drawing.Size(75, 15);
            this.lbl_roller4.TabIndex = 3;
            this.lbl_roller4.Text = "롤러직경4";
            // 
            // lbl_encoder4
            // 
            this.lbl_encoder4.AutoSize = true;
            this.lbl_encoder4.Location = new System.Drawing.Point(26, 267);
            this.lbl_encoder4.Name = "lbl_encoder4";
            this.lbl_encoder4.Size = new System.Drawing.Size(95, 15);
            this.lbl_encoder4.TabIndex = 7;
            this.lbl_encoder4.Text = "엔코더 펄스4";
            // 
            // lbl_encoder3
            // 
            this.lbl_encoder3.AutoSize = true;
            this.lbl_encoder3.Location = new System.Drawing.Point(26, 237);
            this.lbl_encoder3.Name = "lbl_encoder3";
            this.lbl_encoder3.Size = new System.Drawing.Size(95, 15);
            this.lbl_encoder3.TabIndex = 6;
            this.lbl_encoder3.Text = "엔코더 펄스3";
            // 
            // lbl_encoder2
            // 
            this.lbl_encoder2.AutoSize = true;
            this.lbl_encoder2.Location = new System.Drawing.Point(26, 202);
            this.lbl_encoder2.Name = "lbl_encoder2";
            this.lbl_encoder2.Size = new System.Drawing.Size(95, 15);
            this.lbl_encoder2.TabIndex = 5;
            this.lbl_encoder2.Text = "엔코더 펄스2";
            // 
            // lbl_encoder1
            // 
            this.lbl_encoder1.AutoSize = true;
            this.lbl_encoder1.Location = new System.Drawing.Point(26, 169);
            this.lbl_encoder1.Name = "lbl_encoder1";
            this.lbl_encoder1.Size = new System.Drawing.Size(95, 15);
            this.lbl_encoder1.TabIndex = 4;
            this.lbl_encoder1.Text = "엔코더 펄스1";
            // 
            // lbl_type
            // 
            this.lbl_type.AutoSize = true;
            this.lbl_type.Location = new System.Drawing.Point(26, 315);
            this.lbl_type.Name = "lbl_type";
            this.lbl_type.Size = new System.Drawing.Size(102, 15);
            this.lbl_type.TabIndex = 16;
            this.lbl_type.Text = "프로토콜 타입";
            // 
            // cmbCommType
            // 
            this.cmbCommType.FormattingEnabled = true;
            this.cmbCommType.Items.AddRange(new object[] {
            "TCP",
            "UDP",
            "SendMessage"});
            this.cmbCommType.Location = new System.Drawing.Point(144, 312);
            this.cmbCommType.Name = "cmbCommType";
            this.cmbCommType.Size = new System.Drawing.Size(146, 23);
            this.cmbCommType.TabIndex = 9;
            // 
            // txt_roller2
            // 
            this.txt_roller2.Location = new System.Drawing.Point(144, 49);
            this.txt_roller2.Name = "txt_roller2";
            this.txt_roller2.Size = new System.Drawing.Size(146, 25);
            this.txt_roller2.TabIndex = 2;
            // 
            // txt_roller3
            // 
            this.txt_roller3.Location = new System.Drawing.Point(144, 86);
            this.txt_roller3.Name = "txt_roller3";
            this.txt_roller3.Size = new System.Drawing.Size(146, 25);
            this.txt_roller3.TabIndex = 3;
            // 
            // txt_roller4
            // 
            this.txt_roller4.Location = new System.Drawing.Point(144, 117);
            this.txt_roller4.Name = "txt_roller4";
            this.txt_roller4.Size = new System.Drawing.Size(146, 25);
            this.txt_roller4.TabIndex = 4;
            // 
            // txt_encoder1
            // 
            this.txt_encoder1.Location = new System.Drawing.Point(144, 166);
            this.txt_encoder1.Name = "txt_encoder1";
            this.txt_encoder1.Size = new System.Drawing.Size(146, 25);
            this.txt_encoder1.TabIndex = 5;
            // 
            // txt_encoder2
            // 
            this.txt_encoder2.Location = new System.Drawing.Point(144, 197);
            this.txt_encoder2.Name = "txt_encoder2";
            this.txt_encoder2.Size = new System.Drawing.Size(146, 25);
            this.txt_encoder2.TabIndex = 6;
            // 
            // txt_encoder3
            // 
            this.txt_encoder3.Location = new System.Drawing.Point(144, 234);
            this.txt_encoder3.Name = "txt_encoder3";
            this.txt_encoder3.Size = new System.Drawing.Size(146, 25);
            this.txt_encoder3.TabIndex = 7;
            // 
            // txt_encoder4
            // 
            this.txt_encoder4.Location = new System.Drawing.Point(144, 265);
            this.txt_encoder4.Name = "txt_encoder4";
            this.txt_encoder4.Size = new System.Drawing.Size(146, 25);
            this.txt_encoder4.TabIndex = 8;
            // 
            // txt_roller1
            // 
            this.txt_roller1.Location = new System.Drawing.Point(144, 18);
            this.txt_roller1.Name = "txt_roller1";
            this.txt_roller1.Size = new System.Drawing.Size(146, 25);
            this.txt_roller1.TabIndex = 1;
            // 
            // btn_writeToBoard
            // 
            this.btn_writeToBoard.Location = new System.Drawing.Point(29, 509);
            this.btn_writeToBoard.Name = "btn_writeToBoard";
            this.btn_writeToBoard.Size = new System.Drawing.Size(261, 58);
            this.btn_writeToBoard.TabIndex = 10;
            this.btn_writeToBoard.Text = "변경값 적용";
            this.btn_writeToBoard.UseVisualStyleBackColor = true;
            this.btn_writeToBoard.Click += new System.EventHandler(this.btn_writeToBoard_Click);
            // 
            // lbl_speed2
            // 
            this.lbl_speed2.AutoSize = true;
            this.lbl_speed2.Location = new System.Drawing.Point(26, 397);
            this.lbl_speed2.Name = "lbl_speed2";
            this.lbl_speed2.Size = new System.Drawing.Size(50, 15);
            this.lbl_speed2.TabIndex = 22;
            this.lbl_speed2.Text = "속도 2";
            // 
            // lbl_speed1
            // 
            this.lbl_speed1.AutoSize = true;
            this.lbl_speed1.Location = new System.Drawing.Point(26, 368);
            this.lbl_speed1.Name = "lbl_speed1";
            this.lbl_speed1.Size = new System.Drawing.Size(50, 15);
            this.lbl_speed1.TabIndex = 23;
            this.lbl_speed1.Text = "속도 1";
            // 
            // lbl_speed4
            // 
            this.lbl_speed4.AutoSize = true;
            this.lbl_speed4.Location = new System.Drawing.Point(26, 463);
            this.lbl_speed4.Name = "lbl_speed4";
            this.lbl_speed4.Size = new System.Drawing.Size(50, 15);
            this.lbl_speed4.TabIndex = 24;
            this.lbl_speed4.Text = "속도 4";
            // 
            // lbl_speed3
            // 
            this.lbl_speed3.AutoSize = true;
            this.lbl_speed3.Location = new System.Drawing.Point(26, 431);
            this.lbl_speed3.Name = "lbl_speed3";
            this.lbl_speed3.Size = new System.Drawing.Size(50, 15);
            this.lbl_speed3.TabIndex = 25;
            this.lbl_speed3.Text = "속도 3";
            // 
            // txt_speed1
            // 
            this.txt_speed1.Location = new System.Drawing.Point(144, 365);
            this.txt_speed1.Name = "txt_speed1";
            this.txt_speed1.ReadOnly = true;
            this.txt_speed1.Size = new System.Drawing.Size(146, 25);
            this.txt_speed1.TabIndex = 26;
            this.txt_speed1.TabStop = false;
            // 
            // txt_speed4
            // 
            this.txt_speed4.Location = new System.Drawing.Point(144, 462);
            this.txt_speed4.Name = "txt_speed4";
            this.txt_speed4.ReadOnly = true;
            this.txt_speed4.Size = new System.Drawing.Size(146, 25);
            this.txt_speed4.TabIndex = 27;
            this.txt_speed4.TabStop = false;
            // 
            // txt_speed3
            // 
            this.txt_speed3.Location = new System.Drawing.Point(144, 431);
            this.txt_speed3.Name = "txt_speed3";
            this.txt_speed3.ReadOnly = true;
            this.txt_speed3.Size = new System.Drawing.Size(146, 25);
            this.txt_speed3.TabIndex = 28;
            this.txt_speed3.TabStop = false;
            // 
            // txt_speed2
            // 
            this.txt_speed2.Location = new System.Drawing.Point(144, 397);
            this.txt_speed2.Name = "txt_speed2";
            this.txt_speed2.ReadOnly = true;
            this.txt_speed2.Size = new System.Drawing.Size(146, 25);
            this.txt_speed2.TabIndex = 29;
            this.txt_speed2.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(313, 598);
            this.Controls.Add(this.txt_speed2);
            this.Controls.Add(this.txt_speed3);
            this.Controls.Add(this.txt_speed4);
            this.Controls.Add(this.txt_speed1);
            this.Controls.Add(this.lbl_speed3);
            this.Controls.Add(this.lbl_speed4);
            this.Controls.Add(this.lbl_speed1);
            this.Controls.Add(this.lbl_speed2);
            this.Controls.Add(this.btn_writeToBoard);
            this.Controls.Add(this.txt_roller1);
            this.Controls.Add(this.cmbCommType);
            this.Controls.Add(this.lbl_type);
            this.Controls.Add(this.txt_encoder4);
            this.Controls.Add(this.txt_encoder3);
            this.Controls.Add(this.txt_encoder2);
            this.Controls.Add(this.txt_encoder1);
            this.Controls.Add(this.txt_roller4);
            this.Controls.Add(this.txt_roller3);
            this.Controls.Add(this.txt_roller2);
            this.Controls.Add(this.lbl_encoder4);
            this.Controls.Add(this.lbl_encoder3);
            this.Controls.Add(this.lbl_encoder2);
            this.Controls.Add(this.lbl_encoder1);
            this.Controls.Add(this.lbl_roller4);
            this.Controls.Add(this.lbl_roller3);
            this.Controls.Add(this.lbl_roller2);
            this.Controls.Add(this.lbl_roller1);
            this.Name = "MainForm";
            this.Text = "Agent";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_roller1;
        private System.Windows.Forms.Label lbl_roller2;
        private System.Windows.Forms.Label lbl_roller3;
        private System.Windows.Forms.Label lbl_roller4;
        private System.Windows.Forms.Label lbl_encoder4;
        private System.Windows.Forms.Label lbl_encoder3;
        private System.Windows.Forms.Label lbl_encoder2;
        private System.Windows.Forms.Label lbl_encoder1;
        private System.Windows.Forms.Label lbl_type;
        private System.Windows.Forms.ComboBox cmbCommType;
        private System.Windows.Forms.TextBox txt_roller2;
        private System.Windows.Forms.TextBox txt_roller3;
        private System.Windows.Forms.TextBox txt_roller4;
        private System.Windows.Forms.TextBox txt_encoder1;
        private System.Windows.Forms.TextBox txt_encoder2;
        private System.Windows.Forms.TextBox txt_encoder3;
        private System.Windows.Forms.TextBox txt_encoder4;
        private System.Windows.Forms.TextBox txt_roller1;
        private System.Windows.Forms.Button btn_writeToBoard;
        private System.Windows.Forms.Label lbl_speed2;
        private System.Windows.Forms.Label lbl_speed1;
        private System.Windows.Forms.Label lbl_speed4;
        private System.Windows.Forms.Label lbl_speed3;
        private System.Windows.Forms.TextBox txt_speed1;
        private System.Windows.Forms.TextBox txt_speed4;
        private System.Windows.Forms.TextBox txt_speed3;
        private System.Windows.Forms.TextBox txt_speed2;
    }
}

