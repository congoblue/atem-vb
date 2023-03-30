Public Class Form3
    
    Private Sub Form3_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        'If Screen.AllScreens.Count > 1 Then
        'Me.Top = (From scr In Screen.AllScreens Where Not scr.Primary)(0).WorkingArea.Top + 100
        'Me.Left = (From scr In Screen.AllScreens Where Not scr.Primary)(0).WorkingArea.Left + 100
        'End If

        TextBox1.Text = Globals.EditPresetCaption
        TextBox1.SelectAll()
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