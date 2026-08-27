<div align="center">

# 📠 ScanBridge

### مكتبة .NET احترافية لدمج الماسحات الضوئية (TWAIN) في تطبيقات Windows

[![NuGet](https://img.shields.io/badge/NuGet-ScanBridge-blue?logo=nuget)](https://www.nuget.org/)
[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.6.2%20%7C%204.8-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Platform](https://img.shields.io/badge/Platform-Windows-0078D6?logo=windows)](https://www.microsoft.com/windows)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![TWAIN](https://img.shields.io/badge/Protocol-TWAIN-555555)](https://twain.org)

</div>

---

## 📋 جدول المحتويات

- [نظرة عامة](#-نظرة-عامة)
- [المميزات](#-المميزات)
- [متطلبات النظام](#-متطلبات-النظام)
- [التثبيت](#-التثبيت)
- [الاستخدام السريع](#-الاستخدام-السريع)
- [أمثلة متقدمة](#-أمثلة-متقدمة)
  - [Builder المخصص](#1-builder-المخصص)
  - [الإعدادات المسبقة](#2-الإعدادات-المسبقة-presets)
  - [الطريقة التقليدية بالأحداث](#3-الطريقة-التقليدية-بالأحداث)
  - [الإلغاء التعاوني](#4-الإلغاء-التعاوني-cancellation)
  - [قراءة قدرات الماسح](#5-قراءة-قدرات-الماسح-diagnostics)
  - [التسجيل المخصص](#6-التسجيل-المخصص-logging)
- [معالجة الأخطاء](#-معالجة-الأخطاء)
- [تقرير مصالحة الإعدادات](#-تقرير-مصالحة-الإعدادات-settings-reconciliation)
- [بنية المشروع](#-بنية-المشروع)
- [تطبيق العرض التوضيحي](#-تطبيق-العرض-التوضيحي)
- [المساهمة](#-المساهمة)
- [الرخصة](#-الرخصة)

---

## 🔍 نظرة عامة

**ScanBridge** مكتبة .NET مفتوحة المصدر توفّر طبقة عالية المستوى وسهلة الاستخدام فوق بروتوكول **TWAIN**، لتمكين تطبيقات Windows (WinForms, WPF) من التواصل مع الماسحات الضوئية دون الحاجة للتعامل المباشر مع تعقيدات P/Invoke والـ Data Source Manager (DSM).

تجمع المكتبة بين واجهة **Async/Await** حديثة وواجهة **أحداث تقليدية** للتوافق مع الأنماط القديمة، مع طبقة **Fluent Builder** لبناء إعدادات المسح بشكل قابل للقراءة.

```csharp
using var scanner = new Scanner(messageHook, FileLogger.CreateDefault());
scanner.SelectSource("Canon DR-C225");

using var result = await scanner.ScanAsync(ScanSettingsBuilder.DocumentBlackAndWhite());
result.SaveAll(@"C:\Output");
```

---

## ✨ المميزات

| الميزة | الوصف |
|---|---|
| ⚡ **Async/Await** | واجهة `ScanAsync` حديثة مع دعم كامل لـ `CancellationToken` |
| 🧩 **Fluent Builder** | بناء إعدادات المسح بسلسلة استدعاءات قابلة للقراءة (`ScanSettingsBuilder`) |
| 🎯 **إعدادات مسبقة (Presets)** | وثائق أبيض/أسود، وثائق ملوّنة، ثنائي الوجه، صور فوتوغرافية، مسح سريع |
| 📁 **نقل ملف وذاكرة** | دعم كامل لكلا أسلوبَي النقل في TWAIN (File Transfer / Native Memory Transfer) |
| 🔔 **حدث موحّد** | `PageScanned` حدث واحد يُمثّل كل صفحة ممسوحة بغض النظر عن طريقة النقل |
| 🧪 **تشخيص القدرات** | `ScannerDiagnostics` للاستعلام عن قدرات الماسح والتحقق من توافق الإعدادات قبل البدء |
| 📝 **Logging قابل للحقن** | واجهة `IScanBridgeLogger` للتكامل مع Serilog, NLog, Microsoft.Extensions.Logging أو أي نظام آخر |
| ⚠️ **استثناءات متخصصة** | `ScannerException`, `DeviceOpenException`, `FeederEmptyException` لمعالجة دقيقة للأخطاء |
| 📊 **تقرير مصالحة الإعدادات** | كل مسح يُرفَق تلقائياً بـ `SettingsReport` يوضح أي إعداد طُبِّق فعلياً وأيّها تُخطِّي أو رُفض — لا فشل صامت |
| 🖥️ **دعم .NET Framework** | يعمل على `net462` و `net48` |

---

## 💻 متطلبات النظام

- Windows 7 أو أحدث
- .NET Framework 4.6.2 أو 4.8
- TWAIN Data Source Manager مثبّت على الجهاز (`twain_32.dll` أو `TWAINDSM.dll`)
- ماسح ضوئي يدعم معيار **TWAIN**

---

## 📦 التثبيت

```bash
dotnet add package ScanBridge
```

أو عبر NuGet Package Manager في Visual Studio:

```
Install-Package ScanBridge
```

---

## 🚀 الاستخدام السريع

```csharp
using ScanBridge;
using ScanBridge.Fluent;
using ScanBridge.Logging;

// إنشاء جلسة TWAIN (messageHook غالباً يُنفَّذ من نافذة WinForms المضيفة)
using var scanner = new Scanner(messageHook, FileLogger.CreateDefault());

// اختيار الماسح
scanner.SelectSource("Canon DR-C225");
// أو: scanner.SelectSource(); // يعرض مربع حوار الاختيار الافتراضي

// مسح وثائق بالأبيض والأسود (300 DPI، PDF)
using var result = await scanner.ScanAsync(
    ScanSettingsBuilder.DocumentBlackAndWhite());

// حفظ الصفحات على القرص
var savedFiles = result.SaveAll(@"C:\Output", ".pdf");
Console.WriteLine($"تم مسح {result.PageCount} صفحة بنجاح.");
```

---

## 🛠 أمثلة متقدمة

### 1) Builder المخصص

```csharp
var settings = ScanSettingsBuilder.New()
    .Resolution(300)
    .Colour()
    .WithDocumentFeeder()
    .Duplex()
    .PageSize(PageType.A4)
    .SaveAsPdf()
    .Build();

using var result = await scanner.ScanAsync(settings, cancellationToken);
```

### 2) الإعدادات المسبقة (Presets)

```csharp
ScanSettingsBuilder.DocumentBlackAndWhite() // 300 DPI، أبيض وأسود، PDF
ScanSettingsBuilder.DocumentColour()        // 200 DPI، ملوّن، JPEG
ScanSettingsBuilder.DocumentDuplex()        // 300 DPI، ملوّن، PDF، وجهين
ScanSettingsBuilder.Photo()                 // 600 DPI، PNG
ScanSettingsBuilder.QuickScan()             // 150 DPI، مسح سريع
```

### 3) الطريقة التقليدية بالأحداث

حدث واحد `PageScanned` يغطي كل أنواع النقل — تحقق من `e.Page.HasFile` أو `e.Page.HasBitmap` لمعرفة شكل البيانات الواردة:

```csharp
scanner.PageScanned += (s, e) =>
{
    if (e.Page.HasFile)
        Console.WriteLine($"صفحة {e.PageNumber} حُفظت في: {e.Page.FilePath}");
    else
        e.Page.GetBitmap().Save($@"C:\Scans\Page_{e.PageNumber}.jpg");

    e.ContinueScanning = true;
};

scanner.ScanningComplete += (s, e) =>
{
    if (e.Exception != null)
        Console.WriteLine($"خطأ: {e.Exception.Message}");
    else
        Console.WriteLine("اكتمل المسح!");
};

scanner.StartScanning(settings);
```

### 4) الإلغاء التعاوني (Cancellation)

```csharp
var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

try
{
    using var result = await scanner.ScanAsync(settings, cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("تم إلغاء المسح.");
}
```

### 5) قراءة قدرات الماسح (Diagnostics)

تحقّق من القدرات المدعومة قبل بدء المسح، دون الحاجة لإنشاء جلسة `Scanner` كاملة:

```csharp
using var diag = new ScannerDiagnostics(messageHook);

bool hasFeeder = diag.Capabilities.IsSupported(Capabilities.CapFeederLoaded);

foreach (var line in diag.GetCapabilitiesReport())
    Console.WriteLine(line);

var warnings = diag.ValidateSettings(settings);
```

### 6) التسجيل المخصص (Logging)

```csharp
// تسجيل ملفات يومية جاهز للاستخدام
var logger = FileLogger.CreateDefault(FileLogger.LogLevel.Info);
using var scanner = new Scanner(messageHook, logger);

// أو تكامل مع نظام تسجيل خارجي
public class SerilogScannerLogger : IScanBridgeLogger
{
    private readonly Serilog.ILogger _log = Log.ForContext<Scanner>();
    public void Debug(string m)              => _log.Debug(m);
    public void Info(string m)               => _log.Information(m);
    public void Warning(string m)            => _log.Warning(m);
    public void Error(string m)              => _log.Error(m);
    public void Error(string m, Exception e) => _log.Error(e, m);
}
```

---

## ⚠️ معالجة الأخطاء

تستخدم المكتبة تسلسلاً هرمياً من الاستثناءات المتخصصة لتسهيل المعالجة الدقيقة:

```
ScannerException                 // الاستثناء الأساسي لجميع أخطاء TWAIN
 ├── DeviceOpenException         // فشل فتح الماسح أو الـ DSM
 └── FeederEmptyException        // وحدة التغذية التلقائية فارغة
```

```csharp
try
{
    using var result = await scanner.ScanAsync(settings);
}
catch (FeederEmptyException)
{
    Console.WriteLine("الرجاء وضع الورق في وحدة التغذية.");
}
catch (DeviceOpenException ex)
{
    Console.WriteLine($"تعذّر فتح الماسح: {ex.Message}");
}
catch (ScannerException ex)
{
    Console.WriteLine($"خطأ TWAIN [{ex.ReturnCode}]: {ex.Message}");
}
```

---

## 📊 تقرير مصالحة الإعدادات (Settings Reconciliation)

**الفرق الجوهري عن مكتبات TWAIN الأخرى:** أغلب المكتبات، إذا رفض الجهاز إعداداً غير مدعوم
(مثل طلب Duplex على ماسح لا يدعمه)، تتجاهل الأمر بصمت أو تُلقي استثناءً عاماً يُوقف كل شيء.
ScanBridge بدلاً من ذلك يُرفق **كل مسح** بتقرير مصالحة صريح يوضح مصير كل إعداد طلبته فعلياً.

```csharp
using var result = await scanner.ScanAsync(
    ScanSettingsBuilder.New().Duplex(true).Resolution(1200).Build());

if (!result.SettingsReport.AllApplied)
{
    foreach (var missed in result.SettingsReport.NotApplied)
        Console.WriteLine($"⚠ {missed}");
    // مثال: ⚠ Duplex: Skipped — الجهاز المتصل لا يدعم هذا الإعداد.
}
```

### تفاعل فوري قبل بدء المسح

يصل التقرير أيضاً عبر حدث `SettingsApplied` قبل نقل أي صفحة — مفيد لإلغاء المسح
مبكراً أو تنبيه المستخدم فوراً بدل الانتظار حتى النهاية:

```csharp
scanner.SettingsApplied += (s, e) =>
{
    if (e.Report[ScanSettingId.Duplex]?.Outcome == ScanSettingOutcome.Skipped)
        Console.WriteLine("تنبيه: هذا الماسح لا يدعم الوجهين، سيُكمل بوجه واحد.");
};
```

### فحص مُسبق دون فتح مسح كامل

`ScannerDiagnostics.ValidateSettings()` تُشارك **نفس** منطق التفاوض مع الجهاز
المُستخدَم فعلياً عند المسح (وليس تخميناً منفصلاً قد ينحرف عنه):

```csharp
using var diag = new ScannerDiagnostics(messageHook);
var prediction = diag.ValidateSettings(settings);

foreach (var p in prediction.Where(r => r.Outcome != ScanSettingOutcome.Applied))
    Console.WriteLine(p);
```

## 🗂 بنية المشروع

الحل (Solution) مقسَّم إلى مشروعين منفصلين تحت مجلد جذر واحد:

```
ScanBridge/                  ← جذر المستودع
├── ScanBridge.sln           ← ملف الحل، يجمع المشروعين أدناه
├── README.md
├── CHANGELOG.md
│
├── ScanBridge/               ← مشروع المكتبة (ScanBridge.csproj)
│   ├── Core/                 — Scanner, DataSource, DataSourceManager
│   ├── Fluent/                — ScanSettingsBuilder
│   ├── Models/                 — ScannedPage, ScanResult
│   ├── Extensions/              — اختصارات شائعة فوق Scanner (QuickScanAsync...)
│   ├── Logging/                  — IScanBridgeLogger, FileLogger, NullLogger
│   ├── Settings/                  — ScanSettings, ScanSettingsApplier, SettingsApplyReport
│   ├── Transfer/                   — TransferCoordinator وأوامر النقل
│   ├── Native/                      — أنواع TWAIN الأصلية (structs, enums, constants)
│   ├── Interop/                      — P/Invoke: Win32, GDI32, Kernel32, DSM
│   ├── Capabilities/                  — DeviceCapabilitiesFacade وقراءة/كتابة القدرات
│   ├── Exceptions/                     — ScannerException وفروعها
│   └── Hooks/                           — IMessageHook, WinFormsMessageHook
│
└── demo/                      ← مشروع العرض التوضيحي (ScanBridge.Demo.csproj)
    └── MainForm.cs, Program.cs
```

---

## 🖼 تطبيق العرض التوضيحي

يحتوي الحل على تطبيق WinForms كامل (`demo/`) يوضّح:

- اختيار المصدر وعرض قائمة الماسحات المتاحة
- قراءة قدرات الماسح عبر `ScannerDiagnostics`
- المسح بطريقة `Async/Await` مع إلغاء تعاوني
- المسح بالأحداث التقليدية (`PageScanned`)
- حفظ الصفحات وفتح مجلد الإخراج

```bash
cd demo
dotnet run
```

أو من جذر المستودع مباشرة:

```bash
dotnet run --project demo/ScanBridge.Demo.csproj
```

---

## 🤝 المساهمة

المساهمات مرحَّب بها! للمساهمة:

1. Fork المستودع
2. أنشئ فرعاً جديداً (`git checkout -b feature/my-feature`)
3. التزم بنمط الكود الحالي وأضف تعليقات XML للأعضاء العامة الجديدة
4. تأكد من بناء المشروع بدون أخطاء أو تحذيرات على كلا الإطارين `net462` و `net48`
5. افتح Pull Request مع وصف واضح للتغيير

للمشاكل والاقتراحات، يُرجى فتح **Issue** جديد على المستودع.

---

<div align="center">

صُنعت بـ ❤️ لمطوّري Windows الذين يحتاجون دمج الماسحات الضوئية دون صداع TWAIN.

</div>
