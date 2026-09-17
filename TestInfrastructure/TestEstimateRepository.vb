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
        setting.InitializeDB()
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

End Class
