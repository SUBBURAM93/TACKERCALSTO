Public Class RelayCommand
    Implements ICommand

    Private ReadOnly _execute As Action
    Private ReadOnly _canExecute As Func(Of Boolean)

    Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

    Public Sub New(execute As Action, Optional canExecute As Func(Of Boolean) = Nothing)
        If execute Is Nothing Then Throw New ArgumentNullException(NameOf(execute))
        _execute = execute
        _canExecute = canExecute
    End Sub

    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        Return If(_canExecute Is Nothing, True, _canExecute.Invoke())
    End Function

    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        _execute.Invoke()
    End Sub

    Public Sub RaiseCanExecuteChanged()
        RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)
    End Sub
End Class
