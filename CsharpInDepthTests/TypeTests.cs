using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace CsharpInDepthTests
{
    [TestClass]
    public class TypeTests
    {
        [TestMethod]
        public void AnonymousObjectCanMatch()
        {
            var o1 = new { A = "1", B = 1 };
            var o2 = new { A = "2", B = 2 };
            o1 = o2;

            var o3 = new { B = 3, A = "3" };
            // o1 = o3;
            // Invalid: Cannot implicitly convert type '<anonymous type: int B, string A>' to '<anonymous type: string A, int B>'
        }

        bool AreReferenceEqual<T>(T a, T b) where T : class
        {
            return a == b;
        }

        [TestMethod]
        public void GenericTypeReferenceConstraint()
        {
            string a = "Foo";
            string b = "Foo1".Substring(0, 3);
            // If I just let b = "Foo", a and b will be the same reference

            Assert.AreEqual(a, b);

            Assert.IsFalse(AreReferenceEqual(a, b));
        }

        bool UnconstraintTypeCanCompareToNullOnly<T>(T a, T b)
        {
            // return a == b;
            // invalid

            return a == null;
            // valid
        }

        int CompareWithIComparableConstraint<T>(T a, T b) where T : IComparable
        {
            return a.CompareTo(b);
        }

        int CompareWithDefaultComparer<T>(T a, T b)
        {
            var comparer = Comparer<T>.Default;
            return comparer.Compare(a, b);
        }

        [TestMethod]
        public void EqualityComparerTest()
        {
            try
            {
                CompareWithIComparableConstraint(null, "");
                throw new AssertFailedException();
            }
            catch (NullReferenceException)
            {
                // expected
            }

            Assert.AreEqual(-1, CompareWithDefaultComparer(null, "a"));
        }

        class MyClass<T>
        {
            public MyClass(T a)
            {

            }
        }

        MyClass<T> Create<T>(T input) {
            return new MyClass<T>(input);
        }

        [TestMethod]
        public void GenericTypeConstructionTest()
        {
            var obj = new MyClass<int>(1); // Constructor do not have type inference

            obj = Create(1); // Type inference can be used in generic method
        }

        interface IInterfaceA
        {
            void Foo(int a);
        }

        interface IInterfaceB
        {
            bool Foo(int a);
        }

        class MyClass : IInterfaceA, IInterfaceB
        {
            public void Foo(int a)
            {
            }

            // Explicit interface method is needed here because C# do not allow overloading methods with the same name and parameters.
            bool IInterfaceB.Foo(int a)
            {
                return true;
            }
        }

        static IEnumerable<int> Foo()
        {
            yield break;
        }

        [TestMethod]
        public void IEnumerableDisposeTest()
        {
            var enumerable = Foo();
            // Iterator blocks in the Microsoft implementation of the C# compiler happen to create a single type which implements both IEnumerable<T> and IEnumerator<T>. The latter extends IDisposable
            Assert.IsTrue(enumerable is IDisposable);
        }

        class MyEnumerator : IEnumerator<int>, IEnumerable<int>
        {
            public int Current => 1;

            object IEnumerator.Current => 1;

            public bool Disposed { get; set; }

            public void Dispose()
            {
                Disposed = true;
            }

            public bool MoveNext()
            {
                return false;
            }

            public void Reset()
            {
            }

            public IEnumerator<int> GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }
        }

        [TestMethod]
        public void IEnumeratorDisposeTest()
        {
            var enumerator = new MyEnumerator();

            foreach (var item in enumerator)
            {
            }

            Assert.IsTrue(enumerator.Disposed);
        }

        class MyClass2<T>
        {
            public Type MyGetType()
            {
                return typeof(List<T>);
            }
        }

        [TestMethod]
        public void TypeOfGenericTest()
        {
            var t = typeof(List<int>);

            var t2 = new MyClass2<int>().MyGetType();

            Assert.AreSame(t, t2);
            Assert.AreEqual(t, t2);
        }

        [TestMethod]
        public void CovariantArrayTest()
        {
            object[] a = { 1, 2, 3 };

            a[0] = 4L;

            a[1] = "abc";

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NullableExceptionTest()
        {
            int? i = null;

            var val = i.Value;
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NullableExceptionTest2()
        {
            int? i = null;
            int j = (int)i;
        }

        [TestMethod]
        public void LiftedOperatorTest()
        {
            DateTime? d = null;

            // Invalid
            //TimeSpan result = d - new DateTime();

            TimeSpan? result = d - new DateTime();

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void NullCoalescingTest()
        {
            int? i = null;

            int result = i ?? 1;

            // Invalid: Cannot implicitly convert type 'int?' to 'int'
            //int resultA = i ?? i;
            //int resultB = i ?? null;
        }

        [TestMethod]
        public void LiftedComparisonTest()
        {
            int? i = 2;
            Assert.IsFalse(i > null, "Comparing with null always produces false");
            Assert.IsFalse(null < i, "Comparing with null always produces false");

            i = null;
            Assert.IsFalse(null > i, "Comparing with null always produces false");

            // VB can't compare with Nothing
        }
    }
}
