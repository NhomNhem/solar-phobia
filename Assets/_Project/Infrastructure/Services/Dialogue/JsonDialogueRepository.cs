using Newtonsoft.Json;
using SolarPhobia.Application.Services.Interfaces;
using SolarPhobia.Domain.Repositories;
using SolarPhobia.Domain;
using System;
using UnityEngine;

namespace SolarPhobia.Infrastructure.Services.Dialogue
{
    public class JsonDialogueRepository : IDialogueRepository
    {
        public DialogueNode GetDialogue(string dialogueId)
        {
            if (string.IsNullOrWhiteSpace(dialogueId)) return null;

            var textAsset = Resources.Load<TextAsset>("Dialogues/" + dialogueId);
            if (textAsset == null)
            {
                Debug.LogWarning($"Dialogue JSON not found for id: {dialogueId}");
                return null;
            }

            try
            {
                var node = JsonConvert.DeserializeObject<DialogueNode>(textAsset.text);
                return node;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load dialogue {dialogueId}: {ex}");
                return null;
            }
        }
    }
}
