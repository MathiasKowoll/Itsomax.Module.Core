using Itsomax.Module.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Itsomax.Module.Core.Data
{
    public class CoreSeedData
    {
        public static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAppSetting>().HasData(
                new UserAppSetting(1) {Key = "SystemDefaultPage"}
            );

            modelBuilder.Entity<AppSetting>().HasData(
                new AppSetting() {Key = "SystemSeedData", Value = "true"},
                new AppSetting() {Key = "SystemNewModule", Value = "true"},
                new AppSetting() {Key = "SystemCreateAdmin", Value = "true"},
                new AppSetting() {Key = "SystemRefreshClaims", Value = "true"},
                new AppSetting() {Key = "SystemNewModuleCreateMenu", Value = "true"},
                new AppSetting() {Key = "SystemTitle", Value = "This is a system title"},
                new AppSetting() {Key = "SystemLoginText", Value = "This is a login text"},
                new AppSetting() {Key = "SystemLoginImageUrl", Value = ""},
                new AppSetting() {Key = "SystemBigLogoUrl", Value = ""},
                new AppSetting() {Key = "SystemSmallLogoUrl", Value = ""}
            );
        }
    }
}