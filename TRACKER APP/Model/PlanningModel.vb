Imports System.ComponentModel

Public Class PlanningModel
    ' This may contain header-level info like PROJECTNO etc.
    Public Property PROJECTNO As String
    Public Property Customer As String

    ' Sub-table for DataGrid rows
    Public Class PlanningSideDgModel
        Implements INotifyPropertyChanged

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

        ' Checkbox property
        Private _IsSelectedToCreate As Boolean
        Public Property IsSelectedToCreate As Boolean
            Get
                Return _IsSelectedToCreate
            End Get
            Set(value As Boolean)
                If _IsSelectedToCreate <> value Then
                    _IsSelectedToCreate = value
                    OnPropertyChanged(NameOf(IsSelectedToCreate))
                End If
            End Set
        End Property

        ' Notify property changes to UI
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Protected Sub OnPropertyChanged(propertyName As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
    End Class
End Class

' Separate Job Card Model
Public Class JobCardModel
    Public Property JC_no As String
    Public Property WID As String
    Public Property OP_Sequence As String
    Public Property Operation_ID As String
    Public Property Created_date As Nullable(Of Date)
    Public Property Created_by As String
End Class
