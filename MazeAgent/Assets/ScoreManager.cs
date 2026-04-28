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
    [SerializeField] private string currentAlgorithm = "PPO";
    [SerializeField] private string currentEnvironment = "Env 1 - Simple";

    // Statistiken für eine Environment pro Szene
    private int episodeCount = 0;
    private int goalReached = 0;
    private int wallHit = 0;
    private int timeout = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        resetButton.onClick.AddListener(ResetStats);

        algoText.text = currentAlgorithm;
        envText.text = currentEnvironment;

        UpdateUI();
    }

    public void OnEpisodeStart()
    {
        episodeCount++;
        UpdateUI();
    }

    public void OnGoalReached()
    {
        goalReached++;
        UpdateUI();
    }

    public void OnWallHit()
    {
        wallHit++;
        UpdateUI();
    }

    public void OnTimeout()
    {
        timeout++;
        UpdateUI();
    }

    public void ResetStats()
    {
        episodeCount = 0;
        goalReached = 0;
        wallHit = 0;
        timeout = 0;

        UpdateUI();
        Debug.Log("Scoreboard zurückgesetzt!");
    }

    private void UpdateUI()
    {
        int completedEpisodes = goalReached + wallHit + timeout;

        float rate = completedEpisodes > 0
            ? (float)goalReached / completedEpisodes * 100f
            : 0f;

        string display = $"{currentEnvironment} | Episode: {episodeCount}\n";
        display += $"Finished: {completedEpisodes}  " +
                   $"Goal: {goalReached}  " +
                   $"Wall: {wallHit}  " +
                   $"Timeout: {timeout}  " +
                   $"Rate: {rate:F1}%\n";

        scoreboardText.text = display;
    }
}