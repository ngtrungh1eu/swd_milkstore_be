using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class DataContext : DbContext
    {
        public DataContext() { }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }
        public DbSet<PreOrder> PreOrders { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<FavoriteProduct> FavoriteProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure one-to-many relationship between Brand and Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId);

            // Configure many-to-many relationship between Product and Promotion
            modelBuilder.Entity<Product>()
                .HasMany(x => x.Promotes)
                .WithMany(y => y.Products)
                .UsingEntity(j => j.ToTable("ProductPromote"));

            // Configure one-to-many relationship between Role and User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            // Configure one-to-many relationship between User and Blog
            modelBuilder.Entity<Blog>()
                .HasOne(b => b.User)
                .WithMany(u => u.Blogs)
                .HasForeignKey(b => b.UserId);

            // Configure one-to-one relationship between User and Cart
            modelBuilder.Entity<User>()
                .HasOne(u => u.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many relationship between Cart and CartItem
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure many-to-one relationship between CartItem and Product
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-one relationship between User and Favorite
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithOne(u => u.Favorite)
                .HasForeignKey<Favorite>(f => f.UserId);

            // Configure one-to-many relationship between User and Order
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many relationship between Order and ProductOrder
            modelBuilder.Entity<Order>()
                .HasMany(o => o.ProductOrders)
                .WithOne(po => po.Order)
                .HasForeignKey(po => po.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure many-to-one relationship between ProductOrder and Product
            modelBuilder.Entity<ProductOrder>()
                .HasOne(po => po.Product)
                .WithMany(p => p.ProductOrders)
                .HasForeignKey(po => po.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many relationship between User and PreOrder
            modelBuilder.Entity<User>()
                .HasMany(u => u.PreOrders)
                .WithOne(pr => pr.User)
                .HasForeignKey(pr => pr.UserId);

            // Configure one-to-one relationship Product and PreOrder
            modelBuilder.Entity<Product>()
                .HasOne(p => p.PreOrder)
                .WithOne(pr => pr.Product)
                .HasForeignKey<PreOrder>(pr => pr.ProductId);

            // Configure one-to-many relationship between Order and Feedback
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Feedbacks)
                .WithOne(f => f.Order)
                .HasForeignKey(f => f.OrderId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure one-to-one relationship between PreOrder and Feedback
            modelBuilder.Entity<PreOrder>()
                .HasOne(pr => pr.Feedback)
                .WithOne(f => f.PreOrder)
                .HasForeignKey<Feedback>(f => f.PreOrderId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure many-to-one relationship between FavoriteProduct and Favorite
            modelBuilder.Entity<FavoriteProduct>()
                .HasOne(fp => fp.Favorite)
                .WithMany(f => f.FavoriteProducts)
                .HasForeignKey(fp => fp.FavoriteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure many-to-one relationship between FavoriteProduct and Product
            modelBuilder.Entity<FavoriteProduct>()
                .HasOne(fp => fp.Product)
                .WithMany(p => p.FavoriteProducts)
                .HasForeignKey(fp => fp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
