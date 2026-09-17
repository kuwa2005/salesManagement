Option Strict On
Option Infer On

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Domain
Imports Infrastructure

<TestClass()>
Public Class TestPaymentConditionRepository

    <TestInitialize()>
    Public Sub Setup()
        Dim setting = New InfrastructureSetting
        setting.InitializeDB(resetDebugDatabase:=True)
    End Sub

    <TestMethod()>
    Public Sub SaveAndFindByName()
        Dim repo As New PaymentConditionRepositoryImpl
        Dim p As New PaymentCondition(repo)
        p.Name = "20日締翌月末"
        p.CutOff = 20
        p.DueDate = 28
        p.MonthOffset = 1

        Assert.IsTrue(repo.Save(p))
        Dim loaded = repo.FindByName("20日締翌月末")
        Assert.IsNotNull(loaded)
        Assert.AreEqual(20, loaded.CutOff)
        Assert.AreEqual(28, loaded.DueDate)
        Assert.AreEqual(1, loaded.MonthOffset)
    End Sub

End Class
