using System.Linq;
using Itsomax.Module.Core.Data;

namespace Itsomax.Module.Core.Services
{
    public class GetSettings
    {
        private readonly ItsomaxDbContext _context;

        public GetSettings(ItsomaxDbContext context)
        {
            _context = context;
        }

        public string SystemTitle()
        {
            const string title = "System Title";
            var systemTitle =_context.AppSettings.FirstOrDefault(x => x.Key == "SystemTitle");
            return string.IsNullOrEmpty(systemTitle.Value) ? title : systemTitle.Value;
        }
        
        public string SystemLoginText()
        {
            const string loginText = "System Login Text";
            var systemLoginTitle =_context.AppSettings.FirstOrDefault(x => x.Key == "SystemLoginText");
            return string.IsNullOrEmpty(systemLoginTitle.Value) ? loginText : systemLoginTitle.Value;
        }
        
        public string SystemLoginImageUrl()
        {
            const string imageUrl = "/assets/images/background/login-register2.jpg";
            var systemUrlText =_context.AppSettings.FirstOrDefault(x => x.Key == "SystemLoginImageUrl");
            return string.IsNullOrEmpty(systemUrlText.Value) ? imageUrl : systemUrlText.Value;
        }
        
        public string SystemLoginBigLogoUrl()
        {
            const string logoUrl = "/assets/images/background/login-register2.jpg";
            var systemUrlText =_context.AppSettings.FirstOrDefault(x => x.Key == "SystemBigLogoUrl");
            return string.IsNullOrEmpty(systemUrlText.Value) ? logoUrl : systemUrlText.Value;
        }
        
        public string SystemLoginSmallLogoUrl()
        {
            const string logoUrl = "/assets/images/background/login-register2.jpg";
            var systemUrlText =_context.AppSettings.FirstOrDefault(x => x.Key == "SystemBigLogoUrl");
            return string.IsNullOrEmpty(systemUrlText.Value) ? logoUrl : systemUrlText.Value;
        }
    
    
    }
}