using Microsoft.AspNetCore.Mvc;
using ApplicationCenter.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using ApplicationCenter.Api.DTOs;
using ApplicationCenter.Api.Models;
using static ApplicationCenter.Api.Models.Enum;
using System.Xml;
using System.Text;
using ApplicationCenter.Api.Services;

namespace ApplicationCenter.Api.Controllers;

/// <summary>
/// Контроллер для администратора.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly ApplicationCenterContext _context;
    private readonly IWebHostEnvironment _env;

    public AdminController(ApplicationCenterContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    /// <summary>
    /// Получить список всех заявок
    /// </summary>
    /// <returns>Список всех заявок</returns>
    ///  /// <response code="200">Список заявок успешно получен.</response>
    /// <response code="500">Ошибка при получении данных из базы.</response>
    [HttpGet]
    [Route("allapplications")]
    [ProducesResponseType(typeof(List<Application>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetApplications()
    {
        var apps = await _context.Applications
            .OrderByDescending(a => a.Createdat)
            .ToListAsync();

        return Ok(apps);
    }

    /// <summary>
    /// Получить список заявок с фильтрацией
    /// </summary>
    /// <param name="serviceType">Тип сервиса</param>
    /// <param name="status">Статус заявки</param>
    /// <param name="fullName">ФИО заявителя</param>
    /// <param name="startDate">Дата начала</param>
    /// <param name="endDate">Дата конца</param>
    /// <returns>Список отфильтрованных заявок</returns>
    /// <response code="200">Заявки успешно получены.</response>
    /// <response code="400">Некорректные параметры фильтрации.</response>
    /// <response code="500">Ошибка при получении данных.</response>
    [HttpGet]
    [Route("applications")]
    [ProducesResponseType(typeof(List<Application>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetApplications(
    [FromQuery] string? serviceType,
    [FromQuery] ApplicationStatus? status,
    [FromQuery] string? fullName,
    [FromQuery] DateTime? startDate,
    [FromQuery] DateTime? endDate)
    {
        var query = _context.Applications.AsQueryable();

        if (!string.IsNullOrWhiteSpace(serviceType))
            query = query.Where(a => a.Servicetype == serviceType);

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(fullName))
            query = query.Where(a => a.Fullname.ToLower().Contains(fullName));

        if (startDate.HasValue)
            query = query.Where(a => a.Createdat >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(a => a.Createdat <= endDate.Value);

        var applications = await query
            .OrderByDescending(a => a.Createdat)
            .ToListAsync();

        return Ok(applications);
    }

    /// <summary>
    /// Обновляет статус заявки и соответствующее значение в XML-документе.
    /// </summary>
    /// <param name="id">Идентификатор заявки.</param>
    /// <param name="request">Объект, содержащий новый статус заявки.</param>
    /// <returns>Возвращает результат операции обновления статуса.</returns>
    /// <response code="200">Статус успешно обновлён.</response>
    /// <response code="404">Заявка или XML-файл не найдены.</response>
    [HttpPatch("{id}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromForm] UpdateStatusRequest request)
    {
        var application = await _context.Applications.FindAsync(id);
        if (application == null)
            return NotFound(new { status = "error", message = "Application not found" });

        string xmlPath = _env.WebRootPath + application.Xmlpath;

        if (!System.IO.File.Exists(xmlPath))
            return NotFound(new { status = "error", message = "XML file not found at: " + xmlPath });

        var xmlDoc = new XmlDocument();
        xmlDoc.Load(xmlPath);

        var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
        nsmgr.AddNamespace("x", "http://www.w3.org/1999/xhtml");

        var refNode = xmlDoc.SelectSingleNode("//x:referencedataorg", nsmgr);
        if (refNode == null)
        {
            return BadRequest(new { status = "error", message = "Node <referencedataorg> not found in XML (with namespace)" });
        }

        var statusNode = refNode.SelectSingleNode("x:status", nsmgr);

        if (statusNode != null)
        {
            statusNode.InnerText = request.Status.GetDisplayName();
        }
        else
        {
            var newStatusNode = xmlDoc.CreateElement("status", "http://www.w3.org/1999/xhtml");
            newStatusNode.InnerText = request.Status.GetDisplayName();
            refNode.AppendChild(newStatusNode);
        }

        using (var writer = new XmlTextWriter(xmlPath, new UTF8Encoding(false)))
        {
            writer.Formatting = Formatting.Indented;
            xmlDoc.Save(writer);
        }

        application.Status = request.Status;
        await _context.SaveChangesAsync();

        return Ok(new { status = "success", message = "Status updated" });
    }

    /// <summary>
    /// Удаляет заявку, связанный XML-документ и прикреплённые файлы.
    /// </summary>
    /// <param name="id">Идентификатор заявки.</param>
    /// <returns>Возвращает результат операции удаления.</returns>
    /// <response code="200">Заявка и файлы успешно удалены.</response>
    /// <response code="404">Заявка не найдена.</response>
    /// <response code="500">Ошибка при удалении XML или файлов.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteApplication(Guid id)
    {
        var application = await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application == null)
            return NotFound(new { status = "error", message = "Application not found" });

        var xmlPath = _env.WebRootPath + application.Xmlpath;
        {
            try
            {
                System.IO.File.Delete(xmlPath);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = $"Ошибка удаления XML: {ex.Message}" });
            }
        }

        if (application.Files != null && application.Files.Count > 0)
        {
            foreach (var filePath in application.Files)
            {
                var fullPath = _env.WebRootPath + filePath;
                if (System.IO.File.Exists(fullPath))
                {
                    try
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { status = "error", message = $"Ошибка удаления файла '{filePath}': {ex.Message}" });
                    }
                }
            }
        }

        _context.Applications.Remove(application);
        await _context.SaveChangesAsync();

        return Ok(new { status = "success", message = "Заявка и все файлы успешно удалены" });
    }
}