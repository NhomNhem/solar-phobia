# MainMenu UX Specification — Solar Phobia

> **Status**: Draft  
> **Author**: UX Designer  
> **Target**: Unity UI Toolkit (UIToolkit)  
> **Date**: 2026-05-08

---

## 1. Overview

**Screen Purpose**: Primary entry point for the game. Displays title, navigation options, and handles save/load state detection.

**Platform**: Windows PC (keyboard/mouse + gamepad)

**Visual Style**: "Hand-painted watercolour with fire damage" — matches Art Bible. Day-phase styling for MainMenu (as the player is in a state of "before choice").

**Phase Context**: MainMenu is phase-agnostic — it displays the Day palette regardless of where the player last exited, since it's a menu system outside the game loop.

---

## 2. User Flow Map

```
┌─────────────────────────────────────────────────────────────────────────┐
│                           MAIN MENU                                     │
│  ┌─────────────┐  ┌─────────────┐  ┌───────────┐  ┌─────────┐         │
│  │  New Game   │  │  Continue   │  │ Settings  │  │  Quit   │         │
│  │  (enabled)  │  │ (if save)   │  │   (tab)   │  │         │         │
│  └──────┬──────┘  └──────┬──────┘  └─────┬─────┘  └────┬────┘         │
│         │                │                 │            │              │
│         v                v                 v            v              │
│    [Phase: Day]    [Phase: Resume]   [Settings Panel]   [Confirm]      │
│         │                │                 │            │              │
│         v                v                 v            v              │
│    ┌────────┐       ┌────────┐      ┌─────────────┐  ┌──────┐          │
│    │ Start  │       │ Load   │      │ Audio/Video│  │ Quit │          │
│    │ Game   │       │ Save   │      │ Controls   │  │ Game │          │
│    └────────┘       └────────┘      └─────────────┘  └──────┘          │
│                                                      (or Cancel)        │
│                                                                         │
│  [ESC] ──► Close any submenu, return to parent (main → exit confirm)  │
│  [B/Circle] ──► Back action                                             │
│  [A/X] ──► Confirm/Select                                               │
└─────────────────────────────────────────────────────────────────────────┘
```

### Entry Points

| Source | Target | Action |
|--------|--------|--------|
| Title Screen | MainMenu | Fade in from black on boot |
| Pause Menu → Quit | MainMenu | Fade out to MainMenu (if confirmed) |
| Game Over (Resolve) | MainMenu | Fade out after defeat animation |
| Victory | MainMenu | Fade out after victory sequence |

### Exit Points

| Target | Trigger | Data Passed |
|--------|---------|-------------|
| New Game | Select New Game | `PhaseState: DayService` |
| Continue | Select Continue | Save file path/index |
| Settings | Select Settings | None |
| Credits | Select Credits | None |
| Quit to Desktop | Confirm Quit | None |

---

## 3. Wireframes

### 3.1 Main Menu Screen

```
┌─────────────────────────────────────────────────────────────────────────┐
│                                                                         │
│                        ╔═══════════════════╗                           │
│                        ║   SOLAR PHOBIA     ║  ← Title/logo            │
│                        ║   Nắng Gắt        ║  ← Subtitle               │
│                        ╚═══════════════════╝                           │
│                                                                         │
│         ┌───────────────────────────────────────┐                       │
│         │                                       │                       │
│         │    [▶]  NEW GAME                      │  ← Focused (default) │
│         │                                       │                       │
│         │    [ ]  CONTINUE                      │  ← Disabled if no    │
│         │                                       │     save exists      │
│         │                                       │                       │
│         │    [ ]  SETTINGS                      │                       │
│         │                                       │                       │
│         │    [ ]  CREDITS                       │                       │
│         │                                       │                       │
│         │    [ ]  QUIT                          │                       │
│         │                                       │                       │
│         └───────────────────────────────────────┘                       │
│                                                                         │
│                              v1.0.0                                     │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

**Layout Notes**:
- Title centered horizontally, top 25% of screen
- Menu buttons centered, left-aligned text, 60% down screen
- Version number bottom-right corner, small text
- Background: Day palette (Paper White #F5F0E8 with subtle texture)

### 3.2 Settings Menu (Tabbed)

```
┌─────────────────────────────────────────────────────────────────────────┐
│  ═══════════════════════════════════════════════════════════════════   │
│  ▼ SETTINGS                              [X] Close                     │
│  ═══════════════════════════════════════════════════════════════════   │
│                                                                         │
│  ┌─────────┬─────────┬─────────┐                                        │
│  │  AUDIO  │  VIDEO  │ INPUTS  │                                        │
│  └─────────┴─────────┴─────────┘                                        │
│  ┌─────────────────────────────────────────────────────────────────┐    │
│  │                                                                  │    │
│  │  MASTER VOLUME                                                 │    │
│  │  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━○━━  75%          │    │
│  │                                                                  │    │
│  │  MUSIC VOLUME                                                   │    │
│  │  ━━━━━━━━━━━━━━━━━━━━━━━━━━○━━━━━━━━━━━━━━━━━━  60%           │    │
│  │                                                                  │    │
│  │  SFX VOLUME                                                     │    │
│  │  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━○━  80%           │    │
│  │                                                                  │    │
│  │  AMBIENT VOLUME                                                 │    │
│  │  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━○━━━━━━━━━━━━━━━━━  50%           │    │
│  │                                                                  │    │
│  │                                                                  │    │
│  │  SUBTITLES                                    [ON]              │    │
│  │  TEXT SIZE                              [Medium ▼]              │    │
│  │                                                                  │    │
│  └─────────────────────────────────────────────────────────────────┘    │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

**Video Tab**:
```
┌─────────────────────────────────────────────────────────────────────────┐
│  ═══════════════════════════════════════════════════════════════════   │
│  ▼ SETTINGS                              [X] Close                     │
│  ═══════════════════════════════════════════════════════════════════   │
│                                                                         │
│  ┌─────────┬─────────┬─────────┐                                        │
│  │  AUDIO  │  VIDEO  │ INPUTS  │                                        │
│  └─────────┴─────────┴─────────┘                                        │
│  ┌─────────────────────────────────────────────────────────────────┐    │
│  │                                                                  │    │
│  │  RESOLUTION                                              ▼ 1920x1080 │
│  │                                                                  │    │
│  │  WINDOW MODE                                         ▼ Fullscreen  │
│  │                                                                  │    │
│  │  QUALITY                                            ▼ Ultra       │
│  │                                                                  │    │
│  │  V-SYNC                                             [ON]          │
│  │                                                                  │    │
│  │  CAMERA SHAKE                                       [ON]          │    │
│  │                                                                  │    │
│  │  MOTION BLUR                                        [OFF]         │    │
│  │                                                                  │    │
│  │  UI SCALING                                          100%         │    │
│  │  ━━━━━━━━━━━━━━━━━━━━━━○━━━━━━━━━━━━━━━━━━━━━━━━━                 │    │
│  │                                                                  │    │
│  └─────────────────────────────────────────────────────────────────┘    │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

**Controls Tab**:
```
┌─────────────────────────────────────────────────────────────────────────┐
│  ═══════════════════════════════════════════════════════════════════   │
│  ▼ SETTINGS                              [X] Close                     │
│  ═══════════════════════════════════════════════════════════════════   │
│                                                                         │
│  ┌─────────┬─────────┬─────────┐                                        │
│  │  AUDIO  │  VIDEO  │ INPUTS  │                                        │
│  └─────────┴─────────┴─────────┘                                        │
│  ┌─────────────────────────────────────────────────────────────────┐    │
│  │                                                                  │    │
│  │  INPUT DEVICE                                    ▼ Keyboard/Mouse│    │
│  │                                                                  │    │
│  │  ─── KEYBOARD ──────────────────────────────────────────────     │    │
│  │  MOVE                              [W] [A] [S] [D]    [Edit]    │    │
│  │  INTERACT                        [E]                  [Edit]    │    │
│  │  SPRINT                          [Shift]              [Edit]    │    │
│  │  PAUSE                           [Esc]                [Edit]    │    │
│  │                                                                  │    │
│  │  ─── GAMEPAD ───────────────────────────────────────────────     │    │
│  │  MOVE                               Left Stick        [Edit]    │    │
│  │  INTERACT                      [A] Button            [Edit]    │    │
│  │  SPRINT                        [B] Button            [Edit]    │    │
│  │  PAUSE                         [Start]               [Edit]    │    │
│  │                                                                  │    │
│  │  INVERT Y-AXIS                              [OFF]                 │    │
│  │  GAMEPAD VIBRATION                          [ON]                │    │
│  │                                                                  │    │
│  └─────────────────────────────────────────────────────────────────┘    │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

### 3.3 Credits Screen

```
┌─────────────────────────────────────────────────────────────────────────┐
│                                                                         │
│                        ╔═══════════════════╗                           │
│                        ║      CREDITS       ║                           │
│                        ╚═══════════════════╝                           │
│                                                                         │
│  ┌───────────────────────────────────────────────────────────────────┐  │
│  │                                                                   │  │
│  │  CONCEPT & DESIGN                                                │  │
│  │  ─────────────────────────────────────────────────────────────    │  │
│  │  Lead Designer         ████████                                   │  │
│  │  Art Director          ████████                                   │  │
│  │  Game Designer         ████████                                   │  │
│  │                                                                   │  │
│  │  PROGRAMMING                                                 [▶]   │  │
│  │  ─────────────────────────────────────────────────────────────    │  │
│  │  Lead Programmer       ████████                                   │  │
│  │  Technical Director    ████████                                   │  │
│  │                                                                   │  │
│  │  ART                                                       [▶]   │  │
│  │  ─────────────────────────────────────────────────────────────    │  │
│  │  Lead Artist           ████████                                   │  │
│  │  Concept Artist        ████████                                   │  │
│  │                                                                   │  │
│  │  AUDIO                                                      [▶]   │  │
│  │  ─────────────────────────────────────────────────────────────    │  │
│  │  Sound Designer         ████████                                   │  │
│  │  Music Composer        ████████                                   │  │
│  │                                                                   │  │
│  │  SPECIAL THANKS                                                      │  │
│  │  ████████, ████████, ████████                                     │  │
│  │                                                                   │  │
│  └───────────────────────────────────────────────────────────────────┘  │
│                              [◀ BACK]                                   │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

**Notes**:
- Credits auto-scroll vertically (slow, readable pace ~30 seconds)
- Clicking [▶] next to a section expands additional details
- Sections can be collapsed/expanded

### 3.4 Quit Confirmation Dialog

```
┌─────────────────────────────────────────────────────────────────────────┐
│                                                                         │
│  ┌─────────────────────────────────────────────────────────────────┐    │
│  │                                                                  │    │
│  │         ┌─────────────────────────────┐                         │    │
│  │         │    QUIT TO DESKTOP?        │                         │    │
│  │         └─────────────────────────────┘                         │    │
│  │                                                                  │    │
│  │            Your progress is auto-saved.                         │    │
│  │                                                                  │    │
│  │     [CONFIRM QUIT]          [CANCEL]                            │    │
│  │         (Red/Orange)         (Neutral)                           │    │
│  │                                                                  │    │
│  └─────────────────────────────────────────────────────────────────┘    │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## 4. Interaction Patterns

### 4.1 Keyboard/Mouse

| Action | Input | Visual Feedback |
|--------|-------|-----------------|
| Navigate items | Tab / Shift+Tab | Focus ring highlights item |
| Select item | Left Click / Enter | Button press animation, action fires |
| Hover | Mouse hover | Slight scale up (1.02x), color shift to Ochre |
| Cancel/Back | Escape | Close current panel, return to parent |
| Adjust sliders | Click + drag, or Click to set | Slider fills, value updates live |
| Toggle checkboxes | Click | Check/uncheck animation, value updates |

### 4.2 Gamepad

| Action | Input (Xbox) | Input (Generic) | Visual Feedback |
|--------|--------------|-----------------|-----------------|
| Navigate items | D-Pad / Left Stick | D-Pad / Left Stick | Focus ring (thick border) highlights item |
| Select item | A Button | X Button | Button press animation, action fires |
| Cancel/Back | B Button | Circle Button | Return to parent screen |
| Adjust sliders | D-Pad Left/Right | D-Pad Left/Right | Slider moves, value updates |
| Toggle checkboxes | A Button | X Button | Toggle state, checkmark appears |

### 4.3 Default Focus Selection

| Screen | Default Focus | Rationale |
|--------|---------------|------------|
| Main Menu | New Game | Most common start action |
| Settings (any tab) | First setting control | Consistent entry point |
| Credits | Back button | Allows immediate exit |
| Quit Confirm | Cancel button | Prevents accidental quit |

### 4.4 ESC Key Behavior

| Context | ESC Action |
|---------|-------------|
| Main Menu (no submenu) | Show Quit Confirmation |
| Settings Panel | Close panel, return to Main Menu |
| Credits | Close credits, return to Main Menu |
| Quit Confirmation | Cancel quit, return to Main Menu |

---

## 5. Accessibility Requirements

### 5.1 Minimum Text Sizes

| Element | Minimum Size | Recommended Size |
|---------|--------------|------------------|
| Headings (H1 - Title) | 36px | 48px |
| Headings (H2 - Section) | 24px | 32px |
| Body text | 16px | 18px |
| Button labels | 18px | 20px |
| Small/caption text | 14px | 16px |

**UIToolkit Implementation**: Use `UITextElement` and scale via USS `font-size`. Support 100%-200% text scaling via `UI.Settings.scaleFactor`.

### 5.2 Color Contrast (WCAG AA)

| Element | Foreground | Background | Ratio Required |
|---------|------------|------------|-----------------|
| Day UI text (default) | Charcoal #1A1412 | Paper White #F5F0E8 | 10.3:1 ✓ |
| Day UI text (secondary) | Burnt Sienna #C45A30 | Paper White #F5F0E8 | 5.1:1 ✓ |
| Hover state | Ochre Gold #D4A03A | Paper White #F5F0E8 | 4.1:1 ✓ |
| Focus ring | Charcoal #1A1412 | Paper White #F5F0E8 | 10.3:1 ✓ |
| Button pressed | Canvas Cream #D9CDB8 | Burnt Sienna #C45A30 | 7.8:1 ✓ |

### 5.3 Colorblind Support

**Critical**: Never rely solely on color. Every color-coded state must have a shape/icon backup:

| Semantic State | Color | Primary Backup | Secondary Backup |
|---------------|-------|----------------|-------------------|
| Danger/Hot | Ember Orange #E87A30 | **Sharp triangular icon** (fire/spike) | Proximity rumble audio |
| Selection/Active | Ochre Gold #D4A03A | **Filled circle icon** | Glow effect (not color) |
| Disabled/Off | Gray (muted) | **Outline-only icon** (no fill) | Dimmed opacity |
| Focus | N/A (focus ring) | **Thick animated border** (1.5px) | N/A |

### 5.4 Focus Indicators

- **Keyboard**: 1px solid Charcoal border
- **Gamepad**: 3px solid Ochre Gold border, 150ms pulse animation
- **Visibility**: All focus states must meet contrast requirements

### 5.5 Scalable UI

UIToolkit scales via USS percentage or `VisualElement.style.scale`. Support:
- 100% (default)
- 125%
- 150%
- 175%
- 200%

**Implementation**: Root container applies `scale` transform based on `PlayerPrefs.GetFloat("UIScale", 1.0f)`.

---

## 6. Data the UI Reads

### 6.1 Save File Detection

| Data Source | Key | Type | Usage |
|-------------|-----|------|-------|
| Save file presence | `SaveData/hasSave.json` or PlayerPrefs | bool | Enable/disable Continue button |
| Save metadata | `SaveData/save_metadata.json` | JSON | Display "Continue" or "New Game + resume" text |
| Save timestamp | `SaveData/save_metadata.json` | datetime | Display "Continue (Day X)" text |

### 6.2 Audio Settings (PlayerPrefs)

| Setting | Key | Type | Default |
|---------|-----|------|---------|
| Master Volume | `Settings.MasterVolume` | float (0-1) | 0.75 |
| Music Volume | `Settings.MusicVolume` | float (0-1) | 0.60 |
| SFX Volume | `Settings.SFXVolume` | float (0-1) | 0.80 |
| Ambient Volume | `Settings.AmbientVolume` | float (0-1) | 0.50 |
| Subtitles Enabled | `Settings.SubtitlesEnabled` | bool | true |
| Text Size | `Settings.TextSize` | enum (Small/Medium/Large) | Medium |

### 6.3 Video Settings (PlayerPrefs)

| Setting | Key | Type | Default |
|---------|-----|------|---------|
| Resolution Width | `Settings.ResolutionWidth` | int | 1920 |
| Resolution Height | `Settings.ResolutionHeight` | int | 1080 |
| Window Mode | `Settings.WindowMode` | enum (Fullscreen/Windowed/Borderless) | Fullscreen |
| Quality Level | `Settings.QualityLevel` | int (0-5) | 3 (Ultra) |
| V-Sync | `Settings.VSync` | bool | true |
| Camera Shake | `Settings.CameraShake` | bool | true |
| Motion Blur | `Settings.MotionBlur` | bool | false |
| UI Scale | `Settings.UIScale` | float | 1.0 |

### 6.4 Input Settings (PlayerPrefs)

| Setting | Key | Type | Default |
|---------|-----|------|---------|
| Input Device | `Settings.InputDevice` | enum (KeyboardMouse/Gamepad) | KeyboardMouse |
| Invert Y-Axis | `Settings.InvertYAxis` | bool | false |
| Gamepad Vibration | `Settings.GamepadVibration` | bool | true |

---

## 7. Localization

### 7.1 All Text Goes Through Localization System

**Implementation**: Use `ILocalizationService` (project's localization abstraction).

**Key Naming Convention**: `[SCREEN]_[ELEMENT]_[STATE]`

| Key | English (en) | Vietnamese (vi) |
|-----|--------------|-----------------|
| `MAINMENU_TITLE` | Solar Phobia | Solar Phobia |
| `MAINMENU_SUBTITLE` | Nắng Gắt | Nắng Gắt |
| `MAINMENU_BUTTON_NEWGAME` | New Game | Bắt Đầu Mới |
| `MAINMENU_BUTTON_CONTINUE` | Continue | Tiếp Tục |
| `MAINMENU_BUTTON_CONTINUE_NOSAVE` | — | — |
| `MAINMENU_BUTTON_SETTINGS` | Settings | Cài Đặt |
| `MAINMENU_BUTTON_CREDITS` | Credits | Đóng Góp |
| `MAINMENU_BUTTON_QUIT` | Quit | Thoát |
| `SETTINGS_TAB_AUDIO` | Audio | Âm Thanh |
| `SETTINGS_TAB_VIDEO` | Video | Hình Ảnh |
| `SETTINGS_TAB_INPUTS` | Controls | Điều Khiển |
| `SETTINGS_LABEL_MASTERVOLUME` | Master Volume | Âm Lượng Chính |
| `SETTINGS_LABEL_MUSICVOLUME` | Music Volume | Âm Lượng Nhạc |
| `SETTINGS_LABEL_SFXVOLUME` | SFX Volume | Âm Lượng Hiệu Ứng |
| `SETTINGS_LABEL_AMBIENTVOLUME` | Ambient Volume | Âm Lượng Môi Trường |
| `SETTINGS_LABEL_SUBTITLES` | Subtitles | Phụ Đề |
| `SETTINGS_LABEL_TEXTSIZE` | Text Size | Cỡ Chữ |
| `SETTINGS_LABEL_RESOLUTION` | Resolution | Độ Phân Giải |
| `SETTINGS_LABEL_WINDOWMODE` | Window Mode | Chế Độ Cửa Sổ |
| `SETTINGS_LABEL_QUALITY` | Quality | Chất Lượng |
| `SETTINGS_LABEL_VSYNC` | V-Sync | V-Sync |
| `SETTINGS_LABEL_CAMERASHAKE` | Camera Shake | Rung Camera |
| `SETTINGS_LABEL_MOTIONBLUR` | Motion Blur | Chuyển Động Mờ |
| `SETTINGS_LABEL_UISCALE` | UI Scaling | Tỷ Lệ UI |
| `SETTINGS_LABEL_INPUTDEVICE` | Input Device | Thiết Bị Nhập |
| `SETTINGS_LABEL_INvertyAXIS` | Invert Y-Axis | Đảo Trục Y |
| `SETTINGS_LABEL_GAMEPADVIBRATION` | Gamepad Vibration | Rung Gamepad |
| `CREDITS_TITLE` | Credits | Đóng Góp |
| `CREDITS_BUTTON_BACK` | Back | Quay Lại |
| `QUIT_TITLE` | Quit to Desktop? | Thoát Ra Desktop? |
| `QUIT_MESSAGE` | Your progress is auto-saved. | Tiến trình của bạn được lưu tự động. |
| `QUIT_BUTTON_CONFIRM` | Confirm Quit | Xác Nhận Thoát |
| `QUIT_BUTTON_CANCEL` | Cancel | Hủy |

### 7.2 Dynamic Strings

| String | Source | Localization Key |
|--------|--------|-----------------|
| Continue (Day X) | Save metadata | `MAINMENU_CONTINUE_DAYFORMAT` |
| Version number | Build info | `MAINMENU_VERSION` (format: "v{0}") |

---

## 8. Animation Specifications

### 8.1 Menu Transitions

| Transition | Duration | Easing | Description |
|------------|----------|--------|-------------|
| MainMenu → Settings | 350ms | ease-out | Slide left, fade in |
| MainMenu → Credits | 350ms | ease-out | Fade out, fade in |
| Settings → MainMenu | 300ms | ease-in | Slide right, fade out |
| Credits → MainMenu | 300ms | ease-in | Fade out, fade in |
| Submenu open | 250ms | ease-out | Scale from 0.95 to 1.0 + fade in |
| Submenu close | 200ms | ease-in | Scale from 1.0 to 0.95 + fade out |

### 8.2 Entry Animations

**Title and Menu Items**:
- **Staggered fade-in**: Title (0ms) → first button (100ms) → subsequent buttons (+50ms each)
- **Duration**: 400ms per item
- **Easing**: ease-out
- **Initial state**: opacity 0, translateY 20px
- **Final state**: opacity 1, translateY 0

### 8.3 Hover/Focus Animations

| State | Animation | Duration | Properties |
|-------|-----------|----------|------------|
| Hover (mouse) | Scale + color | 200ms | scale 1.02, color → Ochre |
| Pressed | Scale down | 100ms | scale 0.98 |
| Gamepad Focus | Border pulse | 150ms (loop) | border-width 3px, border-color pulse |
| Disabled | Opacity | 200ms | opacity 0.5 |

### 8.4 Confirm/Quit Dialog

- **Dialog appears**: 200ms fade-in + scale from 0.9 to 1.0
- **Dialog closes**: 150ms fade-out + scale to 0.95
- **Background**: Dim to 50% black overlay

---

## 9. Screen States

### 9.1 Button States

| State | Visual | Interaction |
|-------|--------|-------------|
| **Normal** | Default color, no border | Click/A-press to activate |
| **Hover** | Ochre Gold text, slight scale 1.02 | Click/A-press to activate |
| **Pressed** | Scale 0.98, darker shade | — |
| **Focused** | Thick border (gamepad), normal text | Enter/A to activate |
| **Disabled** | 50% opacity, grayscale, no interaction | No input accepted |

### 9.2 Toggle States

| State | Visual | Example |
|-------|--------|---------|
| **On** | Filled checkbox, Ochre tint | Subtitles, VSync |
| **Off** | Empty checkbox outline | Motion Blur |
| **Disabled** | Grayed out, 50% opacity | — |

### 9.3 Slider States

| State | Visual |
|-------|--------|
| **Normal** | Track with filled portion |
| **Hover** | Track highlight, larger thumb |
| **Dragging** | Thumb enlarged, value displays live |
| **Disabled** | Gray track, no interaction |

---

## 10. Save/Load Behavior

### 10.1 Continue Button Visibility

```
IF saveFile exists AND saveFile.isValid:
    Continue button: ENABLED (normal)
    Display text: "Continue (Day X)" where X = saveFile.dayNumber
ELSE:
    Continue button: DISABLED (grayed out, unclickable)
    Display text: "Continue" (or hide completely)
```

### 10.2 Save File Location

| Platform | Path |
|----------|------|
| Windows | `%APPDATA%/SolarPhobia/saves/` |
| Editor | `Assets/_Project/_Data/Saves/` |

### 10.3 New Game Behavior

- Confirm if save exists and player chooses New Game (overwrite warning)
- Transition to `PhaseState.DayService`
- Initialize fresh save file

### 10.4 Continue Behavior

- Load save file data
- Resume from saved phase state
- Transition to appropriate scene

---

## 11. Technical Implementation Notes

### 11.1 UI Toolkit Structure

```
Assets/
└── _Project/
    └── Presentation/
        └── MainMenu/
            ├── Toolkit/
            │   ├── MainMenuView.uxml      # Root UXML
            │   ├── MainMenuView.uss       # Styles
            │   ├── SettingsView.uxml     # Settings panel
            │   ├── SettingsView.uss      # Settings styles
            │   ├── CreditsView.uxml      # Credits panel
            │   ├── CreditsView.uss      # Credits styles
            │   └── ConfirmDialog.uxml    # Shared dialog template
            ├── Scripts/
            │   ├── MainMenuController.cs  # Main controller
            │   ├── SettingsController.cs # Settings logic
            │   ├── CreditsController.cs  # Credits logic
            │   └── MainMenuViewModel.cs  # Data binding VM
            └── Assets/
                └── (sprites, fonts, etc.)
```

### 11.2 USS Class Naming Convention

Follow existing HUD pattern:
- `.mainmenu-container` — Root container
- `.mainmenu-title` — Title text
- `.menu-button` — Button base class
- `.menu-button--primary` — New Game (default)
- `.menu-button--secondary` — Other actions
- `.menu-button--danger` — Quit (red/orange)
- `.menu-button--disabled` — Disabled state
- `.menu-button--hover` — Hover state (via USS pseudo-class)
- `.menu-button--focused` — Gamepad focus (via USS pseudo-class)

### 11.3 Event System

The UI NEVER modifies game state directly. All user actions fire events:

| Event | Payload | Handler |
|-------|----------|---------|
| `NewGameClicked` | None | GameController.StartNewGame |
| `ContinueClicked` | SaveFile save | GameController.LoadGame |
| `SettingsClicked` | None | UI.ShowSettingsPanel |
| `CreditsClicked` | None | UI.ShowCreditsPanel |
| `QuitClicked` | None | UI.ShowQuitConfirmDialog |
| `QuitConfirmed` | None | Application.Quit |
| `SettingChanged` | SettingKey, Value | SettingsManager.ApplySetting |

---

## 12. Acceptance Criteria

### 12.1 Functional Requirements

- [ ] MainMenu displays within 1 second of game launch
- [ ] Continue button enabled only when valid save exists
- [ ] Settings changes persist to PlayerPrefs and apply immediately
- [ ] ESC closes any submenu, returns to parent screen
- [ ] ESC on MainMenu shows Quit Confirmation
- [ ] Gamepad navigation works: D-pad moves focus, A selects, B cancels
- [ ] Keyboard: Tab navigates, Enter selects, Escape cancels
- [ ] Mouse hover provides visual feedback
- [ ] All text displays via localization system (no hardcoded strings)

### 12.2 Visual Requirements

- [ ] Day palette applied (#F5F0E8 background, #C45A30 accents, #D4A03A selection)
- [ ] Hand-drawn font style applied (Caveat or similar)
- [ ] Rounded container edges (per Art Bible)
- [ ] Hover animations: scale + color change within 200ms
- [ ] Entry animations: staggered fade-in on menu load
- [ ] Focus indicators visible for gamepad (thick border)
- [ ] All text meets WCAG AA contrast (4.5:1 minimum)

### 12.3 Accessibility Requirements

- [ ] Minimum 14px body text, 18px headings
- [ ] 100%-200% UI scaling supported
- [ ] Colorblind backup: shapes/icons for all color-coded states
- [ ] Focus indicators visible for both keyboard and gamepad
- [ ] Subtitle toggle works
- [ ] Gamepad vibration toggle works

### 12.4 Platform Requirements

- [ ] Works on Windows 10/11 at 1920x1080 and higher
- [ ] Mouse + keyboard fully functional
- [ ] Xbox/Generic gamepad fully functional
- [ ] ESC key mapped correctly

---

## 13. Dependencies

| System | Dependency Type | Notes |
|--------|-----------------|-------|
| `GameStateManager` | Runtime | Detect current phase for Continue |
| `SaveSystem` | Runtime | Read/write save files |
| `LocalizationService` | Runtime | All text display |
| `SettingsManager` | Runtime | Audio/Video/Input settings |
| `InputSystem` | Runtime | Keyboard/mouse + gamepad detection |
| `PhaseState` enum | Data | Phase state definitions |

---

## 14. Revision History

| Date | Version | Changes |
|------|---------|---------|
| 2026-05-08 | 1.0.0 | Initial draft |