using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

// Random Agent als Baseline
// Wählt zufällige Aktionen und dient als untere Performance-Grenze
public class RandomAgentScript : Agent
{
    [Header("Einstellungen")]
    public float moveSpeed = 8f;      // Muss identisch zum trainierten Agenten sein
    public float turnSpeed = 150f;    // Muss identisch zum trainierten Agenten sein

    [Header("Ziel")]
    public Transform goal;

    [Header("Spawn & Goal Punkte")]
    public Transform[] spawnPoints;
    public Transform[] goalPoints;

    private Rigidbody rb;
    private int lastSpawnIndex = -1;
    private int lastGoalIndex = -1;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        ScoreManager.Instance?.OnEpisodeStart();

        // Zufälligen Spawnpunkt wählen, möglichst nicht denselben wie vorher
        int spawnIndex;

        do
        {
            spawnIndex = Random.Range(0, spawnPoints.Length);
        }
        while (spawnIndex == lastSpawnIndex && spawnPoints.Length > 1);

        lastSpawnIndex = spawnIndex;

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

        goal.position = goalPoints[goalIndex].position;

        // Physik zurücksetzen
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Gleiche Beobachtungen wie beim trainierten Agenten
        // Der Random Agent nutzt sie nicht aktiv, aber die Environment-Spezifikation bleibt identisch
        Vector3 toGoal = goal.position - transform.position;

        sensor.AddObservation(transform.InverseTransformDirection(toGoal.normalized));
        sensor.AddObservation(toGoal.magnitude / 20f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Eingehende Actions werden ignoriert.
        // Stattdessen wird zufällig eine der 9 möglichen Aktionen gewählt.
        int action = Random.Range(0, 9);

        // Mapping:
        // moveAction: 0 = nichts, 1 = vorwärts, 2 = rückwärts
        // turnAction: 0 = nichts, 1 = links, 2 = rechts
        int moveAction = action % 3;
        int turnAction = action / 3;

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

        // Gleiche Schrittstrafe wie beim trainierten Agenten
        AddReward(-0.001f);

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
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.5f);

            ScoreManager.Instance?.OnWallHit();

            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Der ActionBuffer wird nicht genutzt, weil OnActionReceived selbst randomisiert.
        // Trotzdem setzen wir einen gültigen Wert, damit Heuristic Only sauber läuft.
        var da = actionsOut.DiscreteActions;

        da[0] = 0;
    }
}