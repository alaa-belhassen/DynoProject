using login.models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql;
namespace login.data.PostgresConn
{
   public  class postgres :IdentityDbContext<employer3> {
        
        public virtual DbSet<MoreInfoEmployer> EmployesMoreInfo {get;set;}
        public virtual DbSet<RefreshTokenModel> RefreshTokenTable {get;set;}
        public postgres(DbContextOptions<postgres> options):base(options){
             
        }

       protected override void OnModelCreating(ModelBuilder model){
            base.OnModelCreating(model);
            model.Entity<employer3>(entity => 
                entity.HasOne(E=> E.moreInfo)
                .WithOne(M => M.employer)
                .HasForeignKey<MoreInfoEmployer>(m => m.moreInfoId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("Fk_USER_MoreInfo")
            );

          /*  model.Entity<MoreInfoEmployer>(entity =>{
                entity.HasOne(u => u.employer)
                    .WithOne(g => g.moreInfo)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Fk_MoreInfo_USER"); }
            );*/
        }
      
    }
}