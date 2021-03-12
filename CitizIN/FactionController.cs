using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace HFN.Citizin
{
    public class FactionController : MonoBehaviour
    {
        public const string KEY_GOAL = "x {0}";

        [SerializeField, Tooltip("the level of happiness for this faction.")]
        protected int factionHappiness = 0;
        [SerializeField, Tooltip("The faction type.")]
        protected FactionDefinition factionType = FactionDefinition.None;
        [SerializeField, Tooltip("This will turn the image green when happiness goes up and red when it goes down.")]
        protected Image factionImage = null;
        [SerializeField, Tooltip("This will be changed to represent the happiness level of the factions.")]
        protected Slider factionSlider = null;
        [SerializeField, Tooltip("The amount of time we want the sliders to take to adjust.")]
        protected float lerpDuration = 0f;
        [SerializeField, Tooltip("Animation curve to smooth out the lerping of the sliders.")]
        protected AnimationCurve animCurve = null;
        [SerializeField, Tooltip("The color the slider turns to when it is going up.")]
        protected Color positiveColor = Color.green;
        [SerializeField, Tooltip("The color the slider turns to when it is going up.")]
        protected Color negativeColor = Color.red;
        [SerializeField]
        protected GameOverManager gameOver = null;
        [SerializeField]
        protected int resourceGoalOne = 0;
        [SerializeField]
        protected int resourceGoalTwo = 0;
        [SerializeField]
        protected TextMeshProUGUI goalText = null;
        [SerializeField]
        protected List<MapTile> factionTiles = new List<MapTile>();

        protected int factionGoal = 0;
        protected int currentResources = 0;
        protected Color baseColor;
        protected List<MapTile> standardFactionTiles = new List<MapTile>();

        public int CurrentResources
        {
            get { return currentResources; }
            set { currentResources = value; }
        }

        public int FactionGoal
        {
            get { return factionGoal; }
            set { factionGoal = value; }
        }

        public int FactionHappiness
        {
            get { return factionHappiness; }
            set { factionHappiness = value; }
        }

        /// <summary>
        /// Called when a resource has been collected and needs to be added to the total goal
        /// </summary>
        public void AddResourceTowardsGoal ()
        {
            CurrentResources++;
            StartCoroutine(AdjustImage());
            if (CurrentResources == resourceGoalOne)
            {
                //change goal and upgrade a tile
                goalText.text = string.Format(KEY_GOAL, resourceGoalTwo);
                factionSlider.maxValue = resourceGoalTwo;
                factionSlider.value = 0;
                CurrentResources = 0;
                FactionGoal = resourceGoalTwo;
                FactionsManager.Instance.ResourceBonus = 500;
                int tileToUpgrade = Random.Range(0, standardFactionTiles.Count);
                standardFactionTiles[tileToUpgrade].UpgradeTile();
                standardFactionTiles.Remove(standardFactionTiles[tileToUpgrade]);
            }
            else if (CurrentResources == resourceGoalTwo)
            {
                //remove goal and upgrade a tile
                goalText.text = "";
                FactionsManager.Instance.ResourceBonus = 1000;
                int tileToUpgrade = Random.Range(0, standardFactionTiles.Count);
                standardFactionTiles[tileToUpgrade].UpgradeTile();
                standardFactionTiles.Remove(standardFactionTiles[tileToUpgrade]);
            }
        }


        /// <summary>
        /// Refresh the faction data when the game has ended
        /// </summary>
        public void Refresh()
        {
            factionSlider.value = 0f;
            factionImage.color = baseColor;
            CurrentResources = 0;   
            goalText.text = string.Format(KEY_GOAL, resourceGoalOne);
            factionSlider.maxValue = resourceGoalOne;
            FactionGoal = resourceGoalOne;
        }


        /// <summary>s
        /// Add points to the factions happiness level
        /// </summary>
        /// <param name="points">These points will be added to the factions happiness level</param>
        public void AddHappiness(int points)
        {
            if (FactionHappiness < 100)
            {
                FactionHappiness += points;
                StopAllCoroutines();
                StartCoroutine(AdjustImage());
            }
            //make sure the points dont go over 100
            else if (FactionHappiness > 100)
            {
                FactionHappiness = 100;
            }
        }


        /// <summary>
        /// Subtract points from the factions happiness level
        /// </summary>
        /// <param name="points">These points will be subtracted to the factions happiness level</param>
        public void SubtractHappiness (int points)
        {
            FactionHappiness -= points;
            //if we hit zero end the game
            if (FactionHappiness <= 0)
            {
                FactionHappiness = 0;
                //gameOver.GameHasEnded(false);
            }
            StopAllCoroutines();
            StartCoroutine(AdjustImage());
        }


        /// <summary>
        /// Lerp the image for the factions
        /// </summary>
        protected IEnumerator AdjustImage ()
        {          
            float elapsedTime = 0f;
            float duration = lerpDuration + Time.deltaTime;
            float startValue = factionSlider.value;

            if (factionSlider.value < CurrentResources)
            {
                factionImage.color = positiveColor;
            }

            while (elapsedTime < duration)
            {               
                factionSlider.value = Mathf.Lerp(startValue, CurrentResources, animCurve.Evaluate(elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            factionSlider.value = CurrentResources;
            factionImage.color = baseColor;
        }


        protected void Start()
        {
            baseColor = factionImage.color;
            factionSlider.value = 0;
            goalText.text = string.Format(KEY_GOAL, resourceGoalOne);
            CurrentResources = 0;
            factionSlider.maxValue = resourceGoalOne;
            FactionGoal = resourceGoalOne;
            standardFactionTiles = factionTiles;
        }
    }
}