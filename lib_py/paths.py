from pathlib import Path  # wird genutzt, um Pfade sauber und betriebssystemunabhängig zu verwalten

PROJECT_ROOT = Path(__file__).resolve().parents[1]  # Projektroot: RL_NAVIGATION_PROJECT
PROJECT_NAME = PROJECT_ROOT.name  # Name des Projektordners, z. B. RL_NAVIGATION_PROJECT

MAZE_AGENT_DIR = PROJECT_ROOT / "MazeAgent"  # Unity-Projektordner
NOTEBOOKS_DIR = PROJECT_ROOT / "notebooks"  # Ordner für Jupyter Notebooks
TRAINING_DIR = PROJECT_ROOT / "training"  # zentraler Trainingsordner

CONFIG_DIR = TRAINING_DIR / "configs"  # Ordner für YAML-Konfigurationen
MODEL_DIR = TRAINING_DIR / "models"  # Ordner für gespeicherte Modelle
LOG_DIR = TRAINING_DIR / "logs"  # Ordner für Trainingslogs

BASELINE_LOG_DIR = MAZE_AGENT_DIR / "training" / "evaluation_logs"  # Random-Baseline-CSV-Dateien aus Unity

DOCS_DIR = PROJECT_ROOT / "docs"  # Dokumentationsordner
EVALUATION_DIR = DOCS_DIR / "evaluation"  # Ordner für Evaluationsergebnisse
TABLE_DIR = EVALUATION_DIR / "tables"  # Ordner für Ergebnistabellen
FIGURE_DIR = EVALUATION_DIR / "figures"  # Ordner für Abbildungen


# Hilfsfunktion, um die benötigten Projektordner automatisch zu erstellen, falls sie noch nicht existieren. So wird sichergestellt, dass die Trainingspipeline ohne Fehler durchläuft, wenn Modelle gespeichert oder Logs angelegt werden.
def ensure_project_dirs() -> None:
    MODEL_DIR.mkdir(parents=True, exist_ok=True)  # Modellordner erstellen
    LOG_DIR.mkdir(parents=True, exist_ok=True)  # Logordner erstellen
    TABLE_DIR.mkdir(parents=True, exist_ok=True)  # Tabellenordner erstellen
    FIGURE_DIR.mkdir(parents=True, exist_ok=True)  # Abbildungsordner erstellen


# Hilfsfunktion, um Pfade in den Notebooks übersichtlich darzustellen und dabei den Projektordner als Ausgangspunkt zu verwenden (nicht meine komplette Ordner-Struktur)
def show_project_path(path: Path) -> str:
    relative_path = path.resolve().relative_to(PROJECT_ROOT)  # Pfad relativ zum Projektroot berechnen
    return f"{PROJECT_NAME}/{relative_path.as_posix()}"  # Ausgabe ab Projektordner bauen