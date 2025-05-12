namespace ModbusClient
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
            this.lbl_speed1 = new System.Windows.Forms.Label();
            this.lbl_speed2 = new System.Windows.Forms.Label();
            this.lbl_speed3 = new System.Windows.Forms.Label();
            this.lbl_speed4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbl_speed1
            // 
            this.lbl_speed1.AutoSize = true;
            this.lbl_speed1.Location = new System.Drawing.Point(34, 35);
            this.lbl_speed1.Name = "lbl_speed1";
            this.lbl_speed1.Size = new System.Drawing.Size(50, 15);
            this.lbl_speed1.TabIndex = 0;
            this.lbl_speed1.Text = "속도 1";
            // 
            // lbl_speed2
            // 
            this.lbl_speed2.AutoSize = true;
            this.lbl_speed2.Location = new System.Drawing.Point(34, 68);
            this.lbl_speed2.Name = "lbl_speed2";
            this.lbl_speed2.Size = new System.Drawing.Size(50, 15);
            this.lbl_speed2.TabIndex = 1;
            this.lbl_speed2.Text = "속도 2";
            // 
            // lbl_speed3
            // 
            this.lbl_speed3.AutoSize = true;
            this.lbl_speed3.Location = new System.Drawing.Point(34, 102);
            this.lbl_speed3.Name = "lbl_speed3";
            this.lbl_speed3.Size = new System.Drawing.Size(50, 15);
            this.lbl_speed3.TabIndex = 2;
            this.lbl_speed3.Text = "속도 3";
            // 
            // lbl_speed4
            // 
            this.lbl_speed4.AutoSize = true;
            this.lbl_speed4.Location = new System.Drawing.Point(34, 135);
            this.lbl_speed4.Name = "lbl_speed4";
            this.lbl_speed4.Size = new System.Drawing.Size(50, 15);
            this.lbl_speed4.TabIndex = 3;
            this.lbl_speed4.Text = "속도 4";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 510);
            this.Controls.Add(this.lbl_speed4);
            this.Controls.Add(this.lbl_speed3);
            this.Controls.Add(this.lbl_speed2);
            this.Controls.Add(this.lbl_speed1);
            this.Name = "MainForm";
            this.Text = "ModbusClient";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_speed1;
        private System.Windows.Forms.Label lbl_speed2;
        private System.Windows.Forms.Label lbl_speed3;
        private System.Windows.Forms.Label lbl_speed4;
    }
}

