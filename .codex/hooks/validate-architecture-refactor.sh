#!/bin/bash
# Codex PostToolUse hook: advisory architecture checks during refactor
# Exit 0 always. This hook warns when edited files drift from the target layering rules.
#
# Input schema (PostToolUse for Write/Edit):
# { "tool_name": "Write", "tool_input": { "file_path": "Assets/_Project/Domain/Foo.cs", "content": "..." } }

INPUT=$(cat)

if command -v jq >/dev/null 2>&1; then
    FILE_PATH=$(echo "$INPUT" | jq -r '.tool_input.file_path // empty')
else
    FILE_PATH=$(echo "$INPUT" | grep -oE '"file_path"[[:space:]]*:[[:space:]]*"[^"]*"' | sed 's/"file_path"[[:space:]]*:[[:space:]]*"//;s/"$//')
fi

FILE_PATH=$(echo "$FILE_PATH" | sed 's|\\|/|g')

if [ -z "$FILE_PATH" ] || [ ! -f "$FILE_PATH" ]; then
    exit 0
fi

case "$FILE_PATH" in
    *Assets/_Project/*.cs) ;;
    *) exit 0 ;;
esac

WARNINGS=""

is_domain=0
is_application=0
is_runtime_layer=0

if echo "$FILE_PATH" | grep -qE 'Assets/_Project/(01_Domain|Domain)/'; then
    is_domain=1
fi

if echo "$FILE_PATH" | grep -qE 'Assets/_Project/(02_Application|Application)/'; then
    is_application=1
fi

if echo "$FILE_PATH" | grep -qE 'Assets/_Project/(02_Application|Application|03_Infrastructure|Infrastructure|04_Presentation|Presentation|05_Composition|Composition)/'; then
    is_runtime_layer=1
fi

if [ "$is_domain" -eq 1 ]; then
    if grep -qE 'using[[:space:]]+UnityEngine|MonoBehaviour|ScriptableObject|Transform|GameObject|Debug\.' "$FILE_PATH"; then
        WARNINGS="$WARNINGS\n  DOMAIN: $FILE_PATH references Unity runtime types. Domain must stay engine-free."
    fi

    if grep -qE 'using[[:space:]]+R3|ReactiveProperty|ReadOnlyReactiveProperty|Subject<' "$FILE_PATH"; then
        WARNINGS="$WARNINGS\n  DOMAIN: $FILE_PATH uses R3. Reactive state belongs outside Domain."
    fi

    if grep -qE 'ObservableList<|ObservableDictionary<|ObservableQueue<|ObservableHashSet<|using[[:space:]]+ObservableCollections' "$FILE_PATH"; then
        WARNINGS="$WARNINGS\n  DOMAIN: $FILE_PATH uses ObservableCollections. Domain must expose plain contracts and collections."
    fi

    if grep -qE 'using[[:space:]]+MessagePipe|IPublisher<|ISubscriber<|IAsyncPublisher<|IAsyncSubscriber<' "$FILE_PATH"; then
        WARNINGS="$WARNINGS\n  DOMAIN: $FILE_PATH uses MessagePipe. Domain events should stay package-agnostic."
    fi
fi

if [ "$is_application" -eq 1 ]; then
    if grep -qE ':[[:space:]]*MonoBehaviour|PlayerPrefs|AudioListener|QualitySettings|Camera|FindFirstObjectByType|FindObjectsByType|FindAnyObjectByType' "$FILE_PATH"; then
        WARNINGS="$WARNINGS\n  APPLICATION: $FILE_PATH contains Unity runtime/view concerns. Move adapters to Infrastructure or Presentation."
    fi

    if grep -qE 'Debug\.(Log|LogWarning|LogError|Assert)' "$FILE_PATH"; then
        WARNINGS="$WARNINGS\n  APPLICATION: $FILE_PATH logs directly through UnityEngine.Debug. Prefer ports/adapters or project logging abstraction."
    fi

    if grep -qE 'ObservableList<|ObservableDictionary<|ObservableQueue<|ObservableHashSet<' "$FILE_PATH"; then
        WARNINGS="$WARNINGS\n  APPLICATION: $FILE_PATH uses ObservableCollections. Keep this only when collection delta streams are genuinely required."
    fi
fi

if grep -qE 'Debug\.(Log|LogWarning|LogError|Assert)' "$FILE_PATH"; then
    WARNINGS="$WARNINGS\n  LOGGING: $FILE_PATH uses UnityEngine.Debug directly. Prefer INhemLogger/NhemUnityLogger according to project logging policy."
fi

if [ "$is_runtime_layer" -eq 1 ]; then
    if grep -qE 'new[[:space:]]+NhemUnityLogger\(' "$FILE_PATH"; then
        if ! echo "$FILE_PATH" | grep -qE 'Assets/_Project/(Composition/Installers/GameplayBalanceConfigLoader\.cs|Infrastructure/Hazards/|Infrastructure/Dialogue/JsonDialogueRepository\.cs)'; then
            WARNINGS="$WARNINGS\n  LOGGING: $FILE_PATH constructs NhemUnityLogger directly. DI-managed classes should inject INhemLogger and keep direct construction only in static loaders or non-DI MonoBehaviours."
        fi
    fi
fi

if grep -qE 'ObservableList<|ObservableDictionary<|ObservableQueue<|ObservableHashSet<' "$FILE_PATH"; then
    if grep -qE 'public[[:space:]].*(ObservableList<|ObservableDictionary<|ObservableQueue<|ObservableHashSet<)' "$FILE_PATH"; then
        WARNINGS="$WARNINGS\n  API: $FILE_PATH exposes ObservableCollections in a public API. Prefer IReadOnlyCollection/IReadOnlyDictionary/plain DTOs across layers."
    fi
fi

if grep -qE 'using[[:space:]]+ZLinq|global using[[:space:]]+ZLinq|AsValueEnumerable|ToValueEnumerable' "$FILE_PATH"; then
    if echo "$FILE_PATH" | grep -qE '/(Tests|Editor/Tests|Presentation|02_Application|Application)/'; then
        WARNINGS="$WARNINGS\n  ZLINQ: $FILE_PATH uses ZLinq in a non-hot-path area. Default to plain loops or System.Linq unless profiling justifies it."
    fi
fi

if [ -n "$WARNINGS" ]; then
    echo -e "=== Architecture Refactor Warnings ===$WARNINGS\n======================================" >&2
    echo "Run a Unity MCP compile/console check after the current refactor batch." >&2
fi

exit 0
