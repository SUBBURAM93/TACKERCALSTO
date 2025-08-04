
Imports System.Windows.Input
Imports System.Windows.Threading
Imports System.Configuration
Imports Microsoft.Data.SqlClient


Class Login


    Private isPasswordVisible As Boolean = False

    Public Sub New()
        InitializeComponent()
        CheckDatabaseConnection()
        AddHandler btnTogglePassword.Click, AddressOf TogglePasswordVisibility
    End Sub

    ' Toggle between hidden and visible password
    Private Sub TogglePasswordVisibility(sender As Object, e As RoutedEventArgs)
        isPasswordVisible = Not isPasswordVisible
        If isPasswordVisible Then
            ' Convert Secure Password to plain text (Not secure, only for UI simulation)
            Dim plainText = txtPassword.Password
            txtPassword.Visibility = Visibility.Collapsed
            txtPasswordVisible = New TextBox With {
                .Text = plainText,
                .FontSize = 12,
                .Foreground = New SolidColorBrush(Color.FromRgb(&H23, &H2D, &H37)),
                .Background = Brushes.Transparent,
                .BorderThickness = New Thickness(0),
                .Width = 166,
                .Padding = New Thickness(4, 0, 0, 0)
            }

            AddHandler txtPasswordVisible.TextChanged, Sub(s, args)
                                                           txtPassword.Password = txtPasswordVisible.Text
                                                       End Sub

            Dim dockPanel = TryCast(VisualTreeHelper.GetParent(txtPassword), DockPanel)
            If dockPanel IsNot Nothing Then
                Dim index = dockPanel.Children.IndexOf(txtPassword)
                dockPanel.Children.Remove(txtPassword)
                dockPanel.Children.Insert(index, txtPasswordVisible)
            End If
        Else
            Dim plainText = txtPasswordVisible.Text
            txtPassword.Password = plainText
            Dim dockPanel = TryCast(VisualTreeHelper.GetParent(txtPasswordVisible), DockPanel)
            If dockPanel IsNot Nothing Then
                Dim index = dockPanel.Children.IndexOf(txtPasswordVisible)
                dockPanel.Children.Remove(txtPasswordVisible)
                dockPanel.Children.Insert(index, txtPassword)
            End If
            txtPassword.Visibility = Visibility.Visible
        End If
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
