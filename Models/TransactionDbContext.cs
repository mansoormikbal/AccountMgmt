
using EntityFrameworkCore.EncryptColumn.Extension;
using EntityFrameworkCore.EncryptColumn.Interfaces;
using EntityFrameworkCore.EncryptColumn.Util;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountMgmt;

namespace AccountMgmt.Models
{
    public class TransactionDbContext : DbContext
    {
        private readonly IEncryptionProvider _provider;

   
        public TransactionDbContext(DbContextOptions<TransactionDbContext> options) : base(options)
        { 
            
        }

   

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply encryption converter
            modelBuilder.Entity<Transaction>()
                .Property(c => c.Narration)
                .HasConversion(new EncryptionConverter());

         
        }
    }
}
