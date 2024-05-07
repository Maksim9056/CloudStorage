namespace CloudStorageClass.CloudStorageModel
{
    /// <summary>
    /// Класс для  файлов  
    /// </summary>
    public class Filles
    {
        /// <summary>
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
        /// Будет нужно для хранилища
        /// </summary>
        public byte [] Fille { get; set; }
        /// <summary>
        /// Размер
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// Id Пользователя ссылка на  пользователя который будет создан каталог 
        /// </summary>
        public int UserId { get; set; }

    }
}
