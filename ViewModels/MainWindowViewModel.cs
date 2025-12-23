using NLog;
using Prism.Commands;
using Prism.Dialogs;
using Prism.Mvvm;
using RunVision.Services;
using RunVision.Utils;
using RunVision.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VM.Core;
using VM.PlatformSDKCS;
using VMControls.WPF.Release;

namespace RunVision.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IAppConfigService _configService;

        // 窗口标题
        private string _windowTitle = "RunVision";
        public string WindowTitle
        {
            get => _windowTitle;
            set => SetProperty(ref _windowTitle, value);
        }

        // 当前用户
        private string _currentUser = "游客";
        public string CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        // 当前方案
        private string _currentScheme = "null";
        public string CurrentScheme
        {
            get => _currentScheme;
            set => SetProperty(ref _currentScheme, value);
        }

        // 方案列表
        public ObservableCollection<string> Schemes { get; set; } = new ObservableCollection<string>();

        public MainWindowViewModel(IAppConfigService configService)
        {
            WindowTitle = "RunVision";
            _configService = configService;
            Schemes = new ObservableCollection<string>(_configService.ProjectNames);


        }

        // 方案配置
        public ICommand SchemeConfigCommand => new DelegateCommand(() =>
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow == null)
            {
                MyLogger.Warn("打开方案配置窗口失败：主窗口不存在");
                return;
            }

            var schemeConfigWindow = new SchemeConfigWindow
            {
                Owner = mainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            schemeConfigWindow.ShowDialog();
        });

        // 管理员登录
        public ICommand PermissionCommand => new DelegateCommand(() =>
        {
            if (CurrentUser.Equals("管理员"))
            {
                CurrentUser = "游客";
                return;
            }
            var loginWindow = new LoginWindow();
            loginWindow.DataContext = new LoginWindowViewModel();
            bool? result = loginWindow.ShowDialog();
            if (result == true)
            {
                CurrentUser = "管理员";
            }
        });

        // 方案切换
        public ICommand SchemeCommand => new DelegateCommand<string>(schemeName =>
        {
            if (string.IsNullOrWhiteSpace(schemeName) || schemeName == "无方案")
            {
                MyLogger.Warn("方案切换失败：无可用方案（需先创建/导入有效方案）");
                return;
            }
            if (CurrentScheme == schemeName)
            {
                MyLogger.Warn($"方案切换跳过：[{schemeName}]已是当前方案");
                return;
            }

            try
            {
                VmSolution.Instance.CloseSolution();
                _configService.LoadProject(schemeName);
                CurrentScheme = schemeName;
                MyLogger.Info($"方案[{schemeName}]切换完成");

            }
            catch (Exception ex)
            {
                MyLogger.Error($"方案[{schemeName}]加载失败", ex);
                MessageBox.Show($"方案加载失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });

        // 窗口关闭命令
        public ICommand WindowClosingCommand => new DelegateCommand<CancelEventArgs>(e =>
        {
            var result = MessageBox.Show("确定要退出吗？", "退出确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }

            try
            {
                VmSolution.Instance.CloseSolution();
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                LogManager.Flush();
                LogManager.Shutdown();
                MyLogger.Info("程序正常退出！");
                Process currentProcess = Process.GetCurrentProcess();
                currentProcess.Kill();
            }
            catch (Exception ex)
            {
                MyLogger.Warn($"警告：{ex.Message}");
                MyLogger.Info("程序异常退出！");
                Environment.Exit(0);
            }
        });
    }
}
