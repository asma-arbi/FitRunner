using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public RunnerController player;
    public float timeLimit = 50f;        // chrono en secondes
    public float finishDistance = 480f;  // ligne d'arrivée
    public float healthyBoost = 1.5f;
    public float unhealthyPenalty = 2f;

    float timeLeft;
    int healthyCount;
    bool over, won;
    GUIStyle style;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        timeLeft = timeLimit;
    }

    void Update()
    {
        if (over)
        {
            var ts = Touchscreen.current;
            var kb = Keyboard.current;
            bool tap = (ts != null && ts.primaryTouch.press.wasPressedThisFrame)
                    || (kb != null && kb.rKey.wasPressedThisFrame);
            if (tap) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        timeLeft -= Time.deltaTime;
        if (player.transform.position.y < -2f) End(false);          // tombé dans un trou
        else if (timeLeft <= 0f) End(false);                         // chrono fini ou égalité : perdu
        else if (player.Distance >= finishDistance) End(true);       // arrivé avant la fin : gagné
    }

    public void Eat(bool healthy)
    {
        if (over) return;
        if (healthy)
        {
            player.AddSpeed(healthyBoost);
            healthyCount++;
        }
        else
        {
            player.AddSpeed(-unhealthyPenalty);
            if (player.Speed <= player.minSpeed) End(false);         // vitesse trop basse
        }
    }

    public void Crash() { if (!over) End(false); }

    void End(bool win)
    {
        over = true;
        won = win;
        Time.timeScale = 0f;
    }

    int Score() => Mathf.RoundToInt(player.Distance) + healthyCount * 10;

    void OnGUI()
    {
        if (style == null) style = new GUIStyle(GUI.skin.label);
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = Screen.height / 28;
        float lh = style.fontSize * 1.5f;

        GUI.Label(new Rect(20, 20, Screen.width, lh),
                  "Temps : " + Mathf.Max(0f, timeLeft).ToString("0.0") + " s", style);
        GUI.Label(new Rect(20, 20 + lh, Screen.width, lh),
                  "Distance : " + Mathf.RoundToInt(player.Distance) + " / " + (int)finishDistance + " m", style);
        GUI.Label(new Rect(20, 20 + lh * 2, Screen.width, lh),
                  "Vitesse : " + player.Speed.ToString("0.0") + "   Repas sains : " + healthyCount, style);

        if (over)
        {
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = Screen.height / 16;
            GUI.Label(new Rect(0, Screen.height * 0.30f, Screen.width, 300),
                      won ? "BRAVO !" : "GAME OVER", style);

            style.fontSize = Screen.height / 28;
            lh = style.fontSize * 1.5f;
            GUI.Label(new Rect(0, Screen.height * 0.45f, Screen.width, lh), "Score : " + Score(), style);
            GUI.Label(new Rect(0, Screen.height * 0.45f + lh, Screen.width, lh),
                      "Touche l'écran pour rejouer", style);
        }
    }
}