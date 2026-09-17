Option Strict On
Option Infer On
Imports Domain
Imports System.Collections.Generic
Imports System.Linq

''' <summary>
''' 支払条件リポジトリのインメモリ実装（単体テスト用）
''' </summary>
Public Class PaymentConditionRepositoryStub
    Implements Domain.IPaymentConditionRepository

    Private _items As New List(Of PaymentCondition)
    Private _seq As Integer = 0
    Private _lastId As Integer = -1

    Public Function Save(e As PaymentCondition) As Boolean Implements IPaymentConditionRepository.Save
        If e.ID = -1 Then
            _seq += 1
            _lastId = _seq
            Dim stored = New PaymentCondition(_seq, Me)
            stored.Name = e.Name
            stored.DueDate = e.DueDate
            stored.CutOff = e.CutOff
            stored.MonthOffset = e.MonthOffset
            _items.Add(stored)
        Else
            Dim idx = _items.FindIndex(Function(x) x.ID = e.ID)
            If idx < 0 Then Return False
            _items(idx).Name = e.Name
            _items(idx).DueDate = e.DueDate
            _items(idx).CutOff = e.CutOff
            _items(idx).MonthOffset = e.MonthOffset
            _lastId = e.ID
        End If
        Return True
    End Function

    Public Function LastInsertID() As Integer Implements IPaymentConditionRepository.LastInsertID
        Return _lastId
    End Function

    Public Function FindByID(id As Integer) As PaymentCondition Implements IPaymentConditionRepository.FindByID
        Return _items.FirstOrDefault(Function(x) x.ID = id)
    End Function

    Public Function FindByName(name As String) As PaymentCondition Implements IPaymentConditionRepository.FindByName
        Return _items.FirstOrDefault(Function(x) x.Name = name)
    End Function

    Public Function FindAllPaymentCondition() As List(Of PaymentCondition) Implements IPaymentConditionRepository.FindAllPaymentCondition
        Return New List(Of PaymentCondition)(_items)
    End Function

    Public Function FindPaymentConditionByCondition(cond As PaymentConditionRepositorySearchCondition) As List(Of PaymentCondition) Implements IPaymentConditionRepository.FindPaymentConditionByCondition
        Return New List(Of PaymentCondition)(_items)
    End Function

    Public Function CountAllPaymentCondition() As Integer Implements IPaymentConditionRepository.CountAllPaymentCondition
        Return _items.Count
    End Function

    Public Function Seed(name As String, Optional cutOff As Integer = 20, Optional dueDate As Integer = 20, Optional monthOffset As Integer = 1) As PaymentCondition
        Dim p = New PaymentCondition(Me)
        p.Name = name
        p.CutOff = cutOff
        p.DueDate = dueDate
        p.MonthOffset = monthOffset
        Save(p)
        Return FindByID(_lastId)
    End Function
End Class
