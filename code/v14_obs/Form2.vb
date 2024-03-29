﻿Imports System.IO.Ports

Public Class Form2

    Private Sub Form2_Disposed(sender As Object, e As System.EventArgs) Handles Me.Disposed

        MainForm.ComportOpen()
    End Sub

    Private Sub Form2_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        SaveSetting("Atemswitcher", "CamIP", "1", TextBox1.Text)
        SaveSetting("Atemswitcher", "CamIP", "2", TextBox2.Text)
        SaveSetting("Atemswitcher", "CamIP", "3", TextBox3.Text)
        SaveSetting("Atemswitcher", "CamIP", "4", TextBox4.Text)
        SaveSetting("Atemswitcher", "CamIP", "5", TextBox7.Text)

        SaveSetting("Atemswitcher", "Set", "Tally", CheckBoxTally.Checked)
        SaveSetting("Atemswitcher", "Set", "Autoswap", CheckBoxAutoSwap.Checked)
        SaveSetting("Atemswitcher", "Set", "CamStandby", CheckBoxStandby.Checked)
        SaveSetting("Atemswitcher", "Set", "Askprofile", CheckBoxProfile.Checked)
        SaveSetting("Atemswitcher", "Set", "Caminvert1", CheckBoxInvert1.Checked)
        SaveSetting("Atemswitcher", "Set", "Caminvert2", CheckBoxInvert2.Checked)
        SaveSetting("Atemswitcher", "Set", "Caminvert3", CheckBoxInvert3.Checked)
        SaveSetting("Atemswitcher", "Set", "Caminvert4", CheckBoxInvert4.Checked)
        SaveSetting("Atemswitcher", "Set", "PresetsFile", TextBox5.Text)
        SaveSetting("Atemswitcher", "Set", "PresetsPath", TextBox6.Text)

        SaveSetting("Atemswitcher", "Set", "Cam1Dis", CheckBoxCam1Dis.Checked)
        SaveSetting("Atemswitcher", "Set", "Cam2Dis", CheckBoxCam2Dis.Checked)
        SaveSetting("Atemswitcher", "Set", "Cam3Dis", CheckBoxCam3Dis.Checked)
        SaveSetting("Atemswitcher", "Set", "Cam4Dis", CheckBoxCam4Dis.Checked)
        SaveSetting("Atemswitcher", "Set", "Cam5Dis", CheckBoxCam5Dis.Checked)

        SaveSetting("Atemswitcher", "Cliptime", "1", TextBox8.Text)
        SaveSetting("Atemswitcher", "Cliptime", "2", TextBox9.Text)
        SaveSetting("Atemswitcher", "Cliptime", "3", TextBox10.Text)
        SaveSetting("Atemswitcher", "Cliptime", "4", TextBox11.Text)
        Globals.PresetFileName = TextBox5.Text
        Globals.CamIP(1) = TextBox1.Text
        Globals.CamIP(2) = TextBox2.Text
        Globals.CamIP(3) = TextBox3.Text
        Globals.CamIP(4) = TextBox4.Text
        Globals.CamIP(7) = TextBox7.Text

        MainForm.ReadPresetFile()
        If CheckBoxCam5Dis.Checked Then Globals.Cam5Dis = True Else Globals.Cam5Dis = False

    End Sub

    Private Sub Form2_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        If Screen.AllScreens.Count > 1 Then
            Me.Top = (From scr In Screen.AllScreens Where Not scr.Primary)(0).WorkingArea.Top + 50
            Me.Left = (From scr In Screen.AllScreens Where Not scr.Primary)(0).WorkingArea.Left + 50
        End If

        Dim i As Integer
        For Each s In SerialPort.GetPortNames()
            ComboBox2.Items.Add(s)
        Next s
        TextBox1.Text = (GetSetting("Atemswitcher", "CamIP", "1", "192.168.1.144"))
        TextBox2.Text = (GetSetting("Atemswitcher", "CamIP", "2", "192.168.1.145"))
        TextBox3.Text = (GetSetting("Atemswitcher", "CamIP", "3", "192.168.1.146"))
        TextBox4.Text = (GetSetting("Atemswitcher", "CamIP", "4", "192.168.1.147"))
        TextBox7.Text = (GetSetting("Atemswitcher", "CamIP", "5", "192.168.1.95"))

        TextBox5.Text = Globals.PresetFileName
        TextBox6.Text = Globals.PresetFilePath
        i = ComboBox2.FindString(GetSetting("Atemswitcher", "Comm", "2", "COM2"))
        ComboBox2.SelectedIndex = i
        If Globals.TallyMode Then CheckBoxTally.Checked = True
        If Globals.AutoSwap Then CheckBoxAutoSwap.Checked = True
        If GetSetting("Atemswitcher", "Set", "Askprofile", True) = True Then CheckBoxProfile.Checked = True
        If GetSetting("Atemswitcher", "Set", "CamStandby", True) = True Then CheckBoxStandby.Checked = True

        If GetSetting("Atemswitcher", "Set", "Cam1Dis", False) = True Then CheckBoxCam1Dis.Checked = True Else CheckBoxCam1Dis.Checked = False
        If GetSetting("Atemswitcher", "Set", "Cam2Dis", False) = True Then CheckBoxCam2Dis.Checked = True Else CheckBoxCam2Dis.Checked = False
        If GetSetting("Atemswitcher", "Set", "Cam3Dis", False) = True Then CheckBoxCam3Dis.Checked = True Else CheckBoxCam3Dis.Checked = False
        If GetSetting("Atemswitcher", "Set", "Cam4Dis", False) = True Then CheckBoxCam4Dis.Checked = True Else CheckBoxCam4Dis.Checked = False
        If GetSetting("Atemswitcher", "Set", "Cam5Dis", False) = True Then CheckBoxCam5Dis.Checked = True Else CheckBoxCam5Dis.Checked = False

        If Globals.CamInvert(1) Then CheckBoxInvert1.Checked = True
        If Globals.CamInvert(2) Then CheckBoxInvert2.Checked = True
        If Globals.CamInvert(3) Then CheckBoxInvert3.Checked = True
        If Globals.CamInvert(4) Then CheckBoxInvert4.Checked = True
        TextBox8.Text = (GetSetting("Atemswitcher", "Cliptime", "1", "60"))
        TextBox9.Text = (GetSetting("Atemswitcher", "Cliptime", "2", "60"))
        TextBox10.Text = (GetSetting("Atemswitcher", "Cliptime", "3", "60"))
        TextBox11.Text = (GetSetting("Atemswitcher", "Cliptime", "4", "60"))

        If Globals.CamStatus(1) = True Then Label8.Text = "FAIL"
        If Globals.CamStatus(2) = True Then Label9.Text = "FAIL"
        If Globals.CamStatus(3) = True Then Label10.Text = "FAIL"
        If Globals.CamStatus(4) = True Then Label11.Text = "FAIL"
        If Globals.CamStatus(7) = True Then Label13.Text = "FAIL"
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Me.Close()
    End Sub


    Private Sub ComboBox2_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ComboBox2.SelectedIndexChanged
        SaveSetting("Atemswitcher", "Comm", "2", ComboBox2.SelectedItem)
    End Sub

    Private Sub CheckBoxAutoSwap_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBoxAutoSwap.CheckedChanged
        If CheckBoxAutoSwap.Checked Then Globals.AutoSwap = True Else Globals.AutoSwap = False
    End Sub

    Private Sub CheckBoxTally_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBoxTally.CheckedChanged
        If CheckBoxTally.Checked Then Globals.TallyMode = True Else Globals.TallyMode = False
    End Sub

    Private Sub CheckBoxInvert1_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBoxInvert1.CheckedChanged
        If CheckBoxInvert1.Checked Then Globals.CamInvert(1) = True Else Globals.CamInvert(1) = False
    End Sub

    Private Sub CheckBoxInvert2_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBoxInvert2.CheckedChanged
        If CheckBoxInvert2.Checked Then Globals.CamInvert(2) = True Else Globals.CamInvert(2) = False
    End Sub

    Private Sub CheckBoxInvert3_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBoxInvert3.CheckedChanged
        If CheckBoxInvert3.Checked Then Globals.CamInvert(3) = True Else Globals.CamInvert(3) = False
    End Sub

    Private Sub CheckBoxInvert4_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBoxInvert4.CheckedChanged
        If CheckBoxInvert4.Checked Then Globals.CamInvert(4) = True Else Globals.CamInvert(4) = False
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim fd As OpenFileDialog = New OpenFileDialog()

        fd.Title = "Select presets file"
        fd.InitialDirectory = Globals.PresetFilePath
        fd.Filter = "Presets files (*.aps)|*.aps|All files (*.*)|*.*"
        fd.FilterIndex = 1
        fd.RestoreDirectory = True

        If fd.ShowDialog() = DialogResult.OK Then
            Globals.PresetFileName = fd.SafeFileName 'this is the filename without the path
            TextBox5.Text = Globals.PresetFileName
        End If
    End Sub

    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click

        Dim dialog As New FolderBrowserDialog()
        dialog.RootFolder = Environment.SpecialFolder.MyComputer
        dialog.SelectedPath = Globals.PresetFilePath
        dialog.Description = "Select folder for presets file"
        If dialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Globals.PresetFilePath = dialog.SelectedPath & "\"
            TextBox6.Text = Globals.PresetFilePath
        End If
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        MainForm.SendCamQueryNoResponse(7, "aw_cam?cmd=DPG")
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        MainForm.SendCamQueryNoResponse(7, "aw_cam?cmd=DUS:1")
    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        MainForm.SendCamQueryNoResponse(7, "aw_cam?cmd=DIT")
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        MainForm.SendCamQueryNoResponse(7, "aw_cam?cmd=DUP")
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        MainForm.SendCamQueryNoResponse(7, "aw_cam?cmd=DDW")
    End Sub

    Private Sub CheckBoxCam5Dis_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBoxCam5Dis.CheckedChanged
        If CheckBoxCam1Dis.Checked Then Globals.Cam1Dis = True Else Globals.Cam1Dis = False
        If CheckBoxCam2Dis.Checked Then Globals.Cam2Dis = True Else Globals.Cam2Dis = False
        If CheckBoxCam3Dis.Checked Then Globals.Cam3Dis = True Else Globals.Cam3Dis = False
        If CheckBoxCam4Dis.Checked Then Globals.Cam4Dis = True Else Globals.Cam4Dis = False
        If CheckBoxCam5Dis.Checked Then Globals.Cam5Dis = True Else Globals.Cam5Dis = False
    End Sub
End Class