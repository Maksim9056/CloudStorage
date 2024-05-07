using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudStorageClass.CloudStorageModel
{
    internal class FillesView
    {   /// <summary>
        /// Id Файла класса для Database DB Class
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Локальный путь ввиде сылки 
        /// </summary>
        public string StoragePath { get; set; }

        /// <summary>
        /// Имя файла
        /// </summary>
        public string NameFille { get; set; }
        /// <summary>
        /// Размер
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// Id Пользователя ссылка на  пользователя который будет создан каталог 
        /// </summary>
        public User UserId { get; set; }
    }
}
