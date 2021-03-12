using System.Collections;
using UnityEngine.U2D;
using System.Collections.Generic;
using UnityEngine;
using HFN.Common;
using HFN.Common.AddressableAssets;

namespace HFN.Civics
{
    [System.Serializable]
    public class PanelData
    {
        private const string TITLE = "Title";
        private const string INFO_DESCRIPTION = "Info Description";
        private const string NEXT_TITLE = "Next Title";
        private const string PREVIOUS_TITLE = "Previous Title";
        private const string DESCRIPTOR1 = "Descriptor 1";
        private const string DESCRIPTOR2 = "Descriptor 2";
        private const string INFO_STAR = "Info Star Image";
        private const string BACKGROUND_IMAGE = "Background Image";

        public string title = "";
        public string infoDescription = "";
        public string nextTitle = "";
        public string previousTitle = "";
        public string description1 = "";
        public string description2 = "";
        public AddressableSpriteSelector starImageSelector = new AddressableSpriteSelector();
        public AddressableSpriteSelector backgroundImageSelector = new AddressableSpriteSelector();

        public List<CategoryData> categoryData = new List<CategoryData>();

        public PanelData(Dictionary<string, string> data, TextAsset categoryAsset, SpriteAtlas iconAtlas, SpriteAtlas peopleAtlas, SpriteAtlas backgroundAtlas)
        {
            title = data[TITLE];
            infoDescription = data[INFO_DESCRIPTION];
            nextTitle = data[NEXT_TITLE];
            previousTitle = data[PREVIOUS_TITLE];
            description1 = data[DESCRIPTOR1];
            description2 = data[DESCRIPTOR2];

            if(iconAtlas != default(SpriteAtlas) && data[INFO_STAR] != "")
            {
#if UNITY_EDITOR
                starImageSelector.atlas.SetEditorAsset(iconAtlas);
#endif
                starImageSelector.atlas.SubObjectName = data[INFO_STAR];
            }

            if(backgroundAtlas != default(SpriteAtlas) && data[BACKGROUND_IMAGE] != "")
            {
#if UNITY_EDITOR
                backgroundImageSelector.atlas.SetEditorAsset(backgroundAtlas);
#endif
                backgroundImageSelector.atlas.SubObjectName = data[BACKGROUND_IMAGE];
            }

            List<Dictionary<string, string>> categoryCsv = CsvParser.Parse(categoryAsset.text);

            for(int i = 0; i < categoryCsv.Count; i++)
            {
                categoryData.Add(new CategoryData(categoryCsv[i], iconAtlas, peopleAtlas));
            }
        }
    }
}

