Imports Microsoft.Web.WebView2.Core
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.IO
Imports Surf_Clean.ViewModels

Namespace Surf_Clean.Views
    ''' <summary>
    ''' User control that provides a secure web browsing experience with tracking protection
    ''' </summary>
    Partial Public Class WebBrowserView
        Inherits UserControl
        
        ''' <summary>
        ''' View model that handles browser navigation logic and tracking protection state
        ''' </summary>
        Private WithEvents _viewModel As New WebBrowserViewModel()
        
        ''' <summary>
        ''' List of known tracking domains that will be blocked when tracking protection is enabled
        ''' </summary>
        Private ReadOnly _knownTrackerDomains As New List(Of String) From {
            "googletagmanager.com",
            "facebook.net",
            "doubleclick.net",
            "google-analytics.com",
            "amazon-adsystem.com",
            "scorecardresearch.com",
            "criteo.com",
            "adnxs.com",
            "adsrvr.org",
            "pubmatic.com",
            "advertising.com",
            "media.net",
            "outbrain.com",
            "analytics.yahoo.com",
            "analytics.twitter.com"
        }
        
        ''' <summary>
        ''' Initializes a new instance of the WebBrowserView control
        ''' </summary>
        Public Sub New()
            ' This call is required by the designer.
            InitializeComponent()
            
            DataContext = _viewModel
            
            AddHandler Loaded, AddressOf WebBrowserView_Loaded
        End Sub
        
        ''' <summary>
        ''' Initializes the WebView2 control when the UserControl is loaded
        ''' </summary>
        Private Async Sub WebBrowserView_Loaded(sender As Object, e As RoutedEventArgs)
            ' Initialize WebView2
            Try
                Await webView.EnsureCoreWebView2Async()
                
                ' Setup event handlers
                AddHandler webView.NavigationStarting, AddressOf WebView_NavigationStarting
                AddHandler webView.NavigationCompleted, AddressOf WebView_NavigationCompleted
                AddHandler webView.CoreWebView2.WebResourceRequested, AddressOf CoreWebView2_WebResourceRequested
                AddHandler webView.CoreWebView2.NewWindowRequested, AddressOf CoreWebView2_NewWindowRequested

                ' Monitor all web resource requests to enable blocking of tracking resources
                webView.CoreWebView2.AddWebResourceRequestedFilter("*", 
                    CoreWebView2WebResourceContext.All)

                ' Configure privacy-enhancing browser settings
                webView.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36 SurfClean/1.0"
                webView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = False
                webView.CoreWebView2.Settings.IsScriptEnabled = True
                webView.CoreWebView2.Settings.AreDevToolsEnabled = False
                webView.CoreWebView2.Settings.IsWebMessageEnabled = False

                ' Set initial URL
                webView.Source = New Uri("https://duckduckgo.com")
            Catch ex As Exception
                MessageBox.Show($"Error initializing WebView2: {ex.Message}", "SurfClean Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End Sub
        
        ''' <summary>
        ''' Handles the start of navigation to update URL and loading state
        ''' </summary>
        Private Sub WebView_NavigationStarting(sender As Object, e As CoreWebView2NavigationStartingEventArgs)
            _viewModel.IsLoading = True
            _viewModel.CurrentUrl = e.Uri
        End Sub
        
        ''' <summary>
        ''' Updates UI state when navigation completes
        ''' </summary>
        Private Sub WebView_NavigationCompleted(sender As Object, e As CoreWebView2NavigationCompletedEventArgs)
            _viewModel.IsLoading = False
            _viewModel.UpdateNavigationState(webView.CanGoBack, webView.CanGoForward)

            ' Update URL in address bar to the final URL (after redirects)
            _viewModel.CurrentUrl = webView.Source.ToString()
        End Sub
        
        ''' <summary>
        ''' Intercepts and filters web requests to block trackers and third-party cookies
        ''' </summary>
        Private Sub CoreWebView2_WebResourceRequested(sender As Object, e As CoreWebView2WebResourceRequestedEventArgs)
            ' WaveWash Protection: Block known trackers
            If _viewModel.TrackingProtectionEnabled Then
                Dim url As String = e.Request.Uri.ToLower()

                ' Check if this resource is from a known tracker
                If _knownTrackerDomains.Any(Function(domain) url.Contains(domain)) Then
                    Try
                        ' Extract domain for display
                        Dim uri As New Uri(url)
                        Dim trackerDomain As String = uri.Host
                        
                        ' Block the tracker by returning a 403 response
                        e.Response = webView.CoreWebView2.Environment.CreateWebResourceResponse(Nothing, 403, "Blocked", "")
                        
                        ' Update tracker statistics
                        _viewModel.IncrementBlockedTrackers(trackerDomain)
                    Catch ex As Exception
                        ' In case of any errors parsing the URL, just continue
                    End Try
                End If
            End If

            ' SandScrubber Protection: Block third-party cookies
            If _viewModel.CookieBlockingEnabled AndAlso 
               e.ResourceContext.ToString().Equals("Cookie", StringComparison.OrdinalIgnoreCase) Then
                Try
                    ' Compare cookie domain to current page domain
                    Dim pageHost As String = New Uri(_viewModel.CurrentUrl).Host
                    Dim resourceHost As String = New Uri(e.Request.Uri).Host
                    
                    ' Block if domains don't match (third-party cookie)
                    If pageHost <> resourceHost Then
                        e.Response = webView.CoreWebView2.Environment.CreateWebResourceResponse(Nothing, 403, "Blocked", "")
                    End If
                Catch ex As Exception
                    ' In case of any errors parsing the URL, just continue
                End Try
            End If
        End Sub
        
        ''' <summary>
        ''' Prevents pop-up windows by forcing navigation in the same window (CoralGuard protection)
        ''' </summary>
        Private Sub CoreWebView2_NewWindowRequested(sender As Object, e As CoreWebView2NewWindowRequestedEventArgs)
            ' Prevent new windows, open in same window instead
            e.Handled = True
            webView.CoreWebView2.Navigate(e.Uri)
        End Sub
        
        ''' <summary>
        ''' Handles browser navigation commands from the view model
        ''' </summary>
        Private Sub ViewModel_WebViewRequest(sender As Object, e As WebViewRequestEventArgs) Handles _viewModel.WebViewRequest
            Select Case e.RequestType
                Case WebViewRequestType.Navigate
                    If Not String.IsNullOrEmpty(e.Url) Then
                        webView.CoreWebView2.Navigate(e.Url)
                    End If
                    
                Case WebViewRequestType.GoBack
                    If webView.CanGoBack Then
                        webView.GoBack()
                    End If
                    
                Case WebViewRequestType.GoForward
                    If webView.CanGoForward Then
                        webView.GoForward()
                    End If
                    
                Case WebViewRequestType.Refresh
                    webView.Reload()
                    
                Case WebViewRequestType.Stop
                    webView.Stop()
            End Select
        End Sub
        
        ''' <summary>
        ''' Handles Enter key press in the URL text box to trigger navigation
        ''' </summary>
        Private Sub UrlTextBox_KeyDown(sender As Object, e As KeyEventArgs)
            If e.Key = Key.Enter Then
                _viewModel.NavigateCommand.Execute(Nothing)
            End If
        End Sub

        Private Sub WebBrowserView_Unloaded(sender As Object, e As RoutedEventArgs) Handles Me.Unloaded
            If webView IsNot Nothing Then
                RemoveHandler webView.NavigationStarting, AddressOf WebView_NavigationStarting
                RemoveHandler webView.NavigationCompleted, AddressOf WebView_NavigationCompleted
                If webView.CoreWebView2 IsNot Nothing Then
                    RemoveHandler webView.CoreWebView2.WebResourceRequested, AddressOf CoreWebView2_WebResourceRequested
                    RemoveHandler webView.CoreWebView2.NewWindowRequested, AddressOf CoreWebView2_NewWindowRequested
                End If
            End If
        End Sub
    End Class
End Namespace