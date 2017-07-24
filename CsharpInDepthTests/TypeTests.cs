using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        bool AreReferenceEqual<T>(T a, T b) where T:class
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
    }
}
