using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shoppy.Models.DBEntities;

namespace Shoppy.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ProductTagSellOffer>().HasKey(cs => new { cs.ProductTagId, cs.SellOfferId });
        }

        public DbSet<SellOffer> SellOffers { get; set; }
        public DbSet<TransactionHistory> TransactionHistories { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<ProductTagSellOffer> ProductTagSellOffers { get; set; }
    }
}
