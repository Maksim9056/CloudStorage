using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace CloudStorageWPF
{
    public class WorkSeting
    {
        string path = AppDomain.CurrentDomain.BaseDirectory;

        string NameFiles = "seting.txt";

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



        public async Task<string> ReadFillesSeting()
        {
            try
            {
                string url = "";
                var paths = System.IO.Path.Combine(path, NameFiles);

                using (FileStream fileStream = new FileStream(paths, FileMode.OpenOrCreate))
                {
                    byte[] buffer = new byte[fileStream.Length];

                    await fileStream.ReadAsync(buffer, 0, buffer.Length);
                    url = Encoding.Default.GetString(buffer);
                }
                return url;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
