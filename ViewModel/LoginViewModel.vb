Imports System.ComponentModel
Imports System.Windows.Input
Imports System.Windows
Imports System.Configuration
Imports Microsoft.Data.SqlClient
Imports System.Net.NetworkInformation

Public Class LoginViewModel
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub RaisePropertyChanged(prop As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

    Private _username As String
    Public Property Username As String
        Get

            Return _username

        End Get
        Set(value As String)
            _username = value

            RaisePropertyChanged(NameOf(Username))


        End Set
    End Property

    Private _password As String
    Public Property Password As String
        Get
            Return _password

        End Get
        Set(value As String)
            _password = value

            RaisePropertyChanged(NameOf(Password))
            CType(LoginCommand, RelayCommand).RaiseCanExecuteChanged()
        End Set
    End Property

    Private _errorMessage As String
    Public Property ErrorMessage As String
        Get
            Return _errorMessage
        End Get
        Set(value As String)
            _errorMessage = value
            RaisePropertyChanged(NameOf(ErrorMessage))
            CType(LoginCommand, RelayCommand).RaiseCanExecuteChanged()

        End Set
    End Property

    Private _isErrorVisible As Visibility = Visibility.Collapsed
    Public Property IsErrorVisible As Visibility
        Get
            Return _isErrorVisible
        End Get
        Set(value As Visibility)
            _isErrorVisible = value
            RaisePropertyChanged(NameOf(IsErrorVisible))


        End Set
    End Property

    Private _loginCommand As ICommand
    Public ReadOnly Property LoginCommand As ICommand
        Get
            If _loginCommand Is Nothing Then

                _loginCommand = New RelayCommand(AddressOf ExecuteLogin, AddressOf CanExecuteLogin)

            End If
            Return _loginCommand
        End Get
    End Property



    Private Function CanExecuteLogin() As Boolean
        If Not String.IsNullOrWhiteSpace(Username) AndAlso Not String.IsNullOrWhiteSpace(Password) Then
            Return True
        Else
            Return False
        End If
    End Function
    Private Sub ExecuteLogin()

        If String.IsNullOrWhiteSpace(Username) OrElse String.IsNullOrWhiteSpace(Password) Then
            ErrorMessage = "Username and Password are required."
            IsErrorVisible = Visibility.Visible
            Return
        End If

        Dim conString = ConfigurationManager.ConnectionStrings("Db_Server").ConnectionString
        Using con As New SqlConnection(conString)
            con.Open()
            Dim cmd As New SqlCommand("SELECT Role FROM USERDETAILS WHERE UserID = @user AND Password = @pass", con)
            cmd.Parameters.AddWithValue("@user", Username)
            cmd.Parameters.AddWithValue("@pass", Password)

            Dim role = cmd.ExecuteScalar()
            If role IsNot Nothing Then
                ' success
                Dim main As New Home()
                main.Show()
                Application.Current.Windows.OfType(Of Window).FirstOrDefault()?.Close()
            Else
                ErrorMessage = "Invalid username/password."
                IsErrorVisible = Visibility.Visible
            End If
        End Using
    End Sub
End Class
