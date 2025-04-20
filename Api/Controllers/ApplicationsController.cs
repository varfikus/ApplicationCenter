using ApplicationCenter.Api.Data;
using Microsoft.AspNetCore.Mvc;
using ApplicationCenter.Api.Models;
using ApplicationCenter.Api.DTOs;
using static ApplicationCenter.Api.Models.Enum;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using System.Text;
using ApplicationCenter.Api.Services;

namespace ApplicationCenter.Api.Controllers;

/// <summary>
/// Контроллер для обработки заявок пользователей.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ApplicationsController : ControllerBase
{
    private readonly ApplicationCenterContext _context;
    private readonly IWebHostEnvironment _env;

    public ApplicationsController(ApplicationCenterContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    /// <summary>
    /// Создать новую заявку.
    /// </summary>
    /// <remarks>
    /// Принимает форму с данными заявки, сохраняет все файлы и формирует XML-документ с прикреплённой таблицей стилей. 
    /// Заявка и все сопутствующие данные сохраняются в базе данных.
    /// </remarks>
    /// <param name="form">Данные заявки и прикреплённые файлы.</param>
    /// <returns>Результат операции и идентификатор созданной заявки.</returns>
    /// <response code="200">Заявка успешно создана.</response>
    /// <response code="400">Некорректные входные данные.</response>
    /// <response code="500">Внутренняя ошибка сервера при обработке заявки.</response>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SubmitApplication([FromForm] ApplicationRequest form)
    {
        var filePaths = new List<string>();
        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        foreach (var file in form.Files)
        {
            string uniqueName = $"{Guid.NewGuid()}_{file.FileName}";
            string filePath = Path.Combine(uploadsFolder, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            filePaths.Add($"/uploads/{uniqueName}");
        }

        string cssPath = _env.WebRootPath + "/css/xmlstyle.css";
        string embeddedCss = System.IO.File.ReadAllText(cssPath, Encoding.UTF8);

        string xmlFileName = $"application_{Guid.NewGuid()}.xml";
        string xmlPath = Path.Combine(uploadsFolder, xmlFileName);
        string xmlWebPath = $"/uploads/{xmlFileName}";

        XNamespace ns = "http://www.w3.org/1999/xhtml";

        var doc = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(ns + "htmlx",
                new XAttribute(XNamespace.Xml + "lang", "ru"),
                new XAttribute("lang", "ru"),
                new XElement(ns + "body2",
                    new XElement(ns + "container",
                        new XAttribute("id", "electronic-document"),
                        new XElement(ns + "e-doc", "Электронный документ"),
                        new XElement(ns + "content",
                            new XElement(ns + "b",
                                new XElement(ns + "p",
                                    new XAttribute("align", "center"),
                                    "Это текстовый документ формата .XML, подписанный квалифицированной электронной подписью."
                                )
                            ),
                            new XElement(ns + "referencedataorg",
                                new XElement(ns + "nameru", form.FullName),
                                new XElement(ns + "userid", form.UserId),
                                new XElement(ns + "emailaddress", form.Email),
                                new XElement(ns + "phonenumber", form.Phone),
                                new XElement(ns + "servicetype", form.ServiceType),
                                new XElement(ns + "description", form.Description),
                                new XElement(ns + "files",
                                    filePaths.Select(path => new XElement(ns + "file", path))
                                ),
                                new XElement(ns + "vhodregdate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                                new XElement(ns + "status", ApplicationStatus.New.GetDisplayName())
                            )
                        ),
                        new XElement(ns + "style", embeddedCss)
                    )
                )
            )
        );

        doc.Save(xmlPath);

        var application = new Application
        {
            Id = Guid.NewGuid(),
            Fullname = form.FullName,
            Userid = form.UserId,
            Email = form.Email,
            Phone = form.Phone,
            Servicetype = form.ServiceType,
            Description = form.Description,
            Xmlpath = xmlWebPath,
            Files = filePaths,
            Createdat = DateTime.Now,
            Status = ApplicationStatus.New
        };

        _context.Applications.Add(application);
        await _context.SaveChangesAsync();

        return Ok(new { status = "success", applicationId = application.Id });
    }

    /// <summary>
    /// Получить список заявок пользователя.
    /// </summary>
    /// <remarks>
    /// Возвращает все заявки, поданные пользователем по его идентификатору.
    /// </remarks>
    /// <param name="userId">Идентификатор пользователя (GUID в строковом формате).</param>
    /// <returns>Список заявок, отсортированных по дате создания.</returns>
    /// <response code="200">Список заявок успешно получен.</response>
    /// <response code="400">Неверный формат идентификатора пользователя.</response>
    /// <response code="404">Заявки не найдены.</response>
    [HttpGet]
    [Route("{userId}")]
    [ProducesResponseType(typeof(List<Application>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetApplicationsByUser(string userId)
    {
        if (!Guid.TryParse(userId, out Guid userGuid))
            return BadRequest("Invalid userId format.");

        var apps = await _context.Applications
            .Where(a => a.Userid == userGuid)
            .OrderByDescending(a => a.Createdat)
            .ToListAsync();

        return Ok(apps);
    }
}