# Installation Guide – RL Navigation Project

Dieser Guide beschreibt die Installation und Einrichtung des Projekts `rl_navigation_project`.

Das Projekt kombiniert:

- Unity
- Unity ML-Agents
- Python 3.9
- Stable-Baselines3
- Gymnasium / Gym
- Optuna
- Jupyter Notebooks
- PPO-Inference über ONNX-Modelle

Da das Projekt sowohl eine Unity- als auch eine Python-Komponente enthält, reicht eine reine `pip install`-Installation nicht aus.

---

## 1. Systemvoraussetzungen

Empfohlenes Setup:

- Windows 10 oder Windows 11
- Anaconda oder Miniconda
- Unity Hub
- Unity 2022.3.62f3
- Git(hub + Github Desktop)
- Python 3.9
- Visual Studio Code oder eine vergleichbare IDE

Im Repository ist aktuell folgende Unity-Version hinterlegt:

```text
Unity 2022.3.62f3
```

Während der Projektentwicklung wurde mit Unity 2022.3.62f3 gearbeitet. Falls Unity beim Öffnen des Projekts eine andere 2022.3-Version erkennt, sollte das Projekt in der Regel trotzdem kompatibel sein. Für maximale Reproduzierbarkeit sollte eine Unity 2022.3-LTS-Version verwendet werden.

---

## 2. Repository klonen

```bash
git clone https://github.com/DennisLess/rl_navigation_project.git
cd rl_navigation_project
```

---

## 3. Conda Environment erstellen

Für das Projekt wird Python 3.9 empfohlen (siehe `requirements.txt`)

```bash
conda create -n rl_project python=3.9
conda activate rl_project
```

Danach prüfen:

```bash
python --version
```

Erwartet wird eine Python-3.9-Version.

---

## 4. Python-Abhängigkeiten installieren

Die Python-Abhängigkeiten werden über die `requirements.txt` installiert:

```bash
python -m pip install -r requirements.txt
```

Falls es nach der Installation Probleme mit `numpy`, `protobuf` oder `onnx` gibt, können die kritischen Versionen erneut erzwungen werden:

```bash
python -m pip install --force-reinstall numpy==1.23.5 protobuf==3.20.3 onnx==1.13.1
```

Wichtig: Die Kombination aus ML-Agents, Stable-Baselines3, Gym/Gymnasium, ONNX und protobuf ist empfindlich. Deshalb sind mehrere Pakete bewusst gepinnt.

Eine genauere Erklärung befindet sich an den dazugehörigen Stellen in den jeweiligen Notebooks

---

## 5. Jupyter Kernel registrieren

Damit das Conda Environment in Jupyter auswählbar ist:

```bash
python -m ipykernel install --user --name rl_project --display-name "Python (rl_project)"
```

Danach kann in Jupyter oder VS Code der Kernel `Python (rl_project)` ausgewählt werden.

---

## 6. Unity installieren

1. Unity Hub öffnen.
2. Eine Unity 2022.3.62f3-Version installieren.
3. Empfohlen: Unity 2022.3.62f3.
4. Das Unity-Projekt im Ordner `MazeAgent` öffnen.

Projektpfad:

```text
rl_navigation_project/MazeAgent
```

---

## 7. Unity ML-Agents Package prüfen

Im Unity-Projekt sollte das ML-Agents-Package installiert sein.

Verwendetes Unity Package:

```text
com.unity.ml-agents 2.3.0-exp.3
```

Prüfen in Unity:

```text
Window → Package Manager → Packages: In Project
```

Falls das Package fehlt, muss es über den Package Manager bzw. über die Projektdateien nachinstalliert werden.

---

## 8. Python ML-Agents Version prüfen

Im aktivierten Conda Environment:

```bash
mlagents-learn --help
```

Wenn ML-Agents korrekt installiert ist, sollte der Hilfetext erscheinen.

Zusätzlich kann geprüft werden:

```bash
python -c "import mlagents; import mlagents_envs; print(mlagents.__version__); print(mlagents_envs.__version__)"
```

Erwartet:

```text
0.30.0
0.30.0
```

---

## 9. Unity-Python-Verbindung testen

Für A2C und DQN wird Unity aus Python heraus über ML-Agents angesprochen.

Allgemeiner Ablauf:

1. Notebook-Zelle in Python starten.
2. Python wartet auf Unity.
3. In Unity die passende Szene öffnen.
4. In Unity Play drücken.
5. Python verbindet sich mit der Unity-Umgebung.

Wichtig für A2C/DQN:

Beim `MazeAgent` in Unity:

```text
Behavior Type: Default
Model: None
```

Der Agent wird dann nicht über ein Unity-ONNX-Modell gesteuert, sondern durch Python.

---

## 10. PPO-Inference über ONNX

PPO wurde über Unity ML-Agents trainiert. Für die finale Evaluation wird PPO nicht über Stable-Baselines3 geladen, sondern direkt in Unity über ONNX-Inference ausgeführt.

Die ONNX-Modelle müssen im Unity-Assets-Ordner liegen:

```text
MazeAgent/Assets/Models
```

Beispiel:

```text
MazeAgent/Assets/Models/PPO_Env1_Seed1.onnx
```

Damit das Modell in Unity auswählbar ist:

1. ONNX-Datei nach `MazeAgent/Assets/Models` kopieren.
2. Unity fokussieren.
3. Warten, bis Unity das Modell importiert.
4. Beim `MazeAgent` einstellen:

```text
Behavior Parameters → Behavior Type: Inference Only
Behavior Parameters → Model: passendes PPO ONNX-Modell
```

---

## 11. Nachträglicher ONNX-Export für PPO

Falls nur `.pt`-Checkpoints vorhanden sind, können ONNX-Dateien nachträglich exportiert werden, genauere Erklärung sind im Notebook 06 aufgeführt.

Beispiel für Seed 1:

```bash
mlagents-learn training/configs/ppo_env1_config.yaml --run-id=PPO_Env1_Seed1 --resume --results-dir=training/logs/final
```

Dann in Unity Play drücken. ML-Agents exportiert anschließend ein ONNX-Modell, z. B.:

```text
training/logs/final/PPO_Env1_Seed1/MazeAgent.onnx
```

Danach kopieren:

```bash
mkdir -p MazeAgent/Assets/Models
cp training/logs/final/PPO_Env1_Seed1/MazeAgent.onnx MazeAgent/Assets/Models/PPO_Env1_Seed1.onnx
```

Analog für weitere Seeds:

```bash
mlagents-learn training/configs/ppo_env1_config.yaml --run-id=PPO_Env1_Seed27 --resume --results-dir=training/logs/final
cp training/logs/final/PPO_Env1_Seed27/MazeAgent.onnx MazeAgent/Assets/Models/PPO_Env1_Seed27.onnx
```

Beim ONNX-Export sollte in Unity Logging und Recording deaktiviert sein, damit keine zusätzlichen Evaluationsdateien erzeugt werden.

---

## 12. Notebooks ausführen

Die Notebooks liegen im Ordner:

```text
notebooks
```

Empfohlene Reihenfolge:

```text
00a_learning_programming_RL.ipynb (nur als Nootebook um die Kernkonzepte der RL-Programmierung zu erlernen / zuerklären, ist nicht Teil der finalen Evaluation (kann auch übersprungen werden))
01_environment_exploration.ipynb
02_random_baseline.ipynb
03_sb3_wrapper_test.ipynb
04_hyperparameter_tuning.ipynb
05_training_pipeline.ipynb
06_evaluation.ipynb
07_visualization.ipynb
```

Die wichtigsten finalen Notebooks sind:

```text
02_random_baseline.ipynb
05_training_pipeline.ipynb
06_evaluation.ipynb
07_visualization.ipynb
```

Notebook 06 erzeugt die finalen Evaluationstabellen.  
Notebook 07 erstellt die Visualisierungen und finalen Ergebnisübersichten.

---

## 13. Evaluation ausführen

### A2C und DQN

A2C und DQN werden über Python evaluiert.

Unity-Einstellungen:

```text
Behavior Type: Default
Model: None
EpisodeLogger: aktiv
Max Episodes: 50
```

Dann im Notebook die passende Evaluationszelle starten und in Unity Play drücken.

### PPO

PPO wird direkt in Unity evaluiert.

Unity-Einstellungen:

```text
Behavior Type: Inference Only
Model: passendes PPO ONNX-Modell
EpisodeLogger: aktiv
Max Episodes: 50
```

Die PPO-CSV-Dateien werden zunächst unter folgendem Ordner gespeichert:

```text
MazeAgent/training/evaluation_logs
```

Danach werden sie im Notebook normalisiert und nach folgendem Ordner kopiert:

```text
training/evaluation_results/raw
```

---

## 14. Random Baseline

Die Random Baseline wurde mit 200 Episoden pro Run erzeugt.

Für die finale Auswertung werden nur die ersten 50 Episoden je Run verwendet.

Außerdem wird der Reward nach der finalen Reward-Logik neu berechnet, da die ursprünglichen Random-Runs mit einer früheren Reward-Version erzeugt wurden.

Die Normalisierung erfolgt in Notebook 07.

---

## 15. Output-Ordner

Wichtige Output-Pfade:

```text
training/evaluation_results/raw
training/evaluation_results/tables
training/evaluation_results/figures
training/models/final
training/run_summaries
MazeAgent/training/evaluation_logs
```

Wichtige finale Dateien:

```text
training/evaluation_results/tables/evaluation_summary.csv
training/evaluation_results/tables/all_final_evaluations_combined.csv
training/evaluation_results/tables/compact_evaluation_results.csv
```

---

## 16. Häufige Probleme

### Problem: `Descriptors cannot be created directly`

Ursache: inkompatible protobuf-Version.

Fix:

```bash
python -m pip install --force-reinstall protobuf==3.20.3
```

---

### Problem: ONNX braucht eine andere protobuf-Version

Für dieses Projekt wurde folgende Kombination verwendet:

```text
onnx==1.13.1
protobuf==3.20.3
```

Fix:

```bash
python -m pip install --force-reinstall onnx==1.13.1 protobuf==3.20.3
```

Falls dabei NumPy verändert wird:

```bash
python -m pip install --force-reinstall numpy==1.23.5
```

---

### Problem: PPO-Modell erscheint nicht im Unity Model-Feld

Mögliche Ursachen:

- Die ONNX-Datei liegt nicht im Unity-Assets-Ordner.
- Unity hat die Datei noch nicht importiert.
- Das Modell liegt außerhalb von `MazeAgent/Assets`.

Fix:

```text
ONNX-Datei nach MazeAgent/Assets/Models kopieren
Unity fokussieren
Reimport ausführen oder kurz warten
```

---

### Problem: Python wartet auf Unity

Das ist normal.

Wenn im Terminal steht:

```text
Listening on port 5004. Start training by pressing the Play button in the Unity Editor.
```

Dann muss in Unity Play gedrückt werden.

---

### Problem: Unity verbindet sich nicht

Prüfen:

- richtige Szene geöffnet
- Unity ist im Play Mode
- kein altes Python-Training läuft noch
- Port 5004 ist nicht blockiert
- Behavior Type steht bei A2C/DQN auf `Default`
- Model ist bei A2C/DQN leer

---

## 17. Optional: LunarLander-Lernbeispiele

Einige frühe Lernnotebooks verwenden Gymnasium-Beispiele wie LunarLander.

Dafür können zusätzliche Pakete nötig sein:

```bash
python -m pip install "gymnasium[box2d]" swig
```

Diese Pakete sind nicht Teil der Kerninstallation, weil sie nicht für die finale Unity-Evaluation benötigt werden.

---

## 18. Reproduzierbarkeitshinweis

Das Projekt enthält mehrere technisch unterschiedliche Komponenten. Eine vollständige Reproduktion kann je nach System zusätzliche Anpassungen erfordern.

Besonders empfindlich sind:

- Unity-Version
- ML-Agents-Version
- PyTorch-Version
- NumPy-Version
- protobuf-Version
- ONNX-Version
- Gym/Gymnasium-Kompatibilität

Die dokumentierten Versionen beschreiben die verwendete Projektumgebung so genau wie möglich, ersetzen aber keine vollständig containerisierte Reproduktionsumgebung.