using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public string firstLevelName = "Level_01";
    public GameObject mainButtons;
    public GameObject levelSelectPanel;

    void Start()
    {
        ScoreStore.ResetRun();
        // Whatever state the scene was saved in, boot with the main buttons showing.
        CloseLevelSelect();
        RefreshBestScores();
    }

    // Appends the saved best score to each level button's label. Buttons are
    // matched by name ("Level3Button" -> scene "Level_03"), so new level buttons
    // that follow the same naming work automatically.
    void RefreshBestScores()
    {
        if (levelSelectPanel == null) return;

        foreach (Button button in levelSelectPanel.GetComponentsInChildren<Button>(true))
        {
            int n = ParseLevelNumber(button.name);
            if (n < 0) continue;

            Text label = button.GetComponentInChildren<Text>(true);
            if (label == null) continue;

            int best = ScoreStore.GetBest("Level_" + n.ToString("00"));
            label.text = best >= 0 ? "Level " + n + "\nBest: " + best : "Level " + n;
            if (best >= 0)
            {
                label.resizeTextForBestFit = true;
                label.resizeTextMinSize = 8;
            }
        }
    }

    static int ParseLevelNumber(string goName)
    {
        if (!goName.StartsWith("Level") || !goName.EndsWith("Button")) return -1;
        string digits = "";
        foreach (char c in goName) if (char.IsDigit(c)) digits += c;
        return digits.Length > 0 ? int.Parse(digits) : -1;
    }

    public void Play()
    {
        SceneManager.LoadScene(firstLevelName);
    }

    public void OpenLevelSelect()
    {
        if (mainButtons != null) mainButtons.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(true);
    }

    public void CloseLevelSelect()
    {
        if (mainButtons != null) mainButtons.SetActive(true);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName.Trim().Trim('"'));
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}