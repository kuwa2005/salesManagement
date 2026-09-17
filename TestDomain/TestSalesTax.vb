Option Strict On
Option Infer On

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Domain

<TestClass()>
Public Class TestSalesTax

    <TestMethod()>
    Public Sub TaxRate_OutOfRange_IsInvalid()
        Dim repo As New SalesTaxRepositoryStub
        Dim t = New SalesTax(repo)
        t.ApplyStartDate = New Date(2019, 10, 1)
        t.TaxRate = 1.2D

        Assert.IsFalse(t.Validate)
        Assert.AreNotEqual(t.Item(NameOf(t.TaxRate)), String.Empty)
    End Sub

    <TestMethod()>
    Public Sub TaxRateOfPercentage_Converts()
        Dim repo As New SalesTaxRepositoryStub
        Dim t = New SalesTax(repo)
        t.TaxRateOfPercentage = 10D

        Assert.AreEqual(0.1D, t.TaxRate)
        Assert.AreEqual(10D, t.TaxRateOfPercentage)
    End Sub

    <TestMethod()>
    Public Sub ApplyStartDate_Duplicate_IsInvalid()
        Dim repo As New SalesTaxRepositoryStub
        Dim day = New Date(2019, 10, 1)
        repo.Seed(day, 0.1D)

        Dim t = New SalesTax(repo)
        t.ApplyStartDate = day
        t.TaxRate = 0.08D

        Assert.IsFalse(t.Validate)
        Assert.AreNotEqual(t.Item(NameOf(t.ApplyStartDate)), String.Empty)
    End Sub

    <TestMethod()>
    Public Sub TaxOn_ReturnsLatestApplicable()
        Dim repo As New SalesTaxRepositoryStub
        repo.Seed(New Date(2014, 4, 1), 0.08D)
        repo.Seed(New Date(2019, 10, 1), 0.1D)

        Dim tax = repo.TaxOn(New Date(2020, 1, 1))
        Assert.IsNotNull(tax)
        Assert.AreEqual(0.1D, tax.TaxRate)
    End Sub

End Class
