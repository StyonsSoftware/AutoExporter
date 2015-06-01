'corresponds roughly to the BUSINESSPROCESSSTATUS table
Public Class ExportStatus
    Public CRMStatus As Blackbaud.AppFx.Platform.Catalog.WebApiClient.ViewForms.BusinessProcessParameterSet.BusinessProcessParameterSetRecentStatusViewFormData
    Public OutputTableName As String
    Public Name As String

    Public ReadOnly Property CSVHeader As String
        Get
            Return "Name,Duration,Success Count,Error Count,Error Message" + Environment.NewLine
        End Get
    End Property
    Public ReadOnly Property CSVEntry As String
        Get
            Return Name + "," + CRMStatus.DURATION + "," + CRMStatus.SUCCESSCOUNT.ToString + "," + CRMStatus.EXCEPTIONCOUNT.ToString + "," + CRMStatus.MESSAGE + Environment.NewLine
        End Get
    End Property

    Public Sub New(friendlyName As String, crmStatusData As Blackbaud.AppFx.Platform.Catalog.WebApiClient.ViewForms.BusinessProcessParameterSet.BusinessProcessParameterSetRecentStatusViewFormData)
        CRMStatus = crmStatusData
        Name = friendlyName
    End Sub
End Class
