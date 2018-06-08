using System.Linq;
using Itsomax.Module.Core.Data;
using Itsomax.Module.Core.Interfaces;
using Itsomax.Module.Core.ViewModels;

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
            var title = string.Empty;
            var systemTitle =_context.AppSettings.FirstOrDefault(x => x.Key == "SystemTitle");
            return systemTitle == null ? title : systemTitle.Value;
        }
        
        public string SystemLoginText()
        {
            var loginText = string.Empty;
            var systemTitle =_context.AppSettings.FirstOrDefault(x => x.Key == "SystemLoginText");
            return systemTitle == null ? loginText : systemTitle.Value;
        }
        
        public string Header()
        {
            var header = string.Empty;
            var systemTitle =_context.AppSettings.FirstOrDefault(x => x.Key == "SystemTitle");
            return systemTitle == null ? header : systemTitle.Value;
        }
    }
}