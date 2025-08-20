Class Job
    Inherits Page

    Private vm As New MainViewModel()

    Public Sub New()
        InitializeComponent()
        DataContext = vm
    End Sub

End Class
