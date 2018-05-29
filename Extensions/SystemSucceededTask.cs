namespace Itsomax.Module.Core.Extensions
{
    public class SystemSucceededTask
    {
        //private static readonly SystemSucceededTask SucceededTask = new SystemSucceededTask { Succeeded = true};
        //private static readonly SystemSucceededTask DbErrorTask = new SystemSucceededTask { IsDbError = false};
        //private static readonly SystemSucceededTask OtherErrorTask = new SystemSucceededTask { IsOtherError = false};

        private string _errors;
        private string _innerError;
        private string _okMessage;
        public bool IsDbError { get; protected set; }
        public bool IsOtherError { get; protected set; }
        public bool Succeeded { get; protected set; }
        
        //public static SystemSucceededTask Success => SucceededTask;
        //public static SystemSucceededTask DbError => DbErrorTask;
        //public static SystemSucceededTask OtherError => OtherErrorTask;
        public string Errors => _errors;
        public string InnerErrors => _innerError;
        public string OkMessage => _okMessage;

        public static SystemSucceededTask Success(string okMessage)
        {
            var result = new SystemSucceededTask {Succeeded = true,IsDbError = false,IsOtherError = false};
            if (okMessage != null)
            {
                result._okMessage = okMessage;
            }

            return result;
        }
        
        public static SystemSucceededTask Failed(string errors,string innerError,bool isDb,bool OtherError)
        {
            var result = new SystemSucceededTask {Succeeded = false,IsDbError = isDb,IsOtherError = OtherError};
            if (errors != null)
            {
                result._errors = errors;
            }

            if (innerError != null)
            {
                result._innerError = innerError;
            }

            return result;
        }

        public override string ToString()
        {
            return  IsDbError ? "DbError":
                    IsOtherError ? "OtherError" :
                    Succeeded ? string.Format("{0} : {1}", "Succeeded",string.Join(",",OkMessage)) : string.Format("{0} : {1} : {2}", "Failed", string.Join(",", Errors),string.Join(",", InnerErrors));


        }
        
        
    }
}