using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Itsomax.Data.Infrastructure.Data;
using Itsomax.Module.Core.Models;

namespace Itsomax.Module.Core.Data
{
    public class CoreCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppSetting>()
                .ToTable("AppSetting", "Core")
                .HasKey(x => new {x.Key});

            modelBuilder.Entity<UserSetting>(x =>
            {
                x.HasKey(k => new {k.UserId, k.UserAppSettingId});
                x.HasOne(o => o.User).WithMany(o => o.UserSetting).HasForeignKey(k => k.UserId);
                x.HasOne(o => o.UserAppSetting).WithMany(o => o.UserSetting).HasForeignKey(k => k.UserAppSettingId);
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

            modelBuilder.Entity<ModuleContent>(o =>
            {
                o.HasOne(x => x.Modules).WithMany(x => x.ModuleContent).HasForeignKey(x => x.ModulesId);
            });

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
        }
    }
}