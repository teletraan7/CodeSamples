using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using HFN.Common.AddressableAssets;

namespace HFN.Civics
{
    [System.Serializable]
    public class CategoryData
    {
        private const string TITLE = "Title";
        private const string BUTTON_TEXT = "Button Text";
        private const string IMAGE = "Image";
        private const string SECOND_IMAGE = "Second Image";
        private const string MODAL_TITLE = "Modal Title";
        private const string TAG_LINE = "Tag Line";
        private const string OPPOSITION = "Opposition";
        private const string RESPONSE = "Response";
        private const string ANCHORX = "AnchorX";
        private const string ANCHORY = "AnchorY";

        public string title = "";
        public string buttonText = "";
        public string modalTitle = "";
        public string tagLine = "";
        public string opposition = "";
        public string response = "";
        public bool visited = false;
        public Vector2 buttonAnchors = Vector2.zero;
        public AddressableSpriteSelector imageSelector = new AddressableSpriteSelector();
        public AddressableSpriteSelector secondImageSelector = new AddressableSpriteSelector();

        public CategoryData(Dictionary<string, string> data, SpriteAtlas iconAtlas, SpriteAtlas peopleAtlas)
        {
            title = data[TITLE];
            buttonText = data[BUTTON_TEXT];
            modalTitle = data[MODAL_TITLE];
            tagLine = data[TAG_LINE];
            opposition = data[OPPOSITION];
            response = data[RESPONSE];
            buttonAnchors.x = float.Parse(data[ANCHORX]);
            buttonAnchors.y = float.Parse(data[ANCHORY]);

            if(iconAtlas != default(SpriteAtlas) && data[SECOND_IMAGE] != "")
            {
#if UNITY_EDITOR
                secondImageSelector.atlas.SetEditorAsset(iconAtlas);
#endif
                secondImageSelector.atlas.SubObjectName = data[SECOND_IMAGE];
            }

            if (peopleAtlas != default(SpriteAtlas) && data[IMAGE] != "")
            {
#if UNITY_EDITOR
                imageSelector.atlas.SetEditorAsset(peopleAtlas);
#endif
                imageSelector.atlas.SubObjectName = data[IMAGE];
            }
        }
    }
}
