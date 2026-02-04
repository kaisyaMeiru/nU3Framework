# Framework Component ¹èÆ÷ ½Ã½ºÅÛ

## °³¿ä

È­¸é ¸ðµâ(`SYS_MODULE_MST`)°ú º°µµ·Î, Framework DLL, °ø¿ë ¶óÀÌºê·¯¸®, ½ÇÇàÆÄÀÏ µîÀ» °ü¸®ÇÏ´Â ½Ã½ºÅÛÀÔ´Ï´Ù.

## ±âÁ¸ ½Ã½ºÅÛ vs È®Àå ½Ã½ºÅÛ

| ±¸ºÐ | È­¸é ¸ðµâ | Framework ÄÄÆ÷³ÍÆ® |
|------|----------|-------------------|
| Å×ÀÌºí | `SYS_MODULE_MST`, `SYS_MODULE_VER` | `SYS_COMPONENT_MST`, `SYS_COMPONENT_VER` |
| ¼³Ä¡ °æ·Î | `Modules/{Category}/{SubSystem}/` | À¯¿¬ (`InstallPath` ÁöÁ¤) |
| ¹èÆ÷ ´ÜÀ§ | DLL (È­¸é Æ÷ÇÔ) | DLL, EXE, ¼³Á¤ÆÄÀÏ µî |
| ·Îµù ¹æ½Ä | ·±Å¸ÀÓ µ¿Àû ·Îµå | ¾Û ½ÃÀÛ Àü »çÀü ¹èÆ÷ |

## DB ½ºÅ°¸¶

### SYS_COMPONENT_MST (ÄÄÆ÷³ÍÆ® ¸¶½ºÅÍ)

```sql
CREATE TABLE SYS_COMPONENT_MST (
    COMPONENT_ID TEXT PRIMARY KEY,     -- ¿¹: "nU3.Core", "DevExpress.XtraEditors"
    COMPONENT_TYPE INTEGER NOT NULL,   -- 0:Screen, 1:Framework, 2:SharedLib, 3:Exe, ...
    COMPONENT_NAME TEXT NOT NULL,      -- Ç¥½Ã¸í
    FILE_NAME TEXT NOT NULL,           -- ÆÄÀÏ¸í (nU3.Core.dll)
    INSTALL_PATH TEXT,                 -- ¼³Ä¡ °æ·Î (»ó´ë°æ·Î, ºó°ª=·çÆ®)
    GROUP_NAME TEXT,                   -- ±×·ì (Framework, DevExpress, Oracle)
    IS_REQUIRED INTEGER DEFAULT 0,     -- ÇÊ¼ö ¿©ºÎ
    AUTO_UPDATE INTEGER DEFAULT 1,     -- ÀÚµ¿ ¾÷µ¥ÀÌÆ®
    DESCRIPTION TEXT,
    PRIORITY INTEGER DEFAULT 100,      -- ¼³Ä¡ ¿ì¼±¼øÀ§ (³·À»¼ö·Ï ¸ÕÀú)
    DEPENDENCIES TEXT,                 -- ÀÇÁ¸¼º (½°Ç¥ ±¸ºÐ)
    REG_DATE TEXT,
    MOD_DATE TEXT,
    IS_ACTIVE TEXT DEFAULT 'Y'
);
```

### SYS_COMPONENT_VER (¹öÀü °ü¸®)

```sql
CREATE TABLE SYS_COMPONENT_VER (
    COMPONENT_ID TEXT,
    VERSION TEXT,
    FILE_HASH TEXT,                    -- SHA256 ÇØ½Ã
    FILE_SIZE INTEGER,
    STORAGE_PATH TEXT,                 -- ¼­¹ö ÀúÀå °æ·Î
    MIN_FRAMEWORK_VER TEXT,            -- ÃÖ¼Ò Framework ¹öÀü
    MAX_FRAMEWORK_VER TEXT,            -- ÃÖ´ë Framework ¹öÀü
    DEPLOY_DESC TEXT,
    RELEASE_NOTE_URL TEXT,
    REG_DATE TEXT,
    DEL_DATE TEXT,                     -- Soft delete
    IS_ACTIVE TEXT DEFAULT 'Y',
    PRIMARY KEY (COMPONENT_ID, VERSION)
);
```

### ComponentType ¿­°ÅÇü

```csharp
public enum ComponentType
{
    ScreenModule = 0,     // È­¸é ¸ðµâ (±âÁ¸ ¹æ½Ä)
    FrameworkCore = 1,    // nU3.Core.dll µî
    SharedLibrary = 2,    // DevExpress, Oracle µî
    Executable = 3,       // nU3.Shell.exe µî
    Configuration = 4,    // appsettings.json µî
    Resource = 5,         // ÀÌ¹ÌÁö, ¾ÆÀÌÄÜ µî
    Plugin = 6,           // ÇÃ·¯±×ÀÎ
    Other = 99
}
```

---

## ¹èÆ÷ Èå¸§

```
¦£¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¤
¦¢                    ¼­¹ö (Deployer Tool)                         ¦¢
¦§¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦©
¦¢  1. DLL/EXE ÆÄÀÏ ¼±ÅÃ                                          ¦¢
¦¢  2. ¸ÞÅ¸µ¥ÀÌÅÍ ÃßÃâ (¹öÀü, ÇØ½Ã)                                ¦¢
¦¢  3. DB µî·Ï (SYS_COMPONENT_MST, SYS_COMPONENT_VER)             ¦¢
¦¢  4. ¼­¹ö ÀúÀå¼Ò¿¡ ÆÄÀÏ º¹»ç                                     ¦¢
¦¦¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¥
                              ¦¢
                              ¡å
¦£¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¤
¦¢                   Å¬¶óÀÌ¾ðÆ® (Bootstrapper)                     ¦¢
¦§¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦©
¦¢  1. DB¿¡¼­ È°¼º ¹öÀü ¸ñ·Ï Á¶È¸                                  ¦¢
¦¢  2. ·ÎÄÃ ¼³Ä¡ ÇöÈ² È®ÀÎ                                         ¦¢
¦¢  3. ¾÷µ¥ÀÌÆ® ÇÊ¿ä ÄÄÆ÷³ÍÆ® ÆÇº°                                 ¦¢
¦¢  4. ¼­¹ö¿¡¼­ ´Ù¿î·Îµå ¡æ Ä³½Ã                                    ¦¢
¦¢  5. ÇØ½Ã °ËÁõ                                                  ¦¢
¦¢  6. ¼³Ä¡ °æ·Î¿¡ º¹»ç                                            ¦¢
¦¦¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¥
```

---

## »ç¿ë ¿¹½Ã

### 1. Deployer¿¡¼­ ÄÄÆ÷³ÍÆ® ¹èÆ÷

```csharp
// ComponentDeployControl¿¡¼­ Smart Deploy
var componentRepo = Program.ServiceProvider.GetRequiredService<IComponentRepository>();

// ´ÜÀÏ ÆÄÀÏ ¹èÆ÷
componentRepo.SaveComponent(new ComponentMstDto
{
    ComponentId = "nU3.Core",
    ComponentName = "nU3 Core Library",
    FileName = "nU3.Core.dll",
    ComponentType = ComponentType.FrameworkCore,
    InstallPath = "",  // ·çÆ®
    GroupName = "Framework",
    IsRequired = true,
    AutoUpdate = true,
    Priority = 10
});

componentRepo.AddVersion(new ComponentVerDto
{
    ComponentId = "nU3.Core",
    Version = "1.0.0.0",
    FileHash = "a1b2c3d4...",
    FileSize = 245760,
    StoragePath = @"D:\ServerStorage\Components\Framework\nU3.Core.dll",
    IsActive = "Y"
});
```

### 2. Bootstrapper¿¡¼­ ¾÷µ¥ÀÌÆ® Ã¼Å©

```csharp
var updateService = new ComponentUpdateService(componentRepo, installPath);

// ¾÷µ¥ÀÌÆ® È®ÀÎ
var updates = updateService.CheckForUpdates();
if (updates.Any())
{
    Console.WriteLine($"{updates.Count}°³ ¾÷µ¥ÀÌÆ® °¡´É");
    
    // ¾÷µ¥ÀÌÆ® ½ÇÇà
    var progress = new Progress<ComponentUpdateProgressEventArgs>(p =>
    {
        Console.WriteLine($"[{p.Phase}] {p.CurrentComponentName} ({p.PercentComplete}%)");
    });
    
    var result = await updateService.UpdateAllAsync(progress, cancellationToken);
    
    if (result.Success)
        Console.WriteLine("¸ðµç ¾÷µ¥ÀÌÆ® ¿Ï·á!");
    else
        Console.WriteLine($"ÀÏºÎ ½ÇÆÐ: {string.Join(", ", result.FailedComponents.Select(f => f.ComponentId))}");
}
```

### 3. WinForms¿¡¼­ ¾÷µ¥ÀÌÆ® UI

```csharp
private async void CheckAndUpdateComponents()
{
    var updateService = new ComponentUpdateService(_componentRepo);
    var updates = updateService.CheckForUpdates();
    
    if (!updates.Any())
    {
        toolStripStatus.Text = "ÃÖ½Å ¹öÀüÀÔ´Ï´Ù.";
        return;
    }
    
    if (MessageBox.Show($"{updates.Count}°³ ¾÷µ¥ÀÌÆ®°¡ ÀÖ½À´Ï´Ù. Áö±Ý ¾÷µ¥ÀÌÆ®ÇÏ½Ã°Ú½À´Ï±î?",
        "¾÷µ¥ÀÌÆ® È®ÀÎ", MessageBoxButtons.YesNo) != DialogResult.Yes)
        return;
    
    try
    {
        var result = await AsyncOperationHelper.ExecuteWithProgressAsync(
            this,
            "ÄÄÆ÷³ÍÆ® ¾÷µ¥ÀÌÆ® Áß...",
            async (ct, progress) =>
            {
                var updateProgress = new Progress<ComponentUpdateProgressEventArgs>(p =>
                {
                    progress.Report(new BatchOperationProgress
                    {
                        TotalItems = p.TotalComponents,
                        CompletedItems = p.CurrentIndex,
                        CurrentItem = p.CurrentComponentName,
                        PercentComplete = p.PercentComplete
                    });
                });
                
                return await updateService.UpdateAllAsync(updateProgress, ct);
            });
        
        if (result.Success)
        {
            MessageBox.Show("¾÷µ¥ÀÌÆ®°¡ ¿Ï·áµÇ¾ú½À´Ï´Ù.\nº¯°æ»çÇ× Àû¿ëÀ» À§ÇØ ÇÁ·Î±×·¥À» Àç½ÃÀÛÇØÁÖ¼¼¿ä.",
                "¿Ï·á", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
    catch (OperationCanceledException)
    {
        MessageBox.Show("¾÷µ¥ÀÌÆ®°¡ Ãë¼ÒµÇ¾ú½À´Ï´Ù.", "Ãë¼Ò");
    }
}
```

---

## ¼³Ä¡ °æ·Î ¿¹½Ã

```
¼³Ä¡ ·çÆ® (¿¹: C:\Program Files\nU3.Shell\)
¦¢
¦§¦¡¦¡ nU3.Shell.exe                 ¡ç InstallPath: ""
¦§¦¡¦¡ nU3.Core.dll                  ¡ç InstallPath: ""
¦§¦¡¦¡ nU3.Core.UI.dll               ¡ç InstallPath: ""
¦§¦¡¦¡ nU3.Connectivity.dll          ¡ç InstallPath: ""
¦§¦¡¦¡ appsettings.json              ¡ç InstallPath: ""
¦¢
¦§¦¡¦¡ DevExpress.Data.dll           ¡ç InstallPath: ""
¦§¦¡¦¡ DevExpress.XtraEditors.dll    ¡ç InstallPath: ""
¦¢
¦§¦¡¦¡ plugins\                      ¡ç InstallPath: "plugins"
¦¢   ¦¦¦¡¦¡ MyPlugin.dll
¦¢
¦§¦¡¦¡ resources\                    ¡ç InstallPath: "resources"
¦¢   ¦¦¦¡¦¡ images\
¦¢       ¦¦¦¡¦¡ logo.png
¦¢
¦¦¦¡¦¡ Modules\                      ¡ç ±âÁ¸ È­¸é ¸ðµâ (º°µµ ½Ã½ºÅÛ)
    ¦¦¦¡¦¡ EMR\
        ¦¦¦¡¦¡ IN\
            ¦¦¦¡¦¡ nU3.Modules.EMR.IN.Worklist.dll
```

---

## ¿ì¼±¼øÀ§ °¡ÀÌµå

| Priority | À¯Çü | ¿¹½Ã |
|----------|------|------|
| 1-10 | ½ÇÇàÆÄÀÏ | nU3.Shell.exe, nU3.Bootstrapper.exe |
| 11-20 | Framework ÇÙ½É | nU3.Core.dll, nU3.Core.UI.dll |
| 21-50 | Framework È®Àå | nU3.Data.dll, nU3.Connectivity.dll |
| 51-80 | ÇÊ¼ö ¶óÀÌºê·¯¸® | Oracle.ManagedDataAccess.dll |
| 81-100 | UI ¶óÀÌºê·¯¸® | DevExpress.*.dll |
| 100+ | ±âÅ¸ | ÇÃ·¯±×ÀÎ, ¸®¼Ò½º |

---

## °ü·Ã ÆÄÀÏ

| ÆÄÀÏ | ¼³¸í |
|------|------|
| `nU3.Models\ModuleModels.cs` | DTO (ComponentMstDto, ComponentVerDto µî) |
| `nU3.Core\Repositories\IComponentRepository.cs` | Repository ÀÎÅÍÆäÀÌ½º |
| `nU3.Data\Repositories\SQLiteComponentRepository.cs` | SQLite ±¸Çö |
| `nU3.Data\LocalDatabaseManager.cs` | DB ½ºÅ°¸¶ (Å×ÀÌºí »ý¼º) |
| `nU3.Core\Services\ComponentUpdateService.cs` | Å¬¶óÀÌ¾ðÆ® ¾÷µ¥ÀÌÆ® ¼­ºñ½º |
| `nU3.Tools.Deployer\Views\ComponentDeployControl.cs` | Deployer UI |
