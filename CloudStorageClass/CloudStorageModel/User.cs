using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudStorageClass.CloudStorageModel
{
    /// <summary>
    /// User класс
    /// </summary>
    public class User
    {
        /// <summary>
        /// Id Пользователя
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Имя Пользователя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Почта пользователя
        /// </summary>
        public string Email { get; set; }
    }
}
