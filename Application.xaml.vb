Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Markup

Namespace Surf_Clean
    Partial Class App
        Inherits Application
  
        
        ''' <summary>
        ''' Application theme colors for consistent styling
        ''' </summary>
        Public Class SurfCleanColors
            ' Use the Colors class and/or Color constructor instead of FromRgb
            Public Shared ReadOnly PrimaryBlue As Color = Color.FromArgb(255, 0, 188, 212)     ' #00BCD4 - Ocean blue
            Public Shared ReadOnly DarkTeal As Color = Color.FromArgb(255, 0, 96, 100)         ' #006064 - Deep sea
            Public Shared ReadOnly LightCyan As Color = Color.FromArgb(255, 224, 247, 250)     ' #E0F7FA - Seafoam
            Public Shared ReadOnly SandYellow As Color = Color.FromArgb(255, 255, 193, 7)      ' #FFC107 - Beach sand
            Public Shared ReadOnly CoralOrange As Color = Color.FromArgb(255, 255, 87, 34)     ' #FF5722 - Coral reef
        End Class
        
        ''' <summary>
        ''' Status messages with ocean theme
        ''' </summary>
        Public Class SurfCleanMessages
            Public Const Welcome As String = "Welcome to SurfClean - Where the web washes clean"
            Public Const ScanStarted As String = "Checking the surf conditions..."
            Public Const TrackersFound As String = "Found {0} trackers in the tide. Ready to wash them away?"
            Public Const CleaningComplete As String = "All clean! Your digital footprints have been washed away."
            Public Const HighTide As String = "High tracking tide detected! Activating WaveWash protection."
        End Class
        
        Protected Overrides Sub OnStartup(e As StartupEventArgs)
            MyBase.OnStartup(e)
            
            ' Set up global exception handling
            AddHandler Current.DispatcherUnhandledException, Sub(s, args)
                MessageBox.Show($"An unexpected error occurred while surfing: {args.Exception.Message}{vbCrLf}{vbCrLf}Please report this to the developer.",
                                "SurfClean Error", MessageBoxButton.OK, MessageBoxImage.Error)
                args.Handled = True
            End Sub
        End Sub
    End Class
End Namespace