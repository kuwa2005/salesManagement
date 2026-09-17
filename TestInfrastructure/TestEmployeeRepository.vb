Option Strict On
Option Infer On

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Domain
Imports Infrastructure

<TestClass()>
Public Class TestEmployeeRepository

    <TestInitialize()>
    Public Sub Setup()
        Dim setting = New InfrastructureSetting
        setting.InitializeDB(forceResetDebugDb:=True)
    End Sub

    <TestMethod()>
    Public Sub SaveAndFindByEmployeeNo()
        Dim repo As New EmployeeRepositoryImpl
        Dim e As New Employee(repo)
        e.EmployeeNo = "T001"
        e.Name = "テスト社員"
        e.NameKana = "てすとしゃいん"

        Assert.IsTrue(repo.Save(e))
        Dim id = repo.LastInsertID()
        Assert.IsTrue(id > 0)

        Dim loaded = repo.FindByEmployeeNo("T001")
        Assert.IsNotNull(loaded)
        Assert.AreEqual("テスト社員", loaded.Name)
        Assert.AreEqual(id, loaded.ID)
    End Sub

    <TestMethod()>
    Public Sub DuplicateEmployeeNo_SaveFails()
        Dim repo As New EmployeeRepositoryImpl
        Dim e1 As New Employee(repo)
        e1.EmployeeNo = "T002"
        e1.Name = "一人目"
        e1.NameKana = "ひとりめ"
        Assert.IsTrue(repo.Save(e1))

        Dim e2 As New Employee(repo)
        e2.EmployeeNo = "T002"
        e2.Name = "二人目"
        e2.NameKana = "ふたりめ"
        Assert.IsFalse(repo.Save(e2))
    End Sub

End Class
