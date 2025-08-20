Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Threading.Tasks
Imports TACKERCALSTO.PlanningModel
Imports System.Windows.Input
Imports Microsoft.Data.SqlClient
Imports System.Data

Public Class PlanningViewModel
    Implements INotifyPropertyChanged

#Region "Fields"
    Private _PlannigList As ObservableCollection(Of PlanningModel)
    Private _PlanningSideDgList As ObservableCollection(Of PlanningSideDgModel)
    Private _JobCardList As ObservableCollection(Of JobCardModel)

    Private _selectedProj As PlanningModel
    Private _selectedBOM As PlanningSideDgModel

    Private _createJobCardCommand As ICommand
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

    Public Property JobCardList As ObservableCollection(Of JobCardModel)
        Get
            Return _JobCardList
        End Get
        Set(value As ObservableCollection(Of JobCardModel))
            _JobCardList = value
            OnPropertyChanged(NameOf(JobCardList))
            RaiseCreateJobCardCanExecuteChanged()
        End Set
    End Property

    Public Property SelectedProj As PlanningModel
        Get
            Return _selectedProj
        End Get
        Set(value As PlanningModel)
            _selectedProj = value
            OnPropertyChanged(NameOf(SelectedProj))

            If value IsNot Nothing Then
                LoadDetailsFromDatabaseAsync(value.PROJECTNO)
            Else
                PlanningSideDgList = New ObservableCollection(Of PlanningSideDgModel)()
            End If
        End Set
    End Property

    Public Property SelectedBOM As PlanningSideDgModel
        Get
            Return _selectedBOM
        End Get
        Set(value As PlanningSideDgModel)
            _selectedBOM = value
            OnPropertyChanged(NameOf(SelectedBOM))

            If value IsNot Nothing Then
                LoadJobCardDataAsync(value.WID)


            Else
                JobCardList = New ObservableCollection(Of JobCardModel)()
            End If

            RaiseCreateJobCardCanExecuteChanged()
        End Set
    End Property

    Public ReadOnly Property CreateJobCardCommand As ICommand
        Get
            If _createJobCardCommand Is Nothing Then
                _createJobCardCommand = New RelayCommand(AddressOf CreateJobCard, AddressOf CanCreateJobCard)
            End If
            Return _createJobCardCommand
        End Get
    End Property

#End Region

#Region "Constructor"
    Public Sub New()
        LoadOperationsAsync()
        PlanningSideDgList = New ObservableCollection(Of PlanningSideDgModel)()
        JobCardList = New ObservableCollection(Of JobCardModel)()
    End Sub
#End Region

#Region "Load Methods"

    Private Async Sub LoadOperationsAsync()
        Dim list = Await Task.Run(Function() GetWids())
        PlannigList = New ObservableCollection(Of PlanningModel)(list)
    End Sub

    Private Shared Function GetWids() As List(Of PlanningModel)
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

    'Private Async Sub LoadDetailsFromDatabaseAsync(bomNo As String)
    'Dim result = Await Task.Run(Function() GetDetailsByBOM(bomNo))
    '    PlanningSideDgList = New ObservableCollection(Of PlanningSideDgModel)(result)
    'End Sub


    Private Async Sub LoadDetailsFromDatabaseAsync(bomNo As String)
        Try
            Dim result = Await Task.Run(Function() GetDetailsByBOM(bomNo))
            ' ... DB logic to fill newList ...

            If PlanningSideDgList Is Nothing Then
                PlanningSideDgList = New ObservableCollection(Of PlanningSideDgModel)(result)
            Else
                PlanningSideDgList.Clear()
            End If

            For Each item In result
                PlanningSideDgList.Add(item)
            Next

        Catch ex As Exception
            MessageBox.Show("Error loading details: " & ex.Message)
        End Try
    End Sub


    Public Shared Function GetDetailsByBOM(bomNo As String) As List(Of PlanningSideDgModel)
        Dim result As New List(Of PlanningSideDgModel)()
        Dim conString = ConfigurationManager.ConnectionStrings("Db_Server").ConnectionString

        Using con As New SqlConnection(conString)
            Dim cmd As New SqlCommand("SELECT * FROM v_MT_PPC WHERE [BOM No.] = @BOMNo", con)
            cmd.Parameters.AddWithValue("@BOMNo", bomNo)

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
                        .Jobcard = rdr("JobCard").ToString(),
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



    Private Async Sub LoadJobCardDataAsync(wid As String)
        Try
            Dim jobCards = Await Task.Run(Function() GetJobCardsByWID(wid))
            JobCardList = New ObservableCollection(Of JobCardModel)(jobCards)
        Catch ex As Exception
            Debug.WriteLine("Error loading JobCards: " & ex.Message)
        End Try
    End Sub

    Private Shared Function GetJobCardsByWID(wid As String) As List(Of JobCardModel)
        Dim result As New List(Of JobCardModel)()
        Dim conString = ConfigurationManager.ConnectionStrings("Db_Server").ConnectionString

        Using con As New SqlConnection(conString)
            Dim cmd As New SqlCommand("SELECT * FROM RoutingUploadTable WHERE WID = @WID", con)
            cmd.Parameters.AddWithValue("@WID", wid)

            con.Open()
            Using rdr = cmd.ExecuteReader()
                While rdr.Read()
                    result.Add(New JobCardModel With {
                        .JC_no = rdr("JC_no").ToString(),
                        .WID = rdr("WID").ToString(),
                        .OP_Sequence = rdr("OP_Sequence").ToString(),
                        .Operation_ID = rdr("Operation_ID").ToString(),
                        .Created_date = rdr("Created_date"),
                        .Created_by = rdr("Created_by").ToString()
                    })
                End While
            End Using
        End Using

        Return result
    End Function

#End Region

#Region "Command Logic"

    Private Sub CreateJobCard()
        If PlanningSideDgList Is Nothing Then Return

        Dim selectedItems = PlanningSideDgList.Where(Function(x) x.IsSelectedToCreate).ToList()

        If selectedItems.Count = 0 Then
            MessageBox.Show("No BOMs selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning)
            Return
        End If

        Try
            Dim conString = ConfigurationManager.ConnectionStrings("Db_Server").ConnectionString

            Using con As New SqlConnection(conString)
                Using cmd As New SqlCommand("sp_CreateJobCard_ByWID", con)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@WID", SelectedBOM.WID)
                    cmd.Parameters.AddWithValue("@CreatedBy", Environment.UserName)

                    con.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            ' Refresh job cards and update command state
            LoadJobCardDataAsync(SelectedBOM.WID)
            RaiseCreateJobCardCanExecuteChanged()

        Catch ex As Exception
            MessageBox.Show("Error creating job card: " & ex.Message)
        End Try
    End Sub

    Private Function CanCreateJobCard() As Boolean
        If SelectedBOM Is Nothing Then Return False
        Return Not JobCardList.Any(Function(j) j.WID = SelectedBOM.WID)
    End Function

    Private Sub RaiseCreateJobCardCanExecuteChanged()
        If _createJobCardCommand IsNot Nothing Then
            DirectCast(_createJobCardCommand, RelayCommand).RaiseCanExecuteChanged()
        End If
    End Sub

#End Region

#Region "INotifyPropertyChanged"
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub OnPropertyChanged(name As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
#End Region




    Private _SelectAllJobCards As Boolean
    Public Property SelectAllJobCards As Boolean
        Get
            Return _SelectAllJobCards
        End Get
        Set(value As Boolean)
            _SelectAllJobCards = value
            OnPropertyChanged(NameOf(SelectAllJobCards))

            ' When changed, update all items in PlanningSideDgList
            If PlanningSideDgList IsNot Nothing Then
                For Each item In PlanningSideDgList
                    item.IsSelectedToCreate = value
                Next
            End If
        End Set
    End Property



    Public Sub RefreshPlanningSideDgList()
        If SelectedProj IsNot Nothing Then
            LoadDetailsFromDatabaseAsync(SelectedProj.PROJECTNO)
        End If
    End Sub





End Class
