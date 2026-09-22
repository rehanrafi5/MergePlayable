using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameplayHandler : MonoBehaviour
{
    public enum GameState { WaitingToStart, Countdown, Fighting, Win, Lose }
    public GameState currentState;

    [Header("UI Elements")]
    public GameObject startButton;
    public TextMeshProUGUI countdownText; 
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("References")]
    public MonsterController monster;

    void Start()
    {
        SetGameState(GameState.WaitingToStart);
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    void Update()
    {
        // Sirf fighting state mein win/lose conditions check honi chahiye
        if (currentState == GameState.Fighting)
        {
            CheckBattleConditions();
        }
    }

    public void OnStartButtonClicked()
    {
        if (currentState == GameState.WaitingToStart)
        {
            if (startButton != null) startButton.SetActive(false);
            StartCoroutine(StartCountdownRoutine());
        }
    }

    IEnumerator StartCountdownRoutine()
    {
        SetGameState(GameState.Countdown);

        if (countdownText != null) countdownText.gameObject.SetActive(true);

        int countdownTime = 3;
        while (countdownTime > 0)
        {
            if (countdownText != null) countdownText.text = countdownTime.ToString();
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }

        if (countdownText != null)
        {
            countdownText.text = "FIGHT!";
            yield return new WaitForSeconds(0.5f);
            countdownText.gameObject.SetActive(false);
        }

        // Battle Start!
        SetGameState(GameState.Fighting);
        EnableBotShooting(true);
    }

    void CheckBattleConditions()
    {
        // 1. Win Condition: Monster ki health khatam
        if (monster != null && monster.currentHealth <= 0)
        {
            TriggerWin();
            return;
        }

        // 2. Lose Condition: Scene mein zinda bots ki ginti check karo
        BotHealth[] activeBots = FindObjectsOfType<BotHealth>();
        
        // Agar scene mein ek bhi BotHealth component nahi bacha (yani saare bots marr chuke hain)
        if (activeBots.Length == 0)
        {
            TriggerLose();
        }
    }

    void TriggerWin()
    {
        if (currentState == GameState.Win) return;
        
        SetGameState(GameState.Win);
        EnableBotShooting(false);
        
        if (winPanel != null) winPanel.SetActive(true);
        Debug.Log("Player Won the Battle!");
    }

    void TriggerLose()
    {
        if (currentState == GameState.Lose) return;

        SetGameState(GameState.Lose);
        EnableBotShooting(false);

        // Monster ko bhi attack karne se rok do
        MonsterAttacker attacker = FindObjectOfType<MonsterAttacker>();
        if (attacker != null)
        {
            attacker.enabled = false;
        }

        if (losePanel != null) losePanel.SetActive(true);
        Debug.Log("Player Lost the Battle!");
    }

    void SetGameState(GameState newState)
    {
        currentState = newState;

        if (newState == GameState.WaitingToStart || newState == GameState.Countdown)
        {
            EnableBotShooting(false);
        }
    }

    void EnableBotShooting(bool enable)
    {
        BotShooter[] shooters = FindObjectsOfType<BotShooter>();
        foreach (var shooter in shooters)
        {
            if (shooter != null)
                shooter.enabled = enable;
        }
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}