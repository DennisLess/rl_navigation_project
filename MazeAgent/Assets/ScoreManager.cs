using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    // Singleton, damit MazeAgentScript auf den ScoreManager zugreifen kann
    public static ScoreManager Instance;

    [Header("UI Referenzen")]
    public TextMeshProUGUI scoreboardText;
    public TextMeshProUGUI algoText;
    public TextMeshProUGUI envText;
    public Button resetButton;

    [Header("Einstellungen")]
    [SerializeField] private string currentAlgorithm = "Random";
    [SerializeField] private string currentEnvironment = "Env 2 - Complex";

    // Episode bleibt als Anzeige bestehen, wird aber nicht mehr durch jeden Reset hochgezählt
    private int episodeCount = 0;

    // Statistiken für abgeschlossene Episoden
    private int goalReached = 0;
    private int wallHit = 0;
    private int timeout = 0;

    private void Awake()
    {
        Instance = this; // aktuelle ScoreManager-Instanz global verfügbar machen
    }

    private void Start()
    {
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetStats); // Reset-Button verbinden, falls vorhanden
        }

        if (algoText != null)
        {
            algoText.text = currentAlgorithm; // Algorithmus anzeigen
        }

        if (envText != null)
        {
            envText.text = currentEnvironment; // Environment anzeigen, falls genutzt
        }

        UpdateUI(); // UI initial aktualisieren
    }

    public void OnEpisodeStart()
    {
        // Wichtig:
        // Bei SB3/Unity kann OnEpisodeBegin öfter durch Resets ausgelöst werden.
        // Deshalb wird hier nicht einfach hochgezählt, sondern nur die aktuelle Episode sauber gesetzt.
        int completedEpisodes = goalReached + wallHit + timeout;

        episodeCount = completedEpisodes + 1; // aktuelle laufende Episode = fertige Episoden + 1

        UpdateUI(); // UI aktualisieren
    }

    public void OnGoalReached()
    {
        goalReached++; // erfolgreiche Episode zählen

        int completedEpisodes = goalReached + wallHit + timeout;
        episodeCount = completedEpisodes + 1; // nächste laufende Episode vorbereiten

        UpdateUI(); // UI aktualisieren
    }

    public void OnWallHit()
    {
        wallHit++; // Wandkollision zählen

        int completedEpisodes = goalReached + wallHit + timeout;
        episodeCount = completedEpisodes + 1; // nächste laufende Episode vorbereiten

        UpdateUI(); // UI aktualisieren
    }

    public void OnTimeout()
    {
        timeout++; // Timeout zählen

        int completedEpisodes = goalReached + wallHit + timeout;
        episodeCount = completedEpisodes + 1; // nächste laufende Episode vorbereiten

        UpdateUI(); // UI aktualisieren
    }

    public void ResetStats()
    {
        episodeCount = 0; // Anzeige zurücksetzen
        goalReached = 0; // Goals zurücksetzen
        wallHit = 0; // Walls zurücksetzen
        timeout = 0; // Timeouts zurücksetzen

        UpdateUI(); // UI aktualisieren
        Debug.Log("Scoreboard zurückgesetzt!");
    }

    private void UpdateUI()
    {
        int completedEpisodes = goalReached + wallHit + timeout; // wirklich abgeschlossene Episoden

        float rate = completedEpisodes > 0
            ? (float)goalReached / completedEpisodes * 100f
            : 0f;

        string display = $"{currentEnvironment} | Episode: {episodeCount}\n";
        display += $"Finished: {completedEpisodes}\n";
        display += $"Goal: {goalReached}\n";
        display += $"Wall: {wallHit}\n";
        display += $"Timeout: {timeout}\n";
        display += $"Rate: {rate:F1}%\n";

        if (scoreboardText != null)
        {
            scoreboardText.text = display; // Text ins Scoreboard schreiben
        }
    }
}