Option Strict On
Option Infer On

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Domain
Imports Infrastructure

<TestClass()>
Public Class TestSalesTaxRepository

    <TestInitialize()>
    Public Sub Setup()
        Dim setting = New InfrastructureSetting
        setting.InitializeDB(forceResetDebugDb:=True)
    End Sub

    <TestMethod()>
    Public Sub SaveAndTaxOn()
        Dim repo As New SalesTaxRepositoryImpl
        Dim t8 As New SalesTax(repo)
        t8.ApplyStartDate = New Date(2014, 4, 1)
        t8.TaxRate = 0.08D
        Assert.IsTrue(repo.Save(t8))

        Dim t10 As New SalesTax(repo)
        t10.ApplyStartDate = New Date(2019, 10, 1)
        t10.TaxRate = 0.1D
        Assert.IsTrue(repo.Save(t10))

        Dim on2015 = repo.TaxOn(New Date(2015, 1, 1))
        Assert.IsNotNull(on2015)
        Assert.AreEqual(0.08D, on2015.TaxRate)

        Dim on2020 = repo.TaxOn(New Date(2020, 1, 1))
        Assert.IsNotNull(on2020)
        Assert.AreEqual(0.1D, on2020.TaxRate)
    End Sub

End Class
