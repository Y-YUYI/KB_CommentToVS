using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace KB_CommentToVS
{
    internal sealed class InsertSnippetCommand
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("B2C3D4E5-F6A7-8901-BCDE-F23456789012");

        private readonly AsyncPackage package;

        private InsertSnippetCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static InsertSnippetCommand Instance { get; private set; }

        public static async System.Threading.Tasks.Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new InsertSnippetCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            
            var dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
            if (dte == null || dte.ActiveDocument == null)
                return;

            var selection = dte.ActiveDocument.Selection as TextSelection;
            if (selection == null)
                return;

            // 读取自定义配置
            var config = SnippetConfig.Load();
            string dateTime = DateTime.Now.ToString(config.DateFormat);

            // 替换模板中的占位符
            string body = config.Template
                .Replace("{Modifier}", config.Modifier)
                .Replace("{DateTime}", dateTime);

            // 每行加上注释前缀
            string snippet = BuildSnippet(config.CommentPrefix, body);

            selection.Insert(snippet);
        }

        /// <summary>
        /// 给模板内容的每行加上注释前缀
        /// </summary>
        private string BuildSnippet(string prefix, string template)
        {
            var lines = template.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                string trimmed = lines[i].Trim();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    lines[i] = prefix + " " + trimmed;
                }
            }
            return string.Join(Environment.NewLine, lines);
        }
    }
}
