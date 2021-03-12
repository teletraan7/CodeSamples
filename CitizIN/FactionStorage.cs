using UnityEngine;
using TMPro;

namespace HFN.Citizin
{
    public class FactionStorage : MonoBehaviour
    {         
        [SerializeField, Tooltip("The amount of rescources this faction currently has in storage.")]
        protected int amountOfResources = 0;
        [SerializeField, Tooltip("The text to represent the amount of resources this faction has.")]
        protected TextMeshProUGUI amountText = null;
        [SerializeField]
        protected FactionsManager factionManager = null;
        [SerializeField]
        protected FactionDefinition faction = FactionDefinition.None;

        public FactionDefinition Faction
        {
            get { return faction; }
            set { faction = value; }
        }

        public int AmountOfResources
        {
            get { return amountOfResources; }
        }

        /// <summary>
        /// Reset the data when the game has ended
        /// </summary>
        public void ResetData ()
        {
            amountOfResources = 0;
            amountText.text = "0";
        }


        /// <summary>
        /// Add a resource to this factions storage and update the text.
        /// </summary>
        public void AddResource ()
        {
            amountOfResources++;
            amountText.text = amountOfResources.ToString();
        }


        public void AddResourceToGoalTotal ()
        {
            factionManager.AddResourceToFaction(faction);
        }


        /// <summary>
        /// Subtract a resource from this factions storage and update the text.
        /// </summary>
        public void SubtractResources ()
        {
            amountOfResources--;

            if (amountOfResources < 0)
            {
                amountOfResources = 0;
            }

            amountText.text = amountOfResources.ToString();
        }


        protected void Start()
        {
            amountText.text = amountOfResources.ToString();
        }
    }
}