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
    Partial Public Class WebBrowserView
        Inherits UserControl
        
        
        Private WithEvents _viewModel As New WebBrowserViewModel()
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
        
        Public Sub New()
            ' This call is required by the designer.
            InitializeComponent()
            
            DataContext = _viewModel
            
            AddHandler Loaded, AddressOf WebBrowserView_Loaded
        End Sub
        
        Private Async Sub WebBrowserView_Loaded(sender As Object, e As RoutedEventArgs)
            ' Initialize WebView2
            Try
                Await webView.EnsureCoreWebView2Async()
                
                ' Setup event handlers
                AddHandler webView.NavigationStarting, AddressOf WebView_NavigationStarting
                AddHandler webView.NavigationCompleted, AddressOf WebView_NavigationCompleted
                AddHandler webView.CoreWebView2.WebResourceRequested, AddressOf CoreWebView2_WebResourceRequested
                AddHandler webView.CoreWebView2.NewWindowRequested, AddressOf CoreWebView2_NewWindowRequested

' Set the filter for resource types to monitor
                webView.CoreWebView2.AddWebResourceRequestedFilter("*", 
                    CoreWebView2WebResourceContext.All)

' Configure settings for clean surfing
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
        
        Private Sub WebView_NavigationStarting(sender As Object, e As CoreWebView2NavigationStartingEventArgs)
            _viewModel.IsLoading = True
            _viewModel.CurrentUrl = e.Uri
        End Sub
        
        Private Sub WebView_NavigationCompleted(sender As Object, e As CoreWebView2NavigationCompletedEventArgs)
            _viewModel.IsLoading = False
            _viewModel.UpdateNavigationState(webView.CanGoBack, webView.CanGoForward)

' Update URL in address bar to the final URL (after redirects)
            _viewModel.CurrentUrl = webView.Source.ToString()
        End Sub
        
        Private Sub CoreWebView2_WebResourceRequested(sender As Object, e As CoreWebView2WebResourceRequestedEventArgs)
            If _viewModel.TrackingProtectionEnabled Then
                Dim url As String = e.Request.Uri.ToLower()

' Check if this resource is from a known tracker
                If _knownTrackerDomains.Any(Function(domain) url.Contains(domain)) Then
                    Try
                        ' Extract domain for display
                        Dim uri As New Uri(url)
                        Dim trackerDomain As String = uri.Host
                        
                        ' Block the tracker (WaveWash)
                        e.Response = webView.CoreWebView2.Environment.CreateWebResourceResponse(Nothing, 403, "Blocked", "")
                        
                        ' Update tracker count
                        _viewModel.IncrementBlockedTrackers(trackerDomain)
                    Catch ex As Exception
                        ' In case of any errors parsing the URL, just continue
                    End Try
                End If
            End If

' Apply SandScrubber if enabled
' Fix: Cookie context comparison
            If _viewModel.CookieBlockingEnabled AndAlso 
               e.ResourceContext.ToString().Equals("Cookie", StringComparison.OrdinalIgnoreCase) Then
Try
' Block third-party cookies
                    Dim pageHost As String = New Uri(_viewModel.CurrentUrl).Host
                    Dim resourceHost As String = New Uri(e.Request.Uri).Host
                    
                    If pageHost <> resourceHost Then
                        e.Response = webView.CoreWebView2.Environment.CreateWebResourceResponse(Nothing, 403, "Blocked", "")
                    End If
                Catch ex As Exception
                    ' In case of any errors parsing the URL, just continue
                End Try
            End If
        End Sub
        
        Private Sub CoreWebView2_NewWindowRequested(sender As Object, e As CoreWebView2NewWindowRequestedEventArgs)
' Prevent new windows, open in same window instead (part of CoralGuard protection)
            e.Handled = True
            webView.CoreWebView2.Navigate(e.Uri)
        End Sub
        
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
        
        Private Sub UrlTextBox_KeyDown(sender As Object, e As KeyEventArgs)
            If e.Key = Key.Enter Then
                _viewModel.NavigateCommand.Execute(Nothing)
            End If
        End Sub
    End Class
End Namespace