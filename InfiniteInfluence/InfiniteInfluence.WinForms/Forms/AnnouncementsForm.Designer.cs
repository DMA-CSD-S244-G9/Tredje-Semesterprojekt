namespace InfiniteInfluence.WinFormsApp.Forms
{
    partial class AnnouncementsForm
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
            buttonInfluencerCreate = new Button();
            buttonInfluencerEdit = new Button();
            buttonInfluencerView = new Button();
            buttonAnnouncementDelete = new Button();
            panelInfluencerButtonRow = new Panel();
            panelInfluencerSearch = new Panel();
            roundedTextBoxAnnouncementsSearch = new InfiniteInfluence.WinFormsApp.Components.RoundedTextBox();
            panelAnnouncementContent = new Panel();
            panelInfluencerButtonRow.SuspendLayout();
            panelInfluencerSearch.SuspendLayout();
            SuspendLayout();
            // 
            // buttonInfluencerCreate
            // 
            buttonInfluencerCreate.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonInfluencerCreate.BackColor = Color.FromArgb(109, 84, 181);
            buttonInfluencerCreate.FlatAppearance.BorderColor = Color.FromArgb(109, 84, 181);
            buttonInfluencerCreate.FlatAppearance.BorderSize = 0;
            buttonInfluencerCreate.FlatAppearance.MouseDownBackColor = Color.FromArgb(64, 40, 100);
            buttonInfluencerCreate.FlatStyle = FlatStyle.Flat;
            buttonInfluencerCreate.Font = new Font("Century Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonInfluencerCreate.ForeColor = Color.FromArgb(251, 250, 251);
            buttonInfluencerCreate.Location = new Point(727, 55);
            buttonInfluencerCreate.Name = "buttonInfluencerCreate";
            buttonInfluencerCreate.Size = new Size(200, 50);
            buttonInfluencerCreate.TabIndex = 3;
            buttonInfluencerCreate.Text = "Create";
            buttonInfluencerCreate.UseVisualStyleBackColor = false;
            // 
            // buttonInfluencerEdit
            // 
            buttonInfluencerEdit.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonInfluencerEdit.BackColor = Color.FromArgb(109, 84, 181);
            buttonInfluencerEdit.FlatAppearance.BorderColor = Color.FromArgb(109, 84, 181);
            buttonInfluencerEdit.FlatAppearance.BorderSize = 0;
            buttonInfluencerEdit.FlatAppearance.MouseDownBackColor = Color.FromArgb(64, 40, 100);
            buttonInfluencerEdit.FlatStyle = FlatStyle.Flat;
            buttonInfluencerEdit.Font = new Font("Century Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonInfluencerEdit.ForeColor = Color.FromArgb(251, 250, 251);
            buttonInfluencerEdit.Location = new Point(502, 55);
            buttonInfluencerEdit.Name = "buttonInfluencerEdit";
            buttonInfluencerEdit.Size = new Size(200, 50);
            buttonInfluencerEdit.TabIndex = 4;
            buttonInfluencerEdit.Text = "Edit";
            buttonInfluencerEdit.UseVisualStyleBackColor = false;
            // 
            // buttonInfluencerView
            // 
            buttonInfluencerView.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonInfluencerView.BackColor = Color.FromArgb(109, 84, 181);
            buttonInfluencerView.FlatAppearance.BorderColor = Color.FromArgb(109, 84, 181);
            buttonInfluencerView.FlatAppearance.BorderSize = 0;
            buttonInfluencerView.FlatAppearance.MouseDownBackColor = Color.FromArgb(64, 40, 100);
            buttonInfluencerView.FlatStyle = FlatStyle.Flat;
            buttonInfluencerView.Font = new Font("Century Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonInfluencerView.ForeColor = Color.FromArgb(251, 250, 251);
            buttonInfluencerView.Location = new Point(277, 55);
            buttonInfluencerView.Name = "buttonInfluencerView";
            buttonInfluencerView.Size = new Size(200, 50);
            buttonInfluencerView.TabIndex = 5;
            buttonInfluencerView.Text = "View Details";
            buttonInfluencerView.UseVisualStyleBackColor = false;
            // 
            // buttonInfluencerDelete
            // 
            buttonAnnouncementDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonAnnouncementDelete.BackColor = Color.FromArgb(130, 40, 58);
            buttonAnnouncementDelete.FlatAppearance.BorderColor = Color.FromArgb(109, 84, 181);
            buttonAnnouncementDelete.FlatAppearance.BorderSize = 0;
            buttonAnnouncementDelete.FlatAppearance.MouseDownBackColor = Color.FromArgb(99, 30, 46);
            buttonAnnouncementDelete.FlatStyle = FlatStyle.Flat;
            buttonAnnouncementDelete.Font = new Font("Century Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonAnnouncementDelete.ForeColor = Color.FromArgb(251, 250, 251);
            buttonAnnouncementDelete.Location = new Point(52, 55);
            buttonAnnouncementDelete.Name = "buttonInfluencerDelete";
            buttonAnnouncementDelete.Size = new Size(200, 50);
            buttonAnnouncementDelete.TabIndex = 6;
            buttonAnnouncementDelete.Text = "Delete";
            buttonAnnouncementDelete.UseVisualStyleBackColor = false;
            buttonAnnouncementDelete.Click += buttonAnnouncementDelete_Click;
            // 
            // panelInfluencerButtonRow
            // 
            panelInfluencerButtonRow.BackColor = Color.FromArgb(54, 48, 66);
            panelInfluencerButtonRow.Controls.Add(buttonInfluencerCreate);
            panelInfluencerButtonRow.Controls.Add(buttonInfluencerView);
            panelInfluencerButtonRow.Controls.Add(buttonInfluencerEdit);
            panelInfluencerButtonRow.Controls.Add(buttonAnnouncementDelete);
            panelInfluencerButtonRow.Dock = DockStyle.Bottom;
            panelInfluencerButtonRow.Location = new Point(0, 419);
            panelInfluencerButtonRow.Name = "panelInfluencerButtonRow";
            panelInfluencerButtonRow.Size = new Size(978, 125);
            panelInfluencerButtonRow.TabIndex = 5;
            // 
            // panelInfluencerSearch
            // 
            panelInfluencerSearch.Controls.Add(roundedTextBoxAnnouncementsSearch);
            panelInfluencerSearch.Dock = DockStyle.Top;
            panelInfluencerSearch.Location = new Point(0, 0);
            panelInfluencerSearch.Name = "panelInfluencerSearch";
            panelInfluencerSearch.Size = new Size(978, 85);
            panelInfluencerSearch.TabIndex = 6;
            // 
            // roundedTextBoxInfluencerSearch
            // 
            roundedTextBoxAnnouncementsSearch.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            roundedTextBoxAnnouncementsSearch.BackColor = Color.FromArgb(60, 54, 76);
            roundedTextBoxAnnouncementsSearch.BorderColor = Color.FromArgb(134, 129, 158);
            roundedTextBoxAnnouncementsSearch.BorderFocusColor = Color.FromArgb(109, 84, 181);
            roundedTextBoxAnnouncementsSearch.BorderThickness = 3;
            roundedTextBoxAnnouncementsSearch.CornerRadius = 8;
            roundedTextBoxAnnouncementsSearch.Font = new Font("Segoe UI", 9F);
            roundedTextBoxAnnouncementsSearch.ForeColor = Color.FromArgb(134, 129, 158);
            roundedTextBoxAnnouncementsSearch.Location = new Point(727, 33);
            roundedTextBoxAnnouncementsSearch.Name = "roundedTextBoxInfluencerSearch";
            roundedTextBoxAnnouncementsSearch.Padding = new Padding(15, 2, 15, 2);
            roundedTextBoxAnnouncementsSearch.PlaceholderColor = Color.FromArgb(150, 144, 170);
            roundedTextBoxAnnouncementsSearch.PlaceholderText = "🔎   Search...";
            roundedTextBoxAnnouncementsSearch.Size = new Size(200, 31);
            roundedTextBoxAnnouncementsSearch.TabIndex = 5;
            // 
            // panelInfluencerContent
            // 
            panelAnnouncementContent.BackColor = Color.FromArgb(54, 48, 66);
            panelAnnouncementContent.Dock = DockStyle.Fill;
            panelAnnouncementContent.Location = new Point(0, 85);
            panelAnnouncementContent.Name = "panelInfluencerContent";
            panelAnnouncementContent.Padding = new Padding(50, 0, 50, 0);
            panelAnnouncementContent.Size = new Size(978, 334);
            panelAnnouncementContent.TabIndex = 7;
            // 
            // InfluencersForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(54, 48, 66);
            ClientSize = new Size(978, 544);
            Controls.Add(panelAnnouncementContent);
            Controls.Add(panelInfluencerSearch);
            Controls.Add(panelInfluencerButtonRow);
            Name = "InfluencersForm";
            Text = "Form1";
            panelInfluencerButtonRow.ResumeLayout(false);
            panelInfluencerSearch.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button buttonInfluencerCreate;
        private Button buttonInfluencerEdit;
        private Button buttonInfluencerView;
        private Button buttonAnnouncementDelete;
        private Panel panelInfluencerButtonRow;
        private Panel panelInfluencerSearch;
        private Components.RoundedTextBox roundedTextBoxAnnouncementsSearch;
        private Panel panelAnnouncementContent;
    }
}