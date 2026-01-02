using Prism.Commands;
using Prism.Mvvm;
using RunVision.Models;
using RunVision.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RunVision.ViewModels.TabViewModels
{
    public class CameraTabViewModel : BindableBase
    {
        private readonly IAppConfigService _configService;
        public ObservableCollection<CameraModel> CameraModels { get; private set; }

        public ObservableCollection<string> CameraBrands { get; private set; }
        private string _selectedBrand;
        public string SelectedBrand
        {
            get { return _selectedBrand; }
            set
            {
                if (SetProperty(ref _selectedBrand, value))
                {
                    SelectedSN = null;
                    LoadCameraSNs(value);
                    AddCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<string> AvailableSNs { get; private set; }
        private string _selectedSN;
        public string SelectedSN
        {
            get { return _selectedSN; }
            set
            {
                if (SetProperty(ref _selectedSN, value))
                {
                    AddCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _plcReadyAddress;
        public string PlcReadyAddress
        {
            get { return _plcReadyAddress; }
            set
            {
                if (SetProperty(ref _plcReadyAddress, value))
                {
                    AddCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public CameraTabViewModel(IAppConfigService configService)
        {
            _configService = configService;
            CameraModels = new ObservableCollection<CameraModel>();
            CameraBrands = new ObservableCollection<string>
            {
                "海康相机",
                "大恒相机"
            }; 
            AvailableSNs = new ObservableCollection<string>();

            AddCommand = new DelegateCommand(OnAdd, CanAdd);
            SaveConfigCommand = new DelegateCommand(OnSaveConfig);
        }

        private CameraModel _selectedCamera;
        public CameraModel SelectedCamera
        {
            get { return _selectedCamera; }
            set { SetProperty(ref _selectedCamera, value); }
        }
        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand SaveConfigCommand { get; private set; }

        private void OnAdd()
        {
            if (CameraModels.Any(c => c.Sn == SelectedSN))
            {
                MessageBox.Show(
                    "SN 为 [" + SelectedSN + "] 的相机已存在！",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (CameraModels.Any(c => c.PlcAddress == PlcReadyAddress))
            {
                MessageBox.Show(
                    "Ready 信号地址 [" + PlcReadyAddress + "] 已被占用！",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            CameraModel model = new CameraModel
            {
                Brand = SelectedBrand,
                Sn = SelectedSN,
                PlcAddress = PlcReadyAddress
            };

            CameraModels.Add(model);

            // 添加完成后清空输入（保留品牌）
            SelectedSN = null;
            PlcReadyAddress = string.Empty;
        }

        private bool CanAdd()
        {
            return !string.IsNullOrWhiteSpace(SelectedBrand)
                && !string.IsNullOrWhiteSpace(SelectedSN)
                && !string.IsNullOrWhiteSpace(PlcReadyAddress);
        }

        private void OnSaveConfig()
        {
            try
            {
                _configService.SaveConfig();
                MessageBox.Show(
                    "相机配置保存成功！",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "保存配置失败：" + ex.Message,
                    "错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void LoadCameraSNs(string brand)
        {
            AvailableSNs.Clear();

            if (string.IsNullOrWhiteSpace(brand))
                return;

            if (brand == "海康相机")
            {
                AvailableSNs.Add("SN001");
                AvailableSNs.Add("SN002");
            }
            else if (brand == "大恒相机")
            {
                AvailableSNs.Add("AN001");
                AvailableSNs.Add("AN002");
            }
        }

    }
}
