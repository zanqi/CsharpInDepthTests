using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading;

namespace CsharpInDepthTests
{
    public class MyClass
    {
        public delegate void MyDelegate(string input);

        public event MyDelegate MyEvent;

        void FireEvent()
        {
            MyEvent("input");
        }
    }

    [TestClass]
    public class DelegateTests
    {
        delegate object MyDelegate(string input);

        event MyDelegate MyEvent;

        [TestMethod]
        public void EventIsEncapsulatingDelegate()
        {
            // Outside of a class, you can only + and - on the event
            var obj = new MyClass();
            obj.MyEvent += (s) => { Debug.WriteLine("My Event is invoked from outside!"); };

            // obj.MyEvent.Invoke("");
            // Compile error:
            // Error CS0070  The event 'MyClass.MyEvent' can only appear on the left hand side of += or -= (except when used from within the type 

            // Inside a class, you can invoke the delegate
            MyEvent += (s) => { return new object(); };
            MyEvent.Invoke("");
        }

        object MyMethod(object o)
        {
            return new object();
        }

        string MyMethod2(string s)
        {
            return s;
        }

        [TestMethod]
        public void DelegateVarianceIsSupported()
        {
            // Param type Contravariance supported
            MyEvent += MyMethod;

            Action<object> myAction = (o) => { };
            //MyEvent += myAction;
            // This is not: Cannot implicitly convert type 'System.Action<object>' to 'CsharpInDepthTests.DelegateTests.MyDelegate' 

            // Return type Covariance supported
            MyEvent += MyMethod2;
        }

        void MyMethod() { }
        void MyMethod(object sender, EventArgs e) { }

        [TestMethod]
        public void MethodGroupConversionCanHandelOverload()
        {
            // Try F12 on MyMehod below
            ThreadStart x = MyMethod; // take the void MyMethod()
            EventHandler y = MyMethod; // take the void MyMethod(object sender, EventArgs e) 
        }

        [TestMethod]
        public void DelegateWithoutParam()
        {
            MyEvent += delegate { return null; };
        }

        [TestMethod]
        public void DelegateWithoutParamNotWorkWithOverload()
        {
            new Thread(delegate () { Console.WriteLine("t1"); });
            new Thread(delegate (object o) { Console.WriteLine("t2"); });

            //The call is ambiguous between the following methods or properties: 'Thread.Thread(ThreadStart)' and 'Thread.Thread(ParameterizedThreadStart)'
            //new Thread(delegate { Console.WriteLine(); });
        }
    }
}
