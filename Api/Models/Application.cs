using System;
using System.Collections.Generic;

namespace ApplicationCenter.Api.Models;

/// <summary>
/// Представляет собой поданную пользователем заявку.
/// </summary>
public partial class Application
{
    /// <summary>
    /// Уникальный идентификатор заявки.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя, подавшего заявку.
    /// </summary>
    public Guid? Userid { get; set; }

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
    /// Тип выбранной услуги.
    /// </summary>
    public string? Servicetype { get; set; }

    /// <summary>
    /// Дополнительное описание заявки.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Статус заявки.
    /// </summary>
    public Enum.ApplicationStatus? Status { get; set; }

    /// <summary>
    /// Дата и время подачи заявки.
    /// </summary>
    public DateTime? Createdat { get; set; }

    /// <summary>
    /// Относительный путь к XML-файлу заявки.
    /// </summary>
    public string? Xmlpath { get; set; }

    /// <summary>
    /// Список путей к прикреплённым файлам.
    /// </summary>
    public List<string>? Files { get; set; }

    /// <summary>
    /// Пользователь, связанный с заявкой.
    /// </summary>
    public virtual User? User { get; set; }
}