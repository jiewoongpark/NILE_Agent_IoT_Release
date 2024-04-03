namespace IOT
{
    partial class frmIOTMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmIOTMain));
            this.LogSaveCheckBox = new System.Windows.Forms.CheckBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lbCurrentTime = new System.Windows.Forms.TextBox();
            this.LogGroupBox = new System.Windows.Forms.GroupBox();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnFilePath = new System.Windows.Forms.Button();
            this.filePathSaveCheckBox = new System.Windows.Forms.CheckBox();
            this.lbFilePath = new System.Windows.Forms.Label();
            this.txtLogPath = new System.Windows.Forms.TextBox();
            this.lbLogPath = new System.Windows.Forms.Label();
            this.btnSubscribe = new System.Windows.Forms.Button();
            this.lbConnectionIndicator = new System.Windows.Forms.Label();
            this.lbCoapTimer = new System.Windows.Forms.Label();
            this.CoapTimer = new System.Windows.Forms.NumericUpDown();
            this.ShowLog = new System.Windows.Forms.CheckBox();
            this.MessageTime = new System.Windows.Forms.CheckBox();
            folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.LogGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CoapTimer)).BeginInit();
            this.SuspendLayout();
            // 
            // LogSaveCheckBox
            // 
            this.LogSaveCheckBox.AutoSize = true;
            this.LogSaveCheckBox.Checked = true;
            this.LogSaveCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.LogSaveCheckBox.Location = new System.Drawing.Point(610, 390);
            this.LogSaveCheckBox.Name = "LogSaveCheckBox";
            this.LogSaveCheckBox.Size = new System.Drawing.Size(84, 21);
            this.LogSaveCheckBox.TabIndex = 20;
            this.LogSaveCheckBox.Text = "로그 저장";
            this.LogSaveCheckBox.UseVisualStyleBackColor = true;
            this.LogSaveCheckBox.CheckedChanged += new System.EventHandler(this.LogSaveCheckBox_CheckedChanged);
            // 
            // txtLog
            // 
            this.txtLog.AcceptsReturn = true;
            this.txtLog.AcceptsTab = true;
            this.txtLog.AccessibleName = "";
            this.txtLog.Location = new System.Drawing.Point(5, 25);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(690, 330);
            this.txtLog.TabIndex = 4;
            // 
            // lbCurrentTime
            // 
            this.lbCurrentTime.BackColor = System.Drawing.SystemColors.Menu;
            this.lbCurrentTime.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lbCurrentTime.Location = new System.Drawing.Point(495, 405);
            this.lbCurrentTime.Name = "lbCurrentTime";
            this.lbCurrentTime.Size = new System.Drawing.Size(188, 18);
            this.lbCurrentTime.TabIndex = 21;
            // 
            // LogGroupBox
            // 
            this.LogGroupBox.Controls.Add(this.txtFilePath);
            this.LogGroupBox.Controls.Add(this.btnFilePath);
            this.LogGroupBox.Controls.Add(this.filePathSaveCheckBox);
            this.LogGroupBox.Controls.Add(this.lbFilePath);
            this.LogGroupBox.Controls.Add(this.txtLogPath);
            this.LogGroupBox.Controls.Add(this.lbLogPath);
            this.LogGroupBox.Controls.Add(this.txtLog);
            this.LogGroupBox.Controls.Add(this.LogSaveCheckBox);
            this.LogGroupBox.Location = new System.Drawing.Point(5, 5);
            this.LogGroupBox.Name = "LogGroupBox";
            this.LogGroupBox.Size = new System.Drawing.Size(700, 415);
            this.LogGroupBox.TabIndex = 22;
            this.LogGroupBox.TabStop = false;
            this.LogGroupBox.Text = "Logs";
            // 
            // txtFilePath
            // 
            this.txtFilePath.Location = new System.Drawing.Point(108, 360);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(462, 25);
            this.txtFilePath.TabIndex = 43;
            this.txtFilePath.TextChanged += new System.EventHandler(this.txtFilePath_TextChanged);
            // 
            // btnFilePath
            // 
            this.btnFilePath.Location = new System.Drawing.Point(575, 360);
            this.btnFilePath.Name = "btnFilePath";
            this.btnFilePath.Size = new System.Drawing.Size(30, 23);
            this.btnFilePath.TabIndex = 42;
            this.btnFilePath.Text = "...";
            this.btnFilePath.UseVisualStyleBackColor = true;
            this.btnFilePath.Click += new System.EventHandler(this.btnFilePath_Click);
            // 
            // filePathSaveCheckBox
            // 
            this.filePathSaveCheckBox.AutoSize = true;
            this.filePathSaveCheckBox.Checked = true;
            this.filePathSaveCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.filePathSaveCheckBox.Location = new System.Drawing.Point(610, 365);
            this.filePathSaveCheckBox.Name = "filePathSaveCheckBox";
            this.filePathSaveCheckBox.Size = new System.Drawing.Size(84, 21);
            this.filePathSaveCheckBox.TabIndex = 41;
            this.filePathSaveCheckBox.Text = "위치 저장";
            this.filePathSaveCheckBox.UseVisualStyleBackColor = true;
            this.filePathSaveCheckBox.CheckedChanged += new System.EventHandler(this.filePathSaveCheckBox_CheckedChanged);
            // 
            // lbFilePath
            // 
            this.lbFilePath.AutoSize = true;
            this.lbFilePath.Location = new System.Drawing.Point(5, 365);
            this.lbFilePath.Name = "lbFilePath";
            this.lbFilePath.Size = new System.Drawing.Size(97, 22);
            this.lbFilePath.TabIndex = 40;
            this.lbFilePath.Text = "파일 저장 위치:";
            this.lbFilePath.UseCompatibleTextRendering = true;
            // 
            // txtLogPath
            // 
            this.txtLogPath.AcceptsReturn = true;
            this.txtLogPath.AcceptsTab = true;
            this.txtLogPath.AccessibleName = "";
            this.txtLogPath.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.txtLogPath.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLogPath.Location = new System.Drawing.Point(108, 390);
            this.txtLogPath.Multiline = true;
            this.txtLogPath.Name = "txtLogPath";
            this.txtLogPath.ReadOnly = true;
            this.txtLogPath.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLogPath.Size = new System.Drawing.Size(462, 20);
            this.txtLogPath.TabIndex = 39;
            // 
            // lbLogPath
            // 
            this.lbLogPath.AutoSize = true;
            this.lbLogPath.Location = new System.Drawing.Point(5, 390);
            this.lbLogPath.Name = "lbLogPath";
            this.lbLogPath.Size = new System.Drawing.Size(102, 22);
            this.lbLogPath.TabIndex = 38;
            this.lbLogPath.Text = "로그 저장 경로: ";
            this.lbLogPath.UseCompatibleTextRendering = true;
            // 
            // btnSubscribe
            // 
            this.btnSubscribe.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnSubscribe.Location = new System.Drawing.Point(580, 425);
            this.btnSubscribe.Name = "btnSubscribe";
            this.btnSubscribe.Size = new System.Drawing.Size(120, 30);
            this.btnSubscribe.TabIndex = 15;
            this.btnSubscribe.Text = "Subscribe";
            this.btnSubscribe.UseVisualStyleBackColor = false;
            this.btnSubscribe.Click += new System.EventHandler(this.btnSubscribe_Click);
            // 
            // lbConnectionIndicator
            // 
            this.lbConnectionIndicator.AutoSize = true;
            this.lbConnectionIndicator.Location = new System.Drawing.Point(30, 435);
            this.lbConnectionIndicator.Name = "lbConnectionIndicator";
            this.lbConnectionIndicator.Size = new System.Drawing.Size(88, 17);
            this.lbConnectionIndicator.TabIndex = 58;
            this.lbConnectionIndicator.Text = "Disconnected";
            // 
            // lbCoapTimer
            // 
            this.lbCoapTimer.AutoSize = true;
            this.lbCoapTimer.Location = new System.Drawing.Point(370, 432);
            this.lbCoapTimer.Name = "lbCoapTimer";
            this.lbCoapTimer.Size = new System.Drawing.Size(103, 17);
            this.lbCoapTimer.TabIndex = 62;
            this.lbCoapTimer.Text = "CoAP 요청 (sec)";
            // 
            // CoapTimer
            // 
            this.CoapTimer.Location = new System.Drawing.Point(479, 427);
            this.CoapTimer.Maximum = new decimal(new int[] { 3000, 0, 0, 0 });
            this.CoapTimer.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.CoapTimer.Name = "CoapTimer";
            this.CoapTimer.Size = new System.Drawing.Size(76, 25);
            this.CoapTimer.TabIndex = 61;
            this.CoapTimer.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // ShowLog
            // 
            this.ShowLog.AutoSize = true;
            this.ShowLog.Checked = true;
            this.ShowLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ShowLog.Location = new System.Drawing.Point(150, 431);
            this.ShowLog.Name = "ShowLog";
            this.ShowLog.Size = new System.Drawing.Size(84, 21);
            this.ShowLog.TabIndex = 44;
            this.ShowLog.Text = "로그 표출";
            this.ShowLog.UseVisualStyleBackColor = true;
            // 
            // MessageTime
            // 
            this.MessageTime.AutoSize = true;
            this.MessageTime.Checked = true;
            this.MessageTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MessageTime.Location = new System.Drawing.Point(240, 431);
            this.MessageTime.Name = "MessageTime";
            this.MessageTime.Size = new System.Drawing.Size(115, 21);
            this.MessageTime.TabIndex = 63;
            this.MessageTime.Text = "메시지 내 시간";
            this.MessageTime.UseVisualStyleBackColor = true;
            // 
            // frmIOTMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(714, 461);
            this.ControlBox = false;
            this.Controls.Add(this.MessageTime);
            this.Controls.Add(this.ShowLog);
            this.Controls.Add(this.lbCoapTimer);
            this.Controls.Add(this.CoapTimer);
            this.Controls.Add(this.lbConnectionIndicator);
            this.Controls.Add(this.btnSubscribe);
            this.Controls.Add(this.LogGroupBox);
            this.Controls.Add(this.lbCurrentTime);
            this.Font = new System.Drawing.Font("Malgun Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmIOTMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NILE Agent IoT (MQTT & CoAP Subscriber)";
            this.TransparencyKey = System.Drawing.Color.WhiteSmoke;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAgentMain_FormClosing);
            this.Load += new System.EventHandler(this.frmAgentMain_Load);
            this.LogGroupBox.ResumeLayout(false);
            this.LogGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CoapTimer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.CheckBox MessageTime;

        private System.Windows.Forms.CheckBox ShowLog;

        private System.Windows.Forms.Label lbCoapTimer;
        private System.Windows.Forms.NumericUpDown CoapTimer;

        #endregion
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.CheckBox LogSaveCheckBox;
        private System.Windows.Forms.TextBox lbCurrentTime;
        private System.Windows.Forms.GroupBox LogGroupBox;
        private System.Windows.Forms.Button btnSubscribe;
        private System.Windows.Forms.TextBox txtLogPath;
        private System.Windows.Forms.Label lbLogPath;
        private System.Windows.Forms.Label lbConnectionIndicator;
        private System.Windows.Forms.CheckBox filePathSaveCheckBox;
        private System.Windows.Forms.Label lbFilePath;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnFilePath;
    }
}

