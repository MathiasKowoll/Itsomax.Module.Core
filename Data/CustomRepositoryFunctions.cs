using Itsomax.Module.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Itsomax.Module.Core.Data
{
    public class CustomRepositoryFunctions : Repository<Entity>,ICustomRepositoryFunctions
    {
        public CustomRepositoryFunctions(ItsomaxDbContext context) : base(context){}
        
        public byte[] SetEncryption(string stringToEncrypt)
        {
            byte[] setPass = null;
            var query = "select * from \"Core\".\"SetEncrypt\"('" + stringToEncrypt + "')";
            using (var command = Context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                Context.Database.OpenConnection();
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    setPass = (byte[])result[0];
                }
                Context.Database.CloseConnection();
            }
            
            return setPass;
        }

        public string GetDecryption(byte[] stringToDecrypt)
        {
            string stringPassword = null;
            var query = "select * from \"Core\".\"GetEcrypt\"('"+stringToDecrypt+"')";
            using (var command = Context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                Context.Database.OpenConnection();
                var result = command.ExecuteReader();
                while (result.Read())
                {
                    stringPassword = (string)result[0];
                }
                Context.Database.CloseConnection();
            }

            return stringPassword;

        }
    }
    
    
}