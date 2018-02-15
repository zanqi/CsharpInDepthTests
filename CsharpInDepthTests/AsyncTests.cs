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

        [TestMethod]
        public void ArgumentValidationIsNotImmediate()
        {
            // No exception
            Task t = DoWorkAsync(null);
        }

        private async Task DoWorkAsync(object p)
        {
            if (p == null)
            {
                throw new ArgumentNullException();
            }

            await Task.Delay(100);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentValidationWithSyncMethod()
        {
            Task t = DoWork(null);
        }

        private Task DoWork(object p)
        {
            if (p == null)
            {
                throw new ArgumentNullException();
            }

            return DoWorkImpl(p);
        }

        private async Task DoWorkImpl(object p)
        {
            await Task.Delay(100);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentValidationWithAnonymousAsync()
        {
            Task t = DoWork2(null);
        }

        private Task DoWork2(object p)
        {
            if (p == null)
            {
                throw new ArgumentNullException();
            }

            Func<object, Task> func = async a => await Task.Delay(100);
            return func(p);
        }

        [TestMethod]
        public void CancelTask()
        {
            Task task1 = ThrowCancellationException();
            Task task2 = ThrowOtherException();

            Assert.AreEqual(TaskStatus.Canceled, task1.Status);
            Assert.AreEqual(TaskStatus.Faulted, task2.Status);
        }

        private async Task ThrowOtherException()
        {
            throw new InvalidOperationException();
            await Task.Delay(100);
        }

        private async Task ThrowCancellationException()
        {
            throw new OperationCanceledException();
            await Task.Delay(100);
        }
    }
}
