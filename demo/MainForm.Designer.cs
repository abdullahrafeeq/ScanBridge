namespace ScanBridge.Demo
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.GroupBox     grpSource;
        private System.Windows.Forms.ComboBox     cmbSources;
        private System.Windows.Forms.Button       btnSelectSource;
        private System.Windows.Forms.Button       btnReadCapabilities;

        private System.Windows.Forms.Panel        pnlSettings;
        private System.Windows.Forms.GroupBox     grpSettings;
        private System.Windows.Forms.ComboBox     cmbResolution;
        private System.Windows.Forms.Label        lblResolution;
        private System.Windows.Forms.CheckBox     chkBlackAndWhite;
        private System.Windows.Forms.CheckBox     chkDuplex;

        private System.Windows.Forms.GroupBox     grpActions;
        private System.Windows.Forms.Button       btnScanAsync;
        private System.Windows.Forms.Button       btnScanEvents;
        private System.Windows.Forms.Button       btnCancel;

        private System.Windows.Forms.GroupBox     grpLog;
        private System.Windows.Forms.RichTextBox  txtLog;
        private System.Windows.Forms.Button       btnClearLog;
        private System.Windows.Forms.Button       btnOpenFolder;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // النموذج
            this.Text          = "ScanBridge — نموذج عرض احترافي";
            this.Size          = new System.Drawing.Size(780, 620);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font          = new System.Drawing.Font("Segoe UI", 9f);

            // ---- grpSource ----
            grpSource = new System.Windows.Forms.GroupBox
            {
                Text     = "مصدر TWAIN",
                Location = new System.Drawing.Point(12, 12),
                Size     = new System.Drawing.Size(740, 60),
                Anchor   = System.Windows.Forms.AnchorStyles.Top |
                           System.Windows.Forms.AnchorStyles.Left |
                           System.Windows.Forms.AnchorStyles.Right
            };
            cmbSources = new System.Windows.Forms.ComboBox
            { Location = new System.Drawing.Point(10, 25), Size = new System.Drawing.Size(350, 23), DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
            btnSelectSource = new System.Windows.Forms.Button
            { Text = "اختيار...", Location = new System.Drawing.Point(370, 23), Size = new System.Drawing.Size(90, 26) };
            btnReadCapabilities = new System.Windows.Forms.Button
            { Text = "القدرات", Location = new System.Drawing.Point(470, 23), Size = new System.Drawing.Size(90, 26) };
            btnSelectSource.Click       += btnSelectSource_Click;
            btnReadCapabilities.Click   += btnReadCapabilities_Click;
            grpSource.Controls.AddRange(new System.Windows.Forms.Control[]
                { cmbSources, btnSelectSource, btnReadCapabilities });

            // ---- pnlSettings / grpSettings ----
            pnlSettings = new System.Windows.Forms.Panel
            { Location = new System.Drawing.Point(12, 82), Size = new System.Drawing.Size(740, 80) };
            grpSettings = new System.Windows.Forms.GroupBox
            { Text = "إعدادات المسح", Dock = System.Windows.Forms.DockStyle.Fill };
            lblResolution = new System.Windows.Forms.Label
            { Text = "الدقة (DPI):", Location = new System.Drawing.Point(10, 28), Size = new System.Drawing.Size(75, 20) };
            cmbResolution = new System.Windows.Forms.ComboBox
            { Location = new System.Drawing.Point(90, 25), Size = new System.Drawing.Size(100, 23), DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
            cmbResolution.Items.AddRange(new object[] { "100","150","200","300","600" });
            cmbResolution.SelectedIndex = 3; // 300
            chkBlackAndWhite = new System.Windows.Forms.CheckBox
            { Text = "أبيض وأسود", Location = new System.Drawing.Point(210, 27), AutoSize = true };
            chkDuplex = new System.Windows.Forms.CheckBox
            { Text = "مسح على الوجهين", Location = new System.Drawing.Point(320, 27), AutoSize = true };
            cmbResolution.SelectedIndexChanged += cmbResolution_SelectedIndexChanged;
            chkBlackAndWhite.CheckedChanged    += chkBlackAndWhite_CheckedChanged;
            chkDuplex.CheckedChanged           += chkDuplex_CheckedChanged;
            grpSettings.Controls.AddRange(new System.Windows.Forms.Control[]
                { lblResolution, cmbResolution, chkBlackAndWhite, chkDuplex });
            pnlSettings.Controls.Add(grpSettings);

            // ---- grpActions ----
            grpActions = new System.Windows.Forms.GroupBox
            {
                Text     = "عمليات المسح",
                Location = new System.Drawing.Point(12, 172),
                Size     = new System.Drawing.Size(740, 55)
            };
            btnScanAsync = new System.Windows.Forms.Button
            { Text = "🔄 مسح Async", Location = new System.Drawing.Point(10, 20), Size = new System.Drawing.Size(130, 28),
              BackColor = System.Drawing.Color.FromArgb(0, 120, 215), ForeColor = System.Drawing.Color.White, FlatStyle = System.Windows.Forms.FlatStyle.Flat };
            btnScanEvents = new System.Windows.Forms.Button
            { Text = "📡 مسح بالأحداث", Location = new System.Drawing.Point(150, 20), Size = new System.Drawing.Size(130, 28) };
            btnCancel = new System.Windows.Forms.Button
            { Text = "🛑 إلغاء", Location = new System.Drawing.Point(290, 20), Size = new System.Drawing.Size(90, 28),
              Enabled = false, BackColor = System.Drawing.Color.FromArgb(232, 17, 35), ForeColor = System.Drawing.Color.White, FlatStyle = System.Windows.Forms.FlatStyle.Flat };
            btnScanAsync.Click  += btnScanAsync_Click;
            btnScanEvents.Click += btnScanEvents_Click;
            btnCancel.Click     += btnCancel_Click;
            grpActions.Controls.AddRange(new System.Windows.Forms.Control[]
                { btnScanAsync, btnScanEvents, btnCancel });

            // ---- grpLog ----
            grpLog = new System.Windows.Forms.GroupBox
            {
                Text     = "سجل العمليات",
                Location = new System.Drawing.Point(12, 237),
                Size     = new System.Drawing.Size(740, 330),
                Anchor   = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom |
                           System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right
            };
            txtLog = new System.Windows.Forms.RichTextBox
            { Location = new System.Drawing.Point(10, 20), Size = new System.Drawing.Size(720, 265),
              ReadOnly = true, BackColor = System.Drawing.Color.FromArgb(30, 30, 30),
              ForeColor = System.Drawing.Color.FromArgb(200, 220, 200),
              Font = new System.Drawing.Font("Consolas", 9f),
              Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom |
                       System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right };
            btnClearLog = new System.Windows.Forms.Button
            { Text = "مسح السجل", Location = new System.Drawing.Point(10, 295), Size = new System.Drawing.Size(100, 26) };
            btnOpenFolder = new System.Windows.Forms.Button
            { Text = "📁 فتح مجلد المسح", Location = new System.Drawing.Point(120, 295), Size = new System.Drawing.Size(140, 26) };
            btnClearLog.Click   += btnClearLog_Click;
            btnOpenFolder.Click += btnOpenFolder_Click;
            grpLog.Controls.AddRange(new System.Windows.Forms.Control[]
                { txtLog, btnClearLog, btnOpenFolder });

            // ---- إضافة للنموذج ----
            this.Controls.AddRange(new System.Windows.Forms.Control[]
                { grpSource, pnlSettings, grpActions, grpLog });

            this.ResumeLayout(false);
        }
    }
}
