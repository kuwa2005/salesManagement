Option Strict On
Option Infer On
Imports Domain
Imports System.Collections.Generic
Imports System.Linq

''' <summary>
''' 顧客リポジトリのインメモリ実装（単体テスト用）
''' </summary>
Public Class CustomerRepositoryStub
    Implements Domain.ICustomerRepository

    Private _items As New List(Of Customer)
    Private _seq As Integer = 0
    Private _lastId As Integer = -1

    Public Function Save(c As Customer) As Boolean Implements ICustomerRepository.Save
        If c.ID = -1 Then
            _seq += 1
            _lastId = _seq
            ' ID付きの新インスタンスはテスト側で Attach する想定。ここでは連番だけ確保する。
            Return True
        Else
            Dim idx = _items.FindIndex(Function(x) x.ID = c.ID)
            If idx < 0 Then
                _items.Add(c)
            Else
                _items(idx) = c
            End If
            _lastId = c.ID
            Return True
        End If
    End Function

    Public Function LastInsertID() As Integer Implements ICustomerRepository.LastInsertID
        Return _lastId
    End Function

    Public Function FindByCustomerNameAndAddress(custName As String, addr1 As String, addr2 As String) As Customer Implements ICustomerRepository.FindByCustomerNameAndAddress
        Return _items.FirstOrDefault(Function(x) x.Name = custName AndAlso x.Address1 = addr1 AndAlso x.Address2 = addr2)
    End Function

    Public Function FindCustomerByCondition(cond As CustomerRepositorySearchCondition) As List(Of Customer) Implements ICustomerRepository.FindCustomerByCondition
        Return New List(Of Customer)(_items)
    End Function

    Public Function CountAllCustomer() As Integer Implements ICustomerRepository.CountAllCustomer
        Return _items.Count
    End Function

    Public Function FindByID(id As Integer) As Customer Implements ICustomerRepository.FindByID
        Return _items.FirstOrDefault(Function(x) x.ID = id)
    End Function

    ''' <summary>
    ''' ID 済みの顧客をリポジトリに載せる
    ''' </summary>
    Public Sub Attach(c As Customer)
        If c Is Nothing OrElse c.ID < 0 Then
            Throw New ArgumentException("Attach には永続化済み相当（ID>=0）の顧客が必要です")
        End If
        Dim idx = _items.FindIndex(Function(x) x.ID = c.ID)
        If idx < 0 Then
            _items.Add(c)
        Else
            _items(idx) = c
        End If
        If c.ID > _seq Then
            _seq = c.ID
        End If
        _lastId = c.ID
    End Sub
End Class
