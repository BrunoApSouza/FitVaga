using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitVaga.API.Models;

namespace FitVaga.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurriculosController : ControllerBase
{
    private readonly AppDbContext _context;

    public CurriculosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var curriculos = await _context.Curriculos.ToListAsync();
        return Ok(curriculos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var curriculo = await _context.Curriculos.FindAsync(id);
        if (curriculo == null) return NotFound();
        return Ok(curriculo);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Curriculo curriculo)
    {
        _context.Curriculos.Add(curriculo);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = curriculo.Id }, curriculo);
    }
}