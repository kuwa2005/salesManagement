Option Strict On
Option Infer On
Imports Domain
Imports System.Collections.Generic
Imports System.Linq

''' <summary>
''' 従業員リポジトリのインメモリ実装（単体テスト用）
''' </summary>
Public Class EmployeeRepositoryStub
    Implements Domain.IEmployeeRepository

    Private _items As New List(Of Employee)
    Private _seq As Integer = 0
    Private _lastId As Integer = -1

    Public Function Save(e As Employee) As Boolean Implements IEmployeeRepository.Save
        If e.ID = -1 Then
            _seq += 1
            _lastId = _seq
            Dim stored = New Employee(_seq, Me)
            stored.EmployeeNo = e.EmployeeNo
            stored.Name = e.Name
            stored.NameKana = e.NameKana
            _items.Add(stored)
        Else
            Dim idx = _items.FindIndex(Function(x) x.ID = e.ID)
            If idx < 0 Then Return False
            _items(idx).EmployeeNo = e.EmployeeNo
            _items(idx).Name = e.Name
            _items(idx).NameKana = e.NameKana
            _lastId = e.ID
        End If
        Return True
    End Function

    Public Function LastInsertID() As Integer Implements IEmployeeRepository.LastInsertID
        Return _lastId
    End Function

    Public Function FindByID(id As Integer) As Employee Implements IEmployeeRepository.FindByID
        Return _items.FirstOrDefault(Function(x) x.ID = id)
    End Function

    Public Function FindByEmployeeNo(empNo As String) As Employee Implements IEmployeeRepository.FindByEmployeeNo
        Return _items.FirstOrDefault(Function(x) x.EmployeeNo = empNo)
    End Function

    Public Function FindAllEmployee() As List(Of Employee) Implements IEmployeeRepository.FindAllEmployee
        Return New List(Of Employee)(_items)
    End Function

    Public Function FindEmployeeByCondition(cond As EmployeeRepositorySearchCondition) As List(Of Employee) Implements IEmployeeRepository.FindEmployeeByCondition
        Return New List(Of Employee)(_items)
    End Function

    Public Function CountAllEmployee() As Integer Implements IEmployeeRepository.CountAllEmployee
        Return _items.Count
    End Function

    Public Function Seed(empNo As String, name As String, Optional nameKana As String = "") As Employee
        Dim e = New Employee(Me)
        e.EmployeeNo = empNo
        e.Name = name
        e.NameKana = nameKana
        Save(e)
        Return FindByID(_lastId)
    End Function
End Class
