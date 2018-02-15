using System;
using System.Threading;
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
        public void CancelTaskWithException()
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

        [TestMethod]
        public void CancelTaskWithToken()
        {
            var source = new CancellationTokenSource();
            var task = DelayFor30Seconds(source.Token);
            source.CancelAfter(TimeSpan.FromSeconds(1));
            Assert.AreEqual(task.Status, TaskStatus.WaitingForActivation);
            try
            {
                task.Wait();
            }
            catch (AggregateException e)
            {
                Assert.IsInstanceOfType(e.InnerException, typeof(TaskCanceledException));
            }
            Assert.AreEqual(task.Status, TaskStatus.Canceled);
        }

        [TestMethod]
        public async Task CancelTaskWithToken1()
        {
            var source = new CancellationTokenSource();
            var task = DelayFor30Seconds(source.Token);
            source.CancelAfter(TimeSpan.FromSeconds(1));
            Assert.AreEqual(task.Status, TaskStatus.WaitingForActivation);
            try
            {
                await task;
            }
            catch (TaskCanceledException e)
            {

            }
            Assert.AreEqual(task.Status, TaskStatus.Canceled);
        }

        private async Task DelayFor30Seconds(CancellationToken token)
        {
            await Task.Delay(TimeSpan.FromSeconds(30), token);
        }
    }
}
