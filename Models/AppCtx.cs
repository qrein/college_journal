using EducateApp.Models.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EducateApp.Models
{
    public class AppCtx : IdentityDbContext<User>
    {
        public AppCtx(DbContextOptions<AppCtx> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<FormOfStudy> FormsOfStudy { get; set; }

        public DbSet<TypeOfTotal> TypesOfTotals { get; set; }

        public DbSet<Discipline> Disciplines { get; set; }


        public DbSet<Specialty> Specialties { get; set; }

        public DbSet<Group> Groups { get; set; }
    }
}
