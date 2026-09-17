Option Strict On
Option Infer On

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Domain

<TestClass()>
Public Class TestPaymentCondition

    <TestMethod()>
    Public Sub Name_Empty_IsInvalid()
        Dim repo As New PaymentConditionRepositoryStub
        Dim p = New PaymentCondition(repo)
        p.Name = ""
        p.CutOff = 20
        p.DueDate = 20
        p.MonthOffset = 1

        Assert.IsFalse(p.Validate)
        Assert.AreNotEqual(p.Item(NameOf(p.Name)), String.Empty)
    End Sub

    <TestMethod()>
    Public Sub CutOff_OutOfRange_IsInvalid()
        Dim repo As New PaymentConditionRepositoryStub
        Dim p = New PaymentCondition(repo)
        p.Name = "月末締翌月末"
        p.CutOff = 29
        p.DueDate = 20
        p.MonthOffset = 1

        Assert.IsFalse(p.Validate)
        Assert.AreNotEqual(p.Item(NameOf(p.CutOff)), String.Empty)
    End Sub

    <TestMethod()>
    Public Sub MonthOffset_OutOfRange_IsInvalid()
        Dim repo As New PaymentConditionRepositoryStub
        Dim p = New PaymentCondition(repo)
        p.Name = "条件A"
        p.CutOff = 20
        p.DueDate = 20
        p.MonthOffset = 13

        Assert.IsFalse(p.Validate)
        Assert.AreNotEqual(p.Item(NameOf(p.MonthOffset)), String.Empty)
    End Sub

    <TestMethod()>
    Public Sub CutOff28_IsEndOfMonth()
        Dim repo As New PaymentConditionRepositoryStub
        Dim p = New PaymentCondition(repo)
        p.CutOff = PaymentCondition.CutOffEndOfMonth
        p.DueDate = PaymentCondition.DueDateEndOfMonth

        Assert.IsTrue(p.CutOffByEndOfMonth)
        Assert.IsTrue(p.PayEndOfMonth)
    End Sub

    <TestMethod()>
    Public Sub Name_Duplicate_IsInvalid()
        Dim repo As New PaymentConditionRepositoryStub
        repo.Seed("既存条件")

        Dim p = New PaymentCondition(repo)
        p.Name = "既存条件"
        p.CutOff = 15
        p.DueDate = 15
        p.MonthOffset = 0

        Assert.IsFalse(p.Validate)
        Assert.AreNotEqual(p.Item(NameOf(p.Name)), String.Empty)
    End Sub

End Class
