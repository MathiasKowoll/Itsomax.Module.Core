using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace Itsomax.Module.Core.Extensions
{
    public class EfConfigSource : IConfigurationSource
    {
        private readonly Action<DbContextOptionsBuilder> _optionsAction;

        public EfConfigSource(Action<DbContextOptionsBuilder> optionsAction)
        {
            _optionsAction = optionsAction;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new EfConfigProvider(_optionsAction);
        }
    }
}