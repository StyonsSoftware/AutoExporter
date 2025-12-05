Imports System.Net
Imports System.Threading
Imports System.Text
Imports System.Data.SqlClient

Public Class CRMExport
    Private Const MS_BETWEEN_STATUS_CHECKS As Integer = 5000
    Private _params As Parameters
    Private _provider As Blackbaud.AppFx.WebAPI.AppFxWebServiceProvider

    Public ExportID As Guid
    Public Name As String
    Public Status As ExportStatus
    Public OutputFileName As String
    Public IgnoredColumns As List(Of Integer)

    Public Sub New(parms As Parameters)
        _params = parms
        _provider = Util.GetProvider(_params.AppfxWebServiceURL, _params.DBName, _params.CRMUserName, _params.CRMPassword)
        Status = New ExportStatus(String.Empty, Nothing)
        IgnoredColumns = IntegerCSVToIntegerList(_params.IgnoredColumns)
    End Sub

    Private Function IntegerCSVToIntegerList(s As String) As List(Of Integer)
        'turn "1,2,3" into a list(of integer) with 1/2/3.
        'Ignore invalid integers.
        'Always return at least the empty list.
        Dim result As New List(Of Integer)
        If String.IsNullOrEmpty(s) Then
            Return result
        End If
        Dim integerCandidates As List(Of String) = s.Split(",").ToList()
        Dim intCan As Integer
        For Each integerCandidate As String In integerCandidates
            If Integer.TryParse(integerCandidate, intCan) Then
                result.Add(intCan)
            End If
        Next
        Return result
    End Function

    Public Sub New()
        If _provider Is Nothing Then
            Throw New Exception("You must specify a provider")
        End If
    End Sub

    Sub DropToFolder()
        PerformExport(ExportID)
    End Sub

    Public Sub RefreshExportStatus()
        If Status Is Nothing Then
            Status = New ExportStatus(Name, Blackbaud.AppFx.Platform.Catalog.WebApiClient.ViewForms.BusinessProcessParameterSet.BusinessProcessParameterSetRecentStatusViewForm.LoadData(_provider, ExportID.ToString()))
            Status.OutputTableName = String.Empty
        Else
            Status.CRMStatus = Blackbaud.AppFx.Platform.Catalog.WebApiClient.ViewForms.BusinessProcessParameterSet.BusinessProcessParameterSetRecentStatusViewForm.LoadData(_provider, ExportID.ToString())
        End If
        Util.Log("Checking for output table for export id=" + ExportID.ToString(), True, _params)
        Util.Log("Still working on: " + Name + ".", True, _params)
        Dim filterValue As New AutoExporter.MetalWeb.DataLists.TopLevel.ExportOutputSetsDataListFilterData()
        filterValue.BUSINESSPROCESSPARAMETERSETID = ExportID
        filterValue.MINIMUMDATE = DateTime.Now.AddDays(-1)
        For Each dlRow In AutoExporter.MetalWeb.DataLists.TopLevel.ExportOutputSetsDataList.GetRows(_provider, filterValue)
            Status.OutputTableName = dlRow.TABLENAME
        Next
        If String.IsNullOrEmpty(Status.OutputTableName) Then
            Util.Log("No output table found, the export is still running", True, _params)
        End If
    End Sub

    Private Sub PerformExport(exportID As Guid)
        Dim exportBusinessProcessid As Guid = New Guid("64faa344-9c75-4c98-afe3-a40ec2df9249")
        Dim serverName As String = _provider.Url.Split("/")(2)
        Dim virDirName As String = _provider.Url.Split("/")(3)
        Dim startTime As DateTime = DateTime.Now()
        Util.Log("Start time was: " + startTime.ToString("MM/dd/yyyy HH:mm:ss"), False, _params)
        KickOffBusinessProcess(serverName, virDirName, _provider.Database, exportBusinessProcessid, exportID)

        RefreshExportStatus()
        While Not Status.CRMStatus.COMPLETED
            RefreshExportStatus()
            Dim numberOfSeconds As Integer = (DateTime.Now - startTime).TotalSeconds
            Util.Log("Checking status.  Elapsed time is: " + numberOfSeconds.ToString + " seconds", False, _params)
            If (0 < _params.TimeoutSeconds) AndAlso (numberOfSeconds > _params.TimeoutSeconds) Then
                Util.Log("Timeout was set to " + _params.TimeoutSeconds.ToString + " seconds, but this export has run for " + numberOfSeconds.ToString + ".  Moving on to the next one...", False, _params)
                Exit Sub
            End If
            Thread.Sleep(MS_BETWEEN_STATUS_CHECKS)
        End While
        'ok, now it's complete.  write the output to the folder.
        DownloadTable(Status.OutputTableName, IgnoredColumns)
    End Sub

    Private Function KickOffBusinessProcess(server As String, virdir As String, dbName As String, businessProcessID As Guid, parameterSetID As Guid) As String
        'To run a business process from code, you have to call a generic "business process handler" with the right parameters.
        'specifically, you need to pass in the business process id and the id of it's parameter set.        
        Dim result As String = String.Empty
        Try
            Dim useHTTPS As Boolean = _params.AppfxWebServiceURL.StartsWith("https")
            Dim unformattedEndpoint As String = "http" + IIf(useHTTPS, "s", String.Empty) + "://[SERVER]/[VIRDIR]/BusinessProcessInvoke.ashx?DatabaseName=[DBNAME]&BusinessProcessID=[BUSPROCID]&ParameterSetID=[PARAMETERSETID]"
            Dim RESTendpoint As String = unformattedEndpoint
            RESTendpoint = RESTendpoint.Replace("[SERVER]", server)
            RESTendpoint = RESTendpoint.Replace("[VIRDIR]", virdir)
            RESTendpoint = RESTendpoint.Replace("[DBNAME]", dbName)
            RESTendpoint = RESTendpoint.Replace("[BUSPROCID]", businessProcessID.ToString()) 'the "Export Process" business process
            RESTendpoint = RESTendpoint.Replace("[PARAMETERSETID]", parameterSetID.ToString)

            Using client As New WebClient()
                client.Headers("User-Agent") = "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) " + "(compatible; MSIE 6.0; Windows NT 5.1; " + ".NET CLR 1.1.4322; .NET CLR 2.0.50727)"
                If (_params.CRMUserName IsNot Nothing AndAlso 0 < _params.CRMUserName.Trim.Length) OrElse (_params.CRMPassword IsNot Nothing AndAlso 0 < _params.CRMPassword.Trim.Length) Then
                    client.Credentials = New System.Net.NetworkCredential(_params.CRMUserName, _params.CRMPassword)
                Else
                    client.Credentials = System.Net.CredentialCache.DefaultCredentials
                End If

                Util.Log("About to kick off export process with this URL: """ + RESTendpoint + """", True, _params)

                'This will kick off the business process, but it just responds with a "success" or "failed" message.
                result = client.DownloadString(RESTendpoint)
            End Using
        Catch ex As Exception
            result = "Error: " + ex.Message
        End Try
        Return result
    End Function

    Private Sub DownloadTable(outputTableName As String, ignoreColumns As List(Of Integer))
        Try
            Dim sqlStatement As String = "SELECT * FROM " + outputTableName
            Util.Log("SQL for output download: " + sqlStatement, True, _params)
            If 0 < outputTableName.Trim.Length Then
                'grab the output table with a direct sql connection, turn it into a giant string, then save that string to a file
                'this is crude and could be improved.
                Dim conn As SqlConnection = Util.GetSQLConnection(_params.SQLInstanceName, _params.SQLDBName)
                conn.Open()
                Dim sqladapter As New SqlDataAdapter("SELECT * FROM " + outputTableName, conn)
                Dim resultTable As New DataTable("EXPORT")
                sqladapter.Fill(resultTable)
                Dim csv As String = ExportTableToCsv(resultTable, ignoreColumns)
                OutputFileName = GetFileName()
                WriteStringToFile(csv, OutputFileName)
                conn.Close()
            Else
                Util.Log("No download because output table was not found", True, _params)
            End If
        Catch ex As Exception
            Util.Log("DownloadTable failed with error: " + ex.Message, True, _params)
        End Try
    End Sub

    Private Function GetFileName() As String
        If String.IsNullOrEmpty(_params.OutputFilename) Then
            _params.OutputFilename = Name
        End If
        Return _params.OutputFolder + IIf(_params.OutputFolder.EndsWith("\"), String.Empty, "\") + _params.OutputFilename + ".csv"
    End Function

    Private Sub WriteStringToFile(s As String, filename As String)
        Dim fs As New System.IO.FileStream(filename, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None)
        Dim wr As New System.IO.StreamWriter(fs)
        wr.Write(s)
        wr.Close()
        fs.Close()
    End Sub

    Private Shared Function ExportTableToCsv(sourceTable As DataTable, excludeColumns As List(Of Integer)) As String
        'turn a datatable into a giant CSV string
        Dim sb As New StringBuilder()

        For rowCount As Integer = 0 To sourceTable.Rows.Count - 1
            For colCount As Integer = 0 To sourceTable.Columns.Count - 1
                If Not excludeColumns.Contains(colCount) Then
                    sb.Append(sourceTable.Rows(rowCount)(colCount))
                    If colCount <> sourceTable.Columns.Count - 1 Then
                        sb.Append(",")
                    End If
                End If
            Next
            If rowCount <> sourceTable.Rows.Count - 1 Then
                sb.AppendLine()
            End If
        Next
        Return sb.ToString()
    End Function

    Private Function GetSQLConnection(instanceName As String, sqlDBName As String) As SqlConnection
        'build a sql connection based on the values entered by the user on the form
        Dim connStrBuilder As New SqlConnectionStringBuilder()
        connStrBuilder.DataSource = instanceName
        connStrBuilder.InitialCatalog = sqlDBName
        connStrBuilder.IntegratedSecurity = True
        Dim result As New SqlConnection(connStrBuilder.ConnectionString)
        Return result
    End Function
End Class