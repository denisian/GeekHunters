using Microsoft.EntityFrameworkCore;
using GeekHunter.ViewModels;
using System.IO;
using System;

namespace GeekHunter.Models
{
    public class GeekHuntersDbContext : DbContext
    {
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<CandidateSkillViewModel> CandidateSkills { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=GeekHunter.sqlite");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CandidateSkillViewModel>()
                .HasKey(cs => new { cs.CandidateId, cs.SkillId });
            modelBuilder.Entity<CandidateSkillViewModel>()
                .HasOne(cs => cs.Candidate)
                .WithMany(c => c.Skills)
                .HasForeignKey(cs => cs.CandidateId);
            modelBuilder.Entity<CandidateSkillViewModel>()
                .HasOne(cs => cs.Skill)
                .WithMany(c => c.Candidates)
                .HasForeignKey(cs => cs.SkillId);
        }
    }
}
