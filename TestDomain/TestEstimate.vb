Option Strict On
Option Infer On

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Domain

<TestClass()>
Public Class TestEstimate

    <TestMethod()>
    Public Sub Title_Empty_IsInvalid()
        Dim e = EstimateTestFixture.Create().CreateValidEstimate()
        e.Title = ""

        Assert.IsFalse(e.Validate)
        Assert.AreNotEqual(e.Item(NameOf(e.Title)), String.Empty)
    End Sub

    <TestMethod()>
    Public Sub Customer_Nothing_IsInvalid()
        Dim e = EstimateTestFixture.Create().CreateValidEstimate()
        e.Customer = Nothing

        Assert.IsFalse(e.Validate)
        Assert.AreNotEqual(e.Item(NameOf(e.Customer)), String.Empty)
    End Sub

    <TestMethod()>
    Public Sub DueDate_BeforeIssueDate_IsInvalid()
        Dim e = EstimateTestFixture.Create().CreateValidEstimate()
        e.IssueDate = Date.Today
        e.DueDate = Date.Today.AddDays(-1)

        Assert.IsFalse(e.Validate)
        Assert.AreNotEqual(e.Item(NameOf(e.DueDate)), String.Empty)
    End Sub

    <TestMethod()>
    Public Sub EstimatePrice_SumsDetails()
        Dim e = EstimateTestFixture.Create().CreateValidEstimate()
        Dim d1 As New EstimateDetail()
        d1.ItemName = "A"
        d1.Quantity = 2
        d1.UnitPrice = 1000
        Dim d2 As New EstimateDetail()
        d2.ItemName = "B"
        d2.Quantity = 3
        d2.UnitPrice = 500

        Assert.IsTrue(e.AddDetail(d1))
        Assert.IsTrue(e.AddDetail(d2))
        Assert.AreEqual(3500D, e.EstimatePrice)
    End Sub

    <TestMethod()>
    Public Sub EstimatePriceIncludeTax_AppliesRate()
        Dim e = EstimateTestFixture.Create().CreateValidEstimate()
        Dim d As New EstimateDetail()
        d.ItemName = "A"
        d.Quantity = 1
        d.UnitPrice = 1000
        e.AddDetail(d)

        Assert.AreEqual(1100D, e.EstimatePriceIncludeTax)
    End Sub

    <TestMethod()>
    Public Sub AddDetail_ExceedsMax_Fails()
        Dim e = EstimateTestFixture.Create().CreateValidEstimate()
        For i = 1 To Estimate.MAX_ESTIMATE_DETAIL_SIZE
            Dim d As New EstimateDetail()
            d.ItemName = "商品" & i.ToString()
            d.Quantity = 1
            d.UnitPrice = 100
            Assert.IsTrue(e.AddDetail(d), "明細追加に失敗: " & i.ToString())
        Next

        Dim overflow As New EstimateDetail()
        overflow.ItemName = "超過"
        overflow.Quantity = 1
        overflow.UnitPrice = 1
        Assert.IsFalse(e.AddDetail(overflow))
        Assert.AreEqual(Estimate.MAX_ESTIMATE_DETAIL_SIZE, e.Details.Count)
    End Sub

    <TestMethod()>
    Public Sub UpDownDisplayOrder_Swaps()
        Dim e = EstimateTestFixture.Create().CreateValidEstimate()
        Dim d1 As New EstimateDetail()
        d1.ItemName = "1"
        d1.Quantity = 1
        d1.UnitPrice = 1
        Dim d2 As New EstimateDetail()
        d2.ItemName = "2"
        d2.Quantity = 1
        d2.UnitPrice = 1
        e.AddDetail(d1)
        e.AddDetail(d2)

        Assert.AreEqual(1, d1.DisplayOrder)
        Assert.AreEqual(2, d2.DisplayOrder)

        Assert.AreEqual(1, e.UpToDisplayOrder(2))
        Assert.AreEqual(1, d2.DisplayOrder)
        Assert.AreEqual(2, d1.DisplayOrder)

        Assert.AreEqual(2, e.DownToDisplayOrder(1))
        Assert.AreEqual(2, d2.DisplayOrder)
        Assert.AreEqual(1, d1.DisplayOrder)
    End Sub

    <TestMethod()>
    Public Sub RemoveDetail_RemovesByDisplayOrder()
        Dim e = EstimateTestFixture.Create().CreateValidEstimate()
        Dim d1 As New EstimateDetail()
        d1.ItemName = "1"
        d1.Quantity = 1
        d1.UnitPrice = 1
        Dim d2 As New EstimateDetail()
        d2.ItemName = "2"
        d2.Quantity = 1
        d2.UnitPrice = 1
        e.AddDetail(d1)
        e.AddDetail(d2)

        Assert.IsTrue(e.RemoveDetail(1))
        Assert.AreEqual(1, e.Details.Count)
        Assert.AreEqual("2", e.Details(0).ItemName)
    End Sub

    <TestMethod()>
    Public Sub ValidEstimate_Passes()
        Dim e = EstimateTestFixture.Create().CreateValidEstimate()
        Dim d As New EstimateDetail()
        d.ItemName = "開発費"
        d.Quantity = 1
        d.UnitPrice = 100000
        e.AddDetail(d)

        Assert.IsTrue(e.Validate)
        Assert.IsFalse(e.HasError)
    End Sub

End Class
