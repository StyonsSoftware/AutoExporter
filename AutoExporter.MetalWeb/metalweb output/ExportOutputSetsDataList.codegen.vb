Option Infer On
Option Strict Off

Imports bbAppFxWebAPI = Blackbaud.AppFx.WebAPI



Namespace DataLists

    Namespace [TopLevel]

		

		    ''' <summary>
    ''' Provides WebApi access to the "Export Output Sets Data List" catalog feature.  Provides data needed to download the output of an Export Process
    ''' </summary>
<System.CodeDom.Compiler.GeneratedCodeAttribute("BBMetalWeb", "2011.8.2.0")> _
        Public NotInheritable Class [ExportOutputSetsDataList]

            Private Sub New()
                'this is a static class (only shared methods) that should never be instantiated.
            End Sub

            Private Shared ReadOnly _specId As Guid = New Guid("36b8d1c7-0767-4e6e-a318-9f33b42622e2")
            ''' <summary>
            ''' The DataList ID value for the "Export Output Sets Data List" datalist
            ''' </summary>
            Public Shared ReadOnly Property SpecId() As Guid
                Get
                    Return _specId
                End Get
            End Property

			Private Shared ReadOnly _rowFactoryDelegate As Blackbaud.AppFx.WebAPI.DataListRowFactoryDelegate(Of [ExportOutputSetsDataListRow]) = AddressOf CreateListRow

            Private Shared Function CreateListRow(ByVal rowValues As String()) As [ExportOutputSetsDataListRow]
                Return New [ExportOutputSetsDataListRow](rowValues)
            End Function

            Public Shared Function CreateRequest(ByVal provider As bbAppFxWebAPI.AppFxWebServiceProvider) As bbAppFxWebAPI.ServiceProxy.DataListLoadRequest
                Return Blackbaud.AppFx.WebAPI.DataListServices.CreateDataListLoadRequest(provider, [ExportOutputSetsDataList].SpecId)
            End Function
            
            Public Shared Function GetRows(ByVal provider As bbAppFxWebAPI.AppFxWebServiceProvider  , ByVal filter As ExportOutputSetsDataListFilterData) As ExportOutputSetsDataListRow()

				

                Dim request = CreateRequest(provider)

				
				
				                If filter IsNot Nothing Then request.Parameters = filter.BuildDataFormItemForFilter() 
				
                Return GetRows(provider, request)

            End Function

            Public Shared Function GetRows(ByVal provider As bbAppFxWebAPI.AppFxWebServiceProvider, ByVal request As bbAppFxWebAPI.ServiceProxy.DataListLoadRequest) As ExportOutputSetsDataListRow()
                Return bbAppFxWebAPI.DataListServices.GetListRows(Of [ExportOutputSetsDataListRow])(provider, _rowFactoryDelegate, request)
            End Function

        End Class

#Region "Row Data Class"

		<System.CodeDom.Compiler.GeneratedCodeAttribute("BBMetalWeb", "2011.8.2.0")> _
		<System.Serializable> _
        Public NotInheritable Class [ExportOutputSetsDataListRow]

            
Private [_ID] As System.Guid
Public Property [ID] As System.Guid
    Get
        Return Me.[_ID]
    End Get
    Set
        Me.[_ID] = value 
    End Set
End Property

Private [_NAME] As String
Public Property [NAME] As String
    Get
        Return Me.[_NAME]
    End Get
    Set
        Me.[_NAME] = value 
    End Set
End Property

Private [_STATUS] As String
Public Property [STATUS] As String
    Get
        Return Me.[_STATUS]
    End Get
    Set
        Me.[_STATUS] = value 
    End Set
End Property

Private [_TABLENAME] As String
Public Property [TABLENAME] As String
    Get
        Return Me.[_TABLENAME]
    End Get
    Set
        Me.[_TABLENAME] = value 
    End Set
End Property

Private [_BUSINESSPROCESSPARAMETERSETID] As System.Guid
Public Property [BUSINESSPROCESSPARAMETERSETID] As System.Guid
    Get
        Return Me.[_BUSINESSPROCESSPARAMETERSETID]
    End Get
    Set
        Me.[_BUSINESSPROCESSPARAMETERSETID] = value 
    End Set
End Property




			Public Sub New()
				Mybase.New()
			End Sub

            Friend Sub New(ByVal dataListRowValues() As String)

                Blackbaud.AppFx.WebAPI.DataListServices.ValidateDataListOutputColumnCount(4, dataListRowValues, ExportOutputSetsDataList.SpecId)

Me.[_ID] = Blackbaud.AppFx.DataListUtility.DataListStringValueToGuid(dataListRowValues(0))

Me.[_NAME] = dataListRowValues(1)

Me.[_STATUS] = dataListRowValues(2)

Me.[_TABLENAME] = dataListRowValues(3)

Me.[_BUSINESSPROCESSPARAMETERSETID] = Blackbaud.AppFx.DataListUtility.DataListStringValueToGuid(dataListRowValues(4))



            End Sub

        End Class
        
#End Region

		<System.CodeDom.Compiler.GeneratedCodeAttribute("BBMetalWeb", "2011.8.2.0")> _
		<System.Serializable> _
		Public NotInheritable Class ExportOutputSetsDataListFilterData
		    
			Private [_BUSINESSPROCESSPARAMETERSETID] As Nullable(of System.Guid)
''' <summary>
''' BUSINESSPROCESSPARAMETERSETID
''' </summary>
Public Property [BUSINESSPROCESSPARAMETERSETID] As Nullable(of System.Guid)
    Get
        Return Me.[_BUSINESSPROCESSPARAMETERSETID]
    End Get
    Set
        Me.[_BUSINESSPROCESSPARAMETERSETID] = value 
    End Set
End Property

Private [_MINIMUMDATE] As Nullable(of Date)
''' <summary>
''' MINIMUMDATE
''' </summary>
Public Property [MINIMUMDATE] As Nullable(of Date)
    Get
        Return Me.[_MINIMUMDATE]
    End Get
    Set
        Me.[_MINIMUMDATE] = value 
    End Set
End Property



			Friend Function BuildDataFormItemForFilter() As Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
		        
				Dim dfi As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
		    
				Dim value As Object = Nothing
value = Me.[BUSINESSPROCESSPARAMETERSETID]

	dfi.SetValueIfNotNull("BUSINESSPROCESSPARAMETERSETID",value)

value = Me.[MINIMUMDATE]

	dfi.SetValueIfNotNull("MINIMUMDATE",value)



				if dfi.Values Is Nothing orelse dfi.Values.Count=0 then
					return nothing
				else
					Return dfi
				 end if
		        
			End Function
		    
		End Class
  
    End Namespace

End Namespace


