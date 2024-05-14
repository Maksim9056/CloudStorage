using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace CloudStorageWPF
{
    public class WorkSeting
    {
        string path = AppDomain.CurrentDomain.BaseDirectory;

        string NameFiles = "seting.txt";
        public string urlss { get; set; } = "";
        public async void CreateFileSeting()
        {
            try
            {
                var paths = System.IO.Path.Combine(path, NameFiles);

                if (!File.Exists(paths))
                {
                    using (FileStream fileStream = new FileStream(paths, FileMode.OpenOrCreate))
                    {
                        byte[] bufer = Encoding.Default.GetBytes("https://localhost:7262");
                        await fileStream.WriteAsync(bufer, 0, bufer.Length);

                    }
                }
                else
                {

                }
            }
            catch (Exception)
            {

            }
        }



        public  void ReadFillesSeting()
        {
            try
            {
                string url = "";
                var paths = System.IO.Path.Combine(path, NameFiles);

                using (FileStream fileStream = new FileStream(paths, FileMode.OpenOrCreate))
                {
                    byte[] buffer = new byte[fileStream.Length];

                     fileStream.Read(buffer, 0, buffer.Length);
                    urlss = Encoding.Default.GetString(buffer);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
