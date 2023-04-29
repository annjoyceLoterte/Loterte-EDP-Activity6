Imports System.Data.Common
Imports Excel = Microsoft.Office.Interop.Excel
Imports MySql.Data.MySqlClient
Public Class frmDatagrid
    Public sqlColumns As String = "course_name as Course_Name, cNO as Course_No"
    Private Sub Load_Data_to_Grid(ByVal strsql As String)
        Dim myreader As MySqlDataReader
        Dim mycommand As New MySqlCommand
        Dim mydataAdapter As New MySqlDataAdapter
        Dim mydatatable As New DataTable

        Connect_to_DB()
        With Me
            Try
                mycommand.Connection = myconn
                mycommand.CommandText = strsql
                myreader = mycommand.ExecuteReader
                mydatatable = New DataTable

                myreader.Close()
                mydataAdapter.SelectCommand = mycommand

                mydataAdapter.Fill(mydatatable)
                dgreport.AutoSize = True
                .dgreport.Refresh()
                .dgreport.EndEdit()
                .dgreport.DataSource = mydatatable
                .dgreport.ReadOnly = True
                .dgreport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
                'dgreport.Columns("course_code").DefaultCellStyle.Format = DataGridViewContentAlignment.MiddleCenter
                'dgreport.Columns("course_name").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                'dgreport.Columns("course_major").DefaultCellStyle.Format = DataGridViewContentAlignment.MiddleCenter
                'dgreport.Columns("tuition_fee_perSem").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                'dgreport.Columns("school_deptUnicode").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

            Catch ex As MySqlException
                MsgBox(ex.Message, MsgBoxStyle.Critical, "Error on SQL query")
            End Try
            myreader = Nothing
            mycommand = Nothing
            Disconnect_to_DB()
        End With
    End Sub

    Private Sub frmDatagrid_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'strSQL = "select " & Me.sqlColumns & " from course_details"
        ' Me.Label1.Text = strSQL
        Call Load_Data_to_Grid("select " & Me.sqlColumns & " from course_details")
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        'MsgBox(currentDate.ToString)
        Call importToExcel(Me.dgreport, "courses.xlsx")
    End Sub
End Class