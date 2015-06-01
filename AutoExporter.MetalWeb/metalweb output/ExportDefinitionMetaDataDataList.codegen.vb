Option Infer On
Option Strict Off

Imports bbAppFxWebAPI = Blackbaud.AppFx.WebAPI



Namespace DataLists

    Namespace [TopLevel]

		

		    ''' <summary>
    ''' Provides WebApi access to the "ExportDefinitionMetaData Data List" catalog feature.  Given an export definition id, returns metadata about that export definition
    ''' </summary>
<System.CodeDom.Compiler.GeneratedCodeAttribute("BBMetalWeb", "2011.8.2.0")> _
        Public NotInheritable Class [ExportDefinitionMetaDataDataList]

            Private Sub New()
                'this is a static class (only shared methods) that should never be instantiated.
            End Sub

            Private Shared ReadOnly _specId As Guid = New Guid("4eff22cb-712b-475c-ad89-746c2d84e75f")
            ''' <summary>
            ''' The DataList ID value for the "ExportDefinitionMetaData Data List" datalist
            ''' </summary>
            Public Shared ReadOnly Property SpecId() As Guid
                Get
                    Return _specId
                End Get
            End Property

			Private Shared ReadOnly _rowFactoryDelegate As Blackbaud.AppFx.WebAPI.DataListRowFactoryDelegate(Of [ExportDefinitionMetaDataDataListRow]) = AddressOf CreateListRow

            Private Shared Function CreateListRow(ByVal rowValues As String()) As [ExportDefinitionMetaDataDataListRow]
                Return New [ExportDefinitionMetaDataDataListRow](rowValues)
            End Function

            Public Shared Function CreateRequest(ByVal provider As bbAppFxWebAPI.AppFxWebServiceProvider) As bbAppFxWebAPI.ServiceProxy.DataListLoadRequest
                Return Blackbaud.AppFx.WebAPI.DataListServices.CreateDataListLoadRequest(provider, [ExportDefinitionMetaDataDataList].SpecId)
            End Function
            
            Public Shared Function GetRows(ByVal provider As bbAppFxWebAPI.AppFxWebServiceProvider  , ByVal filter As ExportDefinitionMetaDataDataListFilterData) As ExportDefinitionMetaDataDataListRow()

				

                Dim request = CreateRequest(provider)

				
				
				                If filter IsNot Nothing Then request.Parameters = filter.BuildDataFormItemForFilter() 
				
                Return GetRows(provider, request)

            End Function

            Public Shared Function GetRows(ByVal provider As bbAppFxWebAPI.AppFxWebServiceProvider, ByVal request As bbAppFxWebAPI.ServiceProxy.DataListLoadRequest) As ExportDefinitionMetaDataDataListRow()
                Return bbAppFxWebAPI.DataListServices.GetListRows(Of [ExportDefinitionMetaDataDataListRow])(provider, _rowFactoryDelegate, request)
            End Function

        End Class

#Region "Row Data Class"

		<System.CodeDom.Compiler.GeneratedCodeAttribute("BBMetalWeb", "2011.8.2.0")> _
		<System.Serializable> _
        Public NotInheritable Class [ExportDefinitionMetaDataDataListRow]

            
Private [_VIEWPATH] As String
Public Property [VIEWPATH] As String
    Get
        Return Me.[_VIEWPATH]
    End Get
    Set
        Me.[_VIEWPATH] = value 
    End Set
End Property

Private [_DISPLAYPATH] As String
Public Property [DISPLAYPATH] As String
    Get
        Return Me.[_DISPLAYPATH]
    End Get
    Set
        Me.[_DISPLAYPATH] = value 
    End Set
End Property

Private [_QUERYVIEWID] As System.Guid
Public Property [QUERYVIEWID] As System.Guid
    Get
        Return Me.[_QUERYVIEWID]
    End Get
    Set
        Me.[_QUERYVIEWID] = value 
    End Set
End Property

Private [_CARDINALITY] As String
Public Property [CARDINALITY] As String
    Get
        Return Me.[_CARDINALITY]
    End Get
    Set
        Me.[_CARDINALITY] = value 
    End Set
End Property

Private [_NUMBERTOEXPORT] As Integer
Public Property [NUMBERTOEXPORT] As Integer
    Get
        Return Me.[_NUMBERTOEXPORT]
    End Get
    Set
        Me.[_NUMBERTOEXPORT] = value 
    End Set
End Property




			Public Sub New()
				Mybase.New()
			End Sub

            Friend Sub New(ByVal dataListRowValues() As String)

                Blackbaud.AppFx.WebAPI.DataListServices.ValidateDataListOutputColumnCount(4, dataListRowValues, ExportDefinitionMetaDataDataList.SpecId)

Me.[_VIEWPATH] = dataListRowValues(0)

Me.[_DISPLAYPATH] = dataListRowValues(1)

Me.[_QUERYVIEWID] = Blackbaud.AppFx.DataListUtility.DataListStringValueToGuid(dataListRowValues(2))

Me.[_CARDINALITY] = dataListRowValues(3)

Me.[_NUMBERTOEXPORT] = Blackbaud.AppFx.DataListUtility.DataListStringValueToInt(dataListRowValues(4))



            End Sub

        End Class
        
#End Region

		<System.CodeDom.Compiler.GeneratedCodeAttribute("BBMetalWeb", "2011.8.2.0")> _
		<System.Serializable> _
		Public NotInheritable Class ExportDefinitionMetaDataDataListFilterData
		    
			Private [_EXPORTDEFINITIONID] As Nullable(of System.Guid)
''' <summary>
''' Export definition ID
''' </summary>
Public Property [EXPORTDEFINITIONID] As Nullable(of System.Guid)
    Get
        Return Me.[_EXPORTDEFINITIONID]
    End Get
    Set
        Me.[_EXPORTDEFINITIONID] = value 
    End Set
End Property



			Friend Function BuildDataFormItemForFilter() As Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
		        
				Dim dfi As New Blackbaud.AppFx.XmlTypes.DataForms.DataFormItem
		    
				Dim value As Object = Nothing
value = Me.[EXPORTDEFINITIONID]

	dfi.SetValueIfNotNull("EXPORTDEFINITIONID",value)



				if dfi.Values Is Nothing orelse dfi.Values.Count=0 then
					return nothing
				else
					Return dfi
				 end if
		        
			End Function
		    
		End Class
  
    End Namespace

End Namespace


