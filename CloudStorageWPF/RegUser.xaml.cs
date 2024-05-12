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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CloudStorageWPF
{
    /// <summary>
    /// Логика взаимодействия для RegUser.xaml
    /// </summary>
    public partial class RegUser : Window
    {
        public RegUser()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow regUser = new MainWindow();
                regUser.Owner = this;
                regUser.Show();
                this.Hide();

            }catch (Exception)
            {

            }

        }
    }
}
