Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Runtime.CompilerServices
Imports System.Windows.Input
Imports Surf_Clean.Models
Imports Surf_Clean.SurfClean.ViewModels


Namespace ViewModels
    Public Class MainViewModel
        Implements INotifyPropertyChanged
        
        Private _statusMessage As String = "SurfClean - Where the web washes clean"
        Private _trackerCount As Integer = 0
        Private _detectedTrackers As New ObservableCollection(Of TrackerItem)()
        Private _isScanActive As Boolean = False
        
        Public Sub New()
            StartScanCommand = New RelayCommand(AddressOf StartScan, Function(param) Not IsScanActive)
            StopScanCommand = New RelayCommand(AddressOf StopScan, Function(param) IsScanActive)
            ClearResultsCommand = New RelayCommand(AddressOf ClearResults, Function(param) DetectedTrackers.Count > 0)
        End Sub
        
        Public Property StatusMessage As String
            Get
                Return _statusMessage
            End Get
            Set(value As String)
                _statusMessage = value
                OnPropertyChanged()
            End Set
        End Property
        
        Public Property TrackerCount As Integer
            Get
                Return _trackerCount
            End Get
            Set(value As Integer)
                _trackerCount = value
                OnPropertyChanged()
            End Set
        End Property
        
        Public Property DetectedTrackers As ObservableCollection(Of Models.TrackerItem)
            Get
                Return _detectedTrackers
            End Get
            Set(value As ObservableCollection(Of Models.TrackerItem))
                _detectedTrackers = value
                OnPropertyChanged()
            End Set
        End Property
        
        Public Property IsScanActive As Boolean
            Get
                Return _isScanActive
            End Get
            Set(value As Boolean)
                _isScanActive = value
                OnPropertyChanged()
                DirectCast(StartScanCommand, RelayCommand).RaiseCanExecuteChanged()
                DirectCast(StopScanCommand, RelayCommand).RaiseCanExecuteChanged()
            End Set
        End Property
        
        Public ReadOnly Property StartScanCommand As ICommand
        Public ReadOnly Property StopScanCommand As ICommand
        Public ReadOnly Property ClearResultsCommand As ICommand
        
        Private Sub StartScan(param As Object)
            IsScanActive = True
            StatusMessage = "Washing your system for tracking elements..."
            
            ' Simulate scanning process with sample data
            DetectedTrackers.Add(New Models.TrackerItem() With {
                .Name = "Facebook Pixel",
                .Type = TrackerType.SocialMedia,
                .RiskLevel = RiskLevel.High,
                .Location = "Shallow Waters (Browser Cookies)"
            })
            
            DetectedTrackers.Add(New TrackerItem() With {
                .Name = "Google Analytics",
                .Type = TrackerType.Analytics,
                .RiskLevel = RiskLevel.Medium,
                .Location = "Deep Sea (Local Storage)"
            })
            
            DetectedTrackers.Add(New TrackerItem() With {
                .Name = "DoubleClick",
                .Type = TrackerType.Advertising,
                .RiskLevel = RiskLevel.High,
                .Location = "Coral Reef (Browser Cache)"
            })
            
            TrackerCount = DetectedTrackers.Count
            StatusMessage = $"Wash complete. Found {TrackerCount} tracking elements ready to be cleaned."
            IsScanActive = False
        End Sub
        
        Private Sub StopScan(param As Object)
            IsScanActive = False
            StatusMessage = "Wash stopped by user. The ocean is still waiting to be cleaned."
        End Sub
        
        Private Sub ClearResults(param As Object)
            DetectedTrackers.Clear()
            TrackerCount = 0
            StatusMessage = "Results cleared. Ready to surf clean waters."
        End Sub
        
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        
        Protected Overridable Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
    End Class
End Namespace