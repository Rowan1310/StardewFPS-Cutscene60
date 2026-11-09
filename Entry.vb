Imports System.Globalization
Imports StardewModdingAPI
Imports StardewModdingAPI.Events
Imports StardewValley

Public Class Entry
    Inherits [Mod]

    Private _gameplayTarget As TimeSpan? = Nothing
    Private _isCutsceneLocked As Boolean = False

    Public Overrides Sub Entry(helper As IModHelper)
        helper.ConsoleCommands.Add(
            "fps",
            "Usage: fps <number>, like: fps 120, means run to 120 FPS (cutscenes stay at 60 FPS)",
            AddressOf SetFPS)

        helper.Events.GameLoop.UpdateTicked += AddressOf OnUpdateTicked
    End Sub

    Public Sub SetFPS(command As String, args As String())
        If args Is Nothing OrElse args.Length = 0 Then
            Me.Monitor.Log("Usage: fps <number> (example: fps 120).", LogLevel.Warn)
            Return
        End If

        Dim requestedFps As Double
        If Not Double.TryParse(args(0), NumberStyles.Float, CultureInfo.InvariantCulture, requestedFps) _
            OrElse Double.IsNaN(requestedFps) _
            OrElse Double.IsInfinity(requestedFps) _
            OrElse requestedFps <= 0 Then
            Me.Monitor.Log("FPS must be a number greater than 0.", LogLevel.Warn)
            Return
        End If

        Dim runner = GameRunner.instance
        If runner Is Nothing Then
            Me.Monitor.Log("GameRunner is not ready yet; try the command again.", LogLevel.Warn)
            Return
        End If

        If Not _gameplayTarget.HasValue Then
            _gameplayTarget = runner.TargetElapsedTime
        End If

        _gameplayTarget = TimeSpan.FromSeconds(1.0R / requestedFps)

        ' If a cutscene is playing, keep the active target at 60 FPS.
        ' The new gameplay target is restored automatically when it ends.
        If IsCutsceneActive() Then
            ApplyTarget(runner, TimeSpan.FromSeconds(1.0R / 60.0R))
            _isCutsceneLocked = True
        Else
            ApplyTarget(runner, _gameplayTarget.Value)
            _isCutsceneLocked = False
        End If

        Me.Monitor.Log($"Gameplay FPS set to {requestedFps.ToString(CultureInfo.InvariantCulture)}. Cutscenes are locked to 60 FPS.", LogLevel.Info)
    End Sub

    Private Sub OnUpdateTicked(sender As Object, e As UpdateTickedEventArgs)
        Dim runner = GameRunner.instance
        If runner Is Nothing Then
            Return
        End If

        ' Preserve the game's own default target until the player chooses an FPS.
        If Not _gameplayTarget.HasValue Then
            _gameplayTarget = runner.TargetElapsedTime
        End If

        Dim cutsceneActive As Boolean = IsCutsceneActive()
        If cutsceneActive Then
            If Not _isCutsceneLocked OrElse runner.TargetElapsedTime <> TimeSpan.FromSeconds(1.0R / 60.0R) Then
                ApplyTarget(runner, TimeSpan.FromSeconds(1.0R / 60.0R))
            End If
            _isCutsceneLocked = True
        Else
            If _isCutsceneLocked Then
                ApplyTarget(runner, _gameplayTarget.Value)
            End If
            _isCutsceneLocked = False
        End If
    End Sub

    Private Shared Function IsCutsceneActive() As Boolean
        Return Game1.CurrentEvent IsNot Nothing OrElse Game1.eventUp
    End Function

    Private Shared Sub ApplyTarget(runner As GameRunner, target As TimeSpan)
        runner.TargetElapsedTime = target
        runner.IsFixedTimeStep = True
    End Sub
End Class
