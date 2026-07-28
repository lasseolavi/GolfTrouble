using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class RuntimeUi
{
    private static Font font;

    public static Font Font
    {
        get
        {
            if (font == null) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return font;
        }
    }

    public static Canvas MakeOverlayCanvas(string name, Transform parent, int sortingOrder)
    {
        GameObject go = new GameObject(name);
        if (parent != null) go.transform.SetParent(parent, false);
        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;
        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        go.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    public static GameObject MakeDimmer(Transform parent, float alpha)
    {
        GameObject go = new GameObject("Dimmer");
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, alpha);
        Stretch(go.GetComponent<RectTransform>());
        return go;
    }

    public static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    public static Text MakeText(Transform parent, string content, int size, Vector2 anchoredPos)
    {
        GameObject go = new GameObject("Text_" + content);
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = Font;
        text.text = content;
        text.fontSize = size;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(600, 100);
        rt.anchoredPosition = anchoredPos;
        return text;
    }

    public static Button MakeButton(Transform parent, string label, Vector2 anchoredPos, UnityAction onClick)
    {
        GameObject go = new GameObject("Button_" + label);
        go.transform.SetParent(parent, false);
        Image bg = go.AddComponent<Image>();
        bg.color = new Color(0.18f, 0.49f, 0.27f); // golf green
        Button button = go.AddComponent<Button>();
        button.targetGraphic = bg;
        button.onClick.AddListener(onClick);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(340, 75);
        rt.anchoredPosition = anchoredPos;

        Text text = MakeText(go.transform, label, 34, Vector2.zero);
        Stretch(text.GetComponent<RectTransform>());
        text.raycastTarget = false;
        return button;
    }
}
