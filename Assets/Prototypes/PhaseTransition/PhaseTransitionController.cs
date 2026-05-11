// PROTOTYPE - NOT FOR PRODUCTION
// Self-contained: creates its own UI at runtime

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PhaseTransitionController : MonoBehaviour
{
    // === CONFIGURATION ===
    [Header("Day Phase")]
    public float dayCameraDistance = 5f;
    public Color dayColorGrade = new Color(1f, 0.9f, 0.7f);
    public float dayVignetteAlpha = 0f;

    [Header("Night Phase")]
    public float nightCameraDistance = 12f;
    public Color nightColorGrade = new Color(0.4f, 0.5f, 0.8f);
    public float nightVignetteAlpha = 0.6f;

    [Header("Transition Timing")]
    public float transitionDuration = 0.5f;
    public float dayPhaseDuration = 5f;

    // === STATE ===
    private enum Phase { Day, Transitioning, Night }
    private Phase currentPhase = Phase.Day;
    private float phaseTimer;
    
    // === UI REFERENCES (assigned at runtime) ===
    private Text statusText;
    private Text timerText;
    private Button triggerButton;
    private Image vignetteOverlay;
    private RawImage colorGradeOverlay;

    // === AUTO-SETUP ON WAKE ===
    void Awake()
    {
        CreateUIAutomatically();
    }

    private void CreateUIAutomatically()
    {
        // Create Canvas
        var canvasGO = new GameObject("PhaseTransitionCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Status Text
        var statusGO = new GameObject("StatusText");
        statusGO.transform.SetParent(canvasGO.transform);
        statusText = statusGO.AddComponent<Text>();
        statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        statusText.alignment = TextAnchor.UpperCenter;
        statusText.color = Color.white;
        statusText.fontSize = 24;
        var statusRT = statusText.rectTransform;
        statusRT.anchorMin = new Vector2(0.5f, 0.85f);
        statusRT.anchorMax = new Vector2(0.5f, 0.85f);
        statusRT.pivot = new Vector2(0.5f, 0.5f);
        statusRT.sizeDelta = new Vector2(400, 50);
        
        // Timer Text
        var timerGO = new GameObject("TimerText");
        timerGO.transform.SetParent(canvasGO.transform);
        timerText = timerGO.AddComponent<Text>();
        timerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        timerText.alignment = TextAnchor.UpperCenter;
        timerText.color = Color.yellow;
        timerText.fontSize = 18;
        var timerRT = timerText.rectTransform;
        timerRT.anchorMin = new Vector2(0.5f, 0.75f);
        timerRT.anchorMax = new Vector2(0.5f, 0.75f);
        timerRT.pivot = new Vector2(0.5f, 0.5f);
        timerRT.sizeDelta = new Vector2(300, 30);
        
        // Trigger Button
        var buttonGO = new GameObject("TriggerButton");
        buttonGO.transform.SetParent(canvasGO.transform);
        buttonGO.AddComponent<Image>().color = Color.gray;
        triggerButton = buttonGO.AddComponent<Button>();
        var buttonRT = buttonGO.GetComponent<RectTransform>();
        buttonRT.anchorMin = new Vector2(0.5f, 0.2f);
        buttonRT.anchorMax = new Vector2(0.5f, 0.2f);
        buttonRT.pivot = new Vector2(0.5f, 0.5f);
        buttonRT.sizeDelta = new Vector2(200, 50);
        
        // Button Text
        var buttonTextGO = new GameObject("ButtonText");
        buttonTextGO.transform.SetParent(buttonGO.transform);
        var buttonText = buttonTextGO.AddComponent<Text>();
        buttonText.text = "Trigger Night";
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.white;
        buttonText.rectTransform.anchorMin = Vector2.zero;
        buttonText.rectTransform.anchorMax = Vector2.one;
        buttonText.rectTransform.offsetMin = Vector2.zero;
        buttonText.rectTransform.offsetMax = Vector2.zero;
        
        // Vignette (full-screen black image with alpha)
        var vignetteGO = new GameObject("Vignette");
        vignetteGO.transform.SetParent(canvasGO.transform);
        vignetteOverlay = vignetteGO.AddComponent<Image>();
        vignetteOverlay.color = new Color(0, 0, 0, 0);
        vignetteOverlay.rectTransform.anchorMin = Vector2.zero;
        vignetteOverlay.rectTransform.anchorMax = Vector2.one;
        vignetteOverlay.rectTransform.offsetMin = Vector2.zero;
        vignetteOverlay.rectTransform.offsetMax = Vector2.zero;
        
        // Color Grade Overlay
        var colorGO = new GameObject("ColorGrade");
        colorGO.transform.SetParent(canvasGO.transform);
        colorGradeOverlay = colorGO.AddComponent<RawImage>();
        colorGradeOverlay.color = new Color(1f, 0.9f, 0.7f, 0.3f);
        colorGradeOverlay.rectTransform.anchorMin = Vector2.zero;
        colorGradeOverlay.rectTransform.anchorMax = Vector2.one;
        colorGradeOverlay.rectTransform.offsetMin = Vector2.zero;
        colorGradeOverlay.rectTransform.offsetMax = Vector2.zero;

        Debug.Log("Phase Transition UI created automatically!");
    }

    // === LIFECYCLE ===
    private void Start()
    {
        SetupInitialState();
        triggerButton.onClick.AddListener(OnTriggerClicked);
    }

    private void Update()
    {
        if (currentPhase == Phase.Day)
        {
            phaseTimer += Time.deltaTime;
            if (timerText != null)
                timerText.text = $"Day Phase: {(dayPhaseDuration - phaseTimer):F1}s";

            if (phaseTimer >= dayPhaseDuration)
            {
                StartCoroutine(TransitionToNight());
            }
        }
    }

    // === SETUP ===
    private void SetupInitialState()
    {
        Camera.main.transform.position = new Vector3(0, dayCameraDistance, 0);
        Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);
        Camera.main.fieldOfView = 60;

        if (vignetteOverlay != null)
            vignetteOverlay.color = new Color(0, 0, 0, dayVignetteAlpha);
        if (colorGradeOverlay != null)
        {
            colorGradeOverlay.color = dayColorGrade;
            colorGradeOverlay.enabled = true;
        }

        if (statusText != null)
            statusText.text = "PHASE: DAY\n(Calm, confined, warm)";
        if (triggerButton != null)
            triggerButton.interactable = true;
        
        phaseTimer = 0f;
    }

    // === TRANSITION LOGIC ===
    private void OnTriggerClicked()
    {
        if (currentPhase == Phase.Day)
            StartCoroutine(TransitionToNight());
        else if (currentPhase == Phase.Night)
            StartCoroutine(TransitionToDay());
    }

    private IEnumerator TransitionToNight()
    {
        currentPhase = Phase.Transitioning;
        if (statusText != null) statusText.text = "PHASE: TRANSITIONING...";
        if (triggerButton != null) triggerButton.interactable = false;
        if (timerText != null) timerText.text = "";

        float elapsed = 0f;
        float startDist = dayCameraDistance;
        float endDist = nightCameraDistance;
        Color startColor = dayColorGrade;
        Color endColor = nightColorGrade;
        float startVignette = dayVignetteAlpha;
        float endVignette = nightVignetteAlpha;

        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            // Camera zoom-out
            if (Camera.main != null)
                Camera.main.transform.position = new Vector3(0, Mathf.Lerp(startDist, endDist, easedT), 0);

            // Color grade
            if (colorGradeOverlay != null)
                colorGradeOverlay.color = Color.Lerp(startColor, endColor, easedT);

            // Vignette
            if (vignetteOverlay != null)
                vignetteOverlay.color = new Color(0, 0, 0, Mathf.Lerp(startVignette, endVignette, easedT));

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Finalize Night state
        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(0, nightCameraDistance, 0);
            Camera.main.transform.rotation = Quaternion.Euler(60, 0, 0);
        }
        if (colorGradeOverlay != null) colorGradeOverlay.color = nightColorGrade;
        if (vignetteOverlay != null) vignetteOverlay.color = new Color(0, 0, 0, nightVignetteAlpha);

        currentPhase = Phase.Night;
        if (statusText != null) statusText.text = "PHASE: NIGHT\n(Open, dangerous, cold)";
        if (triggerButton != null)
        {
            triggerButton.interactable = true;
            triggerButton.GetComponentInChildren<Text>().text = "Test Day→Night Again";
        }
    }

    private IEnumerator TransitionToDay()
    {
        currentPhase = Phase.Transitioning;
        if (statusText != null) statusText.text = "PHASE: TRANSITIONING...";
        if (triggerButton != null) triggerButton.interactable = false;

        float elapsed = 0f;
        float startDist = nightCameraDistance;
        float endDist = dayCameraDistance;
        Color startColor = nightColorGrade;
        Color endColor = dayColorGrade;
        float startVignette = nightVignetteAlpha;
        float endVignette = dayVignetteAlpha;
        float returnDuration = transitionDuration * 0.6f;

        while (elapsed < returnDuration)
        {
            float t = elapsed / returnDuration;
            float easedT = 1f - Mathf.Pow(1f - t, 2f);

            if (Camera.main != null)
                Camera.main.transform.position = new Vector3(0, Mathf.Lerp(startDist, endDist, easedT), 0);
            if (colorGradeOverlay != null)
                colorGradeOverlay.color = Color.Lerp(startColor, endColor, easedT);
            if (vignetteOverlay != null)
                vignetteOverlay.color = new Color(0, 0, 0, Mathf.Lerp(startVignette, endVignette, easedT));

            elapsed += Time.deltaTime;
            yield return null;
        }

        SetupInitialState();
        if (triggerButton != null)
            triggerButton.GetComponentInChildren<Text>().text = "Trigger Night";
    }
}