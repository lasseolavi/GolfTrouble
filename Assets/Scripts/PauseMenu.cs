using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    private GameObject overlay;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        GameObject go = new GameObject("PauseMenu");
        DontDestroyOnLoad(go);
        go.AddComponent<PauseMenu>();
    }

    void Awake()
    {
        BuildUI();
        SetPaused(false);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetPaused(false); // never carry a paused state into a fresh scene
    }

    void Update()
    {
        if (GameManager.Instance == null) return; // not in a level

        if (Input.GetKeyDown(KeyCode.Escape)) SetPaused(!IsPaused);
        if (!IsPaused && Input.GetKeyDown(KeyCode.R)) RestartLevel();
    }

    void SetPaused(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        AudioListener.pause = paused;
        if (overlay != null) overlay.SetActive(paused);
    }

    public void RestartLevel()
    {
        SetPaused(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void GoToMenu()
    {
        SetPaused(false);
        SceneManager.LoadScene(0);
    }

    void BuildUI()
    {
        Canvas canvas = RuntimeUi.MakeOverlayCanvas("PauseCanvas", transform, 100);
        overlay = RuntimeUi.MakeDimmer(canvas.transform, 0.6f);

        RuntimeUi.MakeText(overlay.transform, "PAUSED", 72, new Vector2(0, 170));
        RuntimeUi.MakeButton(overlay.transform, "Resume", new Vector2(0, 40), () => SetPaused(false));
        RuntimeUi.MakeButton(overlay.transform, "Restart Level  (R)", new Vector2(0, -55), RestartLevel);
        RuntimeUi.MakeButton(overlay.transform, "Main Menu", new Vector2(0, -150), GoToMenu);

        overlay.SetActive(false);
    }
}
