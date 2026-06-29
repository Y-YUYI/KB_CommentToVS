using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace KB_CommentToVS
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("KB注释工具", "插入自定义格式的代码注释", "2.2")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid("9791676F-CC90-4ED6-95E1-C1841624E02A")]
    
    public sealed class SnippetDateTimeExtensionPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            System.Diagnostics.Debug.WriteLine("Package initialized!");

            // 注册插入注释命令
            await InsertSnippetCommand.InitializeAsync(this);

            // 注册设置对话框命令
            await SettingsCommand.InitializeAsync(this);
        }
    }
}
