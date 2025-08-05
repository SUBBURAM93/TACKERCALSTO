Imports System.ComponentModel



Public Class PlanningModel
    Public Property PROJECTNO As String
    Public Property Customer As String



    Public Class PlanningSideDgModel
        Implements INotifyPropertyChanged

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Protected Sub OnPropertyChanged(propertyName As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))

        End Sub
        Public Property WID As String
        Public Property BOMNo As String
        Public Property BOMName As String
        Public Property Customer As String
        Public Property Description As String
        Public Property Assigned As String
        Public Property BOMQty As Integer
        Public Property Colour As String
        Public Property Process As String
        Public Property Jobcard As String
        Public Property PackingBalance As Integer
        Public Property DispatchQty As Integer
        Public Property DispatchPending As Integer
        Public Property DispatchDate As Nullable(Of Date)



        Private _IsSelectedToCreate As Boolean

        Public Property IsSelectedToCreate As Boolean
            Get
                Return _IsSelectedToCreate
            End Get
            Set(value As Boolean)
                _IsSelectedToCreate = value
                OnPropertyChanged(NameOf(IsSelectedToCreate))
            End Set
        End Property
    End Class



End Class


Public Class JobCardModel
    Public Property JC_no As String
    Public Property WID As String
    Public Property OP_Sequence As String
    Public Property Operation_ID As String
    Public Property Created_date As Nullable(Of Date)
    Public Property Created_by As String

End Class
