using Microsoft.EntityFrameworkCore;
using Project.Modules.Inventories.Entites;
using Project.Modules.Products.Entities;
using Project.Modules.DeClarations.Entites;
using Project.Modules.FileUploads.Entities;
using Project.Modules.Seals.Entity;
using Project.Modules.Users.Entities;
using Project.Modules.Destroys.Entities;
using Project.Modules.Sells.Entities;
using Project.Modules.Exchangerates.Entities;
using Project.Modules.Declarations.Entites;

namespace Project.App.Database
{
    public class MariaDBContext : DbContext
    {
        public DbSet<Module> Modules { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<DepartmentPermissions> DepartmentPermissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<FileUpload> FileUploads { get; set; }
        public DbSet<Seal> Seals { get; set; }

        public DbSet<Sell> Sells { get; set; }
        public DbSet<SellDetail> SellDetails { get; set; }

        public MariaDBContext(DbContextOptions<MariaDBContext> options) : base(options)
        {

        }

        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<MenuDetail> MenuDetails { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<DeClaration> Declarations { get; set; }
        public DbSet<DeClarationDetail> DeClarationDetails { get; set; }
        public DbSet<SealDetail> SealDetails { get; set; }
        public DbSet<SealProduct> SealProducts { get; set; }
        public DbSet<Destroy> Destroys { get; set; }
        public DbSet<DestroyDetail> DestroyDetails { get; set; }
        public DbSet<Citypair> Citypairs { get; set; }
        public DbSet<CopySeal> CopySeals { get; set; }
        public DbSet<Airport> Airports { get; set; }
        public DbSet<Exchangerate> Exchangerates { get; set; }
        public DbSet<DeclarationExtension> DeclarationExtensions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>().HasQueryFilter(p => p.Enable);
            modelBuilder.Entity<Permission>().HasQueryFilter(p => p.Enable);
            modelBuilder.Entity<Module>().HasQueryFilter(p => p.Enable);
            modelBuilder.Entity<User>().HasQueryFilter(p => p.Enable);
        }


    }   
    }
