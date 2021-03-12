using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HFN.Common;
using HFN.Common.AddressableAssets;
using HFN.Common.AddressableAssets.UI;
using TMPro;

namespace HFN.Civics
{
    public class PolicyChoicesView : GameView
    {
        private const string KEY_STARTEXT = "Stars Reamining: {0}";

        [SerializeField]
        private Button previousActivityButton = null;
        [SerializeField]
        private Button nextActivityButton = null;
        [SerializeField]
        private Image backgroundImage = null;
        [SerializeField]
        private int totalStars = 0;
        [SerializeField]
        private int starsUsed = 0;
        [SerializeField]
        private TextMeshProUGUI starsText = null;
        [SerializeField]
        private GameViewId speechCreatorView = GameViewId.None;
        [SerializeField]
        private List<PolicyController> policies = new List<PolicyController>(4);
        [SerializeField]
        private AddressableImageSpriteAtlasMapper[] addressableMappers = new AddressableImageSpriteAtlasMapper[0];

        private Sprite presidentSprite = null;

        public override void Open(bool closeOthers = false, GameView previousView = null)
        {
            if (!IsOpen)
            {
                base.Open(closeOthers, previousView);

                for (int i = 0; i < policies.Count; i++)
                {
                    policies[i].ModalOpen();
                    policies[i].PolicyView = this;
                }

                UIManager.GetView<PresidentialPowerView>().EnableScreenChangeButton(false);
            }
        }


        public override void Close(bool openPrevious = true)
        {
            if (IsOpen)
            {
                base.Close(openPrevious);               
            }
        }


        public void CloseAndReset()
        {
            for (int i = 0; i < policies.Count; i++)
            {
                policies[i].ResetPolicy();
            }

            totalStars = 0;
            starsUsed = 0;
            Close();
            Release();
        }


        public void Initialize(int stars, Sprite president)
        {
            Open();
            presidentSprite = president;

            int starsAlreadyUsed = 0;

            //check each policy to see if they already have stars given to them
            for (int i = 0; i < policies.Count; i++)
            {
                starsAlreadyUsed += policies[i].StarsAllocated;
            }

            totalStars = stars;
            starsText.text = string.Format(KEY_STARTEXT, totalStars - starsAlreadyUsed);
        }


        /// <summary>
        /// Determine if the player has enough stars to be allocated to their policy choice. Called when a star button is clicked
        /// </summary>
        /// <param name="starsAllocated">The amount of stars being allocated to the policy</param>
        /// <param name="previouslyAllocated">The amount of stars, if any, that where previously allocated to the policy</param>
        /// <returns>Bool to let the other script know if the player can allocate the stars they want</returns>
        public bool CanStarsBeAllocated (int starsAllocated, int previouslyAllocated)
        {            
            //take the stars previously given to the policy back
            starsUsed -= previouslyAllocated;

            if (starsUsed + starsAllocated <= totalStars)
            {               
                starsUsed += starsAllocated;
                starsText.text = string.Format(KEY_STARTEXT, (totalStars - starsUsed));
                return true;
            }
            else
            {
                starsUsed += previouslyAllocated;
                return false;
            }
        }


        public void Release ()
        {
            for (int i = 0; i < addressableMappers.Length; i++)
            {
                addressableMappers[i].Release();
            }

            backgroundImage.sprite = null;
        }


        private void OnPreviousButtonClicked ()
        {
            Close();

            PresidentialPowerView view = AddressableUIManager.GetView<PresidentialPowerView>();

            view.EnableScreenChangeButton(true);
            view.UnloadCategories();
            view.InitilizeData();
        }


        private void OnNextButtonClicked()
        {
            AddressableUIManager.GetViewAsync<SpeechCreatorView>(speechCreatorView, OnSpeechCreatorViewLoaded);
        }


        private void OnSpeechCreatorViewLoaded(SpeechCreatorView view)
        {
            view.Initialize(starsUsed, policies, presidentSprite);
        }


        private void Start()
        {
            nextActivityButton.onClick.AddListener(OnNextButtonClicked);
            previousActivityButton.onClick.AddListener(OnPreviousButtonClicked);
        }
    }
}