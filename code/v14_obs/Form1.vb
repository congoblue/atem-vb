﻿Imports System.Net.Sockets
Imports System.Net
Imports System.Text
Imports System.IO.Ports
Imports System.Threading
Imports System.ComponentModel
Imports System.Web.Script.Serialization
Imports WebSocket4Net

Public Class MainForm

    Const BUFSIZE As Integer = 256
    Dim _serialPort As New SerialPort
    Dim buffer(BUFSIZE) As Byte
    Dim BufPutIndex As Integer = 0
    Dim BufGetIndex As Integer = 0
    Dim BufCount As Integer = 0
    Dim startuptimer As Integer = 0
    Dim ShutDownTimer As Integer = 0
    Dim ComCheckTimer As Integer = 18
    Dim addr As Integer
    Dim inpaddr As Integer
    Dim liveaddr As Integer
    Dim gotrx As Integer
    Dim kc As Integer
    Dim cdir As Integer
    Dim savemode As Boolean
    Dim legendmode As Boolean
    Dim atemconnect As Boolean = False
    Dim camconnect As Boolean
    Dim overlayactive As Boolean
    Dim mediaoverlayactive As Boolean
    Dim mediaindex As Integer = 0
    Dim VISMacrosConnection As New System.Net.Sockets.TcpClient()
    Dim ServerNetworkStream As NetworkStream
    Dim websocket As WebSocket4Net.WebSocket
    Dim OBSResponse As String
    Dim OBSRecState As Boolean
    Dim OBSStreamState As Boolean
    Dim OBSRecTime As String
    Dim OBSStreamTime As String
    Dim gain(8) As Integer
    Dim wblock(8) As Integer
    Dim DelayStop As Integer
    Dim DelayAddr As Integer
    Dim nextpreview As Integer
    Dim transitionwait As Integer
    Dim SerialInBuf(16) As Byte
    Dim SerialInBufPtr As Byte
    Dim presetstate(8) As Integer
    Dim CamIris(8) As Integer
    Dim CamAgc(8) As Integer
    Dim CamShutter(8) As Integer
    Dim CamGain(8) As Integer
    Dim CamWBRed(8) As Integer
    Dim CamWBBlue(8) As Integer
    Dim CamFocusManual(8) As Integer
    Dim ProgCloseTimer As Integer = 0
    Dim OverlayWasActive As Integer = 0
    Dim CamIgnore(8) As Boolean
    Dim PendingZoom As Integer = 0
    Dim PendingPan As Integer = 0
    Dim PendingTilt As Integer = 0
    Dim PzTimer As Integer
    Dim PzDir As Integer
    Dim PpDir As Integer
    Dim PtDir As Integer
    Dim PzAddr As Integer
    Dim PTZLive As Boolean = False
    Dim PresetLive As Boolean = False
    Dim AutoSongMode As Boolean = False
    Dim AutoSongPreload As Boolean = False
    Dim PrevAutoSongShot As Integer
    Dim AutoSpeechMode As Boolean = False
    Dim PrevAutoSpeech As Boolean = False
    Dim SingleAutoShot As Integer = 0
    Dim AutoPreset As Integer = 0
    Dim AutoWait As Integer = 0
    Dim CutLockoutTimer As Integer
    Dim ctrlkey As Boolean
    Dim CamOverride As Integer = 0
    Dim AudioFade As Integer
    Dim AudioLevel As Integer
    Dim MediaLevel As Integer
    Dim MediaNext As Boolean
    Dim PlayerAutoPause As Boolean
    Dim PlayerAutoNext As Boolean
    Dim PlayerLoop As Boolean
    Dim TieAux3 As Boolean = True
    Dim CurrentAux3 As Integer
    Dim PrevAux3 As Integer = 0
    Dim CamRec(4) As Boolean
    Dim CamRecStatusTimer As Integer = 0
    Dim CamCmdPending As Boolean = False
    Dim PipAddr As Integer = 0 'the input showing pip
    Dim MediaItem As Integer
    Dim MediaPlayerWasActive As Boolean = False
    Dim WebsocketID As Integer
    Dim Websocketwait As Boolean
    Dim WebsocketReinitTimer As Integer = 0
    Dim MovePresetMode As Integer = 0
    Dim MovePresetFrom As Integer = 0

    Dim JoyX As Byte
    Dim JoyY As Byte
    Dim JoyZ As Byte
    Dim PrevJoyX As Byte
    Dim PrevJoyY As Byte
    Dim PrevJoyZ As Byte
    Dim Key(3) As Byte
    Dim REncode As Byte
    Dim JoystickActive As Boolean
    Dim PrevZoom As Byte
    Dim PrevEncode As Byte
    Dim PrevKey(3) As Byte
    Dim KeyHit As Boolean
    Dim EncChange As Boolean
    Dim LastKey As Byte
    Dim serialcount As Byte = 0

    Dim prevmdir As Integer
    Dim prevxspeed As Integer
    Dim prevyspeed As Integer
    Dim joyconvert() As Byte = {1, 5, 8, 9, 10, 12, 13, 14, 15, 15, 16, 17, 17, 18, 18, 19, 19, 20, 20, 21, 21, 22, 22, 23, 23, 24, 24, 24, 25, 25, 25, 26, 26, 26, 27, 27, 27, 28, 28, 28, 29, 29, 29, 30, 30, 30, 31, 31, 31, 31, 32, 32, 32, 33, 33, 33, 34, 34, 34, 34, 34, 35, 35, 36, 36, 36, 36, 36, 37, 37, 37, 38, 38, 38, 38, 39, 39, 39, 39, 40, 40, 40, 40, 41, 41, 41, 41, 42, 42, 42, 42, 42, 43, 43, 44, 44, 44, 44, 44, 45, 45, 45, 45, 46, 46, 46, 46, 47, 47, 47, 47, 47, 48, 48, 48, 49, 49, 49, 49, 49, 50, 50, 50, 50, 50, 50, 50, 50}
    Dim zoomconvert() As Byte = {1, 7, 11, 14, 16, 18, 20, 21, 22, 23, 24, 25, 26, 27, 28, 28, 29, 29, 30, 30, 31, 31, 32, 32, 33, 33, 34, 34, 34, 35, 35, 35, 36, 36, 36, 37, 37, 37, 37, 38, 38, 38, 38, 39, 39, 39, 39, 40, 40, 40, 40, 40, 41, 41, 41, 41, 41, 41, 42, 42, 42, 42, 42, 43, 43, 43, 43, 43, 43, 43, 44, 44, 44, 44, 44, 44, 44, 45, 45, 45, 45, 45, 45, 45, 45, 46, 46, 46, 46, 46, 46, 46, 46, 46, 47, 47, 47, 47, 47, 47, 47, 47, 47, 48, 48, 48, 48, 48, 48, 48, 48, 49, 49, 49, 49, 49, 49, 49, 50, 50, 50, 50, 50, 50, 50, 50, 50, 50}

    Dim alreadysending As Boolean
    Dim PreloadPreset As Integer
    Dim LiveMoveSpeed As Integer = 0
    Dim ClipRemainTime As Integer

    Dim LuaReturn As String
    Dim PipLive As Boolean

    'arrays to store preset positions
    Dim PresetCaption(128) As String
    Dim PresetXPos(64) As String
    Dim PresetYPos(64) As String
    Dim PresetZPos(64) As String
    Dim PresetContent(128) As Integer
    Dim PresetSize(128) As Integer
    Dim PresetAuto(128) As Integer
    Dim PresetFocusAuto(64) As Boolean
    Dim PresetFocus(64) As Integer

    Dim StreamStartTime, RecStartTime As Integer

    'this is the time in sec to move 1000 counts
    Dim PTTime() = {93.3, 62.5, 45.1, 34.1, 26.5, 21.3, 15.5, 10.7, 7.58, 5.5, 3.64, 2.86, 2.29, 1.79, 1.43}
    Dim ZTime() = {20.8, 20.7, 20.6, 20.5, 20.4, 17.5, 14.7, 11.7, 11.3, 10.9}


    Private Sub VSLog(ByVal pvMsg As String)
        If Len(mLog.Text) Then
            mLog.Text = mLog.Text & Chr(13) & Chr(10)
        End If
        mLog.Text = mLog.Text & Now & "   " & pvMsg
    End Sub

    Function ObsSourceName(oaddr As Integer) As String
        ObsSourceName = ""
        If oaddr = 1 Then ObsSourceName = "Cam1"
        If oaddr = 2 Then ObsSourceName = "Cam2"
        If oaddr = 3 Then ObsSourceName = "Cam3"
        If oaddr = 4 Then ObsSourceName = "Cam4"
        If oaddr = 7 Then ObsSourceName = "Cam5"
    End Function

    Private Sub OpenWebSocket()
        websocket = New WebSocket4Net.WebSocket("ws://localhost:4444/")
        'AddHandler websocket.Opened, Sub(s, e) socketOpened(s, e)
        'AddHandler websocket.Error, Sub(s, e) SocketError(s, e)
        'AddHandler websocket.Closed, Sub(s, e) socketClosed(s, e)
        AddHandler websocket.MessageReceived, Sub(s, ev) socketMessage(s, ev)
        AddHandler websocket.DataReceived, Sub(s, ev) socketDataReceived(s, ev)
        websocket.Open()

        WebsocketReinitTimer = 10
    End Sub

    Private Sub WebsocketSendAndWait(ByVal msg As String)
        msg = Replace(msg, "TEST1", "WS" & WebsocketID)
        Websocketwait = True
        websocket.Send(msg)
        While Websocketwait
            Application.DoEvents()
        End While
    End Sub

    Private Sub ReadMediaSources()
        ListBoxMedia.Items.Clear
        websocket.Send("{""request-type"":""GetSceneList"",""message-id"":""GETSCENE""}")
    End Sub

    Private Sub ExecuteLua(ByVal pLuaText As String)
        If atemconnect = False Then Exit Sub
        Dim ServerNetworkStream As NetworkStream = VISMacrosConnection.GetStream()
        Dim lvCommandText As String

        lvCommandText = "<luacommandtoexecute>" & pLuaText & "</luacommandtoexecute>"

        Dim outStream As Byte() = System.Text.Encoding.ASCII.GetBytes(lvCommandText)

        ServerNetworkStream.Write(outStream, 0, outStream.Length)
        ServerNetworkStream.Flush()

        Dim inStream(10024) As Byte

        ServerNetworkStream.Read(inStream, 0, CInt(VISMacrosConnection.ReceiveBufferSize))

        Dim returndata As String = System.Text.Encoding.ASCII.GetString(inStream)

        LuaReturn = returndata

        If True Then 'chkLogMessages.Checked Then
            VSLog(pLuaText & " == " & returndata)
        End If
    End Sub

    Private Sub RequestLua(ByVal pLuaText As String)
        If atemconnect = False Then Exit Sub
        Dim ServerNetworkStream As NetworkStream = VISMacrosConnection.GetStream()
        Dim lvCommandText As String

        lvCommandText = "<request>" & pLuaText & "</request>"

        Dim outStream As Byte() = System.Text.Encoding.ASCII.GetBytes(lvCommandText)

        ServerNetworkStream.Write(outStream, 0, outStream.Length)
        ServerNetworkStream.Flush()

        Dim inStream(10024) As Byte

        ServerNetworkStream.Read(inStream, 0, CInt(VISMacrosConnection.ReceiveBufferSize))

        Dim returndata As String = System.Text.Encoding.ASCII.GetString(inStream)

        LuaReturn = returndata

        VSLog(pLuaText & " == " & returndata)

    End Sub


    Private Sub bConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bConnect.Click
        atemconnect = False
        Try
            VISMacrosConnection.Connect(eVISMacrosAppHost.Text, 33991)
            VISMacrosConnection.ReceiveBufferSize = 10024
            atemconnect = True
        Catch
            If MsgBox("Can't connect to Atem switcher." & vbCrLf & "Before starting this application, you must run the JustMacros application to communicate with the Atem switcher." & vbCrLf & "Click Yes to quit now, or No to continue in test mode", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                Application.Exit()
            Else
                atemconnect = False
            End If
        End Try
        If chkLogMessages.Checked Then
            VSLog("Connected to VIS Macros Server: " & VISMacrosConnection.Connected.ToString)
        End If

        Auxsel1.BackColor = Color.Red : ExecuteLua("ATEMMixerAUXSetInput( 1,2,2 )")
        Aux3Btn1.BackColor = Color.Red : ExecuteLua("ATEMMixerAUXSetInput( 1,3,2 )") ' 
        CurrentAux3 = 1
        ExecuteLua("ATEMMixerMESetPreviewInput( 1,1," & Globals.AtemChannel(1) & " )") 'set preview to 1
        ExecuteLua("ATEMMixerMESetProgramInput( 1,1," & Globals.AtemChannel(4) & " )") 'set program to 4
        ExecuteLua("ATEMMixerDSKSetOnAir( 1, 1, FALSE )") 'turn off overlay
        setactive()
    End Sub

    Private Function SendCamCmd(ByVal cmd As String)
        Dim webClient As New System.Net.WebClient
        Dim url As String, result As String
        If addr = 0 Or (addr > 4 And addr <> 7) Then Return ""
        'If CamCmdPending = True Then Return ""
        If CamIgnore(addr) = True Then Return ""
        url = "http://" & Globals.CamIP(addr) & "/cgi-bin/aw_ptz?cmd=%23" & cmd & "&res=1"
        result = ""
        Try
            'CamCmdPending = True
            result = webClient.DownloadString(url)
        Catch ex As System.Net.WebException
            CamIgnore(addr) = True
            If addr = 7 Then addr = 5 'cam7 is shown as 5 on the panel
            MsgBox("Error sending to camera " & addr & vbCrLf & ex.Message)
        End Try
        'CamCmdPending = False
        Return result
    End Function

    Private Function SendCamCmdNoHash(ByVal cmd As String, ByVal typ As String)
        Dim webClient As New System.Net.WebClient
        Dim url As String, result As String
        If addr = 0 Or (addr > 4 And addr <> 7) Then Return ""
        'If CamCmdPending = True Then Return ""
        If CamIgnore(addr) = True Then Return ""
        If (typ = "") Then typ = "aw_ptz" 'aw_ptz for position, aw_cam for cam settings
        url = "http://" & Globals.CamIP(addr) & "/cgi-bin/" & typ & "?cmd=" & cmd & "&res=1"
        result = ""
        Try
            'CamCmdPending = True
            result = webClient.DownloadString(url)
        Catch ex As System.Net.WebException
            CamIgnore(addr) = True
            If addr = 7 Then addr = 5 'cam7 is shown as 5 on the panel
            MsgBox("Error sending to camera " & addr & vbCrLf & ex.Message)
        End Try
        'CamCmdPending = False
        Return result
    End Function

    Private Function SendCamCmdAddr(ByVal caddr As Integer, ByVal cmd As String)
        Dim webClient As New System.Net.WebClient
        Dim url As String, result As String
        If caddr = 0 Or (caddr > 4 And caddr <> 7) Then Return ""
        'If CamCmdPending = True Then Return ""
        If CamIgnore(caddr) = True Then Return ""
        url = "http://" & Globals.CamIP(caddr) & "/cgi-bin/aw_ptz?cmd=%23" & cmd & "&res=1"
        result = ""
        Try
            'CamCmdPending = True
            result = webClient.DownloadString(url)
        Catch ex As System.Net.WebException
            CamIgnore(caddr) = True
            If caddr = 7 Then caddr = 5 'cam7 is shown as 5 on the panel
            MsgBox("Error sending to camera " & caddr & vbCrLf & ex.Message)
        End Try
        'CamCmdPending = False
        Return result
    End Function
    Private Function SendCamCmdAddrNoHash(ByVal caddr As Integer, ByVal cmd As String, ByVal typ As String)
        Dim webClient As New System.Net.WebClient
        Dim url As String, result As String
        If caddr = 0 Or (caddr > 4 And caddr <> 7) Then Return ""
        'If CamCmdPending = True Then Return ""
        If CamIgnore(caddr) = True Then Return ""
        'cmd = Replace(cmd, ":", "%3A")
        If (typ = "") Then typ = "aw_ptz" 'aw_ptz for position, aw_cam for cam settings
        url = "http://" & Globals.CamIP(caddr) & "/cgi-bin/" & typ & "?cmd=" & cmd & "&res=1"
        result = ""
        Try
            'CamCmdPending = True
            result = webClient.DownloadString(url)
        Catch ex As System.Net.WebException
            CamIgnore(caddr) = True
            If caddr = 7 Then caddr = 5 'cam7 is shown as 5 on the panel
            MsgBox("Error sending to camera " & caddr & vbCrLf & ex.Message)
        End Try
        'CamCmdPending = False
        Return result
    End Function

    Function SendCamQuery(ByVal caddr As Integer, ByVal cmd As String)
        Dim webClient As New System.Net.WebClient
        Dim url As String, result As String
        If caddr = 0 Or (caddr > 4 And caddr <> 7) Then Return ""
        'If CamCmdPending = True Then Return ""
        If CamIgnore(caddr) = True Then Return ""
        url = "http://" & Globals.CamIP(caddr) & "/cgi-bin/" & cmd
        result = ""
        Try
            'CamCmdPending = True
            result = webClient.DownloadString(url)
        Catch ex As System.Net.WebException
            CamIgnore(caddr) = True
            If caddr = 7 Then caddr = 5 'cam7 is shown as 5 on the panel
            MsgBox("Error sending to camera " & caddr & vbCrLf & ex.Message)
        End Try
        'CamCmdPending = False
        Return result
    End Function

    Public Sub SendCamQueryNoResponse(ByVal caddr As Integer, ByVal cmd As String)
        Dim webClient As New System.Net.WebClient
        Dim url As String
        url = "http://" & Globals.CamIP(caddr) & "/cgi-bin/" & cmd
        webClient.DownloadString(url)
    End Sub


    Private Sub MainForm_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        If _serialPort.IsOpen Then _serialPort.Close()
        If SerialPort1.IsOpen Then SerialPort1.Close()
        'BackgroundWorker1.CancelAsync()
    End Sub

    Private Sub MainForm_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        Debug.Print(e.KeyCode)
        Dim op As String

        'If ((Not TextLeaderName.Focus) And (Not TextPreacherName.Focus)) Then
        If False Then 'temporarily disable hotkeys

            If e.KeyCode = 49 Then BtnCam1_Click(BtnCam1, Nothing)
            If e.KeyCode = 50 Then BtnCam1_Click(BtnCam2, Nothing)
            If e.KeyCode = 51 Then BtnCam1_Click(BtnCam3, Nothing)
            If e.KeyCode = 52 Then BtnCam1_Click(BtnCam4, Nothing)
            If e.KeyCode = 53 Then BtnInp_Click(BtnInp1, Nothing)
            If e.KeyCode = 54 Then BtnInp_Click(BtnInp2, Nothing)
            If e.KeyCode = 55 Then BtnInp_Click(BtnInp3, Nothing)
            If e.KeyCode = 56 Then BtnInp_Click(BtnInp4, Nothing)
            If e.KeyCode = Keys.Q Then BtnPreset_Click(BtnPreset1, Nothing)
            If e.KeyCode = Keys.W Then BtnPreset_Click(BtnPreset2, Nothing)
            If e.KeyCode = Keys.E Then BtnPreset_Click(BtnPreset3, Nothing)
            If e.KeyCode = Keys.R Then BtnPreset_Click(BtnPreset4, Nothing)
            If e.KeyCode = Keys.A Then BtnPreset_Click(BtnPreset5, Nothing)
            If e.KeyCode = Keys.S Then BtnPreset_Click(BtnPreset6, Nothing)
            If e.KeyCode = Keys.D Then BtnPreset_Click(BtnPreset7, Nothing)
            If e.KeyCode = Keys.F Then BtnPreset_Click(BtnPreset8, Nothing)
            If e.KeyCode = Keys.Z Then BtnPreset_Click(BtnPreset9, Nothing)
            If e.KeyCode = Keys.X Then BtnPreset_Click(BtnPreset10, Nothing)
            If e.KeyCode = Keys.C Then BtnPreset_Click(BtnPreset11, Nothing)
            If e.KeyCode = Keys.V Then BtnPreset_Click(BtnPreset12, Nothing)
            'If e.KeyCode = Keys.C Then CheckBoxCU.Checked = True : BtnPreset_Click(BtnPreset9, Nothing)
            If e.KeyCode = 33 Then TextBox4.Text = Val(TextBox4.Text - 1) : BtnIrisDown_Click(Nothing, Nothing)
            If e.KeyCode = 34 Then TextBox4.Text = Val(TextBox4.Text + 1) : BtnIrisUp_Click(Nothing, Nothing)
            If e.KeyCode = 32 Then BtnTransition_Click(Nothing, Nothing)
            If e.KeyCode = Asc(".") Then BtnCut_Click(Nothing, Nothing)
            If e.KeyCode = 17 Or e.KeyCode = Keys.O Then BtnOverlay_Click(Nothing, Nothing)
            If e.KeyCode = 100 Then 'left cursor (numeric keypad only)
                If e.Modifiers = Keys.Control Then op = "PTS2050" Else op = "PTS4050"
                If kc <> 37 Then SendCamCmd(op)
                kc = 37
                e.Handled = True
            End If
            If e.KeyCode = 102 Then 'right cursor
                If e.Modifiers = Keys.Control Then op = "PTS8050" Else op = "PTS6050"
                If kc <> 39 Then SendCamCmd(op)
                kc = 39
                e.Handled = True
            End If
            If e.KeyCode = 104 Then 'up cursor
                If e.Modifiers = Keys.Control Then op = "PTS5020" Else op = "PTS5040"
                If kc <> 38 Then SendCamCmd(op)
                kc = 38
                e.Handled = True
            End If
            If e.KeyCode = 98 Then 'down cursor
                If e.Modifiers = Keys.Control Then op = "PTS5080" Else op = "PTS5060"
                If kc <> 40 Then SendCamCmd(op)
                kc = 40
                e.Handled = True
            End If
            If e.KeyCode = 109 Then 'pgup
                If e.Modifiers = Keys.Control Then op = "Z10" Else op = "Z40"
                If kc <> 33 Then SendCamCmd(op)
                kc = 33
                e.Handled = True
            End If
            If e.KeyCode = 107 Then 'pgup
                If e.Modifiers = Keys.Control Then op = "Z90" Else op = "Z60"
                If kc <> 34 Then SendCamCmd(op)
                kc = 34
                e.Handled = True
            End If
        End If

    End Sub

    Private Sub MainForm_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyUp
        If kc <> 0 Then
            If (kc = 33) Or kc = 34 Then
                SendCamCmd("Z50") 'was zooming
            Else
                SendCamCmd("PTS5050") 'was panning
            End If
        End If
        kc = 0
    End Sub


    Private Sub MainForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim i As Integer

        If My.Computer.Keyboard.CtrlKeyDown Then 'Or My.Computer.Keyboard.ShiftKeyDown Then
            ctrlkey = True
        Else
            ctrlkey = False
        End If

        If Screen.AllScreens.Count > 1 Then
            Me.Bounds = (From scr In Screen.AllScreens Where Not scr.Primary)(0).WorkingArea 'open on 2nd monitor
            'Windows.Forms.Cursor.Hide()
            Me.Cursor = Cursors.Cross
        Else
            Me.FormBorderStyle = FormBorderStyle.Sizable 'if no 2nd monitor, open sizeable on main monitor
        End If

        OpenWebSocket()

        gain(1) = 4 : gain(2) = 4 : gain(3) = 4 : gain(4) = 4
        wblock(1) = 0 : wblock(2) = 0 : wblock(3) = 0 : wblock(4) = 0
        addr = 2 : liveaddr = 1 : nextpreview = 2

        For i = 1 To 4 'these will be loaded from the camera settings eventually
            CamIris(i) = 0
            CamAgc(i) = 0
            CamShutter(i) = 0
            CamGain(i) = 0
            CamWBRed(i) = 0
            CamWBBlue(i) = 20
            CamFocusManual(i) = 0
        Next i

        'For i = 0 To 128
        'joyconvert(i) = Int((Math.Log(i + 1) / (Math.Log(128))) * 50)
        'Debug.Print(joyconvert(i) & ",")
        'Next

        Globals.TallyMode = GetSetting("Atemswitcher", "Set", "Tally", False)
        Globals.AutoSwap = GetSetting("Atemswitcher", "Set", "Autoswap", True)
        Globals.PresetFilePath = GetSetting("Atemswitcher", "Set", "PresetsPath", Mid(Application.ExecutablePath, 1, InStrRev(Application.ExecutablePath, "\")))
        If Globals.PresetFilePath = "" Then Globals.PresetFilePath = Mid(Application.ExecutablePath, 1, InStrRev(Application.ExecutablePath, "\"))
        Globals.PresetFileName = GetSetting("Atemswitcher", "Set", "PresetsFile", "default.aps")
        If InStr(Globals.PresetFileName, "\") Then Globals.PresetFileName = Mid(Globals.PresetFileName, InStrRev(Globals.PresetFileName, "\") + 1)
        If Globals.PresetFileName = "" Then Globals.PresetFileName = "default.aps"
        Globals.CamInvert(1) = GetSetting("Atemswitcher", "Set", "Caminvert1", False)
        Globals.CamInvert(2) = GetSetting("Atemswitcher", "Set", "Caminvert2", False)
        Globals.CamInvert(3) = GetSetting("Atemswitcher", "Set", "Caminvert3", False)
        Globals.CamInvert(4) = GetSetting("Atemswitcher", "Set", "Caminvert4", False)

        Globals.Cam1Dis = GetSetting("Atemswitcher", "Set", "Cam1Dis", False)
        Globals.Cam2Dis = GetSetting("Atemswitcher", "Set", "Cam2Dis", False)
        Globals.Cam3Dis = GetSetting("Atemswitcher", "Set", "Cam3Dis", False)
        Globals.Cam4Dis = GetSetting("Atemswitcher", "Set", "Cam4Dis", False)
        Globals.Cam5Dis = GetSetting("Atemswitcher", "Set", "Cam5Dis", False)

        Globals.CamIP(1) = (GetSetting("Atemswitcher", "CamIP", "1", "192.168.1.91"))
        Globals.CamIP(2) = (GetSetting("Atemswitcher", "CamIP", "2", "192.168.1.92"))
        Globals.CamIP(3) = (GetSetting("Atemswitcher", "CamIP", "3", "192.168.1.93"))
        Globals.CamIP(4) = (GetSetting("Atemswitcher", "CamIP", "4", "192.168.1.94"))
        Globals.CamIP(7) = (GetSetting("Atemswitcher", "CamIP", "5", "192.168.1.95"))

        Globals.Cliptime(1) = (GetSetting("Atemswitcher", "Cliptime", "1", "60"))
        Globals.Cliptime(2) = (GetSetting("Atemswitcher", "Cliptime", "2", "60"))
        Globals.Cliptime(3) = (GetSetting("Atemswitcher", "Cliptime", "3", "60"))
        Globals.Cliptime(4) = (GetSetting("Atemswitcher", "Cliptime", "4", "60"))

        BtnFast.BackColor = Color.Green
        BtnLiveSlow.BackColor = Color.Green
        BtnAuxLock.BackColor = Color.Red
        SetDefaultPresets()
        ReadPresetFile()

        'Process.Start("C:\atem_vb\VideoMessage\VideoMessage\bin\Debug\videomessage.exe")

    End Sub

    '----------------------Read camera states back from cameras-----------------------------------------------------
    Sub ReadbackCameraStates(ByVal ta As Integer)
        Dim op As String
        SendCamCmdAddrNoHash(ta, "XSF:1", "aw_cam") 'set scene file "MANUAL 1"
        op = SendCamCmdAddrNoHash(ta, "QSD:B1", "aw_cam") 'get WB setting
        CamWBBlue(ta) = Val("&H" & Mid(op, 8))
        op = SendCamCmdAddrNoHash(ta, "QGU", "aw_cam") 'get AGC setting
        CamAgc(ta) = Val("&H" & Mid(op, 5))
        op = SendCamCmdAddrNoHash(ta, "QSD:69", "aw_cam") 'get AGC gain limit setting
        CamGain(ta) = Val("&H" & Mid(op, 8))
        op = SendCamCmdAddrNoHash(ta, "QSD:48", "aw_cam") 'get "contrast" setting
        CamShutter(ta) = (Val(Mid(op, 8)) - 32) * 20 / 64
        op = SendCamCmdAddrNoHash(ta, "QRV", "aw_cam") 'get iris setting
        CamIris(ta) = Val("&H" & Mid(op, 5))
        op = SendCamCmdAddrNoHash(ta, "QRS", "aw_cam") 'get iris auto/man
        If Val("&H" & Mid(op, 5)) = 1 Then CamIris(ta) = 9999 'flag auto mode
        op = SendCamCmdAddr(ta, "D1") 'get focus auto/man
        If Val("&H" & Mid(op, 3)) = 1 Then CamFocusManual(ta) = 0 Else CamFocusManual(ta) = 1 'flag auto mode if return=1
    End Sub

    '----------------------Read presets back from file-----------------------------------------------------
    Sub ReadPresetFile()
        Dim TextFileReader As Microsoft.VisualBasic.FileIO.TextFieldParser
        Try
            TextFileReader = My.Computer.FileSystem.OpenTextFieldParser(Globals.PresetFilePath & Globals.PresetFileName)
        Catch
            WritePresetFile()
            TextFileReader = My.Computer.FileSystem.OpenTextFieldParser(Globals.PresetFilePath & Globals.PresetFileName)
        End Try
        TextFileReader.TextFieldType = FileIO.FieldType.Delimited
        TextFileReader.SetDelimiters(",")

        Dim i As Integer
        Dim CurrentRow As String()
        Dim a As String

        i = 0
        While Not TextFileReader.EndOfData
            Try
                CurrentRow = TextFileReader.ReadFields()
                If Not CurrentRow Is Nothing Then
                    PresetCaption(i) = CurrentRow(0).ToString
                    PresetXPos(i) = CurrentRow(1).ToString
                    PresetYPos(i) = CurrentRow(2).ToString
                    PresetZPos(i) = CurrentRow(3).ToString
                    If CurrentRow.GetUpperBound(0) > 3 Then 'check if we have these params or not
                        PresetContent(i) = Convert.ToInt32(CurrentRow(4).ToString)
                        PresetSize(i) = Convert.ToInt32(CurrentRow(5).ToString)
                        a = CurrentRow(6).ToString
                        If (a = "0" Or a = "False") Then PresetAuto(i) = False Else PresetAuto(i) = True
                        'PresetAuto(i) = Convert.ToBoolean(CurrentRow(6).ToString)
                    Else
                        PresetContent(i) = 0
                        PresetSize(i) = 0
                        PresetAuto(i) = False
                    End If
                    If CurrentRow.GetUpperBound(0) > 6 Then 'check if we have these params or not
                        PresetFocus(i) = Convert.ToInt32(CurrentRow(7).ToString)
                        a = CurrentRow(8).ToString
                        If (a = "0" Or a = "False") Then PresetFocusAuto(i) = False Else PresetFocusAuto(i) = True
                    Else
                        PresetFocus(i) = 0
                        PresetFocusAuto(i) = True
                    End If
                    i = i + 1
                End If
            Catch ex As  _
            Microsoft.VisualBasic.FileIO.MalformedLineException
                MsgBox("Line " & ex.Message & _
                "is not valid and will be skipped.")
            End Try
        End While
        TextFileReader.Dispose()
        'read preset names for cam7. these are stored in the registry as the presets are fixed in the camera
        For i = 0 To 15
            PresetCaption(16 * 6 + i) = GetSetting("Atemswitcher", "Preset7", i, i + 1)
        Next
        UpdatePresets()
    End Sub

    Sub WritePresetFile()
        Dim file As System.IO.StreamWriter
        Dim i As Integer, j As Integer
        Dim ln As String
        Try
            file = My.Computer.FileSystem.OpenTextFileWriter(Globals.PresetFilePath & Globals.PresetFileName, False) 'false=no append
        Catch
            MsgBox("Could not write preset file " & Globals.PresetFilePath & Globals.PresetFileName)
            Return
        End Try

        For j = 0 To 3
            For i = 0 To 15
                ln = PresetCaption(i + j * 16) & "," & PresetXPos(i + j * 16) & "," & PresetYPos(i + j * 16) & "," & PresetZPos(i + j * 16)
                ln = ln & "," & PresetContent(i + j * 16) & "," & PresetSize(i + j * 16) & "," & PresetAuto(i + j * 16)
                ln = ln & "," & PresetFocus(i + j * 16) & "," & PresetFocusAuto(i + j * 16)
                file.WriteLine(ln)
            Next i
        Next j
        file.Close()

        'also save cam7 legends to registry
        For i = 0 To 15
            SaveSetting("Atemswitcher", "Preset7", i, PresetCaption(6 * 16 + i))
        Next
    End Sub

    Sub SetDefaultPresets()
        Dim i As Integer, j As Integer
        For j = 0 To 3
            For i = 0 To 15
                PresetCaption(i + j * 16) = i + 1
                PresetXPos(i + j * 16) = "8000"
                PresetYPos(i + j * 16) = "8000"
                PresetZPos(i + j * 16) = "555"
            Next i
        Next j
    End Sub

    Public Sub ComportOpen()
        'open com port for camera comms
        'If _serialPort.IsOpen Then _serialPort.Close()
        '_serialPort.PortName = GetSetting("Atemswitcher", "Comm", "1", "COM1")
        '_serialPort.BaudRate = 9600
        '_serialPort.Parity = Parity.None
        '_serialPort.DataBits = 8
        '_serialPort.StopBits = StopBits.One
        '_serialPort.Handshake = Handshake.None
        '_serialPort.DiscardNull = False

        ' Set the read/write timeouts
        '_serialPort.ReadTimeout = 500
        '_serialPort.WriteTimeout = 500
        'camconnect = True
        'Try
        ' _serialPort.Open()
        'Catch
        'MsgBox("The camera com port " & _serialPort.PortName & " cannot be opened.")
        'camconnect = False
        'End Try

        'open com port for controller comms
        If SerialPort1.IsOpen Then SerialPort1.Close()
        SerialPort1.BaudRate = 9600
        SerialPort1.PortName = GetSetting("Atemswitcher", "Comm", "2", "COM2")
        Try
            SerialPort1.Open()
        Catch
            MsgBox("The controller com port " & SerialPort1.PortName & " cannot be opened.")
        End Try
    End Sub
    Sub EditPresetDetails(ByVal index As Integer)
        'op = InputBox("Enter legend", , PresetCaption((addr - 1) * 16 + index - 1))
        Globals.EditPresetIndex = index
        Globals.EditPresetCaption = PresetCaption((addr - 1) * 16 + index - 1)
        Globals.EditPresetContent = PresetContent((addr - 1) * 16 + index - 1)
        Globals.EditPresetSize = PresetSize((addr - 1) * 16 + index - 1)
        Globals.EditPresetAuto = PresetAuto((addr - 1) * 16 + index - 1)

        Form3.ShowDialog()  'needs to be modal
        'PresetCaption((addr - 1) * 16 + index - 1) = op
        PresetCaption((addr - 1) * 16 + index - 1) = Globals.EditPresetCaption
        PresetContent((addr - 1) * 16 + index - 1) = Globals.EditPresetContent
        PresetSize((addr - 1) * 16 + index - 1) = Globals.EditPresetSize
        PresetAuto((addr - 1) * 16 + index - 1) = Globals.EditPresetAuto

    End Sub
    '--------------------------------------------------------------------------------------------------------------
    'Click on a preset button
    '--------------------------------------------------------------------------------------------------------------
    Private Sub BtnPreset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPreset1.Click, BtnPreset2.Click, BtnPreset3.Click, BtnPreset4.Click, BtnPreset5.Click, BtnPreset6.Click, BtnPreset7.Click, BtnPreset8.Click, BtnPreset9.Click, BtnPreset10.Click, BtnPreset11.Click, BtnPreset12.Click, BtnPreset13.Click, BtnPreset14.Click, BtnPreset15.Click, BtnPreset16.Click
        Dim op As String
        Dim cu As String
        Dim index As Integer
        Dim ad As Integer
        Dim cz As Integer, pz As Integer, cx As Integer, px As Integer, cy As Integer, py As Integer, tdiff1 As Integer, tdiff2 As Integer
        Dim zt As Integer
        Dim zsec
        Dim xt, yt
        Dim i As Integer
        index = Val(Mid(sender.name, 10))

        If MovePresetMode <> 0 Then 'we are moving a preset
            If MovePresetMode = 1 Then 'select preset to move
                MovePresetFrom = index
                MovePresetMode = 2
                CType(Me.Controls.Find("BtnPreset" & MovePresetFrom, False)(0), AtemController.MyButton).BackColor = Color.Orange
                For i = 1 To 16
                    If i <> index Then CType(Me.Controls.Find("BtnPreset" & i, False)(0), AtemController.MyButton).ForeColor = Color.Red
                Next
                Exit Sub
            End If
            If MovePresetMode = 2 Then 'select where its going
                If PresetLive = False Then ad = addr Else ad = liveaddr
                'swap caption
                Dim tmp
                tmp = PresetCaption((ad - 1) * 16 + (MovePresetFrom - 1)) : PresetCaption((ad - 1) * 16 + (MovePresetFrom - 1)) = PresetCaption((ad - 1) * 16 + (index - 1)) : PresetCaption((ad - 1) * 16 + (index - 1)) = tmp
                tmp = PresetContent((ad - 1) * 16 + MovePresetFrom - 1) : PresetContent((ad - 1) * 16 + MovePresetFrom - 1) = PresetContent((ad - 1) * 16 + index - 1) : PresetContent((ad - 1) * 16 + index - 1) = tmp
                tmp = PresetSize((ad - 1) * 16 + MovePresetFrom - 1) : PresetSize((ad - 1) * 16 + MovePresetFrom - 1) = PresetSize((ad - 1) * 16 + index - 1) : PresetSize((ad - 1) * 16 + index - 1) = tmp
                tmp = PresetAuto((ad - 1) * 16 + MovePresetFrom - 1) : PresetAuto((ad - 1) * 16 + MovePresetFrom - 1) = PresetAuto((ad - 1) * 16 + index - 1) : PresetAuto((ad - 1) * 16 + index - 1) = tmp
                'swap positions
                tmp = PresetZPos((ad - 1) * 16 + MovePresetFrom - 1) : PresetZPos((ad - 1) * 16 + MovePresetFrom - 1) = PresetZPos((ad - 1) * 16 + index - 1) : PresetZPos((ad - 1) * 16 + index - 1) = tmp
                tmp = PresetXPos((ad - 1) * 16 + MovePresetFrom - 1) : PresetXPos((ad - 1) * 16 + MovePresetFrom - 1) = PresetXPos((ad - 1) * 16 + index - 1) : PresetXPos((ad - 1) * 16 + index - 1) = tmp
                tmp = PresetYPos((ad - 1) * 16 + MovePresetFrom - 1) : PresetYPos((ad - 1) * 16 + MovePresetFrom - 1) = PresetYPos((ad - 1) * 16 + index - 1) : PresetYPos((ad - 1) * 16 + index - 1) = tmp
                tmp = PresetFocusAuto((ad - 1) * 16 + MovePresetFrom - 1) : PresetFocusAuto((ad - 1) * 16 + MovePresetFrom - 1) = PresetFocusAuto((ad - 1) * 16 + index - 1) : PresetFocusAuto((ad - 1) * 16 + index - 1) = tmp
                WritePresetFile()
                MovePresetMode = 0
                BtnMovePreset.ForeColor = Color.Black
                For i = 1 To 16
                    CType(Me.Controls.Find("BtnPreset" & i, False)(0), AtemController.MyButton).ForeColor = Color.Black
                Next
                UpdatePresets()
                Exit Sub
            End If

        End If

        If savemode = False Then
            If legendmode = False Then
                'If AutoSongPreload = True And AutoPreset = 0 Then 'if in autosongmode and it has picked a preload, but user is picking a different preset, cancel the preload
                'If AutoPreset = 1 Then 'if last shot was autoselected, then disable transitions if user picks another preset
                'BtnPreload.BackColor = Color.White
                'PreloadPreset = 0
                'cdir = 0
                'SetLiveMoveIndicators()
                'UpdatePresets()
                'AutoSongPreload = False
                'End If
                AutoPreset = 0
                If PreloadPreset = 0 Then
                    '---Recall a preset either live or preview
                    If (addr = 7) Then
                        If index < 11 Then
                            SendCamCmd("R0" & index - 1) 'recall preset on camera for cam7
                            presetstate(addr) = 2 ^ (index - 1)
                            UpdatePresets() 'show presets for new cam
                        End If
                    End If
                    If ((PresetLive = False) And (addr < 5)) Or ((PresetLive = True) And (liveaddr < 5)) Then 'recall preset
                        'SendCamCmd("UPVS250")
                        If (addr = liveaddr) Or PresetLive = True Then 'live preset recall. attempt to go there slowly
                            ad = liveaddr
                            cu = SendCamCmdAddr(ad, "GZ") 'get current zoom pos
                            cz = Val("&H" & Mid(cu, 3)) 'the current zoom pos in decimal
                            pz = Val("&H" & PresetZPos((ad - 1) * 16 + index - 1)) 'the new zoom pos in decimal
                            'If cz < pz Then SendCamCmdAddr(ad, "Z51") Else SendCamCmdAddr(ad, "Z49")
                            If cz < pz Then PzDir = 1 Else PzDir = -1
                            zt = (cz - pz) '/ (&HFFF - &H555) 'calc zoom time using the time for full range zoom
                            If (zt < 0) Then zt = -zt
                            'zt = Int(zt * 50)
                            If zt = 0 And cz <> pz Then zt = 1
                            'If (zt > 20) Then zt = 20
                            If LiveMoveSpeed = 0 Then
                                zsec = ZTime(0) * zt / 1000 'slow - zoom speed 1
                                zt = 1
                            Else
                                zsec = ZTime(9) * zt / 1000 'fast - zoom speed 9
                                zt = 9
                            End If
                            If PzDir = -1 And zt <> 0 Then zt = 50 - zt Else zt = 50 + zt
                            SendCamCmdAddr(ad, "Z" & Format("00", zt))
                            PzAddr = ad
                            PendingZoom = pz 'the timer interrupt will end this when we get there

                            op = SendCamCmdAddr(ad, "APC") 'get current pt position
                            op = Mid(op, 4)
                            cx = Convert.ToInt32(Mid(op, 1, 4), 16)
                            px = Convert.ToInt32(PresetXPos((ad - 1) * 16 + index - 1), 16)
                            xt = (cx - px)  'distance for the pan movement
                            If xt < 0 Then xt = -xt
                            xt = xt / 1000
                            cy = Convert.ToInt32(Mid(op, 5, 4), 16)
                            py = Convert.ToInt32(PresetYPos((ad - 1) * 16 + index - 1), 16)
                            yt = (cy - py)   'distance for the tilt movement
                            If yt < 0 Then yt = -yt
                            yt = yt / 1000
                            mLog.Text = "x:" & cx - px & " xt:" & xt & " y:" & cy - py & " yt:" & yt & vbCrLf
                            mLog.Text = mLog.Text & "z:" & cz - pz & " zt:" & zsec & vbCrLf


                            'here we need to do a calc to see what is going to reach the end first
                            'probably speeding up the p/t so it ends with the zoom will be sufficient
                            'also check if the cam adjusts the p/t speeds so they both finish at the same time

                            'cu = "030" 'preset move speed - slow if on live as well
                            'cu = String.Format("{0:X2}", Int(30 * yt))
                            'cu = cu & "0"
                            'SendCamCmdAddr(ad, "APS" & PresetXPos((ad - 1) * 16 + index - 1) & PresetYPos((ad - 1) * 16 + index - 1) & cu) 'last 3 set speed 1D=max 0-1-2 = slow med fast
                            If cx < px Then PpDir = 1 Else PpDir = -1
                            If cy < py Then PtDir = 1 Else PtDir = -1
                            'xt = Int(xt * 200) : yt = Int(yt * 200)
                            If (zsec < 8) Then zsec = 8 'try different speeds till the move is just faster than the zoom move
                            For i = 0 To 14
                                If (PTTime(i) * xt) < zsec Then Exit For
                            Next
                            If i <> 0 Then 'check which is nearer, the one we chose or the previous one
                                tdiff1 = Math.Abs((PTTime(i) * xt) - zsec)
                                tdiff2 = Math.Abs((PTTime(i - 1) * xt) - zsec)
                                If tdiff2 < tdiff1 Then i = i - 1
                            End If
                            xt = i + 1
                            For i = 0 To 14
                                If (PTTime(i) * yt) < zsec Then Exit For
                            Next
                            If i <> 0 Then 'check which is nearer, the one we chose or the previous one
                                tdiff1 = Math.Abs((PTTime(i) * yt) - zsec)
                                tdiff2 = Math.Abs((PTTime(i - 1) * yt) - zsec)
                                If tdiff2 < tdiff1 Then i = i - 1
                            End If
                            yt = i + 1
                            'If xt > 15 Then xt = 15
                            'xt = 1
                            'If yt > 15 Then yt = 15
                            'yt = 5
                            If xt = 0 And cx <> px Then xt = 1
                            If yt = 0 And cy <> py Then yt = 1
                            If PpDir = 1 Then xt = 50 - xt Else xt = 50 + xt
                            If PtDir = 1 Then yt = 50 - yt Else yt = 50 + yt
                            mLog.Text = mLog.Text & "Speeds x:" & xt & " y:" & yt & " z:" & zt
                            SendCamCmdAddr(ad, "PTS" & Format("00", xt) & Format("00", yt))
                            If xt <> 0 Then PendingPan = px
                            If yt <> 0 Then PendingTilt = py
                            presetstate(ad) = 2 ^ (index - 1)
                            BtnLive.BackColor = Color.White : PresetLive = False
                        Else
                            cu = "101" 'preview cam move - move at fast speed (not max speed as that is quite noisy)
                            SendCamCmd("AXZ" & PresetZPos((addr - 1) * 16 + index - 1))
                            SendCamCmd("APS" & PresetXPos((addr - 1) * 16 + index - 1) & PresetYPos((addr - 1) * 16 + index - 1) & cu) 'last 3 set speed 1D=max 0-1-2 = slow med fast
                            presetstate(addr) = 2 ^ (index - 1)
                            PrevAutoSongShot = index
                        End If
                        UpdatePresets() 'show presets for new cam

                    End If
                Else 'preload preset mode
                    If (addr <= 4) Then
                        'If (PreloadPreset <> 0) Then
                        AutoSongPreload = False
                        PreloadPreset = index
                        UpdatePresets()
                        cdir = 0
                        SetLiveMoveIndicators()
                        'End If
                    End If
                End If
            Else 'edit preset details
                cu = "0"
                Button7.ForeColor = Color.Black
                EditPresetDetails(index)
                legendmode = False
                WritePresetFile()
                setactive()
            End If
        Else
            'save current preset position
            If (addr <= 4) Then
                op = SendCamCmd("GZ") 'get current zoom position
                op = Mid(op, 3)
                PresetZPos((addr - 1) * 16 + index - 1) = op
                op = SendCamCmd("APC") 'get current pt position
                op = Mid(op, 4)
                PresetXPos((addr - 1) * 16 + index - 1) = Mid(op, 1, 4)
                PresetYPos((addr - 1) * 16 + index - 1) = Mid(op, 5, 4)
                op = SendCamCmd("GF") 'get current focus position
                op = Mid(op, 3)
                'PresetFocus((addr - 1) * 16 + index - 1) = op
                op = SendCamCmd("D1") 'get current autofocus state
                op = Mid(op, 3)
                If (op = "0") Then PresetFocusAuto((addr - 1) * 16 + index - 1) = False Else PresetFocusAuto((addr - 1) * 16 + index - 1) = True
                If PresetCaption((addr - 1) * 16 + index - 1) = Convert.ToString(index) Then 'automatically ask for legend if the legend is just the initial number
                    EditPresetDetails(index)
                End If
                presetstate(addr) = 2 ^ (index - 1)
                setactive()
            ElseIf addr = 7 Then
                If index < 11 Then
                    SendCamCmd("M0" & index - 1) 'store preset on camera for cam7
                    If PresetCaption((addr - 1) * 16 + index - 1) = Convert.ToString(index) Then 'automatically ask for legend if the legend is just the initial number
                        EditPresetDetails(index)
                    End If
                    presetstate(addr) = 2 ^ (index - 1)
                    setactive()
                End If
            End If
            savemode = 0
            WritePresetFile()
            BtnPresetSave.ForeColor = Color.Black

        End If
        'If _serialPort.IsOpen Then _serialPort.Write(buffer, 0, 7)
    End Sub

    Sub ShowCamValues()
        Dim ad As Integer
        If (PTZLive = False) Then ad = addr Else ad = liveaddr
        If CamIris(ad) <> 9999 Then TextBox4.Text = CamIris(ad) Else TextBox4.Text = "Auto"
        If (CamAgc(ad) <= &H38) Then TextBox5.Text = CamAgc(ad) - 8 & "dB" Else TextBox5.Text = "Auto"
        TextBox6.Text = CamShutter(ad)
        TextBox7.Text = CamGain(ad) * 6 & "dB"
        TextBox8.Text = CamWBRed(ad)
        TextBox9.Text = 2400 + CamWBBlue(ad) * 100
        If CamFocusManual(ad) = 0 Then
            BtnFocusAuto.BackColor = Color.Green : BtnFocusLock.BackColor = Color.White
        Else
            BtnFocusAuto.BackColor = Color.White : BtnFocusLock.BackColor = Color.Green
        End If
    End Sub
    Sub setactive()

        BtnCam1.BackColor = Color.White
        BtnCam2.BackColor = Color.White
        BtnCam3.BackColor = Color.White
        BtnCam4.BackColor = Color.White
        BtnInp1.BackColor = Color.White
        BtnInp2.BackColor = Color.White
        BtnInp3.BackColor = Color.White
        BtnInp4.BackColor = Color.White
        BtnInp5.BackColor = Color.White
        If addr = liveaddr Then
            If addr = 1 Then BtnCam1.BackColor = Color.Yellow
            If addr = 2 Then BtnCam2.BackColor = Color.Yellow
            If addr = 3 Then BtnCam3.BackColor = Color.Yellow
            If addr = 4 Then BtnCam4.BackColor = Color.Yellow
            If addr = 5 Then BtnInp1.BackColor = Color.Yellow
            If addr = 6 Then BtnInp2.BackColor = Color.Yellow
            If addr = 7 Then BtnInp3.BackColor = Color.Yellow
            If addr = 8 Then BtnInp4.BackColor = Color.Yellow
            If addr = 9 Then BtnInp5.BackColor = Color.Yellow
        Else
            If addr = 1 Then BtnCam1.BackColor = Color.Green
            If addr = 2 Then BtnCam2.BackColor = Color.Green
            If addr = 3 Then BtnCam3.BackColor = Color.Green
            If addr = 4 Then BtnCam4.BackColor = Color.Green
            If addr = 5 Then BtnInp1.BackColor = Color.Green
            If addr = 6 Then BtnInp2.BackColor = Color.Green
            If addr = 7 Then BtnInp3.BackColor = Color.Green
            If addr = 8 Then BtnInp4.BackColor = Color.Green
            If addr = 9 Then BtnInp5.BackColor = Color.Green
            If liveaddr = 1 Then BtnCam1.BackColor = Color.Red
            If liveaddr = 2 Then BtnCam2.BackColor = Color.Red
            If liveaddr = 3 Then BtnCam3.BackColor = Color.Red
            If liveaddr = 4 Then BtnCam4.BackColor = Color.Red
            If liveaddr = 5 Then BtnInp1.BackColor = Color.Red
            If liveaddr = 6 Then BtnInp2.BackColor = Color.Red
            If liveaddr = 7 Then BtnInp3.BackColor = Color.Red
            If liveaddr = 8 Then BtnInp4.BackColor = Color.Red
            If liveaddr = 9 Then BtnInp5.BackColor = Color.Red
        End If
        'tally
        If Globals.TallyMode Then
            SendCamCmdAddr(nextpreview, "DA0")
            SendCamCmdAddr(liveaddr, "DA1")
        End If
        'atem
        'If addr = 1 Then ExecuteLua("ATEMMixerMESetPreviewInput( 1,1," & Globals.AtemChannel(1) & " )") ' SDI1
        'If addr = 2 Then ExecuteLua("ATEMMixerMESetPreviewInput( 1,1," & Globals.AtemChannel(2) & " )") ' SDI2
        'If addr = 3 Then ExecuteLua("ATEMMixerMESetPreviewInput( 1,1," & Globals.AtemChannel(3) & " )") ' SDI3
        'If addr = 4 Then ExecuteLua("ATEMMixerMESetPreviewInput( 1,1," & Globals.AtemChannel(4) & " )") ' SDI4
        'obs
        If transitionwait = 0 Then
            If addr = 1 Then websocket.Send("{""request-type"":""SetPreviewScene"",""scene-name"":""Cam1"",""message-id"":""TEST1""}")
            If addr = 2 Then websocket.Send("{""request-type"":""SetPreviewScene"",""scene-name"":""Cam2"",""message-id"":""TEST1""}")
            If addr = 3 Then websocket.Send("{""request-type"":""SetPreviewScene"",""scene-name"":""Cam3"",""message-id"":""TEST1""}")
            If addr = 4 Then websocket.Send("{""request-type"":""SetPreviewScene"",""scene-name"":""Cam4"",""message-id"":""TEST1""}")
        End If

        'cam settings
        If (addr <= 4) Or addr = 7 Then
            ShowCamValues()
            'load preset button captions
            UpdatePresets()
        End If

        'disable overlay if words are selected
        If (OverlayWasActive) Then BtnOverlay_Click(BtnOverlay, Nothing) : OverlayWasActive = 0
        'If (liveaddr = 6 And overlayactive) Then BtnOverlay_Click(BtnOverlay, Nothing) : OverlayWasActive = 1

        'turn off live control
        BtnLivePTZ.BackColor = Color.White
        PTZLive = False


    End Sub

    Sub UpdatePresets()
        Dim ad As Integer

        If PresetLive = False Then ad = addr Else ad = liveaddr
        If ad > 4 And ad <> 7 Then 'for non-cam inputs just show 1-16 legends
            BtnPreset1.Text = "1" : BtnPreset1.BackColor = Color.White
            BtnPreset2.Text = "2" : BtnPreset2.BackColor = Color.White
            BtnPreset3.Text = "3" : BtnPreset3.BackColor = Color.White
            BtnPreset4.Text = "4" : BtnPreset4.BackColor = Color.White
            BtnPreset5.Text = "5" : BtnPreset5.BackColor = Color.White
            BtnPreset6.Text = "6" : BtnPreset6.BackColor = Color.White
            BtnPreset7.Text = "7" : BtnPreset7.BackColor = Color.White
            BtnPreset8.Text = "8" : BtnPreset8.BackColor = Color.White
            BtnPreset9.Text = "9" : BtnPreset9.BackColor = Color.White
            BtnPreset10.Text = "10" : BtnPreset10.BackColor = Color.White
            BtnPreset11.Text = "11" : BtnPreset11.BackColor = Color.White
            BtnPreset12.Text = "12" : BtnPreset12.BackColor = Color.White
            BtnPreset13.Text = "13" : BtnPreset13.BackColor = Color.White
            BtnPreset14.Text = "14" : BtnPreset14.BackColor = Color.White
            BtnPreset15.Text = "15" : BtnPreset15.BackColor = Color.White
            BtnPreset16.Text = "16" : BtnPreset16.BackColor = Color.White
            Exit Sub
        End If

        BtnPreset1.Text = PresetCaption((ad - 1) * 16 + 0) : BtnPreset1.BackColor = Color.White
        BtnPreset2.Text = PresetCaption((ad - 1) * 16 + 1) : BtnPreset2.BackColor = Color.White
        BtnPreset3.Text = PresetCaption((ad - 1) * 16 + 2) : BtnPreset3.BackColor = Color.White
        BtnPreset4.Text = PresetCaption((ad - 1) * 16 + 3) : BtnPreset4.BackColor = Color.White
        BtnPreset5.Text = PresetCaption((ad - 1) * 16 + 4) : BtnPreset5.BackColor = Color.White
        BtnPreset6.Text = PresetCaption((ad - 1) * 16 + 5) : BtnPreset6.BackColor = Color.White
        BtnPreset7.Text = PresetCaption((ad - 1) * 16 + 6) : BtnPreset7.BackColor = Color.White
        BtnPreset8.Text = PresetCaption((ad - 1) * 16 + 7) : BtnPreset8.BackColor = Color.White
        BtnPreset9.Text = PresetCaption((ad - 1) * 16 + 8) : BtnPreset9.BackColor = Color.White
        BtnPreset10.Text = PresetCaption((ad - 1) * 16 + 9) : BtnPreset10.BackColor = Color.White
        If addr <> 7 Then
            BtnPreset11.Text = PresetCaption((ad - 1) * 16 + 10) : BtnPreset11.BackColor = Color.White
            BtnPreset12.Text = PresetCaption((ad - 1) * 16 + 11) : BtnPreset12.BackColor = Color.White
            BtnPreset13.Text = PresetCaption((ad - 1) * 16 + 12) : BtnPreset13.BackColor = Color.White
            BtnPreset14.Text = PresetCaption((ad - 1) * 16 + 13) : BtnPreset14.BackColor = Color.White
            BtnPreset15.Text = PresetCaption((ad - 1) * 16 + 14) : BtnPreset15.BackColor = Color.White
            BtnPreset16.Text = PresetCaption((ad - 1) * 16 + 15) : BtnPreset16.BackColor = Color.White
        Else 'these presets not available on he2
            BtnPreset11.Text = "" : BtnPreset11.BackColor = Color.White
            BtnPreset12.Text = "" : BtnPreset12.BackColor = Color.White
            BtnPreset13.Text = "" : BtnPreset13.BackColor = Color.White
            BtnPreset14.Text = "" : BtnPreset14.BackColor = Color.White
            BtnPreset15.Text = "" : BtnPreset15.BackColor = Color.White
            BtnPreset16.Text = "" : BtnPreset16.BackColor = Color.White
        End If


        If ad <= 4 Or ad = 7 Then
            If (presetstate(ad) And 1) <> 0 Then BtnPreset1.BackColor = Color.Green
            If (presetstate(ad) And 2) <> 0 Then BtnPreset2.BackColor = Color.Green
            If (presetstate(ad) And 4) <> 0 Then BtnPreset3.BackColor = Color.Green
            If (presetstate(ad) And 8) <> 0 Then BtnPreset4.BackColor = Color.Green
            If (presetstate(ad) And 16) <> 0 Then BtnPreset5.BackColor = Color.Green
            If (presetstate(ad) And 32) <> 0 Then BtnPreset6.BackColor = Color.Green
            If (presetstate(ad) And 64) <> 0 Then BtnPreset7.BackColor = Color.Green
            If (presetstate(ad) And 128) <> 0 Then BtnPreset8.BackColor = Color.Green
            If (presetstate(ad) And &H100) <> 0 Then BtnPreset9.BackColor = Color.Green
            If (presetstate(ad) And &H200) <> 0 Then BtnPreset10.BackColor = Color.Green
            If (presetstate(ad) And &H400) <> 0 Then BtnPreset11.BackColor = Color.Green
            If (presetstate(ad) And &H800) <> 0 Then BtnPreset12.BackColor = Color.Green
            If (presetstate(ad) And &H1000) <> 0 Then BtnPreset13.BackColor = Color.Green
            If (presetstate(ad) And &H2000) <> 0 Then BtnPreset14.BackColor = Color.Green
            If (presetstate(ad) And &H4000) <> 0 Then BtnPreset15.BackColor = Color.Green
            If (presetstate(ad) And &H8000) <> 0 Then BtnPreset16.BackColor = Color.Green
        End If

        If PreloadPreset > 0 And PreloadPreset < 99 And PresetLive = False Then
            If PreloadPreset = 1 Then BtnPreset1.BackColor = Color.Orange
            If PreloadPreset = 2 Then BtnPreset2.BackColor = Color.Orange
            If PreloadPreset = 3 Then BtnPreset3.BackColor = Color.Orange
            If PreloadPreset = 4 Then BtnPreset4.BackColor = Color.Orange
            If PreloadPreset = 5 Then BtnPreset5.BackColor = Color.Orange
            If PreloadPreset = 6 Then BtnPreset6.BackColor = Color.Orange
            If PreloadPreset = 7 Then BtnPreset7.BackColor = Color.Orange
            If PreloadPreset = 8 Then BtnPreset8.BackColor = Color.Orange
            If PreloadPreset = 9 Then BtnPreset9.BackColor = Color.Orange
            If PreloadPreset = 10 Then BtnPreset10.BackColor = Color.Orange
            If PreloadPreset = 11 Then BtnPreset11.BackColor = Color.Orange
            If PreloadPreset = 12 Then BtnPreset12.BackColor = Color.Orange
            If PreloadPreset = 13 Then BtnPreset13.BackColor = Color.Orange
            If PreloadPreset = 14 Then BtnPreset14.BackColor = Color.Orange
            If PreloadPreset = 15 Then BtnPreset15.BackColor = Color.Orange
            If PreloadPreset = 16 Then BtnPreset16.BackColor = Color.Orange
        End If


        If AutoSongMode = True Or AutoSpeechMode = True Then
            If PresetAuto(16 * (ad - 1) + 0) Then BtnPreset1.AccessibleDescription = "A" Else BtnPreset1.AccessibleDescription = ""
            If PresetAuto(16 * (ad - 1) + 1) Then BtnPreset2.AccessibleDescription = "A" Else BtnPreset2.AccessibleDescription = ""
            If PresetAuto(16 * (ad - 1) + 2) Then BtnPreset3.AccessibleDescription = "A" Else BtnPreset3.AccessibleDescription = ""
            If PresetAuto(16 * (ad - 1) + 3) Then BtnPreset4.AccessibleDescription = "A" Else BtnPreset4.AccessibleDescription = ""
            If PresetAuto(16 * (ad - 1) + 4) Then BtnPreset5.AccessibleDescription = "A" Else BtnPreset5.AccessibleDescription = ""
            If PresetAuto(16 * (ad - 1) + 5) Then BtnPreset6.AccessibleDescription = "A" Else BtnPreset6.AccessibleDescription = ""
            If PresetAuto(16 * (ad - 1) + 6) Then BtnPreset7.AccessibleDescription = "A" Else BtnPreset7.AccessibleDescription = ""
            If PresetAuto(16 * (ad - 1) + 7) Then BtnPreset8.AccessibleDescription = "A" Else BtnPreset8.AccessibleDescription = ""
            If PresetAuto(16 * (ad - 1) + 8) Then BtnPreset9.AccessibleDescription = "A" Else BtnPreset9.AccessibleDescription = ""
            If PresetAuto(16 * (ad - 1) + 9) Then BtnPreset10.AccessibleDescription = "A" Else BtnPreset10.AccessibleDescription = ""
            If PresetAuto(16 * (ad - 1) + 10) Then BtnPreset11.AccessibleDescription = "A" Else BtnPreset11.AccessibleDescription = ""
            If PresetAuto(16 * (ad - 1) + 11) Then BtnPreset12.AccessibleDescription = "A" Else BtnPreset12.AccessibleDescription = ""
            If PresetAuto(16 * (ad - 1) + 12) Then BtnPreset13.AccessibleDescription = "A" Else BtnPreset13.AccessibleDescription = ""
            If PresetAuto(16 * (ad - 1) + 13) Then BtnPreset14.AccessibleDescription = "A" Else BtnPreset14.AccessibleDescription = ""
            If PresetAuto(16 * (ad - 1) + 14) Then BtnPreset15.AccessibleDescription = "A" Else BtnPreset15.AccessibleDescription = ""
            If PresetAuto(16 * (ad - 1) + 15) Then BtnPreset16.AccessibleDescription = "A" Else BtnPreset16.AccessibleDescription = ""
        Else
            BtnPreset1.AccessibleDescription = ""
            BtnPreset2.AccessibleDescription = ""
            BtnPreset3.AccessibleDescription = ""
            BtnPreset4.AccessibleDescription = ""
            BtnPreset5.AccessibleDescription = ""
            BtnPreset6.AccessibleDescription = ""
            BtnPreset7.AccessibleDescription = ""
            BtnPreset8.AccessibleDescription = ""
            BtnPreset9.AccessibleDescription = ""
            BtnPreset10.AccessibleDescription = ""
            BtnPreset11.AccessibleDescription = ""
            BtnPreset12.AccessibleDescription = ""
            BtnPreset13.AccessibleDescription = ""
            BtnPreset14.AccessibleDescription = ""
            BtnPreset15.AccessibleDescription = ""
            BtnPreset16.AccessibleDescription = ""
        End If



    End Sub

    Private Sub BtnCam1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnCam1.Click, BtnCam2.Click, BtnCam3.Click, BtnCam4.Click, BtnInp3.Click
        Dim index As Integer
        index = Val(Mid(sender.name, 7))
        If sender.name = "BtnInp3" Then index = 7

        'If _serialPort.IsOpen Then _serialPort.Write(buffer, 0, 7)

        'cancel any preloads
        PreloadPreset = 0
        cdir = 0
        SetLiveMoveIndicators()
        UpdatePresets()

        addr = index
        BtnLive.BackColor = Color.White : PresetLive = False
        BtnLivePTZ.BackColor = Color.White : PTZLive = False
        setactive()

        If (BtnPip.BackColor = Color.Red) Then
            If PipLive = False Then PipLive = True Else PipLive = False
        End If

    End Sub

    Private Sub BtnCut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnCut.Click
        If CutLockoutTimer > 0 Then Exit Sub
        If addr = 5 Then 'if we are cutting to black - mute audio out
            AudioFade = -1 ': AudioLevel = 0
        End If
        If liveaddr = 5 And addr <> 8 Then 'cutting away from black - restore audio
            AudioFade = 1
        End If
        If liveaddr = 8 Then MediaPlayerWasActive = True 'if cutting to mediaplayer then remember we were on it
        nextpreview = liveaddr
        'If addr <> nextpreview Then addr = nextpreview
        liveaddr = addr
        If (ObsSourceName(addr) <> "") Then
            If (overlayactive) Then websocket.Send("{""request-type"":""SetSceneItemRender"",""scene-name"":""" & ObsSourceName(addr) & """,""source"":""OpenLP"",""render"":true,""message-id"":""TEST1""}")
            If (mediaoverlayactive) Then websocket.Send("{""request-type"":""SetSceneItemRender"",""scene-name"":""" & ObsSourceName(addr) & """,""source"":""Caption"",""render"":true,""message-id"":""TEST1""}")
        End If
        StartLiveMove()
        'ExecuteLua("ATEMMixerMECut( 1,1 )")
        websocket.Send("{""request-type"":""TransitionToProgram"",""with-transition"": { ""name"":""Cut"" } ,""message-id"":""TEST1""}")
        'If Globals.AutoSwap Then addr = nextpreview
        If Globals.AutoSwap Then transitionwait = 5

        If (ObsSourceName(nextpreview) <> "") Then
            If (overlayactive) Then websocket.Send("{""request-type"":""SetSceneItemRender"",""scene-name"":""" & ObsSourceName(nextpreview) & """,""source"":""OpenLP"",""render"":false,""message-id"":""TEST1""}")
            If (mediaoverlayactive) Then websocket.Send("{""request-type"":""SetSceneItemRender"",""scene-name"":""" & ObsSourceName(nextpreview) & """,""source"":""Caption"",""render"":false,""message-id"":""TEST1""}")
        End If

        setactive()
        If AutoSongMode = True Or AutoSpeechMode = True Then AutoWait = 6
        CutLockoutTimer = 8
        DelayStop = 8
        AutoPreset = 0
        If (BtnPip.BackColor = Color.Red) Then
            If PipLive = False Then PipLive = True Else PipLive = False
        End If

    End Sub

    Private Sub BtnTransition_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnTransition.Click
        If CutLockoutTimer > 0 Then Exit Sub
        If addr = 5 Then 'if we are cutting to black - mute audio out
            AudioFade = -1 ': AudioLevel = 0
        End If
        If liveaddr = 5 And addr <> 8 Then 'cutting away from black - restore audio
            AudioFade = 1
        End If
        If liveaddr = 8 Then MediaPlayerWasActive = True 'if cutting to mediaplayer then remember we were on it
        nextpreview = liveaddr
        liveaddr = addr
        If (ObsSourceName(addr) <> "") Then
            If (overlayactive) Then websocket.Send("{""request-type"":""SetSceneItemRender"",""scene-name"":""" & ObsSourceName(addr) & """,""source"":""OpenLP"",""render"":true,""message-id"":""TEST1""}")
            If (mediaoverlayactive) Then websocket.Send("{""request-type"":""SetSceneItemRender"",""scene-name"":""" & ObsSourceName(addr) & """,""source"":""Caption"",""render"":true,""message-id"":""TEST1""}")
        End If
        StartLiveMove()
        'ExecuteLua("ATEMMixerMEAutoTransition( 1,1 )")
        Dim ft As Double
        Double.TryParse(TextBox1.Text, ft)
        websocket.Send("{""request-type"":""TransitionToProgram"",""with-transition"": { ""name"":""Fade"", ""duration"":" & ft * 800 & " } ,""message-id"":""TEST1""}")
        If Globals.AutoSwap Then transitionwait = 5 + Val(TextBox1.Text) * 10

        If (ObsSourceName(nextpreview) <> "") Then
            If (overlayactive) Then websocket.Send("{""request-type"":""SetSceneItemRender"",""scene-name"":""" & ObsSourceName(nextpreview) & """,""source"":""OpenLP"",""render"":false,""message-id"":""TEST1""}")
            If (mediaoverlayactive) Then websocket.Send("{""request-type"":""SetSceneItemRender"",""scene-name"":""" & ObsSourceName(nextpreview) & """,""source"":""Caption"",""render"":false,""message-id"":""TEST1""}")
        End If

        setactive()
        If AutoSongMode = True Or AutoSpeechMode = True Then AutoWait = transitionwait + 2
        CutLockoutTimer = transitionwait
        DelayStop = transitionwait
        AutoPreset = 0
    End Sub

    Private Sub BtnPresetSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPresetSave.Click
        If savemode = False Then
            BtnPresetSave.ForeColor = Color.Red
            savemode = True
        Else
            BtnPresetSave.ForeColor = Color.Black
            savemode = False
        End If
    End Sub

    Private Sub BtnMovePreset_Click(sender As Object, e As EventArgs) Handles BtnMovePreset.Click
        If (MovePresetMode = 0) Then
            MovePresetMode = 1
            BtnMovePreset.ForeColor = Color.Red
        Else
            MovePresetMode = 0
            BtnMovePreset.ForeColor = Color.Black
        End If
    End Sub

    Private Sub BtnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnStop.Click
        SendCamCmdAddr(1, "PTS5050")
        SendCamCmdAddr(1, "Z50")
        SendCamCmdAddr(2, "PTS5050")
        SendCamCmdAddr(2, "Z50")
        SendCamCmdAddr(3, "PTS5050")
        SendCamCmdAddr(3, "Z50")
        SendCamCmdAddr(4, "PTS5050")
        SendCamCmdAddr(4, "Z50")
        PendingZoom = 0 : PendingPan = 0 : PendingTilt = 0
    End Sub


    Private Sub BtnSlowIn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSlowIn.Click
        If PresetLive = False Then
            cdir = cdir Xor &H20
            If (cdir And &H20) Then cdir = cdir And Not &H40 'cancel zoom out if zoom in set
            SetLiveMoveIndicators()
            If cdir <> 0 Then
                BtnPreload.BackColor = Color.Orange : PreloadPreset = 99
            Else
                BtnPreload.BackColor = Color.White : PreloadPreset = 0
            End If
            'PreloadPreset = 0 'if we set fixed zoom, cancel preset preload
            UpdatePresets()
        Else
            If LiveMoveSpeed = 0 Then
                SendCamCmdAddr(liveaddr, "Z51")
            Else
                SendCamCmdAddr(liveaddr, "Z60")
            End If
            BtnLive.BackColor = Color.White
            PresetLive = False
        End If

    End Sub

    Private Sub BtnSlowOut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSlowOut.Click
        If PresetLive = False Then
            cdir = cdir Xor &H40
            If (cdir And &H40) Then cdir = cdir And Not &H20 'cancel zoom in if zoom out set
            SetLiveMoveIndicators()
            If cdir <> 0 Then
                BtnPreload.BackColor = Color.Orange : PreloadPreset = 99
            Else
                BtnPreload.BackColor = Color.White : PreloadPreset = 0
            End If
            'PreloadPreset = 0 'if we set fixed zoom, cancel preset preload
            UpdatePresets()
        Else
            If LiveMoveSpeed = 0 Then
                SendCamCmdAddr(liveaddr, "Z49")
            Else
                SendCamCmdAddr(liveaddr, "Z40")
            End If
            BtnLive.BackColor = Color.White
            PresetLive = False
        End If
    End Sub


    Private Sub TextBox1_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBox1.GotFocus
        Me.KeyPreview = False
    End Sub

    Private Sub TextBox1_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBox1.LostFocus
        ExecuteLua("ATEMMixerMESetTransitionRate( 1,1," & Val(TextBox1.Text) * 25 & " )")
        Me.KeyPreview = True
    End Sub
    Sub SetAux2(ByVal index As Integer)
        Dim aux As String
        aux = "22"
        Auxsel1.BackColor = Color.White
        Auxsel2.BackColor = Color.White
        Auxsel3.BackColor = Color.White
        Auxsel4.BackColor = Color.White
        Auxsel5.BackColor = Color.White
        Auxsel6.BackColor = Color.White
        Auxsel7.BackColor = Color.White
        Auxsel8.BackColor = Color.White
        AuxSel9.BackColor = Color.White
        Auxsel10.BackColor = Color.White
        Auxsel11.BackColor = Color.White
        AuxSel12.BackColor = Color.White
        If index = 1 Then aux = "2" : Auxsel1.BackColor = Color.Red 'words
        If index = 2 Then aux = "4" : Auxsel2.BackColor = Color.Red 'matrix
        If index = 3 Then aux = "26" : Auxsel3.BackColor = Color.Red 'mix (program)
        If index = 4 Then aux = "5" : Auxsel4.BackColor = Color.Red 'local
        If index = 5 Then aux = "6" : Auxsel5.BackColor = Color.Red 'cam1
        If index = 6 Then aux = "7" : Auxsel6.BackColor = Color.Red 'cam2
        If index = 7 Then aux = "8" : Auxsel7.BackColor = Color.Red 'cam3
        If index = 8 Then aux = "9" : Auxsel8.BackColor = Color.Red 'cam4
        If index = 9 Then aux = "1" : AuxSel9.BackColor = Color.Red 'black
        If index = 10 Then aux = "15" : Auxsel10.BackColor = Color.Red 'media1
        If index = 11 Then aux = "3" : Auxsel11.BackColor = Color.Red 'overlay
        If index = 12 Then aux = "10" : AuxSel12.BackColor = Color.Red 'stage (input 9)
        ExecuteLua("ATEMMixerAUXSetInput( 1,2," & aux & " )") ' 
    End Sub
    Private Sub Auxsel1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Auxsel1.Click, Auxsel2.Click, Auxsel3.Click, Auxsel4.Click, Auxsel5.Click, Auxsel6.Click, Auxsel7.Click, Auxsel8.Click, AuxSel9.Click, Auxsel10.Click, Auxsel11.Click, AuxSel12.Click
        If BtnAuxLock.BackColor = Color.Red Then Exit Sub 'must turn off the lock before you can do this
        Dim index As Integer
        index = Val(Mid(sender.name, 7))
        SetAux2(index)
    End Sub

    Sub SetAux3(ByVal index As Integer)
        Dim aux As String
        aux = "22"
        Aux3Btn1.BackColor = Color.White
        Aux3Btn3.BackColor = Color.White
        Aux3Btn2.BackColor = Color.White
        Aux3Btn4.BackColor = Color.White
        Aux3Btn5.BackColor = Color.White
        Aux3Btn6.BackColor = Color.White
        Aux3Btn7.BackColor = Color.White
        Aux3Btn8.BackColor = Color.White
        Aux3Btn9.BackColor = Color.White
        Aux3Btn10.BackColor = Color.White
        Aux3Btn11.BackColor = Color.White
        Aux3Btn12.BackColor = Color.White
        If index = 1 Then aux = "2" : Aux3Btn1.BackColor = Color.Red 'words
        If index = 2 Then aux = "4" : Aux3Btn2.BackColor = Color.Red 'matrix
        If index = 3 Then aux = "26" : Aux3Btn3.BackColor = Color.Red 'mix (program)
        If index = 4 Then aux = "5" : Aux3Btn4.BackColor = Color.Red 'player
        If index = 5 Then aux = "6" : Aux3Btn5.BackColor = Color.Red 'cam1
        If index = 6 Then aux = "7" : Aux3Btn6.BackColor = Color.Red 'cam2
        If index = 7 Then aux = "8" : Aux3Btn7.BackColor = Color.Red 'cam3
        If index = 8 Then aux = "9" : Aux3Btn8.BackColor = Color.Red 'cam4
        If index = 9 Then aux = "1" : Aux3Btn9.BackColor = Color.Red 'black
        If index = 10 Then aux = "15" : Aux3Btn10.BackColor = Color.Red 'media 1
        If index = 11 Then aux = "3" : Aux3Btn11.BackColor = Color.Red 'overlay
        If index = 12 Then aux = "10" : Aux3Btn12.BackColor = Color.Red 'stage (input 9)
        ExecuteLua("ATEMMixerAUXSetInput( 1,3," & aux & " )") ' 
        CurrentAux3 = index
    End Sub

    Private Sub Aux3Btn1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Aux3Btn1.Click, Aux3Btn2.Click, Aux3Btn3.Click, Aux3Btn4.Click, Aux3Btn5.Click, Aux3Btn6.Click, Aux3Btn7.Click, Aux3Btn8.Click, Aux3Btn9.Click, Aux3Btn10.Click, Aux3Btn11.Click, Aux3Btn12.Click
        If BtnAuxLock.BackColor = Color.Red Then Exit Sub 'must turn off the lock before you can do this
        Dim index As Integer
        index = Val(Mid(sender.name, 8))
        SetAux3(index)
    End Sub

    Private Sub TextBox2_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBox2.GotFocus
        Me.KeyPreview = False
    End Sub

    Private Sub TextBox2_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBox2.LostFocus
        ExecuteLua("ATEMMixerDSKSetRate( 1,1," & Val(TextBox1.Text) * 25 & " )")
        Me.KeyPreview = True
    End Sub



    Private Sub BtnInp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnInp1.Click, BtnInp2.Click, BtnInp3.Click, BtnInp4.Click, BtnInp5.Click
        If sender.name = "BtnInp1" Then
            'ExecuteLua("ATEMMixerMESetPreviewInput( 1,1,1 )") ' BLACK
            websocket.Send("{""request-type"":""SetPreviewScene"",""scene-name"":""Black"",""message-id"":""TEST1""}")
            addr = 5
        End If
        If sender.name = "BtnInp2" Then
            'ExecuteLua("ATEMMixerMESetPreviewInput( 1,1,2 )") ' Words (inp1)
            websocket.Send("{""request-type"":""SetPreviewScene"",""scene-name"":""Words"",""message-id"":""TEST1""}")
            addr = 6
        End If
        If sender.name = "BtnInp3" Then
            'ExecuteLua("ATEMMixerMESetPreviewInput( 1,1,4 )") ' Overlay (inp 3)
            websocket.Send("{""request-type"":""SetPreviewScene"",""scene-name"":""Cam5"",""message-id"":""TEST1""}")
            addr = 7
        End If
        If sender.name = "BtnInp4" Then
            'ExecuteLua("ATEMMixerMESetPreviewInput( 1,1," & Globals.AtemChannel(5) & " )") ' Cam 5 (inp 4)
            websocket.Send("{""request-type"":""SetPreviewScene"",""scene-name"":""Mediaplayer1"",""message-id"":""TEST1""}")
            addr = 8
        End If
        If sender.name = "BtnInp5" Then
            'ExecuteLua("ATEMMixerMESetPreviewInput( 1,1,10 )") ' Stage (Inp 9)
            websocket.Send("{""request-type"":""SetPreviewScene"",""scene-name"":""Stage"",""message-id"":""TEST1""}")
            addr = 9
        End If
        setactive()
    End Sub

    Sub SetLiveMoveIndicators()
        If cdir And &H20 Then BtnSlowIn.BackColor = Color.Orange Else BtnSlowIn.BackColor = Color.White
        If cdir And &H40 Then BtnSlowOut.BackColor = Color.Orange Else BtnSlowOut.BackColor = Color.White
        If cdir = 0 And PreloadPreset = 0 Then BtnPreload.BackColor = Color.White
    End Sub

    Sub StartLiveMove()
        Dim ps As String, ts As String
        Dim ncdir As Integer = 0
        If PreloadPreset <> 0 And PreloadPreset < 99 Then
            ts = PreloadPreset
            PreloadPreset = 0
            If ts = 1 Then BtnPreset_Click(BtnPreset1, Nothing)
            If ts = 2 Then BtnPreset_Click(BtnPreset2, Nothing)
            If ts = 3 Then BtnPreset_Click(BtnPreset3, Nothing)
            If ts = 4 Then BtnPreset_Click(BtnPreset4, Nothing)
            If ts = 5 Then BtnPreset_Click(BtnPreset5, Nothing)
            If ts = 6 Then BtnPreset_Click(BtnPreset6, Nothing)
            If ts = 7 Then BtnPreset_Click(BtnPreset7, Nothing)
            If ts = 8 Then BtnPreset_Click(BtnPreset8, Nothing)
            If ts = 9 Then BtnPreset_Click(BtnPreset9, Nothing)
            If ts = 10 Then BtnPreset_Click(BtnPreset10, Nothing)
            If ts = 11 Then BtnPreset_Click(BtnPreset11, Nothing)
            If ts = 12 Then BtnPreset_Click(BtnPreset12, Nothing)
            If ts = 13 Then BtnPreset_Click(BtnPreset13, Nothing)
            If ts = 14 Then BtnPreset_Click(BtnPreset14, Nothing)
            If ts = 15 Then BtnPreset_Click(BtnPreset15, Nothing)
            If ts = 16 Then BtnPreset_Click(BtnPreset16, Nothing)
            BtnPreload.BackColor = Color.White
            Exit Sub
        End If
        If cdir = 0 Then Exit Sub
        'set slow zoom speed
        'If (cdir And &H1E) <> 0 Then
        'If cdir And &H20 Then SendCamCmd("Z60")
        'If cdir And &H40 Then SendCamCmd("Z40")
        'Else
        If LiveMoveSpeed = 0 Then
            If cdir And &H20 Then SendCamCmd("Z51")
            If cdir And &H40 Then SendCamCmd("Z49")
        Else
            If cdir And &H20 Then SendCamCmd("Z60")
            If cdir And &H40 Then SendCamCmd("Z40")
        End If
        'End If
        'If _serialPort.IsOpen Then _serialPort.Write(buffer, 0, 7)
        'Threading.Thread.Sleep(20)
        'output the direction command
        If Globals.CamInvert(addr) Then  'inverted camera
            If (cdir And 2) <> 0 Then ncdir = ncdir Or 4
            If (cdir And 4) <> 0 Then ncdir = ncdir Or 2
            If (cdir And 8) <> 0 Then ncdir = ncdir Or 16
            If (cdir And 16) <> 0 Then ncdir = ncdir Or 8
            cdir = ncdir
        End If
        ps = "50" : ts = "50"
        If (cdir And 2) <> 0 Then ps = "55"
        If (cdir And 4) <> 0 Then ps = "45"
        If (cdir And 8) <> 0 Then ts = "55"
        If (cdir And 16) <> 0 Then ts = "45"
        SendCamCmd("PTS" & ps & ts)
        'If _serialPort.IsOpen Then _serialPort.Write(buffer, 0, 7)
        cdir = 0
        SetLiveMoveIndicators()
        BtnPreload.BackColor = Color.White : PreloadPreset = 0

    End Sub

    Private Sub BtnOverlay_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnOverlay.Click
        'ExecuteLua("ATEMMixerDSKExecuteAutoTransition( 1,1 )") ' dsk 1
        Dim ft As Double
        Double.TryParse(TextBox2.Text, ft)
        'Words overlay. Set preview = current output with overlay in opposite state, start fade
        'At end of fade set preview back to user preview
        If overlayactive = False Then
            overlayactive = True
            If ObsSourceName(liveaddr) <> "" Then
                WebsocketSendAndWait("{""request-type"":""SetPreviewScene"",""scene-name"":""" & ObsSourceName(liveaddr) & """,""message-id"":""TEST1""}")
                WebsocketSendAndWait("{""request-type"":""SetSceneItemRender"",""scene-name"":""" & ObsSourceName(liveaddr) & """,""source"":""OpenLP"",""render"":true,""message-id"":""TEST1""}")
                WebsocketSendAndWait("{""request-type"":""TransitionToProgram"",""with-transition"": { ""name"":""Fade"", ""duration"":" & ft * 800 & " } ,""message-id"":""TEST1""}")
                WebsocketSendAndWait("{""request-type"":""SetPreviewScene"",""scene-name"":""" & ObsSourceName(addr) & """,""message-id"":""TEST1""}")
            End If
        Else
            If ObsSourceName(liveaddr) <> "" Then
                overlayactive = False
                WebsocketSendAndWait("{""request-type"":""SetPreviewScene"",""scene-name"":""" & ObsSourceName(liveaddr) & """,""message-id"":""TEST1""}")
                WebsocketSendAndWait("{""request-type"":""SetSceneItemRender"",""scene-name"":""" & ObsSourceName(liveaddr) & """,""source"":""OpenLP"",""render"":false,""message-id"":""TEST1""}")
                WebsocketSendAndWait("{""request-type"":""TransitionToProgram"",""with-transition"": { ""name"":""Fade"", ""duration"":" & ft * 800 & " } ,""message-id"":""TEST1""}")
                WebsocketSendAndWait("{""request-type"":""SetPreviewScene"",""scene-name"":""" & ObsSourceName(addr) & """,""message-id"":""TEST1""}")
            End If
        End If
        If (overlayactive = True) Then BtnOverlay.BackColor = Color.Red Else BtnOverlay.BackColor = Color.White
    End Sub

    Private Sub BtnMediaOverlay_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnMediaOverlay.Click
        'ExecuteLua("ATEMMixerDSKExecuteAutoTransition( 1,2 )") ' dsk 1
        Dim ft As Double
        Double.TryParse(TextBox2.Text, ft)

        If mediaoverlayactive = False Then
            'media overlay was not previously active
            'swap preview source to current source and add the overlay
            If ObsSourceName(liveaddr) <> "" Then
                WebsocketSendAndWait("{""request-type"":""SetPreviewScene"",""scene-name"":""" & ObsSourceName(liveaddr) & """,""message-id"":""TEST1""}")
                WebsocketSendAndWait("{""request-type"":""SetSceneItemRender"",""scene-name"":""" & ObsSourceName(liveaddr) & """,""source"":""Caption"",""render"":true,""message-id"":""TEST1""}")
                WebsocketSendAndWait("{""request-type"":""TransitionToProgram"",""with-transition"": { ""name"":""Fade"", ""duration"":" & ft * 800 & " } ,""message-id"":""TEST1""}")
                WebsocketSendAndWait("{""request-type"":""SetPreviewScene"",""scene-name"":""" & ObsSourceName(addr) & """,""message-id"":""TEST1""}")
            End If

            ' websocket.Send("{""request-type"":""SetSourceFilterVisibility"",""sourceName"":""Caption"",""filterName"":""Color Correction"",""filterEnabled"":false,""message-id"":""TEST1""}")
            'websocket.Send("{""request-type"":""SetSourceRender"",""source"":""Caption"",""render"":true,""message-id"":""TEST1""}")
            mediaoverlayactive = True
        Else
            If ObsSourceName(liveaddr) <> "" Then
                WebsocketSendAndWait("{""request-type"":""SetPreviewScene"",""scene-name"":""" & ObsSourceName(liveaddr) & """,""message-id"":""TEST1""}")
                WebsocketSendAndWait("{""request-type"":""SetSceneItemRender"",""scene-name"":""" & ObsSourceName(liveaddr) & """,""source"":""Caption"",""render"":false,""message-id"":""TEST1""}")
                WebsocketSendAndWait("{""request-type"":""TransitionToProgram"",""with-transition"": { ""name"":""Fade"", ""duration"":" & ft * 800 & " } ,""message-id"":""TEST1""}")
                WebsocketSendAndWait("{""request-type"":""SetPreviewScene"",""scene-name"":""" & ObsSourceName(addr) & """,""message-id"":""TEST1""}")
            End If

            'websocket.Send("{""request-type"":""SetSourceFilterVisibility"",""sourceName"":""Caption"",""filterName"":""Color Correction"",""filterEnabled"":true,""message-id"":""TEST1""}")
            'websocket.Send("{""request-type"":""SetSourceRender"",""source"":""Caption"",""render"":false,""message-id"":""TEST1""}")
            mediaoverlayactive = False
            End If


            If (mediaoverlayactive = True) Then BtnMediaOverlay.BackColor = Color.Red Else BtnMediaOverlay.BackColor = Color.White
    End Sub

    Private Sub SetCaptionText()
        If mediaindex = 1 Then
            LabelCap1.Text = "Leader *" : LabelCap2.Text = "Preacher" : LabelCap3.Text = "Other"
            WebsocketSendAndWait("{""request-type"":""SetTextGDIPlusProperties"",""source"":""Leader"",""text"":""Leader"",""message-id"":""TEST1""}")
            WebsocketSendAndWait("{""request-type"":""SetTextGDIPlusProperties"",""source"":""Leadername"",""text"":""" & TextLeaderName.Text & """,""message-id"":""TEST1""}")
        End If
        If mediaindex = 2 Then
            LabelCap1.Text = "Leader" : LabelCap2.Text = "Preacher *" : LabelCap3.Text = "Other"
            WebsocketSendAndWait("{""request-type"":""SetTextGDIPlusProperties"",""source"":""Leader"",""text"":""Preacher"",""message-id"":""TEST1""}")
            WebsocketSendAndWait("{""request-type"":""SetTextGDIPlusProperties"",""source"":""Leadername"",""text"":""   " & TextPreacherName.Text & """,""message-id"":""TEST1""}")

        End If
        If mediaindex = 3 Then
            LabelCap1.Text = "Leader" : LabelCap2.Text = "Preacher" : LabelCap3.Text = "Other *"
            WebsocketSendAndWait("{""request-type"":""SetTextGDIPlusProperties"",""source"":""Leader"",""text"":""" & TextCaptionOther.Text & """,""message-id"":""TEST1""}")
            WebsocketSendAndWait("{""request-type"":""SetTextGDIPlusProperties"",""source"":""Leadername"",""text"":"""",""message-id"":""TEST1""}")

        End If
    End Sub

    Private Sub BtnMediaPrev_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnMediaPrev.Click
        If (mediaindex > 1) Then mediaindex = mediaindex - 1
        SetCaptionText()
        'ExecuteLua("ATEMMixerMPSetMediaIndex(1,2," & mediaindex & ")")
    End Sub

    Private Sub BtnMediaNxt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnMediaNxt.Click
        If (mediaindex < 4) Then mediaindex = mediaindex + 1
        SetCaptionText()
        'ExecuteLua("ATEMMixerMPSetMediaIndex(1,2," & mediaindex & ")")
    End Sub

    Private Sub TextLeaderName_LostFocus(ByVal sender As Object, ByVal e As EventArgs) Handles TextLeaderName.LostFocus
        If mediaindex = 1 Then websocket.Send("{""request-type"":""SetTextGDIPlusProperties"",""source"":""Leadername"",""text"":""    " & TextLeaderName.Text & """,""message-id"":""TEST1""}")
    End Sub

    Private Sub BtnMPrev_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnMPrev.Click
        'websocket.Send("{""request-type"":""SetCurrentScene"",""scene-name"":""Mixer output"",""message-id"":""TEST1""}")
        If ListBoxMedia.Items.Count = 0 Then Exit Sub
        If MediaItem > 0 Then
            ListBoxMedia.Items.Item(MediaItem) = Replace(ListBoxMedia.Items.Item(MediaItem), "*", "")
            WebsocketSendAndWait("{""request-type"":""SetSceneItemRender"",""scene-name"":""Mediaplayer1"",""source"":""" & ListBoxMedia.Items.Item(MediaItem) & """,""render"":false,""message-id"":""TEST1""}")
            MediaItem = MediaItem - 1
            WebsocketSendAndWait("{""request-type"":""SetSceneItemRender"",""scene-name"":""Mediaplayer1"",""source"":""" & ListBoxMedia.Items.Item(MediaItem) & """,""render"":true,""message-id"":""TEST1""}")
            ListBoxMedia.Items.Item(MediaItem) = ListBoxMedia.Items.Item(MediaItem) & "*"
        End If
    End Sub
    Private Sub BtnMNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnMNext.Click
        'websocket.Send("{""request-type"":""SetCurrentScene"",""scene-name"":""Loop"",""message-id"":""TEST1""}")
        If ListBoxMedia.Items.Count = 0 Then Exit Sub
        If MediaItem < ListBoxMedia.Items.Count - 1 Then
            ListBoxMedia.Items.Item(MediaItem) = Replace(ListBoxMedia.Items.Item(MediaItem), "*", "")
            WebsocketSendAndWait("{""request-type"":""SetSceneItemRender"",""scene-name"":""Mediaplayer1"",""source"":""" & ListBoxMedia.Items.Item(MediaItem) & """,""render"":false,""message-id"":""TEST1""}")
            MediaItem = MediaItem + 1
            WebsocketSendAndWait("{""request-type"":""SetSceneItemRender"",""scene-name"":""Mediaplayer1"",""source"":""" & ListBoxMedia.Items.Item(MediaItem) & """,""render"":true,""message-id"":""TEST1""}")
            ListBoxMedia.Items.Item(MediaItem) = ListBoxMedia.Items.Item(MediaItem) & "*"
        End If
    End Sub

    Private Sub TextBoxPipSrc_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBoxPipSrc.Click
        Select Case TextBoxPipSrc.Text
            'Case "CAM1" : TextBoxPipSrc.Text = "CAM2" : ExecuteLua("ATEMMixerMEKeySetFillSource(1,1,1,7)")
            'Case "CAM2" : TextBoxPipSrc.Text = "CAM3" : ExecuteLua("ATEMMixerMEKeySetFillSource(1,1,1,8")
            'Case "CAM3" : TextBoxPipSrc.Text = "CAM4" : ExecuteLua("ATEMMixerMEKeySetFillSource(1,1,1,9)")
            'Case "CAM4" : TextBoxPipSrc.Text = "CAM5" : ExecuteLua("ATEMMixerMEKeySetFillSource(1,1,1,4)")
            'Case "CAM5" : TextBoxPipSrc.Text = "Words" : ExecuteLua("ATEMMixerMEKeySetFillSource(1,1,1,2)")
            'Case "Words" : TextBoxPipSrc.Text = "CAM1" : ExecuteLua("ATEMMixerMEKeySetFillSource(1,1,1,6)")
            'Case Else : TextBoxPipSrc.Text = "Words" : ExecuteLua("ATEMMixerMEKeySetFillSource(1,1,1,2)")
        End Select
    End Sub


    Private Sub BtnPip_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPip.Click

        If (BtnPip.BackColor = Color.Red) Then
            'pip is currently active
            BtnPip.BackColor = Color.White
            'If PipLive = True Then ExecuteLua("ATEMMixerMEKeySetOnAir(1,1,1,""FALSE"")")
            'ExecuteLua("ATEMMixerMESetNextTransitionLayers(1,1,""BACKGROUND"")")
            WebsocketSendAndWait("{""request-type"":""SetSceneItemRender"",""scene-name"":""" & ObsSourceName(PipAddr) & """,""source"":""Pip"",""render"":false,""message-id"":""TEST1""}")
            If (PipAddr = liveaddr) Then 'if pip is currently active on live output, we need to do a transition to get it off
                Dim ft As Double
                Double.TryParse(TextBox1.Text, ft)
                WebsocketSendAndWait("{""request-type"":""SetPreviewScene"",""scene-name"":""" & ObsSourceName(liveaddr) & """,""message-id"":""TEST1""}")
                WebsocketSendAndWait("{""request-type"":""TransitionToProgram"",""with-transition"": { ""name"":""Fade"", ""duration"":" & ft * 800 & " } ,""message-id"":""TEST1""}")
                WebsocketSendAndWait("{""request-type"":""SetPreviewScene"",""scene-name"":""" & ObsSourceName(addr) & """,""message-id"":""TEST1""}")
                'set preview back to what it should be
            End If
            PipAddr = 0
            PipLive = False
        Else
            'pip is not active
            BtnPip.BackColor = Color.Red
            'ExecuteLua("ATEMMixerMEKeySetOnAir(1,1,1,""TRUE"")")
            'ExecuteLua("ATEMMixerMESetNextTransitionLayers(1,1,""BACKGROUND, KEY1"")")
            PipAddr = addr
            websocket.Send("{""request-type"":""SetSceneItemRender"",""scene-name"":""" & ObsSourceName(addr) & """,""source"":""Pip"",""render"":true,""message-id"":""TEST1""}")
        End If
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        If legendmode = False Then
            Button7.ForeColor = Color.Red
            legendmode = True
        Else
            Button7.ForeColor = Color.Black
            legendmode = False
        End If
        'test leds on controller
        'SerialPort1.Write(&HFF & &H55 & &H55)
    End Sub

    Private Sub CheckBoxCU_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        setactive()
    End Sub




    Private Sub ButtonSetup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonSetup.Click
        Globals.CamStatus(1) = CamIgnore(1)
        Globals.CamStatus(2) = CamIgnore(2)
        Globals.CamStatus(3) = CamIgnore(3)
        Globals.CamStatus(4) = CamIgnore(4)
        Form2.Show()
    End Sub

    Private Sub BtnIrisAuto_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnIrisAuto.Click
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        mLog.Text = SendCamCmdAddrNoHash(ad, "ORS:1", "aw_cam") 'auto iris
        CamIris(ad) = 9999 'flag auto
        TextBox4.Text = "Auto"
    End Sub
    Private Sub BtnIrisDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnIrisDown.Click
        Dim op As String
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        If CamIris(ad) = 9999 Then 'was auto mode
            If ad <> 7 Then
                op = SendCamCmdAddrNoHash(ad, "QRV", "aw_cam") 'get iris setting
                CamIris(ad) = Val("&H" & Mid(op, 5))
            Else
                CamIris(ad) = &H555
            End If
            mLog.Text = SendCamCmdAddrNoHash(ad, "ORS:0", "aw_cam") 'man iris
        End If
        If ad <> 7 Then
            If CamIris(ad) > 10 Then CamIris(ad) = CamIris(ad) - 10 : Else CamIris(ad) = 0
            TextBox4.Text = CamIris(ad)
            mLog.Text = mLog.Text & SendCamCmdAddrNoHash(ad, "ORV:" & String.Format("{0:X3}", CamIris(ad)), "aw_cam")
        Else
            If CamIris(ad) > &H555 + 64 Then CamIris(ad) = CamIris(ad) - 64
            TextBox4.Text = CamIris(ad)
            mLog.Text = mLog.Text & SendCamCmdAddrNoHash(ad, "%23AXI" & String.Format("{0:X3}", CamIris(ad)), "aw_ptz")
        End If
    End Sub

    Private Sub BtnIrisUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnIrisUp.Click
        Dim op As String
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        If CamIris(ad) = 9999 Then 'was auto mode
            If ad <> 7 Then
                op = SendCamCmdAddrNoHash(ad, "QRV", "aw_cam") 'get iris setting
                CamIris(ad) = Val("&H" & Mid(op, 5))
            Else
                CamIris(ad) = &H555
            End If
            mLog.Text = SendCamCmdAddrNoHash(ad, "ORS:0", "aw_cam") 'man iris
        End If
        If ad <> 7 Then
            If CamIris(ad) < (&H3FF - 10) Then CamIris(ad) = CamIris(ad) + 10 Else CamIris(ad) = &H3FF
            TextBox4.Text = CamIris(ad)
            mLog.Text = mLog.Text & SendCamCmdAddrNoHash(ad, "ORV:" & String.Format("{0:X3}", CamIris(ad)), "aw_cam")
        Else
            If CamIris(ad) < &HFFF - 64 Then CamIris(ad) = CamIris(ad) + 64
            TextBox4.Text = CamIris(ad)
            mLog.Text = mLog.Text & SendCamCmdAddrNoHash(ad, "%23AXI" & String.Format("{0:X3}", CamIris(ad)), "aw_ptz")
        End If
    End Sub


    Private Sub BtnAGCDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAGCDown.Click
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        If ad = 7 Then Return 'not provided on this camera
        'op = SendCamCmdAddrNoHash(ad, "QGU") 'get gain setting
        'mLog.Text = op
        'CamAgc(ad) = Val("&H" & Mid(op, 5))
        If CamAgc(ad) > &H38 Then CamAgc(ad) = &H38
        If CamAgc(ad) > &H8 Then CamAgc(ad) = CamAgc(ad) - 3
        If (CamAgc(ad) <= &H38) Then TextBox5.Text = CamAgc(ad) - 8 & "dB" Else TextBox5.Text = "Auto"
        mLog.Text = mLog.Text & SendCamCmdAddrNoHash(ad, "OGU:" & String.Format("{0:X2}", CamAgc(ad)), "aw_cam")
    End Sub

    Private Sub BtnAGCUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAGCUp.Click
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        If ad = 7 Then Return 'not provided on this camera
        'op = SendCamCmdAddrNoHash(ad, "QGU") 'get gain setting
        'mLog.Text = op
        'CamAgc(ad) = Val("&H" & Mid(op, 5))
        If CamAgc(ad) > &H38 Then CamAgc(ad) = &H38
        If CamAgc(ad) < &H38 Then CamAgc(ad) = CamAgc(ad) + 3
        If (CamAgc(ad) <= &H38) Then TextBox5.Text = CamAgc(ad) - 8 & "dB" Else TextBox5.Text = "Auto"
        mLog.Text = mLog.Text & SendCamCmdAddrNoHash(ad, "OGU:" & String.Format("{0:X2}", CamAgc(ad)), "aw_cam")
    End Sub
    Private Sub BtnAgcAuto_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAgcAuto.Click
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        If ad = 7 Then Return 'not provided on this camera
        CamAgc(ad) = 128
        If (CamAgc(ad) <= &H38) Then TextBox5.Text = CamAgc(ad) - 8 & "dB" Else TextBox5.Text = "Auto"
        mLog.Text = mLog.Text & SendCamCmdAddrNoHash(ad, "OGU:" & String.Format("{0:X2}", CamAgc(ad)), "aw_cam")
    End Sub
    Private Sub BtnGainDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnGainDown.Click
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        If ad = 7 Then Return 'not provided on this camera
        If CamGain(ad) > 1 Then CamGain(ad) = CamGain(ad) - 1
        TextBox7.Text = CamGain(ad) * 6 & "dB"
        mLog.Text = SendCamCmdAddrNoHash(ad, "OSD:69:" & String.Format("{0:X2}", CamGain(ad)), "aw_cam")
    End Sub

    Private Sub BtnGainUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnGainUp.Click
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        If ad = 7 Then Return 'not provided on this camera
        If CamGain(ad) < 8 Then CamGain(ad) = CamGain(ad) + 1
        TextBox7.Text = CamGain(ad) * 6 & "dB"
        mLog.Text = SendCamCmdAddrNoHash(ad, "OSD:69:" & String.Format("{0:X2}", CamGain(ad)), "aw_cam")
    End Sub

    Private Sub BtnShutterDn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnShutterDn.Click
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        If CamShutter(ad) > -10 Then CamShutter(ad) = CamShutter(ad) - 1
        TextBox6.Text = CamShutter(ad)
        mLog.Text = SendCamCmdAddrNoHash(ad, "OSD:48:" & Format((10 + CamShutter(ad)) * 64 / 20, "00"), "aw_cam") 'gain is -10 to +10 but number is 0-64
    End Sub

    Private Sub BtnShutterUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnShutterUp.Click
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        If CamShutter(ad) < 10 Then CamShutter(ad) = CamShutter(ad) + 1
        TextBox6.Text = CamShutter(ad)
        mLog.Text = SendCamCmdAddrNoHash(ad, "OSD:48:" & Format((10 + CamShutter(ad)) * 64 / 20, "00"), "aw_cam")
    End Sub


    Private Sub BtnWbBlueDn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnWbBlueDn.Click
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        If ad <> 7 Then
            mLog.Text = SendCamCmdAddrNoHash(ad, "OAW:9", "aw_cam")
            If CamWBBlue(ad) > 0 Then CamWBBlue(ad) = CamWBBlue(ad) - 1
            TextBox9.Text = 2400 + CamWBBlue(ad) * 100
            mLog.Text = mLog.Text + SendCamCmdNoHash("OSD:B1:" & String.Format("{0:X3}", CamWBBlue(ad)), "aw_cam")
        Else
            If CamWBBlue(ad) > 1 Then CamWBBlue(ad) = CamWBBlue(ad) - 1
            If CamWBBlue(ad) = 1 Then TextBox9.Text = "A"
            If CamWBBlue(ad) = 2 Then TextBox9.Text = "B"
            If CamWBBlue(ad) = 3 Then TextBox9.Text = "Auto"
            If CamWBBlue(ad) = 4 Then TextBox9.Text = "3200"
            If CamWBBlue(ad) = 5 Then TextBox9.Text = "5600"
            If CamWBBlue(ad) = 6 Then TextBox9.Text = "4500"
            If CamWBBlue(ad) = 7 Then TextBox9.Text = "6000"
            If CamWBBlue(ad) = 8 Then TextBox9.Text = "2800"
            mLog.Text = mLog.Text + SendCamCmdNoHash("OAW:" & CamWBBlue(ad), "aw_cam")
        End If
    End Sub

    Private Sub BtnWbBlueUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnWbBlueUp.Click
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        If ad <> 7 Then
            mLog.Text = SendCamCmdAddrNoHash(ad, "OAW:9", "aw_cam")
            If CamWBBlue(ad) < &H4B Then CamWBBlue(ad) = CamWBBlue(ad) + 1
            TextBox9.Text = 2400 + CamWBBlue(ad) * 100
            mLog.Text = mLog.Text + SendCamCmdNoHash("OSD:B1:" & String.Format("{0:X3}", CamWBBlue(ad)), "aw_cam")
        Else
            If CamWBBlue(ad) < 8 Then CamWBBlue(ad) = CamWBBlue(ad) + 1
            If CamWBBlue(ad) = 1 Then TextBox9.Text = "A"
            If CamWBBlue(ad) = 2 Then TextBox9.Text = "B"
            If CamWBBlue(ad) = 3 Then TextBox9.Text = "Auto"
            If CamWBBlue(ad) = 4 Then TextBox9.Text = "3200K"
            If CamWBBlue(ad) = 5 Then TextBox9.Text = "5600K"
            If CamWBBlue(ad) = 6 Then TextBox9.Text = "4500K"
            If CamWBBlue(ad) = 7 Then TextBox9.Text = "6000K"
            If CamWBBlue(ad) = 8 Then TextBox9.Text = "2800K"
            mLog.Text = mLog.Text + SendCamCmdNoHash("OAW:" & CamWBBlue(ad), "aw_cam")
        End If
    End Sub
    Private Sub BtnWBAuto_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnWBAuto.Click
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        mLog.Text = SendCamCmdAddrNoHash(ad, "OAW:0", "aw_cam")
        TextBox9.Text = "Auto"
    End Sub
    Private Sub BtnFocusAuto_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnFocusAuto.Click
        Dim ad As Integer
        If (PTZLive = True) Then ad = liveaddr Else ad = addr
        SendCamCmdAddr(ad, "D11")
        CamFocusManual(ad) = 0
        BtnFocusAuto.BackColor = Color.Green : BtnFocusLock.BackColor = Color.White
    End Sub

    Private Sub BtnFocusLock_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnFocusLock.Click
        Dim ad As Integer
        If (PTZLive = True) Then ad = liveaddr Else ad = addr
        SendCamCmdAddr(ad, "D10")
        CamFocusManual(ad) = 1
        BtnFocusAuto.BackColor = Color.White : BtnFocusLock.BackColor = Color.Green
    End Sub

    Private Sub BtnFocusUp_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles BtnFocusUp.MouseDown
        Dim ad As Integer
        If (PTZLive = True) Then ad = liveaddr Else ad = addr
        If CamFocusManual(ad) = 0 Then
            SendCamCmdAddr(ad, "D10")
            CamFocusManual(ad) = 1
            BtnFocusAuto.BackColor = Color.White : BtnFocusLock.BackColor = Color.Green
        End If
        SendCamCmdAddr(ad, "F60")
    End Sub

    Private Sub BtnFocusUp_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles BtnFocusUp.MouseUp
        Dim ad As Integer
        If (PTZLive = True) Then ad = liveaddr Else ad = addr
        SendCamCmdAddr(ad, "F50")
    End Sub

    Private Sub BtnFocusDn_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles BtnFocusDn.MouseDown
        Dim ad As Integer
        If (PTZLive = True) Then ad = liveaddr Else ad = addr
        If CamFocusManual(ad) = 0 Then
            SendCamCmdAddr(ad, "D10")
            CamFocusManual(ad) = 1
            BtnFocusAuto.BackColor = Color.White : BtnFocusLock.BackColor = Color.Green
        End If
        SendCamCmdAddr(ad, "F40")
    End Sub

    Private Sub BtnFocusDn_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles BtnFocusDn.MouseUp
        Dim ad As Integer
        If (PTZLive = True) Then ad = liveaddr Else ad = addr
        SendCamCmdAddr(ad, "F50")
    End Sub



    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If Button1.ForeColor = Color.Red Then
            ShutDownTimer = 1
        Else
            Button1.ForeColor = Color.Red
            ProgCloseTimer = 100
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        mLog.Text = SendCamCmdAddr(ad, "Z99") 'max speed zoom tele
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        mLog.Text = SendCamCmdAddr(ad, "Z01") 'max speed zoom wide
    End Sub


    Private Sub Button6_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button6.MouseDown
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        If BtnFast.BackColor = Color.Green Then
            mLog.Text = SendCamCmdAddr(ad, "Z05") 'zoom med wide
        Else
            mLog.Text = SendCamCmdAddr(ad, "Z45") 'zoom med wide
        End If
    End Sub


    Private Sub Button2_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button2.MouseDown
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        If BtnFast.BackColor = Color.Green Then
            mLog.Text = SendCamCmdAddr(ad, "Z95") 'zoom med wide
        Else
            mLog.Text = SendCamCmdAddr(ad, "Z55") 'zoom med wide
        End If
    End Sub

    Private Sub Button6_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button6.MouseUp
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        mLog.Text = SendCamCmdAddr(ad, "Z50") 'zoom stop
    End Sub

    Private Sub Button2_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button2.MouseUp
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        mLog.Text = SendCamCmdAddr(ad, "Z50") 'zoom stop
    End Sub

    Private Sub MyButtonCamUL_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyButtonCamUL.MouseDown, MyButtonCamU.MouseDown, MyButtonCamUR.MouseDown, MyButtonCamL.MouseDown, MyButtonCamR.MouseDown, MyButtonCamDL.MouseDown, MyButtonCamD.MouseDown, MyButtonCamDR.MouseDown
        Dim index As String
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        index = (Mid(sender.name, 12))
        Dim xsp As String, ysp As String, xpos As Integer, ypos As Integer, spd As Integer
        If index = "UL" Then xpos = -1 : ypos = -1
        If index = "U" Then xpos = 0 : ypos = -1
        If index = "UR" Then xpos = 1 : ypos = -1
        If index = "L" Then xpos = -1 : ypos = 0
        If index = "R" Then xpos = 1 : ypos = 0
        If index = "DL" Then xpos = -1 : ypos = 1
        If index = "D" Then xpos = 0 : ypos = 1
        If index = "DR" Then xpos = 1 : ypos = 1
        If Globals.CamInvert(ad) Then xpos = -xpos : ypos = -ypos
        xsp = "50" : ysp = "50"
        If BtnFast.BackColor = Color.Green Then spd = 25 Else spd = 10
        If (xpos < 0) Then xsp = Val(xsp) - spd
        If (xpos > 0) Then xsp = Val(xsp) + spd
        If (ypos < 0) Then ysp = Val(ysp) + spd
        If (ypos > 0) Then ysp = Val(ysp) - spd
        mLog.Text = SendCamCmdAddr(ad, "PTS" & xsp & ysp) 'pan tilt at speed
        'Label7.Text = xpos & " " & ypos

    End Sub

    Private Sub MyButtonCamUL_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyButtonCamUL.MouseUp, MyButtonCamU.MouseUp, MyButtonCamUR.MouseUp, MyButtonCamL.MouseUp, MyButtonCamR.MouseUp, MyButtonCamDL.MouseUp, MyButtonCamD.MouseUp, MyButtonCamDR.MouseUp
        Dim ad As Integer
        If PTZLive = False Then ad = addr Else ad = liveaddr
        mLog.Text = SendCamCmdAddr(ad, "PTS5050") 'pt stop
    End Sub

    Private Sub BtnFast_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnFast.Click
        BtnFast.BackColor = Color.Green
        BtnSlow.BackColor = Color.White
    End Sub

    Private Sub BtnSlow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSlow.Click
        BtnFast.BackColor = Color.White
        BtnSlow.BackColor = Color.Green
    End Sub


    Private Sub BtnLive_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnLive.Click
        If PresetLive = False Then
            BtnLive.BackColor = Color.Green
            PresetLive = True
            BtnPreload.BackColor = Color.White
            PreloadPreset = 0
        Else
            BtnLive.BackColor = Color.White
            PresetLive = False
        End If
        UpdatePresets()
    End Sub

    Private Sub BtnLivePTZ_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnLivePTZ.Click
        If PTZLive = False Then
            BtnLivePTZ.BackColor = Color.Green
            PTZLive = True
        Else
            BtnLivePTZ.BackColor = Color.White
            PTZLive = False
        End If
        ShowCamValues()
    End Sub

    Private Sub BtnPreload_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPreload.Click
        If PreloadPreset = 0 Then
            BtnPreload.BackColor = Color.Orange
            PreloadPreset = 99
            BtnLive.BackColor = Color.White
            PresetLive = False
        Else
            BtnPreload.BackColor = Color.White
            PreloadPreset = 0
            'cdir = 0
            SetLiveMoveIndicators()
            UpdatePresets()
        End If
        AutoPreset = 0
    End Sub

    Private Sub MyButton2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAutoSong.Click
        If AutoSongMode = 0 Then
            BtnAutoSong.BackColor = Color.Green
            BtnAutoSpeech.BackColor = Color.White
            AutoSongMode = 1
            AutoSpeechMode = 0
            If overlayactive = False Then BtnOverlay_Click(BtnOverlay, Nothing) 'ensure overlay is turned on in song mode
            'preload next shot for song mode here
            NextAutoShot()

        Else
            BtnAutoSong.BackColor = Color.White
            AutoSongMode = 0
            PreloadPreset = 0
            cdir = 0
            SetLiveMoveIndicators()
            UpdatePresets()
        End If
    End Sub

    Private Sub BtnAutoSpeech_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAutoSpeech.Click
        If AutoSpeechMode = 0 Then
            BtnAutoSpeech.BackColor = Color.Green
            BtnAutoSong.BackColor = Color.White
            AutoSongMode = 0
            AutoSpeechMode = 1
            'preload next shot for speech mode here
            NextAutoShot()
        Else
            BtnAutoSpeech.BackColor = Color.White
            AutoSpeechMode = 0
            PrevAutoSpeech = False
            cdir = 0
            SetLiveMoveIndicators()
            UpdatePresets()
        End If

    End Sub
    Sub PresetClick(ByVal c As Integer)
        If c = 1 Then BtnPreset_Click(BtnPreset1, Nothing)
        If c = 2 Then BtnPreset_Click(BtnPreset2, Nothing)
        If c = 3 Then BtnPreset_Click(BtnPreset3, Nothing)
        If c = 4 Then BtnPreset_Click(BtnPreset4, Nothing)
        If c = 5 Then BtnPreset_Click(BtnPreset5, Nothing)
        If c = 6 Then BtnPreset_Click(BtnPreset6, Nothing)
        If c = 7 Then BtnPreset_Click(BtnPreset7, Nothing)
        If c = 8 Then BtnPreset_Click(BtnPreset8, Nothing)
        If c = 9 Then BtnPreset_Click(BtnPreset9, Nothing)
        If c = 10 Then BtnPreset_Click(BtnPreset10, Nothing)
        If c = 11 Then BtnPreset_Click(BtnPreset11, Nothing)
        If c = 12 Then BtnPreset_Click(BtnPreset12, Nothing)
        If c = 13 Then BtnPreset_Click(BtnPreset13, Nothing)
        If c = 14 Then BtnPreset_Click(BtnPreset14, Nothing)
        If c = 15 Then BtnPreset_Click(BtnPreset15, Nothing)
        If c = 16 Then BtnPreset_Click(BtnPreset16, Nothing)
    End Sub

    Sub NextAutoShot()
        Dim c As Integer, i As Integer
        Dim f As Boolean
        Dim ShotSize As Integer
        Dim Newcontent As Integer
        If AutoSongMode = True Or SingleAutoShot = 1 Then
            'pick next cam at random
            Do
                c = CInt(Rnd() * 4 + 1)
            Loop While c = liveaddr Or (SingleAutoShot = 1 And c = addr) Or c > 4
            If c = 1 Then BtnCam1_Click(BtnCam1, Nothing)
            If c = 2 Then BtnCam1_Click(BtnCam2, Nothing)
            If c = 3 Then BtnCam1_Click(BtnCam3, Nothing)
            If c = 4 Then BtnCam1_Click(BtnCam4, Nothing)
            'find shots for next cam that aren't "stage"
            'PresetContent: 1=stage 2=band (not singers) 3=congregation 4=all 5=singers
            f = False
            'current shot content is in prevautosongshot
            'Debug.Print(PrevAutoSongShot)
            For c = 1 To 16 'check that there is at least one suitable preset
                If PresetAuto((c - 1) + ((addr - 1) * 16)) = True Then 'if shot is enabled for auto
                    Newcontent = PresetContent((c - 1) + ((addr - 1) * 16))
                    If SingleAutoShot = 1 Then
                        If Newcontent <> 1 Then f = True
                    Else
                        If PrevAutoSongShot = 5 Then 'if previous shot singers, we want not singers
                            If Newcontent = 2 Or Newcontent = 3 Or Newcontent = 4 Then f = True
                        Else 'else go for singers
                            If Newcontent = 4 Or Newcontent = 5 Then f = True
                        End If
                    End If
                End If
            Next
            If f = True Then 'only change the shot if there is something suitable
                f = False
                Do
                    c = CInt(Rnd() * 16 + 1)
                    If (c < 17) Then
                        If PresetAuto((c - 1) + ((addr - 1) * 16)) = True Then
                            Newcontent = PresetContent((c - 1) + ((addr - 1) * 16))
                            If SingleAutoShot = 1 Then 'in magic shot mode pick any shot except stage
                                If Newcontent <> 1 Then f = True
                            Else
                                If PrevAutoSongShot = 5 Then 'if previous shot singers, we want not singers
                                    If Newcontent = 2 Or Newcontent = 3 Or Newcontent = 4 Then f = True
                                Else
                                    If Newcontent = 4 Or Newcontent = 5 Then f = True
                                End If
                            End If
                        End If
                    End If
                Loop While f = False
                AutoPreset = 1
                PrevAutoSongShot = Newcontent 'remember the content of this shot so we don't do 2 the same
                PresetClick(c)
                'if new shot is closeup, 25% chance fixed zoom out, 50% chance move to wide shot
                'PresetSize: 1=very close 2=close 3=mid 4=full 5=wide 6=full wide
                ShotSize = PresetSize((c - 1) + ((addr - 1) * 16))
                If ShotSize <= 5 Then
                    If (Rnd() < 0.3) Then ' Or ShotSize = 1 Then
                        BtnSlowOut_Click(BtnSlowOut, Nothing)
                        AutoSongPreload = True
                    Else
                        If (Rnd() > 0.5) Then
                            f = False
                            For i = 1 To 16 'first check that there is another suitable preset to go to
                                If PresetAuto((i - 1) + ((addr - 1) * 16)) = True And PresetContent((i - 1) + ((addr - 1) * 16)) <> 1 And PresetSize((i - 1) + ((addr - 1) * 16)) > 4 And i <> c Then f = True
                            Next
                            If f = True Then 'there is at least one, pick at random
                                BtnPreload_Click(BtnPreload, Nothing)
                                AutoSongPreload = True
                                f = False
                                Do
                                    i = CInt(Rnd() * 16 + 1)
                                    If (i < 17) Then
                                        If PresetAuto((i - 1) + ((addr - 1) * 16)) = True And PresetContent((i - 1) + ((addr - 1) * 16)) <> 1 And PresetSize((i - 1) + ((addr - 1) * 16)) > 4 And i <> c Then f = True
                                    End If
                                Loop While f = False
                                AutoPreset = 1
                                PresetClick(i)
                            End If
                        End If
                    End If

                End If

                'if shot is wide, 50% chance zoom in to close shot
                If ShotSize >= 5 Then
                    If (Rnd() < 0.5) Then
                        f = False
                        For i = 1 To 16 'first check that there is another suitable preset to go to
                            If PresetAuto((i - 1) + ((addr - 1) * 16)) = True And PresetContent((i - 1) + ((addr - 1) * 16)) <> 1 And PresetSize((i - 1) + ((addr - 1) * 16)) <= 4 And i <> c Then f = True
                        Next
                        If f = True Then 'there is at least one, pick at random
                            BtnPreload_Click(BtnPreload, Nothing)
                            AutoSongPreload = True
                            f = False
                            Do
                                i = CInt(Rnd() * 16 + 1)
                                If (i < 17) Then
                                    If PresetAuto((i - 1) + ((addr - 1) * 16)) = True And PresetContent((i - 1) + ((addr - 1) * 16)) <> 1 And PresetSize((i - 1) + ((addr - 1) * 16)) <= 4 And i <> c Then f = True
                                End If
                            Loop While f = False
                            AutoPreset = 1
                            PresetClick(i)
                        End If
                    End If

                End If
            End If
        End If

        If AutoSpeechMode = True Then
            'if livecam is 1 pick next cam at random and pick wide stage cong or all
            'else cam 1, stage close or mid
            If liveaddr = 1 Then
                'pick next cam at random - go to a wide shot
                Do
                    c = CInt(Rnd() * 4 + 1)
                Loop While c = liveaddr Or c > 4
                If c = 1 Then BtnCam1_Click(BtnCam1, Nothing)
                If c = 2 Then BtnCam1_Click(BtnCam2, Nothing)
                If c = 3 Then BtnCam1_Click(BtnCam3, Nothing)
                If c = 4 Then BtnCam1_Click(BtnCam4, Nothing)

                f = False
                For i = 1 To 16 'first check that there is another suitable preset to go to
                    If PresetAuto((i - 1) + ((addr - 1) * 16)) = True And PresetContent((i - 1) + ((addr - 1) * 16)) <> 2 And PresetSize((i - 1) + ((addr - 1) * 16)) >= 4 Then f = True
                Next
                If f = True Then 'there is at least one, pick at random
                    f = False
                    Do
                        i = CInt(Rnd() * 16 + 1)
                        If (i < 17) Then
                            If PresetAuto((i - 1) + ((addr - 1) * 16)) = True And PresetContent((i - 1) + ((addr - 1) * 16)) <> 2 And PresetSize((i - 1) + ((addr - 1) * 16)) >= 4 Then f = True
                        End If
                    Loop While f = False
                    PresetClick(i)
                End If


            Else
                'return to the stage shot and pick a slightly closer or wider shot
                If (PrevAutoSpeech = False) Then 'pick close stage shot if we don't already have one
                    PrevAutoSpeech = True
                    For i = 1 To 16 'first check that there is another suitable preset to go to
                        If PresetAuto((i - 1) + ((addr - 1) * 16)) = True And PresetContent((i - 1) + ((addr - 1) * 16)) = 1 And PresetSize((i - 1) + ((addr - 1) * 16)) <= 3 Then f = True
                    Next
                    If f = True Then 'there is at least one, pick at random
                        f = False
                        Do
                            i = CInt(Rnd() * 16 + 1)
                            If (i < 17) Then
                                If PresetAuto((i - 1) + ((addr - 1) * 16)) = True And PresetContent((i - 1) + ((addr - 1) * 16)) = 1 And PresetSize((i - 1) + ((addr - 1) * 16)) <= 3 Then f = True
                            End If
                        Loop While f = False
                        PresetClick(i)
                    End If
                End If
            End If


        End If
    End Sub

    Private Sub BtnLiveSlow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnLiveSlow.Click
        BtnLiveSlow.BackColor = Color.Green
        BtnLiveFast.BackColor = Color.White
        LiveMoveSpeed = 0
    End Sub

    Private Sub BtnLiveFast_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnLiveFast.Click
        BtnLiveSlow.BackColor = Color.White
        BtnLiveFast.BackColor = Color.Green
        LiveMoveSpeed = 1
    End Sub


    Private Sub MagicShotClick_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MagicShotButton.Click
        SingleAutoShot = 1
        AutoPreset = 1
        NextAutoShot()

        SingleAutoShot = 0
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        CamIgnore(1) = False
        CamIgnore(2) = False
        CamIgnore(3) = False
        CamIgnore(4) = False
        CamCmdPending = False
    End Sub

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        websocket.Close()   'try reconnecting to OBS
        OpenWebSocket()
        While WebsocketReinitTimer > 0
            Application.DoEvents()
        End While
        ReadMediaSources()
    End Sub

    Private Sub OverrideBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OverrideBtn.Click
        If CamOverride <> 0 Then
            OverrideBtn.BackColor = Color.White
            CamOverride = 0
        End If
    End Sub

    Private Sub BtnAuxLock_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAuxLock.Click
        If BtnAuxLock.BackColor = Color.Red Then
            BtnAuxLock.BackColor = Color.White
        Else
            BtnAuxLock.BackColor = Color.Red
        End If
    End Sub
    Function CamRecStatus(ByVal inp As String, ByVal cam As Integer)
        Dim str As String
        Dim op As String
        op = ""
        str = Mid(inp, InStr(inp, "rec=") + 4, 2) 'rec state
        If (cam = 1) Then
            If str = "on" Then
                CamRec(1) = True : BtnCam1Rec.BackColor = Color.Red
            Else
                CamRec(1) = False : BtnCam1Rec.BackColor = Color.White
            End If
        End If
        If (cam = 2) Then
            If str = "on" Then
                CamRec(2) = True : BtnCam2Rec.BackColor = Color.Red
            Else
                CamRec(2) = False : BtnCam2Rec.BackColor = Color.White
            End If
        End If
        If (cam = 3) Then
            If str = "on" Then
                CamRec(3) = True : BtnCam3Rec.BackColor = Color.Red
            Else
                CamRec(3) = False : BtnCam3Rec.BackColor = Color.White
            End If
        End If
        If (cam = 4) Then
            If str = "on" Then
                CamRec(4) = True : BtnCam4Rec.BackColor = Color.Red
            Else
                CamRec(4) = False : BtnCam4Rec.BackColor = Color.White
            End If
        End If
        str = Mid(inp, InStr(inp, "sd_insert=") + 10, 2) 'sd insert state
        If str <> "on" Then
            CamRecStatus = "No SD card"
            Exit Function
        End If
        str = Mid(inp, InStr(inp, "sd_error=") + 9, 2) 'sd error state
        If str <> "of" Then
            CamRecStatus = "SD card error"
            Exit Function
        End If
        op = Mid(inp, InStr(inp, "rec_counter=") + 12, 8) & vbCrLf 'rec counter
        str = Mid(inp, InStr(inp, "sd_rem=") + 7) & vbCrLf 'sd remain
        str = Mid(str, 1, InStr(str, Chr(13)) - 1)
        op = op + str
        str = Mid(inp, InStr(inp, "sd_org=") + 7) & vbCrLf 'sd total
        str = Mid(str, 1, InStr(str, Chr(13)) - 1)
        op = op + " of " + str + "GB"
        CamRecStatus = op
    End Function
    Private Sub BtnCam1Rec_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnCam1Rec.Click
        If CamRec(1) Then
            SendCamQueryNoResponse(1, "sdctrl?save=end")
        Else
            SendCamQueryNoResponse(1, "sdctrl?save=start")
            CamRec(1) = True
        End If
        'TextBoxCam1Rec.Text = CamRecStatus(SendCamQuery(1, "get_state"), 1)
        'rec = off
        'rec_counter=00:00:00
        'ftp_send = off
        'play = off
        'del_file = off
        'download = off
        'sd_format = off
        'sd_insert=on
        'sd_repair = off
        'sd_error = off
        'sd_rem = 0.0
        'sd_org = 29.8
    End Sub

    Private Sub BtnCam2Rec_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnCam2Rec.Click
        If CamRec(2) Then
            SendCamQueryNoResponse(2, "sdctrl?save=end")
        Else
            SendCamQueryNoResponse(2, "sdctrl?save=start")
            CamRec(2) = True
        End If
    End Sub

    Private Sub BtnCam3Rec_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnCam3Rec.Click
        If CamRec(3) Then
            SendCamQueryNoResponse(3, "sdctrl?save=end")
        Else
            SendCamQueryNoResponse(3, "sdctrl?save=start")
            CamRec(3) = True
        End If
    End Sub

    Private Sub BtnCam4Rec_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnCam4Rec.Click
        'TextBoxCam4Rec.Text = CamRecStatus(SendCamQuery(4, "get_state"), 4)
        If CamRec(4) Then
            SendCamQueryNoResponse(4, "sdctrl?save=end")
        Else
            SendCamQueryNoResponse(4, "sdctrl?save=start")
            CamRec(4) = True
        End If
    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        Form4.Show() 'the sd card file download form
    End Sub


    Private Sub BtnOBSamintro_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnOBSamintro.Click
        'websocket.Send("{""request-type"":""SetCurrentScene"",""scene-name"":""Intro video morning"",""message-id"":""TEST1""}")

    End Sub
    Private Sub BtnOBSpmintro_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnOBSpmintro.Click
        websocket.Send("{""request-type"":""SetCurrentScene"",""scene-name"":""Intro video evening"",""message-id"":""TEST1""}")
        BtnMPrev.BackColor = Color.White
        BtnOBSamintro.BackColor = Color.White
        BtnOBSpmintro.BackColor = Color.Green
        BtnOBSIdent.BackColor = Color.White
        BtnMNext.BackColor = Color.White
        If TieAux3 And PrevAux3 = 0 Then
            PrevAux3 = CurrentAux3 'remember what it was set to
            SetAux3(4) 'set foldback output to show OBS preview
        End If

        'if broadcasting but not recording, start the recording
        If OBSStreamState = True And OBSRecState = False Then
            BtnOBSRecord_Click(sender, e)
        End If

        ClipRemainTime = Globals.Cliptime(2)

    End Sub
    Private Sub BtnOBSIdent_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnOBSIdent.Click
        websocket.Send("{""request-type"":""SetCurrentScene"",""scene-name"":""Ident video"",""message-id"":""TEST1""}")
        BtnMPrev.BackColor = Color.White
        BtnOBSamintro.BackColor = Color.White
        BtnOBSpmintro.BackColor = Color.White
        BtnOBSIdent.BackColor = Color.Green
        BtnMNext.BackColor = Color.White
        If TieAux3 And PrevAux3 = 0 Then
            PrevAux3 = CurrentAux3 'remember what it was set to
            SetAux3(4) 'set foldback output to show OBS preview
        End If

        ClipRemainTime = Globals.Cliptime(3)
    End Sub



    Private Sub BtnOBSBroadcast_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnOBSBroadcast.Click
        'Dim ct As Integer
        'OBSResponse = ""
        'websocket.Send("{""request-type"":""GetStreamingStatus"",""message-id"":""OBSSTATE""}")
        'ct = 0
        'While OBSResponse = "" And ct < 1000000 : ct = ct + 1 : End While
        If OBSStreamState = False Then
            'BtnOBSBroadcast.BackColor = Color.Red
            websocket.Send("{""request-type"":""StartStopStreaming"",""message-id"":""OBSSET""}")
            StreamStartTime = Now.TimeOfDay.TotalSeconds
        Else
            'BtnOBSBroadcast.BackColor = Color.White
            websocket.Send("{""request-type"":""StartStopStreaming"",""message-id"":""OBSSET""}")
        End If
        websocket.Send("{""request-type"":""GetStreamingStatus"",""message-id"":""OBSSTATE""}")

    End Sub
    Sub GetOBSState()
        Dim ct As Integer
        OBSResponse = ""
        websocket.Send("{""request-type"":""GetStreamingStatus"",""message-id"":""TEST1""}")
        ct = 0
        While OBSResponse = "" And ct < 1000000 : ct = ct + 1 : End While

    End Sub
    Private Sub BtnOBSRecord_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnOBSRecord.Click
        'websocket.Send("{""request-type"":""StartRecording"",""message-id"":""TEST1""}")
        'Dim ct As Integer
        'OBSResponse = ""
        'websocket.Send("{""request-type"":""GetStreamingStatus"",""message-id"":""OBSSTATE""}")
        'ct = 0
        'While OBSResponse = "" And ct < 1000000 : ct = ct + 1 : End While
        If OBSRecState = False Then
            'BtnOBSRecord.BackColor = Color.Red
            websocket.Send("{""request-type"":""StartStopRecording"",""message-id"":""OBSSET""}")
            RecStartTime = Now.TimeOfDay.TotalSeconds
        Else
            'BtnOBSRecord.BackColor = Color.White
            websocket.Send("{""request-type"":""StartStopRecording"",""message-id"":""OBSSET""}")
        End If
        websocket.Send("{""request-type"":""GetStreamingStatus"",""message-id"":""OBSSTATE""}")
        'websocket.Send("{""request-type"":""SetHeartbeat"",""message-id"":""OBSHB""}")
        'websocket.Send("{""request-type"":""StartRecording"",""message-id"":""TEST""}")
    End Sub


    Sub socketDataReceived(ByVal ss As Object, ByVal e As WebSocket4Net.DataReceivedEventArgs)
        Dim str As String
        str = Encoding.ASCII.GetString(e.Data)
        MsgBox("Got OBS message! " & str)
    End Sub

    'Sub socketOpened(ByVal s As Object, ByVal e As EventArgs)
    '    websocket.Send("{""ticks_history"":""R_50"",""end"":""latest"",""count"":10}")
    'End Sub

    'Sub socketClosed(ByVal s As Object, ByVal e As EventArgs)

    'End Sub

    'Sub socketError(ByVal s As Object, ByVal e As SuperSocket.ClientEngine.ErrorEventArgs)

    'End Sub

    Function jsonValue(ByVal json As String, name As String)
        Dim p As Integer, v As Integer
        jsonValue = ""
        p = InStr(json, name)
        If p = 0 Then Exit Function
        json = Mid(json, p + Len(name))
        v = InStr(json, ":")
        If p = 0 Then Exit Function
        json = Mid(json, v + 1)
        p = InStr(json, ",") : If (p = 0) Then p = InStr(json, "}") : If p = 0 Then p = Len(json)
        jsonValue = Mid(json, 1, p - 1)
    End Function


    Sub socketMessage(ByVal s As Object, ByVal e As WebSocket4Net.MessageReceivedEventArgs)
        Dim RecState As String = ""
        Dim StreamState As String = ""
        Dim SceneState As String = ""
        Dim litem As String
        Dim p As Integer, q As Integer, ct As Integer
        OBSResponse = e.Message

        If InStr(OBSResponse, "OBSSTATE") <> 0 Then 'if this is a response to obs rec/stream status message
            RecState = jsonValue(OBSResponse, """recording""")
            If RecState = "" Or RecState = "false" Then OBSRecState = False Else OBSRecState = True
            If OBSRecState = True And BtnOBSRecord.BackColor <> Color.Red Then BtnOBSRecord.BackColor = Color.Red
            If OBSRecState = False And BtnOBSRecord.BackColor = Color.Red Then BtnOBSRecord.BackColor = Color.White
            StreamState = jsonValue(OBSResponse, """streaming""")
            If StreamState = "" Or StreamState = "false" Then OBSStreamState = False Else OBSStreamState = True
            If OBSStreamState = True And BtnOBSBroadcast.BackColor <> Color.Red Then BtnOBSBroadcast.BackColor = Color.Red
            If OBSStreamState = False And BtnOBSBroadcast.BackColor = Color.Red Then BtnOBSBroadcast.BackColor = Color.White
            If jsonValue(OBSResponse, """stream-timecode""") <> "" Then
                'OBSStreamTime = Mid(OBSResponse, InStr(OBSResponse, "stream-timecode") + 19, 8)
                Dim t As Integer = Now.TimeOfDay.TotalSeconds - StreamStartTime
                Dim hr As Integer = Math.Floor(t / 3600)
                Dim min As Integer = (Math.Floor(t / 60)) Mod 60
                Dim sec As Integer = t Mod 60
                OBSStreamTime = hr.ToString("00") & ":" & min.ToString("00") & ":" & sec.ToString("00")
            Else
                OBSStreamTime = "..."
            End If
            TextBoxOBSBroadcastTime.Text = OBSStreamTime
            If jsonValue(OBSResponse, """rec-timecode""") <> "" Then
                'OBSRecTime = Mid(OBSResponse, InStr(OBSResponse, "rec-timecode") + 16, 8)
                Dim t As Integer = Now.TimeOfDay.TotalSeconds - RecStartTime
                Dim hr As Integer = Math.Floor(t / 3600)
                Dim min As Integer = (Math.Floor(t / 60)) Mod 60
                Dim sec As Integer = t Mod 60
                OBSRecTime = hr.ToString("00") & ":" & min.ToString("00") & ":" & sec.ToString("00")
            Else
                OBSRecTime = "..."
            End If
            TextBoxOBSRecTime.Text = OBSRecTime
            'End If
        ElseIf jsonValue(OBSResponse, """OBSSCENE""") <> "" Then 'response as we are playing a clip. Watch out for it ending.
            If jsonValue(OBSResponse, """name""") <> "" Then
                SceneState = jsonValue(OBSResponse, """name""")
            End If
        ElseIf InStr(OBSResponse, "PreviewSceneChanged") Then 'user has changed the preview scene on OBS
            'Dim scenename As String
            'scenename = Mid(OBSResponse, InStr(OBSResponse, "scene-name") + 14)
            'scenename = Mid(scenename, 1, InStr(scenename, """") - 1)
            'If (scenename = "Cam1") Then addr = 1 : setactive()
            'If (scenename = "Cam2") Then addr = 2 : setactive()
            'If (scenename = "Cam3") Then addr = 3 : setactive()
        ElseIf jsonValue(OBSResponse, """GETSCENE""") <> "" Then 'response to get scene list, used to populate media list
            SceneState = Mid(OBSResponse, InStr(OBSResponse, "Mediaplayer1"))
            SceneState = Mid(SceneState, InStr(SceneState, """sources"":") + 10)
            If InStr(SceneState, """sources"":") <> 0 Then SceneState = Mid(SceneState, 1, InStr(SceneState, """sources"":") - 40) 'if other scenes after this one, remove them
            Debug.Print(SceneState)
            p = 1 : ct = 0
            While p <> 0
                p = InStr(SceneState, """name""")
                If p <> 0 Then
                    litem = jsonValue(SceneState, """name""") 'Mid(SceneState, p + 9, q - p - 9)
                    litem = Replace(litem, """", "")
                    SceneState = Mid(SceneState, p + 9)
                    If ct = 0 Then
                        websocket.Send("{""request-type"":""SetSceneItemRender"",""scene-name"":""Mediaplayer1"",""source"":""" & litem & """,""render"":true,""message-id"":""TEST1""}")
                        litem = litem & "*"
                    Else
                        websocket.Send("{""request-type"":""SetSceneItemRender"",""scene-name"":""Mediaplayer1"",""source"":""" & litem & """,""render"":false,""message-id"":""TEST1""}")
                    End If
                    ListBoxMedia.Invoke(Sub() ListBoxMedia.Items.Add(litem))
                    ct = ct + 1
                End If
            End While
        ElseIf jsonValue(OBSResponse, """WS" & WebsocketID & """") <> "" Then
            Websocketwait = False
            WebsocketID = WebsocketID + 1
        Else
            Debug.Print(OBSResponse)
        End If
        'MsgBox(e.Message)
    End Sub

    '#########################################################################################################################################################################
    ' SERIAL PORT (PTZ controller)
    '#########################################################################################################################################################################

    Private Sub SerialPort1_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
        Dim x As Byte, k As Byte
        Dim ad As Integer
        Dim op As String
        CheckForIllegalCrossThreadCalls = False
        If SerialPort1.IsOpen = False Then Exit Sub
        While SerialPort1.BytesToRead > 0
            x = SerialPort1.ReadByte
            If (x = 255) Then
                serialcount = serialcount + 1
                'If serialcount >= 6 Then 'serial data thinning so we don't overwhelm the cameras
                If 1 Then
                    serialcount = 0
                    SerialInBufPtr = 0
                    JoyX = SerialInBuf(1)
                    JoyY = SerialInBuf(2)
                    JoyZ = SerialInBuf(3)
                    Key(0) = SerialInBuf(4)
                    Key(1) = SerialInBuf(5)
                    Key(2) = SerialInBuf(6)
                    REncode = SerialInBuf(7)
                    'diagnostic
                    'TextBox10.Text = SerialInBuf(1) & vbCrLf & SerialInBuf(2) & vbCrLf & SerialInBuf(3)
                    'TextBox11.Text = SerialInBuf(4) & vbCrLf & SerialInBuf(5) & vbCrLf & SerialInBuf(6) & vbCrLf & SerialInBuf(7)
                    'check for keypress
                    If PrevKey(0) <> Key(0) Or PrevKey(1) <> Key(1) Or PrevKey(2) <> Key(2) Then
                        If Key(0) > PrevKey(0) Then
                            KeyHit = True
                            x = Key(0) Xor PrevKey(0)
                            For k = 0 To 7
                                If (x And (2 ^ k)) <> 0 Then LastKey = k
                            Next k
                        End If
                        PrevKey(0) = Key(0)
                        If Key(1) > PrevKey(1) Then
                            KeyHit = True
                            x = Key(1) Xor PrevKey(1)
                            For k = 0 To 7
                                If (x And (2 ^ k)) <> 0 Then LastKey = k + 8
                            Next k
                        End If
                        PrevKey(1) = Key(1)
                        If Key(2) > PrevKey(2) Then
                            KeyHit = True
                            x = Key(2) Xor PrevKey(2)
                            For k = 0 To 7
                                If (x And (2 ^ k)) <> 0 Then LastKey = k + 16
                            Next k
                        End If
                        PrevKey(2) = Key(2)
                    End If
                    'check for encoder change
                    If REncode <> PrevEncode Then EncChange = True

                    'if 1-4 keypress then override cam operation
                    If (KeyHit) Then
                        mLog.Text = LastKey
                        KeyHit = False
                        If (LastKey = 11) Then
                            If (CamOverride <> 1) Then
                                'SerialPort1.Write(Chr(&HFF) & Chr(&H1) & Chr(&H0) & Chr(&H0) & Chr(&H0) & Chr(&H0) & Chr(&H0))
                                'SerialPort1.Write(Chr(&HFF) & Chr(&HFF) & Chr(&HFF))
                                CamOverride = 1
                                OverrideBtn.BackColor = Color.Orange
                            Else
                                CamOverride = 0
                                'SerialPort1.Write(Chr(&HFF) & Chr(&HAA) & Chr(&HAA))
                                'SerialPort1.Write(Chr(&HFF) & Chr(&H0) & Chr(&H0) & Chr(&H0) & Chr(&H0))
                                OverrideBtn.BackColor = Color.White
                            End If
                        End If
                        If (LastKey = 12) Then
                            If (CamOverride <> 2) Then
                                'SerialPort1.Write(&HFF & &H2 & &H0 & &H0)
                                'SerialPort1.Write(&HFF & &HAA & &HAA)
                                CamOverride = 2
                                OverrideBtn.BackColor = Color.Orange
                            Else
                                CamOverride = 0
                                OverrideBtn.BackColor = Color.White
                            End If
                        End If
                        If (LastKey = 13) Then
                            If (CamOverride <> 3) Then
                                'SerialPort1.Write(&HFF & &H4 & &H0)
                                CamOverride = 3
                                OverrideBtn.BackColor = Color.Orange
                            Else
                                CamOverride = 0
                                OverrideBtn.BackColor = Color.White
                            End If
                        End If
                        If (LastKey = 14) Then
                            If (CamOverride <> 4) Then
                                'SerialPort1.Write(&HFF & &H8 & &H0)
                                CamOverride = 4
                                OverrideBtn.BackColor = Color.Orange
                            Else
                                CamOverride = 0
                                OverrideBtn.BackColor = Color.White
                            End If
                        End If
                        If (LastKey = 15) Then
                            CamOverride = 0
                            OverrideBtn.BackColor = Color.White
                        End If
                    End If

                    'check for joystick being operated
                    'we will define a dead band at the centre of the joystick
                    Dim JoyDB As Byte = 8 'deadband of joystick
                    'If JoyX < (128 - JoyDB) Or JoyX > (128 + JoyDB) Or JoyY < (128 - JoyDB) Or JoyY > (128 + JoyDB) Or JoyZ < (128 - JoyDB) Or JoyZ > (128 + JoyDB) Then

                    Dim xpos As Integer, ypos As Integer, zpos As Integer, zoom As Boolean = False
                    If PTZLive = False Then ad = addr Else ad = liveaddr
                    If (CamOverride > 0) Then ad = CamOverride 'override the selected camera from the buttons
                    If (ad < 5) Or ad = 7 Then
                        xpos = JoyX
                        ypos = JoyY
                        zpos = 255 - JoyZ
                        'If (xpos > 128 - JoyDB) And (xpos < 128 + JoyDB) Then
                        'xpos = 128
                        'Else
                        'If (xpos < 128 - JoyDB) Then
                        'xpos = xpos + JoyDB
                        'Else
                        'xpos = xpos - JoyDB
                        'End If
                        'End If
                        'If (ypos > 128 - JoyDB) And (ypos < 128 + JoyDB) Then
                        'ypos = 128
                        'Else
                        'If (ypos < 128 - JoyDB) Then
                        '    ypos = ypos + JoyDB
                        'Else
                        'ypos = ypos - JoyDB
                        'End If
                        'End If
                        'If (zpos > 128 - JoyDB) And (zpos < 128 + JoyDB) Then
                        'zpos = 128
                        'Else
                        'If (zpos < 128 - JoyDB) Then
                        'zpos = zpos + JoyDB
                        'Else
                        'zpos = zpos - JoyDB
                        'End If
                        'End If
                        If Globals.CamInvert(ad) Then xpos = 255 - xpos : ypos = 255 - ypos
                        'JoyZ = zpos - JoyDB : JoyX = xpos - JoyDB : JoyY = ypos - JoyDB
                        'JoyZ = 1 + Int(JoyZ * 99 / (255 - JoyDB * 2))
                        If (zpos >= 128) Then JoyZ = 100 - zoomconvert(255 - zpos) Else JoyZ = zoomconvert(zpos)
                        If (JoyZ <> PrevJoyZ) Then
                            If (alreadysending = False) Then 'this function is reentrant. We need to make sure we are not already sending something from a previous command.
                                op = Format(JoyZ, "00")
                                alreadysending = True
                                SendCamCmdAddr(ad, "Z" & op)
                                PrevJoyZ = JoyZ 'only store the prev value if we actually send the new value
                                alreadysending = False
                            End If
                        End If
                        'JoyX = 1 + Int(JoyX * 99 / (255 - JoyDB * 2)) : JoyY = 1 + Int(JoyY * 99 / (255 - JoyDB * 2))
                        If (xpos >= 128) Then JoyX = 100 - joyconvert(255 - xpos) Else JoyX = joyconvert(xpos)
                        If (ypos >= 128) Then JoyY = 100 - joyconvert(255 - ypos) Else JoyY = joyconvert(ypos)
                        If (JoyX <> PrevJoyX) Or (JoyY <> PrevJoyY) Then
                            If alreadysending = False Then
                                op = Format(JoyX, "00") & Format(JoyY, "00")
                                alreadysending = True
                                SendCamCmdAddr(ad, "PTS" & op)
                                PrevJoyX = JoyX : PrevJoyY = JoyY
                                alreadysending = False
                            End If
                        End If
                        'mLog.Text = JoyX & ":" & JoyY & ":" & JoyZ
                    End If
                End If
            End If
            SerialInBuf(SerialInBufPtr) = x
            If SerialInBufPtr < 16 Then SerialInBufPtr = SerialInBufPtr + 1
        End While
    End Sub



    '#########################################################################################################################################################################
    ' TIMERS
    '#########################################################################################################################################################################

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Dim ta As Integer
        Dim aux As Integer
        Dim op As String
        Dim px As Integer, py As Integer

        'timer to wait for camera serial bytes
        If (gotrx > 0) Then gotrx = gotrx - 1

        'timer to autoconnect on startup
        If (startuptimer < 70) Then startuptimer = startuptimer + 1
        If startuptimer = 69 Then GroupBox1.Hide()
        If (startuptimer = 5) Then 'open com ports
            Timer1.Enabled = False
            If GetSetting("Atemswitcher", "Set", "Askprofile", True) = True Then
                Dim fd As OpenFileDialog = New OpenFileDialog()

                fd.Title = "Select presets file"
                fd.InitialDirectory = Globals.PresetFilePath
                fd.FileName = Globals.PresetFileName
                fd.Filter = "Presets files (*.aps)|*.aps|All files (*.*)|*.*"
                fd.FilterIndex = 1
                fd.RestoreDirectory = True

                If fd.ShowDialog() = DialogResult.OK Then
                    Globals.PresetFileName = Mid(fd.FileName, InStrRev(fd.FileName, "\") + 1) 'this is the filename without the path
                    Globals.PresetFilePath = Mid(fd.FileName, 1, InStrRev(fd.FileName, "\")) 'this is the filename without the path
                    SaveSetting("Atemswitcher", "Set", "PresetsFile", Globals.PresetFileName)
                    ReadPresetFile()
                End If
            End If
            GroupBox1.Show()
            GroupBox1.Left = 100 : GroupBox1.Top = 100
            If ctrlkey Then Label22.Text = "EMERGENCY STARTUP MODE" & vbCrLf Else Label22.Text = ""
            Label22.Text = Label22.Text & "Profile: " & Globals.PresetFileName & vbCrLf
            Label22.Text = Label22.Text & "Opening com port..."
            ComportOpen()
            If (SerialPort1.IsOpen) Then
                Label22.Text = Label22.Text & "Done" & vbCrLf
            Else
                Label22.Text = Label22.Text & "Fail" & vbCrLf
            End If
            GroupBox1.Refresh()
            'also make sure OBS is in the right profile
            websocket.Send("{""request-type"":""SetCurrentProfile"",""profile-name"":""SJN Youtube live"",""message-id"":""TEST1""}")
            'and open the multiviewer
            'websocket.Send("{""request-type"":""OpenProjector"",""type"":""Multiview"",""monitor"":-1,""message-id"":""TEST1""}")

            Timer1.Enabled = True
        End If
        If (startuptimer = 10) Then 'connect to atem
            websocket.Send("{""request-type"":""SetCurrentScene"",""scene-name"":""Cam1"",""message-id"":""TEST1""}")
            'websocket.Send("{""request-type"":""SetPreviewScene"",""scene-name"":""Cam2"",""message-id"":""TEST1""}")
            'setactive()
            nextpreview = 2
            transitionwait = 2 'will set preview to 2
            SetCaptionText()
            '        Timer1.Enabled = False
            '        Label22.Text = Label22.Text & "Connecting to Atem..."
            '        bConnect_Click(Nothing, Nothing)
            '        If (atemconnect = True) Then Label22.Text = Label22.Text & "Done" & vbCrLf Else Label22.Text = Label22.Text & "Fail" & vbCrLf
            '        GroupBox1.Refresh()
            '        Timer1.Enabled = True
            ReadMediaSources()
        End If
        If (startuptimer = 15) Then
            Timer1.Enabled = False
            'If atemconnect = False Then CamIgnore(1) = True : CamIgnore(2) = True : CamIgnore(3) = True : CamIgnore(4) = True 'if atem connect fails, don't try to connect cameras
            For ta = 1 To 4
                SendCamCmdAddr(ta, "O1") 'power on command
            Next
            Timer1.Enabled = True

        End If
        If (startuptimer = 30) Then 'get/set camera defaults
            Timer1.Enabled = False
            For ta = 1 To 4

                Label22.Text = Label22.Text & "Initialise Camera " & ta & "..."
                GroupBox1.Refresh()
                ReadbackCameraStates(ta)
                If (CamIgnore(ta) = False) Then Label22.Text = Label22.Text & "Done" & vbCrLf Else Label22.Text = Label22.Text & "Fail" & vbCrLf
                If (ctrlkey = False) Then
                    'temporarily don't move the cameras on power up
                    'SendCamCmdAddr(ta, "APC80008000")
                    'SendCamCmdAddr(ta, "AXZ555")
                End If
            Next
            Label22.Text = Label22.Text & "Initialise Camera 5..." 'send power on command to he2 and check we get a response
            GroupBox1.Refresh()
            If Globals.Cam5Dis = False Then
                If SendCamQuery(7, "aw_ptz?cmd=%23O1&res=1") <> "" Then CamIgnore(7) = False Else CamIgnore(7) = True
                If CamIgnore(7) = False Then Label22.Text = Label22.Text & "Done" & vbCrLf Else Label22.Text = Label22.Text & "Fail" & vbCrLf
            Else
                Label22.Text = Label22.Text & "Ignore" & vbCrLf
                CamIgnore(7) = True
            End If
            Label22.Text = Label22.Text & "Waiting for cameras to come out of standby..."
            Timer1.Enabled = True
            ShowCamValues()
            'also, get cam card info
            TextBoxCam1Rec.Text = CamRecStatus(SendCamQuery(1, "get_state"), 1)
            TextBoxCam2Rec.Text = CamRecStatus(SendCamQuery(2, "get_state"), 2)
            TextBoxCam3Rec.Text = CamRecStatus(SendCamQuery(3, "get_state"), 3)
            TextBoxCam4Rec.Text = CamRecStatus(SendCamQuery(4, "get_state"), 4)
            'also, set aux volume on Atem to normal
            'ExecuteLua("ATEMMixerAudioSetInputGain( 1,11, " & 0 & ")") 'set the aux input to 0db
            'also, set OBS to mix
            'BtnOBSOut.PerformClick()
            'also, set the Pip box
            'set up the parameters for the USK 1, we only need to do this once
            ExecuteLua("ATEMMixerMEKeySetKeyType(1,1,1,""DVE"")") 'dve mode
            ExecuteLua("ATEMMixerMEKeyDVESetEnabled(1,1,1,""TRUE"")") 'border
            ExecuteLua("ATEMMixerMEKeyFlySetOffsetX(1,1,1,7.8)")
            ExecuteLua("ATEMMixerMEKeyFlySetOffsetY(1,1,1,4.3)")
            ExecuteLua("ATEMMixerMEKeyFlySetStretchX(1,1,1,0.5)")
            ExecuteLua("ATEMMixerMEKeyFlySetStretchY(1,1,1,0.5)")
            ExecuteLua("ATEMMixerMEKeyDVESetOpacity(1,1,1,0.5)")
            ExecuteLua("ATEMMixerMEKeyDVESetWidthIn(1,1,1,0.2)")
            ExecuteLua("ATEMMixerMEKeyDVESetWidthOut(1,1,1,0.2)")
            ExecuteLua("ATEMMixerMEKeyDVESetLightAngle(1,1,1,51)")
            ExecuteLua("ATEMMixerMEKeyDVESetLightAltitude(1,1,1,49)")
            ExecuteLua("ATEMMixerMEKeyDVESetShadowEnabled(1,1,1,""TRUE"")")
            ExecuteLua("ATEMMixerMEKeySetFillSource(1,1,1,2)") 'default to words input

        End If

        'check for disabled cams and show on buttons
        ComCheckTimer = ComCheckTimer + 1
        If (ComCheckTimer > 20) Then
            ComCheckTimer = 0
            If Globals.Cam1Dis Then 'user disabled
                CamIgnore(1) = True
                BtnCam1.Enabled = False 'lock out the button
                BtnCam1.Text = "CAM1 Disabled"
            Else 'not user disabled but check for comm loss
                If CamIgnore(1) Then BtnCam1.Text = "CAM1 com-fail" Else BtnCam1.Text = "CAM1"
            End If

            If Globals.Cam2Dis Then 'user disabled
                CamIgnore(2) = True
                BtnCam2.Enabled = False 'lock out the button
                BtnCam2.Text = "CAM2 Disabled"
            Else 'not user disabled but check for comm loss
                If CamIgnore(2) Then BtnCam2.Text = "CAM2 com-fail" Else BtnCam2.Text = "CAM2"
            End If

            If Globals.Cam3Dis Then 'user disabled
                CamIgnore(3) = True
                BtnCam3.Enabled = False 'lock out the button
                BtnCam3.Text = "CAM3 Disabled"
            Else 'not user disabled but check for comm loss
                If CamIgnore(3) Then BtnCam3.Text = "CAM3 com-fail" Else BtnCam3.Text = "CAM3"
            End If

            If Globals.Cam4Dis Then 'user disabled
                CamIgnore(4) = True
                BtnCam4.Enabled = False 'lock out the button
                BtnCam4.Text = "CAM4 Disabled"
            Else 'not user disabled but check for comm loss
                If CamIgnore(4) Then BtnCam4.Text = "CAM4 com-fail" Else BtnCam4.Text = "CAM4"
            End If

            If Globals.Cam5Dis Then 'user disabled
                CamIgnore(5) = True
                BtnInp3.Enabled = False 'lock out the button
                BtnInp3.Text = "CAM5 Disabled"
            Else 'not user disabled but check for comm loss
                If CamIgnore(5) Then BtnInp3.Text = "CAM5 com-fail" Else BtnInp3.Text = "CAM5"
            End If
        End If

        'timer to stop cam movements once cam is off air
        'If DelayStop > 0 And liveaddr <> DelayAddr Then
        If DelayStop > 0 Then
            DelayStop = DelayStop - 1
            If DelayStop = 0 Then 'we will stop everything except the live cam, just in case
                If liveaddr <> 1 And addr <> 1 Then
                    SendCamCmdAddr(1, "PTS5050")
                    SendCamCmdAddr(1, "Z50")
                End If
                If liveaddr <> 2 And addr <> 2 Then
                    SendCamCmdAddr(2, "PTS5050")
                    SendCamCmdAddr(2, "Z50")
                End If
                If liveaddr <> 3 And addr <> 3 Then
                    SendCamCmdAddr(3, "PTS5050")
                    SendCamCmdAddr(3, "Z50")
                End If
                If liveaddr <> 4 And addr <> 4 Then
                    SendCamCmdAddr(4, "PTS5050")
                    SendCamCmdAddr(4, "Z50")
                End If
                'PendingZoom = 0 : PendingPan = 0 : PendingTilt = 0
            End If
        End If

        'timer to change preview after transition
        If transitionwait > 0 Then
            transitionwait = transitionwait - 1
            If transitionwait = 0 Then
                addr = nextpreview
                setactive()
                If MediaPlayerWasActive Then 'if we just transitioned away from media player, go to the next clip
                    MediaPlayerWasActive = False
                    BtnMNext.PerformClick()
                End If
            End If
        End If

        If WebsocketReinitTimer > 0 Then
            WebsocketReinitTimer = WebsocketReinitTimer - 1
        End If

        'timer to check for double presses of cut and fade, and also check that the live/prev cams match what we think they are
        If CutLockoutTimer > 0 Then
            CutLockoutTimer = CutLockoutTimer - 1
            If CutLockoutTimer = 0 Then
                'check that the preview and main outs are correct
                'If liveaddr >= 1 And liveaddr <= 4 Then
                'RequestLua("ATEMMixerMEGetProgramInput(1,1)") 'read prog input back from atem (comes back as <response>INTEGER:6</response>)
                'px = InStr(LuaReturn, "INTEGER:")
                'If (px <> 0) Then py = Val(Mid(LuaReturn, px + 8, 1))
                'If py <> liveaddr + 5 Then
                ''error!!! the live cam setting is wrong
                'ExecuteLua("ATEMMixerMESetProgramInput( 1,1," & liveaddr + 5 & " )")
                'Dim objWriter As New System.IO.StreamWriter("vblog.txt", True)
                'objWriter.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & " " & "AtemLive:" & py - 5 & " VBLive:" & liveaddr)
                'objWriter.Close()
                'End If
                'End If

                If addr >= 1 And addr <= 4 Then
                    'mLog.Text = LuaReturn
                    RequestLua("ATEMMixerMEGetPreviewInput(1,1)")
                    'mLog.Text = LuaReturn
                    px = InStr(LuaReturn, "INTEGER:")
                    If (px <> 0) Then py = Val(Mid(LuaReturn, px + 8, 1))
                    'If py <> addr + 5 Then
                    '    'error!!! the live cam setting is wrong
                    '    ExecuteLua("ATEMMixerMESetPreviewInput( 1,1," & addr + 5 & " )")
                    '    Dim objWriter As New System.IO.StreamWriter("vblog.txt", True)
                    '    objWriter.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & " " & "AtemPrev:" & py - '5 & " VBPrev:" & liveaddr)
                    '    objWriter.Close()
                    'End If
                End If
            End If
        End If

        'pending zoom - check if reached desired pos or no longer live
        If PendingZoom <> 0 Or PendingPan <> 0 Or PendingTilt <> 0 Then
            If (liveaddr <> PzAddr) And DelayStop = 0 Then
                SendCamCmdAddr(PzAddr, "Z50")
                SendCamCmdAddr(PzAddr, "PTS5050")
                PendingZoom = 0 'cam is no longer live, we can stop
                PendingPan = 0
                PendingTilt = 0
            End If
            PzTimer = PzTimer + 1
            If PzTimer >= 3 And CamCmdPending = False Then 'don't do it too often
                PzTimer = 0

                CamCmdPending = True 'in case this routine re-enters before we finish
                op = SendCamCmdAddr(PzAddr, "GZ") 'get zoom position
                ta = Val("&H" & Mid(op, 3))

                op = SendCamCmdAddr(PzAddr, "APC") 'get current pt position
                op = Mid(op, 4)
                px = Convert.ToInt32(Mid(op, 1, 4), 16)
                py = Convert.ToInt32(Mid(op, 5, 4), 16)
                CamCmdPending = False

                '----zoom is currently active
                If PendingZoom <> 0 Then
                    mLog.Text = "Z:" & ta & ">" & PendingZoom
                    If PzDir = -1 Then
                        'If (ta < PendingZoom) Then SendCamCmdAddr(PzAddr, "Z50") : PendingZoom = 0
                        If (ta <= PendingZoom) Then
                            SendCamCmdAddr(PzAddr, "Z50") : PendingZoom = 0
                            If Math.Abs(px - PendingPan) < 100 And Math.Abs(py - PendingTilt) < 100 Then
                                SendCamCmdAddr(PzAddr, "PTS5050") : PendingPan = 0 : PendingTilt = 0 'stop pt only if small distance still to go
                            End If
                        End If
                    Else
                        'If (ta > PendingZoom) Then SendCamCmdAddr(PzAddr, "Z50") : PendingZoom = 0
                        If (ta >= PendingZoom) Then
                            SendCamCmdAddr(PzAddr, "Z50") : PendingZoom = 0
                            If Math.Abs(px - PendingPan) < 100 And Math.Abs(py - PendingTilt) < 100 Then
                                SendCamCmdAddr(PzAddr, "PTS5050") : PendingPan = 0 : PendingTilt = 0 'stop pt only if small distance still to go
                            End If
                        End If
                    End If
                End If
                '----pan/tilt is currently active
                If PendingPan <> 0 Or PendingTilt <> 0 Then
                    If PendingZoom = 0 Then mLog.Text = "Z:STOP"
                    mLog.Text = mLog.Text & vbCrLf & "P:" & px & ">" & PendingPan
                    mLog.Text = mLog.Text & vbCrLf & "T:" & py & ">" & PendingTilt
                    If PendingPan <> 0 Then
                        If PpDir = -1 Then
                            If (px <= PendingPan) Then
                                SendCamCmdAddr(PzAddr, "P50") : PendingPan = 0
                                If Math.Abs(ta - PendingZoom) < 100 And Math.Abs(py - PendingTilt) < 100 Then
                                    SendCamCmdAddr(PzAddr, "T50") : PendingTilt = 0 'stop t only if small distance still to go
                                    SendCamCmdAddr(PzAddr, "Z50") : PendingZoom = 0 'stop z only if small distance still to go
                                End If
                            End If
                        Else
                            If (px >= PendingPan) Then
                                SendCamCmdAddr(PzAddr, "P50") : PendingPan = 0
                                If Math.Abs(ta - PendingZoom) < 100 And Math.Abs(py - PendingTilt) < 100 Then
                                    SendCamCmdAddr(PzAddr, "T50") : PendingTilt = 0 'stop t only if small distance still to go
                                    SendCamCmdAddr(PzAddr, "Z50") : PendingZoom = 0 'stop z only if small distance still to go
                                End If
                            End If
                        End If

                    End If
                    If PendingTilt <> 0 Then
                        If PtDir = -1 Then
                            If (py <= PendingTilt) Then
                                SendCamCmdAddr(PzAddr, "T50") : PendingTilt = 0
                                If Math.Abs(ta - PendingZoom) < 100 And Math.Abs(px - PendingPan) < 100 Then
                                    SendCamCmdAddr(PzAddr, "P50") : PendingPan = 0 'stop t only if small distance still to go
                                    SendCamCmdAddr(PzAddr, "Z50") : PendingZoom = 0 'stop z only if small distance still to go
                                End If
                            End If

                        Else
                            If (py >= PendingTilt) Then
                                SendCamCmdAddr(PzAddr, "T50") : PendingTilt = 0
                                If Math.Abs(ta - PendingZoom) < 100 And Math.Abs(px - PendingPan) < 100 Then
                                    SendCamCmdAddr(PzAddr, "P50") : PendingPan = 0 'stop t only if small distance still to go
                                    SendCamCmdAddr(PzAddr, "Z50") : PendingZoom = 0 'stop z only if small distance still to go
                                End If
                            End If

                        End If
                        'If (PendingPan = 0 Or PendingTilt = 0) And PendingZoom <> 0 And Math.Abs(ta - PendingZoom) < 100 Then
                        '    SendCamCmdAddr(PzAddr, "Z50") : PendingZoom = 0
                        '    SendCamCmdAddr(PzAddr, "PTS5050") : PendingPan = 0 : PendingTilt = 0
                        'End If
                        'If PendingZoom = 0 And Math.Abs(py - PendingTilt) < 100 Then
                        '    SendCamCmdAddr(PzAddr, "T50") : PendingTilt = 0
                        'End If
                    End If

                End If

            End If
        End If

        'timer to load next shot if in auto mode
        If AutoWait > 0 Then
            AutoWait = AutoWait - 1
            If (AutoWait = 0) And (AutoSongMode = True Or AutoSpeechMode = True) Then
                NextAutoShot()
            End If
        End If

        'process keyhit from cam controller
        If (KeyHit) Then
            KeyHit = False
            'this code sets the preview button from the cam controller
            'If LastKey = 11 Then BtnCam1_Click(BtnCam1, Nothing)
            'If LastKey = 12 Then BtnCam1_Click(BtnCam2, Nothing)
            'If LastKey = 13 Then BtnCam1_Click(BtnCam3, Nothing)
            'If LastKey = 14 Then BtnCam1_Click(BtnCam4, Nothing)
            'If LastKey = 15 Then BtnInp_Click(BtnInp2, Nothing)

            'this code set the projector output
            aux = 0
            If LastKey = 11 Then aux = 6
            If LastKey = 12 Then aux = 7
            If LastKey = 13 Then aux = 8
            If LastKey = 14 Then aux = 2
            If LastKey = 15 Then aux = 5
            If LastKey = 16 Then BtnCut_Click(BtnCut, Nothing)
            If aux <> 0 Then ExecuteLua("ATEMMixerAUXSetInput( 1,2," & aux & " )") ' 

        End If

        'program close safety timer
        If (ProgCloseTimer > 0) Then
            ProgCloseTimer = ProgCloseTimer - 1
            If (ProgCloseTimer = 0) Then Button1.ForeColor = Color.Black
        End If
        LabelProfile.Text = Globals.PresetFileName

        'cam shutdown timer on prog close
        If (ShutDownTimer > 0) Then
            ShutDownTimer = ShutDownTimer + 1
        End If
        If (ShutDownTimer = 2) And GetSetting("Atemswitcher", "Set", "CamStandby", True) = True Then 'put cams into standby
            Timer1.Enabled = False
            GroupBox1.Show()
            GroupBox1.Left = 100 : GroupBox1.Top = 100
            Label22.Text = "Closing down cameras..." & vbCrLf
            SendCamCmdAddr(1, "O0")
            SendCamCmdAddr(2, "O0")
            SendCamCmdAddr(3, "O0")
            SendCamCmdAddr(4, "O0")
            SendCamCmdAddr(7, "O0")
            'SendCamCmdAddr(1, "APC8000FFFF") 'central position lens fully down
            'SendCamCmdAddr(2, "APC8000FFFF")
            'SendCamCmdAddr(3, "APC8000FFFF")
            'SendCamCmdAddr(4, "APC8000FFFF")
            GroupBox1.Refresh()
            Timer1.Enabled = True
        End If
        If ShutDownTimer = 30 Then Label22.Text = Label22.Text & "Exit application..." & vbCrLf
        If ShutDownTimer = 50 Then Application.Exit()

        'audio fades for black output
        If AudioFade = -1 Then 'fading out the main sound, going to media player
            AudioLevel = AudioLevel - 3
            If (AudioLevel < -60) Then
                AudioLevel = -60
                AudioFade = 0
            End If
            ExecuteLua("ATEMMixerAudioSetInputGain( 1,11, " & AudioLevel & ")") 'fade the aux input (main sound)
        End If
        If AudioFade = 1 Then 'fading back to main sound from media
            AudioLevel = AudioLevel + 3
            If (AudioLevel > 0) Then
                AudioLevel = 0
                AudioFade = 0
            End If
            ExecuteLua("ATEMMixerAudioSetInputGain( 1,11, " & AudioLevel & ")")
        End If


    End Sub

    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        'ticks every 1 sec
        'check OBS status (returns in websocket handler routine elsewhere)
        websocket.Send("{""request-type"":""GetStreamingStatus"",""message-id"":""OBSSTATE""}")

        If (PrevAux3 <> 0) Then 'we are playing a clip on obs
            websocket.Send("{""request-type"":""GetCurrentScene"",""message-id"":""OBSSCENE""}")
            If (ClipRemainTime > 0) Then ClipRemainTime = ClipRemainTime - 1
            Dim hr As Integer = Math.Floor(ClipRemainTime / 3600)
            Dim min As Integer = (Math.Floor(ClipRemainTime / 60)) Mod 60
            Dim sec As Integer = ClipRemainTime Mod 60
            TextPlayerTime.Text = hr.ToString("00") & ":" & min.ToString("00") & ":" & sec.ToString("00")
        End If

        'check if cameras recording
        CamRecStatusTimer = CamRecStatusTimer + 1
        If CamRecStatusTimer > 60 Then CamRecStatusTimer = 0
        If CamRec(1) Or CamRecStatusTimer = 60 Then TextBoxCam1Rec.Text = CamRecStatus(SendCamQuery(1, "get_state"), 1)
        If CamRec(2) Or CamRecStatusTimer = 60 Then TextBoxCam2Rec.Text = CamRecStatus(SendCamQuery(2, "get_state"), 2)
        If CamRec(3) Or CamRecStatusTimer = 60 Then TextBoxCam3Rec.Text = CamRecStatus(SendCamQuery(3, "get_state"), 3)
        If CamRec(4) Or CamRecStatusTimer = 60 Then TextBoxCam4Rec.Text = CamRecStatus(SendCamQuery(4, "get_state"), 4)


    End Sub



    Private Sub TextPreacherName_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextPreacherName.TextChanged

    End Sub
    Private Sub Label26_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LabelCap2.Click

    End Sub



    Private Sub ShapeContainer1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ShapeContainer1.Load

    End Sub

End Class
