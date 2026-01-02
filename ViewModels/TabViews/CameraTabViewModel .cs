using Prism.Commands;
using Prism.Mvvm;
using RunVision.Models;
using RunVision.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RunVision.ViewModels.TabViews
{
    public class CameraTabViewModel : BindableBase
    {
        private readonly IAppConfigService _configService;

        // 所有已添加的相机列表，绑定到 DataGrid
        public ObservableCollection<CameraModel> CameraModels { get; private set; }

        // 相机品牌列表
        public ObservableCollection<string> CameraBrands { get; private set; }

        // 用户选择的品牌
        private string _selectedBrand;
        public string SelectedBrand
        {
            get => _selectedBrand;
            set
            {
                if (SetProperty(ref _selectedBrand, value))
                {
                    // 品牌变动后，清空 SN 并加载对应的可选 SN
                    SelectedSN = null;
                    LoadCameraSNs(value);

                    // 刷新“添加”按钮状态
                    AddCommand.RaiseCanExecuteChanged();
                }
            }
        }

        // 可选的相机 SN 列表
        public ObservableCollection<string> AvailableSNs { get; private set; }

        // 用户选择的 SN
        private string _selectedSN;
        public string SelectedSN
        {
            get => _selectedSN;
            set
            {
                if (SetProperty(ref _selectedSN, value))
                {
                    // 刷新“添加”按钮状态
                    AddCommand.RaiseCanExecuteChanged();
                }
            }
        }

        // PLC Ready 信号地址
        private string _plcReadyAddress;
        public string PlcReadyAddress
        {
            get => _plcReadyAddress;
            set
            {
                if (SetProperty(ref _plcReadyAddress, value))
                {
                    // 刷新“添加”按钮状态
                    AddCommand.RaiseCanExecuteChanged();
                }
            }
        }

        // PLC Ready信号值
        private string _plcReadyValue;
        public string PlcReadyValue
        {
            get => _plcReadyValue;
            set
            {
                if (SetProperty(ref _plcReadyValue, value))
                {
                    // 刷新“添加”按钮状态
                    AddCommand.RaiseCanExecuteChanged();
                }
            }
        }

        // DataGrid 中选中的相机，用于删除操作
        private CameraModel _selectedCamera;
        public CameraModel SelectedCamera
        {
            get => _selectedCamera;
            set
            {
                if (SetProperty(ref _selectedCamera, value))
                {
                    // 刷新“删除”按钮状态
                    DeleteSelectedCameraCommand.RaiseCanExecuteChanged();
                }
            }
        }

        // 按钮命令
        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand DeleteSelectedCameraCommand { get; private set; }
        public DelegateCommand SaveConfigCommand { get; private set; }

        // 构造函数
        public CameraTabViewModel(IAppConfigService configService)
        {
            _configService = configService;
            // 确保项目已经加载
            if (_configService.ProjectModels?.CamerasConfig != null)
            {
                CameraModels = new ObservableCollection<CameraModel>(_configService.ProjectModels.CamerasConfig);
            }
            else
            {
                CameraModels = new ObservableCollection<CameraModel>();
            }
            CameraBrands = new ObservableCollection<string> { "海康相机", "大恒相机" };
            AvailableSNs = new ObservableCollection<string>();

            // 初始化命令
            AddCommand = new DelegateCommand(OnAdd, CanAdd);
            DeleteSelectedCameraCommand = new DelegateCommand(OnDelete, CanDelete);
            SaveConfigCommand = new DelegateCommand(OnSaveConfig, CanSave);

            // 当 CameraModels 内容变化时，刷新保存按钮状态
            CameraModels.CollectionChanged += (s, e) => SaveConfigCommand.RaiseCanExecuteChanged();
        }

        #region 添加相机
        private void OnAdd()
        {
            // 检查 PlcReadyAddress 是否包含指定字符串
            if (!string.IsNullOrWhiteSpace(PlcReadyAddress) && !PlcReadyAddress.Equals("无", StringComparison.OrdinalIgnoreCase) && !(PlcReadyAddress.IndexOf("D", StringComparison.OrdinalIgnoreCase) >= 0 || PlcReadyAddress.IndexOf("M", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                MessageBox.Show("Ready信号地址必须包含 'D' 或 'M'，或者填写为空/无！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 检查 PlcReadyValue 是否为数字或 "无"
            if (!string.IsNullOrWhiteSpace(PlcReadyValue) && !PlcReadyValue.Equals("无", StringComparison.OrdinalIgnoreCase) && !int.TryParse(PlcReadyValue, out _))
            {
                MessageBox.Show("Ready信号值必须为数字或填写 '无'！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 检查 SN 是否重复
            if (CameraModels.Any(c => c.Sn == SelectedSN))
            {
                MessageBox.Show($"SN 为 [{SelectedSN}] 的相机已存在！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //// 检查 Ready 信号地址是否重复
            //if (CameraModels.Any(c => c.PlcAddress == PlcReadyAddress))
            //{
            //    MessageBox.Show($"Ready 信号地址 [{PlcReadyAddress}] 已被占用！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}

            // 添加新相机
            CameraModels.Add(new CameraModel
            {
                Brand = SelectedBrand,
                Sn = SelectedSN,
                PlcAddress = PlcReadyAddress,
                PlcValue = PlcReadyValue
            });
            SelectedSN = null;
        }

        // 添加按钮可用性判断
        private bool CanAdd()
        {
            return !string.IsNullOrWhiteSpace(SelectedBrand)
                && !string.IsNullOrWhiteSpace(SelectedSN)
                && !string.IsNullOrWhiteSpace(PlcReadyAddress)
                && !string.IsNullOrWhiteSpace(PlcReadyValue);
        }
        #endregion

        #region 删除相机
        private void OnDelete()
        {
            if (SelectedCamera != null)
            {
                CameraModels.Remove(SelectedCamera);
                SelectedCamera = null;
            }
        }

        // 删除按钮可用性判断
        private bool CanDelete()
        {
            return SelectedCamera != null;
        }
        #endregion

        #region 保存配置
        private void OnSaveConfig()
        {
            try
            {

                foreach (var item in CameraModels)
                {
                    // 如果 Address 非空且不为 "无"，才校验首字母
                    if (!string.IsNullOrWhiteSpace(item.PlcAddress) && !item.PlcAddress.Equals("无", StringComparison.OrdinalIgnoreCase) &&
                        !(item.PlcAddress.StartsWith("D", StringComparison.OrdinalIgnoreCase) || item.PlcAddress.StartsWith("M", StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show($"读取地址 [{item.Sn ?? "未知"}] 的地址必须以 'D' 或 'M' 开头，或填写为空/无！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // 如果 Value 非空且不为 "无"，才校验是否数字
                    if (!string.IsNullOrWhiteSpace(item.PlcValue) && !item.PlcValue.Equals("无", StringComparison.OrdinalIgnoreCase) && !int.TryParse(item.PlcValue, out _))
                    {
                        MessageBox.Show($"读取地址 [{item.Sn ?? "未知"}] 的值只能是数字，或填写为空/无！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                _configService.ProjectModels.CamerasConfig = CameraModels.ToList();
                _configService.SaveConfig();
                MessageBox.Show("相机配置保存成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存配置失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // 保存按钮可用性判断
        private bool CanSave()
        {
            // 只有有相机数据时才可保存
            return CameraModels.Any();
        }
        #endregion

        #region 加载 SN 列表
        private void LoadCameraSNs(string brand)
        {
            AvailableSNs.Clear();

            if (string.IsNullOrWhiteSpace(brand)) return;

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
        #endregion
    }
}
