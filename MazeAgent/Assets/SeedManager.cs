using UnityEngine;

public class SeedManager : MonoBehaviour
{
    public static SeedManager Instance; // Singleton, damit andere Scripts auf den Seed zugreifen können

    [Header("Seed Settings")]
    public int seed = 1; // Seed wird im Inspector pro Run gesetzt

    [Header("Debug")]
    public bool logSeed = true; // Kann aktiviert bleiben, damit man den Seed in der Console sieht

    private void Awake()
    {
        Instance = this; // aktuelle Instanz global verfügbar machen

        Random.InitState(seed); // Unity-Zufallsgenerator reproduzierbar setzen

        if (logSeed)
        {
            Debug.Log($"[SeedManager] Seed set to: {seed}"); // kurzer Debug-Check für den aktuellen Seed
        }
    }
}