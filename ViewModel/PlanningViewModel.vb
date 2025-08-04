Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Data.SqlClient
Imports System.Configuration
Imports Microsoft.Data.SqlClient
Imports System.Threading.Tasks
Imports TACKERCALSTO.PlanningModel

Public Class PlanningViewModel
    Implements INotifyPropertyChanged



#Region "Fields"

    Private _PlannigList As ObservableCollection(Of PlanningModel)
    Private _PlanningSideDgList As ObservableCollection(Of PlanningSideDgModel)
    Private _selectedWID As PlanningModel

#End Region


#Region "Properties"

    Public Property PlannigList As ObservableCollection(Of PlanningModel)
        Get
            Return _PlannigList
        End Get
        Set(value As ObservableCollection(Of PlanningModel))
            _PlannigList = value
            OnPropertyChanged(NameOf(PlannigList))
        End Set
    End Property

    Public Property PlanningSideDgList As ObservableCollection(Of PlanningSideDgModel)
        Get
            Return _PlanningSideDgList
        End Get
        Set(value As ObservableCollection(Of PlanningSideDgModel))
            _PlanningSideDgList = value
            OnPropertyChanged(NameOf(PlanningSideDgList))
        End Set
    End Property

    Public Property SelectedWID As PlanningModel
        Get
            Return _selectedWID
        End Get
        Set(value As PlanningModel)
            _selectedWID = value
            OnPropertyChanged(NameOf(SelectedWID))

            If value IsNot Nothing Then
                LoadDetailsFromDatabaseAsync(value.PROJECTNO)
            Else
                ' Clear details list when no selection
                PlanningSideDgList = New ObservableCollection(Of PlanningSideDgModel)()
            End If
        End Set
    End Property

#End Region


#Region "Constructor"

    Public Sub New()
        ' Load operations asynchronously avoiding UI blocking
        LoadOperationsAsync()
        PlanningSideDgList = New ObservableCollection(Of PlanningSideDgModel)()
    End Sub

#End Region


#Region "Data Loading Methods"

    ''' <summary>
    ''' Asynchronously load Operations from database into OperationsList
    ''' </summary>
    Private Async Sub LoadOperationsAsync()
        Dim list = Await Task.Run(Function() GetOperations())
        PlannigList = New ObservableCollection(Of PlanningModel)(list)
    End Sub

    ''' <summary>
    ''' Synchronously fetch operations list from database
    ''' </summary>
    ''' <returns>List of OperationsModel</returns>
    Private Shared Function GetOperations() As List(Of PlanningModel)
        Dim list As New List(Of PlanningModel)()
        Dim conString = ConfigurationManager.ConnectionStrings("Db_Server").ConnectionString

        Using con As New SqlConnection(conString)
            con.Open()
            Using cmd As New SqlCommand("SELECT * FROM Plan_Maindg", con)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        list.Add(New PlanningModel With {
                        .PROJECTNO = reader("PRO_No").ToString(),
                        .Customer = reader("Customer").ToString()
                    })
                    End While
                End Using
            End Using
        End Using

        Return list
    End Function



    ''' <summary>
    ''' Async load details filtered by BOMNo and update FilteredDetailsList
    ''' </summary>
    ''' <param name="wid"></param>
    Private Async Sub LoadDetailsFromDatabaseAsync(wid As String)
        Dim result = Await Task.Run(Function() GetDetailsByBOM(wid))
        PlanningSideDgList = New ObservableCollection(Of PlanningSideDgModel)(result)
    End Sub


    ''' <summary>
    ''' Synchronously fetch details filtered by BOMNo from database
    ''' </summary>
    ''' <param name="BOMNo"></param>
    ''' <returns>List of DetailModel</returns>
    Public Shared Function GetDetailsByBOM(BOMNo As String) As List(Of PlanningSideDgModel)
        Dim result As New List(Of PlanningSideDgModel)()
        Dim conString = ConfigurationManager.ConnectionStrings("Db_Server").ConnectionString

        Using con As New SqlConnection(conString)
            Dim cmd As New SqlCommand("SELECT * FROM v_MT_PPC WHERE [BOM No.] = @BOMNo", con)
            cmd.Parameters.AddWithValue("@BOMNo", BOMNo)

            con.Open()
            Using rdr = cmd.ExecuteReader()
                While rdr.Read()
                    result.Add(New PlanningSideDgModel With {
                        .WID = rdr("WID").ToString(),
                        .BOMNo = rdr("BOM No.").ToString(),
                        .BOMName = rdr("BOM Name").ToString(),
                        .Customer = rdr("Customer").ToString(),
                        .Description = rdr("Description").ToString(),
                        .Assigned = rdr("Assinged").ToString(),
                        .BOMQty = Convert.ToInt32(rdr("BOM Qty")),
                        .Colour = rdr("Colour").ToString(),
                        .Process = rdr("Process").ToString(),
                        .PackingBalance = Convert.ToInt32(rdr("Packing Balance.")),
                        .DispatchQty = Convert.ToInt32(rdr("Dispatched Qty")),
                        .DispatchPending = Convert.ToInt32(rdr("Dispatch Pending.")),
                        .DispatchDate = If(IsDBNull(rdr("Dispatch Date")), Nothing, CDate(rdr("Dispatch Date")))
                    })
                End While
            End Using
        End Using

        Return result
    End Function

#End Region


#Region "INotifyPropertyChanged Implementation"

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub

#End Region

End Class
