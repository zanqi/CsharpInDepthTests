Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class UnitTest1

    <ExpectedException(GetType(InvalidOperationException))>
    <TestMethod()> Public Sub LiftedComparisonTest()
        Dim i As Integer? = 1
        Assert.IsTrue(i > Nothing, "VB can't compare with nothing")
    End Sub

End Class