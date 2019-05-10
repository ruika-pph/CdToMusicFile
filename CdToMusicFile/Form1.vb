Imports System.IO

Public Class Form1

    'WMP関連インスタンス生成
    Private wmp As New WMPLib.WindowsMediaPlayer
    Private cdDrives As WMPLib.IWMPCdromCollection = wmp.cdromCollection
    Private cdRom As WMPLib.IWMPCdrom = Nothing
    Private ripper As WMPLib.IWMPCdromRip = Nothing

    ''' <summary>
    ''' 初期化処理
    ''' </summary>
    ''' <param name="sender">イベントオブジェクト</param>
    ''' <param name="e">イベント情報</param>
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            CDInfoGet()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 取込
    ''' </summary>
    ''' <param name="sender">イベントオブジェクト</param>
    ''' <param name="e">イベント情報</param>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        ' フォームを無効にしてボタンを押せないようにする
        If ListBox1.SelectedItems.Count > 0 Then
            Me.Enabled = False

            For i = 0 To ListBox1.Items.Count - 1
                If ListBox1.GetSelected(i) Then
                    cdRom.Playlist.Item(i).setItemInfo("SelectedForRip", "True")
                Else
                    cdRom.Playlist.Item(i).setItemInfo("SelectedForRip", "False")
                End If
            Next

            ripper.startRip()

            Timer1.Enabled = True
        Else
            MsgBox("項目が選択されていません")
        End If

    End Sub

    ''' <summary>
    ''' 更新
    ''' </summary>
    ''' <param name="sender">イベントオブジェクト</param>
    ''' <param name="e">イベント情報</param>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            CDInfoGet()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 設定
    ''' </summary>
    ''' <param name="sender">イベントオブジェクト</param>
    ''' <param name="e">イベント情報</param>
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        'Form2クラスのインスタンスを作成する
        Dim frm As New Form2()
        'モーダルで表示
        frm.ShowDialog(Me)
        '破棄
        frm.Dispose()
    End Sub

    ''' <summary>
    ''' 進捗状況更新
    ''' </summary>
    ''' <param name="sender">イベントオブジェクト</param>
    ''' <param name="e">イベント情報</param>
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ProgressBar1.Value = ripper.ripProgress
        'メッセージ・キューにあるWindowsメッセージをすべて処理し、画面が固まらないようにする
        Application.DoEvents()

        If ripper.ripState <> WMPLib.WMPRipState.wmprsRipping Then
            Timer1.Enabled = False
            'フォームを有効に戻す
            Me.Enabled = True
        End If
    End Sub

    ''' <summary>
    ''' 全て選択
    ''' </summary>
    ''' <param name="sender">イベントオブジェクト</param>
    ''' <param name="e">イベント情報</param>
    Private Sub 全て選択ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 全て選択ToolStripMenuItem.Click
        Dim k As Integer
        For k = 0 To ListBox1.Items.Count - 1
            If ListBox1.GetSelected(k) Then
            Else
                ListBox1.SetSelected(k, True)
            End If
        Next
    End Sub

    ''' <summary>
    ''' 全て解除
    ''' </summary>
    ''' <param name="sender">イベントオブジェクト</param>
    ''' <param name="e">イベント情報</param>
    Private Sub 全て解除ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 全て解除ToolStripMenuItem.Click
        ListBox1.ClearSelected()
    End Sub

    ''' <summary>
    ''' CDから情報を取得する関数
    ''' </summary>
    Private Sub CDInfoGet()
        Try
            'ドライブ一覧を取得及びドライブのインスタンス生成
            Dim drives As String() = Directory.GetLogicalDrives()
            Dim drive As System.IO.DriveInfo = Nothing
            Dim cdDriveName As String = ""
            Dim cdTitle As String
            Dim songTitle As String
            Dim CdDrive As Boolean = False
            Dim K As Integer

            ListBox1.Items.Clear()
            wmp = New WMPLib.WindowsMediaPlayer
            cdDrives = wmp.cdromCollection

            'ディスクトライブがあるかチェック
            For Each driveName As String In drives
                'ドライブの情報を取得
                drive = New System.IO.DriveInfo(driveName)

                'CDドライブか判定
                Select Case drive.DriveType
                    Case System.IO.DriveType.CDRom
                        cdDriveName = driveName
                        CdDrive = True
                        '見つかったらループ終了
                        Exit For
                End Select
            Next

            'ドライブが使用できるか
            If CdDrive AndAlso drive.IsReady Then
                cdRom = cdDrives.getByDriveSpecifier(cdDriveName(0))
                ripper = cdRom
                cdTitle = cdRom.Playlist.name
                CdName.Text = cdTitle

                '曲の数ループ
                For K = 0 To cdRom.Playlist.count - 1
                    songTitle = cdRom.Playlist.Item(K).name
                    '曲のタイトルを追加
                    ListBox1.Items.Add(songTitle)
                Next
            Else
                CdName.Text = "ドライブにディスクがありません"
            End If

        Catch ex As Exception
            Throw New Exception(System.Reflection.MethodBase.GetCurrentMethod.Name & ":" & ex.Message)
        End Try
    End Sub
End Class
