namespace TestsDisplays
{
    partial class Form1
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
            this.PassedTest1textBox = new System.Windows.Forms.TextBox();
            this.TestTypecomboBox = new System.Windows.Forms.ComboBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.FailedTests1textBox = new System.Windows.Forms.TextBox();
            this.failedTests2label = new System.Windows.Forms.Label();
            this.failedTests2textBox = new System.Windows.Forms.TextBox();
            this.PassedTests2label = new System.Windows.Forms.Label();
            this.passedTests3textBox = new System.Windows.Forms.TextBox();
            this.TestName = new System.Windows.Forms.Label();
            this.SmartAirPortcomboBox = new System.Windows.Forms.ComboBox();
            this.SMAPortbutton = new System.Windows.Forms.Button();
            this.ArduinoPortcomboBox = new System.Windows.Forms.ComboBox();
            this.ArduinoPortbutton = new System.Windows.Forms.Button();
            this.ChannelIDLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PassedTest1textBox
            // 
            this.PassedTest1textBox.Location = new System.Drawing.Point(182, 154);
            this.PassedTest1textBox.Name = "PassedTest1textBox";
            this.PassedTest1textBox.ReadOnly = true;
            this.PassedTest1textBox.Size = new System.Drawing.Size(100, 20);
            this.PassedTest1textBox.TabIndex = 0;
            // 
            // TestTypecomboBox
            // 
            this.TestTypecomboBox.FormattingEnabled = true;
            this.TestTypecomboBox.Items.AddRange(new object[] {
            "Motor Signal",
            "Modes",
            "ArmDisarm",
            "Discharge",
            "XBT Disarm test",
            "XBT Power Up",
            "Trigger Due to RC",
            "Arm At Init tests",
            "Test Auto Port",
            "9. PWM @idle, arm & trigger ",
            "10. PWM @soft reset"});
            this.TestTypecomboBox.Location = new System.Drawing.Point(47, 98);
            this.TestTypecomboBox.Name = "TestTypecomboBox";
            this.TestTypecomboBox.Size = new System.Drawing.Size(121, 21);
            this.TestTypecomboBox.TabIndex = 2;
            this.TestTypecomboBox.Text = "Test Type";
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(207, 98);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 3;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 158);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Passed Tests:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(47, 206);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Failed Tests:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // FailedTests1textBox
            // 
            this.FailedTests1textBox.Location = new System.Drawing.Point(182, 202);
            this.FailedTests1textBox.Name = "FailedTests1textBox";
            this.FailedTests1textBox.ReadOnly = true;
            this.FailedTests1textBox.Size = new System.Drawing.Size(100, 20);
            this.FailedTests1textBox.TabIndex = 5;
            this.FailedTests1textBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // failedTests2label
            // 
            this.failedTests2label.AutoSize = true;
            this.failedTests2label.Location = new System.Drawing.Point(47, 306);
            this.failedTests2label.Name = "failedTests2label";
            this.failedTests2label.Size = new System.Drawing.Size(67, 13);
            this.failedTests2label.TabIndex = 10;
            this.failedTests2label.Text = "Failed Tests:";
            this.failedTests2label.Visible = false;
            // 
            // failedTests2textBox
            // 
            this.failedTests2textBox.Location = new System.Drawing.Point(182, 302);
            this.failedTests2textBox.Name = "failedTests2textBox";
            this.failedTests2textBox.ReadOnly = true;
            this.failedTests2textBox.Size = new System.Drawing.Size(100, 20);
            this.failedTests2textBox.TabIndex = 9;
            this.failedTests2textBox.Visible = false;
            // 
            // PassedTests2label
            // 
            this.PassedTests2label.AutoSize = true;
            this.PassedTests2label.Location = new System.Drawing.Point(47, 258);
            this.PassedTests2label.Name = "PassedTests2label";
            this.PassedTests2label.Size = new System.Drawing.Size(74, 13);
            this.PassedTests2label.TabIndex = 8;
            this.PassedTests2label.Text = "Passed Tests:";
            this.PassedTests2label.Visible = false;
            // 
            // passedTests3textBox
            // 
            this.passedTests3textBox.Location = new System.Drawing.Point(182, 254);
            this.passedTests3textBox.Name = "passedTests3textBox";
            this.passedTests3textBox.ReadOnly = true;
            this.passedTests3textBox.Size = new System.Drawing.Size(100, 20);
            this.passedTests3textBox.TabIndex = 7;
            this.passedTests3textBox.Visible = false;
            // 
            // TestName
            // 
            this.TestName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TestName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.TestName.Location = new System.Drawing.Point(24, 9);
            this.TestName.Name = "TestName";
            this.TestName.Size = new System.Drawing.Size(291, 23);
            this.TestName.TabIndex = 11;
            this.TestName.Text = "label3";
            this.TestName.Visible = false;
            // 
            // SmartAirPortcomboBox
            // 
            this.SmartAirPortcomboBox.FormattingEnabled = true;
            this.SmartAirPortcomboBox.Items.AddRange(new object[] {
            "SmartAir Port"});
            this.SmartAirPortcomboBox.Location = new System.Drawing.Point(47, 68);
            this.SmartAirPortcomboBox.Name = "SmartAirPortcomboBox";
            this.SmartAirPortcomboBox.Size = new System.Drawing.Size(121, 21);
            this.SmartAirPortcomboBox.TabIndex = 12;
            // 
            // SMAPortbutton
            // 
            this.SMAPortbutton.Location = new System.Drawing.Point(207, 67);
            this.SMAPortbutton.Name = "SMAPortbutton";
            this.SMAPortbutton.Size = new System.Drawing.Size(75, 23);
            this.SMAPortbutton.TabIndex = 13;
            this.SMAPortbutton.Text = "Open";
            this.SMAPortbutton.UseVisualStyleBackColor = true;
            this.SMAPortbutton.Click += new System.EventHandler(this.SMAPortbutton_Click);
            // 
            // ArduinoPortcomboBox
            // 
            this.ArduinoPortcomboBox.FormattingEnabled = true;
            this.ArduinoPortcomboBox.Items.AddRange(new object[] {
            "Arduino Port"});
            this.ArduinoPortcomboBox.Location = new System.Drawing.Point(47, 36);
            this.ArduinoPortcomboBox.Name = "ArduinoPortcomboBox";
            this.ArduinoPortcomboBox.Size = new System.Drawing.Size(121, 21);
            this.ArduinoPortcomboBox.TabIndex = 14;
            // 
            // ArduinoPortbutton
            // 
            this.ArduinoPortbutton.Location = new System.Drawing.Point(207, 36);
            this.ArduinoPortbutton.Name = "ArduinoPortbutton";
            this.ArduinoPortbutton.Size = new System.Drawing.Size(75, 23);
            this.ArduinoPortbutton.TabIndex = 15;
            this.ArduinoPortbutton.Text = "Open";
            this.ArduinoPortbutton.UseVisualStyleBackColor = true;
            this.ArduinoPortbutton.Click += new System.EventHandler(this.ArduinoPortbutton_Click);
            // 
            // ChannelIDLabel
            // 
            this.ChannelIDLabel.AutoSize = true;
            this.ChannelIDLabel.Location = new System.Drawing.Point(47, 133);
            this.ChannelIDLabel.Name = "ChannelIDLabel";
            this.ChannelIDLabel.Size = new System.Drawing.Size(63, 13);
            this.ChannelIDLabel.TabIndex = 16;
            this.ChannelIDLabel.Text = "Channel ID:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 332);
            this.Controls.Add(this.ChannelIDLabel);
            this.Controls.Add(this.ArduinoPortbutton);
            this.Controls.Add(this.ArduinoPortcomboBox);
            this.Controls.Add(this.SMAPortbutton);
            this.Controls.Add(this.SmartAirPortcomboBox);
            this.Controls.Add(this.TestName);
            this.Controls.Add(this.failedTests2label);
            this.Controls.Add(this.failedTests2textBox);
            this.Controls.Add(this.PassedTests2label);
            this.Controls.Add(this.passedTests3textBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.FailedTests1textBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.TestTypecomboBox);
            this.Controls.Add(this.PassedTest1textBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox PassedTest1textBox;
        private System.Windows.Forms.ComboBox TestTypecomboBox;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox FailedTests1textBox;
        private System.Windows.Forms.Label failedTests2label;
        private System.Windows.Forms.TextBox failedTests2textBox;
        private System.Windows.Forms.Label PassedTests2label;
        private System.Windows.Forms.TextBox passedTests3textBox;
        private System.Windows.Forms.Label TestName;
        private System.Windows.Forms.ComboBox SmartAirPortcomboBox;
        private System.Windows.Forms.Button SMAPortbutton;
        private System.Windows.Forms.ComboBox ArduinoPortcomboBox;
        private System.Windows.Forms.Button ArduinoPortbutton;
        private System.Windows.Forms.Label ChannelIDLabel;
    }
}

