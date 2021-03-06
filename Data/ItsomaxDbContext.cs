using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Itsomax.Data.Infrastructure;
using Itsomax.Data.Infrastructure.Data;
using Itsomax.Data.Infrastructure.Models;
using Itsomax.Module.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Itsomax.Module.Core.Data
{
    public class ItsomaxDbContext : IdentityDbContext<User, Role, long, IdentityUserClaim<long>, 
        UserRole, IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>
    {
        public ItsomaxDbContext(DbContextOptions options) : base(options)
        {
        }
        
        public DbSet<Modules> Modules { get; set; }
        public DbSet<SubModule> SubModule { get; set; }
        public DbSet<AppSetting> AppSettings { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var typeToRegisters = new List<Type>();
            foreach (var module in GlobalConfiguration.Modules)
            {
                typeToRegisters.AddRange(module.Assembly.DefinedTypes.Select(t => t.AsType()));
            }

            RegisterEntities(modelBuilder, typeToRegisters);

            RegisterConvention(modelBuilder);

            base.OnModelCreating(modelBuilder);

            RegisterCustomMappings(modelBuilder, typeToRegisters);

        }

        private static void RegisterConvention(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (entity.ClrType.Namespace == null) continue;
                var nameParts = entity.ClrType.Namespace.Split('.');
                var schemaName = nameParts[2].Replace("Management", "").Replace("System", "");
                modelBuilder.Entity(entity.Name).ToTable(entity.ClrType.Name,schemaName);
            }
        }

        private static void RegisterEntities(ModelBuilder modelBuilder, IEnumerable<Type> typeToRegisters)
        {
            var entityTypes = typeToRegisters.Where(x =>
                x.GetTypeInfo().IsSubclassOf(typeof(EntityBase)) && !x.GetTypeInfo().IsAbstract);
            foreach (var type in entityTypes)
            {
                modelBuilder.Entity(type);
            }
        }

        private static void RegisterCustomMappings(ModelBuilder modelBuilder, IEnumerable<Type> typeToRegisters)
        {
            var customModelBuilderTypes = typeToRegisters.Where(x => typeof(ICustomModelBuilder).IsAssignableFrom(x));
            foreach (var builderType in customModelBuilderTypes)
            {
                if (builderType == null || builderType == typeof(ICustomModelBuilder)) continue;
                var builder = (ICustomModelBuilder)Activator.CreateInstance(builderType);
                builder.Build(modelBuilder);
            }
        }


    }
}