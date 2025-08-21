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
public class TodosController : ControllerBase
{
    private readonly AppDbContext _db;
    public TodosController(AppDbContext db) => _db = db;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public Task<List<TodoItem>> Get() =>
        _db.Todos.Where(t => t.UserId == UserId).OrderBy(t => t.Id).ToListAsync();

    [HttpPost]
    public async Task<ActionResult<TodoItem>> Create(TodoItem item)
    {
        item.Id = 0; item.UserId = UserId;
        _db.Todos.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoItem>> GetById(int id)
    {
        var item = await _db.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId);
        return item is null ? NotFound() : item;
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TodoItem dto)
    {
        var item = await _db.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId);
        if (item is null) return NotFound();
        item.Title = dto.Title; item.IsDone = dto.IsDone;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId);
        if (item is null) return NotFound();
        _db.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}