using System.ComponentModel.DataAnnotations;

namespace ApplicationCenter.Api.Models
{
    public class Enum
    {
        /// <summary>
        /// Перечисление, определяющее текущий статус заявки.
        /// </summary>
        public enum ApplicationStatus
        {
            /// <summary>
            /// Новая заявка, ещё не обработана.
            /// </summary>
            [Display(Name = "Новая")]
            New,

            /// <summary>
            /// Заявка находится в процессе обработки.
            /// </summary>
            [Display(Name = "В работе")]
            InProgress,

            /// <summary>
            /// Заявка завершена.
            /// </summary>
            [Display(Name = "Завершена")]
            Completed
        }
    }
}
