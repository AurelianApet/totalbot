namespace TotalBot
{
    partial class LoginForm
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
            this.SerialkeyGroup = new System.Windows.Forms.GroupBox();
            this.SerialkeyBtn = new System.Windows.Forms.Button();
            this.SerialkeyText = new System.Windows.Forms.TextBox();
            this.MpandaLoginGroup = new System.Windows.Forms.GroupBox();
            this.pandaLoginBtn = new System.Windows.Forms.Button();
            this.pandaPassText = new System.Windows.Forms.TextBox();
            this.pandaIdText = new System.Windows.Forms.TextBox();
            this.pandaPassLabel = new System.Windows.Forms.Label();
            this.pandaIdLabel = new System.Windows.Forms.Label();
            this.SerialkeyGroup.SuspendLayout();
            this.MpandaLoginGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // SerialkeyGroup
            // 
            this.SerialkeyGroup.Controls.Add(this.SerialkeyBtn);
            this.SerialkeyGroup.Controls.Add(this.SerialkeyText);
            this.SerialkeyGroup.Font = new System.Drawing.Font("BatangChe", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SerialkeyGroup.Location = new System.Drawing.Point(12, 12);
            this.SerialkeyGroup.Name = "SerialkeyGroup";
            this.SerialkeyGroup.Size = new System.Drawing.Size(542, 97);
            this.SerialkeyGroup.TabIndex = 4;
            this.SerialkeyGroup.TabStop = false;
            this.SerialkeyGroup.Text = "비밀번호";
            // 
            // SerialkeyBtn
            // 
            this.SerialkeyBtn.Location = new System.Drawing.Point(419, 41);
            this.SerialkeyBtn.Name = "SerialkeyBtn";
            this.SerialkeyBtn.Size = new System.Drawing.Size(100, 24);
            this.SerialkeyBtn.TabIndex = 6;
            this.SerialkeyBtn.Text = "확인";
            this.SerialkeyBtn.UseVisualStyleBackColor = true;
            this.SerialkeyBtn.Click += new System.EventHandler(this.SerialkeyBtn_Click);
            // 
            // SerialkeyText
            // 
            this.SerialkeyText.Location = new System.Drawing.Point(22, 43);
            this.SerialkeyText.Name = "SerialkeyText";
            this.SerialkeyText.Size = new System.Drawing.Size(370, 22);
            this.SerialkeyText.TabIndex = 5;
            this.SerialkeyText.UseSystemPasswordChar = true;
            // 
            // MpandaLoginGroup
            // 
            this.MpandaLoginGroup.BackColor = System.Drawing.SystemColors.Control;
            this.MpandaLoginGroup.Controls.Add(this.pandaLoginBtn);
            this.MpandaLoginGroup.Controls.Add(this.pandaPassText);
            this.MpandaLoginGroup.Controls.Add(this.pandaIdText);
            this.MpandaLoginGroup.Controls.Add(this.pandaPassLabel);
            this.MpandaLoginGroup.Controls.Add(this.pandaIdLabel);
            this.MpandaLoginGroup.Enabled = false;
            this.MpandaLoginGroup.Font = new System.Drawing.Font("BatangChe", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MpandaLoginGroup.Location = new System.Drawing.Point(12, 115);
            this.MpandaLoginGroup.Name = "MpandaLoginGroup";
            this.MpandaLoginGroup.Size = new System.Drawing.Size(542, 98);
            this.MpandaLoginGroup.TabIndex = 5;
            this.MpandaLoginGroup.TabStop = false;
            this.MpandaLoginGroup.Text = "팬더TV 로그인";
            // 
            // pandaLoginBtn
            // 
            this.pandaLoginBtn.Location = new System.Drawing.Point(419, 27);
            this.pandaLoginBtn.Name = "pandaLoginBtn";
            this.pandaLoginBtn.Size = new System.Drawing.Size(100, 56);
            this.pandaLoginBtn.TabIndex = 4;
            this.pandaLoginBtn.Text = "로그인";
            this.pandaLoginBtn.UseVisualStyleBackColor = true;
            this.pandaLoginBtn.Click += new System.EventHandler(this.pandaLoginBtn_Click);
            // 
            // pandaPassText
            // 
            this.pandaPassText.Location = new System.Drawing.Point(125, 61);
            this.pandaPassText.Name = "pandaPassText";
            this.pandaPassText.Size = new System.Drawing.Size(267, 22);
            this.pandaPassText.TabIndex = 3;
            this.pandaPassText.UseSystemPasswordChar = true;
            // 
            // pandaIdText
            // 
            this.pandaIdText.Location = new System.Drawing.Point(125, 27);
            this.pandaIdText.Name = "pandaIdText";
            this.pandaIdText.Size = new System.Drawing.Size(267, 22);
            this.pandaIdText.TabIndex = 2;
            // 
            // pandaPassLabel
            // 
            this.pandaPassLabel.AutoSize = true;
            this.pandaPassLabel.Font = new System.Drawing.Font("BatangChe", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pandaPassLabel.Location = new System.Drawing.Point(29, 65);
            this.pandaPassLabel.Name = "pandaPassLabel";
            this.pandaPassLabel.Size = new System.Drawing.Size(59, 12);
            this.pandaPassLabel.TabIndex = 1;
            this.pandaPassLabel.Text = "비밀번호:";
            // 
            // pandaIdLabel
            // 
            this.pandaIdLabel.AutoSize = true;
            this.pandaIdLabel.Font = new System.Drawing.Font("BatangChe", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pandaIdLabel.Location = new System.Drawing.Point(30, 31);
            this.pandaIdLabel.Name = "pandaIdLabel";
            this.pandaIdLabel.Size = new System.Drawing.Size(53, 12);
            this.pandaIdLabel.TabIndex = 0;
            this.pandaIdLabel.Text = "아이디: ";
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 228);
            this.Controls.Add(this.MpandaLoginGroup);
            this.Controls.Add(this.SerialkeyGroup);
            this.Name = "LoginForm";
            this.Text = "로그인";
            this.SerialkeyGroup.ResumeLayout(false);
            this.SerialkeyGroup.PerformLayout();
            this.MpandaLoginGroup.ResumeLayout(false);
            this.MpandaLoginGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox SerialkeyGroup;
        private System.Windows.Forms.Button SerialkeyBtn;
        private System.Windows.Forms.TextBox SerialkeyText;
        private System.Windows.Forms.GroupBox MpandaLoginGroup;
        private System.Windows.Forms.Button pandaLoginBtn;
        private System.Windows.Forms.TextBox pandaPassText;
        private System.Windows.Forms.TextBox pandaIdText;
        private System.Windows.Forms.Label pandaPassLabel;
        private System.Windows.Forms.Label pandaIdLabel;
    }
}