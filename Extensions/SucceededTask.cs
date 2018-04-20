namespace Itsomax.Module.Core.Extensions
{
    public class SucceededTask
    {
        private static readonly SucceededTask SuccessTask = new SucceededTask { Succeeded = true };
        private static readonly SucceededTask LockedOutTask = new SucceededTask { IsLockedOut = true };
        private static readonly SucceededTask NotAllowedTask = new SucceededTask { IsNotAllowed = true };
        private static readonly SucceededTask UserExistsTask = new SucceededTask { IsUserExists = true };
        //private List<ErrorTask> _errors = new List<ErrorTask>();
        private string _errors;
        //public IEnumerable<ErrorTask> Errors => _errors;
        public bool Succeeded { get; protected set; }
        public bool IsLockedOut { get; protected set; }
        public bool IsNotAllowed { get; protected set; }
        public bool IsUserExists { get; protected set; }

        public static SucceededTask Success => SuccessTask;
        public string Errors => _errors;
        public static SucceededTask LockedOut => LockedOutTask;
        public static SucceededTask NotAllowed => NotAllowedTask;
        public static SucceededTask UserExists => UserExistsTask;



        public static SucceededTask Failed(string errors)
        {
            var result = new SucceededTask { Succeeded = false };
            if (errors != null)
            {
                result._errors = errors;
            }

            return result;
        }

        public override string ToString()
        {
            return IsLockedOut ? "LockedOut" :
                   IsNotAllowed ? "NotAllowed" :
                   IsUserExists ? "UserExists" :
                   Succeeded ? "Succeeded" : string.Format("{0} : {1}", "Failed", string.Join(",", Errors));

        }
    }
}