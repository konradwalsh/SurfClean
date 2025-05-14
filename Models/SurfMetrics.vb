Imports System
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Surf_Clean.Models
Imports Surf_Clean.ViewModels  

Namespace SurfClean.Models

''' <summary>
''' Represents metrics and statistics for the SurfClean application
''' </summary>
Public Class SurfMetrics
    Implements INotifyPropertyChanged
    
    ' Private backing fields for properties
    Private _totalTrackerCount As Integer = 0
    Private _totalSessionsCount As Integer = 0
    Private _cleanlinessScore As Integer = 0
    Private _dateLastScan As DateTime = DateTime.Now
    Private _timeSpentBrowsing As TimeSpan = TimeSpan.Zero
    
    ''' <summary>
    ''' Total number of trackers blocked since installation
    ''' </summary>
    Public Property TotalTrackerCount As Integer
        Get
            Return _totalTrackerCount
        End Get
        Set(value As Integer)
            _totalTrackerCount = value
            OnPropertyChanged()
            CalculateCleanlinessScore()
        End Set
    End Property
    
    ''' <summary>
    ''' Total number of browsing sessions
    ''' </summary>
    Public Property TotalSessionsCount As Integer
        Get
            Return _totalSessionsCount
        End Get
        Set(value As Integer)
            _totalSessionsCount = value
            OnPropertyChanged()
            CalculateCleanlinessScore()
        End Set
    End Property
    
    ''' <summary>
    ''' Overall cleanliness score (0-100)
    ''' </summary>
    Public Property CleanlinessScore As Integer
        Get
            Return _cleanlinessScore
        End Get
        Private Set(value As Integer)
            _cleanlinessScore = value
            OnPropertyChanged()
        End Set
    End Property
    
    ''' <summary>
    ''' Date and time of the last system scan
    ''' </summary>
    Public Property DateLastScan As DateTime
        Get
            Return _dateLastScan
        End Get
        Set(value As DateTime)
            _dateLastScan = value
            OnPropertyChanged()
        End Set
    End Property
    
    ''' <summary>
    ''' Total time spent browsing with SurfClean
    ''' </summary>
    Public Property TimeSpentBrowsing As TimeSpan
        Get
            Return _timeSpentBrowsing
        End Get
        Set(value As TimeSpan)
            _timeSpentBrowsing = value
            OnPropertyChanged()
        End Set
    End Property
    
    ''' <summary>
    ''' Average trackers blocked per session
    ''' </summary>
    Public ReadOnly Property AverageTrackersPerSession As Double
        Get
            If TotalSessionsCount = 0 Then
                Return 0
            End If
            Return CDbl(TotalTrackerCount) / CDbl(TotalSessionsCount)
        End Get
    End Property
    
    ''' <summary>
    ''' Days since last scan was performed
    ''' </summary>
    Public ReadOnly Property DaysSinceLastScan As Integer
        Get
            Return CInt((DateTime.Now - DateLastScan).TotalDays)
        End Get
    End Property
    
    ''' <summary>
    ''' Human-readable description of the surf conditions
    ''' </summary>
    Public ReadOnly Property SurfConditionsDescription As String
        Get
            Return If(CleanlinessScore > 90, "Perfect Waves", 
                   If(CleanlinessScore > 75, "Excellent", 
                   If(CleanlinessScore > 60, "Good", 
                   If(CleanlinessScore > 40, "Choppy", "Stormy"))))
        End Get
    End Property
    
    ''' <summary>
    ''' Calculates the cleanliness score based on metrics
    ''' </summary>
    Private Sub CalculateCleanlinessScore()
        ' This is a simplified algorithm
        ' A real implementation would consider many factors
        Dim baseScore As Integer = 70
        
        ' More trackers blocked = better score
        If TotalTrackerCount > 1000 Then
            baseScore += 15
        ElseIf TotalTrackerCount > 500 Then
            baseScore += 10
        ElseIf TotalTrackerCount > 100 Then
            baseScore += 5
        End If
        
        ' Regular use = better score
        If TotalSessionsCount > 50 Then
            baseScore += 10
        ElseIf TotalSessionsCount > 20 Then
            baseScore += 5
        End If
        
        ' Recent scan = better score
        If DaysSinceLastScan < 1 Then
            baseScore += 5
        ElseIf DaysSinceLastScan > 7 Then
            baseScore -= 5
        End If
        
        ' Ensure score is within 0-100 range
        CleanlinessScore = Math.Max(0, Math.Min(100, baseScore))
    End Sub
    
    ''' <summary>
    ''' Increments the tracker count by one or a specified amount
    ''' </summary>
    Public Sub IncrementTrackerCount(Optional amount As Integer = 1)
        TotalTrackerCount += amount
    End Sub
    
    ''' <summary>
    ''' Increments the session count
    ''' </summary>
    Public Sub IncrementSessionCount()
        TotalSessionsCount += 1
    End Sub
    
    ''' <summary>
    ''' Updates the last scan date to current time
    ''' </summary>
    Public Sub UpdateLastScanDate()
        DateLastScan = DateTime.Now
    End Sub
    
    ''' <summary>
    ''' Adds time to the total browsing time
    ''' </summary>
    Public Sub AddBrowsingTime(duration As TimeSpan)
        TimeSpentBrowsing = TimeSpentBrowsing.Add(duration)
    End Sub
    
    ' INotifyPropertyChanged implementation
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    
    Protected Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class
End Namespace