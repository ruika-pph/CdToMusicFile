Public Class Form2
    'レジストリ関連
    Private regkey As Microsoft.Win32.RegistryKey
    Private cdRecordMode As Integer
    Private mp3RecordRate As Integer
    Private wmaProRecordRate As Integer
    Private wmaRecordQuality As Integer
    Private wmaRecordRate As Integer

    ''' <summary>
    ''' 初期化処理
    ''' </summary>
    ''' <param name="sender">イベントオブジェクト</param>
    ''' <param name="e">イベント情報</param>
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'キー（HKEY_CURRENT_USER\Software\Microsoft\MediaPlayer\Preferences）を開く
        regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\Microsoft\MediaPlayer\Preferences", True)

        'キーが存在しない場合
        If regkey Is Nothing Then
            MsgBox("キー異常")
            Me.Close()
        End If

        '値を読み込む
        cdRecordMode = CInt(regkey.GetValue("CDRecordMode"))
        mp3RecordRate = CInt(regkey.GetValue("MP3RecordRate"))
        wmaProRecordRate = CInt(regkey.GetValue("WMAProRecordRate"))
        wmaRecordQuality = CInt(regkey.GetValue("WMARecordQuality"))
        wmaRecordRate = CInt(regkey.GetValue("WMARecordRate"))

        'コンボボックス1の項目を設定
        ComboBox1.SelectedIndex = ConvertListIdx(cdRecordMode)
    End Sub

    ''' <summary>
    ''' OK
    ''' </summary>
    ''' <param name="sender">イベントオブジェクト</param>
    ''' <param name="e">イベント情報</param>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim setVal As Integer

        setVal = ConvertCDRecordMode(ComboBox1.SelectedIndex)

        '正常時以外は設定しない
        If setVal <> -1 Then
            '値を書き込む
            regkey.SetValue("CDRecordMode", setVal)
            regkey.SetValue("MP3RecordRate", mp3RecordRate)
            regkey.SetValue("WMAProRecordRate", wmaProRecordRate)
            regkey.SetValue("WMARecordQuality", wmaRecordQuality)
            regkey.SetValue("WMARecordRate", wmaRecordRate)
        End If

        Me.Close()
    End Sub

    ''' <summary>
    ''' キャンセル
    ''' </summary>
    ''' <param name="sender">イベントオブジェクト</param>
    ''' <param name="e">イベント情報</param>
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.Close()
    End Sub

    ''' <summary>
    ''' 終了時
    ''' </summary>
    ''' <param name="sender">イベントオブジェクト</param>
    ''' <param name="e">イベント情報</param>
    Private Sub Form2_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        'レジストリを閉じる
        regkey.Close()
    End Sub

    ''' <summary>
    ''' 音楽形式項目変更時
    ''' </summary>
    ''' <param name="sender">イベントオブジェクト</param>
    ''' <param name="e">イベント情報</param>
    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Select Case ComboBox1.SelectedIndex
            Case 0 'Windows Media オーディオ
                ComboBox2.Items.Clear()
                ComboBox2.Enabled = True
                ComboBox2.Items.Add("48 Kbps")
                ComboBox2.Items.Add("64 Kbps")
                ComboBox2.Items.Add("96 Kbps")
                ComboBox2.Items.Add("128 Kbps")
                ComboBox2.Items.Add("160 Kbps")
                ComboBox2.Items.Add("192 Kbps")
                ComboBox2.SelectedIndex = ConvertComb2(ComboBox1.SelectedIndex, wmaRecordRate)
            Case 1 'Windows Media オーディオ プロ
                ComboBox2.Items.Clear()
                ComboBox2.Enabled = True
                ComboBox2.Items.Add("32 Kbps")
                ComboBox2.Items.Add("48 Kbps")
                ComboBox2.Items.Add("64 Kbps")
                ComboBox2.Items.Add("96 Kbps")
                ComboBox2.Items.Add("128 Kbps")
                ComboBox2.Items.Add("160 Kbps")
                ComboBox2.Items.Add("192 Kbps")
                ComboBox2.SelectedIndex = ConvertComb2(ComboBox1.SelectedIndex, wmaProRecordRate)
            Case 2 'Windows Media オーディオ　(可変ビットレート)
                ComboBox2.Items.Clear()
                ComboBox2.Enabled = True
                ComboBox2.Items.Add("40 から 75 Kbps")
                ComboBox2.Items.Add("50 から 95 Kbps")
                ComboBox2.Items.Add("85 から 145 Kbps")
                ComboBox2.Items.Add("135 から 215 Kbps")
                ComboBox2.Items.Add("2400 から 355 Kbps")
                ComboBox2.SelectedIndex = ConvertComb2(ComboBox1.SelectedIndex, wmaRecordQuality)
            Case 4 'MP3
                ComboBox2.Items.Clear()
                ComboBox2.Enabled = True
                ComboBox2.Items.Add("128 Kbps")
                ComboBox2.Items.Add("192 Kbps")
                ComboBox2.Items.Add("256 Kbps")
                ComboBox2.Items.Add("320 Kbps")
                ComboBox2.SelectedIndex = ConvertComb2(ComboBox1.SelectedIndex, mp3RecordRate)
            Case Else
                ComboBox2.Items.Clear()
                ComboBox2.Enabled = False
        End Select

    End Sub

    ''' <summary>
    ''' 音質変更時
    ''' </summary>
    ''' <param name="sender">イベントオブジェクト</param>
    ''' <param name="e">イベント情報</param>
    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        ConvertKbps(ComboBox1.SelectedIndex, ComboBox2.SelectedIndex)
    End Sub

#Region "値変換関数"
    ''' <summary>
    ''' レジストリの値から対応するコンボボックスの項目に変換
    ''' </summary>
    ''' <param name="Comb1Idx">コンボボックス1のインデックス</param>
    ''' <param name="Comb2Val">レジストリの値</param>
    ''' <returns></returns>
    Private Function ConvertComb2(ByVal Comb1Idx As Integer, ByVal Comb2Val As Integer) As Integer
        Dim rtn As Integer = -1

        Select Case Comb1Idx
            Case 0 'Windows Media オーディオ
                Select Case Comb2Val
                    Case 48000
                        rtn = 0
                    Case 64000
                        rtn = 1
                    Case 96000
                        rtn = 2
                    Case 128000
                        rtn = 3
                    Case 160000
                        rtn = 4
                    Case 192000
                        rtn = 5
                End Select

            Case 1 'Windows Media オーディオ プロ
                Select Case Comb2Val
                    Case 32000
                        rtn = 0
                    Case 48000
                        rtn = 1
                    Case 64000
                        rtn = 2
                    Case 96000
                        rtn = 3
                    Case 128000
                        rtn = 4
                    Case 160000
                        rtn = 5
                    Case 192000
                        rtn = 6
                End Select
            Case 2 'Windows Media オーディオ　(可変ビットレート)
                Select Case Comb2Val
                    Case 25
                        rtn = 0
                    Case 50
                        rtn = 1
                    Case 75
                        rtn = 2
                    Case 90
                        rtn = 3
                    Case 98
                        rtn = 4
                End Select
            Case 4 'MP3
                Select Case Comb2Val
                    Case 128000
                        rtn = 0
                    Case 192000
                        rtn = 1
                    Case 256000
                        rtn = 2
                    Case 320000
                        rtn = 3
                End Select
        End Select

        Return rtn

    End Function

    ''' <summary>
    ''' コンボボックスの値からレジストリの値に変換
    ''' </summary>
    ''' <param name="Comb1Idx">コンボボックス1のインデックス</param>
    ''' <param name="Comb2Idx">コンボボックス2のインデックス</param>
    Private Sub ConvertKbps(ByVal Comb1Idx As Integer, ByVal Comb2Idx As Integer)
        Select Case Comb1Idx
            Case 0 'Windows Media オーディオ
                Select Case Comb2Idx
                    Case 0
                        wmaRecordRate = 48000
                    Case 1
                        wmaRecordRate = 64000
                    Case 2
                        wmaRecordRate = 96000
                    Case 3
                        wmaRecordRate = 128000
                    Case 4
                        wmaRecordRate = 160000
                    Case 5
                        wmaRecordRate = 192000
                End Select

            Case 1 'Windows Media オーディオ プロ
                Select Case Comb2Idx
                    Case 0
                        wmaProRecordRate = 32000
                    Case 1
                        wmaProRecordRate = 48000
                    Case 2
                        wmaProRecordRate = 64000
                    Case 3
                        wmaProRecordRate = 96000
                    Case 4
                        wmaProRecordRate = 128000
                    Case 5
                        wmaProRecordRate = 160000
                    Case 6
                        wmaProRecordRate = 192000
                End Select
            Case 2 'Windows Media オーディオ　(可変ビットレート)
                Select Case Comb2Idx
                    Case 0
                        wmaRecordQuality = 25
                    Case 1
                        wmaRecordQuality = 50
                    Case 2
                        wmaRecordQuality = 75
                    Case 3
                        wmaRecordQuality = 90
                    Case 4
                        wmaRecordQuality = 98
                End Select
            Case 4 'MP3
                Select Case Comb2Idx
                    Case 0
                        mp3RecordRate = 128000
                    Case 1
                        mp3RecordRate = 192000
                    Case 2
                        mp3RecordRate = 256000
                    Case 3
                        mp3RecordRate = 320000
                End Select
        End Select
    End Sub


    ''' <summary>
    ''' レジストリからコンボボックスの何番目のアイテムか判別する
    ''' </summary>
    ''' <param name="intVal">レジストリの値</param>
    ''' <returns>コンボボックスのインデックス</returns>
    Private Function ConvertListIdx(ByVal intVal As Integer) As Integer
        Dim rtn As Integer = -1

        Select Case intVal
            Case 0 'Windows Media オーディオ
                rtn = 0
            Case 1 'MP3
                rtn = 4
            Case 2 'Windows Media オーディオ　(可変ビットレート)
                rtn = 2
            Case 3 'Windows Media オーディオ　ロスレス
                rtn = 3
            Case 4 'WAV(無損失)
                rtn = 5
            Case 5 'Windows Media オーディオ プロ
                rtn = 1
            Case 6 'ALAC(ロスレス) 
                rtn = 6
            Case 7 'FLAC(ロスレス)
                rtn = 7
        End Select

        Return rtn
    End Function

    ''' <summary>
    ''' コンボボックスの項目からレジストリの値を判別する
    ''' </summary>
    ''' <returns>レジストリの値</returns>
    Private Function ConvertCDRecordMode(ByVal intVal As Integer) As Integer
        Dim rtn As Integer = -1

        Select Case intVal
            Case 0 'Windows Media オーディオ
                rtn = 0
            Case 1 'Windows Media オーディオ プロ
                rtn = 5
            Case 2 'Windows Media オーディオ　(可変ビットレート)
                rtn = 2
            Case 3 'Windows Media オーディオ　ロスレス
                rtn = 3
            Case 4 'MP3
                rtn = 1
            Case 5 'WAV(無損失)
                rtn = 4
            Case 6 'ALAC(ロスレス) 
                rtn = 6
            Case 7 'FLAC(ロスレス)
                rtn = 7
        End Select

        Return rtn
    End Function
#End Region

End Class