using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    public Text strokeText;
    public Text clubText;
    public Text parText;
    public GameObject levelCompletePanel;
    public Text levelCompleteText;

    [Header("Level")]
    public int par = 3;

    private int strokes = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // Entering the first level starts a fresh round
        if (SceneManager.GetActiveScene().buildIndex == 1) ScoreStore.ResetRun();

        UpdateStrokeUI();
        if (parText != null) parText.text = "Par: " + par;
    }

    public void AddStroke() { strokes++; UpdateStrokeUI(); }

    public void GoToMenu() { SceneManager.LoadScene(0); }

    public void SetClubLabel(string name)
    {
        if (clubText != null) clubText.text = "Club: " + name;
    }

    public void ShowLevelComplete()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        bool newBest = ScoreStore.ReportScore(sceneName, strokes);
        ScoreStore.AddCompletedLevel(strokes, par);

        if (levelCompletePanel != null) levelCompletePanel.SetActive(true);
        if (levelCompleteText != null)
        {
            string bestLine = newBest ? "New best!" : "Best: " + ScoreStore.GetBest(sceneName);
            levelCompleteText.text = ScoreMessage() + "\n" + bestLine;
        }
    }

    public void LoadNextLevel()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(next);
        else GameCompleteScreen.Show(); // finished the last level
    }

    string ScoreMessage()
    {
        int diff = strokes - par;
        string title;
        if (strokes == 1) title = "Hole in one!";
        else if (diff <= -3) title = "Albatross!";
        else if (diff == -2) title = "Eagle!";
        else if (diff == -1) title = "Birdie!";
        else if (diff == 0) title = "Par";
        else if (diff == 1) title = "Bogey";
        else if (diff == 2) title = "Double bogey";
        else title = "+" + diff;

        return title + "\n" + strokes + " strokes (par " + par + ")";
    }

    void UpdateStrokeUI()
    {
        if (strokeText != null) strokeText.text = "Strokes: " + strokes;
    }
}