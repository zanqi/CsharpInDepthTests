using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CsharpInDepthTests
{
    static class TaskExtensions
    {
        public static AggregatedExceptionAwaitable WithAggregatedExceptions(this Task task)
        {
            return new AggregatedExceptionAwaitable(task);
        }
    }

    internal class AggregatedExceptionAwaitable
    {
        private Task task;

        public AggregatedExceptionAwaitable(Task task)
        {
            this.task = task;
        }

        public AggregatedExceptionAwaiter GetAwaiter()
        {
            return new AggregatedExceptionAwaiter(task);
        }

    }

    internal class AggregatedExceptionAwaiter : INotifyCompletion
    {
        private Task task;

        public AggregatedExceptionAwaiter(Task task)
        {
            this.task = task;
        }

        public bool IsCompleted
        {
            get { return task.GetAwaiter().IsCompleted; }
        }

        public void OnCompleted(Action continuation)
        {
            task.GetAwaiter().OnCompleted(continuation);
        }

        public void GetResult()
        {
            task.Wait(); // This is where multiple exceptions got wrapped
        }
    }
}
