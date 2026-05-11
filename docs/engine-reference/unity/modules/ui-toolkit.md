# UI Toolkit Module Reference

> Last verified: 2026-05-07

## Overview

UI Toolkit is Unity's modern UI system, recommended over legacy uGUI and IMGUI.

---

## Key Concepts

| Concept | Description |
|---------|-------------|
| **UXML** | UI layout markup (like HTML) |
| **USS** | UI styling (like CSS) |
| **VisualElement** | Base UI node |
| **UIDocument** | Component that loads UXML at runtime |

---

## Migration from uGUI

### Component Mapping

| uGUI | UI Toolkit |
|------|-------------|
| `Canvas` | `UIDocument` (root VisualElement) |
| `RectTransform` | `VisualElement` (layout-based) |
| `Image` | `Image` VisualElement |
| `Button` | `Button` VisualElement |
| `Text` | `Label` VisualElement |
| `Panel` | `VisualElement` with style |
| `ScrollRect` | `ScrollView` VisualElement |

---

## Event Handling Changes (Unity 6.0+)

### Old Pattern (still works but deprecated for new code)
```csharp
void OnGUI() {
    // Immediate mode - avoid
}
```

### New Pattern (USS/UXML + callbacks)
```csharp
// UXML
<UXML xmlns="UnityEngine.UIElements">
    <Button name="my-button">Click Me</Button>
</UXML>

// Code-behind
var button = rootVisualElement.Q<Button>("my-button");
button.clicked += () => Debug.Log("Clicked!");
```

---

## Best Practices

1. **Use UXML for layout** — separate structure from style
2. **Use USS for styling** — reusable style classes
3. **Use USS selectors** — `.class`, `#id`, `element`
4. **Avoid inline styles** — use USS files
5. **Use data binding** — for complex UI with ViewModels

---

## Project Integration

Since project uses VContainer + R3, UI Toolkit integrates well:

```csharp
public class UIInstaller : MonoInstaller {
    public override void Install(IContainerBuilder builder) {
        builder.RegisterComponentInHierarchy<UIDocument>();
    }
}

public class MenuController : MonoBehaviour {
    [Inject] private UIDocument _menuDocument;
    
    private void Awake() {
        var button = _menuDocument.rootVisualElement.Q<Button>("start");
        button.clicked += OnStart;
    }
}
```

---

## Cross-Reference

- See `breaking-changes.md` for UI changes
- See `deprecated-apis.md` for deprecated UI APIs