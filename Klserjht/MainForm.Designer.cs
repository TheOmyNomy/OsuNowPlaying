namespace Klserjht
{
    partial class MainForm
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
            this.usernameLabel = new System.Windows.Forms.Label();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.tokenTextBox = new System.Windows.Forms.TextBox();
            this.channelTextBox = new System.Windows.Forms.TextBox();
            this.formatTextBox = new System.Windows.Forms.TextBox();
            this.tokenLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.commandLabel = new System.Windows.Forms.Label();
            this.commandTextBox = new System.Windows.Forms.TextBox();
            this.loginButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.helpLinkLabel = new System.Windows.Forms.LinkLabel();
            this.updateLinkLabel = new System.Windows.Forms.LinkLabel();
            this.updateWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // usernameLabel
            // 
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Location = new System.Drawing.Point(13, 13);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(55, 13);
            this.usernameLabel.TabIndex = 8;
            this.usernameLabel.Text = "Username";
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Location = new System.Drawing.Point(74, 10);
            this.usernameTextBox.MaxLength = 25;
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(150, 20);
            this.usernameTextBox.TabIndex = 0;
            this.usernameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            // 
            // tokenTextBox
            // 
            this.tokenTextBox.Location = new System.Drawing.Point(74, 37);
            this.tokenTextBox.MaxLength = 36;
            this.tokenTextBox.Name = "tokenTextBox";
            this.tokenTextBox.PasswordChar = '*';
            this.tokenTextBox.Size = new System.Drawing.Size(150, 20);
            this.tokenTextBox.TabIndex = 1;
            this.tokenTextBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.tokenTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            // 
            // channelTextBox
            // 
            this.channelTextBox.Location = new System.Drawing.Point(74, 64);
            this.channelTextBox.MaxLength = 25;
            this.channelTextBox.Name = "channelTextBox";
            this.channelTextBox.Size = new System.Drawing.Size(150, 20);
            this.channelTextBox.TabIndex = 2;
            this.channelTextBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.channelTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            // 
            // formatTextBox
            // 
            this.formatTextBox.Location = new System.Drawing.Point(74, 91);
            this.formatTextBox.Name = "formatTextBox";
            this.formatTextBox.Size = new System.Drawing.Size(150, 20);
            this.formatTextBox.TabIndex = 3;
            this.formatTextBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.formatTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            // 
            // tokenLabel
            // 
            this.tokenLabel.AutoSize = true;
            this.tokenLabel.Location = new System.Drawing.Point(30, 40);
            this.tokenLabel.Name = "tokenLabel";
            this.tokenLabel.Size = new System.Drawing.Size(38, 13);
            this.tokenLabel.TabIndex = 9;
            this.tokenLabel.Text = "Token";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Channel";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Format";
            // 
            // commandLabel
            // 
            this.commandLabel.AutoSize = true;
            this.commandLabel.Location = new System.Drawing.Point(230, 13);
            this.commandLabel.Name = "commandLabel";
            this.commandLabel.Size = new System.Drawing.Size(54, 13);
            this.commandLabel.TabIndex = 12;
            this.commandLabel.Text = "Command";
            // 
            // commandTextBox
            // 
            this.commandTextBox.Location = new System.Drawing.Point(290, 10);
            this.commandTextBox.Name = "commandTextBox";
            this.commandTextBox.Size = new System.Drawing.Size(100, 20);
            this.commandTextBox.TabIndex = 4;
            this.commandTextBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.commandTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            // 
            // loginButton
            // 
            this.loginButton.Enabled = false;
            this.loginButton.Location = new System.Drawing.Point(233, 89);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(75, 23);
            this.loginButton.TabIndex = 6;
            this.loginButton.Text = "Login";
            this.loginButton.UseVisualStyleBackColor = true;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.Location = new System.Drawing.Point(315, 89);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(75, 23);
            this.exitButton.TabIndex = 7;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // helpLinkLabel
            // 
            this.helpLinkLabel.AutoSize = true;
            this.helpLinkLabel.Location = new System.Drawing.Point(251, 41);
            this.helpLinkLabel.Name = "helpLinkLabel";
            this.helpLinkLabel.Size = new System.Drawing.Size(115, 13);
            this.helpLinkLabel.TabIndex = 5;
            this.helpLinkLabel.TabStop = true;
            this.helpLinkLabel.Text = "Need help? Click here!";
            this.helpLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.helpLinkLabel_LinkClicked);
            // 
            // updateLinkLabel
            // 
            this.updateLinkLabel.Location = new System.Drawing.Point(230, 61);
            this.updateLinkLabel.Name = "updateLinkLabel";
            this.updateLinkLabel.Size = new System.Drawing.Size(160, 13);
            this.updateLinkLabel.TabIndex = 13;
            this.updateLinkLabel.TabStop = true;
            this.updateLinkLabel.Text = "Checking for updates...";
            this.updateLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.updateLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.updateLinkLabel_LinkClicked);
            // 
            // updateWorker
            // 
            this.updateWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.updateWorker_DoWork);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 122);
            this.Controls.Add(this.updateLinkLabel);
            this.Controls.Add(this.helpLinkLabel);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.commandTextBox);
            this.Controls.Add(this.commandLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tokenLabel);
            this.Controls.Add(this.formatTextBox);
            this.Controls.Add(this.channelTextBox);
            this.Controls.Add(this.tokenTextBox);
            this.Controls.Add(this.usernameTextBox);
            this.Controls.Add(this.usernameLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Klserjht";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.TextBox tokenTextBox;
        private System.Windows.Forms.TextBox channelTextBox;
        private System.Windows.Forms.TextBox formatTextBox;
        private System.Windows.Forms.Label tokenLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label commandLabel;
        private System.Windows.Forms.TextBox commandTextBox;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.LinkLabel helpLinkLabel;
        private System.Windows.Forms.LinkLabel updateLinkLabel;
        private System.ComponentModel.BackgroundWorker updateWorker;
    }
}

