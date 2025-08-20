Imports System.ComponentModel
Imports System.Configuration
Imports System.Windows.Input
Imports Microsoft.Data.SqlClient

Public Class LoginViewModel
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub RaisePropertyChanged(prop As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

    ' Username
    Private _username As String
    Public Property Username As String
        Get
            Return _username
        End Get
        Set(value As String)
            _username = value
            RaisePropertyChanged(NameOf(Username))
            CType(LoginCommand, RelayCommand).RaiseCanExecuteChanged()
        End Set
    End Property

    ' Password
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

    ' Error message
    Private _errorMessage As String
    Public Property ErrorMessage As String
        Get
            Return _errorMessage
        End Get
        Set(value As String)
            _errorMessage = value
            RaisePropertyChanged(NameOf(ErrorMessage))
        End Set
    End Property

    ' Error visibility
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

    ' Version valid flag
    Private _isVersionValid As Boolean = True
    Public Property IsVersionValid As Boolean
        Get
            Return _isVersionValid
        End Get
        Set(value As Boolean)
            _isVersionValid = value
            RaisePropertyChanged(NameOf(IsVersionValid))
            CType(LoginCommand, RelayCommand).RaiseCanExecuteChanged()
        End Set
    End Property

    ' Update label visibility
    Private _isUpdateLabelVisible As Visibility = Visibility.Collapsed
    Public Property IsUpdateLabelVisible As Visibility
        Get
            Return _isUpdateLabelVisible
        End Get
        Set(value As Visibility)
            _isUpdateLabelVisible = value
            RaisePropertyChanged(NameOf(IsUpdateLabelVisible))
        End Set
    End Property

    ' App version (hardcode or get from assembly)
    Public ReadOnly Property CurrentVersion As String = "P25.0.0"
    Public ReadOnly Property Prduct As String = "PRODUCTION"


    ' Login command
    Private _loginCommand As ICommand
    Public ReadOnly Property LoginCommand As ICommand
        Get
            If _loginCommand Is Nothing Then
                _loginCommand = New RelayCommand(AddressOf ExecuteLogin, AddressOf CanExecuteLogin)
            End If
            Return _loginCommand
        End Get
    End Property

    Public Sub New()
        CheckAppVersion()
    End Sub

    ' Version check logic
    Private Sub CheckAppVersion()
        Dim dbVersion As String = ""
        Try
            Using con = LoginDBHelper.GetConnection()
                con.Open()
                Dim cmd As New SqlCommand("SELECT TOP 1 [Version] FROM VERSION_TABLE ORDER BY [Version] DESC", con)
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then dbVersion = result.ToString()
            End Using
        Catch ex As Exception
            dbVersion = ""
        End Try

        If dbVersion <> CurrentVersion Then
            IsVersionValid = False
            IsUpdateLabelVisible = Visibility.Visible
        Else
            IsVersionValid = True
            IsUpdateLabelVisible = Visibility.Collapsed
        End If
    End Sub

    ' Login button enabled only if all fields and version valid
    Private Function CanExecuteLogin() As Boolean
        Return Not String.IsNullOrWhiteSpace(Username) AndAlso Not String.IsNullOrWhiteSpace(Password) AndAlso IsVersionValid
    End Function

    ' Logging to LOGIN_LOG table
    Private Sub LogLogin(success As Boolean, message As String)
        Try
            Using con = LoginDBHelper.GetConnection()
                con.Open()
                Dim cmd As New SqlCommand("INSERT INTO LOGIN_LOG (USERID, SYSTEMNAME, MACADDRESS, STATUS, MESSAGE) VALUES (@user, @system, @mac, @status, @msg)", con)
                cmd.Parameters.AddWithValue("@user", If(Username, ""))
                cmd.Parameters.AddWithValue("@system", Environment.MachineName)
                cmd.Parameters.AddWithValue("@mac", GetMacAddress())
                cmd.Parameters.AddWithValue("@status", If(success, "Success", "Failed"))
                cmd.Parameters.AddWithValue("@msg", message)
                cmd.ExecuteNonQuery()
            End Using
        Catch
            ' Swallow errors for logging
        End Try
    End Sub

    ' Get MAC address (first found)
    Private Function GetMacAddress() As String
        Try
            For Each nic In Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                If nic.OperationalStatus = Net.NetworkInformation.OperationalStatus.Up Then
                    Return nic.GetPhysicalAddress().ToString()
                End If
            Next
            Return ""
        Catch
            Return ""
        End Try
    End Function

    ' Login logic
    Private Sub ExecuteLogin()
        If String.IsNullOrWhiteSpace(Username) OrElse String.IsNullOrWhiteSpace(Password) Then
            ErrorMessage = "Username and Password are required."
            IsErrorVisible = Visibility.Visible
            LogLogin(False, "Empty credentials")
            Return
        End If

        Try
            Using con = LoginDBHelper.GetConnection()
                con.Open()
                Dim cmd As New SqlCommand("SELECT Role FROM USERDETAILS WHERE UserID = @user AND Password = @pass", con)
                cmd.Parameters.AddWithValue("@user", Username)
                cmd.Parameters.AddWithValue("@pass", Password)

                Dim role = cmd.ExecuteScalar()
                If role IsNot Nothing Then
                    LogLogin(True, "Login success")
                    ' Show main window, close login (handled in code-behind)
                    Application.Current.Dispatcher.Invoke(Sub()
                                                              Dim main As New Home()
                                                              main.Show()
                                                              Application.Current.Windows.OfType(Of Window).FirstOrDefault()?.Close()
                                                          End Sub)
                Else
                    ErrorMessage = "Invalid username/password."
                    IsErrorVisible = Visibility.Visible
                    LogLogin(False, "Invalid credentials")
                End If
            End Using
        Catch ex As Exception
            ErrorMessage = "Login error: " & ex.Message
            IsErrorVisible = Visibility.Visible
            LogLogin(False, ex.Message)
        End Try
    End Sub
End Class