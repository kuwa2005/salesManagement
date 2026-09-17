Option Strict On
Option Infer On

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Domain

<TestClass()> Public Class TestCustomer

    Private Class Fixture
        Public CustRepo As CustomerRepositoryStub
        Public PayRepo As PaymentConditionRepositoryStub
        Public EmpRepo As EmployeeRepositoryStub
        Public Employee As Employee
        Public Payment As PaymentCondition
    End Class

    Private Function CreateFixture() As Fixture
        Dim fx As New Fixture
        fx.CustRepo = New CustomerRepositoryStub
        fx.PayRepo = New PaymentConditionRepositoryStub
        fx.EmpRepo = New EmployeeRepositoryStub
        fx.Employee = fx.EmpRepo.Seed("E001", "営業", "えいぎょう")
        fx.Payment = fx.PayRepo.Seed("条件1")
        Return fx
    End Function

    <TestMethod()> Public Sub TestNameShouldNotEmpty()
        Dim fx = CreateFixture()
        Dim c = New Customer(fx.CustRepo, fx.PayRepo, fx.EmpRepo)
        With c
            .Name = ""
            .KanaName = ""
            .PostalCode = "100-8111"
            .Address1 = "東京都千代田区"
            .Address2 = "千代田1-1"
            .PIC = Nothing
        End With

        Assert.IsFalse(c.Validate)
        Assert.AreNotEqual(c.Item(NameOf(c.Name)), String.Empty)
    End Sub

    <TestMethod()> Public Sub ValidCustomer_Passes()
        Dim fx = CreateFixture()
        Dim c = New Customer(fx.CustRepo, fx.PayRepo, fx.EmpRepo)
        With c
            .Name = "株式会社サンプル"
            .KanaName = "かぶしきがいしゃさんぷる"
            .PIC = fx.Employee
            .PaymentCondition = fx.Payment
            .PostalCode = "100-0001"
            .Address1 = "東京都千代田区"
            .Address2 = "1-1"
        End With

        Assert.IsTrue(c.Validate)
        Assert.IsFalse(c.HasError)
    End Sub

    <TestMethod()> Public Sub PostalCode_InvalidLength_IsInvalid()
        Dim fx = CreateFixture()
        Dim c = New Customer(fx.CustRepo, fx.PayRepo, fx.EmpRepo)
        With c
            .Name = "株式会社サンプル"
            .KanaName = "かぶしきがいしゃさんぷる"
            .PIC = fx.Employee
            .PaymentCondition = fx.Payment
            .PostalCode = "1000001"
            .Address1 = "東京都千代田区"
            .Address2 = "1-1"
        End With

        Assert.IsFalse(c.Validate)
        Assert.AreNotEqual(c.Item(NameOf(c.PostalCode)), String.Empty)
    End Sub

    <TestMethod()> Public Sub PIC_Nothing_IsInvalid()
        Dim fx = CreateFixture()
        Dim c = New Customer(fx.CustRepo, fx.PayRepo, fx.EmpRepo)
        With c
            .Name = "株式会社サンプル"
            .KanaName = "かぶしきがいしゃさんぷる"
            .PIC = Nothing
            .PaymentCondition = fx.Payment
            .PostalCode = "100-0001"
            .Address1 = "東京都千代田区"
            .Address2 = "1-1"
        End With

        Assert.IsFalse(c.Validate)
        Assert.AreNotEqual(c.Item(NameOf(c.PIC)), String.Empty)
    End Sub

End Class
