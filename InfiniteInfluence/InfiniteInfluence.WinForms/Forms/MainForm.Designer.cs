using InfiniteInfluence.WinFormsApp.Components;

namespace InfiniteInfluence.WinFormsApp
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            labelTitleControlPanel = new Label();
            buttonLogin = new Button();
            labelTitleLogin = new Label();
            pictureBoxBackground = new PictureBox();
            pictureBoxStarMedium = new PictureBox();
            pictureBoxStarSmall = new PictureBox();
            username = new RoundedTextBox();
            roundedTextBoxUsername = new RoundedTextBox();
            roundedTextBoxPassword = new RoundedTextBox();
            pictureBoxStarLarge = new PictureBox();
            linkLabelForgotPassword = new LinkLabel();
            ((System.ComponentModel.ISupportInitialize)pictureBoxBackground).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStarMedium).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStarSmall).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStarLarge).BeginInit();
            SuspendLayout();
            // 
            // labelTitleControlPanel
            // 
            labelTitleControlPanel.Anchor = AnchorStyles.None;
            labelTitleControlPanel.AutoSize = true;
            labelTitleControlPanel.Font = new Font("Century Gothic", 14F, FontStyle.Bold);
            labelTitleControlPanel.ForeColor = Color.FromArgb(251, 250, 251);
            labelTitleControlPanel.Location = new Point(590, 165);
            labelTitleControlPanel.Name = "labelTitleControlPanel";
            labelTitleControlPanel.Size = new Size(198, 34);
            labelTitleControlPanel.TabIndex = 0;
            labelTitleControlPanel.Text = "Control Panel";
            // 
            // buttonLogin
            // 
            buttonLogin.Anchor = AnchorStyles.None;
            buttonLogin.BackColor = Color.FromArgb(109, 84, 181);
            buttonLogin.FlatAppearance.BorderColor = Color.FromArgb(109, 84, 181);
            buttonLogin.FlatAppearance.BorderSize = 0;
            buttonLogin.FlatAppearance.MouseDownBackColor = Color.FromArgb(64, 40, 100);
            buttonLogin.FlatStyle = FlatStyle.Flat;
            buttonLogin.Font = new Font("Century Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonLogin.ForeColor = Color.FromArgb(251, 250, 251);
            buttonLogin.Location = new Point(572, 426);
            buttonLogin.Name = "buttonLogin";
            buttonLogin.Size = new Size(236, 50);
            buttonLogin.TabIndex = 3;
            buttonLogin.Text = "Login";
            buttonLogin.UseVisualStyleBackColor = false;
            buttonLogin.Click += buttonLogin_Click;
            // 
            // labelTitleLogin
            // 
            labelTitleLogin.Anchor = AnchorStyles.None;
            labelTitleLogin.AutoSize = true;
            labelTitleLogin.Font = new Font("Century Gothic", 10F, FontStyle.Bold);
            labelTitleLogin.ForeColor = Color.FromArgb(150, 144, 170);
            labelTitleLogin.Location = new Point(663, 205);
            labelTitleLogin.Name = "labelTitleLogin";
            labelTitleLogin.Size = new Size(62, 23);
            labelTitleLogin.TabIndex = 0;
            labelTitleLogin.Text = "Login";
            // 
            // pictureBoxBackground
            // 
            pictureBoxBackground.Dock = DockStyle.Left;
            pictureBoxBackground.Image = (Image)resources.GetObject("pictureBoxBackground.Image");
            pictureBoxBackground.Location = new Point(0, 0);
            pictureBoxBackground.Name = "pictureBoxBackground";
            pictureBoxBackground.Size = new Size(573, 660);
            pictureBoxBackground.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxBackground.TabIndex = 4;
            pictureBoxBackground.TabStop = false;
            // 
            // pictureBoxStarMedium
            // 
            pictureBoxStarMedium.Anchor = AnchorStyles.None;
            pictureBoxStarMedium.Image = (Image)resources.GetObject("pictureBoxStarMedium.Image");
            pictureBoxStarMedium.Location = new Point(795, 81);
            pictureBoxStarMedium.Name = "pictureBoxStarMedium";
            pictureBoxStarMedium.Size = new Size(64, 64);
            pictureBoxStarMedium.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxStarMedium.TabIndex = 5;
            pictureBoxStarMedium.TabStop = false;
            // 
            // pictureBoxStarSmall
            // 
            pictureBoxStarSmall.Anchor = AnchorStyles.None;
            pictureBoxStarSmall.Image = (Image)resources.GetObject("pictureBoxStarSmall.Image");
            pictureBoxStarSmall.Location = new Point(845, 151);
            pictureBoxStarSmall.Name = "pictureBoxStarSmall";
            pictureBoxStarSmall.Size = new Size(48, 48);
            pictureBoxStarSmall.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxStarSmall.TabIndex = 6;
            pictureBoxStarSmall.TabStop = false;
            // 
            // username
            // 
            username.BackColor = Color.FromArgb(60, 54, 76);
            username.BorderColor = Color.FromArgb(134, 129, 158);
            username.BorderFocusColor = Color.FromArgb(190, 170, 255);
            username.CornerRadius = 8;
            username.Font = new Font("Segoe UI", 10F);
            username.ForeColor = Color.FromArgb(134, 129, 158);
            username.Location = new Point(0, 0);
            username.Name = "username";
            username.Padding = new Padding(12, 8, 12, 8);
            username.PlaceholderColor = Color.FromArgb(150, 144, 170);
            username.Size = new Size(235, 32);
            username.TabIndex = 0;
            // 
            // roundedTextBoxUsername
            // 
            roundedTextBoxUsername.Anchor = AnchorStyles.None;
            roundedTextBoxUsername.BackColor = Color.FromArgb(60, 54, 76);
            roundedTextBoxUsername.BorderColor = Color.FromArgb(134, 129, 158);
            roundedTextBoxUsername.BorderFocusColor = Color.FromArgb(109, 84, 181);
            roundedTextBoxUsername.BorderThickness = 3;
            roundedTextBoxUsername.CornerRadius = 8;
            roundedTextBoxUsername.Font = new Font("Segoe UI", 9F);
            roundedTextBoxUsername.ForeColor = Color.FromArgb(134, 129, 158);
            roundedTextBoxUsername.Location = new Point(572, 263);
            roundedTextBoxUsername.Name = "roundedTextBoxUsername";
            roundedTextBoxUsername.Padding = new Padding(15, 2, 15, 2);
            roundedTextBoxUsername.PlaceholderColor = Color.FromArgb(150, 144, 170);
            roundedTextBoxUsername.PlaceholderText = "MariaJuhl27";
            roundedTextBoxUsername.RightToLeft = RightToLeft.No;
            roundedTextBoxUsername.Size = new Size(236, 31);
            roundedTextBoxUsername.TabIndex = 7;
            // 
            // roundedTextBoxPassword
            // 
            roundedTextBoxPassword.Anchor = AnchorStyles.None;
            roundedTextBoxPassword.BackColor = Color.FromArgb(60, 54, 76);
            roundedTextBoxPassword.BorderColor = Color.FromArgb(134, 129, 158);
            roundedTextBoxPassword.BorderFocusColor = Color.FromArgb(109, 84, 181);
            roundedTextBoxPassword.BorderThickness = 3;
            roundedTextBoxPassword.CornerRadius = 8;
            roundedTextBoxPassword.Font = new Font("Segoe UI", 9F);
            roundedTextBoxPassword.ForeColor = Color.FromArgb(134, 129, 158);
            roundedTextBoxPassword.IsPasswordField = true;
            roundedTextBoxPassword.Location = new Point(572, 312);
            roundedTextBoxPassword.Name = "roundedTextBoxPassword";
            roundedTextBoxPassword.Padding = new Padding(15, 2, 15, 2);
            roundedTextBoxPassword.PlaceholderColor = Color.FromArgb(150, 144, 170);
            roundedTextBoxPassword.PlaceholderText = "Password";
            roundedTextBoxPassword.Size = new Size(236, 31);
            roundedTextBoxPassword.TabIndex = 8;
            // 
            // pictureBoxStarLarge
            // 
            pictureBoxStarLarge.Anchor = AnchorStyles.None;
            pictureBoxStarLarge.Image = (Image)resources.GetObject("pictureBoxStarLarge.Image");
            pictureBoxStarLarge.Location = new Point(778, 508);
            pictureBoxStarLarge.Name = "pictureBoxStarLarge";
            pictureBoxStarLarge.Size = new Size(96, 96);
            pictureBoxStarLarge.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxStarLarge.TabIndex = 9;
            pictureBoxStarLarge.TabStop = false;
            // 
            // linkLabelForgotPassword
            // 
            linkLabelForgotPassword.ActiveLinkColor = Color.FromArgb(109, 84, 181);
            linkLabelForgotPassword.Anchor = AnchorStyles.None;
            linkLabelForgotPassword.AutoSize = true;
            linkLabelForgotPassword.Font = new Font("Century Gothic", 7F);
            linkLabelForgotPassword.ForeColor = Color.FromArgb(134, 129, 158);
            linkLabelForgotPassword.LinkBehavior = LinkBehavior.HoverUnderline;
            linkLabelForgotPassword.LinkColor = Color.FromArgb(134, 129, 158);
            linkLabelForgotPassword.Location = new Point(681, 350);
            linkLabelForgotPassword.Name = "linkLabelForgotPassword";
            linkLabelForgotPassword.Size = new Size(127, 19);
            linkLabelForgotPassword.TabIndex = 11;
            linkLabelForgotPassword.TabStop = true;
            linkLabelForgotPassword.Text = "Forgot Password?";
            linkLabelForgotPassword.MouseEnter += LinkLabelForgotPassword_Normal;
            linkLabelForgotPassword.MouseLeave += LinkLabelForgotPassword_Hover;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(44, 33, 56);
            ClientSize = new Size(950, 660);
            Controls.Add(linkLabelForgotPassword);
            Controls.Add(pictureBoxStarLarge);
            Controls.Add(roundedTextBoxUsername);
            Controls.Add(pictureBoxStarSmall);
            Controls.Add(pictureBoxStarMedium);
            Controls.Add(buttonLogin);
            Controls.Add(labelTitleLogin);
            Controls.Add(labelTitleControlPanel);
            Controls.Add(roundedTextBoxPassword);
            Controls.Add(pictureBoxBackground);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Infinite Influence Control Panel";
            ((System.ComponentModel.ISupportInitialize)pictureBoxBackground).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStarMedium).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStarSmall).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStarLarge).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }




        #endregion
        private Label labelTitleControlPanel;
        private Button buttonLogin;
        private Label labelTitleLogin;
        private PictureBox pictureBoxBackground;
        private PictureBox pictureBoxStarMedium;
        private PictureBox pictureBoxStarSmall;
        private RoundedTextBox username;
        private RoundedTextBox roundedTextBoxUsername;
        private RoundedTextBox roundedTextBoxPassword;
        private PictureBox pictureBoxStarLarge;
        private LinkLabel linkLabelForgotPassword;
    }
}
