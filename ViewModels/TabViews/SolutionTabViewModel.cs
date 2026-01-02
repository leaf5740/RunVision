using GlobalVariableModuleCs;
using Prism.Commands;
using Prism.Mvvm;
using RunVision.Models;
using RunVision.Services;
using RunVision.Utils;
using RunVision.ViewModels.TabViews.View;
using RunVision.Views;
using RunVision.Views.TabViews.View;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using VM.Core;
using static RunVision.Models.SolutionModel;

namespace RunVision.ViewModels.TabViews
{
    public class SolutionTabViewModel : BindableBase
    {
        private readonly IAppConfigService _configService;
        private GlobalVariableModuleTool globalVarTool = VmSolution.Instance["全局变量1"] as GlobalVariableModuleTool;

        #region 流程步骤列表（DataGrid）

        public ObservableCollection<SolutionModel.FlowStepModel> FlowSteps { get; private set; }

        private SolutionModel.FlowStepModel _selectedFlowStep;
        public SolutionModel.FlowStepModel SelectedFlowStep
        {
            get => _selectedFlowStep;
            set
            {
                if (SetProperty(ref _selectedFlowStep, value))
                {
                    DeleteSelectedFlowStepCommand.RaiseCanExecuteChanged();
                    ConfigPcsCommand.RaiseCanExecuteChanged(); // 选中项变化时刷新PCS配置命令状态
                }
            }
        }

        #endregion

        #region 流程类型（ComboBox）

        public ObservableCollection<string> FlowTypes { get; private set; } = new ObservableCollection<string>();

        private string _selectedFlowType;
        public string SelectedFlowType
        {
            get => _selectedFlowType;
            set
            {
                if (SetProperty(ref _selectedFlowType, value))
                {
                    AddFlowStepCommand?.RaiseCanExecuteChanged(); // ? 避免空引用
                }
            }
        }

        #endregion

        // 总PCS 绑定VM全局变量
        private string _totalPCS;
        public string TotalPCS
        {
            get
            {
                // 获取全局变量指定值
                var value = globalVarTool?.GetGlobalVar("总PCS");
                // 如果获取不到，就返回默认字符串
                if (string.IsNullOrEmpty(value))
                {
                    return "总PCS：未获取";
                }
                return $"总PCS：{value}";
            }
            set => SetProperty(ref _totalPCS, value);
        }

        #region Commands

        public DelegateCommand AddFlowStepCommand { get; private set; }
        public DelegateCommand DeleteSelectedFlowStepCommand { get; private set; }
        public DelegateCommand SaveConfigCommand { get; private set; }

        public DelegateCommand<FlowStepModel> ConfigPcsCommand { get; private set; }

        #endregion

        public SolutionTabViewModel(IAppConfigService configService)
        {
            _configService = configService;
            // 获取全部流程遍历新增FlowTypes
            ProcessInfoList process = VmSolution.Instance.GetAllProcedureList();
            for (int i = 0; i < process.nNum; i++)
            {
                FlowTypes.Add(process.astProcessInfo[i].strProcessName);
            }

            // 初始化流程步骤（从配置加载）
            if (_configService.ProjectModels?.SolutionConfig?.FlowSteps != null)
            {
                FlowSteps = new ObservableCollection<SolutionModel.FlowStepModel>(
                    _configService.ProjectModels.SolutionConfig.FlowSteps
                );
            }
            else
            {
                FlowSteps = new ObservableCollection<SolutionModel.FlowStepModel>();
            }

            // 初始化 Commands
            AddFlowStepCommand = new DelegateCommand(OnAddFlowStep, CanAddFlowStep);
            DeleteSelectedFlowStepCommand = new DelegateCommand(OnDeleteFlowStep, CanDelete);
            SaveConfigCommand = new DelegateCommand(OnSaveConfig, CanSave);
            ConfigPcsCommand = new DelegateCommand<FlowStepModel>(OnConfigPcs, CanConfigPcs);

            // 当 FlowSteps 变化时刷新按钮状态
            FlowSteps.CollectionChanged += (s, e) =>
            {
                SaveConfigCommand.RaiseCanExecuteChanged();
                AddFlowStepCommand.RaiseCanExecuteChanged();
                ConfigPcsCommand.RaiseCanExecuteChanged(); // 列表变化时刷新PCS配置命令状态
            };
        }

        #region 添加流程

        private void OnAddFlowStep()
        {
            if (string.IsNullOrWhiteSpace(SelectedFlowType))
                return;

            // 流程名称重复校验
            if (FlowSteps.Any(f => f.StepName.Equals(SelectedFlowType, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show($"流程名称 [{SelectedFlowType}] 已存在！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string[] possibleVarNames =
                {
                    $"{SelectedFlowType}索引",
                    $"{SelectedFlowType}图像索引",
                    $"{SelectedFlowType}图片索引",
                    $"{SelectedFlowType}图片",
                    $"{SelectedFlowType}运行索引"
                };

                string imageIndex = null;
                foreach (var varName in possibleVarNames)
                {
                    try
                    {
                        var value = globalVarTool.GetGlobalVar(varName);
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            imageIndex = value;
                            break;
                        }
                    }
                    catch (VM.PlatformSDKCS.VmException)
                    {
                        // 单个变量不存在是“正常情况”,继续往下执行
                    }
                }

                if (string.IsNullOrWhiteSpace(imageIndex))
                {
                    throw new InvalidOperationException(
                        $"未找到流程 [{SelectedFlowType}] 对应的图像索引全局变量");
                }

                var newStep = new SolutionModel.FlowStepModel
                {
                    StepName = SelectedFlowType,
                    ImageIndex = imageIndex
                };

                FlowSteps.Add(newStep);
                SelectedFlowStep = newStep;
            }
            catch (Exception e)
            {
                MyLogger.Error($"获取 [{SelectedFlowType}] 图片运行索引失败: {e}");
            }
        }

        // 控制“添加”按钮是否可用
        private bool CanAddFlowStep()
        {
            return !string.IsNullOrWhiteSpace(SelectedFlowType)
                   && !FlowSteps.Any(f => f.StepName.Equals(SelectedFlowType, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region 删除流程

        private void OnDeleteFlowStep()
        {
            if (SelectedFlowStep != null)
            {
                FlowSteps.Remove(SelectedFlowStep);
                SelectedFlowStep = null;
            }
        }

        private bool CanDelete()
        {
            return SelectedFlowStep != null;
        }

        #endregion

        #region PCS配置

        private void OnConfigPcs(FlowStepModel step)
        {
            var targetStep = step ?? SelectedFlowStep;
            if (targetStep == null)
            {
                MessageBox.Show("请先选中要配置的流程步骤！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 1. 手动创建ViewModel，传入初始化值（业务参数）
                var pcsConfigViewModel = new PcsConfigViewModel(targetStep.RetPcs);

                // 2. 创建窗口（注意类名是PcsConfig，不是PcsConfigView）
                var pcsConfigWindow = new RunVision.Views.TabViews.View.PcsConfig
                {
                    DataContext = pcsConfigViewModel, // 手动绑定ViewModel，替代AutoWire
                    Owner = Application.Current.MainWindow
                };

                // 3. 给ViewModel的CloseAction赋值，实现窗口关闭逻辑
                pcsConfigViewModel.CloseAction = (dialogResult) =>
                {
                    pcsConfigWindow.DialogResult = dialogResult;
                    pcsConfigWindow.Close();
                };

                // 4. 显示模态窗口，获取用户操作结果
                var result = pcsConfigWindow.ShowDialog();
                if (result == true)
                {
                    // 用户点击确定，更新选中步骤的RetPcs
                    targetStep.RetPcs = pcsConfigViewModel.Result;
                    RaisePropertyChanged(nameof(SelectedFlowStep)); // 通知UI更新
                }
                // 取消则不做任何操作
            }
            catch (Exception ex)
            {
                MyLogger.Error($"配置流程 [{targetStep?.StepName}] 的PCS失败: {ex}");
                MessageBox.Show($"PCS配置失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 控制PCS配置按钮是否可用：仅当传入的step不为空时可用
        private bool CanConfigPcs(FlowStepModel step)
        {
            // 优先判断传入的step，兼容直接绑定SelectedFlowStep的场景
            return step != null || SelectedFlowStep != null;
        }

        #endregion

        #region 保存配置（对齐 CameraTab）

        private void OnSaveConfig()
        {
            try
            {
                foreach (var step in FlowSteps)
                {
                    if (string.IsNullOrWhiteSpace(step.StepName))
                    {
                        MessageBox.Show("流程名称不能为空！", "提示",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                // 初始化 SolutionConfig
                if (_configService.ProjectModels.SolutionConfig == null)
                {
                    _configService.ProjectModels.SolutionConfig = new SolutionModel();
                }

                _configService.ProjectModels.SolutionConfig.FlowSteps = FlowSteps.ToList();

                _configService.SaveConfig();

                MessageBox.Show("方案流程配置保存成功！",
                    "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存配置失败：" + ex.Message,
                    "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSave()
        {
            return FlowSteps.Any();
        }

        #endregion
    }
}