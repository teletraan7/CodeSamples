using UnityEngine.UI;
using TMPro;

namespace HFN.Citizin
{
    [System.Serializable]
    public class FactionData
    {
        public FactionDefinition factionType = FactionDefinition.None;
        public FactionController factionController = null;

        public FactionData(FactionDefinition faction, FactionController controller)
        {

        }
    }
}