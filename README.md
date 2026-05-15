# RL Navigation Project – Reinforcement Learning in Unity

Dieses Repository enthält ein Reinforcement-Learning-Projekt zur Navigation eines Agenten in selbst erstellten Unity-Environments.

Ziel des Projekts war es, verschiedene Reinforcement-Learning-Algorithmen auf einer Unity-basierten Navigationsaufgabe zu trainieren, zu evaluieren und kritisch miteinander zu vergleichen. Im Fokus standen dabei nicht nur die finalen Erfolgsraten, sondern auch typische praktische Herausforderungen von Reinforcement Learning, z. B. Reward Engineering, Seed-Sensitivität, Sparse Rewards, Lazy-Agent-Verhalten, Frozen-Agent-Verhalten und Transferfähigkeit auf neue Environments.

---

## Kurzfassung

In diesem Projekt wurden Random, A2C, DQN und PPO auf einer selbst entwickelten Unity-Navigationsaufgabe verglichen. Das finale Setup konnte keine robuste Zielnavigation über alle Environments hinweg zeigen. A2C und PPO erreichten in der finalen Evaluation keine Goals. DQN zeigte als einziger Algorithmus nennenswerte Zielerreichung, allerdings nur in einer spezifischen Seed- und Modus-Kombination: DQN Seed 27 im stochastischen Modus erreichte 16 % Goal Rate in Env1 und 12 % in Env3.

Der wichtigste Beitrag des Projekts liegt daher nicht in einem vollständig gelösten Navigationsproblem, sondern in der Analyse praktischer RL-Herausforderungen: Sparse Rewards, Reward Engineering, Seed-Sensitivität, Lazy-Agent-Verhalten, Frozen-Agent-Verhalten, Evaluationsmodus und eingeschränkte Transferfähigkeit.

---

## 1. Projektüberblick

In diesem Projekt wurde ein eigener MazeAgent in Unity umgesetzt. Der Agent soll sich durch verschiedene Umgebungen bewegen und ein Zielobjekt erreichen, ohne gegen Wände zu laufen.

Verglichen wurden:

- Random Baseline
- A2C
- DQN
- PPO

Die Algorithmen wurden nicht alle über dieselbe technische Pipeline umgesetzt:

- A2C und DQN wurden über Stable-Baselines3 trainiert und evaluiert.
- PPO wurde über Unity ML-Agents trainiert und über ONNX-Inference in Unity evaluiert.
- Random wurde direkt in Unity über ein eigenes Random-Agent-Script evaluiert.

Das Projekt kombiniert daher Python, Stable-Baselines3, Unity, ML-Agents, Jupyter Notebooks und eigene Logging-/Evaluation-Skripte.

---

## 2. Forschungsfrage und Hypothesen

Die übergeordnete Forschungsfrage des Projekts lautet:

> Wie unterscheiden sich A2C, DQN und PPO bei der Navigation eines Agenten in selbst entwickelten Unity-Environments hinsichtlich Zielerreichung, Fehlerverhalten und Transferfähigkeit?

Diese Forschungsfrage wurde gewählt, weil Reinforcement Learning zunehmend in simulierten Umgebungen eingesetzt wird, z. B. für Robotik, autonome Navigation, Logistik- und Warehouse-Simulationen. Gleichzeitig zeigen praktische RL-Projekte häufig, dass ein theoretisch passender Algorithmus nicht automatisch zu robustem Verhalten führt. Besonders Reward Engineering, Seed-Sensitivität, Sparse Rewards und Transferfähigkeit sind zentrale Herausforderungen.

Aus der Hauptforschungsfrage ergeben sich folgende Unterforschungsfragen:

- Lernen A2C, DQN und PPO zielgerichtete Navigation in Env1?
- Welcher Algorithmus erreicht die höchste Goal Rate in der primären Trainingsumgebung?
- Wie unterscheiden sich die Algorithmen hinsichtlich Mean Reward, Timeout Rate und Wall-like Failure Rate?
- Wie stark hängen die Ergebnisse vom gewählten Seed ab?
- Welchen Einfluss hat der Evaluationsmodus bei A2C und DQN, also deterministic vs. stochastic?
- Können die besten in Env1 trainierten Modelle auf Env2 und Env3 übertragen werden?
- Welche praktischen RL-Probleme werden im Experiment sichtbar, z. B. Lazy-Agent-Verhalten, Frozen-Agent-Verhalten oder Sparse-Reward-Probleme?

### Hypothesen

Für die empirische Analyse wurden folgende Hypothesen formuliert:

| Hypothese | Formulierung |
|---|---|
| H1 | Trainierte RL-Agenten erreichen eine höhere Goal Rate als die Random Baseline. |
| H2 | DQN erzielt aufgrund seiner wertbasierten Struktur in der diskreten Action-Space-Umgebung bessere Ergebnisse als A2C. |
| H3 | PPO zeigt im Unity-ML-Agents-Setup stabileres Verhalten als A2C und DQN, da PPO direkt in Unity trainiert wird. |
| H4 | Die Performance der Modelle ist seedabhängig, sodass einzelne Seeds deutlich bessere oder schlechtere Ergebnisse liefern können. |
| H5 | Stochastische Evaluation kann bei A2C und DQN andere Ergebnisse liefern als deterministische Evaluation, da Exploration bzw. probabilistische Action-Auswahl weiterhin Einfluss auf das Verhalten haben kann. |
| H6 | Ein in Env1 trainiertes Modell generalisiert nur eingeschränkt auf Env2 und Env3, da sich die Layouts und Navigationsanforderungen unterscheiden. |

Die Hypothesen werden in den späteren Notebooks empirisch anhand von Goal Rate, Mean Reward, Timeout Rate, Wall-like Failure Rate und Mean Steps überprüft. Eine ausführlichere theoretische Herleitung und Einordnung erfolgt in der schriftlichen Ausarbeitung.

---

## 3. Methodischer Bezug zur Aufgabenstellung

Die Aufgabenstellung fordert eine klare Forschungsfrage, überprüfbare Hypothesen, eine theoretische Einordnung der verwendeten Methoden, eine praktische Implementierung mehrerer Machine-Learning-Modelle sowie eine Evaluation.

Dieses Projekt überträgt diese Anforderungen auf ein Reinforcement-Learning-Setting:

| Anforderung der Aufgabenstellung | Umsetzung im Projekt |
|---|---|
| Forschungsfrage | Vergleich von A2C, DQN und PPO für Unity-basierte Navigation |
| Hypothesen | Formulierung von sechs überprüfbaren Hypothesen zu Goal Rate, Seed-Sensitivität, Evaluationsmodus und Transferfähigkeit |
| Datengrundlage | Kein externer Datensatz; die Trainings- und Evaluationsdaten werden vollständig durch die selbst entwickelten Unity-Environments erzeugt |
| Mehrere Modelle | Random Baseline, A2C, DQN und PPO |
| Hyperparameter-Tuning | Optuna-Tuning als explorativer Schritt; anschließend manuelle Modellkonfiguration, da Optuna keine ausreichend überzeugenden Modelle lieferte |
| Evaluation | Vergleich anhand von Goal Rate, Mean Reward, Timeout Rate, Wall-like Failure Rate und Mean Steps |
| Praxisbezug | Navigation in simulierten Maze- und Warehouse-Umgebungen als vereinfachtes Beispiel für autonome Agenten und Logistik-/Robotik-Simulationen |
| Kritische Reflexion | Diskussion von Reward Engineering, Sparse Rewards, Lazy-Agent-Verhalten, Frozen-Agent-Verhalten, Seed-Sensitivität und Transferfähigkeit |

Da es sich um ein Reinforcement-Learning-Projekt handelt, wird kein statischer öffentlicher Datensatz verwendet. Stattdessen entstehen die Daten dynamisch durch Interaktion zwischen Agent und Unity-Environment. Die Evaluation basiert daher auf generierten Episodenlogs, Rewards, Erfolgsraten und Fehlerereignissen.

---

## 4. Verwendete Environments

Im Unity-Projekt wurden drei Environments erstellt.

| Environment | Unity Scene | Rolle im Projekt | Beschreibung |
|---|---|---|---|
| Env1 | `MazeScene.unity` | Trainings- und Hauptvergleichsumgebung | Einfacheres Maze zur grundlegenden Navigation |
| Env2 | `MazeScene_Env2.unity` | Transferumgebung | Komplexere Maze-Struktur zur Prüfung von Generalisierung |
| Env3 | `MazeScene_Env3.unity` | Transferumgebung | Warehouse-artige Umgebung als praxisnähere Transferaufgabe |

Env1 wurde als zentrale Trainingsumgebung verwendet. Env2 und Env3 dienten zur Transfer-Evaluation der besten Modelle.

---

## 5. Agent und Aufgabe

Der Agent bewegt sich in einer Unity-Szene und soll das Zielobjekt erreichen.

Wichtige Objekte:

| Objekt | Funktion |
|---|---|
| Agent | Lernt die Navigation durch das Environment |
| Goal | Zielobjekt, das erreicht werden soll |
| Walls | Hindernisse, die vermieden werden müssen |
| Spawn Points | Startpositionen für Episoden |
| Floor | Bewegungsfläche des Agenten |

Eine Episode kann durch folgende Ereignisse enden:

- Goal erreicht
- Wall-Hit
- Timeout nach maximaler Schrittzahl

---

## 6. Action Space

Der Agent nutzt einen diskreten Action Space mit 9 Aktionen.

| Action Index | Bedeutung |
|---:|---|
| 0 | No-op |
| 1 | Forward |
| 2 | Backward |
| 3 | Turn Left |
| 4 | Forward + Turn Left |
| 5 | Backward + Turn Left |
| 6 | Turn Right |
| 7 | Forward + Turn Right |
| 8 | Backward + Turn Right |

Diese Aktionsstruktur wurde gewählt, weil sich der Agent nicht wie in einer klassischen Grid World feldweise bewegen soll. Stattdessen bewegt er sich in Unity kontinuierlicher durch den Raum, während die Entscheidungsstruktur für die Algorithmen diskret bleibt.

---

## 7. Observation Space

Die Python-Evaluation zeigte für die Unity-Umgebung einen Observation Space der Form:

```text
Box(-inf, inf, (48,), float32)
```

Die Beobachtungsstruktur wurde über die Experimente hinweg konstant gehalten, damit A2C, DQN und PPO auf derselben Navigationsaufgabe arbeiten.

---

## 8. Reward Design

Die finale Reward-Struktur ergab sich aus einem Event Reward und einer laufenden Step Penalty:

```text
Step Penalty: -0.0005 pro Schritt
Goal Event:   +10
Wall Event:   -5
Timeout:      kein zusätzlicher Event Reward
```

Ziel des Reward Designs war es, den Agenten dazu zu bringen:

- das Goal möglichst schnell zu erreichen,
- Wall-Hits zu vermeiden,
- nicht unnötig lange im Environment zu bleiben.

Eine zentrale Schwierigkeit war das Sparse-Reward-Problem. Der wichtigste positive Reward entsteht erst beim Erreichen des Goals. Gerade zu Beginn des Trainings passiert das selten. Dadurch erhält der Agent nur wenige positive Lernsignale und lernt überwiegend aus negativen Signalen wie Wall-Hits, Timeouts und Step Penalty.

---

## 9. Algorithmen

### Random Baseline

Die Random Baseline wurde als untere Vergleichsgröße genutzt. Der Agent wählt zufällige Aktionen und dient damit als Referenzpunkt für nicht gelerntes Verhalten.

### A2C

A2C wurde über Stable-Baselines3 trainiert. Aufgrund sehr schwacher früher Ergebnisse wurden für A2C nur drei Seeds trainiert und evaluiert. Diese Entscheidung reduziert den Rechenaufwand, ist aber methodisch eine Limitation, da nicht ausgeschlossen werden kann, dass weitere Seeds andere Ergebnisse geliefert hätten.

### DQN

DQN wurde ebenfalls über Stable-Baselines3 trainiert. DQN zeigte die interessantesten, aber auch instabilsten Ergebnisse. Besonders DQN Seed 27 im stochastischen Modus erreichte als einziges Modell nennenswerte Goals.

### PPO

PPO wurde über Unity ML-Agents trainiert. Für die finale Evaluation wurden die PPO-Modelle nachträglich als ONNX-Dateien exportiert und direkt in Unity über Inference evaluiert.

---

## 10. Evaluationsdesign

Die finale Evaluation wurde auf 50 Episoden pro Run gesetzt.

A2C und DQN wurden in Env1 jeweils in zwei Modi evaluiert:

- deterministic
- stochastic

PPO wurde über Unity ML-Agents Inference evaluiert.

Die Random Baseline wurde ursprünglich mit 200 Episoden pro Run erzeugt. Für die finale Vergleichbarkeit wurden nur die ersten 50 Episoden verwendet. Da die Random Baseline mit einer früheren Reward-Logik erzeugt wurde, wurden die Rewards nachträglich nach der finalen Reward-Logik neu berechnet.

Für Env2 und Env3 wurde keine vollständige Evaluation aller Seeds und Modi durchgeführt. Stattdessen wurden die besten Modelle aus Env1 auf Env2 und Env3 übertragen.

---

## 11. Wichtigste finale Ergebnisse

Die finale Evaluation zeigte keine robuste Zielnavigation über alle Algorithmen und Environments hinweg.

### Random Baseline

Die Random Baseline erreichte fast keine Goals. In den meisten Runs endete sie vollständig in Wall-like Failures. Damit bestätigt sie, dass zufälliges Verhalten keine stabile Navigation erzeugt.

### A2C

A2C erreichte in keiner evaluierten Konfiguration ein Goal.

- Env1: 0 % Goal Rate in allen getesteten Seeds und Modi
- Env2/Env3: keine erfolgreiche Zielnavigation
- stochastische A2C-Runs führten häufig zu Wall-like Failures
- deterministische A2C-Runs führten häufig zu Timeouts

### PPO

PPO erreichte ebenfalls keine Goals.

Besonders PPO Seed 1 und PPO Seed 100 liefen in Env1 vollständig bis zum Timeout, ohne Goals oder Wall-Hits zu erzeugen. Dieses Verhalten kann als Lazy- oder Survival-Verhalten interpretiert werden: Der Agent vermeidet negative Ereignisse, erreicht aber auch nicht das Ziel.

### DQN

DQN zeigte als einziger Algorithmus nennenswerte Zielerreichung.

Der wichtigste positive Befund war:

| Modell | Env1 Goal Rate | Env2 Goal Rate | Env3 Goal Rate |
|---|---:|---:|---:|
| DQN Seed 27 stochastic | 16 % | 0 % | 12 % |

DQN Seed 27 im stochastischen Modus war damit das stärkste Modell. Gleichzeitig war das Verhalten nicht robust, da viele Episoden weiterhin in Wall-like Failures endeten und Env2 vollständig scheiterte.

---

## 12. Hypothesenbewertung

Die Hypothesen wurden anhand der finalen Evaluationsergebnisse bewertet.

| Hypothese | Ergebnis | Bewertung |
|---|---|---|
| H1 | Trainierte RL-Agenten erreichen eine höhere Goal Rate als Random. | Teilweise bestätigt. Nur DQN Seed 27 stochastic übertraf Random klar; A2C und PPO erreichten keine Goals. |
| H2 | DQN erzielt bessere Ergebnisse als A2C. | Bestätigt für die getesteten Seeds. A2C erreichte keine Goals, während DQN Seed 27 stochastic Goals erreichte. |
| H3 | PPO zeigt stabileres Verhalten durch natives Unity-Training. | Nicht im eigentlichen Sinne bestätigt. PPO zeigte zwar teilweise stabiles Survival-Verhalten ohne Wall-Hits, erreichte aber keine Goals und konvergierte damit nicht zu erfolgreicher Navigation. |
| H4 | Die Performance ist seedabhängig. | Bestätigt. Besonders DQN zeigte starke Unterschiede zwischen den Seeds. |
| H5 | Stochastic und deterministic Evaluation unterscheiden sich. | Bestätigt. DQN Seed 27 erreichte nur im stochastischen Modus Goals. |
| H6 | Transfer auf Env2 und Env3 ist eingeschränkt. | Bestätigt. Die Best Models generalisierten nicht robust; DQN Seed 27 stochastic erreichte Env3 teilweise, scheiterte aber in Env2. |

---

## 13. Zentrale Erkenntnisse

Die wichtigsten Erkenntnisse des Projekts sind:

1. Es konnte keine robuste Zielnavigation über alle Algorithmen und Environments nachgewiesen werden.
2. A2C und PPO erreichten in der finalen Evaluation keine Goals.
3. DQN zeigte als einziger Algorithmus zielgerichtetes Verhalten, aber nur in einer spezifischen Seed- und Modus-Kombination.
4. DQN war stark seed- und evaluationsmodusabhängig.
5. Die stochastische Evaluation war bei DQN entscheidend, da die deterministische Policy keine Goals erreichte.
6. PPO zeigte Lazy-/Survival-Verhalten.
7. Die Transferfähigkeit war eingeschränkt.
8. Reward Engineering und Sparse Rewards waren zentrale Herausforderungen.
9. Das Projekt zeigt eher eine kritische Analyse praktischer RL-Probleme als ein vollständig gelöstes Navigationssystem.

---

## 14. Projektstruktur

Auszug aus der wichtigsten Projektstruktur:

```text
rl_navigation_project/
│
├── MazeAgent/
│   ├── Assets/
│   │   ├── Models/
│   │   │   ├── PPO_Env1_Seed1.onnx
│   │   │   ├── PPO_Env1_Seed27.onnx
│   │   │   ├── PPO_Env1_Seed42.onnx
│   │   │   ├── PPO_Env1_Seed72.onnx
│   │   │   └── PPO_Env1_Seed100.onnx
│   │   │
│   │   ├── Scenes/
│   │   │   ├── MazeScene.unity
│   │   │   ├── MazeScene_Env2.unity
│   │   │   └── MazeScene_Env3.unity
│   │   │
│   │   ├── EpisodeLogger.cs
│   │   ├── MazeAgentScript.cs
│   │   ├── RandomAgentScript.cs
│   │   ├── ScoreManager.cs
│   │   └── SeedManager.cs
│   │
│   ├── Packages/
│   ├── ProjectSettings/
│   └── training/evaluation_logs/
│
├── docs/
│   ├── images/
│   └── evaluation/tables/
│
├── lib_py/
│   └── paths.py
│
├── notebooks/
│   ├── 01_environment_exploration.ipynb
│   ├── 02_random_baseline.ipynb
│   ├── 03_sb3_wrapper_test.ipynb
│   ├── 04_hyperparameter_tuning.ipynb
│   ├── 05_training_pipeline.ipynb
│   ├── 06_evaluation.ipynb
│   └── 07_visualization.ipynb
│
├── training/
│   ├── configs/
│   ├── evaluation_results/
│   │   ├── raw/
│   │   ├── tables/
│   │   └── figures/
│   ├── logs/
│   ├── models/
│   ├── run_summaries/
│   └── tuning/
│
├── .gitignore
├── Installation.md
├── README.md
└── requirements.txt
```

---

## 15. Notebooks

Die finale Projektpipeline ist in mehreren Notebooks dokumentiert.

| Notebook | Inhalt |
|---|---|
| `01_environment_exploration.ipynb` | Beschreibung der Unity-Environments, Action Space, Observation Space und Reward Design |
| `02_random_baseline.ipynb` | Erstellung und Analyse der Random Baseline |
| `03_sb3_wrapper_test.ipynb` | Test der Unity-SB3-Verbindung |
| `04_hyperparameter_tuning.ipynb` | Optuna-Tuning und manuelle Hyperparameter-Entscheidungen |
| `05_training_pipeline.ipynb` | Training der finalen A2C-, DQN- und PPO-Modelle |
| `06_evaluation.ipynb` | Finale Evaluation, Normalisierung der Ergebnisse, Best-Model-Auswahl |
| `07_visualization.ipynb` | Visualisierung der finalen Evaluationsergebnisse |

Hinweis: Das Notebook `00a_learning_programming_RL.ipynb` war ein persönliches Lernnotebook zur Vorbereitung und ist nicht Teil der finalen öffentlichen Projektpipeline.

---

## 16. Training und Evaluation

### A2C und DQN

A2C und DQN wurden über Stable-Baselines3 trainiert und aus Python heraus mit der Unity-Umgebung verbunden.

Die finalen Modelle liegen unter:

```text
training/models/final/
```

Die Run Summaries liegen unter:

```text
training/run_summaries/
```

### PPO

PPO wurde über Unity ML-Agents trainiert. Die finalen ONNX-Modelle liegen unter:

```text
MazeAgent/Assets/Models/
```

### Evaluationsergebnisse

Die finalen Evaluationsergebnisse liegen unter:

```text
training/evaluation_results/
```

Wichtige Dateien:

```text
training/evaluation_results/tables/evaluation_summary.csv
training/evaluation_results/tables/all_final_evaluations_combined.csv
training/evaluation_results/tables/compact_evaluation_results.csv
```

---

## 17. Installation

Die Installation ist in einer eigenen Datei beschrieben:

```text
Installation.md
```

Kurzfassung:

```bash
conda create -n rl_project python=3.9
conda activate rl_project
python -m pip install -r requirements.txt
```

Zusätzlich muss Unity installiert und das Unity-Projekt im Ordner `MazeAgent` geöffnet werden.

Verwendete Unity-Version:

```text
Unity 2022.3.62f3
```

Weitere Details stehen in `Installation.md`.

---

## 18. Reproduktion der Ergebnisse

Die wichtigsten Schritte zur Reproduktion sind:

1. Python-Environment nach `requirements.txt` einrichten.
2. Unity-Projekt `MazeAgent` mit Unity 2022.3.62f3 öffnen.
3. Notebooks in Reihenfolge ausführen.
4. Für A2C/DQN Unity im `Default` Behavior Type starten.
5. Für PPO ONNX-Modell in Unity auswählen und Inference starten.
6. Notebook 06 für finale Evaluation ausführen.
7. Notebook 07 für Visualisierungen ausführen.

Wichtig: Eine vollständige Reproduktion kann je nach System zusätzliche Anpassungen erfordern, da Unity, ML-Agents, PyTorch, ONNX, protobuf und Gym/Gymnasium empfindlich auf Versionsunterschiede reagieren.

---

## 19. Limitationen

Die wichtigsten Limitationen des Projekts sind:

- finale Evaluation nur mit 50 Episoden pro Run
- Random Baseline nachträglich auf 50 Episoden gekürzt
- Random Rewards nach finaler Reward-Logik neu berechnet
- A2C nur mit drei Seeds trainiert
- starke Seed-Sensitivität, besonders bei DQN
- Optuna-Tuning führte nicht zu ausreichend überzeugenden Modellen
- finale Modelle wurden anschließend manuell konfiguriert
- Reward Engineering nicht vollständig optimiert
- Sparse-Reward-Problem durch seltene Goal-Erreichung
- unterschiedliche Evaluationsmechanismen für PPO und SB3-Algorithmen
- PPO musste nachträglich als ONNX exportiert werden
- Transfer-Evaluation nur mit ausgewählten Best Models
- keine vollständige Trajektorienanalyse
- keine vollständig kontrollierte Vergleichbarkeit zwischen allen Algorithmen durch unterschiedliche Frameworks

Diese Limitationen werden in den Notebooks ausführlich diskutiert.

---

## 20. Abschließende Einordnung

Das Projekt führte nicht zu einem robust gelösten Navigationsproblem. Keiner der getesteten Algorithmen konnte über alle Environments hinweg stabile Zielnavigation zeigen.

Trotzdem liefert das Projekt einen wichtigen positiven Befund: DQN Seed 27 im stochastischen Modus erreichte als einziges Modell nennenswerte Goal Rates in Env1 und Env3. Dieses Ergebnis zeigt, dass das entwickelte Setup grundsätzlich lernbares Navigationsverhalten ermöglicht, auch wenn die resultierende Policy noch nicht robust ist.

Der Wert des Projekts liegt daher vor allem in der geschaffenen experimentellen Grundlage. Die Unity-Environments, die Trainings- und Evaluationspipeline, die Random Baseline, die Transferstruktur und die dokumentierten Limitationen bilden eine Basis, auf der zukünftige Experimente gezielter aufbauen können.

Für weitere Untersuchungen wären insbesondere Reward Engineering, Curriculum Learning, alternative Action Spaces, längere Trainingsläufe, robustere Hyperparameter-Optimierung und eine breitere Seed-Evaluation relevant.

---

## 21. Technologiestack

Wichtige Komponenten:

- Unity 2022.3.62f3
- Unity ML-Agents
- Python 3.9
- Stable-Baselines3
- Gymnasium / Gym
- PyTorch
- Optuna
- pandas
- NumPy
- matplotlib
- TensorBoard
- ONNX

---

## 22. Hinweise zu großen Dateien

Gerade die Videoaufnahmen der Trainings- und Evaluationsläufe können sehr groß werden. Diese sind über die `.gitignore` Datei aus dem Repository ausgeschlossen.

Temporäre Unity-Ordner, Cache-Dateien, private Lernnotizen und große generierte Artefakte werden über `.gitignore` ausgeschlossen.

---

## 23. Autor

Dennis Blum

Digital Business University of Applied Sciences

Studiengang: M.Sc. Data Science & Management

Modul: Machine Learning

Hinweis: Das Projekt wurde im Rahmen einer Studienarbeit im Masterstudiengang Data Science & Management erstellt.