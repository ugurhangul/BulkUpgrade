using System;
using System.IO;
using System.Linq;
using Il2Cpp;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;

namespace BulkUpgrade
{
    public class BulkUpgradeMod : MelonMod
    {
        private MelonPreferences_Category modCategory;
        private MelonPreferences_Entry<KeyCode> triggerKey;
        private GameManager gameManager;

        /// <inheritdoc/>
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Loading configuration ...");
            modCategory = MelonPreferences.CreateCategory("BulkUpgrade");
            modCategory.SetFilePath(Path.Combine(MelonEnvironment.UserDataDirectory, "BulkUpgrade.cfg"));
            triggerKey = modCategory.CreateEntry("TriggerKey", KeyCode.U, null, null, false, false, null, null);
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
                LoggerInstance.Error("Failed to upgrade buildings.", ex);
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
                LoggerInstance.Msg($"Searching for building upgrade...");
                var buildings = UnityEngine.Object.FindObjectsOfType<Building>();
                LoggerInstance.Msg($"Found {buildings.Count(c=>c.buildingUpgradeInfo != null && c.buildingUpgradeInfo.canUpgradeNow)} buildings.");
                foreach (var ruin in buildings)
                {
                    if (ruin.buildingUpgradeInfo != null && ruin.buildingUpgradeInfo.canUpgradeNow)
                    {
                        ruin.UpgradeBuilding();
                    }
                    
                }
            }
        }
    }
}
