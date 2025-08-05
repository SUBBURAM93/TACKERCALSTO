Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Controls
Imports System.Windows.Input

Class Login
    Private isPasswordVisible As Boolean = False
    Private txtPasswordVisible As TextBox = Nothing

    Private Const AppVersion As String = "P25.0.0"
    Private Const Product As String = "PRODUCTION"

    Private Sub CheckVersion()
        Dim isCurrent = LoginDBHelper.CheckVersion(AppVersion, Product)
        If Not isCurrent Then
            btnLogin.IsEnabled = False
            lblUpdate.Visibility = Visibility.Visible
        Else
            btnLogin.IsEnabled = True
            lblUpdate.Visibility = Visibility.Collapsed
        End If
    End Sub
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        CheckVersion()
        CheckDatabaseConnection()
        lblVersion.Text = AppVersion
        If txtPasswordVisible Is Nothing Then
            txtPasswordVisible = New TextBox With {
                .FontSize = 12,
                .Foreground = New SolidColorBrush(Color.FromRgb(&H23, &H2D, &H37)),
                .Background = Brushes.Transparent,
                .BorderThickness = New Thickness(0),
                .Width = 166,
                .Padding = New Thickness(4, 0, 0, 0),
                .VerticalContentAlignment = VerticalAlignment.Center,
                .Visibility = Visibility.Collapsed
            }
            AddHandler txtPasswordVisible.TextChanged, AddressOf txtPasswordVisible_TextChanged
        End If
    End Sub

    Private Sub btnLogin_Click(sender As Object, e As RoutedEventArgs)

        lblError.Visibility = Visibility.Collapsed
        If String.IsNullOrWhiteSpace(txtUsername.Text) OrElse
       (If(isPasswordVisible, String.IsNullOrWhiteSpace(txtPasswordVisible.Text), String.IsNullOrWhiteSpace(txtPassword.Password))) Then
            lblError.Text = "Please enter username and password."
            lblError.Visibility = Visibility.Visible
            LoginDBHelper.LogLogin(txtUsername.Text, False, "Empty credentials")
            Return
        End If

        Dim user = LoginDBHelper.ValidateUser(txtUsername.Text, If(isPasswordVisible, txtPasswordVisible.Text, txtPassword.Password))
        If user IsNot Nothing Then
            LoginDBHelper.LogLogin(user.UserID, True, "Login success")
            MessageBox.Show($"Hi, {user.Name} ({user.Role})", "Login Successful")
            Dim homeWindow As New Home()
            homeWindow.Show()
            Me.Close()
        Else
            lblError.Text = "Invalid username or password."
            lblError.Visibility = Visibility.Visible
            LoginDBHelper.LogLogin(txtUsername.Text, False, "Invalid credentials")
        End If
        AddHandler txtUsername.KeyDown, AddressOf EnterKeyPressed
        AddHandler txtPassword.KeyDown, AddressOf EnterKeyPressed
        AddHandler txtPasswordVisible.KeyDown, AddressOf EnterKeyPressed
    End Sub
    Private Sub EnterKeyPressed(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Enter Then
            btnLogin_Click(btnLogin, New RoutedEventArgs())
        End If
    End Sub



    Private Sub btnTogglePassword_Click(sender As Object, e As RoutedEventArgs)
        isPasswordVisible = Not isPasswordVisible
        Dim dockPanel = TryCast(txtPassword.Parent, DockPanel)
        If dockPanel Is Nothing Then Return

        If isPasswordVisible Then
            txtPasswordVisible.Text = txtPassword.Password
            txtPassword.Visibility = Visibility.Collapsed
            If Not dockPanel.Children.Contains(txtPasswordVisible) Then
                dockPanel.Children.Insert(2, txtPasswordVisible)
            End If
            txtPasswordVisible.Visibility = Visibility.Visible
        Else
            txtPassword.Password = txtPasswordVisible.Text
            txtPassword.Visibility = Visibility.Visible
            txtPasswordVisible.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Sub txtPasswordVisible_TextChanged(sender As Object, e As TextChangedEventArgs)
        
        If Not isPasswordVisible Then
            txtPassword.Password = txtPasswordVisible.Text
        End If
    End Sub

    Private Sub txtForgotPassword_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        MessageBox.Show("Please contact admin.", "Forgot Password", MessageBoxButton.OK, MessageBoxImage.Information)
    End Sub

    Private Sub txtPassword_PreviewKeyDown(sender As Object, e As KeyEventArgs) Handles txtPassword.PreviewKeyDown
        lblCapsLock.Visibility = If(Console.CapsLock, Visibility.Visible, Visibility.Collapsed)
    End Sub

    Private Sub txtPassword_GotFocus(sender As Object, e As RoutedEventArgs) Handles txtPassword.GotFocus
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

    Private Sub label_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles label.MouseDown
        Close()
        Application.Current.Shutdown()
    End Sub
End Class