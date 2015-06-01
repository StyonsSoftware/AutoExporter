Imports System.Data.SqlClient

Public Class Util
    Private Shared _provider As Blackbaud.AppFx.WebAPI.AppFxWebServiceProvider
    Public Shared Function GetProvider(AppfxWebServiceURL As String, DBName As String, CRMUsername As String, CRMPassword As String) As Blackbaud.AppFx.WebAPI.AppFxWebServiceProvider
        'endpoints expect an AppFxWebServiceProvider.  This function builds it once, then caches it for future use.
        If _provider Is Nothing Then
            _provider = New Blackbaud.AppFx.WebAPI.AppFxWebServiceProvider(AppfxWebServiceURL, DBName)
        End If

        'use custom credentials if they have been provided, otherwise assume current user
        If (CRMUsername IsNot Nothing AndAlso 0 < CRMUsername.Trim.Length) OrElse (CRMPassword IsNot Nothing AndAlso 0 < CRMPassword.Trim.Length) Then
            _provider.Credentials = New System.Net.NetworkCredential(CRMUsername, CRMPassword)
        End If
        Return _provider
    End Function

    Public Shared Function GetSQLConnection(instanceName As String, sqlDBName As String) As SqlConnection
        'build a sql connection based on the values entered by the user on the form
        Dim connStrBuilder As New SqlConnectionStringBuilder()
        connStrBuilder.DataSource = instanceName
        connStrBuilder.InitialCatalog = sqlDBName
        connStrBuilder.IntegratedSecurity = True
        Dim result As New SqlConnection(connStrBuilder.ConnectionString)
        Return result
    End Function

    Public Shared Sub Log(msg As String, debugOnly As Boolean, parms As Parameters)
        If Not debugOnly OrElse parms.Debug Then
            Console.WriteLine(msg)
        End If
    End Sub
End Class