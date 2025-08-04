Class Planning
    Inherits Page

    Public Sub New()
        InitializeComponent()
        Try
            Me.DataContext = New PlanningViewModel()
        Catch ex As Exception
            MessageBox.Show("Constructor error: " & ex.Message)
        End Try
    End Sub

End Class
