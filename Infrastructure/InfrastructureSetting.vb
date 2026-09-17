Option Strict On
Option Infer On

''' <summary>
''' インフラストラクチャの設定
''' </summary>
Public Class InfrastructureSetting

    ''' <summary>
    ''' データベースを初期化する
    ''' </summary>
    ''' <param name="resetDebugDatabase">DEBUG 時に既存 DB を削除して作り直す場合は True（テスト向け）</param>
    Public Function InitializeDB(Optional ByVal resetDebugDatabase As Boolean = False) As Boolean
#If DEBUG Then
        If resetDebugDatabase OrElse IsDebugResetRequested() Then
            ResetDebugDatabase()
        End If
#End If
        If IsExistDB() = False Then
            execDDL()
        End If

        Return True
    End Function

    ''' <summary>
    ''' DDLを実行
    ''' </summary>
    Private Sub execDDL()
        'DDLを読み込み
#If DEBUG Then
        Dim ddl = IO.File.ReadAllText("../../../Infrastructure/DDL/DDL.txt")
#Else
        Dim ddl = IO.File.ReadAllText("DDL.txt")
#End If


        'DDLを実行
        Using accessor As New ADOWrapper.DBAccessor
            Dim q = accessor.CreateQuery
            q.Query.AppendLine(ddl)
            q.ExecNonQuery()
        End Using
    End Sub

    ''' <summary>
    ''' 環境変数またはセンチネルファイルで DEBUG DB リセットが要求されているか
    ''' </summary>
    Private Shared Function IsDebugResetRequested() As Boolean
        Dim resetEnv = Environment.GetEnvironmentVariable("SM_RESET_DEBUG_DB")
        Return String.Equals(resetEnv, "1", StringComparison.Ordinal) OrElse
            IO.File.Exists("RESET_DEBUG_DB")
    End Function

    ''' <summary>
    ''' デバッグ用データベースファイルを削除する
    ''' </summary>
    Private Sub ResetDebugDatabase()
        ' 接続プールが残っていると Delete に失敗するため解放する
        System.Data.SQLite.SQLiteConnection.ClearAllPools()
        GC.Collect()
        GC.WaitForPendingFinalizers()

        If IO.File.Exists("myDb.db") Then
            IO.File.Delete("myDb.db")
        End If
    End Sub

    ''' <summary>
    ''' DBが既に存在している場合はTrue
    ''' </summary>
    Private Function IsExistDB() As Boolean
        If IO.File.Exists(InfrastructureBackup.GetDBPath()) = True Then
            Return True
        End If
        Return False
    End Function

End Class
