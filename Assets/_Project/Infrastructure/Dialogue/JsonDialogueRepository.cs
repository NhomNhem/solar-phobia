using System;
using NhemDangFugBixs.NhemLogging;
using Newtonsoft.Json;
using SolarPhobia.Domain;
using SolarPhobia.Domain.Repositories;
using UnityEngine;

namespace TinyMonsterArena.Infrastructure.Dialogue
{
    /// <summary>
    /// Loads dialogue JSON files from `Resources/Dialogues/{dialogueId}.json` and deserializes into Domain.DialogueNode.
    /// Place dialogue files under `Assets/Resources/Dialogues/`.
    /// </summary>
    public class JsonDialogueRepository : IDialogueRepository
    {
        private static readonly INhemLogger Logger = new NhemUnityLogger();

        public DialogueNode GetDialogue(string dialogueId)
        {
            if (string.IsNullOrWhiteSpace(dialogueId))
            {
                return null;
            }

            try
            {
                var textAsset = Resources.Load<TextAsset>("Dialogues/" + dialogueId);
                if (textAsset == null)
                {
                    Logger.LogWarning($"Dialogue JSON not found for id: {dialogueId}");
                    return null;
                }

                var node = JsonConvert.DeserializeObject<DialogueNode>(textAsset.text);
                return node;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to load dialogue {dialogueId}: {ex}");
                return null;
            }
        }
    }
}
