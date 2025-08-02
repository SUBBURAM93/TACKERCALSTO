Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Data.SqlClient
Imports System.Configuration
Imports Microsoft.Data.SqlClient
Imports System.Threading.Tasks
Imports TACKERCALSTO.DashboardModel

Public Class DashboardViewModel
    Implements INotifyPropertyChanged



#Region "Fields"

    Private _operationsList As ObservableCollection(Of DashboardModel)
    Private _filteredDetailsList As ObservableCollection(Of DetailModel)
    Private _selectedOperation As DashboardModel

#End Region


#Region "Properties"

    Public Property OperationsList As ObservableCollection(Of DashboardModel)
        Get
            Return _operationsList
        End Get
        Set(value As ObservableCollection(Of DashboardModel))
            _operationsList = value
            OnPropertyChanged(NameOf(OperationsList))
        End Set
    End Property

    Public Property FilteredDetailsList As ObservableCollection(Of DetailModel)
        Get
            Return _filteredDetailsList
        End Get
        Set(value As ObservableCollection(Of DetailModel))
            _filteredDetailsList = value
            OnPropertyChanged(NameOf(FilteredDetailsList))
        End Set
    End Property

    Public Property SelectedOperation As DashboardModel
        Get
            Return _selectedOperation
        End Get
        Set(value As DashboardModel)
            _selectedOperation = value
            OnPropertyChanged(NameOf(SelectedOperation))

            If value IsNot Nothing Then
                LoadDetailsFromDatabaseAsync(value.BOMNo)
            Else
                ' Clear details list when no selection
                FilteredDetailsList = New ObservableCollection(Of DetailModel)()
            End If
        End Set
    End Property

#End Region


#Region "Constructor"

    Public Sub New()
        ' Load operations asynchronously avoiding UI blocking
        LoadOperationsAsync()
        FilteredDetailsList = New ObservableCollection(Of DetailModel)()
    End Sub

#End Region


#Region "Data Loading Methods"

    ''' <summary>
    ''' Asynchronously load Operations from database into OperationsList
    ''' </summary>
    Private Async Sub LoadOperationsAsync()
        Dim list = Await Task.Run(Function() GetOperations())
        OperationsList = New ObservableCollection(Of DashboardModel)(list)
    End Sub

    ''' <summary>
    ''' Synchronously fetch operations list from database
    ''' </summary>
    ''' <returns>List of OperationsModel</returns>
    Private Shared Function GetOperations() As List(Of DashboardModel)
        Dim list As New List(Of DashboardModel)()
        Dim conString = ConfigurationManager.ConnectionStrings("Db_Server").ConnectionString

        Using con As New SqlConnection(conString)
            con.Open()
            Using cmd As New SqlCommand("SELECT * FROM v_MT_OPERATIONS", con)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        list.Add(New DashboardModel With {
                            .BOMNo = reader("BOM No.").ToString(),
                            .CustomerName = reader("Customer Name").ToString(),
                            .Location = reader("Location").ToString(),
                            .PONo = reader("PO No").ToString(),
                            .BookingDate = If(IsDBNull(reader("Booking Date")), Nothing, CDate(reader("Booking Date"))),
                            .Sales = reader("Sales").ToString(),
                            .TotalWIDs = Convert.ToInt32(reader("Total WIDs")),
                            .PendingWIDs = Convert.ToInt32(reader("Pending WIDs")),
                            .PODispatchDate = If(IsDBNull(reader("PO Dispatch Date")), Nothing, CDate(reader("PO Dispatch Date"))),
                            .DispatchDate = If(IsDBNull(reader("Dispatch Date")), Nothing, CDate(reader("Dispatch Date"))),
                            .ProjectStatus = reader("Project Status").ToString(),
                            .DueDate = If(IsDBNull(reader("Due Date")), Nothing, Convert.ToInt32(reader("Due Date")))
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
    ''' <param name="bomNo"></param>
    Private Async Sub LoadDetailsFromDatabaseAsync(bomNo As String)
        Dim result = Await Task.Run(Function() GetDetailsByBOM(bomNo))
        FilteredDetailsList = New ObservableCollection(Of DetailModel)(result)
    End Sub


    ''' <summary>
    ''' Synchronously fetch details filtered by BOMNo from database
    ''' </summary>
    ''' <param name="bomNo"></param>
    ''' <returns>List of DetailModel</returns>
    Public Shared Function GetDetailsByBOM(bomNo As String) As List(Of DetailModel)
        Dim result As New List(Of DetailModel)()
        Dim conString = ConfigurationManager.ConnectionStrings("Db_Server").ConnectionString

        Using con As New SqlConnection(conString)
            Dim cmd As New SqlCommand("SELECT Field, Detail FROM PROJTAB_sideDG WHERE PRO_No = @bomNo", con)
            cmd.Parameters.AddWithValue("@bomNo", bomNo)

            con.Open()
            Using rdr = cmd.ExecuteReader()
                While rdr.Read()
                    result.Add(New DetailModel With {
                        .Field = rdr("Field").ToString(),
                        .Detail = rdr("Detail").ToString()
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
