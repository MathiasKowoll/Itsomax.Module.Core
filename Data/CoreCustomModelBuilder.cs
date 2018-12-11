using Itsomax.Data.Infrastructure.Data;
using Itsomax.Module.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Itsomax.Module.Core.Data
{
    public class CoreCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppSetting>()
                .ToTable("AppSetting", "Core")
                .HasKey(x => new {x.Key});

            modelBuilder.Entity<UserSettingDetail>(x =>
            {
                x.HasOne(b => b.UserAppSetting).WithMany(b => b.UserSettingDetail)
                    .HasForeignKey(b => b.UserAppSettingId);
                x.HasOne(b => b.User).WithMany(b => b.UserSettingDetail).HasForeignKey(b => b.UserId);
                x.HasKey(b => new {b.UserId, b.UserAppSettingId});
                x.ToTable("UserSettingDetail", "Core");
            });
            
            modelBuilder.Entity<AuditLogs>()
                .ToTable("AuditLogs", "Core")
                .HasKey(x => new {x.CreatedOn, x.ActionTrigered, x.UserName});
            
            modelBuilder.Entity<User>()
                .ToTable("User", "Core");

            modelBuilder.Entity<Role>()
                .ToTable("Role", "Core");

            modelBuilder.Entity<IdentityUserClaim<long>>(b =>
            {
                b.HasKey(uc => uc.Id);
                b.ToTable("UserClaim", "Core");
            });

            modelBuilder.Entity<IdentityRoleClaim<long>>(b =>
            {
                b.HasKey(rc => rc.Id);
                b.ToTable("RoleClaim", "Core");
            });

            modelBuilder.Entity<UserRole>(b =>
            {
                b.HasKey(ur => new {ur.UserId, ur.RoleId});
                b.HasOne(ur => ur.Role).WithMany(x => x.Users).HasForeignKey(r => r.RoleId);
                b.HasOne(ur => ur.User).WithMany(u => u.Roles).HasForeignKey(u => u.UserId);
                b.ToTable("UserRole", "Core");
            });

            modelBuilder.Entity<IdentityUserLogin<long>>(b => { b.ToTable("UserLogin", "Core"); });

            modelBuilder.Entity<IdentityUserToken<long>>(b => { b.ToTable("UserToken", "Core"); });

            modelBuilder.Entity<Entity>(e => { e.HasKey(x => x.Id); });

            modelBuilder.Entity<ModuleRole>(o =>
            {
                o.HasKey(x => new {x.RoleId, x.SubModuleId});
                o.HasOne(x => x.Role).WithMany(x => x.ModuleRoles).HasForeignKey(x => x.RoleId);
                o.HasOne(x => x.SubModule).WithMany(x => x.ModuleRoles).HasForeignKey(x => x.SubModuleId);
                o.ToTable("ModuleRole", "Core");
            });

            modelBuilder.Entity<SubModule>(o =>
            {
                o.HasOne(x => x.Modules).WithMany(x => x.SubModules).HasForeignKey(x => x.ModulesId);
            });
            
            CoreSeedData.SeedData(modelBuilder);
        }
    }
}