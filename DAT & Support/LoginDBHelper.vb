Imports System.Configuration
Imports System.Data.SqlClient
Imports Microsoft.Data.SqlClient

Module LoginDBHelper

    Public ReadOnly Property Connstring As String

        Get
            Return ConfigurationManager.ConnectionStrings("Db_Server").ConnectionString
        End Get
    End Property

    Public Function GetConnection() As SqlConnection
        Return New SqlConnection(Connstring)
    End Function
    Public Function CheckVersion(appVersion As String, product As String) As Boolean
        Using con = GetConnection()
            con.Open()
            Dim cmd As New SqlCommand("SELECT [Version] FROM VERSION_TABLE WHERE Product = @Product", con)
            cmd.Parameters.AddWithValue("@Product", product)
            Dim dbVersion = cmd.ExecuteScalar()?.ToString()
            Return dbVersion = appVersion
        End Using
    End Function

    Public Function ValidateUser(username As String, password As String) As User
        Using con = GetConnection()
            con.Open()
            Dim cmd As New SqlCommand("SELECT UserID, Name, Email, Role FROM USERDETAILS WHERE UserID = @user AND Password = @pass", con)
            cmd.Parameters.AddWithValue("@user", username)
            cmd.Parameters.AddWithValue("@pass", password)
            Using rdr = cmd.ExecuteReader()
                If rdr.Read() Then
                    Return New User With {
                        .UserID = rdr("UserID").ToString(),
                        .Name = rdr("Name").ToString(),
                        .Email = rdr("Email").ToString(),
                        .Role = rdr("Role").ToString()
                    }
                End If
            End Using
        End Using
        Return Nothing
    End Function

    Public Sub LogLogin(userId As String, success As Boolean, message As String)
        Using con = GetConnection()
            con.Open()
            Dim cmd As New SqlCommand("INSERT INTO LOGIN_LOG (USERID, SYSTEMNAME, MACADDRESS, STATUS, MESSAGE) VALUES (@user, @system, @mac, @status, @msg)", con)
            cmd.Parameters.AddWithValue("@user", userId)
            cmd.Parameters.AddWithValue("@system", Environment.MachineName)
            cmd.Parameters.AddWithValue("@mac", GetMacAddress())
            cmd.Parameters.AddWithValue("@status", If(success, "Success", "Failed"))
            cmd.Parameters.AddWithValue("@msg", message)
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Function GetMacAddress() As String
        For Each nic In Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
            If nic.OperationalStatus = Net.NetworkInformation.OperationalStatus.Up Then
                Return nic.GetPhysicalAddress().ToString()
            End If
        Next
        Return ""
    End Function
End Module