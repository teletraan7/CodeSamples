using UnityEngine;
using HFN.Common.AddressableAssets;

namespace HFN.Citizin
{
    [System.Serializable]
    public class JobData
    {
        public Sprite factionImg = null;
        public string nameText = null;
        public string descriptionText = null;
        public int rewardAmount = 0;
        public FactionDefinition faction = FactionDefinition.None;
        public AddressableSpriteSelector addressableSprite = new AddressableSpriteSelector();
    }
}
