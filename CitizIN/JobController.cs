using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using HFN.Common;
using HFN.Common.AddressableAssets;
using HFN.Common.AddressableAssets.UI;
using TMPro;

namespace HFN.Citizin
{
    public class JobController : ExtendedMonoBehaviour
    {
        protected const string KEY_REWARD = "Reward: ${0}";

        [SerializeField]
        protected FactionDefinition jobFaction = FactionDefinition.None;
        [Tooltip("The number of resources needed for this job.")]
        protected int resourcesNeeded = 0;
        [SerializeField, Tooltip("The base color for item slots.")]
        protected string baseColor = null;
        [SerializeField, Tooltip("The time interval we use to take money away from a job reward.")]
        protected float looseReward = 0f;
        [SerializeField, Tooltip("The number of resources gathered for this job.")]
        protected int resourcesAcquired = 0;
        [SerializeField, Tooltip("The image representing this faction.")]
        protected Image factionImage = null;
        [SerializeField, Tooltip("The TMPro object for the name of this faction.")]
        protected TextMeshProUGUI factionName = null;
        [SerializeField, Tooltip("The TMPro object for the job description.")]
        protected TextMeshProUGUI descriptionText = null;
        [SerializeField, Tooltip("The TMPro object for the job reward.")]
        protected TextMeshProUGUI rewardText = null;
        [SerializeField, Tooltip("The button of the entire object it self.")]
        protected Button jobButton = null;
        [SerializeField, SoundId]
        protected int buttonClickId = 0;
        [SerializeField, SoundId]
        protected int jobSetId = 0;
        [SerializeField]
        protected Animation jobAnim = null;
        [SerializeField, Tooltip("The image for the item solts.")]
        protected Image[] itemImage = new Image[0];

        [Header("Addressable Sprites")]
        [SerializeField]
        protected AddressableImageSpriteAtlasMapper[] addressableSprites = new AddressableImageSpriteAtlasMapper[0];

        protected int reward = 0;
        protected JobData jobData = null;
        protected List<Image> itemsNeededForJob = new List<Image>();

        public int Reward
        {
            get { return reward; }
            set
            {
                reward = value;
                rewardText.text = string.Format(KEY_REWARD, reward);
            }
        } 

        public FactionDefinition JobFaction
        {
            get { return jobFaction; }
            set { jobFaction = value; }
        }

        public int ResourcesNeeded
        {
            get { return resourcesNeeded; }
            set { resourcesNeeded = value; }
        }

        public void ReleaseSprites ()
        {
            jobData.addressableSprite.Release();

            for(int i=0; i < addressableSprites.Length; i++)
            {
                addressableSprites[i].Release();
            }

            factionImage.sprite = null;
            factionImage.enabled = false;
            for (int i = 0; i < itemsNeededForJob.Count; i++)
            {
                itemsNeededForJob[i].sprite = null;
            }
            AddressablePoolManager.Recycle(gameObject);
        }


        /// <summary>
        /// Set the needed info for this job: like faction type, description, and rewards. Anything the UI would need to display for the new job.
        /// </summary>
        /// <param name="newData">The new data for this job.</param>
        /// <param name="itemsNeeded">The int for the number of items needed for this job.</param>
        public void SetJobInfo(JobData newData, int itemsNeeded)
        {
            for (int i = 0; i < addressableSprites.Length; i++)
            {
                addressableSprites[i].Refresh();
            }

            //same goes for the jobbutton
            jobButton.onClick.RemoveAllListeners();
            jobButton.onClick.AddListener(OnCompleteClick);
            //give this job the data it needs and change the values for this script accordingly
            jobData = newData;
            jobData.addressableSprite.GetSprite(delegate (Sprite sprite) { OnFactionSpriteLoaded(sprite, itemsNeeded); });
        }


        /// <summary>
        /// Add resources to this jobs item slots
        /// </summary>
        public void AddResource ()
        {
            //if the job still needs resources
            if (resourcesAcquired < ResourcesNeeded)
            {
                int firstItem = 0;
                //get the first item slot that isnt being used
                itemsNeededForJob[firstItem].sprite = factionImage.sprite;
                //show the image of this item in the slot
                Color newColor;
                newColor = Color.white;
                newColor.a = 1;
                itemsNeededForJob[firstItem].color = newColor;
                //no take this slot out of the list since it is now being used
                itemsNeededForJob.Remove(itemsNeededForJob[firstItem]);
                //add 1 to the amount of resources acquired
                resourcesAcquired++;
                //check if the complete button can be turned on
                if (resourcesAcquired == ResourcesNeeded)
                {
                    //completeButton.gameObject.SetActive(true);
                    OnCompleteClick();
                }
            }
        }

        
        /// <summary>
        /// Used to check if the job has all of its item slots filled.
        /// </summary>
        /// <returns>A bool: true if the jobs item slots are full, and false if some are still open.</returns>
        public bool CheckJobCompletion ()
        {
            //bool to represent if this job is finished, set it to false by default
            bool jobStatus = false;

            //if this job is finished
            if (resourcesAcquired == ResourcesNeeded)
            {
                jobStatus = true;
            }

            //return to the job manager
            return jobStatus;
        }       


        /// <summary>
        /// The onclick event for the job being completed.
        /// </summary>
        protected void OnCompleteClick()
        {
            //Check if the job has enough items
            if (resourcesAcquired == ResourcesNeeded)
            {
                if (buttonClickId != 0)
                {
                    Client.Get<SoundManager>().Play(buttonClickId);
                }

                //Update the factions, money, and job managers then recyle the job
                MoneyManager.Instance.GetMoneyFromJob(reward);
                JobManager.Instance.JobComplete(this);

                for(int i=0; i < itemImage.Length; i++)
                {
                    itemImage[i].enabled = false;
                }

                PoolManager.Recycle(gameObject);
            }
        }


        protected void OnFactionSpriteLoaded(Sprite sprite, int itemsNeeded)
        {
            factionImage.sprite = sprite;
            factionImage.enabled = true;
            factionName.text = jobData.nameText;
            descriptionText.text = jobData.descriptionText;
            Reward = jobData.rewardAmount * itemsNeeded;
            ResourcesNeeded = itemsNeeded;
            resourcesAcquired = 0;
            JobFaction = jobData.faction;

            Color newColor;

            //Go throught the item slots
            for (int i = 0; i < itemImage.Length; i++)
            {
                //make visible the ones we need
                if ((i + 1) <= itemsNeeded)
                {
                    newColor = Color.white;
                    newColor.a = 0;
                    itemImage[i].color = newColor;
                    itemImage[i].sprite = null;
                    itemImage[i].enabled = true;
                    itemsNeededForJob.Add(itemImage[i]);
                }
                //turn off the ones we dont
                else if ((i + 1) > itemsNeeded)
                {
                    ColorUtility.TryParseHtmlString(baseColor, out newColor);
                    newColor.a = 1;
                    itemImage[i].color = newColor;
                    itemImage[i].sprite = null;
                    itemImage[i].enabled = true;
                }
            }

            if (buttonClickId != 0)
            {
                Client.Get<SoundManager>().Play(jobSetId);
            }

            //start updating the job
            StartCoroutine(UpdateJob());
            jobAnim.Play();

            StationStorageManager.Instance.NewJobMade(this);
        }


        /// <summary>
        /// While their is a reward to be given check if we need to subtract from it, else expire the job.
        /// </summary>
        protected IEnumerator UpdateJob()
        {
            float elapsedTime = 0f;

            while (Reward > 0)
            {
                while (elapsedTime < looseReward)
                {
                    elapsedTime += Time.deltaTime;

                    yield return null;
                }

                elapsedTime = 0f;
                Reward -= 10;
                if (Reward <= 0)
                {
                    Reward = 0;
                }
            }

            //expire the job and recyle this object
            JobManager.Instance.JobExpired(this);
            PoolManager.Recycle(gameObject);
        }
    }
}