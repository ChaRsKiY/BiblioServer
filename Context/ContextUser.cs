using BiblioServer.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;

namespace BiblioServer.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            if (Database.EnsureCreated())
            {
                Users.Add(new User { UserName = "ChaRsKiY", Email = "mtovkay@gmail.com", RegistrationDate = DateTime.Now });
                SaveChanges();
            }
        }
    }


}
