using Microsoft.EntityFrameworkCore;
using NewMyProject.Entities;

namespace NewMyProject.Data
{
    public class EfContext : DbContext
    {
        public EfContext(DbContextOptions dbContextOptions)
           : base(dbContextOptions)
        {
        }

        //Регистрируем DbSet он кстати содержит IDisposable, есть автоматическая чистка мусора(но в некоторые моменты лучше воспользоваться Destructor либо system.object.finilaze()
        public DbSet<User> LoginModels { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Profile> Profiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Настройка связи профиля по следующему принципу - у профиля есть множество заказов.
            modelBuilder.Entity<Profile>().HasMany(p => p.orders).WithOne();
        }
    }
}
