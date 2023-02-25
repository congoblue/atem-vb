Public Class Form3
    Sub SetContentButtons()
        BtnContent1.BackColor = Color.White
        BtnContent2.BackColor = Color.White
        BtnContent3.BackColor = Color.White
        BtnContent4.BackColor = Color.White
        BtnContent5.BackColor = Color.White
        If Globals.EditPresetContent = 1 Then BtnContent1.BackColor = Color.Green
        If Globals.EditPresetContent = 2 Then BtnContent2.BackColor = Color.Green
        If Globals.EditPresetContent = 3 Then BtnContent3.BackColor = Color.Green
        If Globals.EditPresetContent = 4 Then BtnContent4.BackColor = Color.Green
        If Globals.EditPresetContent = 5 Then BtnContent5.BackColor = Color.Green
    End Sub
    Sub SetShotButtons()
        BtnShot1.BackColor = Color.White
        BtnShot2.BackColor = Color.White
        BtnShot3.BackColor = Color.White
        BtnShot4.BackColor = Color.White
        BtnShot5.BackColor = Color.White
        BtnShot6.BackColor = Color.White
        If Globals.EditPresetSize = 1 Then BtnShot1.BackColor = Color.Green
        If Globals.EditPresetSize = 2 Then BtnShot2.BackColor = Color.Green
        If Globals.EditPresetSize = 3 Then BtnShot3.BackColor = Color.Green
        If Globals.EditPresetSize = 4 Then BtnShot4.BackColor = Color.Green
        If Globals.EditPresetSize = 5 Then BtnShot5.BackColor = Color.Green
        If Globals.EditPresetSize = 6 Then BtnShot6.BackColor = Color.Green
    End Sub
    Sub SetAutoButton()
        If Globals.EditPresetAuto = True Then BtnAuto.BackColor = Color.Green Else BtnAuto.BackColor = Color.White
    End Sub
    Private Sub Form3_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        If Screen.AllScreens.Count > 1 Then
            Me.Top = (From scr In Screen.AllScreens Where Not scr.Primary)(0).WorkingArea.Top + 100
            Me.Left = (From scr In Screen.AllScreens Where Not scr.Primary)(0).WorkingArea.Left + 100
        End If

        TextBox1.Text = Globals.EditPresetCaption
        SetContentButtons()
        SetShotButtons()
        SetAutoButton()
        TextBox1.SelectAll()
    End Sub

    Private Sub BtnShot1_Click(sender As System.Object, e As System.EventArgs) Handles BtnShot1.Click, BtnShot2.Click, BtnShot3.Click, BtnShot4.Click, BtnShot5.Click, BtnShot6.Click
        Dim index As Integer
        index = Val(Mid(sender.name, 8))
        Globals.EditPresetSize = index
        SetShotButtons()
    End Sub

    Private Sub BtnContent1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnContent1.Click, BtnContent2.Click, BtnContent3.Click, BtnContent4.Click, BtnContent5.Click
        Dim index As Integer
        index = Val(Mid(sender.name, 11))
        Globals.EditPresetContent = index
        SetContentButtons()
    End Sub

    Private Sub BtnAuto_Click(sender As System.Object, e As System.EventArgs) Handles BtnAuto.Click
        If Globals.EditPresetAuto = True Then Globals.EditPresetAuto = False Else Globals.EditPresetAuto = True
        SetAutoButton()
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Globals.EditPresetCaption = TextBox1.Text
        Me.Close()
    End Sub

    Private Sub TextBox1_TextChanged(sender As System.Object, e As System.EventArgs) Handles TextBox1.TextChanged
        If Globals.EditPresetIndex = 1 Then MainForm.BtnPreset1.Text = TextBox1.Text
        If Globals.EditPresetIndex = 2 Then MainForm.BtnPreset2.Text = TextBox1.Text
        If Globals.EditPresetIndex = 3 Then MainForm.BtnPreset3.Text = TextBox1.Text
        If Globals.EditPresetIndex = 4 Then MainForm.BtnPreset4.Text = TextBox1.Text
        If Globals.EditPresetIndex = 5 Then MainForm.BtnPreset5.Text = TextBox1.Text
        If Globals.EditPresetIndex = 6 Then MainForm.BtnPreset6.Text = TextBox1.Text
        If Globals.EditPresetIndex = 7 Then MainForm.BtnPreset7.Text = TextBox1.Text
        If Globals.EditPresetIndex = 8 Then MainForm.BtnPreset8.Text = TextBox1.Text
        If Globals.EditPresetIndex = 9 Then MainForm.BtnPreset9.Text = TextBox1.Text
        If Globals.EditPresetIndex = 10 Then MainForm.BtnPreset10.Text = TextBox1.Text
        If Globals.EditPresetIndex = 11 Then MainForm.BtnPreset11.Text = TextBox1.Text
        If Globals.EditPresetIndex = 12 Then MainForm.BtnPreset12.Text = TextBox1.Text
        If Globals.EditPresetIndex = 13 Then MainForm.BtnPreset13.Text = TextBox1.Text
        If Globals.EditPresetIndex = 14 Then MainForm.BtnPreset14.Text = TextBox1.Text
        If Globals.EditPresetIndex = 15 Then MainForm.BtnPreset15.Text = TextBox1.Text
        If Globals.EditPresetIndex = 16 Then MainForm.BtnPreset16.Text = TextBox1.Text
    End Sub
End Class