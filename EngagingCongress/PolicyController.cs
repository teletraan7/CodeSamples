using System.Collections.Generic;
using UnityEngine;
using HFN.Common;
using UnityEngine.UI;
using HFN.Common.AddressableAssets;

namespace HFN.Civics
{
    [System.Serializable]
    public class PolicyController : ExtendedMonoBehaviour
    {
        [SerializeField]
        private GameObject starContainer = null;
        [SerializeField]
        private List<PolicyStarData> starData = new List<PolicyStarData>();
        [SerializeField]
        private AddressableSpriteSelector addressableGreyStar = new AddressableSpriteSelector();
        [SerializeField]
        private AddressableSpriteSelector addressableGoldStar = new AddressableSpriteSelector();

        private PolicyChoicesView policyView = null;
        private int starsAllocated = 0;

        public int StarsAllocated
        {
            get { return starsAllocated; }
        }

        public PolicyChoicesView PolicyView
        {
            get { return policyView; }
            set { policyView = value; }
        }


        /// <summary>
        /// When the policy modal opens this sets up the initial paramaters of each policy
        /// </summary>
        public void ModalOpen()
        {
            for(int i = 0; i < starContainer.transform.childCount; i++)
            {
                PolicyStarData star = starContainer.transform.GetChild(i).GetComponent<PolicyStarData>();
                if (!starData.Contains(star))
                {
                    star.starIndex = i;
                    star.starImage = star.gameObject.GetComponent<Image>();
                    star.starButton = star.gameObject.GetComponent<Button>();
                    star.starButton.onClick.AddListener(delegate { OnStarButtonClicked(star.starIndex); });
                    starData.Add(star);
                }
            }

            if (starsAllocated > 0)
            {
                addressableGreyStar.GetSprite(delegate (Sprite sprite) { OnGreyStarSpriteLoaded(sprite, starsAllocated - 1); });
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void ResetPolicy ()
        {
            starsAllocated = 0;
        }


        /// <summary>
        /// If able, change the grey stars to gold up to, and including, the target index
        /// </summary>
        /// <param name="targetIndex">The index number of the star clicked in the list of stars</param>
        private void OnStarButtonClicked (int targetIndex)
        {
            if (policyView.CanStarsBeAllocated(targetIndex + 1, starsAllocated))
            {
                starsAllocated = (targetIndex) + 1;
                addressableGreyStar.GetSprite(delegate (Sprite sprite) { OnGreyStarSpriteLoaded(sprite, targetIndex); });
            }
        }


        /// <summary>
        /// Change the star image to the grey star
        /// </summary>
        /// <param name="sprite">the grey sprite</param>
        /// <param name="lastStarIndex">the index of the last star being changed</param>
        private void OnGreyStarSpriteLoaded (Sprite sprite, int lastStarIndex)
        {
            for (int i = 0; i < starData.Count; i++)
            {
                starData[i].starImage.sprite = sprite;
            }

            addressableGoldStar.GetSprite(delegate (Sprite goldSprite) { OnStarSpriteLoaded(goldSprite, lastStarIndex); });
        }


        /// <summary>
        /// Change the star image to the gold star
        /// </summary>
        /// <param name="sprite">the gold sprite</param>
        /// <param name="lastStarIndex">the index of the last star being changed</param>
        private void OnStarSpriteLoaded (Sprite sprite, int lastStarIndex)
        {
            for (int i = 0; i <= lastStarIndex; i++)
            {
                starData[i].starImage.sprite = sprite;
            }
        }
    }
}