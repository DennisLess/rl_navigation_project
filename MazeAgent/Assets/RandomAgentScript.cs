using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

// Random Agent als Baseline, der zufällige Aktionen aus demselben Aktionsraum wählt
public class RandomAgentScript : Agent
{
    [Header("Einstellungen")]
    public float moveSpeed = 8f; // muss identisch zum trainierten Agenten sein
    public float turnSpeed = 150f; // muss identisch zum trainierten Agenten sein

    [Header("Ziel")]
    public Transform goal; // Ziel-Objekt, wird im Inspector zugewiesen

    [Header("Spawn & Goal Punkte")]
    public Transform[] spawnPoints; // mögliche Spawnpunkte
    public Transform[] goalPoints; // mögliche Zielpositionen

    private Rigidbody rb; // Rigidbody des Agenten

    private int lastSpawnIndex = -1; // letzter Spawn, damit es nicht direkt wieder derselbe ist
    private int lastGoalIndex = -1; // letztes Ziel, damit es nicht direkt wieder dasselbe ist

    private int currentSpawnIndex = -1; // aktueller Spawnindex für CSV
    private int currentGoalIndex = -1; // aktueller Goalindex für CSV

    private float episodeReward = 0f; // sammelt Reward pro Episode
    private bool episodeFinished = false; // verhindert doppeltes Loggen

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody einmalig holen
    }

    public override void OnEpisodeBegin()
    {
        ScoreManager.Instance?.OnEpisodeStart(); // UI über neue Episode informieren

        episodeReward = 0f; // Reward für neue Episode zurücksetzen
        episodeFinished = false; // Episode ist am Anfang noch offen

        int spawnIndex; // neuer Spawnindex

        do
        {
            spawnIndex = Random.Range(0, spawnPoints.Length); // zufälligen Spawn wählen
        }
        while (spawnIndex == lastSpawnIndex && spawnPoints.Length > 1); // möglichst nicht denselben wie vorher

        lastSpawnIndex = spawnIndex; // Spawn merken
        currentSpawnIndex = spawnIndex; // Spawn fürs Logging merken

        transform.position = spawnPoints[spawnIndex].position; // Agent an Spawn setzen
        transform.rotation = spawnPoints[spawnIndex].rotation; // Rotation übernehmen

        int goalIndex; // neuer Goalindex

        do
        {
            goalIndex = Random.Range(0, goalPoints.Length); // zufälliges Ziel wählen
        }
        while (goalIndex == lastGoalIndex && goalPoints.Length > 1); // möglichst nicht dasselbe Ziel wie vorher

        lastGoalIndex = goalIndex; // Ziel merken
        currentGoalIndex = goalIndex; // Ziel fürs Logging merken

        goal.position = goalPoints[goalIndex].position; // Ziel verschieben

        rb.linearVelocity = Vector3.zero; // alte Bewegung stoppen
        rb.angularVelocity = Vector3.zero; // alte Rotation stoppen
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 toGoal = goal.position - transform.position; // Vektor zum Ziel

        sensor.AddObservation(transform.InverseTransformDirection(toGoal.normalized)); // gleiche Beobachtung wie beim Trainingsagenten
        sensor.AddObservation(toGoal.magnitude / 20f); // gleiche Distanzbeobachtung wie beim Trainingsagenten
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int action = Random.Range(0, 9); // zufällige Aktion aus Discrete(9)

        int moveAction = action % 3; // 0 = nichts, 1 = vorwärts, 2 = rückwärts
        int turnAction = action / 3; // 0 = nichts, 1 = links, 2 = rechts

        if (moveAction == 1)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime, Space.Self); // vorwärts fahren
        }

        if (moveAction == 2)
        {
            transform.Translate(Vector3.back * moveSpeed * Time.fixedDeltaTime, Space.Self); // rückwärts fahren
        }

        if (turnAction == 1)
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.fixedDeltaTime, Space.Self); // links drehen
        }

        if (turnAction == 2)
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.fixedDeltaTime, Space.Self); // rechts drehen
        }

        AddTrackedReward(-0.001f); // gleiche Schrittstrafe wie beim Trainingsagenten

        if (StepCount == MaxStep - 1)
        {
            FinishEpisode("Timeout"); // Timeout loggen
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            AddTrackedReward(5.0f); // gleicher Reward für Ziel

            FinishEpisode("Goal"); // Erfolg loggen

            EndEpisode(); // Episode beenden
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddTrackedReward(-0.5f); // gleiche Wandstrafe

            FinishEpisode("Wall"); // Wandkollision loggen

            EndEpisode(); // Episode beenden
        }
    }

    private void AddTrackedReward(float reward)
    {
        AddReward(reward); // Reward an ML-Agents geben
        episodeReward += reward; // Reward für CSV aufsummieren
    }

    private void FinishEpisode(string result)
    {
        if (episodeFinished)
        {
            return; // verhindert doppeltes Loggen
        }

        episodeFinished = true; // Episode als beendet markieren

        if (result == "Goal")
        {
            ScoreManager.Instance?.OnGoalReached(); // UI Goal hochzählen
        }
        else if (result == "Wall")
        {
            ScoreManager.Instance?.OnWallHit(); // UI Wall hochzählen
        }
        else if (result == "Timeout")
        {
            ScoreManager.Instance?.OnTimeout(); // UI Timeout hochzählen
        }

        EpisodeLogger.Instance?.LogEpisode(
            result,
            StepCount,
            episodeReward,
            currentSpawnIndex,
            currentGoalIndex,
            MaxStep
        ); // Episode in CSV schreiben
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var da = actionsOut.DiscreteActions; // ActionBuffer holen

        da[0] = 0; // Wert ist egal, da OnActionReceived selbst randomisiert
    }
}