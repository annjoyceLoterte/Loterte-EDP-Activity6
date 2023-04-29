Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Data.SqlClient
Imports System.Text

Public Class CampusForm
    'adding
    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        With Me
            Call Connect_to_DB()
            Dim mycmd As New MySqlCommand
            Try
                strSQL = "Insert into campus values('" _
                & .Textcampus_code.Text & "', '" _
                & .Textcampus_address.Text & "', '" & .Textcampus_dean _
                .Text & "')"

                mycmd.CommandText = strSQL
                mycmd.Connection = myconn
                mycmd.ExecuteNonQuery()
                MsgBox("Record Successfully Added!")
                Call Clear_Boxes()
            Catch ex As MySqlException
                MsgBox(ex.Number & " " & ex.Message)
            End Try
            Disconnect_to_DB()
        End With
    End Sub
    Private Sub Clear_Boxes()
        With Me
            .Textcampus_code.Text = ""
            .Textcampus_address.Text = ""
            .Textcampus_dean.Text = ""
        End With
    End Sub

    'updating
    Private Sub UpdateButton_Click(sender As Object, e As EventArgs) Handles UpdateButton.Click
        With Me
            Call Connect_to_DB()
            Dim mycmd As New MySqlCommand
            Try
                strSQL = "Update campus set campus_address= ('" & .Textcampus_address.Text & "'), 
                campus_dean= ('" & .Textcampus_dean.Text & "')
                where campus_code= '" & .Textcampus_code.Text & "'"
                mycmd.CommandText = strSQL
                mycmd.Connection = myconn
                mycmd.ExecuteNonQuery()
                MsgBox("Record Successfully Updated!")
                Call Clear_Boxes()
            Catch ex As MySqlException
                MsgBox(ex.Number & " " & ex.Message)
            End Try
            Disconnect_to_DB()
        End With
    End Sub

    'deleting
    Private Sub deleteButton_Click(sender As Object, e As EventArgs) Handles deleteButton.Click
        With Me
            Call Connect_to_DB()
            Dim mycmd As New MySqlCommand
            Try
                strSQL = "Delete from campus where campus_code= '" & .Textcampus_code.Text & "'"
                mycmd.CommandText = strSQL
                mycmd.Connection = myconn
                mycmd.ExecuteNonQuery()
                MsgBox("Record Successfully Deleted!")
                Call Clear_Boxes()
            Catch ex As MySqlException
                MsgBox(ex.Number & " " & ex.Message)
            End Try
            Disconnect_to_DB()
        End With
    End Sub

    'Private Sub buttonLoad_Click(sender As Object, e As EventArgs)
    '    Me.dgcampuses.Rows.Clear()
    '    Dim strsql As String
    '    Dim mycmd As New MySqlCommand
    '    strsql = "Select * from campus"
    '    Connect_to_DB()
    '    With mycmd
    '        .Connection = myconn
    '        .CommandType = CommandType.Text
    '        .CommandText = strsql
    '    End With
    '    Dim myreader As MySqlDataReader
    '    myreader = mycmd.ExecuteReader()
    '    While myreader.Read()
    '        Me.dgcampuses.Rows.Add(New Object() {myreader.Item("campus_code"), myreader.Item("campus_address"), myreader.Item("campus_dean")})
    '    End While
    '    Disconnect_to_DB()
    'End Sub
    Private Sub gobackbtn_Click_1(sender As Object, e As EventArgs) Handles gobackbtn.Click
        Hide()
        MenuForm3.Show()
    End Sub

    'export to csv
    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Dim tableName As String = "campus"

        Dim saveDialog As New SaveFileDialog()
        saveDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
        saveDialog.Title = "Export to CSV"
        saveDialog.FileName = tableName + ".csv"

        If saveDialog.ShowDialog() = DialogResult.OK Then
            ExportToCSV(tableName, saveDialog.FileName)
        End If
    End Sub
    'export to csv
    Private Sub ExportToCSV(tableName As String, fileName As String)
        Dim query As String = "SELECT * FROM " + tableName
        Dim adapter As New MySqlDataAdapter(query, myconn)
        Dim dataTable As New DataTable()

        myconn.Open()
        adapter.Fill(dataTable)
        myconn.Close()

        Dim csv As New StringBuilder()

        ' Add column names to CSV file
        Dim headerLine As String = String.Empty
        For Each column As DataColumn In dataTable.Columns
            headerLine += column.ColumnName + ","
        Next
        headerLine = headerLine.TrimEnd(",")
        csv.AppendLine(headerLine)

        ' Add rows to CSV file
        For Each row As DataRow In dataTable.Rows
            Dim line As String = ""
            For i As Integer = 0 To dataTable.Columns.Count - 1
                line += row(i).ToString() + ","
            Next
            line = line.TrimEnd(",")
            csv.AppendLine(line)
        Next

        File.WriteAllText(fileName, csv.ToString())
        MessageBox.Show("Data exported to " + fileName)
    End Sub

    Private Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click
        Dim openDialog As New OpenFileDialog()
        openDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
        openDialog.Title = "Upload from CSV"

        If openDialog.ShowDialog() = DialogResult.OK Then
            UploadFromCSV(openDialog.FileName)
        End If
    End Sub
    Private Sub UploadFromCSV(fileName As String)
        ' Open the CSV file
        Dim csv As New StreamReader(fileName)

        ' Read the column names from the first line
        Dim columns As String() = csv.ReadLine().Split(","c)

        ' Loop through the remaining lines
        While Not csv.EndOfStream
            ' Read the values from the line
            Dim values As String() = csv.ReadLine().Split(","c)

            ' Insert the values into the database
            myconn.Open()

            Dim query As String = "INSERT INTO campus (" + String.Join(", ", columns) + ") VALUES (" + String.Join(", ", values.Select(Function(v) "'" + v + "'")) + ")"
            Dim cmd As New MySqlCommand(query, myconn)
            cmd.ExecuteNonQuery()

            myconn.Close()
        End While

        ' Close the CSV file
        csv.Close()

        ' Display a message to the user
        MessageBox.Show("Data successfully uploaded from " + fileName)
    End Sub
    'printing to excel
    Public sqlColumns As String = "campus_address as CampusAddress, campus_code as Code, campus_dean as Dean"
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
                dgcampuses.AutoSize = True
                .dgcampuses.Refresh()
                .dgcampuses.EndEdit()

                ' Create a Panel container control
                Dim panel As New Panel()
                panel.Dock = DockStyle.Fill
                panel.AutoScroll = True
                .Controls.Add(panel)

                ' Add the DataGridView to the Panel
                panel.Controls.Add(dgcampuses)

                dgcampuses.Dock = DockStyle.Top
                dgcampuses.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
                dgcampuses.ScrollBars = ScrollBars.None ' Remove this line as it is not needed anymore
                dgcampuses.DataSource = mydatatable
                dgcampuses.ReadOnly = True
                dgcampuses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells

            Catch ex As MySqlException
                MsgBox(ex.Message, MsgBoxStyle.Critical, "Error on SQL query")
            End Try
            myreader = Nothing
            mycommand = Nothing
            Disconnect_to_DB()
        End With
    End Sub
    Private Sub Formcourses_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        strSQL = "select " & Me.sqlColumns & " from campus"
        'Me.Label1.Text = strSQL
        Call Load_Data_to_Grid("select " & Me.sqlColumns & " from campus")
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        'MsgBox(currentDate.ToString)
        Call importToExcel(Me.dgcampuses, "campus_report.xlsx")
    End Sub
End Class
