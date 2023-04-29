Imports MySql.Data.MySqlClient
Imports System.Data.Common
Imports System.Data.SqlClient
Imports System.IO
Imports System.Text

Public Class Formcourses

    Private Sub Clear_Boxes()
        With Me
            .Textc_code.Text = ""
            .Textcourse_name.Text = ""
            .Textcourse_major.Text = ""
            .TextTuition_fee_perSem.Text = ""
            .Textschool_deptUnicode.Text = ""
            .Textcno.Text = ""
        End With
    End Sub
    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Hide()
        MenuForm3.Show()
    End Sub
    'adding
    Private Sub btnAdd_Click_1(sender As Object, e As EventArgs) Handles btnAdd.Click
        With Me
            Call Connect_to_DB()
            Dim mycmd As New MySqlCommand
            Try
                strSQL = "Insert into course_details values('" & .Textc_code.Text & "', '" _
                & .Textcourse_name.Text & "', '" & .Textcourse_major.Text & "', '" _
                & .TextTuition_fee_perSem.Text & "' , '" & .Textschool_deptUnicode.Text & "', '" _
                & .Textcno.Text & "')"

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

    'updating
    Private Sub UpdateButton_Click(sender As Object, e As EventArgs) Handles UpdateButton.Click
        With Me
            Call Connect_to_DB()
            Dim mycmd As New MySqlCommand
            Try
                strSQL = "Update course_details set course_name= ('" & .Textcourse_name.Text & "'),
                course_major= ('" & .Textcourse_major.Text & "'), 
                tuition_fee_perSem= ('" & .TextTuition_fee_perSem.Text & "'),
                school_deptUnicode= ('" & .Textschool_deptUnicode.Text & "'), cNo= ('" & .Textcno.Text & "')
                where course_code= '" & .Textc_code.Text & "'"
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
                strSQL = "Delete from course_details where course_code= '" & .Textc_code.Text & "'"
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
    'loading
    'Private Sub btnLoad_Click(sender As Object, e As EventArgs) Handles btnLoad.Click
    '    Me.dgcourses.Rows.Clear()
    '    Dim strsql As String
    '    Dim mycmd As New MySqlCommand
    '    strsql = "Select * from course_details"
    '    Connect_to_DB()
    '    With mycmd
    '        .Connection = myconn
    '        .CommandType = CommandType.Text
    '        .CommandText = strsql
    '    End With
    '    Dim myreader As MySqlDataReader
    '    myreader = mycmd.ExecuteReader()
    '    While myreader.Read()
    '        Me.dgcourses.Rows.Add(New Object() {myreader.Item("cNo"), myreader.Item("course_code"), myreader.Item("course_name"), myreader.Item("course_major"), myreader.Item("tuition_fee_perSem"), myreader.Item("school_deptUnicode")})
    '    End While
    '    Disconnect_to_DB()
    'End Sub

    'exporting to csv
    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Dim tableName As String = "course_details"
        Dim saveDialog As New SaveFileDialog()
        saveDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
        saveDialog.Title = "Export to CSV"
        saveDialog.FileName = tableName + ".csv"

        If saveDialog.ShowDialog() = DialogResult.OK Then
            ExportToCSV(tableName, saveDialog.FileName)
        End If
    End Sub
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

    'uploading csv
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

            Dim query As String = "INSERT INTO course_details (" + String.Join(", ", columns) + ") VALUES (" + String.Join(", ", values.Select(Function(v) "'" + v + "'")) + ")"
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
    Public sqlColumns As String = "course_name as Course_Name, cNO as Course_No, course_code as Course_code, 
    course_major as Major, tuition_fee_perSem as Tuition, school_deptUnicode as Department"
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
                dgcourses.AutoSize = True
                .dgcourses.Refresh()
                .dgcourses.EndEdit()

                ' Create a Panel container control
                Dim panel As New Panel()
                panel.Dock = DockStyle.Fill
                panel.AutoScroll = True
                .Controls.Add(panel)

                ' Add the DataGridView to the Panel
                panel.Controls.Add(dgcourses)

                dgcourses.Dock = DockStyle.Top
                dgcourses.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
                dgcourses.ScrollBars = ScrollBars.None ' Remove this line as it is not needed anymore
                dgcourses.DataSource = mydatatable
                dgcourses.ReadOnly = True
                dgcourses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
                'dgcourses.Columns("course_code").DefaultCellStyle.Format = DataGridViewContentAlignment.MiddleCenter
                'dgcourses.Columns("course_name").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
                'dgcourses.Columns("course_major").DefaultCellStyle.Format = DataGridViewContentAlignment.MiddleCenter
                'dgcourses.Columns("tuition_fee_perSem").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                'dgcourses.Columns("school_deptUnicode").DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft

            Catch ex As MySqlException
                MsgBox(ex.Message, MsgBoxStyle.Critical, "Error on SQL query")
            End Try
            myreader = Nothing
            mycommand = Nothing
            Disconnect_to_DB()
        End With
    End Sub
    Private Sub Formcourses_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        strSQL = "select " & Me.sqlColumns & " from course_details"
        'Me.Label1.Text = strSQL
        Call Load_Data_to_Grid("select " & Me.sqlColumns & " from course_details")
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        'MsgBox(currentDate.ToString)
        Call importToExcel(Me.dgcourses, "courses_report.xlsx")
    End Sub
End Class