using Microsoft.EntityFrameworkCore;
using PasswordManagerApp.Models.Entities;

namespace PasswordManagerApp.Models
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserDevice> UserDevices { get; set; }
        public virtual DbSet<Totp_user> Totp_Users { get; set; }
        public virtual DbSet<LoginData> LoginDatas { get; set; }
        public virtual DbSet<CreditCard> CreditCards { get; set; }
        public virtual DbSet<PaypallAcount> PaypallAcounts { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<PersonalInfo> PersonalInfos { get; set; }
        public virtual DbSet<PhoneNumber> PhoneNumbers { get; set; }
        public virtual DbSet<VisitorAgent> VisitorAgents { get; set; }
        public virtual DbSet<BreachedPassword> BreachedPasswords { get; set; }
        public virtual DbSet<SharedLoginData> SharedLoginsData { get; set; }
        



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(c => c.UserDevices)

                .WithOne(e => e.User);
            
        }
        
    }
}
