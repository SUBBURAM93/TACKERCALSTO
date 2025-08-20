Class Dashboard
    Inherits Page

    Public Sub New()
        InitializeComponent()
        Try
            Me.DataContext = New DashboardViewModel()
        Catch ex As Exception
            MessageBox.Show("Constructor error: " & ex.Message)
        End Try
    End Sub

End Class
