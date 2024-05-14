using CloudStorageClass.CloudStorageModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        public string Email { get; set; }
        public string Name { get; set; }

        public string Password { get; set; }

        string url = "https://localhost:7262";

    
        public RegUser( string urls)
        {
            try
            {
                InitializeComponent();
                url = urls+"/api/Users"; ;
            }
            catch (Exception)
            {

            }
        }


        public string json(object obj, int i)
        {
            try
            {
                string str = "";
                Filles files = null;
                User users = null;
                switch (i)
                {
                    case 1:
                        files = (Filles)obj;
                        str = JsonConvert.SerializeObject(files);
                        break;
                    case 2:
                        users = (User)obj;
                        str = JsonConvert.SerializeObject(users);
                        break;

                }
                return str;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(Email))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(url);
                        User users = new User { Id = 0, Name = Name, Password = Password, Email = Email };
                        string str = json(users, 2);

                        HttpContent content = new StringContent(str, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync(url, content);
                        // Проверяем, успешно ли выполнен запрос
                        if (response.IsSuccessStatusCode)
                        {
                            // Считываем содержимое ответа в строку
                            string responseContent = await response.Content.ReadAsStringAsync();
                            MainWindow regUser = new MainWindow();
                            regUser.Owner = this;
                            regUser.Show();
                            this.Hide();
                            // Files = JsonConvert.DeserializeObject<User>(responseContent);
                            // Теперь вы можете работать с содержимым ответа
                            Console.WriteLine("Содержимое ответа: " + responseContent);

                        }
                        else
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();

                            if (responseContent.Contains("Такой пользователь уже есть с таким именем"))
                            {

                                MessageBox.Show("Такой пользователь уже существует");
                                // Делайте что-то, если сообщение найдено
                                Console.WriteLine("Такой пользователь уже существует");
                            }
                            else
                            {
                                MessageBox.Show("Пользователь успешно создан");

                                // Делайте что-то, если сообщение не найдено
                                Console.WriteLine("Пользователь успешно создан");
                            }


                            // Если запрос не удался, вы можете обработать соответствующий случай здесь
                            MessageBox.Show("Ошибка: " + response.StatusCode);
                        }
                    }
               
                }

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
                Name = textBox.Text;
            }
            catch (Exception)
            {

            }
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox textBox = (TextBox)sender;
                Email = textBox.Text;
            }
            catch (Exception)
            {

            }
        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox textBox = (TextBox)sender;

                Password = textBox.Text;

            }
            catch (Exception)
            {

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow regUser = new MainWindow();
                regUser.Owner = this;
                regUser.Show();
                this.Hide();

            }
            catch (Exception)
            {

            }
        }
    }
}
