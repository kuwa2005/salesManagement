Option Strict On
Option Infer On

Imports System.IO
Imports System.Reflection
Imports Microsoft.Reporting.WinForms

''' <summary>
''' 見積書プレビュー画面
''' </summary>
Public Class EstimateReportViewer

#Region "定数"

    Private Const MainReportResource As String = "Application.Report.Estimate.rdlc"
    Private Const DetailReportResource As String = "Application.Report.EstimateDetail.rdlc"
    Private Const DetailSubreportName As String = "EstimateDetail"

#End Region

#Region "プロパティ"

    Private _PreviewEstimate As Domain.Estimate

    ''' <summary>
    ''' プレビューする見積
    ''' </summary>
    Public WriteOnly Property PreviewEstimate As Domain.Estimate
        Set(value As Domain.Estimate)
            _PreviewEstimate = value
            UpdateReport()
        End Set
    End Property

#End Region

#Region "イベント"

    ''' <summary>
    ''' ビュワーロード時
    ''' </summary>
    Private Sub EstimateReportViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        UpdateReport()
    End Sub

    ''' <summary>
    ''' サブレポート処理時に呼び出されるイベント
    ''' </summary>
    Public Sub SubReportProcessingEventHandler(ByVal sender As Object, ByVal e As SubreportProcessingEventArgs)
        e.DataSources.Clear()
        e.DataSources.Add(New ReportDataSource("EstimateDetailDataSet", MakeDetailsList))
    End Sub

#End Region

#Region "レポート制御"

    ''' <summary>
    ''' 明細のサブレポート用にリストを構成する
    ''' </summary>
    Private Function MakeDetailsList() As List(Of EstimateDetailReportPresenter)
        Dim ret As New List(Of EstimateDetailReportPresenter)
        If _PreviewEstimate Is Nothing OrElse _PreviewEstimate.Details Is Nothing Then
            Return ret
        End If

        For Each d In _PreviewEstimate.Details
            Dim p = New EstimateDetailReportPresenter(d)
            ret.Add(p)
        Next

        Return ret
    End Function

    ''' <summary>
    ''' レポートを表示する
    ''' </summary>
    Private Sub UpdateReport()
        If _PreviewEstimate Is Nothing Then
            Return
        End If

        ReportViewer.Reset()
        ReportViewer.ProcessingMode = ProcessingMode.Local
        ' Reset 後の LocalReport に対して、Refresh 前にハンドラを付ける
        AddHandler ReportViewer.LocalReport.SubreportProcessing, AddressOf SubReportProcessingEventHandler
        LoadReportsFromEmbeddedResources()

        BindingSource.DataSource = New EstimateReportPresenter(_PreviewEstimate)
        Dim rds = New ReportDataSource("EstimateDataSet", BindingSource)
        ReportViewer.LocalReport.DataSources.Add(rds)
        ReportViewer.RefreshReport()
    End Sub

    ''' <summary>
    ''' 埋め込み rdlc からメイン／サブレポート定義を読み込む
    ''' </summary>
    Private Sub LoadReportsFromEmbeddedResources()
        Dim asm = Assembly.GetExecutingAssembly()

        Using mainStream = OpenEmbeddedReport(asm, MainReportResource)
            ReportViewer.LocalReport.LoadReportDefinition(mainStream)
        End Using

        Using detailStream = OpenEmbeddedReport(asm, DetailReportResource)
            ReportViewer.LocalReport.LoadSubreportDefinition(DetailSubreportName, detailStream)
        End Using
    End Sub

    ''' <summary>
    ''' 埋め込みリソースのストリームを開く
    ''' </summary>
    Private Shared Function OpenEmbeddedReport(ByVal asm As Assembly, ByVal resourceName As String) As Stream
        Dim stream = asm.GetManifestResourceStream(resourceName)
        If stream Is Nothing Then
            Throw New InvalidOperationException("帳票リソースが見つかりません: " & resourceName)
        End If
        Return stream
    End Function

#End Region

End Class
