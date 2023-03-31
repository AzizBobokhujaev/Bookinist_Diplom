using Bookinist.Models.DTO;
using Bookinist.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Bookinist.Context
{
    public class BookinistContext : IdentityDbContext<User,IdentityRole<int>,int>
    {
        public BookinistContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public override DbSet<User> Users { get; set; }
        public DbSet<AudioBook>AudioBooks { get; set; }
        public DbSet<PdfBook> PdfBooks { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(entity => entity.ToTable("Users"));
            builder.Entity<IdentityRole<int>>(entity => entity.ToTable("Roles"));
            builder.Entity<IdentityUserRole<int>>(entity => entity.ToTable("UserRoles"));
            builder.Entity<IdentityUserClaim<int>>(entity => entity.ToTable("UserClaims"));
            builder.Entity<IdentityUserLogin<int>>(entity => entity.ToTable("UserLogins"));
            builder.Entity<IdentityUserToken<int>>(entity => entity.ToTable("UserTokens"));
            builder.Entity<IdentityRoleClaim<int>>(entity => entity.ToTable("RoleClaims"));

            builder.Entity<Category>().HasData
                (
                    new Category { Id = 1, Name = "Детектив" },
                        new Category { Id = 2, Name = "Фантастика" },
                        new Category { Id = 3, Name = "Приключения" },
                        new Category { Id = 4, Name = "Роман" },
                        new Category { Id = 5, Name = "Научная книга" },
                        new Category { Id = 6, Name = "Фольклор" },
                        new Category { Id = 7, Name = "Юмор" },
                        new Category { Id = 8, Name = "Справочная книга" }
                );
          
        }
    }
}
