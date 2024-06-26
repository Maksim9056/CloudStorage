﻿using CloudStorageClass.CloudStorageModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
using static System.Net.WebRequestMethods;
using static System.Reflection.Metadata.BlobBuilder;

namespace CloudStorageWPF
{
    /// <summary>
    /// Логика взаимодействия для Logout.xaml
    /// </summary>
    public partial class Logout : Window
    {
        User User { get; set; }
        string url = "https://localhost:7262";
        string UrlFillesApi = "/api/Filles/";
                        WorkSeting workSeting = new WorkSeting();

        public Logout(User user,  string url    )
        {
            try
            {
                User = user;
                InitializeComponent();
                NameUser.Content = user.Name;
                this.url = url;
                Thread thread = new Thread(async () => await FetchDataFromApi(user));

                // Запускаем поток
                thread.Start();

                // Ожидаем завершения выполнения потока
                thread.Join();
             

            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Получение данных из api
        /// </summary>
        /// <returns></returns>
        private async Task FetchDataFromApi(User user)
        {
            List<Filles> filles = new List<Filles>();
            try
            {
                // Create an instance of HttpClient
                using (HttpClient client = new HttpClient())
                {
                    // API endpoint
                    client.BaseAddress = new Uri(url + UrlFillesApi+"user" + user.Id);
                    // Send a GET request to the API
                    HttpResponseMessage response = await client.GetAsync(url+ UrlFillesApi+ "user" +user.Id);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content as a string
                        string jsonString = await response.Content.ReadAsStringAsync();

                        // Deserialize the JSON string to a list of Book objects
                        filles = JsonConvert.DeserializeObject<List<Filles>>(jsonString);
                    }
                    else
                    {
                        MessageBox.Show("Failed to retrieve data from the API. Status code: " + response.StatusCode);
                    }
                }
                Data.ItemsSource = filles;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " );
            }

            // Return an empty list if there is an error or no data retrieved
        }
        private void Data_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
               await  FetchDataFromApi(User);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " );
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
        //private async void UIElement_OnDrop(object sender, DragEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        //    {
        //        // Получить путь к перетаскиваемому файлу
        //        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

        //            // Выполнить нужные операции с путем к файлу
        //        foreach (string file in files)
        //        {
        //            using (HttpClient client = new HttpClient())
        //            {
        //                Filles s = new Filles { Id = 0,  TypeFiles = "", StoragePath = "", UserId = User.Id };

        //                using (FileStream fileStream = new FileStream(file, FileMode.OpenOrCreate))
        //                {
        //                    using (MemoryStream memoryStream = new MemoryStream())
        //                    {

        //                        fileStream.CopyTo(memoryStream);
        //                        //await memoryStream.CopyToAsync(fileStream);
        //                        s.NameFille = System.IO.Path.GetFileName(fileStream.Name);
        //                        s.Size = fileStream.Length;
        //                        s.Fille = memoryStream.ToArray();
        //                    }
        //                }
        //                string urlS = "/api/Filles";

        //                    //StreamContent fileContent = new StreamContent(s);

        //                    // Создаем экземпляр MultipartFormDataContent для отправки данных в формате multipart/form-data
        //                    var formData = new MultipartFormDataContent();

        //                    // Добавляем данные класса Filles ByteArrayContent
        //                    formData.Add(new StringContent(s.Id.ToString()), "Id");
        //                    formData.Add(new StringContent(s.StoragePath), "StoragePath");
        //                    formData.Add(new StringContent(s.NameFille), "NameFille");
        //                    //formData.Add(fileContent, "Fille", filles.NameFille);
        //                    formData.Add(new ByteArrayContent(s.Fille), "Fille", s.NameFille); // Возможно, ошибка здесь

        //                    //formData.Add(new StringContent(filles.Fille), "Fille", );
        //                    formData.Add(new StringContent(s.TypeFiles), "TypeFiles");
        //                    formData.Add(new StringContent(s.Size.ToString()), "Size");
        //                    formData.Add(new StringContent(s.UserId.ToString()), "UserId");
        //                    //
        //                    // Отправляем POST-запрос на указанный URL
        //                    //var response = await client.PostAsync("http://localhost:5127/upload", formData);
        //                    var response = await client.PostAsync("http://localhost:5127/upload", formData);

        //                    //client.BaseAddress = new Uri(url+ urlS);
        //                    //string jsonString = json(s, 1);
        //                    //HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
        //                    //var response = await client.PostAsync(url + urlS, content);

        //                if (response.IsSuccessStatusCode)
        //                {
        //                    string responseContent = await response.Content.ReadAsStringAsync();
        //                    MessageBox.Show("Файл успешно отправился");
        //                }
        //                else
        //                {
        //                    MessageBox.Show("Ошибка: " + response.StatusCode);
        //                }

        //                // Здесь вы можете использовать путь к файлу
        //                //MessageBox.Show($"Путь к файлу: {file}");
        //            }
        //        }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        private async void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    // Получить путь к перетаскиваемому файлу
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                    // Используйте один экземпляр HttpClient для всех запросов
                    using (HttpClient client = new HttpClient())
                    {
                        foreach (string file in files)
                        {
                            using (FileStream fileStream = new FileStream(file, FileMode.OpenOrCreate))
                            {
                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    fileStream.CopyTo(memoryStream);
                                    Filles s = new Filles
                                    {
                                        Id = 0,
                                        TypeFiles = "",
                                        StoragePath = "",
                                        UserId = User.Id,
                                        NameFille = System.IO.Path.GetFileName(fileStream.Name),
                                        Size = fileStream.Length,
                                        Fille = memoryStream.ToArray()
                                    };

                                    // Создаем экземпляр MultipartFormDataContent для отправки данных в формате multipart/form-data
                                    var formData = new MultipartFormDataContent();

                                    // Добавляем данные класса Filles ByteArrayContent
                                    formData.Add(new StringContent(s.Id.ToString()), "Id");
                                    formData.Add(new StringContent(s.StoragePath), "StoragePath");
                                    formData.Add(new StringContent(s.NameFille), "NameFille");
                                    formData.Add(new ByteArrayContent(s.Fille), "Fille", s.NameFille); // Возможно, ошибка здесь
                                    formData.Add(new StringContent(s.TypeFiles), "TypeFiles");
                                    formData.Add(new StringContent(s.Size.ToString()), "Size");
                                    formData.Add(new StringContent(s.UserId.ToString()), "UserId");

                                    // Отправляем POST-запрос на указанный URL
                                    var response = await client.PostAsync(url + "/upload", formData);

                                    if (response.IsSuccessStatusCode)
                                    {
                                        string responseContent = await response.Content.ReadAsStringAsync();
                                        MessageBox.Show("Файл успешно отправился");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Ошибка: " + response.StatusCode);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void UIElement_OnDragOver(object sender, DragEventArgs e)
        {

        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow regUser = new MainWindow();
                regUser.Owner = this;
                regUser.Show();
                this.Hide();
            }
            catch (Exception )
            {
            }
        }
    }
}
