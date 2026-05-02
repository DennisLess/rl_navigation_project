using System;
using System.IO;
using System.Globalization;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EpisodeLogger : MonoBehaviour
{
    public static EpisodeLogger Instance; // Singleton, damit Agent-Scripts einfach darauf zugreifen können

    [Header("Experiment Settings")]
    public string runId = "Random_Env1_Seed1"; // eindeutiger Name für diesen Lauf
    public string environmentName = "Env1_Simple"; // aktuelles Environment
    public string algorithmName = "Random"; // Random, PPO, A2C oder DQN

    [Header("Logging Settings")]
    public bool enableLogging = true; // Logging global an- oder ausschalten
    public string outputFolder = "training/evaluation_logs"; // Zielordner relativ zum Unity-Projektordner

    [Header("Run Limits")]
    public bool useMaxEpisodes = true; // wenn aktiv, wird nach einer festen Episodenanzahl gestoppt
    public int maxEpisodes = 200; // maximale Anzahl abgeschlossener Episoden

    public bool useMaxTotalSteps = false; // wenn aktiv, wird nach einer Gesamtanzahl Steps gestoppt
    public int maxTotalSteps = 500000; // maximale Gesamtanzahl an Steps über alle Episoden

    public bool stopPlayModeWhenLimitReached = true; // stoppt Play Mode automatisch, wenn ein Limit erreicht wurde

    [Header("Debug")]
    public bool logFilePath = true; // gibt den CSV-Pfad in der Console aus
    public bool logLimitReached = true; // gibt eine Meldung aus, wenn ein Stop-Limit erreicht wurde

    private string logFilePathInternal; // kompletter Pfad zur CSV-Datei
    private int loggedEpisodeCount = 0; // zählt alle geloggten Episoden
    private int totalLoggedSteps = 0; // zählt alle Steps über alle geloggten Episoden

    private bool runStopped = false; // verhindert mehrfaches Stoppen

    private void Awake()
    {
        Instance = this; // aktuelle Logger-Instanz global verfügbar machen
    }

    private void Start()
    {
        if (!enableLogging)
        {
            Debug.Log("[EpisodeLogger] Logging is disabled."); // kurzer Hinweis, falls Logging aus ist
            return;
        }

        int seed = SeedManager.Instance != null ? SeedManager.Instance.seed : 0; // Seed aus SeedManager holen

        string projectRoot = Directory.GetParent(Application.dataPath).FullName; // Unity-Projektordner bestimmen
        string fullOutputFolder = Path.Combine(projectRoot, outputFolder); // Zielordner zusammensetzen

        Directory.CreateDirectory(fullOutputFolder); // Ordner erstellen, falls er noch nicht existiert

        string safeRunId = MakeSafeFileName(runId); // runId dateisicher machen

        string fileName = $"{safeRunId}_Seed{seed}.csv"; // CSV-Dateiname ohne Zeitstempel
        logFilePathInternal = Path.Combine(fullOutputFolder, fileName); // kompletter Dateipfad

        string header =
            "run_id,environment,algorithm,seed,episode,result,steps,total_reward,spawn_index,goal_index,max_steps,success,wall_hit,timeout,total_logged_steps\n";

        File.WriteAllText(logFilePathInternal, header); // CSV-Datei mit Header anlegen

        if (logFilePath)
        {
            Debug.Log($"[EpisodeLogger] Logging to: {logFilePathInternal}"); // Pfad in Console anzeigen
        }
    }

    public void LogEpisode(
        string result,
        int steps,
        float totalReward,
        int spawnIndex,
        int goalIndex,
        int maxSteps
    )
    {
        if (!enableLogging || string.IsNullOrEmpty(logFilePathInternal))
        {
            return; // nicht loggen, wenn Logging deaktiviert oder Datei nicht vorbereitet ist
        }

        if (runStopped)
        {
            return; // verhindert weitere Logeinträge, nachdem ein Limit erreicht wurde
        }

        loggedEpisodeCount++; // Episode hochzählen
        totalLoggedSteps += steps; // Steps dieser Episode zur Gesamtsumme addieren

        int seed = SeedManager.Instance != null ? SeedManager.Instance.seed : 0; // Seed aus SeedManager holen

        int success = result == "Goal" ? 1 : 0; // 1, wenn Ziel erreicht wurde
        int wallHit = result == "Wall" ? 1 : 0; // 1, wenn Wand getroffen wurde
        int timeout = result == "Timeout" ? 1 : 0; // 1, wenn Episode durch MaxStep endete

        string line =
            $"{Sanitize(runId)}," +
            $"{Sanitize(environmentName)}," +
            $"{Sanitize(algorithmName)}," +
            $"{seed}," +
            $"{loggedEpisodeCount}," +
            $"{Sanitize(result)}," +
            $"{steps}," +
            $"{totalReward.ToString("F4", CultureInfo.InvariantCulture)}," +
            $"{spawnIndex}," +
            $"{goalIndex}," +
            $"{maxSteps}," +
            $"{success}," +
            $"{wallHit}," +
            $"{timeout}," +
            $"{totalLoggedSteps}\n";

        File.AppendAllText(logFilePathInternal, line); // Episode als CSV-Zeile anhängen

        CheckStopConditions(); // nach jeder Episode prüfen, ob ein Limit erreicht wurde
    }

    private void CheckStopConditions()
    {
        bool reachedEpisodeLimit = useMaxEpisodes && maxEpisodes > 0 && loggedEpisodeCount >= maxEpisodes; // Episodenlimit prüfen
        bool reachedStepLimit = useMaxTotalSteps && maxTotalSteps > 0 && totalLoggedSteps >= maxTotalSteps; // Gesamtstep-Limit prüfen

        if (!reachedEpisodeLimit && !reachedStepLimit)
        {
            return; // kein Limit erreicht, also weiterlaufen
        }

        runStopped = true; // markieren, dass der Run gestoppt wurde

        if (logLimitReached)
        {
            Debug.Log(
                $"[EpisodeLogger] Limit reached. Episodes: {loggedEpisodeCount}, Total Steps: {totalLoggedSteps}"
            ); // kurze Zusammenfassung in der Console
        }

        if (stopPlayModeWhenLimitReached)
        {
            StopPlayMode(); // Play Mode stoppen, wenn gewünscht
        }
    }

    private void StopPlayMode()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // stoppt Play Mode im Unity Editor
#else
        Application.Quit(); // beendet Build-Version, falls außerhalb vom Editor genutzt
#endif
    }

    private string Sanitize(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return ""; // leere Werte vermeiden Null-Probleme
        }

        return value.Replace(",", "_"); // Kommas ersetzen, damit CSV sauber bleibt
    }

    private string MakeSafeFileName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            value = "run"; // Fallback-Dateiname
        }

        foreach (char c in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(c, '_'); // ungültige Dateizeichen ersetzen
        }

        return value.Replace(" ", "_"); // Leerzeichen im Dateinamen vermeiden
    }
}