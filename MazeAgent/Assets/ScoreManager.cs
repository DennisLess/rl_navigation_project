using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    // Sinlgeton dmait MazeAgentScript auf die Statistiken zugreifen kann
    public static ScoreManager Instance;

    [Header("UI Referenzen")]
    public TextMeshProUGUI scoreboardText;
    public TextMeshProUGUI algoText;
    public Button resetButton;


     [Header("Einstellungen")]
    public string currentAlgorithm = "PPO";

    // Statistiken für jedes Environment (Index 0, 1, 2)
    private int[] episodeCount = new int[3];
    private int[] goalReached  = new int[3];
    private int[] wallHit      = new int[3];
    private int[] timeout      = new int[3];

    private void Awake()
    {
        Instance = this;
    }

     private void Start()
    {
        resetButton.onClick.AddListener(ResetStats);
        algoText.text = currentAlgorithm;
        UpdateUI();
    }

    public void OnEpisodeStart(int envIndex)
    {
        episodeCount[envIndex]++;
        UpdateUI();
    }

    public void OnGoalReached(int envIndex)
    {
        goalReached[envIndex]++;
        UpdateUI();
    }

    public void OnWallHit(int envIndex)
    {
        wallHit[envIndex]++;
        UpdateUI();
    }

    public void OnTimeout(int envIndex)
    {
        timeout[envIndex]++;
        UpdateUI();
    }

    public void ResetStats()
    {
        for (int i = 0; i < 3; i++)
        {
            episodeCount[i] = 0;
            goalReached[i]  = 0;
            wallHit[i]      = 0;
            timeout[i]      = 0;
        }
        UpdateUI();
        Debug.Log("Scoreboard zurückgesetzt!");
    }

    private void UpdateUI()
    {
        string[] envNames = {
            "Env 1 - Simple ",
            "Env 2 - Complex ",
            "Env 3 - Warehouse "
        };

        string display = "";
        for (int i = 0; i < 3; i++)
        {
            int completedEpisodes = goalReached[i] + wallHit[i] + timeout[i];
            
            float rate = completedEpisodes > 0
                ? (float)goalReached[i] / completedEpisodes * 100f
                : 0f;

            display += $"{envNames[i]}  |  Episode: {episodeCount[i]}\n";
            display += $"  Ep: {completedEpisodes}  Goal: {goalReached[i]}  " +
                       $"Wall: {wallHit[i]}  Rate: {rate:F1}%\n\n";
        }

        scoreboardText.text = display;
    }
}
