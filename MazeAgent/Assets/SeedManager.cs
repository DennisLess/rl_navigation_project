using UnityEngine;

public class SeedManager : MonoBehaviour
{
    public static SeedManager Instance;

    [Header("Seed Settings")]
    public int seed = 1;
    
    [Header("Debug")]
    public bool logSeed = true;

    private void Awake()
    {
        Instance = this;
        Random.InitState(seed);

        if(logSeed)
        {
            Debug.Log($"[Seed Manager] Seed set to: {seed}");
        }
    }


}
