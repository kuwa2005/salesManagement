Option Strict On
Option Infer On
Imports Domain
Imports System.Collections.Generic
Imports System.Linq

''' <summary>
''' 見積リポジトリのインメモリ実装（単体テスト用）
''' </summary>
Public Class EstimateRepositoryStub
    Implements Domain.IEstimateRepository

    Private _items As New List(Of Estimate)
    Private _seq As Integer = 0
    Private _lastId As Integer = -1

    Public Function Save(e As Estimate) As Boolean Implements IEstimateRepository.Save
        If e.Validate = False Then Return False
        If e.ID = -1 Then
            _seq += 1
            _lastId = _seq
            _items.Add(e)
        Else
            Dim idx = _items.FindIndex(Function(x) x.ID = e.ID)
            If idx < 0 Then Return False
            _items(idx) = e
            _lastId = e.ID
        End If
        Return True
    End Function

    Public Function LastInsertID() As Integer Implements IEstimateRepository.LastInsertID
        Return _lastId
    End Function

    Public Function CountEstimateOnDay(d As Date) As Integer Implements IEstimateRepository.CountEstimateOnDay
        Return _items.Where(Function(x) x.IssueDate.Date = d.Date).Count()
    End Function

    Public Function CountAllEstimate() As Integer Implements IEstimateRepository.CountAllEstimate
        Return _items.Count
    End Function

    Public Function FindEstimateByCondition(c As EstimateRepositorySearchCondition) As List(Of Estimate) Implements IEstimateRepository.FindEstimateByCondition
        Return New List(Of Estimate)(_items)
    End Function
End Class
