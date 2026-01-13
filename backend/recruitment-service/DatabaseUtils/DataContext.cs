namespace RecruitmentService.DatabaseUtils
{
    using Microsoft.EntityFrameworkCore;
    using VolunteerManagement.Model;

    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             modelBuilder.Entity<RecruitmentCampaign>()
                .HasMany(e => e.Volunteers)
                .WithMany();
            modelBuilder.Entity<RecruitmentCampaign>()
                .HasMany(e => e.Locations)
                .WithMany();
            modelBuilder.Entity<RecruitmentCampaign>()
                .HasIndex(e => e.Name)
                .IsUnique();
            modelBuilder.Entity<Interview>()
                .HasMany(e => e.Interviewers)
                .WithMany();
            modelBuilder.Entity<Interview>()
                .HasIndex(e => new { e.CandidateId, e.LocationId, e.DateTime })
                .IsUnique();
            modelBuilder.Entity<BlockedPeriod>()
                .Property(b => b.Start)
                .HasColumnType("timestamp without time zone");
            modelBuilder.Entity<Interview>()
              .Property(b => b.DateTime)
              .HasColumnType("timestamp without time zone");
            modelBuilder.Entity<VolunteerDisponibility>()
              .Property(b => b.DateTime)
              .HasColumnType("timestamp without time zone");
        }

        public DbSet<Interview> Interviews { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<PersonalInfo> PersonalInfo { get; set; }
        public DbSet<RecruitmentCampaign> RecruitmentCampaigns { get; set; }
        public DbSet<VolunteerDisponibility> VolunteerDisponibilities { get; set; }
        public DbSet<InterviewTemplate> InterviewTemplates { get; set; }
        public DbSet<RecruitmentFormTemplate> RecruitmentFormTemplates { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<BlockedPeriod> BlockedPeriods { get; set; }
    }
}
