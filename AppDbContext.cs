using Microsoft.EntityFrameworkCore;
using FitVaga.API.Models;

namespace FitVaga.API;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Curriculo> Curriculos { get; set; }
    public DbSet<Vaga> Vagas { get; set; }
}