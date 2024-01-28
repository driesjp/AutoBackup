namespace AutoBackup_Config
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
            components = new System.ComponentModel.Container();
            BtnServiceStart = new Button();
            groupBoxService = new GroupBox();
            LblServiceStatus = new Label();
            LblServiceStatusText = new Label();
            BtnServiceStop = new Button();
            groupBoxFolders = new GroupBox();
            BtnFolderRemove = new Button();
            BtnFolderAdd = new Button();
            listBoxFolderSources = new ListBox();
            groupBoxTargetFolder = new GroupBox();
            textBoxTargetFolder = new TextBox();
            BtnChangeTargetFolder = new Button();
            BtnSaveSettings = new Button();
            BtnIntegrityCheck = new Button();
            linkLabelWebsite = new LinkLabel();
            serviceStatusTimer = new System.Windows.Forms.Timer(components);
            folderBrowserDialog = new FolderBrowserDialog();
            BtnOpenLogs = new Button();
            IntegrityCheckProgress = new ProgressBar();
            lblSettingsSaved = new Label();
            savedIndicatorTimer = new System.Windows.Forms.Timer(components);
            groupBoxService.SuspendLayout();
            groupBoxFolders.SuspendLayout();
            groupBoxTargetFolder.SuspendLayout();
            SuspendLayout();
            // 
            // BtnServiceStart
            // 
            BtnServiceStart.Location = new Point(20, 22);
            BtnServiceStart.Name = "BtnServiceStart";
            BtnServiceStart.Size = new Size(92, 31);
            BtnServiceStart.TabIndex = 0;
            BtnServiceStart.Text = "Start";
            BtnServiceStart.UseVisualStyleBackColor = true;
            BtnServiceStart.Click += BtnServiceStart_Click;
            // 
            // groupBoxService
            // 
            groupBoxService.Controls.Add(LblServiceStatus);
            groupBoxService.Controls.Add(LblServiceStatusText);
            groupBoxService.Controls.Add(BtnServiceStop);
            groupBoxService.Controls.Add(BtnServiceStart);
            groupBoxService.Location = new Point(615, 12);
            groupBoxService.Name = "groupBoxService";
            groupBoxService.Size = new Size(130, 134);
            groupBoxService.TabIndex = 1;
            groupBoxService.TabStop = false;
            groupBoxService.Text = "Service";
            // 
            // LblServiceStatus
            // 
            LblServiceStatus.AutoSize = true;
            LblServiceStatus.ForeColor = Color.FromArgb(192, 64, 0);
            LblServiceStatus.Location = new Point(20, 108);
            LblServiceStatus.Name = "LblServiceStatus";
            LblServiceStatus.Size = new Size(63, 15);
            LblServiceStatus.TabIndex = 3;
            LblServiceStatus.Text = "Checking..";
            // 
            // LblServiceStatusText
            // 
            LblServiceStatusText.AutoSize = true;
            LblServiceStatusText.Location = new Point(20, 93);
            LblServiceStatusText.Name = "LblServiceStatusText";
            LblServiceStatusText.Size = new Size(83, 15);
            LblServiceStatusText.TabIndex = 2;
            LblServiceStatusText.Text = "Backup status:";
            // 
            // BtnServiceStop
            // 
            BtnServiceStop.Location = new Point(20, 59);
            BtnServiceStop.Name = "BtnServiceStop";
            BtnServiceStop.Size = new Size(92, 31);
            BtnServiceStop.TabIndex = 1;
            BtnServiceStop.Text = "Stop";
            BtnServiceStop.UseVisualStyleBackColor = true;
            BtnServiceStop.Click += BtnServiceStop_Click;
            // 
            // groupBoxFolders
            // 
            groupBoxFolders.Controls.Add(BtnFolderRemove);
            groupBoxFolders.Controls.Add(BtnFolderAdd);
            groupBoxFolders.Controls.Add(listBoxFolderSources);
            groupBoxFolders.Location = new Point(12, 12);
            groupBoxFolders.Name = "groupBoxFolders";
            groupBoxFolders.Size = new Size(597, 340);
            groupBoxFolders.TabIndex = 2;
            groupBoxFolders.TabStop = false;
            groupBoxFolders.Text = "Folders";
            // 
            // BtnFolderRemove
            // 
            BtnFolderRemove.Location = new Point(104, 302);
            BtnFolderRemove.Name = "BtnFolderRemove";
            BtnFolderRemove.Size = new Size(92, 31);
            BtnFolderRemove.TabIndex = 2;
            BtnFolderRemove.Text = "Remove";
            BtnFolderRemove.UseVisualStyleBackColor = true;
            BtnFolderRemove.Click += BtnFolderRemove_Click;
            // 
            // BtnFolderAdd
            // 
            BtnFolderAdd.Location = new Point(6, 302);
            BtnFolderAdd.Name = "BtnFolderAdd";
            BtnFolderAdd.Size = new Size(92, 31);
            BtnFolderAdd.TabIndex = 1;
            BtnFolderAdd.Text = "Add";
            BtnFolderAdd.UseVisualStyleBackColor = true;
            BtnFolderAdd.Click += BtnFolderAdd_Click;
            // 
            // listBoxFolderSources
            // 
            listBoxFolderSources.FormattingEnabled = true;
            listBoxFolderSources.ItemHeight = 15;
            listBoxFolderSources.Location = new Point(6, 22);
            listBoxFolderSources.Name = "listBoxFolderSources";
            listBoxFolderSources.Size = new Size(585, 274);
            listBoxFolderSources.TabIndex = 0;
            // 
            // groupBoxTargetFolder
            // 
            groupBoxTargetFolder.Controls.Add(textBoxTargetFolder);
            groupBoxTargetFolder.Controls.Add(BtnChangeTargetFolder);
            groupBoxTargetFolder.Location = new Point(12, 358);
            groupBoxTargetFolder.Name = "groupBoxTargetFolder";
            groupBoxTargetFolder.Size = new Size(597, 55);
            groupBoxTargetFolder.TabIndex = 3;
            groupBoxTargetFolder.TabStop = false;
            groupBoxTargetFolder.Text = "Target Folder";
            // 
            // textBoxTargetFolder
            // 
            textBoxTargetFolder.Enabled = false;
            textBoxTargetFolder.Location = new Point(6, 22);
            textBoxTargetFolder.Name = "textBoxTargetFolder";
            textBoxTargetFolder.Size = new Size(487, 23);
            textBoxTargetFolder.TabIndex = 1;
            textBoxTargetFolder.Text = "C:\\Temp";
            // 
            // BtnChangeTargetFolder
            // 
            BtnChangeTargetFolder.Location = new Point(499, 17);
            BtnChangeTargetFolder.Name = "BtnChangeTargetFolder";
            BtnChangeTargetFolder.Size = new Size(92, 31);
            BtnChangeTargetFolder.TabIndex = 0;
            BtnChangeTargetFolder.Text = "Change..";
            BtnChangeTargetFolder.UseVisualStyleBackColor = true;
            BtnChangeTargetFolder.Click += BtnChangeTargetFolder_Click;
            // 
            // BtnSaveSettings
            // 
            BtnSaveSettings.Location = new Point(615, 277);
            BtnSaveSettings.Name = "BtnSaveSettings";
            BtnSaveSettings.Size = new Size(130, 31);
            BtnSaveSettings.TabIndex = 4;
            BtnSaveSettings.Text = "Save";
            BtnSaveSettings.UseVisualStyleBackColor = true;
            BtnSaveSettings.Click += BtnSaveSettings_Click;
            // 
            // BtnIntegrityCheck
            // 
            BtnIntegrityCheck.Location = new Point(615, 152);
            BtnIntegrityCheck.Name = "BtnIntegrityCheck";
            BtnIntegrityCheck.Size = new Size(130, 41);
            BtnIntegrityCheck.TabIndex = 5;
            BtnIntegrityCheck.Text = "Integrity check";
            BtnIntegrityCheck.UseVisualStyleBackColor = true;
            BtnIntegrityCheck.Click += BtnIntegrityCheck_Click;
            // 
            // linkLabelWebsite
            // 
            linkLabelWebsite.AutoSize = true;
            linkLabelWebsite.Location = new Point(700, 398);
            linkLabelWebsite.Name = "linkLabelWebsite";
            linkLabelWebsite.Size = new Size(45, 15);
            linkLabelWebsite.TabIndex = 6;
            linkLabelWebsite.TabStop = true;
            linkLabelWebsite.Text = "dries.jp";
            linkLabelWebsite.LinkClicked += linkLabelWebsite_LinkClicked;
            // 
            // serviceStatusTimer
            // 
            serviceStatusTimer.Interval = 500;
            // 
            // BtnOpenLogs
            // 
            BtnOpenLogs.Location = new Point(634, 360);
            BtnOpenLogs.Name = "BtnOpenLogs";
            BtnOpenLogs.Size = new Size(75, 23);
            BtnOpenLogs.TabIndex = 7;
            BtnOpenLogs.Text = "Open logs";
            BtnOpenLogs.UseVisualStyleBackColor = true;
            BtnOpenLogs.Click += BtnOpenLogs_Click;
            // 
            // IntegrityCheckProgress
            // 
            IntegrityCheckProgress.Location = new Point(618, 199);
            IntegrityCheckProgress.Name = "IntegrityCheckProgress";
            IntegrityCheckProgress.Size = new Size(126, 23);
            IntegrityCheckProgress.TabIndex = 8;
            IntegrityCheckProgress.Visible = false;
            // 
            // lblSettingsSaved
            // 
            lblSettingsSaved.AutoSize = true;
            lblSettingsSaved.Location = new Point(636, 337);
            lblSettingsSaved.Name = "lblSettingsSaved";
            lblSettingsSaved.Size = new Size(82, 15);
            lblSettingsSaved.TabIndex = 9;
            lblSettingsSaved.Text = "Settings saved";
            lblSettingsSaved.Visible = false;
            // 
            // savedIndicatorTimer
            // 
            savedIndicatorTimer.Interval = 500;
            savedIndicatorTimer.Tick += savedIndicatorTimer_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(756, 420);
            Controls.Add(lblSettingsSaved);
            Controls.Add(IntegrityCheckProgress);
            Controls.Add(BtnOpenLogs);
            Controls.Add(linkLabelWebsite);
            Controls.Add(BtnIntegrityCheck);
            Controls.Add(BtnSaveSettings);
            Controls.Add(groupBoxTargetFolder);
            Controls.Add(groupBoxFolders);
            Controls.Add(groupBoxService);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            Text = "AutoBackup Config";
            Load += MainForm_Load;
            groupBoxService.ResumeLayout(false);
            groupBoxService.PerformLayout();
            groupBoxFolders.ResumeLayout(false);
            groupBoxTargetFolder.ResumeLayout(false);
            groupBoxTargetFolder.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnServiceStart;
        private GroupBox groupBoxService;
        private Label LblServiceStatusText;
        private Button BtnServiceStop;
        private Label LblServiceStatus;
        private GroupBox groupBoxFolders;
        private Button BtnFolderRemove;
        private Button BtnFolderAdd;
        private ListBox listBoxFolderSources;
        private GroupBox groupBoxTargetFolder;
        private TextBox textBoxTargetFolder;
        private Button BtnChangeTargetFolder;
        private Button BtnSaveSettings;
        private Button BtnIntegrityCheck;
        private LinkLabel linkLabelWebsite;
        private System.Windows.Forms.Timer serviceStatusTimer;
        private FolderBrowserDialog folderBrowserDialog;
        private Button BtnOpenLogs;
        private ProgressBar IntegrityCheckProgress;
        private Label lblSettingsSaved;
        private System.Windows.Forms.Timer savedIndicatorTimer;
    }
}
