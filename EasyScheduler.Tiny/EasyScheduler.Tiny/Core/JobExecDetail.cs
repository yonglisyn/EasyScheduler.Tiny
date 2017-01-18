namespace EasyScheduler.Tiny.Core
{
    public class JobExecDetail
    {
        private JobExecResult _Result;
        private string _JobName;

        public JobExecResult Result
        {
            get { return _Result; }
        }

        public string JobName
        {
            get { return _JobName; }
        }

        public JobExecDetail(JobExecResult result, string jobName)
        {
            _Result = result;
            _JobName = jobName;
        }
    }

    public enum JobExecResult
    {
        Success  = 1,
        Fail = 2,
        Cancel =3
    }
}