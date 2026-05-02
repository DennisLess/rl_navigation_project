using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

// Klasse für den trainierten Agenten, der das Labyrinth durchqueren soll
public class MazeAgentScript : Agent
{
    [Header("Einstellungen")]
    public float moveSpeed = 8f; // Geschwindigkeit für Vorwärts- und Rückwärtsbewegung
    public float turnSpeed = 150f; // Drehgeschwindigkeit in Grad pro Sekunde

    [Header("Ziel")]
    public Transform goal; // Ziel-Objekt, wird im Inspector zugewiesen

    [Header("Spawn & Goal Punkte")]
    public Transform[] spawnPoints; // mögliche Spawnpunkte für den Agenten
    public Transform[] goalPoints; // mögliche Zielpositionen

    private Rigidbody rb; // Rigidbody des Agenten

    private int lastSpawnIndex = -1; // letzter Spawn, damit nicht immer derselbe kommt
    private int lastGoalIndex = -1; // letztes Ziel, damit nicht immer dasselbe kommt

    private int currentSpawnIndex = -1; // aktueller Spawnindex für das Logging
    private int currentGoalIndex = -1; // aktueller Goalindex für das Logging

    private float episodeReward = 0f; // sammelt Reward pro Episode für die CSV
    private bool episodeFinished = false; // verhindert doppeltes Loggen derselben Episode

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody einmalig holen
    }

    public override void OnEpisodeBegin()
    {
        ScoreManager.Instance?.OnEpisodeStart(); // UI über neue Episode informieren

        episodeReward = 0f; // Reward für neue Episode zurücksetzen
        episodeFinished = false; // neue Episode ist noch nicht beendet

        int spawnIndex; // hier wird der neue Spawn gespeichert

        do
        {
            spawnIndex = Random.Range(0, spawnPoints.Length); // zufälligen Spawn wählen
        }
        while (spawnIndex == lastSpawnIndex && spawnPoints.Length > 1); // möglichst nicht denselben wie vorher

        lastSpawnIndex = spawnIndex; // letzten Spawn merken
        currentSpawnIndex = spawnIndex; // aktuellen Spawn fürs Logging merken

        transform.position = spawnPoints[spawnIndex].position; // Agent an Spawn setzen
        transform.rotation = spawnPoints[spawnIndex].rotation; // Agent-Rotation vom Spawn übernehmen

        int goalIndex; // hier wird das neue Ziel gespeichert

        do
        {
            goalIndex = Random.Range(0, goalPoints.Length); // zufälligen Goalpoint wählen
        }
        while (goalIndex == lastGoalIndex && goalPoints.Length > 1); // möglichst nicht denselben wie vorher

        lastGoalIndex = goalIndex; // letztes Ziel merken
        currentGoalIndex = goalIndex; // aktuelles Ziel fürs Logging merken

        goal.position = goalPoints[goalIndex].position; // Ziel an neue Position setzen

        rb.linearVelocity = Vector3.zero; // alte Bewegung zurücksetzen
        rb.angularVelocity = Vector3.zero; // alte Drehbewegung zurücksetzen
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 toGoal = goal.position - transform.position; // Vektor vom Agenten zum Ziel

        sensor.AddObservation(transform.InverseTransformDirection(toGoal.normalized)); // Zielrichtung relativ zum Agenten
        sensor.AddObservation(toGoal.magnitude / 20f); // Distanz grob normalisiert
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int action = actions.DiscreteActions[0]; // eine diskrete Aktion von 0 bis 8

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
            transform.Rotate(Vector3.up, -turnSpeed * Time.fixedDeltaTime, Space.Self); // nach links drehen
        }

        if (turnAction == 2)
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.fixedDeltaTime, Space.Self); // nach rechts drehen
        }

        AddTrackedReward(-0.001f); // kleine Schrittstrafe für effizientes Verhalten

        if (StepCount == MaxStep - 1)
        {
            FinishEpisode("Timeout"); // Timeout loggen, kurz bevor ML-Agents beendet
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            AddTrackedReward(5.0f); // Belohnung für Zielerreichung

            FinishEpisode("Goal"); // Episode als Erfolg loggen

            EndEpisode(); // Episode beenden
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddTrackedReward(-0.5f); // Strafe für Wandkollision

            FinishEpisode("Wall"); // Episode als Wandkollision loggen

            EndEpisode(); // Episode beenden
        }
    }

    private void AddTrackedReward(float reward)
    {
        AddReward(reward); // Reward an ML-Agents geben
        episodeReward += reward; // Reward zusätzlich für CSV sammeln
    }

    private void FinishEpisode(string result)
    {
        if (episodeFinished)
        {
            return; // verhindert doppeltes Zählen oder doppeltes Loggen
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
        var da = actionsOut.DiscreteActions; // Zugriff auf diskrete Aktion

        int moveAction = 0; // Standard: keine Bewegung
        int turnAction = 0; // Standard: keine Drehung

        if (Input.GetKey(KeyCode.W))
        {
            moveAction = 1; // vorwärts
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveAction = 2; // rückwärts
        }

        if (Input.GetKey(KeyCode.A))
        {
            turnAction = 1; // links drehen
        }

        if (Input.GetKey(KeyCode.D))
        {
            turnAction = 2; // rechts drehen
        }

        da[0] = moveAction + 3 * turnAction; // aus Bewegung und Drehung wieder Aktion 0-8 bauen
    }
}