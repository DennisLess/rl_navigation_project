using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

// Klasse für den Agenten, der das Labyrinth durchqueren soll
public class MazeAgentScript : Agent
{
    [Header("Einstellungen")]
    public float moveSpeed = 3f;      // Geschwindigkeit für Vorwärts- und Rückwärtsbewegung
    public float turnSpeed = 150f;    // Drehgeschwindigkeit in Grad pro Sekunde

    [Header("Ziel")]
    public Transform goal;            // Ziel-Objekt, wird im Inspector zugewiesen

    [Header("Spawn & Goal Punkte")]
    public Transform[] spawnPoints;   // mögliche Spawnpunkte für den Agenten
    public Transform[] goalPoints;    // mögliche Positionen für das Ziel

    private Rigidbody rb;             // Rigidbody des Agenten
    private int lastSpawnIndex = -1;  // letzter Spawnpunkt, um Wiederholungen zu vermeiden
    private int lastGoalIndex = -1;   // letzter Goalpunkt, um Wiederholungen zu vermeiden

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // ScoreManager über neue Episode informieren
        ScoreManager.Instance?.OnEpisodeStart();

        // Zufälligen Spawnpunkt wählen, möglichst nicht denselben wie vorher
        int spawnIndex;

        do
        {
            spawnIndex = Random.Range(0, spawnPoints.Length);
        }
        while (spawnIndex == lastSpawnIndex && spawnPoints.Length > 1);

        lastSpawnIndex = spawnIndex;

        // Agent an Spawnpunkt setzen
        transform.position = spawnPoints[spawnIndex].position;
        transform.rotation = spawnPoints[spawnIndex].rotation;

        // Zufälligen Goalpunkt wählen, möglichst nicht denselben wie vorher
        int goalIndex;

        do
        {
            goalIndex = Random.Range(0, goalPoints.Length);
        }
        while (goalIndex == lastGoalIndex && goalPoints.Length > 1);

        lastGoalIndex = goalIndex;

        // Ziel an Goalpunkt setzen
        goal.position = goalPoints[goalIndex].position;

        // Physik zurücksetzen, damit der Agent nicht mit alter Bewegung startet
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Richtung vom Agenten zum Ziel
        Vector3 toGoal = goal.position - transform.position;

        // Zielrichtung relativ zur Ausrichtung des Agenten
        sensor.AddObservation(transform.InverseTransformDirection(toGoal.normalized));

        // Distanz zum Ziel, grob normalisiert
        sensor.AddObservation(toGoal.magnitude / 20f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveAction = actions.DiscreteActions[0]; // 0 = nichts, 1 = vorwärts, 2 = rückwärts
        int turnAction = actions.DiscreteActions[1]; // 0 = nichts, 1 = links, 2 = rechts

        // Bewegung relativ zur aktuellen Ausrichtung des Agenten
        if (moveAction == 1)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime, Space.Self);
        }

        if (moveAction == 2)
        {
            transform.Translate(Vector3.back * moveSpeed * Time.fixedDeltaTime, Space.Self);
        }

        if (turnAction == 1)
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.fixedDeltaTime, Space.Self);
        }

        if (turnAction == 2)
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.fixedDeltaTime, Space.Self);
        }

        // Kleine Strafe pro Schritt, damit der Agent effiziente Wege lernt
        AddReward(-0.001f);

        // Timeout zählen, wenn MaxStep fast erreicht ist
        if (StepCount == MaxStep - 1)
        {
            ScoreManager.Instance?.OnTimeout();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            AddReward(5.0f);

            ScoreManager.Instance?.OnGoalReached();

            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Nur Wände beenden die Episode
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.5f);

            ScoreManager.Instance?.OnWallHit();

            EndEpisode();
        }
    }

    // Manuelle Steuerung nur zum Testen mit Behavior Type = Heuristic Only
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var da = actionsOut.DiscreteActions;

        da[0] = 0;
        da[1] = 0;

        if (Input.GetKey(KeyCode.W))
        {
            da[0] = 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            da[0] = 2;
        }

        if (Input.GetKey(KeyCode.A))
        {
            da[1] = 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            da[1] = 2;
        }
    }
}