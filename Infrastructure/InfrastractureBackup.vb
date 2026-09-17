Option Strict On
Option Infer On

Imports System.Data.SQLite
Imports System.IO
Imports System.Text

''' <summary>
''' 永続化先のバックアップとリストアを提供する
''' </summary>
Public Class InfrastractureBackup

    Private Shared ReadOnly SqliteHeader As Byte() =
        Encoding.ASCII.GetBytes("SQLite format 3" & ChrW(0))

    ''' <summary>
    ''' バックアップする
    ''' </summary>
    Public Shared Function BackupToFile(ByVal path As String) As Boolean
        If String.IsNullOrWhiteSpace(path) Then
            Return False
        End If

        Dim dbPath = GetDBPath()
        If File.Exists(dbPath) = False Then
            Return False
        End If

        Try
            ReleaseConnections()
            File.Copy(dbPath, path, True)
            Return True
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' リストアする
    ''' </summary>
    Public Shared Function RestoreFromFile(ByVal path As String) As Boolean
        If String.IsNullOrWhiteSpace(path) OrElse File.Exists(path) = False Then
            Return False
        End If

        If IsValidSqliteDatabase(path) = False Then
            Return False
        End If

        Dim dbPath = GetDBPath()
        Dim safetyCopy = dbPath & ".pre-restore-" & DateTime.Now.ToString("yyyyMMddHHmmssfff")
        Dim hadExisting = File.Exists(dbPath)

        Try
            ReleaseConnections()

            If hadExisting Then
                File.Copy(dbPath, safetyCopy, True)
            End If

            File.Copy(path, dbPath, True)

            If IsValidSqliteDatabase(dbPath) = False Then
                RollbackRestore(dbPath, safetyCopy, hadExisting)
                Return False
            End If

            ReleaseConnections()
            Return True
        Catch
            RollbackRestore(dbPath, safetyCopy, hadExisting)
            Return False
        Finally
            If File.Exists(safetyCopy) Then
                Try
                    File.Delete(safetyCopy)
                Catch
                    ' 安全コピー削除失敗は致命ではない
                End Try
            End If
        End Try
    End Function

    ''' <summary>
    ''' SQLite3のDBファイルパスを取得
    ''' </summary>
    Public Shared Function GetDBPath() As String
#If DEBUG Then
        Dim path = "."
#Else
        Dim path = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
#End If
        Return IO.Path.Combine(path, "myDb.db")
    End Function

    ''' <summary>
    ''' 接続プールを解放してファイル操作可能にする
    ''' </summary>
    Private Shared Sub ReleaseConnections()
        SQLiteConnection.ClearAllPools()
        GC.Collect()
        GC.WaitForPendingFinalizers()
    End Sub

    ''' <summary>
    ''' ファイルが SQLite DB として妥当か検証する
    ''' </summary>
    Private Shared Function IsValidSqliteDatabase(ByVal path As String) As Boolean
        If HasSqliteHeader(path) = False Then
            Return False
        End If

        Try
            Using con As New SQLiteConnection($"data source={path};read only=True;failifmissing=True;")
                con.Open()
                Using cmd = con.CreateCommand()
                    cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master;"
                    cmd.ExecuteScalar()
                End Using
            End Using
            Return True
        Catch
            Return False
        Finally
            SQLiteConnection.ClearAllPools()
        End Try
    End Function

    ''' <summary>
    ''' SQLite ファイルヘッダを確認
    ''' </summary>
    Private Shared Function HasSqliteHeader(ByVal path As String) As Boolean
        Try
            Using fs = File.OpenRead(path)
                If fs.Length < SqliteHeader.Length Then
                    Return False
                End If
                Dim buf(SqliteHeader.Length - 1) As Byte
                If fs.Read(buf, 0, buf.Length) <> buf.Length Then
                    Return False
                End If
                For i = 0 To SqliteHeader.Length - 1
                    If buf(i) <> SqliteHeader(i) Then
                        Return False
                    End If
                Next
            End Using
            Return True
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' リストア失敗時に元 DB を復元
    ''' </summary>
    Private Shared Sub RollbackRestore(ByVal dbPath As String, ByVal safetyCopy As String, ByVal hadExisting As Boolean)
        Try
            ReleaseConnections()
            If hadExisting AndAlso File.Exists(safetyCopy) Then
                File.Copy(safetyCopy, dbPath, True)
            ElseIf hadExisting = False AndAlso File.Exists(dbPath) Then
                File.Delete(dbPath)
            End If
        Catch
            ' ロールバック失敗時は呼び出し側で False を返す
        End Try
    End Sub

End Class
