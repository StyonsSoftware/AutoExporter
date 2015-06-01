Public Class Parameters
    Public AppfxWebServiceURL As String 'something like 'http://localhost/bbappfx_293/appfxwebservice.asmx'
    Public DBName As String 'Friendly name of DB, something like BBInfinity, pulled from the web.config of CRM
    Public OutputFolder As String 'folder that will contain output
    Public SimultaneousCount As Integer 'not implemented, idea is to specify a max # that can run simultaneously
    Public StatsOutputFileName As String 'file that will contain statistics
    Public Debug As Boolean 'specify true to get extra debug output
    Public SQLInstanceName As String
    Public SQLDBName As String
    Public CRMUserName As String
    Public CRMPassword As String
    Public TimeoutSeconds As Integer 'wait for this many seconds before giving up on an export.
End Class
