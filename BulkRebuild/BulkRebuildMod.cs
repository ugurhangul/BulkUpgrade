using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BulkRebuild
{
    public class BulkRebuildMod : MelonMod
    {
        private MelonPreferences_Category modCategory;
        private MelonPreferences_Entry<KeyCode> triggerKey;
        private GameManager gameManager;

        /// <inheritdoc/>
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Loading configuration ...");
            modCategory = MelonPreferences.CreateCategory("BulkRebuild");
            modCategory.SetFilePath(Path.Combine(MelonUtils.UserDataDirectory, "BulkRebuild.cfg"));
            triggerKey = modCategory.CreateEntry("TriggerKey", KeyCode.R, null, null, false, false, null, null);
            LoggerInstance.Msg($"KeyCode is CTRL+{triggerKey.Value}");
        }

        /// <inheritdoc/>
        public override void OnUpdate()
        {
            base.OnUpdate();

            try
            {
                if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl) || !Input.GetKeyDown(triggerKey.Value))
                {
                    return;
                }

                // Rebuild all buildings
                BulkRebuild();
            }
            catch (Exception ex)
            {
                LoggerInstance.Error("Failed to rebuild buildings.", ex);
            }
        }

        /// <summary>
        /// Executes rebuild operation on all ruins.
        /// </summary>
        private void BulkRebuild()
        {
            if (gameManager == null)
            {
                gameManager = UnityEngine.Object.FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    LoggerInstance.Msg($"Initialized GameManager");
                }
            }

            if (gameManager != null)
            {
                LoggerInstance.Msg($"Searching for building ruins...");
                var ruins = UnityEngine.Object.FindObjectsOfType<BuildingRuins>();
                LoggerInstance.Msg($"Found {ruins.Count} ruins.");
                foreach (var ruin in ruins)
                {
                    ruin.Rebuild();
                }
            }
        }
    }
}
