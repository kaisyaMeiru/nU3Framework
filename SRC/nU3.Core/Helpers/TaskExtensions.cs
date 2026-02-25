using System;
using System.Threading.Tasks;
using nU3.Core.Logging;

namespace nU3.Core.Helpers
{
    public static class TaskExtensions
    {
        /// <summary>
        /// 비동기 작업을 기다리지 않고 실행하며, 예외 발생 시 로그를 남깁니다.
        /// </summary>
        public static void Forget(this Task task)
        {
            task.ContinueWith(t =>
            {
                if (t.IsFaulted && t.Exception != null)
                {
                    foreach (var ex in t.Exception.Flatten().InnerExceptions)
                    {
                        LogManager.Error("비동기 작업(Forget) 중 오류 발생", "Task", ex);
                    }
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
