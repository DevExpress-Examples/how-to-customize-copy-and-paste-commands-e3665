﻿Imports Microsoft.VisualBasic
Imports System
Imports System.Text
Imports DevExpress.XtraRichEdit.Services
Imports DevExpress.XtraRichEdit.Services.Implementation
Imports DevExpress.XtraRichEdit.Utils

Namespace RichEditCustomCopyPaste
	Public Class CF_HTMLHelper
		'HTML Clipboard Format http://msdn.microsoft.com/en-us/library/aa767917(v=vs.85).aspx
		Private Const StartFragmentTag As String = "<!--StartFragment-->"
		Private Const EndFragmentTag As String = "<!--EndFragment-->"

		Private Const bodyTag As String = "<body>" & Constants.vbCrLf
		Private Const bodyTagClose As String = "</body>"
		Private Const EmptyDescription As String = "Version:0.9" & Constants.vbCrLf & "StartHTML:{0:D10}" & Constants.vbCrLf & "EndHTML:{1:D10}" & Constants.vbCrLf & "StartFragment:{2:D10}" & Constants.vbCrLf & "EndFragment:{3:D10}" & Constants.vbCrLf

		Public Shared Function GetHtmlClipboardFormat(ByVal html As String) As String
			Dim startBodyTagPos As Integer = html.IndexOf(bodyTag)
			Dim bodyEndTagPos As Integer = html.LastIndexOf(bodyTagClose)

			Dim contentBeforeFramentLength As Integer = startBodyTagPos + bodyTag.Length
			Dim contentBeforeFragment As String = html.Substring(0, contentBeforeFramentLength)

			Dim fragment As String = html.Substring(contentBeforeFramentLength, bodyEndTagPos - contentBeforeFramentLength)

			Dim contentAfterFragment As String = html.Substring(bodyEndTagPos, html.Length - bodyEndTagPos)

			Dim result As String = Get_CF_HTML(contentBeforeFragment & StartFragmentTag, fragment, EndFragmentTag & contentAfterFragment)

			Return result
		End Function

		Private Shared Function Get_CF_HTML(ByVal contentBeforeFragment As String, ByVal fragment As String, ByVal contentAfterFragment As String) As String
			Dim contentBeforeFragmentCount As Integer = Encoding.UTF8.GetByteCount(contentBeforeFragment)
			Dim fragmentCount As Integer = Encoding.UTF8.GetByteCount(fragment)
			Dim contentAfterFragmentCount As Integer = Encoding.UTF8.GetByteCount(contentAfterFragment)

			Dim descriptionOffset As Integer = Encoding.UTF8.GetByteCount(String.Format(EmptyDescription, 0, 0, 0, 0))
			Dim endHTMLOffset As Integer = descriptionOffset + contentBeforeFragmentCount + fragmentCount + contentAfterFragmentCount
			Dim startFragmentOffset As Integer = descriptionOffset + contentBeforeFragmentCount
			Dim endFragmentOffset As Integer = descriptionOffset + contentBeforeFragmentCount + fragmentCount

			Dim description As String = String.Format(EmptyDescription, descriptionOffset, endHTMLOffset, startFragmentOffset, endFragmentOffset)

			Dim content As New StringBuilder()
			content.Append(description)
			content.Append(contentBeforeFragment)
			content.Append(fragment)
			content.Append(contentAfterFragment)
			Return content.ToString()
		End Function
	End Class

	Public Class CustomUriProvider
		Implements IUriProvider
        Public Function CreateCssUri(ByVal rootUri As String, ByVal styleText As String, ByVal relativeUri As String) As String Implements IUriProvider.CreateCssUri
            Return String.Empty
        End Function

        Public Function CreateImageUri(ByVal rootUri As String, ByVal image As RichEditImage, ByVal relativeUri As String) As String Implements IUriProvider.CreateImageUri
            If String.IsNullOrEmpty(image.Uri) Then
                Return New DataStringUriProvider().CreateImageUri(rootUri, image, relativeUri)
            Else
                Return image.Uri
            End If
        End Function
	End Class
End Namespace
