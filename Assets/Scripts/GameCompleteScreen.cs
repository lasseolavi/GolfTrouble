using UnityEngine;
using UnityEngine.SceneManagement;

// Shown after finishing the last level
public static class GameCompleteScreen
{
    private static GameObject instance;

    public static void Show()
    {
        if (instance != null) return;

        instance = new GameObject("GameCompleteScreen");
        Canvas canvas = RuntimeUi.MakeOverlayCanvas("Canvas", instance.transform, 90);
        GameObject panel = RuntimeUi.MakeDimmer(canvas.transform, 0.85f);

        RuntimeUi.MakeText(panel.transform, "COURSE COMPLETE!", 80, new Vector2(0, 200));
        RuntimeUi.MakeText(panel.transform, Summary(), 42, new Vector2(0, 80));
        RuntimeUi.MakeButton(panel.transform, "Play Again", new Vector2(0, -60),
                             () => SceneManager.LoadScene(1));
        RuntimeUi.MakeButton(panel.transform, "Main Menu", new Vector2(0, -155),
                             () => SceneManager.LoadScene(0));
    }

    private static string Summary()
    {
        int diff = ScoreStore.RunStrokes - ScoreStore.RunPar;
        string diffText = diff == 0 ? "even par" : (diff > 0 ? "+" + diff : diff.ToString());
        string levels = ScoreStore.RunLevels == 1 ? "1 level" : ScoreStore.RunLevels + " levels";
        return levels + "  |  " + ScoreStore.RunStrokes + " strokes (par "
               + ScoreStore.RunPar + ", " + diffText + ")";
    }
}
