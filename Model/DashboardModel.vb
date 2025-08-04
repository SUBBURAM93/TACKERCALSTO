Public Class DashboardModel

    Public Property BOMNo As String
    Public Property CustomerName As String
    Public Property Location As String
    Public Property PONo As String
    Public Property BookingDate As Nullable(Of Date)
    Public Property Sales As String
    Public Property TotalWIDs As Integer
    Public Property PendingWIDs As Integer
    Public Property PODispatchDate As Nullable(Of Date)
    Public Property DispatchDate As Nullable(Of Date)
    Public Property ProjectStatus As String
    Public Property DueDate As Nullable(Of Integer)


    Public Class DetailModel

        Public Property Field As String
        Public Property Detail As String
    End Class


End Class
