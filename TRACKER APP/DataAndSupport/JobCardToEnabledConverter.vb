Imports System.Globalization
Imports System.Windows.Data

Namespace Converters
    Public Class JobCardToEnabledConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            Dim jobCardValue = TryCast(value, String)
            If jobCardValue IsNot Nothing AndAlso jobCardValue.Trim().ToUpper() = "Y" Then
                Return Visibility.Collapsed ' Hide the checkbox
            End If
            Return Visibility.Visible ' Enable checkbox
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function

    End Class
End Namespace

