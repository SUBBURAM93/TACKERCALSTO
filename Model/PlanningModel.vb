Public Class PlanningModel
    Public Property PROJECTNO As String
    Public Property Customer As String



    Public Class PlanningSideDgModel

        Public Property WID As String
        Public Property BOMNo As String
        Public Property BOMName As String
        Public Property Customer As String
        Public Property Description As String
        Public Property Assigned As String
        Public Property BOMQty As Integer
        Public Property Colour As String
        Public Property Process As String
        Public Property PackingBalance As Integer
        Public Property DispatchQty As Integer
        Public Property DispatchPending As Integer
        Public Property DispatchDate As Nullable(Of Date)
    End Class


End Class
