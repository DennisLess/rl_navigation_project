using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

// Klasse für den Agenten, der das Labyrinth durchqueren soll (erbt von MazeAgent)
public class MazeAgentScript : Agent
{
    // Header erstellt Überschrift im Inspector und 
    // public macht die Variablen im Inspector sichtbar, damit sie angepasst werden können ohne in den Code zu gehen
    [Header("Einstellungen")]
    public float moveSpeed = 3f; // Wie schnell sich der Agent (vorwärts / rückwärts) bewegen soll
    public float turnSpeed = 150f; // Wie schnell sich der Agent drehen soll (links / rechts) im Grad pro Sekunde

    [Header("Ziel")]
    public Transform goal; 

    private Rigidbody rb; // Referenz zum Rigidbody des Agenten, um die Physik zu steuern (Rigidbody = Komponente, die es einem Objekt ermöglicht, sich physikalisch korrekt zu bewegen)
    private Vector3 startPosition; // Startposition des Agenten; wird beim Reset gebraucht
    private Quaternion startRotation; // Startrotation des Agenten; wird beim Reset gebraucht

    [Header("Environment Index")]
    public int envIndex = 0; // 0 = Env1, 1 = Env2, 2 = Env3

    // Aufgerufen zum ertstmaligen Start des Agenten
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
    }

    public override void OnEpisodeBegin()
    {
        ScoreManager.Instance?.OnEpisodeStart(envIndex);
        transform.localPosition = startPosition;
        transform.localRotation = startRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveAction = actions.DiscreteActions[0]; // Aktion für Bewegung (0 = keine Bewegung, 1 = vorwärts, 2 = rückwärts)
        int turnAction = actions.DiscreteActions[1]; // Aktion für Drehung (0 = keine Drehung, 1 = links, 2 = rechts)

        if (moveAction == 1)
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime); // Vorwärtsbewegung
        if (moveAction == 2)
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime); // Rückwärtsbewegung
        if (turnAction == 1)
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime); // Drehung nach links (negative Richtung)
        if (turnAction == 2)
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime); // Drehung nach rechts (positive Richtung)

        AddReward(-0.001f); // Strafe für jede Aktion, um den Agenten zu motivieren, schneller zum Ziel zu kommen
        if (StepCount >= MaxStep - 1)
            ScoreManager.Instance?.OnTimeout(envIndex);
    }

    private void Update()
    {
        // Nur im Editor aktiv für manuellen Test
        #if UNITY_EDITOR
        if (Input.GetKey(KeyCode.W))
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S))
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.A))
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D))
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
        #endif
    }
    
    // Zielerkennung, um den Agenten zu belohnen, wenn er das Ziel erreicht
    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("Goal"))
        {
            AddReward(1.0f); // Belohnung für das Erreichen des Ziels
            ScoreManager.Instance?.OnGoalReached(envIndex); //Informiert Scoremanger über das Erreichen des Ziels
            EndEpisode(); // Beendet die aktuelle Episode, damit der Agent neu starten kann
        }
    }

    // Kollisionserkennung mit Wänden, um den Agenten zu bestrafen, wenn er mit einer Wand kollidiert
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.5f); // Strafe für das Zusammenstoßen mit einer Wand
            ScoreManager.Instance?.OnWallHit(envIndex); // Informiert den ScoreManager über das Treffen einer Wand

            EndEpisode(); // Beendet die aktuelle Episode, damit der Agent neu starten kann -> Treffen einer Wand wird bestraft
        }
    }


    // DAS FOLGENDE IST NUR FÜR TESTZWECKE UND NICHT FÜR DAS TRAINING DES AGENTEN NOTWENDIG!! 
    // manuelle Steuerung zum testen des Agenten soll überprüft werden -> stimmen die Phyics, funktioniert die Kollisionserkennung, etc.
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
