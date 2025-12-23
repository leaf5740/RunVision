using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Windows;
using System.Windows.Input;
using HC = HandyControl.Controls;

namespace RunVision.ViewModels
{
    public class LoginWindowViewModel : BindableBase
    {
        public ICommand LoginCommand { get; }

        public LoginWindowViewModel()
        {
            LoginCommand = new DelegateCommand<object>(OnLogin);
        }

        private void OnLogin(object parameter)
        {
            if (parameter is HandyControl.Controls.PasswordBox pwdBox)
            {
                string password = pwdBox.Password;

                // 登录逻辑
                if (password == "123")
                {
                    //System.Windows.MessageBox.Show("登录成功！");
                    var window = System.Windows.Window.GetWindow(pwdBox);
                    window.DialogResult = true;
                    window.Close();
                }
                else
                {
                    System.Windows.MessageBox.Show("密码错误！");
                    //pwdBox.Clear();
                    //pwdBox.Focus();
                }
            }
        }
    }
}
