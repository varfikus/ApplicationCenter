using ApplicationCenter.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml.Serialization;
using System.Security.Cryptography;
using ApplicationCenter.Api.Data;

namespace ApplicationCenter.Api.Controllers;

/// <summary>
/// Контроллер для авторизации пользователей.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly ApplicationCenterContext _context;

    public AuthorizationController(ApplicationCenterContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Авторизация пользователя или администратора.
    /// </summary>
    /// <param name="request">Запрос с логином и паролем пользователя.</param>
    /// <returns>
    /// Возвращает объект с информацией о пользователе и ссылкой для перенаправления при успешной авторизации.
    /// В случае ошибки возвращает соответствующий статус.
    /// </returns>
    /// <response code="200">Успешная авторизация. Возвращает пользователя и URL для перенаправления.</response>
    /// <response code="401">Неверный логин или пароль.</response>
    [HttpPost]
    [Route("authorize")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Authorize([FromBody] AuthorizationRequest request)
    {
        if (request.Login == "admin" && request.Password == "admin")
        {
            return Ok(new
            {
                status = "success",
                redirectUrl = "/admin/index.html",
                user = new
                {
                    id = 0,
                    login = "admin"
                }
            });
        }

        var hashedPassword = ComputeSha256Hash(request.Password);
        var user = _context.Users.FirstOrDefault(u => u.Login == request.Login && u.Passwordhash == hashedPassword);

        if (user == null)
        {
            return Unauthorized(new { status = "error", message = "Invalid credentials" });
        }

        return Ok(new
        {
            status = "success",
            redirectUrl = "/index.html",
            user = new
            {
                id = user.Id,
                login = user.Login
            }
        });
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