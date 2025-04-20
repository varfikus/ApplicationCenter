using System.ComponentModel.DataAnnotations;

namespace ApplicationCenter.Api.DTOs
{
    /// <summary>
    /// Модель запроса на подачу заявки через форму.
    /// </summary>
    public class ApplicationRequest
    {
        /// <summary>
        /// Полное имя.
        /// </summary>
        [Required]
        public string FullName { get; set; }

        /// <summary>
        /// Уникальный идентификатор пользователя (GUID).
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Адрес электронной почты.
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Контактный номер телефона.
        /// </summary>
        [Required]
        public string Phone { get; set; }

        /// <summary>
        /// Тип услуги.
        /// </summary>
        [Required]
        public string ServiceType { get; set; }

        /// <summary>
        /// Дополнительное описание или комментарии к заявке.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Путь к XML-файлу заявки.
        /// </summary>
        public string? XmlPath { get; set; }

        /// <summary>
        /// Список прикреплённых файлов.
        /// </summary>
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();
    }
}
