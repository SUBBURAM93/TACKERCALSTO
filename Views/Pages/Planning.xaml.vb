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
        ' Get filtered items from the WIDBOMDG
        For Each item In WIDBOMDG.Items
            Dim widItem = TryCast(item, PlanningSideDgModel) ' Replace with your actual model class name
            If widItem IsNot Nothing AndAlso widItem.IsSelectedToCreate Then
                CreateJobCardInDB(widItem.WID)
            End If
        Next

        MessageBox.Show("Job card(s) created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information)
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
        Catch ex As Exception
            MessageBox.Show($"Error creating job card for WID {wid}:{Environment.NewLine}{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub DeleteJobCard_Click(sender As Object, e As RoutedEventArgs)
        For Each item In WIDBOMDG.Items
            Dim widItem = TryCast(item, PlanningSideDgModel) ' Replace with your actual model
            If widItem IsNot Nothing AndAlso widItem.IsSelectedToCreate Then
                DeleteJobCardInDB(widItem.WID)
            End If
        Next

        MessageBox.Show("Job card(s) deleted successfully!", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information)
    End Sub

    Private Sub WIDBOMDG_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim selectedItem = TryCast(WIDBOMDG.SelectedItem, PlanningSideDgModel) ' Replace with your model class
        If selectedItem IsNot Nothing Then
            Dim result = MessageBox.Show($"Do you want to delete job card for WID: {selectedItem.WID}?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question)
            If result = MessageBoxResult.Yes Then
                DeleteJobCardInDB(selectedItem.WID)
            End If
        End If
    End Sub


    Private Sub DeleteJobCardInDB(wid As String)
        Try
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("YourConnectionStringName").ConnectionString
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
