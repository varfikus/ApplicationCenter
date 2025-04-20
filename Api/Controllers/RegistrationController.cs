using ApplicationCenter.Api.DTOs;
using ApplicationCenter.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml.Serialization;
using ApplicationCenter.Api.Data;
using System.Security.Cryptography;

namespace ApplicationCenter.Api.Controllers;

/// <summary>
/// Контроллер для регистрации новых пользователей.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RegistrationController : ControllerBase
{
    private readonly ApplicationCenterContext _context;
    private readonly IWebHostEnvironment _env;

    public RegistrationController(ApplicationCenterContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    /// <summary>
    /// Регистрация нового пользователя.
    /// </summary>
    /// <param name="form">Данные пользователя, переданные в XML-формате.</param>
    /// <returns>
    /// Возвращает результат регистрации, включая статус и ID нового пользователя.
    /// </returns>
    /// <response code="200">Регистрация прошла успешно.</response>
    /// <response code="409">Пользователь с таким логином уже существует.</response>
    [HttpPost("register")]
    [Consumes("application/xml")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegistrationRequest form)
    {
        if (_context.Users.Any(u => u.Login == form.Login))
            return Conflict(new { status = "error", message = "Login already exists" });

        string passwordHash = ComputeSha256Hash(form.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Fullname = form.FullName,
            Email = form.Email,
            Phone = form.Phone,
            Address = form.Address,
            Placeofwork = form.PlaceOfWork,
            Login = form.Login,
            Passwordhash = passwordHash
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { status = "success", userId = user.Id });
    }

    /// <summary>
    /// Хэширует строку с использованием SHA-256.
    /// </summary>
    /// <param name="rawData">Строка для хэширования.</param>
    /// <returns>SHA-256 хэш в нижнем регистре без дефисов.</returns>
    private string ComputeSha256Hash(string rawData)
    {
        using var sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}