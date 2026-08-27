using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScanBridge;
using ScanBridge.Fluent;
using ScanBridge.Models;

namespace ScanBridge.Demo
{
    /// <summary>
    /// نموذج عرض احترافي لمكتبة ScanBridge.
    /// يوضح:
    /// - اختيار المصدر
    /// - قراءة قدرات الماسح
    /// - المسح بالأحداث التقليدية
    /// - المسح بـ Async/Await
    /// - الإلغاء
    /// - حفظ الصور
    /// </summary>
    public partial class MainForm : Form
    {
        private Scanner                   _scanner;
        private WinFormsMessageHook _hook;
        private ScanSettings              _settings;
        private CancellationTokenSource   _cts;
        private readonly List<string>     _scannedFiles = new List<string>();

        // ================================================================
        // البناء والتهيئة
        // ================================================================

        public MainForm()
        {
            InitializeComponent();
            InitializeSettings();
        }

        private void InitializeSettings()
        {
            _settings = ScanSettingsBuilder.New()
                .Resolution(300)
                .Colour()
                .WithDocumentFeeder()
                .SaveAsJpeg()
                .Build();
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await InitializeTwainAsync();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _cts?.Cancel();
            _scanner?.Dispose();
            _hook?.Dispose();
            base.OnFormClosing(e);
        }

        // ================================================================
        // تهيئة TWAIN
        // ================================================================

        private async Task InitializeTwainAsync()
        {
            try
            {
                _hook  = new WinFormsMessageHook(this);
                _scanner = new Scanner(_hook,
                    Logging.FileLogger.CreateDefault(
                        Logging.FileLogger.LogLevel.Info));

                await Task.Delay(100); // انتظر نافذة
                PopulateSourceList();
                UpdateUIState(ready: true);
                LogMessage("✅ TWAIN جاهز.");
            }
            catch (Exception ex)
            {
                LogMessage($"❌ خطأ في التهيئة: {ex.Message}");
                UpdateUIState(ready: false);
            }
        }

        private void PopulateSourceList()
        {
            cmbSources.Items.Clear();
            foreach (var name in _scanner.SourceNames)
                cmbSources.Items.Add(name);

            if (cmbSources.Items.Count > 0)
                cmbSources.SelectedIndex = 0;
        }

        // ================================================================
        // اختيار المصدر وقراءة القدرات
        // ================================================================

        private void btnSelectSource_Click(object sender, EventArgs e)
        {
            bool selected = _scanner.SelectSource();
            if (!selected)
            {
                LogMessage("ℹ️  ألغى المستخدم اختيار المصدر.");
                return;
            }
            LogMessage($"✅ مصدر مختار: {_scanner.DefaultSourceName}");
        }

        private async void btnReadCapabilities_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateUIState(scanning: true);
                LogMessage("📋 قراءة قدرات الماسح...");

                using (var diag = new ScannerDiagnostics(_hook))
                {
                    var report = diag.GetCapabilitiesReport();
                    foreach (var line in report)
                        LogMessage(line);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ فشل قراءة القدرات: {ex.Message}");
            }
            finally
            {
                UpdateUIState(ready: true);
            }
        }

        // ================================================================
        // المسح بـ Async/Await (الطريقة الموصى بها)
        // ================================================================

        private async void btnScanAsync_Click(object sender, EventArgs e)
        {
            _cts = new CancellationTokenSource();
            UpdateUIState(scanning: true);
            LogMessage("🔄 بدء المسح...");
            _scannedFiles.Clear();

            try
            {
                string outputDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "ScanBridgeOutput");

                using var result = await _scanner.ScanAsync(_settings, _cts.Token);

                if (result.IsEmpty)
                {
                    LogMessage("⚠️  لم تُمسح أي صفحة.");
                    return;
                }

                var savedPaths = result.SaveAll(outputDir, ".jpg");
                foreach (var path in savedPaths)
                {
                    _scannedFiles.Add(path);
                    LogMessage($"  💾 {Path.GetFileName(path)}");
                }

                LogMessage($"✅ اكتمل المسح — {result.PageCount} صفحة في:\n   {outputDir}");
            }
            catch (OperationCanceledException)
            {
                LogMessage("🛑 تم إلغاء المسح.");
            }
            catch (Exception ex)
            {
                LogMessage($"❌ خطأ: {ex.Message}");
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
                UpdateUIState(ready: true);
            }
        }

        // ================================================================
        // المسح بالأحداث التقليدية (للتوافق مع الكود القديم)
        // ================================================================

        private void btnScanEvents_Click(object sender, EventArgs e)
        {
            UpdateUIState(scanning: true);
            LogMessage("🔄 مسح بالأحداث...");
            _scannedFiles.Clear();

            // الاشتراك في الأحداث
            _scanner.PageScanned     += OnPageScanned;
            _scanner.ScanningComplete += OnScanningComplete;

            try
            {
                _scanner.StartScanning(_settings);
            }
            catch (Exception ex)
            {
                LogMessage($"❌ فشل بدء المسح: {ex.Message}");
                UnsubscribeEvents();
                UpdateUIState(ready: true);
            }
        }

        private void OnPageScanned(object sender, PageScannedEventArgs e)
        {
            if (InvokeRequired) { Invoke(new Action(() => OnPageScanned(sender, e))); return; }

            try
            {
                if (e.Page.HasFile)
                {
                    // نقل ملف
                    _scannedFiles.Add(e.Page.FilePath);
                    LogMessage($"  📄 صفحة {e.PageNumber}: {Path.GetFileName(e.Page.FilePath)}");
                }
                else
                {
                    // Bitmap في الذاكرة → احفظه مؤقتاً
                    string dest = Path.Combine(
                        Path.GetTempPath(), "ScanBridgeOutput",
                        $"Scan_{e.PageNumber}_{DateTime.Now:HHmmss}.jpg");
                    Directory.CreateDirectory(Path.GetDirectoryName(dest));
                    e.Page.Save(dest);
                    _scannedFiles.Add(dest);
                    LogMessage($"  🖼️  صفحة {e.PageNumber}: {Path.GetFileName(dest)}");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"  ❌ فشل معالجة الصفحة {e.PageNumber}: {ex.Message}");
            }

            e.ContinueScanning = true;
        }

        private void OnScanningComplete(object sender, ScanningCompleteEventArgs e)
        {
            if (InvokeRequired) { Invoke(new Action(() => OnScanningComplete(sender, e))); return; }

            UnsubscribeEvents();

            if (e.Exception != null)
                LogMessage($"❌ خطأ في المسح: {e.Exception.Message}");
            else
                LogMessage($"✅ اكتمل المسح — {_scannedFiles.Count} صفحة");

            UpdateUIState(ready: true);
        }

        private void UnsubscribeEvents()
        {
            _scanner.PageScanned     -= OnPageScanned;
            _scanner.ScanningComplete -= OnScanningComplete;
        }

        // ================================================================
        // الإلغاء
        // ================================================================

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_cts != null)
            {
                _cts.Cancel();
                LogMessage("🛑 طلب إلغاء المسح...");
            }
            else
            {
                _scanner.IsAbortRequested = true;
                LogMessage("🛑 طلب إيقاف المسح...");
            }
        }

        // ================================================================
        // الإعدادات
        // ================================================================

        private void cmbResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(cmbResolution.SelectedItem?.ToString(), out int dpi))
                _settings = ScanSettingsBuilder.New()
                    .Resolution(dpi)
                    .Colour()
                    .WithDocumentFeeder()
                    .SaveAsJpeg()
                    .Build();
        }

        private void chkBlackAndWhite_CheckedChanged(object sender, EventArgs e)
        {
            var builder = ScanSettingsBuilder.New()
                .Resolution(300)
                .WithDocumentFeeder()
                .SaveAsJpeg();

            _settings = (chkBlackAndWhite.Checked ? builder.BlackAndWhite() : builder.Colour())
                .Build();
        }

        private void chkDuplex_CheckedChanged(object sender, EventArgs e)
        {
            _settings = ScanSettingsBuilder.New()
                .Resolution(300)
                .Colour()
                .WithDocumentFeeder()
                .Duplex(chkDuplex.Checked)
                .SaveAsJpeg()
                .Build();
        }

        // ================================================================
        // مساعدات واجهة المستخدم
        // ================================================================

        private void UpdateUIState(bool ready = false, bool scanning = false)
        {
            if (ready)
            {
                btnScanAsync.Enabled   = true;
                btnScanEvents.Enabled  = true;
                btnCancel.Enabled      = false;
                btnSelectSource.Enabled = true;
                btnReadCapabilities.Enabled = true;
                pnlSettings.Enabled   = true;
            }
            else if (scanning)
            {
                btnScanAsync.Enabled   = false;
                btnScanEvents.Enabled  = false;
                btnCancel.Enabled      = true;
                btnSelectSource.Enabled = false;
                btnReadCapabilities.Enabled = false;
                pnlSettings.Enabled   = false;
            }
        }

        private void LogMessage(string message)
        {
            if (InvokeRequired) { Invoke(new Action(() => LogMessage(message))); return; }
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.ScrollToCaret();
        }

        private void btnClearLog_Click(object sender, EventArgs e) =>
            txtLog.Clear();

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            string dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "ScanBridgeOutput");
            if (Directory.Exists(dir))
                System.Diagnostics.Process.Start("explorer.exe", dir);
        }
    }
}
