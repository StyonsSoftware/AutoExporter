Imports System.IO

Module Main
    Private _params As Parameters

    Private Function UIToParams() As Parameters
        If _params IsNot Nothing Then
            Return _params
        Else
            _params = New Parameters
            _params.Debug = False
            _params.SimultaneousCount = 1
            _params.StatsOutputFileName = String.Empty

            Dim CommandLineArgs As System.Collections.ObjectModel.ReadOnlyCollection(Of String) = My.Application.CommandLineArgs
            For Each arg In CommandLineArgs
                Dim components As String() = arg.Split("=")
                If components IsNot Nothing Then
                    components(0) = components(0).Replace("/", "-").ToUpper()
                    If 2 = components.Count Then
                        Select Case components(0)
                            Case "-URL"
                                _params.AppfxWebServiceURL = components(1)
                            Case "-DB"
                                _params.DBName = components(1)
                            Case "-OUTPUTFOLDER"
                                _params.OutputFolder = components(1)
                            Case "-SIMULTANEOUS"
                                _params.SimultaneousCount = components(1)
                            Case "-STATFILE"
                                _params.StatsOutputFileName = components(1)
                            Case "-SQLINSTANCENAME"
                                _params.SQLInstanceName = components(1)
                            Case "-SQLDBNAME"
                                _params.SQLDBName = components(1)
                            Case "-CRMUSERNAME"
                                _params.CRMUserName = components(1)
                            Case "-CRMPASSWORD"
                                _params.CRMPassword = components(1)
                            Case "-TIMEOUT"
                                If Not Integer.TryParse(components(1), _params.TimeoutSeconds) Then
                                    _params.TimeoutSeconds = -1
                                End If
                            Case "-EXPORTNAME"
                                _params.ExportName = components(1)
                            Case "-IGNORECOLUMNS"
                                _params.IgnoredColumns = components(1)
                        End Select
                    ElseIf 1 = components.Count Then
                        Select Case components(0)
                            Case "-DEBUG"
                                _params.Debug = True
                        End Select
                    End If
                End If
            Next
            Return _params
        End If
    End Function

    Private Sub ParamsToUI(params As Parameters)
        Console.WriteLine("URL=" + params.AppfxWebServiceURL)
        Console.WriteLine("DB=" + params.DBName)
        Console.WriteLine("OUTPUTFOLDER=" + params.OutputFolder)
        Console.WriteLine("SIMULTANEOUS=" + params.SimultaneousCount.ToString)
        Console.WriteLine("STATSFILE=" + params.StatsOutputFileName)
        Console.WriteLine("SQLINSTANCENAME=" + params.SQLInstanceName)
        Console.WriteLine("SQLDBNAME=" + params.SQLDBName)
        Console.WriteLine("DEBUG=" + params.Debug.ToString())
        Console.WriteLine("TIMEOUT=" + params.TimeoutSeconds.ToString())
        Console.WriteLine("CRMUSERNAME=" + params.CRMUserName)
        Console.WriteLine("CRMPASSWORD=**********") ' + params.CRMPassword)
        Console.WriteLine("EXPORTNAME=" + params.ExportName)
        Console.WriteLine("IGNORECOLUMNS=" + params.IgnoredColumns)
    End Sub

    Private Sub ShowHelp()
        Console.WriteLine("")
        Console.WriteLine("Exporter")
        Console.WriteLine("A utility to automate the download of exports from a Blackbaud CRM instance")
        Console.WriteLine("")
        Console.WriteLine("Usage:")
        Console.WriteLine("exporter.exe /url=<url> /db=<db name> /outputfolder=<folder location> /statfile=""<stat file name>"" /sqlinstancename=<sql server>\<sql instance> /sqldbname=<sql database name>")
        Console.WriteLine("")
        Console.WriteLine("Example:")
        Console.WriteLine("exporter.exe /url=""http://localhost/bbappfx_400/appfxwebservice.asmx"" /db=BBInfinity /outputfolder=""c:\output\exports"" /statfile=""c:\output\stats.csv"" /sqlinstancename=localhost\SQL2014 /sqldbname=MyDatabase")
        Console.WriteLine("")
        Console.WriteLine("Parameter details:")
        Console.WriteLine("/url              The appfxwebservice.asmx for the Blackbaud instance you wish")
        Console.WriteLine("                  to export from.")
        Console.WriteLine("/db               The friendly name of the Blackbaud database (often BBInfinity)")
        Console.WriteLine("/outputfolder     The folder into which all export output will be placed")
        Console.WriteLine("                  (output is CSV)")
        Console.WriteLine("/statfile         The file that will contain statistics about all completed")
        Console.WriteLine("                  exports (output is CSV)")
        Console.WriteLine("/sqlinstancename  The SQL instance behind Blackbaud CRM")
        Console.WriteLine("/sqldbname        The SQL database name behind Blackbaud CRM")
        Console.WriteLine("/timeout          Seconds to wait for an export to complete before moving on")
        Console.WriteLine("/debug            When specified, produces much more verbose output,")
        Console.WriteLine("                  including error messages and ongoing status updates")
        Console.WriteLine("/exportname       The name of the CRM export to retrieve.")
        Console.WriteLine("                  If not specified, then ALL exports will be downloaded.")
        Console.WriteLine("/ignoredcolumns   A comma-delimited list of columns to exclude from the output.")
        Console.WriteLine("                  Column indexes are zero-based.")
        Console.WriteLine("                  If not specified, then all columns will be included.")
        Console.WriteLine("                  Example: /ignoredcolumns=""1,2,3""")

    End Sub

    Private Function ArgsValid(ByRef msg As String) As Boolean
        Dim result As Boolean = True
        msg = String.Empty

        _params = UIToParams()
        MessageForEmptyRequiredParameter("URL", _params.AppfxWebServiceURL, msg)
        MessageForEmptyRequiredParameter("DB", _params.DBName, msg)
        MessageForEmptyRequiredParameter("OUTPUTFOLDER", _params.DBName, msg)
        MessageForEmptyRequiredParameter("STATFILE", _params.StatsOutputFileName, msg)
        MessageForEmptyRequiredParameter("SQLINSTANCENAME", _params.SQLInstanceName, msg)
        MessageForEmptyRequiredParameter("SQLDBNAME", _params.SQLDBName, msg)

        Return String.IsNullOrEmpty(msg)
    End Function

    Private Sub MessageForEmptyRequiredParameter(fieldName As String, enteredValue As String, ByRef msg As String)
        If String.IsNullOrEmpty(enteredValue) Then
            msg += IIf(String.IsNullOrEmpty(msg), String.Empty, ", ") + fieldName + " is required."
        End If
    End Sub

    Sub Main()
        Dim errorMessage As String = String.Empty
        If Not ArgsValid(errorMessage) Then
            Console.WriteLine(errorMessage)
            ShowHelp()
        Else
            Try
                If _params Is Nothing Then
                    _params = UIToParams()
                End If

                If _params.Debug Then
                    ParamsToUI(_params)
                End If
                Dim exports As List(Of CRMExport) = GetExportsFromCRM(_params.ExportName)
                If exports Is Nothing OrElse exports.Count = 0 Then
                    Console.WriteLine("No export was found with a name of '" + _params.ExportName + "'.  No action has been taken.")
                End If
                Dim wroteHeader As Boolean = False
                For Each export In exports
                    If Not wroteHeader Then
                        AddToFile(_params.StatsOutputFileName, export.Status.CSVHeader)
                        wroteHeader = True
                    End If
                    Console.WriteLine("Working on export '" + export.Name + "'")
                    export.Status.Name = export.Name
                    export.DropToFolder()
                    export.RefreshExportStatus()
                    AddToFile(_params.StatsOutputFileName, export.Status.CSVEntry)
                    Util.Log("Complete with " + export.Status.CRMStatus.TOTALNUMBERPROCESSED.ToString() + " records processed", False, _params)
                    Util.Log("Output file: '" + export.OutputFileName + "'", True, _params)
                Next
            Catch ex As Exception
                Console.WriteLine("Failed with error:")
                Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-")
                Console.WriteLine(ex.Message)
                Console.WriteLine("=-=-=-=-=-=-=-=-=-=-=-=-")
            End Try
        End If
        Console.WriteLine("Press ENTER to continue...")
        Console.Read()
    End Sub

    Private Function GetExportsFromCRM(Optional ExportName As String = "") As List(Of CRMExport)
        Dim result As New List(Of CRMExport)
        Dim filter As New Blackbaud.AppFx.Platform.Catalog.WebApiClient.DataLists.TopLevel.ExportProcessListFilterData()
        For Each export In Blackbaud.AppFx.Platform.Catalog.WebApiClient.DataLists.TopLevel.ExportProcessList.GetRows(Util.GetProvider(_params.AppfxWebServiceURL, _params.DBName, _params.CRMUserName, _params.CRMPassword), filter)
            If String.IsNullOrEmpty(ExportName) OrElse export.Name = ExportName Then
                Dim ex As New CRMExport(_params)
                ex.ExportID = export.ID
                ex.Name = export.Name
                result.Add(ex)
            End If
        Next
        Return result
    End Function

    Private Sub AddToFile(filename As String, text As String)
        Dim strFile As String = filename
        File.AppendAllText(strFile, text)
    End Sub
End Module