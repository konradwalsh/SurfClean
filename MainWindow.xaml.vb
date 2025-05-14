Imports System.Windows
Imports System.Windows.Controls
Imports Surf_Clean.ViewModels
Imports Surf_Clean.Models

Namespace Surf_Clean
    ''' <summary>
    ''' Interaction logic for MainWindow.xaml
    ''' </summary>
    Partial Public Class MainWindow
        Inherits Window
        
        Public Sub New()
' This call is required by the designer.
            InitializeComponent()
            
            ' Add any initialization after the InitializeComponent() call.
            DataContext = New MainViewModel()
            
            ' Set initial page
            ShowPage(BrowserPage)
            SetActiveButton(BrowserButton)
        End Sub
        
        Private Sub NavigationButton_Click(sender As Object, e As RoutedEventArgs)
            Dim clickedButton As Button = TryCast(sender, Button)
            
            If clickedButton Is DashboardButton Then
                ShowPage(DashboardPage)
                SetActiveButton(DashboardButton)
            ElseIf clickedButton Is ScannerButton Then
                ShowPage(ScannerPage)
                SetActiveButton(ScannerButton)
            ElseIf clickedButton Is BrowserButton Then
                ShowPage(BrowserPage)
                SetActiveButton(BrowserButton)
            ElseIf clickedButton Is SettingsButton Then
                ShowPage(SettingsPage)
                SetActiveButton(SettingsButton)
            End If
        End Sub
        
        Private Sub ShowPage(pageToShow As UIElement)
            ' Hide all pages
            DashboardPage.Visibility = Visibility.Collapsed
            ScannerPage.Visibility = Visibility.Collapsed
            BrowserPage.Visibility = Visibility.Collapsed
            SettingsPage.Visibility = Visibility.Collapsed
            
            ' Show the selected page
            pageToShow.Visibility = Visibility.Visible
        End Sub
        
        Private Sub SetActiveButton(activeButton As Button)
            ' Reset all buttons to default style
            DashboardButton.Style = TryCast(FindResource("NavButtonStyle"), Style)
            ScannerButton.Style = TryCast(FindResource("NavButtonStyle"), Style)
            BrowserButton.Style = TryCast(FindResource("NavButtonStyle"), Style)
            SettingsButton.Style = TryCast(FindResource("NavButtonStyle"), Style)
            
            ' Set active button style
            activeButton.Style = TryCast(FindResource("ActiveNavButtonStyle"), Style)
        End Sub
    End Class
End Namespace