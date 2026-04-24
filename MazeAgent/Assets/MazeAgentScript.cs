using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

// Klasse für den Agenten, der das Labyrinth durchqueren soll (erbt von Agent)
public class MazeAgentScript : Agent
{
    // Header erstellt Überschrift im Inspector und 
    // public macht die Variablen im Inspector sichtbar, damit sie angepasst werden können ohne in den Code zu gehen
    [Header("Einstellungen")]
    public float moveSpeed = 3f; // Wie schnell sich der Agent (vorwärts / rückwärts) bewegen soll
    public float turnSpeed = 150f; // Wie schnell sich der Agent drehen soll (links / rechts) im Grad pro Sekunde

    [Header("Ziel")]
    public Transform goal; // Das Ziel-Objekt (grüne Kugel) – wird im Inspector per Drag & Drop zugewiesen

    [Header("Spawn & Goal Punkte")]
    public Transform[] spawnPoints; // Array aller Spawnpunkte – werden im Inspector zugewiesen
    public Transform[] goalPoints;  // Array aller Goalpunkte – werden im Inspector zugewiesen

    [Header("Environment Index")]
    public int envIndex = 0; // 0 = Env1, 1 = Env2, 2 = Env3

    private Rigidbody rb; // Referenz zum Rigidbody des Agenten, um die Physik zu steuern (Rigidbody = Komponente, die es einem Objekt ermöglicht, sich physikalisch korrekt zu bewegen)
    private int lastSpawnIndex = -1; // Speichert den letzten Spawnpunkt um Wiederholungen zu vermeiden
    private int lastGoalIndex  = -1; // Speichert den letzten Goalpunkt um Wiederholungen zu vermeiden

    // Aufgerufen zum erstmaligen Start des Agenten
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // ScoreManager über neue Episode informieren
        ScoreManager.Instance?.OnEpisodeStart(envIndex);

        // Zufälligen Spawnpunkt wählen – nicht gleich wie der letzte damit der Agent
        // nicht immer von derselben Position startet (verhindert Overfitting auf eine Route)
        int spawnIndex;
        do {
            spawnIndex = Random.Range(0, spawnPoints.Length);
        } while (spawnIndex == lastSpawnIndex && spawnPoints.Length > 1);
        lastSpawnIndex = spawnIndex;

        // Agent an den gewählten Spawnpunkt teleportieren
        transform.position = spawnPoints[spawnIndex].position;
        transform.rotation = spawnPoints[spawnIndex].rotation;

        // Zufälligen Goalpunkt wählen – nicht gleich wie der letzte damit das Ziel
        // nicht immer an derselben Stelle steht (erhöht Generalisierung)
        int goalIndex;
        do {
            goalIndex = Random.Range(0, goalPoints.Length);
        } while (goalIndex == lastGoalIndex && goalPoints.Length > 1);
        lastGoalIndex = goalIndex;

        // Ziel an den gewählten Goalpunkt verschieben
        goal.position = goalPoints[goalIndex].position;

        // Physik-Bewegung komplett stoppen damit der Agent nicht mit alter
        // Geschwindigkeit in die neue Episode startet
        rb.velocity        = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // Beobachtungen sammeln – gibt dem Agenten zusätzliche Informationen über die Umgebung
    // Der RayPerceptionSensor gibt Wandinformationen, CollectObservations gibt Zielinformationen
    public override void CollectObservations(VectorSensor sensor)
    {
        // Richtung zum Ziel relativ zur Agenten-Ausrichtung (3 Werte: x, y, z)
        Vector3 toGoal = goal.position - transform.position;
        sensor.AddObservation(transform.InverseTransformDirection(toGoal.normalized));

        // Distanz zum Ziel normalisiert auf 0-1 (1 Wert)
        // Division durch 20 weil das Maze ca. 20 Einheiten groß ist
        sensor.AddObservation(toGoal.magnitude / 20f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveAction = actions.DiscreteActions[0]; // Aktion für Bewegung (0 = keine Bewegung, 1 = vorwärts, 2 = rückwärts)
        int turnAction = actions.DiscreteActions[1]; // Aktion für Drehung (0 = keine Drehung, 1 = links, 2 = rechts)

        // Space.Self stellt sicher dass die Bewegung relativ zur Agenten-Ausrichtung ist
        if (moveAction == 1)
            transform.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime, Space.Self); // Vorwärtsbewegung
        if (moveAction == 2)
            transform.Translate(Vector3.back * moveSpeed * Time.fixedDeltaTime, Space.Self); // Rückwärtsbewegung
        if (turnAction == 1)
            transform.Rotate(Vector3.up, -turnSpeed * Time.fixedDeltaTime, Space.Self); // Drehung nach links (negative Richtung)
        if (turnAction == 2)
            transform.Rotate(Vector3.up, turnSpeed * Time.fixedDeltaTime, Space.Self); // Drehung nach rechts (positive Richtung)

        AddReward(-0.001f); // Strafe für jede Aktion, um den Agenten zu motivieren, schneller zum Ziel zu kommen

        // Timeout tracken – wenn Max Steps erreicht wird die Episode als Timeout gezählt
        // == statt >= damit Timeout nur einmal gezählt wird
        if (StepCount == MaxStep - 1)
            ScoreManager.Instance?.OnTimeout(envIndex);
    }

    // Zielerkennung, um den Agenten zu belohnen, wenn er das Ziel erreicht
    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("Goal"))
        {
            AddReward(5.0f); // Belohnung für das Erreichen des Ziels
            ScoreManager.Instance?.OnGoalReached(envIndex); // Informiert ScoreManager über das Erreichen des Ziels
            EndEpisode(); // Beendet die aktuelle Episode, damit der Agent neu starten kann
        }
    }

    // Kollisionserkennung mit Wänden, um den Agenten zu bestrafen, wenn er mit einer Wand kollidiert
    private void OnCollisionEnter(Collision collision)
    {
        // Nur Wände beenden die Episode – Boden und andere Objekte werden ignoriert
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.5f); // Strafe für das Zusammenstoßen mit einer Wand
            ScoreManager.Instance?.OnWallHit(envIndex); // Informiert den ScoreManager über das Treffen einer Wand
            EndEpisode(); // Beendet die aktuelle Episode, damit der Agent neu starten kann -> Treffen einer Wand wird bestraft
        }
    }

    // DAS FOLGENDE IST NUR FÜR TESTZWECKE UND NICHT FÜR DAS TRAINING DES AGENTEN NOTWENDIG!! 
    // manuelle Steuerung zum testen des Agenten soll überprüft werden -> stimmen die Physics, funktioniert die Kollisionserkennung, etc.
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var da = actionsOut.DiscreteActions;
        da[0] = 0; da[1] = 0; // Standardaktionen (keine Bewegung, keine Drehung)
        if (Input.GetKey(KeyCode.W))
            da[0] = 1; // Vorwärtsbewegung
        if (Input.GetKey(KeyCode.S))
            da[0] = 2; // Rückwärtsbewegung
        if (Input.GetKey(KeyCode.A))
            da[1] = 1; // Drehung nach links
        if (Input.GetKey(KeyCode.D))
            da[1] = 2; // Drehung nach rechts
    }
}