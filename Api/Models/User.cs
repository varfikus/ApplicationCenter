using System;
using System.Collections.Generic;

namespace ApplicationCenter.Api.Models;

/// <summary>
/// Представляет пользователя системы.
/// </summary>
public partial class User
{
    /// <summary>
    /// Уникальный идентификатор пользователя.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Полное имя пользователя.
    /// </summary>
    public string? Fullname { get; set; }

    /// <summary>
    /// Адрес электронной почты пользователя.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Номер телефона пользователя.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Адрес пользователя.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Место работы пользователя.
    /// </summary>
    public string? Placeofwork { get; set; }

    /// <summary>
    /// Логин пользователя.
    /// </summary>
    public string? Login { get; set; }

    /// <summary>
    /// Хэш пароля пользователя.
    /// </summary>
    public string? Passwordhash { get; set; }

    /// <summary>
    /// Коллекция заявок, связанных с этим пользователем.
    /// </summary>
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
}