using Prism.Commands;
using Prism.Mvvm;
using RunVision.Models;
using RunVision.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace RunVision.ViewModels.TabViews
{
    public class DatabaseTabViewModel : BindableBase
    {
        private readonly IAppConfigService _configService;
        public DatabaseTabViewModel(IAppConfigService configService)
        {
            _configService = configService;
            // 数据库类型初始化
            AvailableDatabases = new ObservableCollection<string>
            {
                "MySQL"
            };

            SelectedDatabase = AvailableDatabases[0];

            DatabaseModel = new DatabaseModel();

            ConnectCommand = new DelegateCommand(OnConnect, CanConnect)
                .ObservesProperty(() => DatabaseModel.Ip)
                .ObservesProperty(() => DatabaseModel.Port)
                .ObservesProperty(() => DatabaseModel.Password)
                .ObservesProperty(() => DatabaseModel.LibraryName)
                .ObservesProperty(() => DatabaseModel.CodeTableName)
                .ObservesProperty(() => DatabaseModel.DataTableName);


            SaveConfigCommand = new DelegateCommand(OnSaveConfig, CanSave)
                .ObservesProperty(() => DatabaseModel.Ip)
                .ObservesProperty(() => DatabaseModel.Port)
                .ObservesProperty(() => DatabaseModel.Password)
                .ObservesProperty(() => DatabaseModel.LibraryName)
                .ObservesProperty(() => DatabaseModel.CodeTableName)
                .ObservesProperty(() => DatabaseModel.DataTableName);

        }

        #region 数据库类型

        private ObservableCollection<string> _availableDatabases;
        public ObservableCollection<string> AvailableDatabases
        {
            get => _availableDatabases;
            set => SetProperty(ref _availableDatabases, value);
        }

        private string _selectedDatabase;
        public string SelectedDatabase
        {
            get => _selectedDatabase;
            set => SetProperty(ref _selectedDatabase, value);
        }

        #endregion

        #region 数据库配置模型

        private DatabaseModel _databaseModel;
        public DatabaseModel DatabaseModel
        {
            get => _databaseModel;
            set => SetProperty(ref _databaseModel, value);
        }

        #endregion

        #region Commands

        public DelegateCommand ConnectCommand { get; }
        public DelegateCommand SaveConfigCommand { get; }

        #endregion

        #region Command Methods

        private void OnConnect()
        {
            // 这里以后你可以接真实数据库连接
            MessageBox.Show(
                $"连接数据库：{SelectedDatabase}\n" +
                $"IP：{DatabaseModel.Ip}\n" +
                $"端口：{DatabaseModel.Port}",
                "连接测试"
            );
        }

        private bool CanConnect()
        {
            return !string.IsNullOrWhiteSpace(DatabaseModel.Ip)
                && !string.IsNullOrWhiteSpace(DatabaseModel.Port)
                && !string.IsNullOrWhiteSpace(DatabaseModel.Password)
                && !string.IsNullOrWhiteSpace(DatabaseModel.LibraryName)
                && !string.IsNullOrWhiteSpace(DatabaseModel.CodeTableName)
                && !string.IsNullOrWhiteSpace(DatabaseModel.DataTableName);
        }


        private void OnSaveConfig()
        {
            try
            {
                // 保存配置
                DatabaseModel.DatabaseBrand = SelectedDatabase;
                _configService.ProjectModels.DatabaseConfig = DatabaseModel;
                _configService.SaveConfig();

                MessageBox.Show("存图配置保存成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存配置失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(DatabaseModel.Ip)
                && !string.IsNullOrWhiteSpace(DatabaseModel.Port)
                && !string.IsNullOrWhiteSpace(DatabaseModel.Password)
                && !string.IsNullOrWhiteSpace(DatabaseModel.LibraryName)
                && !string.IsNullOrWhiteSpace(DatabaseModel.CodeTableName)
                && !string.IsNullOrWhiteSpace(DatabaseModel.DataTableName);
        }

        #endregion
    }
}
