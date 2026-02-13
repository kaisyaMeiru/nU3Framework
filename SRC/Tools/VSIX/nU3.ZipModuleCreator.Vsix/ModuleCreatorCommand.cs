using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace nU3.ModuleCreator.Vsix
{
    internal sealed class ModuleCreatorCommand
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("11111111-2222-3333-4444-555555555555");
        private readonly AsyncPackage package;

        private ModuleCreatorCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package;
            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                new ModuleCreatorCommand(package, commandService);
            }
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            using (var form = new WizardForm())
            {
                if (form.ShowDialog() != DialogResult.OK)
                    return;

                var dte = (DTE)package.GetServiceAsync(typeof(DTE)).Result;
                if (dte?.Solution == null || string.IsNullOrEmpty(dte.Solution.FullName))
                {
                    MessageBox.Show("먼저 솔루션을 열어주세요.");
                    return;
                }

                var solutionDir = Path.GetDirectoryName(dte.Solution.FullName);
                if (string.IsNullOrEmpty(solutionDir))
                    throw new InvalidOperationException("솔루션 디렉토리를 찾을 수 없습니다.");

                // Project Name: nU3.Modules.{System}.{SubSystem}.{ModuleNamespace}
                var projectName = string.Format("nU3.Modules.{0}.{1}.{2}", form.SystemName, form.SubSystem, form.ModuleNamespace);
                
                // Target Directory: Modules\{System}\{SubSystem}\{ProjectName}
                var targetDir = Path.Combine(solutionDir, "Modules", form.SystemName, form.SubSystem, projectName);
                
                if (Directory.Exists(targetDir))
                {
                    MessageBox.Show($"대상 디렉토리가 이미 존재합니다:\n{targetDir}");
                    return;
                }
                Directory.CreateDirectory(targetDir);

                // Get Template Directory
                string assemblyDir = Path.GetDirectoryName(typeof(ModuleCreatorCommand).Assembly.Location);
                string templateDir = Path.Combine(assemblyDir, "ProjectTemplate");
                
                if (!Directory.Exists(templateDir))
                {
                    MessageBox.Show($"템플릿 디렉토리를 찾을 수 없습니다:\n{templateDir}");
                    return;
                }

                var tokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["$system$"] = form.SystemName,
                    ["$subsystem$"] = form.SubSystem,
                    ["$namespace$"] = form.ModuleNamespace,
                    ["$programname$"] = form.ProgramName,
                    ["$programid$"] = form.ProgramId,
                    ["$projectname$"] = projectName,
                    ["$dllname$"] = projectName,
                    ["$controlclassname$"] = form.ProgramId,
                    ["$bizlogicclassname$"] = form.ProgramId + "BizLogic",
                };

                foreach (var srcPath in Directory.GetFiles(templateDir, "*", SearchOption.AllDirectories))
                {
                    var relPath = GetRelativePath(templateDir, srcPath);

                    // Rename rules
                    if (string.Equals(Path.GetFileName(relPath), "Project.csproj", StringComparison.OrdinalIgnoreCase))
                    {
                        relPath = Path.Combine(Path.GetDirectoryName(relPath), projectName + ".csproj");
                    }
                    else if (string.Equals(Path.GetFileName(relPath), "ZipCodeSearchControl.cs", StringComparison.OrdinalIgnoreCase))
                    {
                        relPath = Path.Combine(Path.GetDirectoryName(relPath), tokens["$controlclassname$"] + ".cs");
                    }
                    else if (string.Equals(Path.GetFileName(relPath), "ZipCodeSearchControl.Designer.cs", StringComparison.OrdinalIgnoreCase))
                    {
                        relPath = Path.Combine(Path.GetDirectoryName(relPath), tokens["$controlclassname$"] + ".Designer.cs");
                    }
                    else if (string.Equals(Path.GetFileName(relPath), "ZipCodeBizLogic.cs", StringComparison.OrdinalIgnoreCase))
                    {
                        relPath = Path.Combine(Path.GetDirectoryName(relPath), tokens["$bizlogicclassname$"] + ".cs");
                    }

                    // Replace tokens in path
                    foreach (var kv in tokens)
                        relPath = ReplaceIgnoreCase(relPath, kv.Key, kv.Value);

                    var destPath = Path.Combine(targetDir, relPath);
                    var destFolder = Path.GetDirectoryName(destPath);
                    if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);

                    var content = File.ReadAllText(srcPath);
                    foreach (var kv in tokens)
                        content = ReplaceIgnoreCase(content, kv.Key, kv.Value);

                    File.WriteAllText(destPath, content);
                }

                // Add to solution
                var csprojPath = Path.Combine(targetDir, projectName + ".csproj");
                if (File.Exists(csprojPath))
                {
                    try 
                    {
                        dte.Solution.AddFromFile(csprojPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"프로젝트가 생성되었으나 솔루션에 추가하지 못했습니다.\n{ex.Message}");
                    }
                }
            }
        }

        private static string GetRelativePath(string baseDir, string fullPath)
        {
            if (!baseDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                baseDir += Path.DirectorySeparatorChar;

            var baseUri = new Uri(baseDir);
            var fullUri = new Uri(fullPath);
            var relUri = baseUri.MakeRelativeUri(fullUri);
            return Uri.UnescapeDataString(relUri.ToString()).Replace('/', Path.DirectorySeparatorChar);
        }

        private static string ReplaceIgnoreCase(string input, string search, string replacement)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(search)) return input;

            int index = 0;
            while (true)
            {
                index = input.IndexOf(search, index, StringComparison.OrdinalIgnoreCase);
                if (index < 0) break;
                input = input.Substring(0, index) + replacement + input.Substring(index + search.Length);
                index += replacement.Length;
            }
            return input;
        }
    }
}
