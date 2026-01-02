using Prism.Commands;
using Prism.Mvvm;
using System;

namespace RunVision.ViewModels.TabViews.View
{
    public class PcsConfigViewModel : BindableBase
    {
        private string _pcs;
        public string Pcs
        {
            get => _pcs;
            set => SetProperty(ref _pcs, value);
        }

        public string Result => Pcs;

        public DelegateCommand OkCommand { get; }
        public DelegateCommand CancelCommand { get; }

        // 由 Window 注入，用于关闭窗口并返回结果
        public Action<bool?> CloseAction { get; set; }

        // 给initValue加默认值，避免传入null导致Pcs为空
        public PcsConfigViewModel(string initValue = "")
        {
            Pcs = initValue ?? string.Empty; // 双重保障，防止null

            OkCommand = new DelegateCommand(() =>
            {
                CloseAction?.Invoke(true); // 确定：返回true
            });

            CancelCommand = new DelegateCommand(() =>
            {
                CloseAction?.Invoke(false); // 取消：返回false
            });
        }
    }
}