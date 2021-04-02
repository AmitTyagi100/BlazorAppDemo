using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CrossCuttingEntities;

namespace BlazorDemoAPI.Models
{
    public class BlazorDemoDbContext : DbContext


    {
        public BlazorDemoDbContext(DbContextOptions<BlazorDemoDbContext> option) : base(option) { }

        public DbSet<User> tblUser { get; set; }

        public DbSet<UserInfo> tblUserInfo { get; set; }

        public DbSet<Department> tblDepartment { get; set; }

    }
}
