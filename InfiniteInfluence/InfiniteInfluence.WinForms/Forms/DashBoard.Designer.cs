namespace InfiniteInfluence.WinFormsApp
{
    partial class DashBoard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DashBoard));
            panelNavigationMenu = new Panel();
            iconButtonLogOut = new FontAwesome.Sharp.IconButton();
            iconButtonInfluencer = new FontAwesome.Sharp.IconButton();
            iconButtonHome = new FontAwesome.Sharp.IconButton();
            panelCompanyLogo = new Panel();
            pictureBoxCompanyLogo = new PictureBox();
            panelNavigationTitle = new Panel();
            pictureBoxStarSmall2 = new PictureBox();
            pictureBoxStarTiny = new PictureBox();
            pictureBoxStarSmall1 = new PictureBox();
            pictureBoxStarMedium = new PictureBox();
            pictureBoxStarLarge = new PictureBox();
            labelSelectedSideMenuTitle = new Label();
            pictureBoxTopFrame = new PictureBox();
            panelDesktop = new Panel();
            labelHomeUserName = new Label();
            labelHomeUserTitle = new Label();
            labelHomeWelcome = new Label();
            panelNavigationMenu.SuspendLayout();
            panelCompanyLogo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCompanyLogo).BeginInit();
            panelNavigationTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStarSmall2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStarTiny).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStarSmall1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStarMedium).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStarLarge).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxTopFrame).BeginInit();
            panelDesktop.SuspendLayout();
            SuspendLayout();
            // 
            // panelNavigationMenu
            // 
            panelNavigationMenu.BackColor = Color.FromArgb(44, 33, 56);
            panelNavigationMenu.Controls.Add(iconButtonLogOut);
            panelNavigationMenu.Controls.Add(iconButtonInfluencer);
            panelNavigationMenu.Controls.Add(iconButtonHome);
            panelNavigationMenu.Controls.Add(panelCompanyLogo);
            panelNavigationMenu.Dock = DockStyle.Left;
            panelNavigationMenu.Location = new Point(0, 0);
            panelNavigationMenu.Name = "panelNavigationMenu";
            panelNavigationMenu.Padding = new Padding(0, 0, 0, 15);
            panelNavigationMenu.Size = new Size(250, 825);
            panelNavigationMenu.TabIndex = 0;
            // 
            // iconButtonLogOut
            // 
            iconButtonLogOut.Dock = DockStyle.Bottom;
            iconButtonLogOut.FlatAppearance.BorderSize = 0;
            iconButtonLogOut.FlatStyle = FlatStyle.Flat;
            iconButtonLogOut.ForeColor = Color.FromArgb(222, 222, 222);
            iconButtonLogOut.IconChar = FontAwesome.Sharp.IconChar.SignOut;
            iconButtonLogOut.IconColor = Color.FromArgb(251, 250, 251);
            iconButtonLogOut.IconFont = FontAwesome.Sharp.IconFont.Auto;
            iconButtonLogOut.ImageAlign = ContentAlignment.MiddleLeft;
            iconButtonLogOut.Location = new Point(0, 750);
            iconButtonLogOut.Name = "iconButtonLogOut";
            iconButtonLogOut.Padding = new Padding(10, 0, 10, 0);
            iconButtonLogOut.Size = new Size(250, 60);
            iconButtonLogOut.TabIndex = 7;
            iconButtonLogOut.Text = "Log Out";
            iconButtonLogOut.TextImageRelation = TextImageRelation.ImageBeforeText;
            iconButtonLogOut.UseVisualStyleBackColor = true;
            iconButtonLogOut.Click += IconButtonLogOut_Click;
            // 
            // iconButtonInfluencer
            // 
            iconButtonInfluencer.Dock = DockStyle.Top;
            iconButtonInfluencer.FlatAppearance.BorderSize = 0;
            iconButtonInfluencer.FlatStyle = FlatStyle.Flat;
            iconButtonInfluencer.ForeColor = Color.FromArgb(222, 222, 222);
            iconButtonInfluencer.IconChar = FontAwesome.Sharp.IconChar.Scroll;
            iconButtonInfluencer.IconColor = Color.FromArgb(251, 250, 251);
            iconButtonInfluencer.IconFont = FontAwesome.Sharp.IconFont.Auto;
            iconButtonInfluencer.ImageAlign = ContentAlignment.MiddleLeft;
            iconButtonInfluencer.Location = new Point(0, 200);
            iconButtonInfluencer.Name = "iconButtonInfluencer";
            iconButtonInfluencer.Padding = new Padding(10, 0, 10, 0);
            iconButtonInfluencer.Size = new Size(250, 60);
            iconButtonInfluencer.TabIndex = 2;
            iconButtonInfluencer.Text = "Announcements";
            iconButtonInfluencer.TextImageRelation = TextImageRelation.ImageBeforeText;
            iconButtonInfluencer.UseVisualStyleBackColor = true;
            iconButtonInfluencer.Click += IconButtonAnnouncement_Click;
            // 
            // iconButtonHome
            // 
            iconButtonHome.Dock = DockStyle.Top;
            iconButtonHome.FlatAppearance.BorderSize = 0;
            iconButtonHome.FlatStyle = FlatStyle.Flat;
            iconButtonHome.ForeColor = Color.FromArgb(222, 222, 222);
            iconButtonHome.IconChar = FontAwesome.Sharp.IconChar.HomeLg;
            iconButtonHome.IconColor = Color.FromArgb(251, 250, 251);
            iconButtonHome.IconFont = FontAwesome.Sharp.IconFont.Auto;
            iconButtonHome.ImageAlign = ContentAlignment.MiddleLeft;
            iconButtonHome.Location = new Point(0, 140);
            iconButtonHome.Name = "iconButtonHome";
            iconButtonHome.Padding = new Padding(10, 0, 10, 0);
            iconButtonHome.Size = new Size(250, 60);
            iconButtonHome.TabIndex = 1;
            iconButtonHome.Text = "Home";
            iconButtonHome.TextImageRelation = TextImageRelation.ImageBeforeText;
            iconButtonHome.UseVisualStyleBackColor = true;
            iconButtonHome.Click += IconButtonHome_Click;
            // 
            // panelCompanyLogo
            // 
            panelCompanyLogo.Controls.Add(pictureBoxCompanyLogo);
            panelCompanyLogo.Dock = DockStyle.Top;
            panelCompanyLogo.Location = new Point(0, 0);
            panelCompanyLogo.Name = "panelCompanyLogo";
            panelCompanyLogo.Size = new Size(250, 140);
            panelCompanyLogo.TabIndex = 0;
            // 
            // pictureBoxCompanyLogo
            // 
            pictureBoxCompanyLogo.Anchor = AnchorStyles.None;
            pictureBoxCompanyLogo.Image = (Image)resources.GetObject("pictureBoxCompanyLogo.Image");
            pictureBoxCompanyLogo.Location = new Point(50, 15);
            pictureBoxCompanyLogo.Name = "pictureBoxCompanyLogo";
            pictureBoxCompanyLogo.Size = new Size(150, 100);
            pictureBoxCompanyLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxCompanyLogo.TabIndex = 6;
            pictureBoxCompanyLogo.TabStop = false;
            pictureBoxCompanyLogo.Click += PictureBoxCompanyLogo_Click;
            // 
            // panelNavigationTitle
            // 
            panelNavigationTitle.BackColor = Color.FromArgb(44, 33, 56);
            panelNavigationTitle.Controls.Add(pictureBoxStarSmall2);
            panelNavigationTitle.Controls.Add(pictureBoxStarTiny);
            panelNavigationTitle.Controls.Add(pictureBoxStarSmall1);
            panelNavigationTitle.Controls.Add(pictureBoxStarMedium);
            panelNavigationTitle.Controls.Add(pictureBoxStarLarge);
            panelNavigationTitle.Controls.Add(labelSelectedSideMenuTitle);
            panelNavigationTitle.Controls.Add(pictureBoxTopFrame);
            panelNavigationTitle.Dock = DockStyle.Top;
            panelNavigationTitle.Location = new Point(250, 0);
            panelNavigationTitle.Name = "panelNavigationTitle";
            panelNavigationTitle.Size = new Size(976, 190);
            panelNavigationTitle.TabIndex = 1;
            // 
            // pictureBoxStarSmall2
            // 
            pictureBoxStarSmall2.Anchor = AnchorStyles.None;
            pictureBoxStarSmall2.BackColor = Color.Transparent;
            pictureBoxStarSmall2.Image = (Image)resources.GetObject("pictureBoxStarSmall2.Image");
            pictureBoxStarSmall2.Location = new Point(69, 76);
            pictureBoxStarSmall2.Name = "pictureBoxStarSmall2";
            pictureBoxStarSmall2.Size = new Size(32, 32);
            pictureBoxStarSmall2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxStarSmall2.TabIndex = 14;
            pictureBoxStarSmall2.TabStop = false;
            // 
            // pictureBoxStarTiny
            // 
            pictureBoxStarTiny.Anchor = AnchorStyles.None;
            pictureBoxStarTiny.BackColor = Color.Transparent;
            pictureBoxStarTiny.Image = (Image)resources.GetObject("pictureBoxStarTiny.Image");
            pictureBoxStarTiny.Location = new Point(94, 41);
            pictureBoxStarTiny.Name = "pictureBoxStarTiny";
            pictureBoxStarTiny.Size = new Size(24, 24);
            pictureBoxStarTiny.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxStarTiny.TabIndex = 13;
            pictureBoxStarTiny.TabStop = false;
            // 
            // pictureBoxStarSmall1
            // 
            pictureBoxStarSmall1.Anchor = AnchorStyles.None;
            pictureBoxStarSmall1.BackColor = Color.Transparent;
            pictureBoxStarSmall1.Image = (Image)resources.GetObject("pictureBoxStarSmall1.Image");
            pictureBoxStarSmall1.Location = new Point(835, 65);
            pictureBoxStarSmall1.Name = "pictureBoxStarSmall1";
            pictureBoxStarSmall1.Size = new Size(32, 32);
            pictureBoxStarSmall1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxStarSmall1.TabIndex = 10;
            pictureBoxStarSmall1.TabStop = false;
            // 
            // pictureBoxStarMedium
            // 
            pictureBoxStarMedium.Anchor = AnchorStyles.None;
            pictureBoxStarMedium.BackColor = Color.Transparent;
            pictureBoxStarMedium.Image = (Image)resources.GetObject("pictureBoxStarMedium.Image");
            pictureBoxStarMedium.Location = new Point(766, 26);
            pictureBoxStarMedium.Name = "pictureBoxStarMedium";
            pictureBoxStarMedium.Size = new Size(40, 40);
            pictureBoxStarMedium.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxStarMedium.TabIndex = 11;
            pictureBoxStarMedium.TabStop = false;
            // 
            // pictureBoxStarLarge
            // 
            pictureBoxStarLarge.Anchor = AnchorStyles.None;
            pictureBoxStarLarge.BackColor = Color.Transparent;
            pictureBoxStarLarge.Image = (Image)resources.GetObject("pictureBoxStarLarge.Image");
            pictureBoxStarLarge.Location = new Point(152, 26);
            pictureBoxStarLarge.Name = "pictureBoxStarLarge";
            pictureBoxStarLarge.Size = new Size(64, 64);
            pictureBoxStarLarge.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxStarLarge.TabIndex = 12;
            pictureBoxStarLarge.TabStop = false;
            // 
            // labelSelectedSideMenuTitle
            // 
            labelSelectedSideMenuTitle.Anchor = AnchorStyles.None;
            labelSelectedSideMenuTitle.BackColor = Color.Transparent;
            labelSelectedSideMenuTitle.Font = new Font("Segoe UI", 18F);
            labelSelectedSideMenuTitle.ForeColor = Color.FromArgb(222, 222, 222);
            labelSelectedSideMenuTitle.Location = new Point(-12, 41);
            labelSelectedSideMenuTitle.Name = "labelSelectedSideMenuTitle";
            labelSelectedSideMenuTitle.Size = new Size(1000, 48);
            labelSelectedSideMenuTitle.TabIndex = 1;
            labelSelectedSideMenuTitle.Text = "Home";
            labelSelectedSideMenuTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pictureBoxTopFrame
            // 
            pictureBoxTopFrame.BackColor = Color.FromArgb(54, 48, 66);
            pictureBoxTopFrame.Dock = DockStyle.Fill;
            pictureBoxTopFrame.Image = (Image)resources.GetObject("pictureBoxTopFrame.Image");
            pictureBoxTopFrame.Location = new Point(0, 0);
            pictureBoxTopFrame.Name = "pictureBoxTopFrame";
            pictureBoxTopFrame.Size = new Size(976, 190);
            pictureBoxTopFrame.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxTopFrame.TabIndex = 15;
            pictureBoxTopFrame.TabStop = false;
            // 
            // panelDesktop
            // 
            panelDesktop.BackColor = Color.FromArgb(54, 48, 66);
            panelDesktop.Controls.Add(labelHomeUserName);
            panelDesktop.Controls.Add(labelHomeUserTitle);
            panelDesktop.Controls.Add(labelHomeWelcome);
            panelDesktop.Dock = DockStyle.Fill;
            panelDesktop.Location = new Point(250, 190);
            panelDesktop.Name = "panelDesktop";
            panelDesktop.Size = new Size(976, 635);
            panelDesktop.TabIndex = 2;
            // 
            // labelHomeUserName
            // 
            labelHomeUserName.Anchor = AnchorStyles.None;
            labelHomeUserName.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelHomeUserName.ForeColor = Color.FromArgb(251, 250, 251);
            labelHomeUserName.Location = new Point(0, 328);
            labelHomeUserName.Name = "labelHomeUserName";
            labelHomeUserName.Size = new Size(976, 39);
            labelHomeUserName.TabIndex = 3;
            labelHomeUserName.Text = "Maria Andreasen Juhl";
            labelHomeUserName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelHomeUserTitle
            // 
            labelHomeUserTitle.Anchor = AnchorStyles.None;
            labelHomeUserTitle.Font = new Font("Century Gothic", 10F);
            labelHomeUserTitle.ForeColor = Color.FromArgb(150, 144, 170);
            labelHomeUserTitle.Location = new Point(0, 282);
            labelHomeUserTitle.Name = "labelHomeUserTitle";
            labelHomeUserTitle.Size = new Size(976, 39);
            labelHomeUserTitle.TabIndex = 2;
            labelHomeUserTitle.Text = "System Administrator";
            labelHomeUserTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelHomeWelcome
            // 
            labelHomeWelcome.Anchor = AnchorStyles.None;
            labelHomeWelcome.Font = new Font("Century Gothic", 32F, FontStyle.Bold);
            labelHomeWelcome.ForeColor = Color.FromArgb(150, 144, 170);
            labelHomeWelcome.Location = new Point(0, 138);
            labelHomeWelcome.Name = "labelHomeWelcome";
            labelHomeWelcome.Size = new Size(976, 144);
            labelHomeWelcome.TabIndex = 1;
            labelHomeWelcome.Text = "Welcome Back";
            labelHomeWelcome.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DashBoard
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1226, 825);
            Controls.Add(panelDesktop);
            Controls.Add(panelNavigationTitle);
            Controls.Add(panelNavigationMenu);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "DashBoard";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Infinite Influence Control Panel";
            panelNavigationMenu.ResumeLayout(false);
            panelCompanyLogo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxCompanyLogo).EndInit();
            panelNavigationTitle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxStarSmall2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStarTiny).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStarSmall1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStarMedium).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxStarLarge).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxTopFrame).EndInit();
            panelDesktop.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panelNavigationMenu;
        private Panel panelCompanyLogo;
        private FontAwesome.Sharp.IconButton iconButtonHome;
        private FontAwesome.Sharp.IconButton iconButtonLogOut;
        private FontAwesome.Sharp.IconButton iconButtonInfluencer;
        private PictureBox pictureBoxCompanyLogo;
        private Panel panelNavigationTitle;
        private Label labelSelectedSideMenuTitle;
        private Panel panelDesktop;
        private PictureBox pictureBoxStarSmall2;
        private PictureBox pictureBoxStarTiny;
        private PictureBox pictureBoxStarSmall1;
        private PictureBox pictureBoxStarMedium;
        private PictureBox pictureBoxStarLarge;
        private Label labelHomeWelcome;
        private Label labelHomeUserTitle;
        private Label labelHomeUserName;
        private PictureBox pictureBoxTopFrame;
    }
}