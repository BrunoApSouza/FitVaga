using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitVaga.API.DTOs;
using FitVaga.API.Services;

namespace FitVaga.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnaliseController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AnaliseFitService _analiseFitService;

    public AnaliseController(AppDbContext context, AnaliseFitService analiseFitService)
    {
        _context = context;
        _analiseFitService = analiseFitService;
    }

    [HttpPost]
    public async Task<IActionResult> Analisar([FromBody] AnaliseFitRequest request)
    {
        var curriculo = await _context.Curriculos.FindAsync(request.CurriculoId);
        if (curriculo == null)
            return NotFound("Currículo não encontrado.");

        var resultado = await _analiseFitService.AnalisarFit(
            curriculo.Conteudo,
            request.DescricaoVaga);

        return Ok(resultado);
    }
}