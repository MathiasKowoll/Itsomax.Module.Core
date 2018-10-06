using Itsomax.Module.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Itsomax.Module.Core.Data
{
    public class CustomRepositoryFunctions : Repository<Entity>,ICustomRepositoryFunctions
    {
        public CustomRepositoryFunctions(ItsomaxDbContext context) : base(context){}
        
        public byte[] SetEncryption(string stringToEncrypt)
        {
            byte[] setEncrypt = null;
            var query = "select * from \"Core\".\"SetEncrypt\"('" + stringToEncrypt + "')";
            using (var command = Context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                Context.Database.OpenConnection();
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    setEncrypt = (byte[])result[0];
                }
                Context.Database.CloseConnection();
            }
            
            return setEncrypt;
        }

        public string GetDecryption(byte[] stringToDecrypt)
        {
            string getEncrypt = null;
            var query = "select * from \"Core\".\"GetEncrypt\"('"+stringToDecrypt+"')";
            using (var command = Context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                Context.Database.OpenConnection();
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    getEncrypt = (string)result[0];
                }
                Context.Database.CloseConnection();
            }

            return getEncrypt;

        }
    }
    
    
}