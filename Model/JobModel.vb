Public Class JobModel
    Public Property BOMName As String
    Public Property JC_no As String
    Public Property PartNumber As String
    Public Property Description As String
    Public Property Length As Double
    Public Property Width As Double
    Public Property Thick As Double
    Public Property UnitWeight As Double
    Public Property TotalWeight As Double
    Public Property Colour As String
    Public Property BOMQty As Integer
    Public Property ProducedQty As Integer
    Public Property BalanceQty As Integer
    Public Property DispatchDate As Date?
    Public Property DueDate As Integer
    Public Property IsSelectedToCreate As Boolean ' For UI selection
End Class
