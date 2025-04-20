using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ApplicationCenter.Api.DTOs
{
    /// <summary>
    /// Модель запроса на регистрацию нового пользователя.
    /// </summary>
    [XmlRoot("RegistrationRequest")]
    public class RegistrationRequest
    {
        /// <summary>
        /// Полное имя пользователя.
        /// </summary>
        [XmlElement("fullName")]
        [Required]
        public string FullName { get; set; }

        /// <summary>
        /// Адрес электронной почты пользователя.
        /// </summary>
        [XmlElement("email")]
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Номер телефона пользователя.
        /// </summary>
        [XmlElement("phone")]
        [Required]
        public string Phone { get; set; }

        /// <summary>
        /// Адрес проживания пользователя.
        /// </summary>
        [XmlElement("address")]
        [Required]
        public string Address { get; set; }

        /// <summary>
        /// Место работы пользователя.
        /// </summary>
        [XmlElement("placeOfWork")]
        [Required]
        public string PlaceOfWork { get; set; }

        /// <summary>
        /// Логин пользователя.
        /// </summary>
        [XmlElement("login")]
        [Required]
        public string Login { get; set; }

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        [XmlElement("password")]
        [Required]
        public string Password { get; set; }
    }
}
