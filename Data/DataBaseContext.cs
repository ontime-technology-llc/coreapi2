using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

namespace WebApi.Data
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) { }

        // Table бүртгүүлэх
        public DbSet<User> Users { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<User>(entity =>
        //    {
        //        entity.ToTable("User");
        //        entity.HasKey("Id");
        //        entity.Property(e=>e.FirstName).HasMaxLength(50);
        //        entity.Property(e=>e.LastName).HasMaxLength(50);
        //        entity.Property(e=>e.Username).HasMaxLength(50);
        //        entity.Property(e=>e.Password).HasMaxLength(50);
        //    });
        //}
        //private void MigrateDatabase()
        //{
        //    // Бааз болон table - үүсгэж байгаа үйлдэл
        //    Database.Migrate();
        //}
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    // Баазтай холбогдох тохиргоо

        //    // 1. SqlServerAuthentication - тай холбогдох үед
        //    string connectionString = "Server=192.82.95.103,1456;Database=CoreApiDb;User Id=sa;Password=0nTime@123;TrustServerCertificate=true;";

        //    // 2. Windows Authentication - тай холбогдох үед
        //    //string connectionString = "Data Source=DESKTOP-79FDGV8;Initial Catalog=WebPos;Integrated Security=True;";
        //    //string connectionString = "Data Source=./MSSQLSERVER;Initial Catalog=WebPos;Integrated Security=True;";
        //    optionsBuilder.UseSqlServer(connectionString);
        //}
    }
}
