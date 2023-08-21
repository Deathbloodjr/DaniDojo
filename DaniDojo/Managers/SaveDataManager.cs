using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using DaniDojo.Data;

namespace DaniDojo.Managers
{
    internal class SaveDataManager
    {
        const string SaveFileName = "DaniSave.json";
        const string TmpSaveFileName = "TmpSave.json";
        const string OldSaveFileName = "dansave.json";

        static DaniSaveData SaveData { get; set; }

        #region Loading
        public static void LoadSaveData()
        {
            Plugin.LogInfo("LoadSaveData Start", true);
            SaveData = new DaniSaveData(); // I'm not sure if this line is actually needed, or even detrimental
            SaveData = LoadSaveData(Plugin.Instance.ConfigDaniDojoSaveLocation.Value);
            Plugin.LogInfo("LoadSaveData Finished", true);
        }

        static DaniSaveData LoadSaveData(string folderLocation)
        {
            DaniSaveData data = new DaniSaveData();
            // Check for old save first
            // If it is old save, save it as the new save type, and delete the old save?
            // This is assuming there won't be a new save and old save
            if (File.Exists(Path.Combine(folderLocation, SaveFileName)))
            {
                var node = JsonNode.Parse(File.ReadAllText(Path.Combine(folderLocation, SaveFileName)));
                data = LoadSaveFile(node);
            }
            else if (File.Exists(Path.Combine(folderLocation, TmpSaveFileName)))
            {
                var node = JsonNode.Parse(File.ReadAllText(Path.Combine(folderLocation, TmpSaveFileName)));
                data = LoadSaveFile(node);
                SaveSaveData(data);
            }
            else if (File.Exists(Path.Combine(folderLocation, OldSaveFileName)))
            {
                var node = JsonNode.Parse(File.ReadAllText(Path.Combine(folderLocation, OldSaveFileName)));
                data = LoadOldSaveFile(node);
            }
            

            return data;
        }

        static DaniSaveData LoadSaveFile(JsonNode node)
        {
            DaniSaveData data = new DaniSaveData();


            return data;
        }

        static DaniSaveData LoadOldSaveFile(JsonNode node)
        {
            DaniSaveData data = new DaniSaveData();


            return data;
        }
        #endregion


        #region Saving

        static void SaveSaveData()
        {

        }

        static void SaveSaveData(DaniSaveData saveData)
        {

        }

        #endregion

    }
}
