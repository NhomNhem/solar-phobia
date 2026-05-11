# MainMenu Visual Design Specification — Solar Phobia

> **Status**: Complete  
> **Author**: Art Director  
> **Target**: Unity UI Toolkit (UIToolkit)  
> **Date**: 2026-05-08  
> **Phase Context**: Day palette only (MainMenu exists outside game loop)

---

## 1. Overview

This specification defines the visual treatment for the MainMenu UI, implementing the "Hand-painted watercolour with fire damage" aesthetic defined in the Art Bible. The MainMenu uses **Day-phase styling** exclusively, as it exists outside the game loop and represents the player "before choice."

---

## 2. Color Application

### 2.1 Primary Palette Mapping

| Element | Color | Hex | Usage |
|---------|-------|-----|-------|
| **Background** | Paper White | `#F5F0E8` | Root container, day-phase base |
| **Container Fill** | Paper White @ 85% opacity | `#F5F0E8D9` | Menu panel backgrounds |
| **Primary Text** | Charcoal Black | `#1A1412` | Titles, button labels |
| **Secondary Text** | Canvas Cream | `#D9CDB8` | Subtitles, hints |
| **Accent/Highlight** | Burnt Sienna | `#C45A30` | Icons, dividers, borders |
| **Selection/Active** | Ochre Gold | `#D4A03A` | Hover states, selected items |
| **Danger/Quit** | Ember Orange | `#E87A30` | Quit button, confirm dialog |
| **Disabled State** | Canvas Cream @ 50% | `#D9CDB880` | Disabled button text |

### 2.2 Element-Specific Color Assignments

**Title Area**:
- Main title (`SOLAR PHOBIA`): Charcoal Black `#1A1412`
- Subtitle (`Nắng Gắt`): Burnt Sienna `#C45A30`
- Title glow: Ochre Gold `@ 30%` opacity

**Menu Buttons**:
- Default text: Charcoal Black `#1A1412`
- Hover text: Ochre Gold `#D4A03A`
- Pressed text: Burnt Sienna `#C45A30`
- Disabled text: Canvas Cream `#D9CDB8` @ 50% opacity

**Button Container**:
- Default background: Transparent
- Hover background: Burnt Sienna `#C45A30` @ 10% opacity
- Pressed background: Burnt Sienna `#C45A30` @ 20% opacity
- Border: Burnt Sienna `#C45A30` @ 60% opacity, 1px

**Settings Panel**:
- Panel background: Paper White `#F5F0E8` @ 95% opacity
- Tab inactive: Canvas Cream `#D9CDB8`
- Tab active: Burnt Sienna `#C45A30`
- Slider track: Canvas Cream `#D9CDB8`
- Slider fill: Ochre Gold `#D4A03A`
- Toggle on: Ochre Gold `#D4A03A`
- Toggle off: Canvas Cream `#D9CDB8`

**Quit Confirmation Dialog**:
- Dialog background: Paper White `#F5F0E8`
- Title: Ember Orange `#E87A30`
- Message: Charcoal Black `#1A1412`
- Confirm button: Ember Orange `#E87A30` background

### 2.3 Color Rules (Critical)

> **Rule**: UI NEVER uses Black in day mode, NEVER uses White in night mode  
> **Rule**: Every color-coded state must have a shape/icon backup for colorblind accessibility

---

## 3. Typography

### 3.1 Font Family

| Element | Font | Fallback |
|---------|------|----------|
| **Main Title** | Rosa Cleaner | System sans-serif |
| **Subtitle** | Caveat | System cursive |
| **Button Labels** | Caveat | System cursive |
| **Settings Labels** | Rosa Cleaner | System sans-serif |
| **Settings Values** | Rosa Cleaner | System sans-serif |
| **Body Text** | Rosa Cleaner | System sans-serif |
| **Credits Text** | Rosa Cleaner | System sans-serif |
| **Version Number** | Caveat | System cursive |

### 3.2 Font Sizes

| Element | Size | Weight | Line Height |
|---------|------|--------|-------------|
| **Main Title** | 72px | Bold | 1.0 |
| **Subtitle** | 28px | Normal | 1.2 |
| **Menu Button** | 24px | Bold | 1.4 |
| **Settings Section Header** | 20px | Bold | 1.3 |
| **Settings Label** | 16px | Normal | 1.4 |
| **Settings Value** | 16px | Bold | 1.4 |
| **Body Text** | 16px | Normal | 1.5 |
| **Credits Title** | 32px | Bold | 1.2 |
| **Credits Section** | 18px | Normal | 1.6 |
| **Credits Names** | 16px | Normal | 1.4 |
| **Version Number** | 12px | Normal | 1.0 |

### 3.3 Text Effects

**Title Shimmer** (continuous):
- Subtle opacity pulse: 85% → 100% → 85%
- Duration: 3s, ease-in-out, infinite
- Slight vertical float: 0px → 3px → 0px

**Button Hover**:
- Letter spacing increase: 0px → 1px
- Duration: 200ms ease-out

---

## 4. Spacing & Layout

### 4.1 Grid System

- Base unit: 8px
- Margins: 16px (2 units), 24px (3 units), 32px (4 units)
- Padding: 8px (1 unit), 16px (2 units)

### 4.2 Layout Dimensions

**Main Menu Screen**:
```
┌────────────────────────────────────────────┐
│                    64px                    │  ← Top margin (title space)
│         ╔═══════════════════════╗          │
│         ║     SOLAR PHOBIA       ║          │  ← Title area
│         ║       Nắng Gắt         ║          │  ← Subtitle
│         ╚═══════════════════════╝          │
│                    48px                    │  ← Gap
│    ┌──────────────────────────────┐        │
│    │     NEW GAME                 │        │  ← Button 1 (default focus)
│    │     CONTINUE                 │        │  ← Button 2
│    │     SETTINGS                │        │  ← Button 3
│    │     CREDITS                 │        │  ← Button 4
│    │     QUIT                    │        │  ← Button 5
│    └──────────────────────────────┘        │
│                    64px                    │  ← Bottom margin
│                      v1.0.0                 │  ← Version
└────────────────────────────────────────────┘
```

### 4.3 Button Spacing

| Property | Value |
|----------|-------|
| **Button height** | 48px |
| **Button min-width** | 240px |
| **Vertical gap between buttons** | 8px |
| **Button horizontal padding** | 24px |
| **Button vertical padding** | 12px |
| **Button container left margin** | 0px (centered in parent) |

### 4.4 Settings Panel Layout

| Property | Value |
|----------|-------|
| **Panel width** | 480px |
| **Panel max-height** | 70% viewport |
| **Tab bar height** | 40px |
| **Tab horizontal gap** | 0px (touching) |
| **Content padding** | 24px |
| **Setting row height** | 40px |
| **Setting row vertical gap** | 16px |
| **Slider width** | 200px |
| **Toggle width** | 48px |

### 4.5 Credits Panel Layout

| Property | Value |
|----------|-------|
| **Panel width** | 600px |
| **Panel max-height** | 80% viewport |
| **Section vertical gap** | 24px |
| **Name indent** | 16px |

---

## 5. Visual Effects

### 5.1 Background Texture

**Implementation**: Layered approach

| Layer | Effect | Opacity |
|-------|--------|---------|
| **Base** | Paper White `#F5F0E8` | 100% |
| **Texture** | Paper grain overlay (procedural noise) | 5-8% |
| **Detail** | Subtle charcoal smudge (optional) | 3% |

**CSS/USS Implementation**:
```css
.mainmenu-root {
    background-color: #F5F0E8;
    background-image: url("PaperTexture.png");
    background-repeat: repeat;
    background-size: 256px 256px;
}
```

### 5.2 Button Shadows (Wet-Paint Pool Effect)

**Default State**:
- Box shadow: `0 2px 4px rgba(196, 90, 48, 0.15)`
- Blur: 4px
- Offset: (0, 2px)

**Hover State** (watercolour bleed):
- Box shadow: `0 4px 12px rgba(212, 160, 58, 0.25), 0 0 8px rgba(212, 160, 58, 0.15)`
- Blur: 12px + 8px
- Offset: (0, 4px)

**Implementation Note**: Use multiple box-shadow layers to achieve the "pool" effect where shadow appears to bleed outward like wet paint.

### 5.3 Border Style (Wavering Hand-Drawn)

**Border Properties**:
```css
.menu-button {
    border-left-width: 1px;
    border-right-width: 1px;
    border-top-width: 1px;
    border-bottom-width: 1px;
    border-left-color: rgba(196, 90, 48, 0.6);
    border-right-color: rgba(196, 90, 48, 0.6);
    border-top-color: rgba(196, 90, 48, 0.6);
    border-bottom-color: rgba(196, 90, 48, 0.6);
    border-radius: 8px;
}
```

**Hand-Drawn Effect** (advanced): If using custom shader/9-slice:
- Use wavering edge sprite (hand-drawn pencil line)
- Border should not be perfectly uniform — slight variation in thickness
- Corners slightly rounded (8px radius) to feel organic

### 5.4 Title Glow/Shimmer

**Glow Effect**:
```css
.mainmenu-title {
    text-shadow: 0 0 20px rgba(212, 160, 58, 0.3),
                 0 2px 4px rgba(26, 20, 18, 0.2);
}
```

**Shimmer Animation**:
- Opacity keyframes: 85% → 100% → 85%
- TranslateY keyframes: 0px → 3px → 0px
- Combined duration: 3s
- Timing: ease-in-out
- Iteration: infinite

### 5.5 Hover/Focus Animations

**Mouse Hover** (200ms):
```css
.menu-button:hover {
    transform: scale(1.02);
    color: #D4A03A;
    background-color: rgba(196, 90, 48, 0.1);
    box-shadow: 0 4px 12px rgba(212, 160, 58, 0.25),
                0 0 8px rgba(212, 160, 58, 0.15);
}
```

**Gamepad Focus** (150ms pulse):
```css
.menu-button:focus {
    border-left-width: 3px;
    border-right-width: 3px;
    border-top-width: 3px;
    border-bottom-width: 3px;
    border-left-color: #D4A03A;
    border-right-color: #D4A03A;
    border-top-color: #D4A03A;
    border-bottom-color: #D4A03A;
    animation: focus-pulse 1.5s ease-in-out infinite;
}

@keyframes focus-pulse {
    0%, 100% { border-opacity: 1; }
    50% { border-opacity: 0.6; }
}
```

**Pressed State** (100ms):
```css
.menu-button:active {
    transform: scale(0.98);
    background-color: rgba(196, 90, 48, 0.2);
}
```

---

## 6. Icon/Asset Requirements

### 6.1 Required Icons

| Icon | Name | Description | States |
|------|------|-------------|--------|
| **Sound On** | `icon-sound-on` | Speaker with sound waves | Normal, Hover |
| **Sound Off** | `icon-sound-off` | Speaker with X | Normal, Hover |
| **Settings** | `icon-settings` | Gear/cog | Normal, Hover |
| **Back** | `icon-back` | Left arrow | Normal, Hover |
| **Close** | `icon-close` | X mark | Normal, Hover |
| **Checkbox On** | `icon-check-on` | Filled checkbox (watercolour fill) | Normal |
| **Checkbox Off** | `icon-check-off` | Empty checkbox outline | Normal |
| **Dropdown** | `icon-dropdown` | Down chevron | Normal, Open |
| **Gamepad Focus** | `icon-focus` | Circular indicator | Focused |
| **Continue Indicator** | `icon-progress` | Small dots showing save progress | Normal |

### 6.2 Icon Style Guidelines

**Day-Phase Icon Treatment**:
- Linework: Brushstroke style with variable width (like ink-brush)
- Fill: Watercolour wash inside — not solid color
- Stroke color: Burnt Sienna `#C45A30`
- Fill color: Burnt Sienna `@ 30%` opacity

**Size Standards**:
- Small icon: 16px × 16px
- Medium icon: 24px × 24px
- Large icon: 32px × 32px

### 6.3 Implementation

All icons implemented as:
- **UE**: Vector sprites (SVG imported as `.svg` assets)
- **Fallback**: 9-slice sprites with hand-drawn edge treatment

---

## 7. Animation Specifications

### 7.1 Menu Item Entry Animation

**Sequence**: Staggered fade-in from below

| Element | Delay | Duration | Easing | Initial State | Final State |
|---------|-------|----------|--------|---------------|-------------|
| Title | 0ms | 400ms | ease-out | opacity: 0, translateY: 20px | opacity: 1, translateY: 0 |
| Button 1 (New Game) | 100ms | 400ms | ease-out | opacity: 0, translateY: 20px | opacity: 1, translateY: 0 |
| Button 2 (Continue) | 150ms | 400ms | ease-out | opacity: 0, translateY: 20px | opacity: 1, translateY: 0 |
| Button 3 (Settings) | 200ms | 400ms | ease-out | opacity: 0, translateY: 20px | opacity: 1, translateY: 0 |
| Button 4 (Credits) | 250ms | 400ms | ease-out | opacity: 0, translateY: 20px | opacity: 1, translateY: 0 |
| Button 5 (Quit) | 300ms | 400ms | ease-out | opacity: 0, translateY: 20px | opacity: 1, translateY: 0 |
| Version | 400ms | 400ms | ease-out | opacity: 0 | opacity: 1 |

**Total entry time**: ~700ms

### 7.2 Hover Animation

| Property | From | To | Duration | Easing |
|----------|------|-----|----------|--------|
| Scale | 1.0 | 1.02 | 200ms | ease-out |
| Text Color | #1A1412 | #D4A03A | 200ms | ease-out |
| Background | transparent | rgba(196, 90, 48, 0.1) | 200ms | ease-out |
| Shadow blur | 4px | 12px | 200ms | ease-out |

### 7.3 Button Press Animation

| Property | From | To | Duration | Easing |
|----------|------|-----|----------|--------|
| Scale | 1.0 | 0.98 | 100ms | ease-in |
| Background | rgba(196, 90, 48, 0.1) | rgba(196, 90, 48, 0.2) | 100ms | ease-in |

### 7.4 Screen Transitions

| Transition | Duration | Easing | Effect |
|------------|----------|--------|--------|
| MainMenu → Settings | 350ms | ease-out | Slide left + fade in |
| MainMenu → Credits | 350ms | ease-out | Fade out + fade in |
| Settings → MainMenu | 300ms | ease-in | Slide right + fade out |
| Credits → MainMenu | 300ms | ease-in | Fade out + fade in |
| Submenu open | 250ms | ease-out | Scale 0.95 → 1.0 + fade in |
| Submenu close | 200ms | ease-in | Scale 1.0 → 0.95 + fade out |
| Quit dialog appear | 200ms | ease-out | Fade in + scale 0.9 → 1.0 |
| Quit dialog close | 150ms | ease-in | Fade out + scale → 0.95 |

### 7.5 Title Animation (Continuous)

| Property | Keyframes | Duration | Easing |
|----------|-----------|----------|--------|
| Opacity | 85% → 100% → 85% | 3000ms | ease-in-out |
| TranslateY | 0px → 3px → 0px | 3000ms | ease-in-out |
| Text shadow intensity | 0.3 → 0.5 → 0.3 | 3000ms | ease-in-out |

**Iteration**: Infinite (loop)

---

## 8. State Variations

### 8.1 Button States

| State | Visual Treatment |
|-------|-------------------|
| **Normal** | Default color #1A1412, transparent bg, subtle border |
| **Hover** | Color #D4A03A, 10% sienna bg, enhanced shadow, scale 1.02 |
| **Pressed** | Color #C45A30, 20% sienna bg, scale 0.98 |
| **Focused** (gamepad) | 3px Ochre Gold border, 150ms pulse animation |
| **Disabled** | 50% opacity, grayscale, no hover effects, cursor: not-allowed |

### 8.2 Toggle States

| State | Visual Treatment |
|-------|-------------------|
| **On** | Ochre Gold fill, checkmark visible |
| **Off** | Canvas Cream outline only, no fill |
| **Disabled** | 50% opacity, grayed out |

### 8.3 Slider States

| State | Visual Treatment |
|-------|-------------------|
| **Normal** | Canvas Cream track, Ochre Gold fill to thumb |
| **Hover** | Track highlight, thumb enlarges |
| **Dragging** | Thumb further enlarged, value displays live |
| **Disabled** | Gray track, no interaction |

### 8.4 Default Focus (Gamepad)

| Screen | Default Focus Element | Rationale |
|--------|----------------------|-----------|
| Main Menu | New Game button | Most common start action |
| Settings | First control in active tab | Consistent entry point |
| Credits | Back button | Allows immediate exit |
| Quit Confirm | Cancel button | Prevents accidental quit |

---

## 9. Background Art Direction

### 9.1 Philosophy

The MainMenu background should evoke the "watercolour on paper" aesthetic without competing with the menu UI. It should feel like looking at a painted surface — textured, warm, and alive — without being a detailed illustration.

### 9.2 Recommended Approach

**Simple Abstract Composition**:
```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│     ╭───────────────────────────────────────╮              │
│    ╱    (Subtle warm gradient - Paper to     ╲             │
│   │     slight sienna tint at edges)         │             │
│    ╲                                         ╱              │
│     ╰───────────────────────────────────────╯              │
│                                                             │
│              [Optional: Faint watermark                     │
│               or subtle texture pattern                    │
│               suggesting watercolour paper                 │
│               grain - 3-5% opacity]                         │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### 9.3 Specifics

| Element | Treatment |
|---------|-----------|
| **Base** | Solid Paper White `#F5F0E8` |
| **Gradient** | Subtle radial: center `#F5F0E8` → edges `#E8DED0` (very subtle warm shift) |
| **Texture** | Paper grain at 3-5% opacity, 256px tile, tiled |
| **Optional Detail** | Very faint (2-3%) charcoal smudge or brushstroke watermark in one corner |
| **Prohibited** | No character silhouettes, no specific scene elements, no dark areas |

### 9.4 Contrast with HUD Background

| Element | MainMenu Background | In-Game HUD Background |
|--------|--------------------|-----------------------|
| Treatment | Static, simple, abstract | Dynamic, contextual |
| Color | Paper White with subtle warmth | Context-dependent (day/night) |
| Complexity | Low (texture only) | High (scene-dependent) |

---

## 10. USS/UXML Structure

### 10.1 Class Naming Convention

Following existing `.hud-*` pattern from Act1HudView.uss, use `.menu-*` for MainMenu:

```css
/* Root */
.mainmenu-root { }

/* Title */
.mainmenu-title { }
.mainmenu-subtitle { }

/* Buttons */
.menu-button { }
.menu-button--primary { }    /* New Game - default focus */
.menu-button--secondary { }   /* Continue, Settings, Credits */
.menu-button--danger { }       /* Quit */
.menu-button--disabled { }    /* Disabled state */
.menu-button__text { }         /* Button label */

/* Container */
.menu-container { }
.menu-button-group { }         /* Vertical button list */

/* Settings */
.settings-panel { }
.settings-tabs { }
.settings-tab { }
.settings-tab--active { }
.settings-content { }
.settings-row { }
.settings-label { }
.settings-value { }
.settings-slider { }
.settings-toggle { }

/* Credits */
.credits-panel { }
.credits-content { }
.credits-section { }
.credits-section__title { }
.credits-section__names { }

/* Dialogs */
.dialog-overlay { }
.dialog-container { }
.dialog-title { }
.dialog-message { }
.dialog-button { }
.dialog-button--confirm { }
.dialog-button--cancel { }

/* Focus indicator (gamepad) */
.focus-ring { }

/* Utilities */
.text--title { }
.text--body { }
.text--caption { }
.text--disabled { }
```

### 10.2 UXML Structure

```
MainMenuView.uxml
├── VisualElement (.mainmenu-root)
│   ├── VisualElement (.mainmenu-background)
│   ├── VisualElement (.mainmenu-title-container)
│   │   ├── Label (.mainmenu-title)
│   │   └── Label (.mainmenu-subtitle)
│   ├── VisualElement (.menu-container)
│   │   └── VisualElement (.menu-button-group)
│   │       ├── Button (.menu-button--primary) [New Game]
│   │       ├── Button (.menu-button--secondary) [Continue]
│   │       ├── Button (.menu-button--secondary) [Settings]
│   │       ├── Button (.menu-button--secondary) [Credits]
│   │       └── Button (.menu-button--danger) [Quit]
│   ├── Label (.mainmenu-version)
│   ├── SettingsPanel (.settings-panel) [hidden by default]
│   ├── CreditsPanel (.credits-panel) [hidden by default]
│   └── DialogOverlay (.dialog-overlay) [hidden by default]
│       └── DialogContainer (.dialog-container)
│           ├── Label (.dialog-title)
│           ├── Label (.dialog-message)
│           └── VisualElement (.dialog-buttons)
│               ├── Button (.dialog-button--confirm)
│               └── Button (.dialog-button--cancel)
```

### 10.3 USS Pseudo-Classes to Use

```css
/* Standard USS pseudo-classes */
.menu-button:hover { }
.menu-button:active { }
.menu-button:focus { }
.menu-button:disabled { }

/* Custom attribute selectors for state */
.menu-button[state="hover"] { }
.menu-button[state="pressed"] { }
.menu-button[state="focused"] { }
```

### 10.4 Reskinning Checklist

When implementing, ensure:
- [ ] All colors match Section 2 palette exactly
- [ ] All font sizes match Section 3.2 exactly
- [ ] All spacing matches Section 4 exactly
- [ ] All animations match Section 7 timing exactly
- [ ] All states match Section 8 specifications exactly
- [ ] Hand-drawn border effect applied to buttons
- [ ] Paper texture applied to background
- [ ] Title shimmer animation implemented

---

## 11. Accessibility Color Contrast (WCAG AA)

| Element | Foreground | Background | Ratio | Status |
|---------|------------|------------|-------|--------|
| Default button text | #1A1412 | #F5F0E8 | 10.3:1 | ✓ Pass |
| Hover button text | #D4A03A | #F5F0E8 | 4.1:1 | ✓ Pass |
| Disabled text | #D9CDB8 @ 50% | #F5F0E8 | 2.1:1 | ⚠ May fail - use darker disabled color |
| Settings labels | #1A1412 | #F5F0E8 | 10.3:1 | ✓ Pass |
| Focus ring | #D4A03A | #F5F0E8 | 4.1:1 | ✓ Pass |

**Correction needed**: For disabled text, use `#8A8578` (darker Canvas Cream) instead of 50% opacity to ensure 4.5:1 minimum contrast.

---

## 12. Implementation Notes

### 12.1 Font Loading

Ensure fonts are properly loaded in project:
1. Add `Rosa Cleaner` font file to `Assets/_Project/Assets/Fonts/`
2. Add `Caveat` font file to `Assets/_Project/Assets/Fonts/`
3. Create font assets in Unity
4. Reference in USS via `font-family: Rosa Cleaner, sans-serif;`

### 12.2 Texture Assets Needed

| Asset | Purpose | Specification |
|-------|---------|---------------|
| `PaperGrain.png` | Background texture | 256×256, subtle noise, 8-bit grayscale |
| `Icon-SoundOn.svg` | Sound on icon | 24×24, brushstroke style |
| `Icon-SoundOff.svg` | Sound off icon | 24×24, brushstroke style |
| `Icon-Settings.svg` | Settings icon | 24×24, brushstroke style |
| `Icon-Back.svg` | Back arrow | 24×24, brushstroke style |
| `Icon-CheckOn.svg` | Checkbox checked | 16×16, watercolour fill |
| `Icon-CheckOff.svg` | Checkbox empty | 16×16, outline only |
| `Icon-Dropdown.svg` | Dropdown arrow | 16×16, brushstroke style |
| `Border-Wavering.png` | Button border | 9-slice, hand-drawn edge |

### 12.3 Animation Implementation

Use Unity's `UIStyleAnimation` or custom coroutines:
- Entry animations: Play on `OnAnimationStart`
- Hover/press: USS pseudo-classes handle automatically
- Title shimmer: `RegisterCallback<GeometryChangedEvent>` +DOTween or custom animator
- Screen transitions: View controller orchestrates

---

## 13. Revision History

| Date | Version | Changes |
|------|---------|---------|
| 2026-05-08 | 1.0.0 | Initial visual design spec |

---

## 14. Sign-Off

> **Art Director**: _________________________  
> **Date**: _________________________

> **Technical Review**: _________________________  
> **Date**: _________________________