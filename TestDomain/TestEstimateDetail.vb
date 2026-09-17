Option Strict On
Option Infer On

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Domain

<TestClass()>
Public Class TestEstimateDetail

    <TestMethod()>
    Public Sub ItemName_Empty_IsInvalid()
        Dim d As New EstimateDetail()
        d.DisplayOrder = 1
        d.ItemName = ""
        d.Quantity = 1
        d.UnitPrice = 100

        Assert.IsFalse(d.Validate)
        Assert.AreNotEqual(d.Item(NameOf(d.ItemName)), String.Empty)
    End Sub

    <TestMethod()>
    Public Sub Quantity_TooLarge_IsInvalid()
        Dim d As New EstimateDetail()
        d.DisplayOrder = 1
        d.ItemName = "商品A"
        d.Quantity = 1000
        d.UnitPrice = 100

        Assert.IsFalse(d.Validate)
        Assert.AreNotEqual(d.Item(NameOf(d.Quantity)), String.Empty)
    End Sub

    <TestMethod()>
    Public Sub UnitPrice_Negative_IsInvalid()
        Dim d As New EstimateDetail()
        d.DisplayOrder = 1
        d.ItemName = "商品A"
        d.Quantity = 1
        d.UnitPrice = -1

        Assert.IsFalse(d.Validate)
        Assert.AreNotEqual(d.Item(NameOf(d.UnitPrice)), String.Empty)
    End Sub

    <TestMethod()>
    Public Sub ValidDetail_Passes()
        Dim d As New EstimateDetail()
        d.DisplayOrder = 1
        d.ItemName = "商品A"
        d.Quantity = 2
        d.UnitPrice = 1500

        Assert.IsTrue(d.Validate)
        Assert.IsFalse(d.HasError)
    End Sub

    <TestMethod()>
    Public Sub Quantity_Zero_IsInvalid()
        Dim d As New EstimateDetail()
        d.DisplayOrder = 1
        d.ItemName = "商品A"
        d.Quantity = 0
        d.UnitPrice = 100

        Assert.IsFalse(d.Validate)
        Assert.AreNotEqual(String.Empty, d.Item(NameOf(d.Quantity)))
    End Sub

    <TestMethod()>
    Public Sub Quantity_One_IsValid()
        Dim d As New EstimateDetail()
        d.DisplayOrder = 1
        d.ItemName = "商品A"
        d.Quantity = 1
        d.UnitPrice = 100

        Assert.IsTrue(d.Validate)
        Assert.AreEqual(String.Empty, d.Item(NameOf(d.Quantity)))
    End Sub

End Class
