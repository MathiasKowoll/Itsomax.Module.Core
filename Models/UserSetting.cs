namespace Itsomax.Module.Core.Models
{
    public class UserSetting
    {
        public long UserId { get; set; }
        public User User { get; set; }
        public long UserAppSettingId { get; set; }
        public UserAppSetting UserAppSetting { get; set; }
    }
}