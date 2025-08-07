' Services/DatabaseService.vb
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Collections.ObjectModel
Imports Microsoft.Data.SqlClient

Public Class DatabaseService
    Private ReadOnly _connectionString As String = ConfigurationManager.ConnectionStrings("Db_Server").ConnectionString

    Public Async Function GetPlanningListAsync() As Task(Of ObservableCollection(Of PlanningModel))
        Dim list As New ObservableCollection(Of PlanningModel)()

        Using con As New SqlConnection(_connectionString)
            Await con.OpenAsync()

            Using cmd As New SqlCommand("SELECT [PROJECT NO], [CUSTOMER NAME] FROM [CASPRO].[dbo].[PROJTAB]", con)
                Using rdr = Await cmd.ExecuteReaderAsync()
                    While Await rdr.ReadAsync()
                        list.Add(New PlanningModel With {
                            .PROJECTNO = rdr("PROJECT NO").ToString(),
                            .Customer = rdr("CUSTOMER NAME").ToString()
                        })
                    End While
                End Using
            End Using
        End Using

        Return list
    End Function

    Public Async Function GetJobFormingListAsync() As Task(Of ObservableCollection(Of JobModel))
        Dim list As New ObservableCollection(Of JobModel)()

        Using con As New SqlConnection(_connectionString)
            Await con.OpenAsync()

            Using cmd As New SqlCommand("SELECT * FROM V_JOB_FORMING", con)
                Using rdr = Await cmd.ExecuteReaderAsync()
                    While Await rdr.ReadAsync()
                        list.Add(New JobModel With {
                            .BOMName = rdr("BOM Name").ToString(),
                            .JC_no = rdr("JC_no").ToString(),
                            .PartNumber = rdr("Part Number").ToString(),
                            .Description = rdr("Description").ToString(),
                            .Length = Convert.ToDecimal(rdr("Length")),
                            .Width = Convert.ToDecimal(rdr("Width")),
                            .Thick = Convert.ToDecimal(rdr("Thick")),
                            .UnitWeight = Convert.ToDecimal(rdr("Unit Weight")),
                            .TotalWeight = Convert.ToDecimal(rdr("Total Weight")),
                            .Colour = rdr("Colour").ToString(),
                            .BOMQty = Convert.ToDecimal(rdr("BOM Qty")),
                            .ProducedQty = Convert.ToDecimal(rdr("Produced Qty")),
                            .BalanceQty = Convert.ToDecimal(rdr("Balance Qty")),
                            .DispatchDate = Convert.ToDateTime(rdr("Dispatch Date")),
                            .DueDate = Convert.ToInt32(rdr("Due Date"))
                        })
                    End While
                End Using
            End Using
        End Using

        Return list
    End Function
End Class
