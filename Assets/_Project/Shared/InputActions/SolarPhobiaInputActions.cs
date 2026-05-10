// Assets/_Project/Shared/InputActions/SolarPhobiaInputActions.cs
// Strongly-typed wrapper for SolarPhobiaInputActions.inputactions
// Mirrors the structure Unity generates when "Generate C# Class" is enabled on the asset.
// If you enable auto-generation in the Inspector, delete this file and use the generated one.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace SolarPhobia.Shared.InputActions
{
    /// <summary>
    /// Strongly-typed wrapper for SolarPhobiaInputActions.inputactions.
    /// Provides Player and UI action maps for use by PlayerController and UI systems.
    ///
    /// Usage:
    ///   var actions = new SolarPhobiaInputActions();
    ///   actions.Player.Enable();
    ///   var moveInput = actions.Player.Move.ReadValue&lt;Vector2&gt;();
    ///   bool sprintHeld = actions.Player.Sprint.IsPressed();
    /// </summary>
    public class SolarPhobiaInputActions : IDisposable
    {
        // ── Asset ──────────────────────────────────────────────────
        private readonly InputActionAsset _asset;

        // ── Action Maps ────────────────────────────────────────────
        private PlayerActions _player;
        private UIActions _ui;

        // ── Constructor ────────────────────────────────────────────
        public SolarPhobiaInputActions()
        {
            _asset = InputActionAsset.FromJson(k_AssetJson);
            _player = new PlayerActions(this);
            _ui     = new UIActions(this);
        }

        // ── Public Properties ──────────────────────────────────────
    /// <summary>Player action map — Move, Sprint, Interact, Look.</summary>
    public PlayerActions Player => _player;

        /// <summary>UI action map — Click, Navigate, Submit, Cancel.</summary>
        public UIActions UI => _ui;

        /// <summary>Underlying InputActionAsset for advanced usage.</summary>
        public InputActionAsset Asset => _asset;

        // ── Enable / Disable ───────────────────────────────────────
        /// <summary>Enable all action maps.</summary>
        public void Enable()  => _asset.Enable();

        /// <summary>Disable all action maps.</summary>
        public void Disable() => _asset.Disable();

        // ── IDisposable ────────────────────────────────────────────
        public void Dispose()
        {
            UnityEngine.Object.Destroy(_asset);
        }

        // ── Player Action Map ──────────────────────────────────────
        /// <summary>
        /// Player action map: Move (Vector2), Sprint (Button), Interact (Button), Look (Vector2).
        /// Enable/disable independently from the UI map.
        /// </summary>
        public class PlayerActions
        {
            private readonly InputActionMap _map;

            /// <summary>WASD / left stick — horizontal movement input.</summary>
            public InputAction Move     { get; }

            /// <summary>Left Shift / South button — sprint hold.</summary>
            public InputAction Sprint   { get; }

            /// <summary>E key / West button — contextual interact.</summary>
            public InputAction Interact { get; }

            /// <summary>Mouse delta / right stick — camera look input (Y-axis only).</summary>
            public InputAction Look     { get; }

            internal PlayerActions(SolarPhobiaInputActions wrapper)
            {
                _map     = wrapper._asset.FindActionMap("Player", throwIfNotFound: true);
                Move     = _map.FindAction("Move",     throwIfNotFound: true);
                Sprint   = _map.FindAction("Sprint",   throwIfNotFound: true);
                Interact = _map.FindAction("Interact", throwIfNotFound: true);
                Look     = _map.FindAction("Look",     throwIfNotFound: true);
            }

            /// <summary>Enable the Player action map.</summary>
            public void Enable()  => _map.Enable();

            /// <summary>Disable the Player action map.</summary>
            public void Disable() => _map.Disable();

            public static implicit operator InputActionMap(PlayerActions set) => set._map;
        }

        // ── UI Action Map ──────────────────────────────────────────
        /// <summary>
        /// UI action map: Click, Navigate, Submit, Cancel.
        /// Active during DayService and other non-Night phases.
        /// </summary>
        public class UIActions
        {
            private readonly InputActionMap _map;

            /// <summary>Left mouse button / South button — UI click.</summary>
            public InputAction Click    { get; }

            /// <summary>Arrow keys / left stick — UI navigation.</summary>
            public InputAction Navigate { get; }

            /// <summary>Enter / South button — confirm.</summary>
            public InputAction Submit   { get; }

            /// <summary>Escape / East button — cancel / back.</summary>
            public InputAction Cancel   { get; }

            internal UIActions(SolarPhobiaInputActions wrapper)
            {
                _map     = wrapper._asset.FindActionMap("UI", throwIfNotFound: true);
                Click    = _map.FindAction("Click",    throwIfNotFound: true);
                Navigate = _map.FindAction("Navigate", throwIfNotFound: true);
                Submit   = _map.FindAction("Submit",   throwIfNotFound: true);
                Cancel   = _map.FindAction("Cancel",   throwIfNotFound: true);
            }

            /// <summary>Enable the UI action map.</summary>
            public void Enable()  => _map.Enable();

            /// <summary>Disable the UI action map.</summary>
            public void Disable() => _map.Disable();

            public static implicit operator InputActionMap(UIActions set) => set._map;
        }

        // ── Embedded Asset JSON ────────────────────────────────────
        private const string k_AssetJson = @"{
    ""name"": ""SolarPhobiaInputActions"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""a1b2c3d4-0001-0001-0001-000000000001"",
            ""actions"": [
                { ""name"": ""Move"",     ""type"": ""Value"",  ""id"": ""a1b2c3d4-0001-0002-0001-000000000001"", ""expectedControlType"": ""Vector2"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": true },
                { ""name"": ""Sprint"",   ""type"": ""Button"", ""id"": ""a1b2c3d4-0001-0003-0001-000000000001"", ""expectedControlType"": ""Button"",  ""processors"": """", ""interactions"": """", ""initialStateCheck"": false },
                { ""name"": ""Interact"", ""type"": ""Button"", ""id"": ""a1b2c3d4-0001-0004-0001-000000000001"", ""expectedControlType"": ""Button"",  ""processors"": """", ""interactions"": """", ""initialStateCheck"": false },
                { ""name"": ""Look"",     ""type"": ""Value"",  ""id"": ""a1b2c3d4-0001-0005-0001-000000000001"", ""expectedControlType"": ""Vector2"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": true }
            ],
            ""bindings"": [
                { ""name"": ""WASD"", ""id"": ""a1b2c3d4-0002-0001-0001-000000000001"", ""path"": ""2DVector"",             ""groups"": """",               ""action"": ""Move"",     ""isComposite"": true,  ""isPartOfComposite"": false },
                { ""name"": ""up"",   ""id"": ""a1b2c3d4-0002-0002-0001-000000000001"", ""path"": ""<Keyboard>/w"",         ""groups"": ""Keyboard&Mouse"", ""action"": ""Move"",     ""isComposite"": false, ""isPartOfComposite"": true },
                { ""name"": ""down"", ""id"": ""a1b2c3d4-0002-0003-0001-000000000001"", ""path"": ""<Keyboard>/s"",         ""groups"": ""Keyboard&Mouse"", ""action"": ""Move"",     ""isComposite"": false, ""isPartOfComposite"": true },
                { ""name"": ""left"", ""id"": ""a1b2c3d4-0002-0004-0001-000000000001"", ""path"": ""<Keyboard>/a"",         ""groups"": ""Keyboard&Mouse"", ""action"": ""Move"",     ""isComposite"": false, ""isPartOfComposite"": true },
                { ""name"": ""right"",""id"": ""a1b2c3d4-0002-0005-0001-000000000001"", ""path"": ""<Keyboard>/d"",         ""groups"": ""Keyboard&Mouse"", ""action"": ""Move"",     ""isComposite"": false, ""isPartOfComposite"": true },
                { ""name"": """",     ""id"": ""a1b2c3d4-0002-0006-0001-000000000001"", ""path"": ""<Gamepad>/leftStick"",  ""groups"": ""Gamepad"",        ""action"": ""Move"",     ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """",     ""id"": ""a1b2c3d4-0003-0001-0001-000000000001"", ""path"": ""<Keyboard>/leftShift"", ""groups"": ""Keyboard&Mouse"", ""action"": ""Sprint"",   ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """",     ""id"": ""a1b2c3d4-0003-0002-0001-000000000001"", ""path"": ""<Gamepad>/buttonSouth"",""groups"": ""Gamepad"",        ""action"": ""Sprint"",   ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """",     ""id"": ""a1b2c3d4-0004-0001-0001-000000000001"", ""path"": ""<Keyboard>/e"",         ""groups"": ""Keyboard&Mouse"", ""action"": ""Interact"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """",     ""id"": ""a1b2c3d4-0004-0002-0001-000000000001"", ""path"": ""<Gamepad>/buttonWest"", ""groups"": ""Gamepad"",        ""action"": ""Interact"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """",     ""id"": ""a1b2c3d4-0009-0001-0001-000000000001"", ""path"": ""<Mouse>/delta"",           ""groups"": ""Keyboard&Mouse"", ""action"": ""Look"",     ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """",     ""id"": ""a1b2c3d4-0009-0002-0001-000000000001"", ""path"": ""<Gamepad>/rightStick"",    ""groups"": ""Gamepad"",        ""action"": ""Look"",     ""isComposite"": false, ""isPartOfComposite"": false }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""a1b2c3d4-0001-0001-0002-000000000001"",
            ""actions"": [
                { ""name"": ""Click"",    ""type"": ""Button"", ""id"": ""a1b2c3d4-0001-0002-0002-000000000001"", ""expectedControlType"": ""Button"",  ""processors"": """", ""interactions"": """", ""initialStateCheck"": false },
                { ""name"": ""Navigate"", ""type"": ""Value"",  ""id"": ""a1b2c3d4-0001-0003-0002-000000000001"", ""expectedControlType"": ""Vector2"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": true },
                { ""name"": ""Submit"",   ""type"": ""Button"", ""id"": ""a1b2c3d4-0001-0004-0002-000000000001"", ""expectedControlType"": ""Button"",  ""processors"": """", ""interactions"": """", ""initialStateCheck"": false },
                { ""name"": ""Cancel"",   ""type"": ""Button"", ""id"": ""a1b2c3d4-0001-0005-0002-000000000001"", ""expectedControlType"": ""Button"",  ""processors"": """", ""interactions"": """", ""initialStateCheck"": false }
            ],
            ""bindings"": [
                { ""name"": """", ""id"": ""a1b2c3d4-0005-0001-0002-000000000001"", ""path"": ""<Mouse>/leftButton"",    ""groups"": ""Keyboard&Mouse"", ""action"": ""Click"",  ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""a1b2c3d4-0005-0002-0002-000000000001"", ""path"": ""<Gamepad>/buttonSouth"", ""groups"": ""Gamepad"",        ""action"": ""Click"",  ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""a1b2c3d4-0007-0001-0002-000000000001"", ""path"": ""<Keyboard>/enter"",      ""groups"": ""Keyboard&Mouse"", ""action"": ""Submit"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""a1b2c3d4-0007-0002-0002-000000000001"", ""path"": ""<Gamepad>/buttonSouth"", ""groups"": ""Gamepad"",        ""action"": ""Submit"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""a1b2c3d4-0008-0001-0002-000000000001"", ""path"": ""<Keyboard>/escape"",     ""groups"": ""Keyboard&Mouse"", ""action"": ""Cancel"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""a1b2c3d4-0008-0002-0002-000000000001"", ""path"": ""<Gamepad>/buttonEast"",  ""groups"": ""Gamepad"",        ""action"": ""Cancel"", ""isComposite"": false, ""isPartOfComposite"": false }
            ]
        }
    ],
    ""controlSchemes"": [
        { ""name"": ""Keyboard&Mouse"", ""bindingGroup"": ""Keyboard&Mouse"", ""devices"": [ { ""devicePath"": ""<Keyboard>"", ""isOptional"": false, ""isOR"": false }, { ""devicePath"": ""<Mouse>"", ""isOptional"": false, ""isOR"": false } ] },
        { ""name"": ""Gamepad"",        ""bindingGroup"": ""Gamepad"",        ""devices"": [ { ""devicePath"": ""<Gamepad>"",   ""isOptional"": false, ""isOR"": false } ] }
    ]
}";
    }
}
