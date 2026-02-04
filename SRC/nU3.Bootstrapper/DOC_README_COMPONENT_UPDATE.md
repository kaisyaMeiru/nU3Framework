# nU3 Bootstrapper - ¾÷µ¥ÀÌÆ® ÆĞÄ¡ ¸ğµâ

## °³¿ä

`nU3.Bootstrapper`´Â Framework ÄÄÆ÷³ÍÆ®¿Í È­¸é ¸ğµâÀ» ÀÚµ¿À¸·Î ¾÷µ¥ÀÌÆ®ÇÏ°í MainShellÀ» ½ÇÇàÇÏ´Â ·±Ã³ÀÔ´Ï´Ù.

## ÁÖ¿ä ±â´É

### 1. Framework ÄÄÆ÷³ÍÆ® ¾÷µ¥ÀÌÆ® (½Å±Ô)

```
¦£¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¤
¦¢                    Bootstrapper ½ÃÀÛ                         ¦¢
¦¢                          ¦¢                                   ¦¢
¦¢                    DB ÃÊ±âÈ­                                 ¦¢
¦¢                          ¦¢                                   ¦¢
¦¢              Framework ÄÄÆ÷³ÍÆ® È®ÀÎ                         ¦¢
¦¢         (nU3.Core.dll, DevExpress.*.dll µî)                 ¦¢
¦¢                          ¦¢                                   ¦¢
¦¢           ¦£¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦ª¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¤                   ¦¢
¦¢           ¦¢                              ¦¢                   ¦¢
¦¢     ¾÷µ¥ÀÌÆ® ÀÖÀ½                   ÃÖ½Å ¹öÀü                ¦¢
¦¢           ¦¢                              ¦¢                   ¦¢
¦¢     UI Ç¥½Ã + ´Ù¿î·Îµå                   ¦¢                   ¦¢
¦¢           ¦¢                              ¦¢                   ¦¢
¦¢           ¦¦¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¨¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¥                   ¦¢
¦¢                          ¦¢                                   ¦¢
¦¢              È­¸é ¸ğµâ ¾÷µ¥ÀÌÆ® (±âÁ¸)                       ¦¢
¦¢                          ¦¢                                   ¦¢
¦¢                  MainShell ½ÇÇà                              ¦¢
¦¦¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¥
```

### 2. ¾÷µ¥ÀÌÆ® UI

```
¦£¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¤
¦¢ ?? Framework ÄÄÆ÷³ÍÆ® ¾÷µ¥ÀÌÆ®                              ¦¢
¦§¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦©
¦¢ ´Ù¿î·Îµå Áß... (3/10)                                       ¦¢
¦¢ [??????????????????????????????] 30%                        ¦¢
¦¢ DevExpress.XtraEditors                                      ¦¢
¦§¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦©
¦¢ »óÅÂ ¦¢ ÄÄÆ÷³ÍÆ®            ¦¢ ¹öÀü     ¦¢ Å©±â   ¦¢ À¯Çü      ¦¢
¦¢¦¡¦¡¦¡¦¡¦¡¦¡¦«¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦«¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦«¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦«¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¢
¦¢ ?   ¦¢ nU3.Core           ¦¢ 1.0.0.0  ¦¢ 245 KB ¦¢ Core      ¦¢
¦¢ ?   ¦¢ nU3.Core.UI        ¦¢ 1.0.0.0  ¦¢ 189 KB ¦¢ Core      ¦¢
¦¢ ??   ¦¢ DevExpress.XtraEdi ¦¢ 23.2.9   ¦¢ 2.1 MB ¦¢ Library   ¦¢
¦¢ ?   ¦¢ Oracle.Managed...  ¦¢ 21.12.0  ¦¢ 8.5 MB ¦¢ Library   ¦¢
¦§¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦©
¦¢                        [ Ãë¼Ò ]                              ¦¢
¦¦¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¡¦¥
```

## ÆÄÀÏ ±¸Á¶

```
nU3.Bootstrapper/
¦§¦¡¦¡ Program.cs              # ¸ŞÀÎ ÁøÀÔÁ¡
¦§¦¡¦¡ ComponentLoader.cs      # Framework ÄÄÆ÷³ÍÆ® ·Î´õ (½Å±Ô)
¦§¦¡¦¡ ModuleLoader.cs         # È­¸é ¸ğµâ ·Î´õ (±âÁ¸)
¦§¦¡¦¡ UpdateProgressForm.cs   # ¾÷µ¥ÀÌÆ® UI (½Å±Ô)
¦§¦¡¦¡ Seeder.cs               # °³¹ß¿ë Å×½ºÆ® µ¥ÀÌÅÍ
¦¦¦¡¦¡ nU3.Bootstrapper.csproj
```

## Å¬·¡½º ¼³¸í

### ComponentLoader

Framework ÄÄÆ÷³ÍÆ® (DLL, EXE) ¾÷µ¥ÀÌÆ® ´ã´ç:

```csharp
var loader = new ComponentLoader(dbManager, installPath);

// 1. ¾÷µ¥ÀÌÆ® È®ÀÎ
var updates = loader.CheckForUpdates();

// 2. ÇÊ¼ö ÄÄÆ÷³ÍÆ® ´©¶ô È®ÀÎ
var missing = loader.GetMissingRequiredComponents();

// 3. ÀüÃ¼ ¾÷µ¥ÀÌÆ®
var result = loader.UpdateAll();

// 4. ÁøÇà ÀÌº¥Æ®
loader.UpdateProgress += (s, e) => 
{
    Console.WriteLine($"{e.Phase}: {e.ComponentName} ({e.PercentComplete}%)");
};
```

### UpdateProgressForm

¾÷µ¥ÀÌÆ® ÁøÇà UI:

```csharp
using var form = new UpdateProgressForm();

// ¾÷µ¥ÀÌÆ® ¸ñ·Ï ÃÊ±âÈ­
form.InitializeUpdateList(updates);

// ÁøÇà »óÅÂ ¾÷µ¥ÀÌÆ®
loader.UpdateProgress += (s, e) => form.UpdateProgress(e);

// °á°ú Ç¥½Ã
form.ShowResult(result);

form.ShowDialog();
```

### ModuleLoader (±âÁ¸)

È­¸é ¸ğµâ (DLL with screens) ¾÷µ¥ÀÌÆ®:

```csharp
var moduleLoader = new ModuleLoader();
moduleLoader.EnsureDatabaseInitialized();
moduleLoader.CheckAndLoadModules(shellPath);
```

## ½ÇÇà Èå¸§

### 1. Á¤»ó ½Ã³ª¸®¿À

```
1. nU3.Bootstrapper.exe ½ÇÇà
   ¡é
2. DB ÃÊ±âÈ­ (SYS_COMPONENT_MST, SYS_MODULE_MST µî)
   ¡é
3. Shell °æ·Î Ã£±â (nU3.MainShell.exe)
   ¡é
4. Framework ÄÄÆ÷³ÍÆ® ¾÷µ¥ÀÌÆ® È®ÀÎ
   - ¼­¹ö ¹öÀü vs ·ÎÄÃ ¹öÀü ºñ±³
   - ÇØ½Ã ºñ±³·Î º¯°æ °¨Áö
   ¡é
5. ¾÷µ¥ÀÌÆ® ÀÖÀ¸¸é UI Ç¥½Ã + ´Ù¿î·Îµå
   - ¼­¹ö ÀúÀå¼Ò ¡æ Ä³½Ã ¡æ ¼³Ä¡ °æ·Î
   ¡é
6. È­¸é ¸ğµâ ¾÷µ¥ÀÌÆ®
   ¡é
7. MainShell ½ÇÇà
```

### 2. ¾÷µ¥ÀÌÆ® Ãë¼Ò ½Ã

- ÇÊ¼ö ÄÄÆ÷³ÍÆ® ´©¶ô ¿©ºÎ È®ÀÎ
- ´©¶ô ½Ã: °æ°í ¸Ş½ÃÁö Ç¥½Ã ÈÄ Á¾·á
- ´©¶ô ¾øÀ½: Shell ½ÇÇà °è¼Ó

### 3. ¿À·ù ¹ß»ı ½Ã

- ÆÄÀÏ Àá±İ: 3È¸ Àç½Ãµµ ÈÄ ½ÇÆĞ Ã³¸®
- ³×Æ®¿öÅ© ¿À·ù: ½ÇÆĞ ¸ñ·Ï¿¡ Ãß°¡, °è¼Ó ÁøÇà
- ÇÊ¼ö ÄÄÆ÷³ÍÆ® ½ÇÆĞ: °æ°í ÈÄ Á¾·á

## °æ·Î ¼³Á¤

### ¼­¹ö ÀúÀå¼Ò (¹èÆ÷ ¿øº»)

```
%AppData%\nU3.Framework\ServerStorage\Components\
¦§¦¡¦¡ Framework\
¦¢   ¦§¦¡¦¡ nU3.Core.dll
¦¢   ¦¦¦¡¦¡ nU3.Core.UI.dll
¦§¦¡¦¡ DevExpress\
¦¢   ¦¦¦¡¦¡ DevExpress.XtraEditors.dll
¦¦¦¡¦¡ Oracle\
    ¦¦¦¡¦¡ Oracle.ManagedDataAccess.dll
```

### Ä³½Ã (´Ù¿î·Îµå ¿µ¿ª)

```
%AppData%\nU3.Framework\Cache\Components\
¦§¦¡¦¡ Framework\
¦§¦¡¦¡ DevExpress\
¦¦¦¡¦¡ Oracle\
```

### ¼³Ä¡ °æ·Î (·±Å¸ÀÓ)

```
C:\Program Files\nU3\              ¡ç Shell À§Ä¡ ±âÁØ
¦§¦¡¦¡ nU3.MainShell.exe
¦§¦¡¦¡ nU3.Core.dll                   ¡ç InstallPath: ""
¦§¦¡¦¡ nU3.Core.UI.dll
¦§¦¡¦¡ plugins\                       ¡ç InstallPath: "plugins"
¦¢   ¦¦¦¡¦¡ MyPlugin.dll
¦¦¦¡¦¡ Modules\                       ¡ç È­¸é ¸ğµâ (ModuleLoader)
    ¦¦¦¡¦¡ EMR\IN\
        ¦¦¦¡¦¡ nU3.Modules.EMR.IN.Worklist.dll
```

## ÀÌº¥Æ® ¸ğµ¨

### UpdateProgress ÀÌº¥Æ®

```csharp
public class ComponentUpdateEventArgs : EventArgs
{
    public UpdatePhase Phase { get; set; }      // Checking, Downloading, Installing, Completed, Failed
    public string ComponentId { get; set; }
    public string ComponentName { get; set; }
    public int CurrentIndex { get; set; }
    public int TotalCount { get; set; }
    public int PercentComplete { get; set; }
    public string ErrorMessage { get; set; }
}
```

### UpdatePhase ¿­°ÅÇü

| Phase | ¼³¸í |
|-------|------|
| `Checking` | ¾÷µ¥ÀÌÆ® È®ÀÎ Áß |
| `Downloading` | ¼­¹ö¿¡¼­ Ä³½Ã·Î ´Ù¿î·Îµå |
| `Installing` | Ä³½Ã¿¡¼­ ¼³Ä¡ °æ·Î·Î º¹»ç |
| `Completed` | ¾÷µ¥ÀÌÆ® ¿Ï·á |
| `Failed` | ¾÷µ¥ÀÌÆ® ½ÇÆĞ |

## ¹èÆ÷ ½Ã³ª¸®¿À

### °³¹ß È¯°æ

```bash
# Bootstrapper ºôµå ÈÄ ½ÇÇà
dotnet run --project nU3.Bootstrapper

# ÀÚµ¿À¸·Î:
# 1. DB ÃÊ±âÈ­
# 2. Å×½ºÆ® µ¥ÀÌÅÍ ½Ãµå (DEBUG ¸ğµå)
# 3. ÄÄÆ÷³ÍÆ® ¾÷µ¥ÀÌÆ® È®ÀÎ (¼­¹ö ÀúÀå¼Ò ºñ¾îÀÖÀ¸¸é ½ºÅµ)
# 4. MainShell ½ÇÇà
```

### ¿î¿µ È¯°æ

```
¹èÆ÷ ÆĞÅ°Áö:
¦§¦¡¦¡ nU3.Bootstrapper.exe      ¡ç »ç¿ëÀÚ ½ÇÇà
¦§¦¡¦¡ nU3.MainShell.exe
¦§¦¡¦¡ nU3.Core.dll
¦§¦¡¦¡ nU3.Core.UI.dll
¦§¦¡¦¡ nU3.Data.dll
¦§¦¡¦¡ nU3.Models.dll
¦¦¦¡¦¡ ... (±âÅ¸ ÇÊ¼ö DLL)

¼­¹ö ÀúÀå¼Ò (Áß¾Ó °ü¸®):
\\server\nU3.Framework\ServerStorage\Components\
¦§¦¡¦¡ Framework\...
¦§¦¡¦¡ DevExpress\...
¦¦¦¡¦¡ ...
```

## Âü°í

- ÄÄÆ÷³ÍÆ® ¹èÆ÷: `nU3.Tools.Deployer` ¡æ "Framework ÄÄÆ÷³ÍÆ®" ÅÇ
- DB ½ºÅ°¸¶: `SYS_COMPONENT_MST`, `SYS_COMPONENT_VER`
- °ü·Ã °¡ÀÌµå: `INSTALL_PATH_GUIDE.md`, `COMPONENT_DEPLOY_GUIDE.md`

---

## ë‹¨ì¼ ì‹¤í–‰ íŒŒì¼ ë¹Œë“œ

### ë¹ ë¥¸ ì‹œì‘

```batch
# Release ë¹Œë“œ (í”„ë¡œë•ì…˜)
build_single.bat

# Debug ë¹Œë“œ (ê°œë°œ)
build_single_debug.bat
```

### ì¶œë ¥

```
publish/
â”œâ”€â”€ nU3.Bootstrapper.exe    # ë‹¨ì¼ ì‹¤í–‰ íŒŒì¼ (ì•½ 70-100MB)
â””â”€â”€ (ê¸°íƒ€ íŒŒì¼ ì—†ìŒ)
```

### ê¸°ëŠ¥

- ë‹¨ì¼ EXE íŒŒì¼ë¡œ ëª¨ë“  ì¢…ì†ì„± í¬í•¨
- ëŸ°íƒ€ì„ í¬í•¨ (ë³„ë„ .NET ì„¤ì¹˜ ë¶ˆí•„ìš”)
- `appsettings.json` ë¦¬ì†ŒìŠ¤ í¬í•¨
- LOG í´ë”ì— ìë™ ë¡œê¹…

### ìì„¸í•œ ì •ë³´

ìì„¸í•œ ë‚´ìš©ì€ `BUILD_SINGLE_GUIDE.md` ì°¸ì¡°
