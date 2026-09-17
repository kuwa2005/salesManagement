Option Strict On
Option Infer On
Imports Domain
Imports System.Collections.Generic
Imports System.Linq

''' <summary>
''' 消費税率リポジトリのインメモリ実装（単体テスト用）
''' </summary>
Public Class SalesTaxRepositoryStub
    Implements Domain.ISalesTaxRepository

    Private _items As New List(Of SalesTax)
    Private _seq As Integer = 0
    Private _lastId As Integer = -1

    Public Function LastInsertID() As Integer Implements ISalesTaxRepository.LastInsertID
        Return _lastId
    End Function

    Public Function Save(tax As SalesTax) As Boolean Implements ISalesTaxRepository.Save
        If tax.ID = -1 Then
            _seq += 1
            _lastId = _seq
            Dim stored = New SalesTax(_seq, Me)
            stored.ApplyStartDate = tax.ApplyStartDate
            stored.TaxRate = tax.TaxRate
            _items.Add(stored)
        Else
            Dim idx = _items.FindIndex(Function(x) x.ID = tax.ID)
            If idx < 0 Then Return False
            _items(idx).ApplyStartDate = tax.ApplyStartDate
            _items(idx).TaxRate = tax.TaxRate
            _lastId = tax.ID
        End If
        Return True
    End Function

    Public Function Save(taxes As List(Of SalesTax)) As Boolean Implements ISalesTaxRepository.Save
        For Each t In taxes
            If Save(t) = False Then Return False
        Next
        Return True
    End Function

    Public Function TaxOn(d As Date) As SalesTax Implements ISalesTaxRepository.TaxOn
        Return _items.
            Where(Function(x) x.ApplyStartDate.Date <= d.Date).
            OrderByDescending(Function(x) x.ApplyStartDate).
            FirstOrDefault()
    End Function

    Public Function FindAll() As List(Of SalesTax) Implements ISalesTaxRepository.FindAll
        Return New List(Of SalesTax)(_items)
    End Function

    Public Function FindByApplyDate(d As Date) As SalesTax Implements ISalesTaxRepository.FindByApplyDate
        Return _items.FirstOrDefault(Function(x) x.ApplyStartDate.Date = d.Date)
    End Function

    Public Function FindByID(id As Integer) As SalesTax Implements ISalesTaxRepository.FindByID
        Return _items.FirstOrDefault(Function(x) x.ID = id)
    End Function

    Public Function Seed(applyStart As Date, rate As Decimal) As SalesTax
        Dim t = New SalesTax(Me)
        t.ApplyStartDate = applyStart
        t.TaxRate = rate
        Save(t)
        Return FindByID(_lastId)
    End Function
End Class
