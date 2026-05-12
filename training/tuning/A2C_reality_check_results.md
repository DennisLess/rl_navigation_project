### Dokumentation der A2C-Konfigurationsversuche

A2C zeigte sich in diesem Projekt als besonders sensibel gegenüber Reward-Struktur und Hyperparameter-Wahl. Deshalb wurden die A2C-Parameter nicht einfach blind aus dem Optuna-Tuning übernommen, sondern in mehreren Reality Checks überprüft.

#### Versuch 1: Optuna-Parameter mit ursprünglicher Reward-Struktur

Zunächst wurde A2C mit den besten Parametern aus dem Optuna-Tuning gestartet:

- `learning_rate = 1.197e-05`
- `n_steps = 5`
- `gamma = 0.995`
- `ent_coef = 0.0026`
- `vf_coef = 0.75`
- `max_grad_norm = 0.5`

Dieser Run zeigte keinen stabilen Lernfortschritt. Der Agent erreichte das Ziel nur sehr selten und die meisten Episoden endeten weiterhin durch Wandkontakt. Gleichzeitig wurde während dieses Runs ein Problem in der ursprünglichen Reward-Struktur sichtbar: Eine frühe Wandkollision wurde nur mit `-0.5` bestraft, während ein vollständiges Timeout durch die aufsummierte Step-Penalty bis zu etwa `-5.0` erreichen konnte. Dadurch war frühes Scheitern aus Reward-Sicht weniger negativ als langes, aber erfolgloses Navigieren.

Dieser Run wird deshalb nicht als finaler A2C-Trainingsrun gewertet, sondern als Reality Check der Optuna-Konfiguration und der ursprünglichen Reward-Struktur.

#### Anpassung der Reward-Struktur

Vor den finalen Trainingsruns wurde die Reward-Struktur angepasst:

- Step Penalty: von `-0.001` auf `-0.0005`
- Goal Reward: von `+5.0` auf `+10.0`
- Wall Penalty: von `-0.5` auf `-5.0`
- Timeout: weiterhin keine zusätzliche Strafe

Damit ist eine Wandkollision klar negativ, ein Timeout weniger hart als eine frühe Wandkollision, und Zielerreichung wird deutlich stärker positiv gewichtet. Diese Anpassung soll verhindern, dass der Agent indirekt frühes Scheitern bevorzugt.

Die Random Baseline muss dafür nicht erneut ausgeführt werden, da der Random Agent seine Aktionen unabhängig vom Reward gewählt hat. Die aufgezeichneten Episodenverläufe bleiben gültig. Für Reward-basierte Vergleiche werden die Rewards rechnerisch mit der neuen Reward-Definition neu bestimmt.

#### Versuch 2: Manuell angepasste A2C-Konfiguration mit Reward v2

Nach der Reward-Anpassung wurde A2C mit einer moderat angepassten Konfiguration getestet:

- `learning_rate = 1e-04`
- `n_steps = 32`
- `gamma = 0.995`
- `ent_coef = 0.01`
- `vf_coef = 0.5`
- `max_grad_norm = 0.5`

Diese Anpassung sollte drei Probleme adressieren: Die Learning Rate wurde gegenüber Optuna erhöht, weil der ursprüngliche Wert sehr niedrig war. `n_steps` wurde erhöht, damit A2C längere Erfahrungsausschnitte pro Update nutzt. Zusätzlich wurde `ent_coef` erhöht, um mehr Exploration zu fördern.

Der Run zeigte leichte Verbesserungen gegenüber dem vorherigen Setup, aber weiterhin keinen stabilen Lernfortschritt. Der `recent_training_mean_reward` verbesserte sich zunächst von etwa `-5.33` auf etwa `-4.56`, stagnierte danach jedoch. Gleichzeitig blieb die Anzahl der Zielerfolge gering und der Agent zeigte weiterhin keine zuverlässige Navigationsstrategie.

#### Versuch 3: Finaler A2C-Reality-Check mit stärkerer Exploration und schnellerem Lernen

Da der zweite Versuch nur begrenzten Fortschritt zeigte, wird ein letzter A2C-Reality-Check durchgeführt. Dieser Versuch ist bewusst aggressiver gewählt, bleibt aber methodisch begründbar:

- `learning_rate = 3e-04`
- `n_steps = 64`
- `gamma = 0.995`
- `ent_coef = 0.02`
- `vf_coef = 0.5`
- `max_grad_norm = 0.5`

Die Learning Rate wird erhöht, damit A2C schneller auf Erfahrungen reagieren kann. `n_steps` wird auf `64` erhöht, um längere Rollouts für die Advantage-Schätzung zu nutzen. Außerdem wird `ent_coef` auf `0.02` erhöht, damit der Agent stärker exploriert und in der frühen Trainingsphase mehr Chancen hat, Zielerfahrungen zu sammeln.

Die übrigen Werte bleiben bewusst stabil. `gamma = 0.995` bleibt erhalten, weil Navigation langfristige Belohnungen beinhaltet. `vf_coef = 0.5` und `max_grad_norm = 0.5` bleiben als konservative Standardwerte bestehen.

Dieser dritte Versuch ist der letzte A2C-Reality-Check. Wenn auch diese Konfiguration keinen klaren Lernfortschritt zeigt, wird A2C nicht weiter optimiert. In diesem Fall wird A2C als Algorithmus dokumentiert, der in diesem Unity-Navigationssetup trotz Reward-Anpassung und Hyperparameter-Korrektur keinen stabilen Lernfortschritt gezeigt hat. Der Fokus wird anschließend auf DQN und PPO gelegt.



