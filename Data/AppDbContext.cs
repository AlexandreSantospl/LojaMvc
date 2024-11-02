using Microsoft.EntityFrameworkCore;
using Models;
using SetorDeCompras.Models;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ProdutosModel> Produtos { get; set; }

        public DbSet<AuthValidationModel> AuthValidation { get; set; }
    }
}