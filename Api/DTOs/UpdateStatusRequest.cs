using System.ComponentModel.DataAnnotations;
using static ApplicationCenter.Api.Models.Enum;

namespace ApplicationCenter.Api.DTOs
{
    /// <summary>
    /// Модель запроса для обновления статуса заявки и загрузки XML-файла.
    /// </summary>
    public class UpdateStatusRequest
    {
        /// <summary>
        /// Новый статус заявки. Должен быть одним из значений перечисления <see cref="ApplicationStatus"/>.
        /// </summary>
        [Required]
        [EnumDataType(typeof(ApplicationStatus))]
        public ApplicationStatus Status { get; set; }

        /// <summary>
        /// XML-файл заявки.
        /// </summary>
        [Required]
        public IFormFile XmlFile { get; set; } = default!;
    }
}
