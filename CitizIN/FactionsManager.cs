using UnityEngine;
using System.Collections.Generic;

namespace HFN.Citizin
{
    /// <summary>
    /// Will manage the factions and call their methods
    /// </summary>
    public class FactionsManager : MonoBehaviour
    {
        [SerializeField, Tooltip("This is the base rate at which factions loose happiness when a job is incomplete.")]
        protected int loseHappinessBase = 0;
        [SerializeField, Tooltip("This is the number that pay for each job will be divided by to get that jobs happiness points.")]
        protected int dividePayBy = 0;
        [SerializeField, Tooltip("A list of factions and their needed data.")]
        protected List<FactionData> factions = new List<FactionData>();

        protected int resourceBonus = 0;

        public int ResourceBonus
        {
            get { return resourceBonus; }
            set { resourceBonus += value; }
        }

        private static FactionsManager instance = null;

        public static FactionsManager Instance
        {
            get { return instance ? instance : instance = FindObjectOfType<FactionsManager>(); }
        }

        /// <summary>
        /// Reset the faction info
        /// </summary>
        public void ResetFactions ()
        {
            for (int i = 0; i < factions.Count; i++)
            {
                factions[i].factionController.Refresh();
            }
            resourceBonus = 0;
        }


        /// <summary>
        /// Let the faction know a resource has been acquired
        /// </summary>
        /// <param name="faction">The desired faction you wish to update</param>
        public void AddResourceToFaction(FactionDefinition faction)
        {
            for (int i = 0; i < factions.Count; i++)
            {
                if (factions[i].factionType == faction)
                {
                    factions[i].factionController.AddResourceTowardsGoal();
                }
            }
        }


        /// <summary>
        /// Gather all of the resources collected for each of the factions
        /// </summary>
        /// <returns>The total amount of collected resources of all of the factions</returns>
        public int TotalResourcesOfFactions ()
        {
            int resourceTotal = 0;
            for (int i = 0; i < factions.Count; i++)
            {
                resourceTotal += factions[i].factionController.CurrentResources;
            }
            return resourceTotal;
        }


        /// <summary>
        /// Gather the total resource goal for all of the factions
        /// </summary>
        /// <returns>The total resource goal of all the factions added up</returns>
        public int TotalResourceGoal ()
        {
            int resourceGoal = 0;
            for (int i = 0; i < factions.Count; i++)
            {
                resourceGoal += factions[i].factionController.FactionGoal;
            }
            return resourceGoal;
        }
    }
}