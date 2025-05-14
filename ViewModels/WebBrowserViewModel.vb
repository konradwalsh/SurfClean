Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Runtime.CompilerServices
Imports System.Windows.Input
Imports System.Windows
Imports Surf_Clean.Models
Imports Surf_Clean.ViewModels  ' For WebBrowserViewModel



Namespace ViewModels
    Public Class WebBrowserViewModel
        Implements INotifyPropertyChanged
        
        Private _currentUrl As String = "https://duckduckgo.com"
        Private _searchText As String = String.Empty
        Private _isLoading As Boolean = False
        Private _trackingProtectionEnabled As Boolean = True
        Private _cookieBlockingEnabled As Boolean = True
        Private _fingerprintProtectionEnabled As Boolean = True
        Private _trackersBlocked As Integer = 0
        Private _blockedTrackers As New ObservableCollection(Of String)()
        Private _canGoBack As Boolean = False
        Private _canGoForward As Boolean = False
        
        Public Sub New()
            NavigateCommand = New RelayCommand(AddressOf Navigate)
            GoBackCommand = New RelayCommand(AddressOf GoBack, Function(param) CanGoBack)
            GoForwardCommand = New RelayCommand(AddressOf GoForward, Function(param) CanGoForward)
            RefreshCommand = New RelayCommand(AddressOf Refresh)
            StopCommand = New RelayCommand(AddressOf [Stop], Function(param) IsLoading)
            HomeCommand = New RelayCommand(AddressOf NavigateHome)
            ToggleTrackingProtectionCommand = New RelayCommand(AddressOf ToggleTrackingProtection)
            ToggleCookieBlockingCommand = New RelayCommand(AddressOf ToggleCookieBlocking)
            ToggleFingerprintProtectionCommand = New RelayCommand(AddressOf ToggleFingerprintProtection)
        End Sub
        
        ' Event to request operations on the WebView2 control
        Public Event WebViewRequest As EventHandler(Of WebViewRequestEventArgs)
        
        Public Property CurrentUrl As String
            Get
                Return _currentUrl
            End Get
            Set(value As String)
                _currentUrl = value
                OnPropertyChanged()
                SearchText = value
            End Set
        End Property
        
        Public Property SearchText As String
            Get
                Return _searchText
            End Get
            Set(value As String)
                _searchText = value
                OnPropertyChanged()
            End Set
        End Property
        
        Public Property IsLoading As Boolean
            Get
                Return _isLoading
            End Get
            Set(value As Boolean)
                _isLoading = value
                OnPropertyChanged()
                DirectCast(StopCommand, RelayCommand).RaiseCanExecuteChanged()
            End Set
        End Property
        
        Public Property TrackingProtectionEnabled As Boolean
            Get
                Return _trackingProtectionEnabled
            End Get
            Set(value As Boolean)
                _trackingProtectionEnabled = value
                OnPropertyChanged()
            End Set
        End Property
        
        Public Property CookieBlockingEnabled As Boolean
            Get
                Return _cookieBlockingEnabled
            End Get
            Set(value As Boolean)
                _cookieBlockingEnabled = value
                OnPropertyChanged()
            End Set
        End Property
        
        Public Property FingerprintProtectionEnabled As Boolean
            Get
                Return _fingerprintProtectionEnabled
            End Get
            Set(value As Boolean)
                _fingerprintProtectionEnabled = value
                OnPropertyChanged()
            End Set
        End Property
        
        Public Property TrackersBlocked As Integer
            Get
                Return _trackersBlocked
            End Get
            Set(value As Integer)
                _trackersBlocked = value
                OnPropertyChanged()
            End Set
        End Property
        
        Public Property BlockedTrackers As ObservableCollection(Of String)
            Get
                Return _blockedTrackers
            End Get
            Set(value As ObservableCollection(Of String))
                _blockedTrackers = value
                OnPropertyChanged()
            End Set
        End Property
        
        Public Property CanGoBack As Boolean
            Get
                Return _canGoBack
            End Get
            Set(value As Boolean)
                _canGoBack = value
                OnPropertyChanged()
                DirectCast(GoBackCommand, RelayCommand).RaiseCanExecuteChanged()
            End Set
        End Property
        
        Public Property CanGoForward As Boolean
            Get
                Return _canGoForward
            End Get
            Set(value As Boolean)
                _canGoForward = value
                OnPropertyChanged()
                DirectCast(GoForwardCommand, RelayCommand).RaiseCanExecuteChanged()
            End Set
        End Property
        
        Public ReadOnly Property NavigateCommand As ICommand
        Public ReadOnly Property GoBackCommand As ICommand
        Public ReadOnly Property GoForwardCommand As ICommand
        Public ReadOnly Property RefreshCommand As ICommand
        Public ReadOnly Property StopCommand As ICommand
        Public ReadOnly Property HomeCommand As ICommand
        Public ReadOnly Property ToggleTrackingProtectionCommand As ICommand
        Public ReadOnly Property ToggleCookieBlockingCommand As ICommand
        Public ReadOnly Property ToggleFingerprintProtectionCommand As ICommand
        
        Public Sub UpdateNavigationState(canGoBack As Boolean, canGoForward As Boolean)
            Me.CanGoBack = canGoBack
            Me.CanGoForward = canGoForward
        End Sub
        
        Public Sub IncrementBlockedTrackers(trackerDomain As String)
            Application.Current.Dispatcher.Invoke(Sub()
                TrackersBlocked += 1
                If Not BlockedTrackers.Contains(trackerDomain) Then
                    BlockedTrackers.Add(trackerDomain)
                End If
            End Sub)
        End Sub
        
        Private Sub Navigate(param As Object)
            If String.IsNullOrWhiteSpace(SearchText) Then
                Return
            End If
            
            ' Determine if the input is a URL or a search query
            Dim url As String = SearchText
            If Not url.StartsWith("http://") AndAlso Not url.StartsWith("https://") Then
                ' Check if it's a valid domain format
                If Not url.Contains(".") OrElse url.Contains(" ") Then
                    ' Treat as a search query
                    url = $"https://duckduckgo.com/?q={Uri.EscapeDataString(url)}"
                Else
                    ' Likely a URL without protocol
                    url = "https://" & url
                End If
            End If
            
            RaiseEvent WebViewRequest(Me, New WebViewRequestEventArgs With {
                .RequestType = WebViewRequestType.Navigate,
                .Url = url
            })
        End Sub
        
        Private Sub GoBack(param As Object)
            RaiseEvent WebViewRequest(Me, New WebViewRequestEventArgs With {
                .RequestType = WebViewRequestType.GoBack
            })
        End Sub
        
        Private Sub GoForward(param As Object)
            RaiseEvent WebViewRequest(Me, New WebViewRequestEventArgs With {
                .RequestType = WebViewRequestType.GoForward
            })
        End Sub
        
        Private Sub Refresh(param As Object)
            RaiseEvent WebViewRequest(Me, New WebViewRequestEventArgs With {
                .RequestType = WebViewRequestType.Refresh
            })
        End Sub
        
        Private Sub [Stop](param As Object)
            RaiseEvent WebViewRequest(Me, New WebViewRequestEventArgs With {
                .RequestType = WebViewRequestType.Stop
            })
        End Sub
        
        Private Sub NavigateHome(param As Object)
            RaiseEvent WebViewRequest(Me, New WebViewRequestEventArgs With {
                .RequestType = WebViewRequestType.Navigate,
                .Url = "https://duckduckgo.com"
            })
        End Sub
        
        Private Sub ToggleTrackingProtection(param As Object)
            TrackingProtectionEnabled = Not TrackingProtectionEnabled
            ' This would apply filters on the WebView2 content
        End Sub
        
        Private Sub ToggleCookieBlocking(param As Object)
            CookieBlockingEnabled = Not CookieBlockingEnabled
            ' This would update cookie settings on WebView2
        End Sub
        
        Private Sub ToggleFingerprintProtection(param As Object)
            FingerprintProtectionEnabled = Not FingerprintProtectionEnabled
            ' This would update fingerprint protection settings on WebView2
        End Sub
        
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        
        Protected Overridable Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
    End Class

    Public Enum WebViewRequestType
        Navigate
        GoBack
        GoForward
        Refresh
        [Stop]
    End Enum

    Public Class WebViewRequestEventArgs
        Inherits EventArgs
        
        Public Property RequestType As WebViewRequestType
        Public Property Url As String
    End Class
End Namespace