Option Strict On
Option Infer On

Imports Domain

''' <summary>
''' 見積テスト用の依存関係セット
''' </summary>
Friend Class EstimateTestFixture
    Public Property EstRepo As EstimateRepositoryStub
    Public Property CustRepo As CustomerRepositoryStub
    Public Property PayRepo As PaymentConditionRepositoryStub
    Public Property EmpRepo As EmployeeRepositoryStub
    Public Property TaxRepo As SalesTaxRepositoryStub
    Public Property Customer As Customer
    Public Property Payment As PaymentCondition
    Public Property Employee As Employee
    Public Property Tax As SalesTax

    Public Shared Function Create() As EstimateTestFixture
        Dim fx As New EstimateTestFixture
        fx.EstRepo = New EstimateRepositoryStub
        fx.CustRepo = New CustomerRepositoryStub
        fx.PayRepo = New PaymentConditionRepositoryStub
        fx.EmpRepo = New EmployeeRepositoryStub
        fx.TaxRepo = New SalesTaxRepositoryStub

        fx.Employee = fx.EmpRepo.Seed("E001", "担当太郎", "たんとうたろう")
        fx.Payment = fx.PayRepo.Seed("月末締翌月末")
        fx.Tax = fx.TaxRepo.Seed(New Date(2019, 10, 1), 0.1D)

        fx.Customer = New Customer(1, fx.CustRepo, fx.PayRepo, fx.EmpRepo)
        fx.Customer.Name = "株式会社テスト"
        fx.Customer.KanaName = "かぶしきがいしゃてすと"
        fx.Customer.PIC = fx.Employee
        fx.Customer.PaymentCondition = fx.Payment
        fx.Customer.PostalCode = "100-0001"
        fx.Customer.Address1 = "東京都千代田区"
        fx.Customer.Address2 = "1-1"
        fx.CustRepo.Attach(fx.Customer)

        Return fx
    End Function

    Public Function CreateValidEstimate() As Estimate
        Dim e = New Estimate(EstRepo, CustRepo, PayRepo, EmpRepo, TaxRepo)
        e.Customer = Customer
        e.Title = "システム開発"
        e.PaymentCondition = Payment
        e.PICEmployee = Employee
        e.SalesTax = Tax
        e.IssueDate = Date.Today
        e.DueDate = Date.Today.AddDays(30)
        e.EffectiveDate = Date.Today.AddDays(14)
        e.Remarks = ""
        Return e
    End Function
End Class
