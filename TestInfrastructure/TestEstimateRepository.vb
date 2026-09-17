Option Strict On
Option Infer On

Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Domain
Imports Infrastructure

<TestClass()>
Public Class TestEstimateRepository

    Private Class Masters
        Public Employee As Employee
        Public Payment As PaymentCondition
        Public Tax As SalesTax
        Public Customer As Customer
    End Class

    <TestInitialize()>
    Public Sub Setup()
        Dim setting = New InfrastructureSetting
        setting.InitializeDB(resetDebugDatabase:=True)
    End Sub

    Private Function SeedMasters() As Masters
        Dim empRepo As New EmployeeRepositoryImpl
        Dim payRepo As New PaymentConditionRepositoryImpl
        Dim taxRepo As New SalesTaxRepositoryImpl
        Dim custRepo As New CustomerRepositoryImpl

        Dim employee As New Employee(empRepo)
        employee.EmployeeNo = "S001"
        employee.Name = "見積担当"
        employee.NameKana = "みつもりたんとう"
        Assert.IsTrue(empRepo.Save(employee))
        employee = empRepo.FindByEmployeeNo("S001")

        Dim payment As New PaymentCondition(payRepo)
        payment.Name = "見積用支払"
        payment.CutOff = 20
        payment.DueDate = 20
        payment.MonthOffset = 1
        Assert.IsTrue(payRepo.Save(payment))
        payment = payRepo.FindByName("見積用支払")

        Dim tax As New SalesTax(taxRepo)
        tax.ApplyStartDate = New Date(2019, 10, 1)
        tax.TaxRate = 0.1D
        Assert.IsTrue(taxRepo.Save(tax))
        tax = taxRepo.FindByApplyDate(New Date(2019, 10, 1))

        Dim customer As New Customer(custRepo, payRepo, empRepo)
        customer.Name = "見積顧客"
        customer.KanaName = "みつもりこきゃく"
        customer.PIC = employee
        customer.PaymentCondition = payment
        customer.PostalCode = "160-0022"
        customer.Address1 = "東京都新宿区"
        customer.Address2 = "1-1-1"
        Assert.IsTrue(custRepo.Save(customer))
        customer = custRepo.FindByCustomerNameAndAddress("見積顧客", "東京都新宿区", "1-1-1")

        Dim m As New Masters
        m.Employee = employee
        m.Payment = payment
        m.Tax = tax
        m.Customer = customer
        Return m
    End Function

    <TestMethod()>
    Public Sub SaveEstimate_AssignsNumberAndPersists()
        Dim masters = SeedMasters()
        Dim estRepo As New EstimateRepositoryImpl
        Dim custRepo As New CustomerRepositoryImpl
        Dim payRepo As New PaymentConditionRepositoryImpl
        Dim empRepo As New EmployeeRepositoryImpl
        Dim taxRepo As New SalesTaxRepositoryImpl

        Dim e As New Estimate(estRepo, custRepo, payRepo, empRepo, taxRepo)
        e.Customer = masters.Customer
        e.Title = "永続化テスト"
        e.PaymentCondition = masters.Payment
        e.PICEmployee = masters.Employee
        e.SalesTax = masters.Tax
        e.IssueDate = Date.Today
        e.DueDate = Date.Today.AddDays(10)
        e.EffectiveDate = Date.Today.AddDays(7)
        e.Remarks = "備考"

        Dim detail As New EstimateDetail()
        detail.ItemName = "作業費"
        detail.Quantity = 2
        detail.UnitPrice = 5000
        e.AddDetail(detail)

        Assert.IsTrue(e.Save())
        Assert.AreNotEqual(-1, e.ID)
        Assert.IsFalse(String.IsNullOrEmpty(e.EstimateNo))
        Assert.AreEqual(1, estRepo.CountAllEstimate())
    End Sub

    <TestMethod()>
    Public Sub SaveEstimate_MultipleSameDay_HaveUniqueNumbers()
        Dim masters = SeedMasters()
        Dim estRepo As New EstimateRepositoryImpl
        Dim custRepo As New CustomerRepositoryImpl
        Dim payRepo As New PaymentConditionRepositoryImpl
        Dim empRepo As New EmployeeRepositoryImpl
        Dim taxRepo As New SalesTaxRepositoryImpl

        Dim numbers As New List(Of String)
        For i = 1 To 3
            Dim e As New Estimate(estRepo, custRepo, payRepo, empRepo, taxRepo)
            e.Customer = masters.Customer
            e.Title = "同日採番" & i.ToString()
            e.PaymentCondition = masters.Payment
            e.PICEmployee = masters.Employee
            e.SalesTax = masters.Tax
            e.IssueDate = Date.Today
            e.DueDate = Date.Today.AddDays(10)
            e.EffectiveDate = Date.Today.AddDays(7)
            e.Remarks = ""

            Dim detail As New EstimateDetail()
            detail.ItemName = "項目"
            detail.Quantity = 1
            detail.UnitPrice = 1000
            e.AddDetail(detail)

            Assert.IsTrue(e.Save())
            numbers.Add(e.EstimateNo)
        Next

        Assert.AreEqual(3, numbers.Where(Function(n) n <> Nothing).Distinct().Count())
        Assert.AreEqual(3, estRepo.CountEstimateOnDay(Date.Today))
    End Sub

    <TestMethod()>
    Public Sub FindEstimate_ByNumberForwardMatch()
        Dim masters = SeedMasters()
        Dim estRepo As New EstimateRepositoryImpl
        Dim custRepo As New CustomerRepositoryImpl
        Dim payRepo As New PaymentConditionRepositoryImpl
        Dim empRepo As New EmployeeRepositoryImpl
        Dim taxRepo As New SalesTaxRepositoryImpl

        Dim e As New Estimate(estRepo, custRepo, payRepo, empRepo, taxRepo)
        e.Customer = masters.Customer
        e.Title = "検索テスト"
        e.PaymentCondition = masters.Payment
        e.PICEmployee = masters.Employee
        e.SalesTax = masters.Tax
        e.IssueDate = Date.Today
        e.DueDate = Date.Today.AddDays(5)
        e.EffectiveDate = Date.Today.AddDays(3)
        e.Remarks = ""
        Dim detail As New EstimateDetail()
        detail.ItemName = "A"
        detail.Quantity = 1
        detail.UnitPrice = 100
        e.AddDetail(detail)
        Assert.IsTrue(e.Save())

        Dim cond As New EstimateRepositorySearchCondition
        cond.EstimateNoForwardMatch = Date.Today.ToString("yyyyMMdd")
        Dim found = estRepo.FindEstimateByCondition(cond)
        Assert.IsTrue(found.Where(Function(x) x.ID = e.ID).Any())
    End Sub

    <TestMethod()>
    Public Sub SaveEstimate_UpdateAddsAndRemovesDetails()
        Dim masters = SeedMasters()
        Dim estRepo As New EstimateRepositoryImpl
        Dim custRepo As New CustomerRepositoryImpl
        Dim payRepo As New PaymentConditionRepositoryImpl
        Dim empRepo As New EmployeeRepositoryImpl
        Dim taxRepo As New SalesTaxRepositoryImpl

        Dim e As New Estimate(estRepo, custRepo, payRepo, empRepo, taxRepo)
        e.Customer = masters.Customer
        e.Title = "明細更新"
        e.PaymentCondition = masters.Payment
        e.PICEmployee = masters.Employee
        e.SalesTax = masters.Tax
        e.IssueDate = Date.Today
        e.DueDate = Date.Today.AddDays(5)
        e.EffectiveDate = Date.Today.AddDays(3)
        e.Remarks = ""

        Dim d1 As New EstimateDetail()
        d1.ItemName = "残す"
        d1.Quantity = 1
        d1.UnitPrice = 100
        e.AddDetail(d1)

        Dim d2 As New EstimateDetail()
        d2.ItemName = "消す"
        d2.Quantity = 1
        d2.UnitPrice = 200
        e.AddDetail(d2)

        Assert.IsTrue(e.Save())

        Assert.IsTrue(e.RemoveDetail(d2.DisplayOrder))

        Dim d3 As New EstimateDetail()
        d3.ItemName = "追加"
        d3.Quantity = 3
        d3.UnitPrice = 300
        e.AddDetail(d3)

        Assert.IsTrue(e.Save())

        Dim cond As New EstimateRepositorySearchCondition
        cond.TitleForwardMatch = "明細更新"
        Dim reloaded = estRepo.FindEstimateByCondition(cond).Single()
        Assert.AreEqual(2, reloaded.Details.Count)
        Assert.IsTrue(reloaded.Details.Where(Function(d) d.ItemName = "残す").Any())
        Assert.IsTrue(reloaded.Details.Where(Function(d) d.ItemName = "追加").Any())
        Assert.IsFalse(reloaded.Details.Where(Function(d) d.ItemName = "消す").Any())
    End Sub

End Class
