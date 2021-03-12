using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HFN.Common;
using HFN.Common.AddressableAssets;

namespace HFN.Civics
{
    public class CategoryButton : MonoBehaviour, IAddressableAsset
    {
        [SerializeField]
        private RectTransform rectTransform = null;
        [SerializeField]
        private TextMeshProUGUI categoryText = null;
        [SerializeField]
        private Image primaryImage = null;
        [SerializeField]
        private Image secondaryImage = null;
        [SerializeField]
        private Image checkMark = null;
        [SerializeField]
        private TweenPosition checkMarkBounce = null;
        [SerializeField]
        private Button button = null;
        [SerializeField]
        private GameViewId categoryViewId = GameViewId.None;
        [SerializeField]
        private List<Image> starImageList = new List<Image>();

        private int currentStars = 0;
        private Sprite currentStarSprite = null;
        private CategoryData currentData = null;
        private PresidentialPowerView powerView = null;

        public string ID { get { return currentData.buttonText; } }

        public int CurrentStars
        {
            get { return currentStars; }
            set
            {
                currentStars = value;

                for(int i = 0; i < starImageList.Count; i++)
                {
                    if (i < currentStars)
                    {
                        starImageList[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        starImageList[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        private PresidentialPowerView PowerView
        {
            get
            {
                if (!powerView) powerView = AddressableUIManager.GetView<PresidentialPowerView>();
                return powerView;
            }
        }

        public void Initilize(CategoryData data/*, int stars*/)
        {
            currentData = data;
            //CurrentStars = stars;
            currentStarSprite = PowerView.ColorStar;
            categoryText.text = data.buttonText;

            /*for (int i = 0; i < starImageList.Count; i++)
            {
                starImageList[i].sprite = currentStarSprite;
            }*/

            if (data.imageSelector.atlas.SubObjectName != "")
            {
                data.imageSelector.GetSprite(OnPrimaryImageLoaded);
            }
            else
            {
                primaryImage.gameObject.SetActive(false);
            }

            if (data.secondImageSelector.atlas.SubObjectName != "")
            {
                data.secondImageSelector.GetSprite(OnSecondaryImageLoaded);
            }
            else
            {
                secondaryImage.gameObject.SetActive(false);
            }

            CheckVisited();

            rectTransform.anchorMax = data.buttonAnchors;
            rectTransform.anchorMin = data.buttonAnchors;

            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
        }


        public void ChangeStarColor (PanelData currentPanel)
        {
            secondaryImage.sprite = currentStarSprite;
        }


        public void CheckVisited()
        {
            if (currentData.visited && !checkMark.isActiveAndEnabled)
            {
                checkMark.gameObject.SetActive(true);
                checkMarkBounce.StartTween(true);
                //secondaryImage.sprite = currentStarSprite;
            }
            else if(!currentData.visited)
            {
                checkMark.gameObject.SetActive(false);
            }
        }


        public void CleanUp()
        {
            Release();

            if (checkMark.isActiveAndEnabled)
            {
                checkMarkBounce.StopTween(true);
                checkMarkBounce.ForceStartValue(true);
            }

            primaryImage.gameObject.SetActive(true);
            secondaryImage.gameObject.SetActive(true);
        }


        private void OnPrimaryImageLoaded(Sprite sprite)
        {
            primaryImage.sprite = sprite;
        }


        private void OnSecondaryImageLoaded(Sprite sprite)
        {
            //if (currentData.visited)
            //{
            //    secondaryImage.sprite = currentStarSprite;
            //}
            if(!currentData.visited)
            {
                secondaryImage.sprite = sprite;
            }
        }


        private void OnCategoryViewLoaded(PresidentialCategoryView view)
        {
            view.Initialize(currentData);
        }


        private void OnButtonClicked()
        {
            AddressableUIManager.GetViewAsync<PresidentialCategoryView>(categoryViewId, OnCategoryViewLoaded);
        }


        public void Release()
        {
            currentData.imageSelector.Release();
            currentData.secondImageSelector.Release();

            for(int i = 0; i < starImageList.Count; i++)
            {
                starImageList[i].sprite = null;
                starImageList[i].gameObject.SetActive(false);
            }
        }


        private void Start()
        {
            button.onClick.AddListener(OnButtonClicked);
;        }
    }
}
