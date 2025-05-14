Imports System.Windows.Media

Namespace Models
    ''' <summary>
    ''' Represents a tracking item detected by the scanner
    ''' </summary>
    Public Class TrackerItem
        ''' <summary>
        ''' Name of the tracker
        ''' </summary>
        Public Property Name As String
        
        ''' <summary>
        ''' Type of tracker
        ''' </summary>
        Public Property Type As TrackerType
        
        ''' <summary>
        ''' Risk level of the tracker
        ''' </summary>
        Public Property RiskLevel As RiskLevel
        
        ''' <summary>
        ''' Location where the tracker was found
        ''' </summary>
        Public Property Location As String
        
        ''' <summary>
        ''' Gets the brush color based on risk level
        ''' </summary>
        Public ReadOnly Property RiskBrush As Brush
            Get
                Select Case RiskLevel
                    Case RiskLevel.Low
                        Return Brushes.Green
                    Case RiskLevel.Medium
                        Return Brushes.Orange
                    Case RiskLevel.High
                        Return Brushes.Red
                    Case Else
                        Return Brushes.Gray
                End Select
            End Get
        End Property
    End Class

    ''' <summary>
    ''' Types of trackers that can be detected
    ''' </summary>
    Public Enum TrackerType
        Advertising
        Analytics
        SocialMedia
        Fingerprinting
        Cookie
        Other
    End Enum

    ''' <summary>
    ''' Risk levels for trackers
    ''' </summary>
    Public Enum RiskLevel
        Low
        Medium
        High
    End Enum
End Namespace