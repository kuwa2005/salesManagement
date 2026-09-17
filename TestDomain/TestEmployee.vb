Option Strict On
Option Infer On

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Domain

<TestClass()>
Public Class TestEmployee

    <TestMethod()>
    Public Sub EmployeeNo_Empty_IsInvalid()
        Dim repo As New EmployeeRepositoryStub
        Dim e = New Employee(repo)
        e.EmployeeNo = ""
        e.Name = "山田"
        e.NameKana = "やまだ"

        Assert.IsFalse(e.Validate)
        Assert.AreNotEqual(e.Item(NameOf(e.EmployeeNo)), String.Empty)
    End Sub

    <TestMethod()>
    Public Sub EmployeeNo_TooLong_IsInvalid()
        Dim repo As New EmployeeRepositoryStub
        Dim e = New Employee(repo)
        e.EmployeeNo = "123456"
        e.Name = "山田"
        e.NameKana = "やまだ"

        Assert.IsFalse(e.Validate)
        Assert.AreNotEqual(e.Item(NameOf(e.EmployeeNo)), String.Empty)
    End Sub

    <TestMethod()>
    Public Sub EmployeeNo_Duplicate_IsInvalid()
        Dim repo As New EmployeeRepositoryStub
        repo.Seed("E001", "既存")

        Dim e = New Employee(repo)
        e.EmployeeNo = "E001"
        e.Name = "新規"
        e.NameKana = "しんき"

        Assert.IsFalse(e.Validate)
        Assert.AreNotEqual(e.Item(NameOf(e.EmployeeNo)), String.Empty)
    End Sub

    <TestMethod()>
    Public Sub ValidEmployee_Passes()
        Dim repo As New EmployeeRepositoryStub
        Dim e = New Employee(repo)
        e.EmployeeNo = "E002"
        e.Name = "佐藤"
        e.NameKana = "さとう"

        Assert.IsTrue(e.Validate)
        Assert.IsFalse(e.HasError)
    End Sub

End Class
