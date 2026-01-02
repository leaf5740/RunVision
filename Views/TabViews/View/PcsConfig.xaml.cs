using RunVision.ViewModels.TabViews.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RunVision.Views.TabViews.View
{
    /// <summary>
    /// PcsConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PcsConfig : Window
    {
        public PcsConfig()
        {
            InitializeComponent();

            Loaded += (_, __) =>
            {
                if (DataContext is PcsConfigViewModel vm)
                {
                    vm.CloseAction = r =>
                    {
                        DialogResult = r;
                        Close();
                    };
                }
            };

        }
    }
}
