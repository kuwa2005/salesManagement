Option Strict On
Option Infer On

Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Infrastructure

<TestClass()>
Public Class TestBackup

    <TestInitialize()>
    Public Sub Setup()
        Dim setting = New InfrastructureSetting
        setting.InitializeDB(forceResetDebugDb:=True)
    End Sub

    <TestMethod()>
    Public Sub RestoreFromFile_RejectsNonSqliteFile()
        Dim junk = Path.Combine(Path.GetTempPath(), "not-a-db-" & Guid.NewGuid().ToString("N") & ".db")
        File.WriteAllText(junk, "this is not sqlite")
        Try
            Assert.IsFalse(InfrastructureBackup.RestoreFromFile(junk))
            Assert.IsTrue(File.Exists(InfrastructureBackup.GetDBPath()))
        Finally
            If File.Exists(junk) Then
                File.Delete(junk)
            End If
        End Try
    End Sub

    <TestMethod()>
    Public Sub BackupToFile_CreatesCopy()
        Dim dest = Path.Combine(Path.GetTempPath(), "backup-" & Guid.NewGuid().ToString("N") & ".db")
        Try
            Assert.IsTrue(InfrastructureBackup.BackupToFile(dest))
            Assert.IsTrue(File.Exists(dest))
            Assert.IsTrue(New FileInfo(dest).Length > 0)
        Finally
            If File.Exists(dest) Then
                File.Delete(dest)
            End If
        End Try
    End Sub

End Class
