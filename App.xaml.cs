using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;
using RunVision.Services;
using RunVision.Utils;
using RunVision.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using VM.Core;
using VM.PlatformSDKCS;

namespace RunVision
{
    public partial class App : PrismApplication
    {
        private static Mutex _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string mutexName = "RunVision_Mutex";

            bool createdNew;
            _mutex = new Mutex(true, mutexName, out createdNew);

            if (!createdNew)
            {
                BringExistingInstanceToFront();
                Shutdown();
                return;
            }

            VmSolution.OnDongleEvent += Handle_OnDongleEvent;

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
            base.OnExit(e);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<Views.MainWindow>();

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppConfigService, ProjectConfigService>();
            containerRegistry.Register<MainWindowViewModel>();


        }

        private void Handle_OnDongleEvent(ImvsSdkDefine.IMVS_DONGLE_INFO moduleInfo)
        {
            // 获取加密狗状态，正常时为0，异常时为错误码
            int dongleStatus = moduleInfo.nDongleStatus;
            // 获取加密狗型号
            byte[] bytDongleType = moduleInfo.strDongleType;
            string str = Encoding.UTF8.GetString(bytDongleType).TrimEnd('\0');
            if (dongleStatus != 0)
            {
                MyLogger.Error($"加密狗异常，状态码: {dongleStatus}");
                MessageBoxResult result = MessageBox.Show($"加密狗异常，状态码: {dongleStatus}", "加密狗状态", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    VmSolution.Instance.CloseSolution();
                    Process currentProcess = Process.GetCurrentProcess();
                    currentProcess.Kill();
                }
            }
        }

        private void BringExistingInstanceToFront()
        {
            try
            {
                var current = Process.GetCurrentProcess();
                var existing = Process.GetProcessesByName(current.ProcessName)
                    .FirstOrDefault(p => p.Id != current.Id);

                if (existing == null)
                    return;

                existing.WaitForInputIdle(1000);
                IntPtr hWnd = existing.MainWindowHandle;
                if (hWnd == IntPtr.Zero)
                    return;

                ShowWindow(hWnd, SW_RESTORE);
                SetForegroundWindow(hWnd);
            }
            catch { }
        }

        #region Win32 API
        private const int SW_RESTORE = 9;
        [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        #endregion
    }
}
