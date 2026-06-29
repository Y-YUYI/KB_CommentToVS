using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace KB_CommentToVS
{
    internal sealed class SettingsCommand
    {
        public const int CommandId = 0x0101;
        public static readonly Guid CommandSet = new Guid("B2C3D4E5-F6A7-8901-BCDE-F23456789012");

        private readonly AsyncPackage package;

        private SettingsCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static SettingsCommand Instance { get; private set; }

        public static async System.Threading.Tasks.Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new SettingsCommand(package, commandService);
        }
        
        
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            if (dte == null)
            {
                using (var dialog = new SettingsDialog())
                {
                    dialog.ShowDialog();
                }
                return;
            }

            NativeWindow nativeWindow = null;
            try
            {
                IntPtr mainWindowPtr = (IntPtr)dte.MainWindow.HWnd;
                nativeWindow = new NativeWindow();
                nativeWindow.AssignHandle(mainWindowPtr);

                using (var dialog = new SettingsDialog())
                {
                    dialog.ShowDialog(nativeWindow);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SettingsDialog error: " + ex.Message);
                using (var dialog = new SettingsDialog())
                {
                    dialog.ShowDialog();
                }
            }
            finally
            {
                if (nativeWindow != null && nativeWindow.Handle != IntPtr.Zero)
                {
                    nativeWindow.ReleaseHandle();
                }
            }
        }
    }
}
