using System;
using Microsoft.EntityFrameworkCore;

namespace wasabi.Models{
    public class AppDbContext: DbContext
    {

        // protected override void OnModelCreating(ModelBuilder modelBuilder) {
        // //    modelBuilder.Seed();
        // }


        // protected override void OnConfiguring(DbContextOptionsBuilder dbctxoptbuilder   ) {
        //     dbctxoptbuilder.UseSqlServer();
        // }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users {get; set;}   
        public DbSet<Fl> Fls {get;set;}
    }
}