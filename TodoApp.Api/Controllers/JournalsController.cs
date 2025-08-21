using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TodoApp.Api.Data;
using TodoApp.Api.DTO;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JournalsController : ControllerBase
{
    private readonly AppDbContext _db;
    public JournalsController(AppDbContext db) => _db = db;
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public Task<List<JournalEntry>> Get(string? q = null)
    {
        var x = _db.Journals.Where(j => j.UserId == UserId);
        if (!string.IsNullOrWhiteSpace(q)) x = x.Where(j => j.Title.Contains(q) || j.Content.Contains(q));
        return x.OrderByDescending(j => j.CreatedUtc).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<JournalEntry>> Create(JournalEntry e)
    {
        e.Id = 0; e.UserId = UserId; e.CreatedUtc = DateTime.UtcNow; e.UpdatedUtc = null;
        _db.Journals.Add(e);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = e.Id }, e);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<JournalEntry>> GetById(int id)
    {
        var e = await _db.Journals.FirstOrDefaultAsync(j => j.Id == id && j.UserId == UserId);
        return e is null ? NotFound() : e;
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, JournalEntry dto)
    {
        var e = await _db.Journals.FirstOrDefaultAsync(j => j.Id == id && j.UserId == UserId);
        if (e is null) return NotFound();
        e.Title = dto.Title; e.Content = dto.Content; e.UpdatedUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await _db.Journals.FirstOrDefaultAsync(j => j.Id == id && j.UserId == UserId);
        if (e is null) return NotFound();
        _db.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
