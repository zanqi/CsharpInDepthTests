using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CsharpInDepthTests
{
    [TestClass]
    public class AsyncTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task AwaitOnlyThrowFirstException()
        {
            Task task1 = Task.Run(() => { throw new ArgumentNullException("Message 1"); });
            Task task2 = Task.Run(() => { throw new InvalidOperationException("Message 2"); });
            await Task.WhenAll(task1, task2);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public async Task CatchingMultipleExceptions()
        {
            Task task1 = Task.Run(() => { throw new ArgumentNullException("Message 1"); });
            Task task2 = Task.Run(() => { throw new InvalidOperationException("Message 2"); });
            await Task.WhenAll(task1, task2).WithAggregatedExceptions();
        }
    }
}
