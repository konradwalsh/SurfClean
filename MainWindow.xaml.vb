Imports System.Windows
Imports System.Windows.Controls
Imports Surf_Clean.ViewModels
Imports Surf_Clean.Models
Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Namespace Surf_Clean
    ''' <summary>
    ''' Interaction logic for MainWindow.xaml
    ''' </summary>
    Partial Public Class MainWindow
        Inherits Window
        
        ''' <summary>
        ''' Initializes a new instance of the MainWindow class
        ''' </summary>
        Public Sub New()
' This call is required by the designer.
            InitializeComponent()
            
            ' Add any initialization after the InitializeComponent() call.
            DataContext = New MainViewModel()
            
            ' Set initial page
            ShowPage(BrowserPage)
            SetActiveButton(BrowserButton)
        End Sub
        
        ''' <summary>
        ''' Handles navigation button clicks to switch between application pages
        ''' </summary>
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
        
        ''' <summary>
        ''' Shows the specified page and hides all others
        ''' </summary>
        Private Sub ShowPage(pageToShow As UIElement)
            ' Hide all pages
            DashboardPage.Visibility = Visibility.Collapsed
            ScannerPage.Visibility = Visibility.Collapsed
            BrowserPage.Visibility = Visibility.Collapsed
            SettingsPage.Visibility = Visibility.Collapsed
            
            ' Show the selected page
            pageToShow.Visibility = Visibility.Visible
        End Sub
        
        ''' <summary>
        ''' Sets the active navigation button style
        ''' </summary>
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