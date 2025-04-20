namespace ApplicationCenter.Api.DTOs
{
    /// <summary>
    /// Модель запроса для авторизации пользователя.
    /// </summary>
    public class AuthorizationRequest
    {
        /// <summary>
        /// Логин пользователя.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        public string Password { get; set; }
    }
}
