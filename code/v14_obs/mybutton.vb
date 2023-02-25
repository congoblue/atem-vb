Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D

Partial Public Class MyButton
    Inherits System.Windows.Forms.Button
    Private pushed As Boolean

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        Dim offset As Integer
        'MyBase.OnPaint(e) 'draw default appearance
        If Not pushed Then
            If Me.BackColor = Color.Green Then
                e.Graphics.DrawImage(My.Resources.greenBtn, ClientRectangle.Top, ClientRectangle.Left, ClientRectangle.Width, ClientRectangle.Height + 3)
            ElseIf Me.BackColor = Color.Red Then
                e.Graphics.DrawImage(My.Resources.redBtn, ClientRectangle.Top, ClientRectangle.Left, ClientRectangle.Width, ClientRectangle.Height + 3)
            ElseIf Me.BackColor = Color.Yellow Then
                e.Graphics.DrawImage(My.Resources.RedGrnBtn, ClientRectangle.Top, ClientRectangle.Left, ClientRectangle.Width, ClientRectangle.Height + 3)
            ElseIf Me.BackColor = Color.Orange Then
                e.Graphics.DrawImage(My.Resources.yellowBtn, ClientRectangle.Top, ClientRectangle.Left, ClientRectangle.Width, ClientRectangle.Height + 3)
            Else
                e.Graphics.DrawImage(My.Resources.WhiteBtn, ClientRectangle.Top, ClientRectangle.Left, ClientRectangle.Width, ClientRectangle.Height + 3)
            End If
            offset = 0
        Else
            If Me.BackColor = Color.Green Then
                e.Graphics.DrawImage(My.Resources.greenBtnpush, ClientRectangle.Top, ClientRectangle.Left, ClientRectangle.Width, ClientRectangle.Height + 3)
            ElseIf Me.BackColor = Color.Red Then
                e.Graphics.DrawImage(My.Resources.redBtnpush, ClientRectangle.Top, ClientRectangle.Left, ClientRectangle.Width, ClientRectangle.Height + 3)
            ElseIf Me.BackColor = Color.Yellow Then
                e.Graphics.DrawImage(My.Resources.RedGrnBtn, ClientRectangle.Top, ClientRectangle.Left, ClientRectangle.Width, ClientRectangle.Height + 3)
            ElseIf Me.BackColor = Color.Orange Then
                e.Graphics.DrawImage(My.Resources.yellowBtn, ClientRectangle.Top, ClientRectangle.Left, ClientRectangle.Width, ClientRectangle.Height + 3)
            Else
                e.Graphics.DrawImage(My.Resources.WhiteBtnPush, ClientRectangle.Top, ClientRectangle.Left, ClientRectangle.Width, ClientRectangle.Height + 3)
            End If
            offset = 2
        End If
        'if this string is set to something other than "", show Auto status on the button
        'If Me.AccessibleDescription <> "" Then 
        Dim flags As TextFormatFlags = TextFormatFlags.WordBreak Or TextFormatFlags.TextBoxControl Or TextFormatFlags.VerticalCenter Or TextFormatFlags.HorizontalCenter Or TextFormatFlags.NoPadding
        'the textboxcontrol flag causes it to split long words
        'If pushed Then Me.ClientSize = New Size(PreferredSize.Width - 2, PreferredSize.Height - 2)
        'Me.ClientSize = New Size(PreferredSize.Width + 2, PreferredSize.Height + 2)
        TextRenderer.DrawText(e.Graphics, MyBase.Text, Me.Font, ClientRectangle, Me.ForeColor, flags)

        If Me.AccessibleDescription <> "" Then
            flags = TextFormatFlags.Bottom Or TextFormatFlags.Right
            TextRenderer.DrawText(e.Graphics, "A", Me.Font, ClientRectangle, Color.Red, flags)
        End If

    End Sub


    Private Sub MyButton_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
        pushed = True
        Me.Invalidate()
    End Sub

    Private Sub MyButton_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        pushed = False
        Me.Invalidate()
    End Sub
End Class
