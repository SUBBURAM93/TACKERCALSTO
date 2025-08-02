Imports System.Configuration
Imports System.Net.NetworkInformation
Imports System.Windows.Input
Imports System.Windows.Threading
Imports Microsoft.Data.SqlClient

Class Login

    Private ViewModel As New LoginViewModel()

    Public Sub New()
        InitializeComponent()
        Me.DataContext = ViewModel

        AddHandler txtPassword.PasswordChanged, Sub(s, e)
                                                    ViewModel.Password = txtPassword.Password
                                                End Sub
        AddHandler txtPasswordVisible.TextChanged, Sub(s, e)
                                                       ViewModel.Password = txtPasswordVisible.Text
                                                   End Sub
    End Sub
    Private Sub chkShowPassword_Checked(sender As Object, e As RoutedEventArgs)
        txtPasswordVisible.Text = txtPassword.Password
        txtPasswordVisible.Visibility = Visibility.Visible
        txtPassword.Visibility = Visibility.Collapsed
    End Sub
    Private Sub txtPasswordVisible_TextChanged(sender As Object, e As TextChangedEventArgs)
        If txtPassword.Visibility = Visibility.Collapsed Then
            txtPassword.Password = txtPasswordVisible.Text
        End If
    End Sub
    Private Sub chkShowPassword_Unchecked(sender As Object, e As RoutedEventArgs)
        txtPassword.Password = txtPasswordVisible.Text
        txtPassword.Visibility = Visibility.Visible
        txtPasswordVisible.Visibility = Visibility.Collapsed
    End Sub
    Private Sub txtForgotPassword_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        MessageBox.Show("Please contact admin.", "Forgot Password", MessageBoxButton.OK, MessageBoxImage.Information)
    End Sub
    Private Sub txtPassword_PreviewKeyDown(sender As Object, e As KeyEventArgs) Handles txtPassword.PreviewKeyDown
        CheckCapsLock()
    End Sub

    Private Sub txtPassword_GotFocus(sender As Object, e As RoutedEventArgs) Handles txtPassword.GotFocus
        CheckCapsLock()
    End Sub
    Private Sub CheckCapsLock()
        lblCapsLock.Visibility = If(Console.CapsLock, Visibility.Visible, Visibility.Collapsed)
    End Sub
    Private Sub txtPassword_PasswordChanged(sender As Object, e As RoutedEventArgs)


        Dim viewModel = TryCast(Me.DataContext, LoginViewModel)
        If viewModel IsNot Nothing Then
            viewModel.Password = txtPassword.Password

        End If
    End Sub


    Private Sub CheckDatabaseConnection()
        Try
            Using con = LoginDBHelper.GetConnection()
                con.Open()
                dbStatusIndicator.Fill = New SolidColorBrush(Colors.Green)
                dbStatusText.Text = "Connected"
            End Using
        Catch
            dbStatusIndicator.Fill = New SolidColorBrush(Colors.Red)
            dbStatusText.Text = "Not Connected"
        End Try
    End Sub




End Class
