using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon.Primitives;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox textBox = (TextBox)sender;
                Email = textBox.Text;
            }
            catch(Exception)
            {

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RegUser regUser = new RegUser();
                regUser.Owner = this;
                regUser.Show();
                this.Hide();

            }
            catch(Exception)
            {

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RegUser regUser = new RegUser();
            regUser.Owner = this;
            regUser.Show();
            this.Hide();
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Password = textBox.Text;
        }
    }
}