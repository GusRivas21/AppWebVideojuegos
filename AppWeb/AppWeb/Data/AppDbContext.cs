using AppWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AppWeb.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Videojuego> Videojuegos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Correo)
                .IsUnique();

            modelBuilder.Entity<Compra>()
                .HasOne(c => c.Usuarios)
                .WithMany(u => u.Compras)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade); 

                modelBuilder.Entity<Compra>()
                .HasOne(c => c.Videojuegos)
                .WithMany(v => v.Compras)
                .HasForeignKey(c => c.VideojuegoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
