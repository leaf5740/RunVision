using Prism.Commands;
using Prism.Mvvm;
using RunVision.Models;
using RunVision.Services;
using System;
using System.Windows;

namespace RunVision.ViewModels.TabViews
{
    public class SaveImageTabViewModel : BindableBase
    {
        private readonly IAppConfigService _configService;

        #region 图片保存配置模型

        private ImageSaveModel _imageSaveModel;
        public ImageSaveModel ImageSaveModel
        {
            get => _imageSaveModel;
            set => SetProperty(ref _imageSaveModel, value);
        }

        #endregion

        #region 命令

        public DelegateCommand SaveConfigCommand { get; }

        #endregion

        #region 构造函数

        public SaveImageTabViewModel(IAppConfigService configService)
        {
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));

            // 读取配置，如果没有就创建默认
            ImageSaveModel = _configService.ProjectModels?.ImageSaveConfig ?? new Models.ImageSaveModel
            {
                ImageSavePath = "",
                CompressionLevel = ""
            };

            // 初始化命令
            SaveConfigCommand = new DelegateCommand(OnSaveConfig, CanSaveConfig);

            // 订阅属性变化，当用户修改 TextBox 时刷新按钮状态
            ImageSaveModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ImageSaveModel.ImageSavePath) || e.PropertyName == nameof(ImageSaveModel.CompressionLevel))
                {
                    SaveConfigCommand.RaiseCanExecuteChanged();
                }
            };
        }


        #endregion

        #region 命令实现

        private void OnSaveConfig()
        {
            // 校验路径
            if (string.IsNullOrWhiteSpace(ImageSaveModel.ImageSavePath))
            {
                MessageBox.Show("存图路径不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 校验压缩等级
            if (!int.TryParse(ImageSaveModel.CompressionLevel, out int level) || level < 0 || level > 100)
            {
                MessageBox.Show("压缩等级必须是 0-100 的数字！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 保存到配置服务
                _configService.ProjectModels.ImageSaveConfig = ImageSaveModel;
                _configService.SaveConfig();

                MessageBox.Show("存图配置保存成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存配置失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSaveConfig()
        {
            return !string.IsNullOrWhiteSpace(ImageSaveModel?.ImageSavePath)
                   && int.TryParse(ImageSaveModel?.CompressionLevel, out int level) && level >= 0 && level <= 100;
        }

        #endregion
    }


}
