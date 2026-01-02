using Prism.Commands;
using Prism.Mvvm;
using RunVision.Models;
using RunVision.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RunVision.ViewModels.TabViews
{
    public class PlcTabViewModel : BindableBase
    {
        private readonly IAppConfigService _configService;

        // PLC 协议
        public ObservableCollection<string> AvailableProtocols { get; }
        private string _selectedProtocol;
        public string SelectedProtocol
        {
            get => _selectedProtocol;
            set
            {
                if (SetProperty(ref _selectedProtocol, value))
                    ConnectCommand.RaiseCanExecuteChanged();
            }
        }

        //PLC 品牌
        public ObservableCollection<string> PlcBrands { get; }
        private string _selectedBrand;
        public string SelectedBrand
        {
            get => _selectedBrand;
            set
            {
                if (SetProperty(ref _selectedBrand, value))
                    ConnectCommand.RaiseCanExecuteChanged();
            }
        }

        // PLC 配置
        private PlcModel _plcConfig;
        public PlcModel PlcConfig
        {
            get => _plcConfig;
            private set
            {
                if (SetProperty(ref _plcConfig, value))
                {
                    ConnectCommand.RaiseCanExecuteChanged();
                    SaveConfigCommand.RaiseCanExecuteChanged();

                    // 当 PLC IP/Port 改变时刷新按钮状态
                    _plcConfig.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(PlcModel.Ip) || e.PropertyName == nameof(PlcModel.Port))
                        {
                            ConnectCommand.RaiseCanExecuteChanged();
                            SaveConfigCommand.RaiseCanExecuteChanged();
                        }
                    };
                }
            }
        }

        public ObservableCollection<PlcModel.PLCAddressModels> ReadPLCAddress { get; }
        public ObservableCollection<PlcModel.PLCAddressModels> WritePLCAddress { get; }

        public DelegateCommand ConnectCommand { get; }
        public DelegateCommand SaveConfigCommand { get; }

        public PlcTabViewModel(IAppConfigService configService)
        {
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));

            // 初始化命令
            ConnectCommand = new DelegateCommand(OnConnect, CanConnect);
            SaveConfigCommand = new DelegateCommand(OnSaveConfig, CanSaveConfig);

            // 初始化协议和品牌
            AvailableProtocols = new ObservableCollection<string> { "Modbus TCP" };
            SelectedProtocol = AvailableProtocols.FirstOrDefault();

            PlcBrands = new ObservableCollection<string> { "汇川", "三菱" };
            SelectedBrand = PlcBrands.FirstOrDefault();

            // 初始化 PLC 配置
            PlcConfig = _configService.ProjectModels?.PlcConfig ?? new PlcModel();

            // 初始化 DataGrid 集合，先加载已有配置，如果没有就创建空列表
            ReadPLCAddress = new ObservableCollection<PlcModel.PLCAddressModels>((PlcConfig.ReadPLCAddress != null && PlcConfig.ReadPLCAddress.Any()) ? PlcConfig.ReadPLCAddress : new List<PlcModel.PLCAddressModels>
            {
                new PlcModel.PLCAddressModels { Name = "异常自检重启软件" ,Address="无",Value="无"},
                new PlcModel.PLCAddressModels { Name = "心跳信号",Address="无",Value="无" },
                new PlcModel.PLCAddressModels { Name = "要板信号",Address="无",Value="无" },
            });

            WritePLCAddress = new ObservableCollection<PlcModel.PLCAddressModels>((PlcConfig.WritePLCAddress != null && PlcConfig.WritePLCAddress.Any()) ? PlcConfig.WritePLCAddress : new List<PlcModel.PLCAddressModels>
            {
                new PlcModel.PLCAddressModels { Name = "扫码失败" ,Address="无",Value="无"},
                new PlcModel.PLCAddressModels { Name = "检测完成",Address="无",Value="无" },
            });

            // 当集合变化时刷新保存按钮状态
            ReadPLCAddress.CollectionChanged += (s, e) => SaveConfigCommand.RaiseCanExecuteChanged();
            WritePLCAddress.CollectionChanged += (s, e) => SaveConfigCommand.RaiseCanExecuteChanged();
        }

        private void OnConnect()
        {
            if (!CanConnect())
            {
                MessageBox.Show("请填写完整的协议、品牌、IP 和端口！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show($"PLC 连接成功！\n协议：{SelectedProtocol}\n品牌：{SelectedBrand}\nIP：{PlcConfig.Ip}\nPort：{PlcConfig.Port}",
                "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool CanConnect()
        {
            return !string.IsNullOrWhiteSpace(SelectedProtocol)
                   && !string.IsNullOrWhiteSpace(SelectedBrand)
                   && !string.IsNullOrWhiteSpace(PlcConfig?.Ip)
                   && int.TryParse(PlcConfig?.Port, out int port) && port > 0;
        }

        private void OnSaveConfig()
        {
            try
            {
                // 校验读取地址列表
                foreach (var item in ReadPLCAddress)
                {
                    // 如果 Address 非空且不为 "无"，才校验首字母
                    if (!string.IsNullOrWhiteSpace(item.Address) && !item.Address.Equals("无", StringComparison.OrdinalIgnoreCase) &&
                        !(item.Address.StartsWith("D", StringComparison.OrdinalIgnoreCase) || item.Address.StartsWith("M", StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show($"读取地址 [{item.Name ?? "未知"}] 的地址必须以 'D' 或 'M' 开头，或填写为空/无！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // 如果 Value 非空且不为 "无"，才校验是否数字
                    if (!string.IsNullOrWhiteSpace(item.Value) && !item.Value.Equals("无", StringComparison.OrdinalIgnoreCase) && !int.TryParse(item.Value, out _))
                    {
                        MessageBox.Show($"读取地址 [{item.Name ?? "未知"}] 的值只能是数字，或填写为空/无！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // 校验写入地址列表
                foreach (var item in WritePLCAddress)
                {
                    if (!string.IsNullOrWhiteSpace(item.Address) && !item.Address.Equals("无", StringComparison.OrdinalIgnoreCase) &&
                        !(item.Address.StartsWith("D", StringComparison.OrdinalIgnoreCase) || item.Address.StartsWith("M", StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show($"写入地址 [{item.Name ?? "未知"}] 的地址必须以 'D' 或 'M' 开头，或填写为空/无！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!string.IsNullOrWhiteSpace(item.Value) && !item.Value.Equals("无", StringComparison.OrdinalIgnoreCase) && !int.TryParse(item.Value, out _))
                    {
                        MessageBox.Show($"写入地址 [{item.Name ?? "未知"}] 的值只能是数字，或填写为空/无！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }


                // 同步 UI 选择到 PlcConfig
                PlcConfig.Brand = SelectedBrand;
                PlcConfig.Protocol = SelectedProtocol;
                PlcConfig.ReadPLCAddress = ReadPLCAddress.ToList();
                PlcConfig.WritePLCAddress = WritePLCAddress.ToList();
                _configService.ProjectModels.PlcConfig = PlcConfig;
                _configService.SaveConfig();

                MessageBox.Show("PLC 配置保存成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存配置失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSaveConfig()
        {
            return !string.IsNullOrWhiteSpace(PlcConfig?.Ip)
                   && int.TryParse(PlcConfig?.Port, out int port) && port > 0
                   && (ReadPLCAddress.Any() || WritePLCAddress.Any());
        }

    }
}
