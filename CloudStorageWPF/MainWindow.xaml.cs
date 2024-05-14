using CloudStorageClass.CloudStorageModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
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
using static System.Reflection.Metadata.BlobBuilder;

namespace CloudStorageWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Email { get; set; }
        
        public string Password { get; set; }
        string url = "https://localhost:7262";
        string urls = "/api/Users/";
        public MainWindow()
        {
            try
            {

                InitializeComponent();
                WorkSeting workSeting = new WorkSeting();

                workSeting.CreateFileSeting();
                workSeting.ReadFillesSeting();

                var result = workSeting.urlss;
                url = result;

            }
            catch (Exception)
            {
            
            }
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

        private  void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
             
                RegUser regUser = new RegUser(url);
                regUser.Owner = this;
                regUser.Show();
                this.Hide();

            }
            catch(Exception)
            {

            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(Email))
                {
                    User user = new User();
                    using (HttpClient client = new HttpClient())
                    {
                        // API endpoint
                        client.BaseAddress = new Uri(url + urls + Email + "," + Password);
                        // Send a GET request to the API
                        HttpResponseMessage response = await client.GetAsync(url+ urls + Email + "," + Password);

                        // Check if the response is successful
                        if (response.IsSuccessStatusCode)
                        {
                            // Read the response content as a string
                            string jsonString = await response.Content.ReadAsStringAsync();

                            // Deserialize the JSON string to a list of Book objects
                            user = JsonConvert.DeserializeObject<User>(jsonString);
                            Logout regUser = new Logout(user,url);
                            regUser.Owner = this;
                            regUser.Show();
                            this.Hide();
                            // Return the list of books
                            //client.BaseAddress = new Uri("https://your.api.endpoint.com/");
                            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        }
                        else
                        {
                            MessageBox.Show("Failed to retrieve data from the API. Status code: " + response.StatusCode);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Password = textBox.Text;
        }
    }
}