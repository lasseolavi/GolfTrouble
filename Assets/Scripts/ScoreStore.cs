using UnityEngine;

// Persistent best scores (PlayerPrefs) + running totals for the current playthrough.
public static class ScoreStore
{
    private static string Key(string sceneName) => "Best_" + sceneName;

    /// Best stroke count for a level, or -1 if never completed.
    public static int GetBest(string sceneName)
    {
        return PlayerPrefs.GetInt(Key(sceneName), -1);
    }

    /// Records the score; returns true if it's a new best.
    public static bool ReportScore(string sceneName, int strokes)
    {
        int best = GetBest(sceneName);
        if (best >= 0 && strokes >= best) return false;
        PlayerPrefs.SetInt(Key(sceneName), strokes);
        PlayerPrefs.Save();
        return true;
    }

    // ----- current run (in-memory, reset when a new round starts) -----

    public static int RunStrokes { get; private set; }
    public static int RunPar { get; private set; }
    public static int RunLevels { get; private set; }

    public static void ResetRun()
    {
        RunStrokes = 0;
        RunPar = 0;
        RunLevels = 0;
    }

    public static void AddCompletedLevel(int strokes, int par)
    {
        RunStrokes += strokes;
        RunPar += par;
        RunLevels++;
    }
}
