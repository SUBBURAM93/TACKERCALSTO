Imports Microsoft.Data.SqlClient
Imports System.Configuration
Imports System.Data


Module LoginDBHelper

    Public ReadOnly Property Connstring As String
        Get
            Return ConfigurationManager.ConnectionStrings("Db_Server").ConnectionString
        End Get
    End Property
    Public Function GetConnection() As SqlConnection
        Return New SqlConnection(Connstring)
    End Function

End Module
