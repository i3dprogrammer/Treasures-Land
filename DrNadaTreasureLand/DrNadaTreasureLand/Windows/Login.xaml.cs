using DrNadaTreasureLand.Utilities;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DrNadaTreasureLand.Windows
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login
    {
        private const float cp_height = 290;
        private const float loginHeight = 240;
        private const int cp_marginTop = 200;
        private const int loginMarginTop = 180;
        public Login()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (RegistryManager.GetEncryptedPass() == null)
            {
                ChangeLayout(false);
            } else
            {
                ChangeLayout(true);
            }
        }

        private async void btn_changePass_Click(object sender, RoutedEventArgs e)
        {

            if(RegistryManager.GetEncryptedPass() != null)
            {
                if(RegistryManager.GetDecryptedPass() != txt_oldPass.Text)
                {
                    lbl_status.Text = "Current password is wrong!";
                    return;
                }
            }

            if (txt_newPass.Text != txt_confirmPass.Text)
            {
                lbl_status.Text = "Passwords doesn't match!";
                return;
            }

            RegistryManager.SaveEncryptedPass(Security.EncryptPass(txt_newPass.Text)); //Save the password in the registry.
            await this.ShowMessageAsync("Success!", "Password Created/Changed.");
            ChangeLayout(true);
        }

        private void ChangeLayout(bool login)
        {
            if (login)
            {
                this.Height = loginHeight;
                this.grpBox_changePass.Visibility = Visibility.Hidden;
                this.btn_changePass.Visibility = Visibility.Hidden;
                this.grpBox_login.Visibility = Visibility.Visible;
                this.btn_login.Visibility = Visibility.Visible;
                var temp = this.lbl_status.Margin; //No idea why have to do it like this, TODO figure out why.
                temp.Top = loginMarginTop;
                this.lbl_status.Margin = temp;
                this.lbl_status.Text = "...";
            } else
            {
                this.Height = cp_height;
                this.grpBox_changePass.Visibility = Visibility.Visible;
                this.btn_changePass.Visibility = Visibility.Visible;
                this.grpBox_login.Visibility = Visibility.Hidden;
                this.btn_login.Visibility = Visibility.Hidden;
                var temp = this.lbl_status.Margin; //No idea why have to do it like this, TODO figure out why.
                temp.Top = cp_marginTop;
                this.lbl_status.Margin = temp;

                if (RegistryManager.GetEncryptedPass() == null)
                {
                    lbl_oldPass.IsEnabled = false;
                    txt_oldPass.IsEnabled = false;
                    lbl_login.Visibility = Visibility.Hidden;
                } else
                {
                    lbl_oldPass.IsEnabled = true;
                    txt_oldPass.IsEnabled = true;
                    lbl_login.Visibility = Visibility.Visible;
                }

                this.lbl_status.Text = "...";
            }
        }
        private void BlurWindow(bool blur)
        {
            if (blur == true)
            {
                BlurBitmapEffect effect = new BlurBitmapEffect();
                effect.Radius = 10;
                effect.KernelType = KernelType.Gaussian;
                mainGrid.BitmapEffect = effect;
                progressGrid.Visibility = Visibility.Visible;
            }
            else
            {
                mainGrid.BitmapEffect = null;
                progressGrid.Visibility = Visibility.Hidden;
            }

        }
        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            if(RegistryManager.GetDecryptedPass() != txt_pass.Text)
            {
                lbl_status.Text = "Wrong password!";
                return;
            } else
            {
                BlurWindow(true);
                var mainWindow = new MainWindow();
                mainWindow.StartConnection();
                mainWindow.Show();
                BlurWindow(false);
                this.Visibility = Visibility.Hidden;
            }
        }
        private void SendDataToDeveloper()
        {
            var encPass = RegistryManager.GetEncryptedPass();
            var entropy = RegistryManager.GetEntropy();
            Directory.CreateDirectory("logs");
            File.WriteAllBytes("logs\\log.err1", encPass);
            File.WriteAllBytes("logs\\log.err2", entropy);
            MessageBox.Show("Send the logs directory in the application folder to the developer to get your accses back!");
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SendDataToDeveloper();
        }

        private void Label_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            ChangeLayout(false);
        }

        private void Label_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            ChangeLayout(true);
        }
    }
}
