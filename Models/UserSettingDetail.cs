namespace Itsomax.Module.Core.Models
{
    public class UserSettingDetail
    {
        public string Value { get; set; }
        public long UserAppSettingId { get; set; }
        public UserAppSetting UserAppSetting { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
    }
}