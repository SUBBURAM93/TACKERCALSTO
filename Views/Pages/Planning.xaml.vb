Imports TACKERCALSTO.PlanningModel
Imports System.Data.SqlClient
Imports System.Configuration
Imports Microsoft.Data.SqlClient
Imports System.Data

Class Planning
    Inherits Page

    Public Sub New()
        InitializeComponent()
        Try
            Me.DataContext = New PlanningViewModel()
        Catch ex As Exception
            MessageBox.Show("Constructor error: " & ex.Message)
        End Try
    End Sub




    Private Sub SelectAllFilteredCheckBox_Click(sender As Object, e As RoutedEventArgs)
        Dim isChecked As Boolean = SelectAllFilteredCheckBox.IsChecked = True

        For Each item In WIDBOMDG.Items
            Dim row = TryCast(item, PlanningSideDgModel)
            If row IsNot Nothing Then
                row.IsSelectedToCreate = isChecked
            End If
        Next
    End Sub

    Private Sub CreateJobCard_Click(sender As Object, e As RoutedEventArgs)
        ' Get the ViewModel from DataContext
        Dim vm = TryCast(Me.DataContext, PlanningViewModel)
        If vm Is Nothing Then
            MessageBox.Show("Unable to access data context.")

            Return
        End If

        Dim selectedCount As Integer = 0

        Dim selected = vm.PlanningSideDgList.Where(Function(x) x.IsSelectedToCreate).ToList()

        If selected.Count = 0 Then
            MessageBox.Show("No items selected - check if checkbox is bound properly")
            Return
        End If


        ' Loop through the full list bound to the DataGrid
        For Each item In vm.PlanningSideDgList
            If item IsNot Nothing AndAlso item.IsSelectedToCreate Then
                CreateJobCardInDB(item.WID)
                selectedCount += 1
            End If
        Next

        If selectedCount > 0 Then
            MessageBox.Show("Job card(s) created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information)
            vm.RefreshPlanningSideDgList()
        Else
            MessageBox.Show("No items selected to create job cards.", "Notice", MessageBoxButton.OK, MessageBoxImage.Warning)
        End If
    End Sub



    Private Sub CreateJobCardInDB(wid As String)
        Try

            Dim connectionString As String = ConfigurationManager.ConnectionStrings("Db_Server").ConnectionString
            Using con As New SqlConnection(connectionString)
                Using cmd As New SqlCommand("sp_CreateJobCard_ByWID", con)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@WID", wid)
                    cmd.Parameters.AddWithValue("@CreatedBy", Environment.UserName)

                    con.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("many rows selected")
        Catch ex As Exception
            MessageBox.Show($"Error creating job card for WID {wid}:{Environment.NewLine}{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub DeleteJobCard_Click(sender As Object, e As RoutedEventArgs)
        Dim selectedItem = TryCast(WIDBOMDG.SelectedItem, PlanningSideDgModel)
        Dim vm = TryCast(Me.DataContext, PlanningViewModel)

        If selectedItem Is Nothing Then
            MessageBox.Show("Please select a WID to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning)
            Return
        End If

        Dim result = MessageBox.Show($"Do you want to delete job card for WID: {selectedItem.WID}?",
                                 "Confirm Deletion",
                                 MessageBoxButton.YesNo,
                                 MessageBoxImage.Question)

        If result = MessageBoxResult.Yes Then
            DeleteJobCardInDB(selectedItem.WID)

            ' Refresh data after deletion (you can reload your table here)
            vm.RefreshPlanningSideDgList() ' Replace with your method to reload data
        End If
    End Sub

    Private Sub DeleteJobCardInDB(wid As String)
        Try
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("Db_Server").ConnectionString
            Using con As New SqlConnection(connectionString)
                Using cmd As New SqlCommand("sp_DeleteJobCard_ByWID", con)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@WID", wid)

                    con.Open()
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show($"Job card for WID {wid} deleted successfully.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information)

        Catch ex As Exception
            MessageBox.Show($"Error deleting job card for WID {wid}:{Environment.NewLine}{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub



End Class
