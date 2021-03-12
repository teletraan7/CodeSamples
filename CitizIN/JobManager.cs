using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using HFN.Common;
using HFN.Common.AddressableAssets;

namespace HFN.Citizin
{
    public class JobManager : MonoBehaviour
    {
        [SerializeField, Tooltip("The time interval between job spawns.")]
        protected float updateJobsAt = 0f;
        [SerializeField, Tooltip("The time from the start of the game to the first job posting.")]
        protected float beginDelay = 0f;
        [SerializeField, Tooltip("The max number of jobs that can be on the board.")]
        protected int maxNumOfJobs = 0;
        [SerializeField, Tooltip("the current number of jobs on the board.")]
        protected int currentNumOfJobs = 0;
        [SerializeField, Tooltip("The max amount of items a job could recquire to be complete.")]
        protected int maxItemsNeeded = 0;
        [SerializeField, Tooltip("The minimum amount of items a job could recquire to be complete.")]
        protected int minItemsNeeded = 0;
        [SerializeField, Tooltip("A list that contains all of the current jobs on the job board.")]
        protected List<JobController> currentJobs = new List<JobController>();
        [SerializeField, Tooltip("A list of all possible jobs the player could take.")]
        protected List<JobData> availableJobs = new List<JobData>();
        [SerializeField, Tooltip("A pool containing all of the resources that are being stored at the station.")]
        protected List<FactionDefinition> resourcePool = new List<FactionDefinition>();
        [SerializeField]
        protected RectTransform boatPath = null;
        [Header("Manager Scripts")]
        [SerializeField]
        protected StationStorageManager storageManager = null;
        [Header("Addressables")]
        [SerializeField]
        protected AssetReferenceGameObject addressableJobPrefab = new AssetReferenceGameObject("");
        [SerializeField]
        protected AssetReferenceGameObject addressableBoatPrefab = new AssetReferenceGameObject("");

        private float delayTime = 0f;
        private List<GameObject> boats = new List<GameObject>();

        private static JobManager instance = null;

        public static JobManager Instance
        {
            get { return instance ? instance : instance = FindObjectOfType<JobManager>(); }
        }

        /// <summary>
        /// When the game is unpaused start the job board again
        /// </summary>
        public void UnPauseGame ()
        {
            StartCoroutine(UpdateJobBoard());
        }


        /// <summary>
        ///When the game has ended clear the job board and reset its info 
        /// </summary>
        public void ClearJobBoard ()
        {
            addressableJobPrefab.ReleaseAsset();
            currentNumOfJobs = 0;
            maxItemsNeeded = minItemsNeeded;
            for (int i = 0; i < currentJobs.Count; i++)
            {
                currentJobs[i].ReleaseSprites();
                AddressablePoolManager.Release(currentJobs[i].gameObject);
            }
            for (int i = 0; i < currentJobs.Count; i++)
            {               
                currentJobs[i].gameObject.Delete();
            }
            currentJobs.Clear();
            resourcePool.Clear();

            if (boats.Count > 0)
            {
                for (int i = 0; i < boats.Count; i++)
                {
                    AddressablePoolManager.Release(boats[i]);
                }
            }

            boats.Clear();
        }      


        /// <summary>
        /// Drop off resources at the station and put them in storage, or use them right away. 
        /// </summary>
        /// <param name="resourceFaction">The faction of the resource being dropped off at the station.</param>
        public void EvalulateCargoAtStation (FactionDefinition resourceFaction)
        {
            storageManager.SendUpdateToFactionGoal(resourceFaction);
            //Go through the jobs
            for (int i = 0; i < currentJobs.Count; i++)
            {
                //check if the job is already done
                bool jobStatus = currentJobs[i].CheckJobCompletion();
                //if if isnt
                if (currentJobs[i].JobFaction == resourceFaction && !jobStatus)
                {
                    //then add this resource to that job if it matches the jobs faction
                    currentJobs[i].AddResource();
                    resourceFaction = FactionDefinition.None;
                    break;
                }
                //if it is
                else if (currentJobs[i].JobFaction == resourceFaction && jobStatus)
                {
                    //Add this resource to the storage of that faction
                    resourcePool.Add(resourceFaction);
                    storageManager.AdjustResources(resourceFaction, true);
                    resourceFaction = FactionDefinition.None;
                    break;
                }
            }
            //make sure to update the resourcepool, and station manager
            if (resourceFaction != FactionDefinition.None)
            {
                resourcePool.Add(resourceFaction);
                storageManager.AdjustResources(resourceFaction, true);
            }
        }


        /// <summary>
        /// When a job is completed update the current job list and count
        /// </summary>
        /// <param name="jobCompleted">A reference to the controller of the completed job. Will be needed remove the job from the current jobs list.</param>
        public void JobComplete (JobController jobCompleted)
        {
            //subtract one from the number of current jobs
            currentNumOfJobs -= 1;
            //remove this job from the list
            currentJobs.Remove(jobCompleted);
            jobCompleted.ReleaseSprites();
            //make sure the current number of jobs never goes below zero
            if (currentNumOfJobs < 0)
            {
                currentNumOfJobs = 0;
            }

            LaunchBoat();
        }


        /// <summary>
        /// When a job is expired update the current job list and count
        /// </summary>
        /// <param name="jobCompleted">A reference to the controller of the completed job. Will be needed remove the job from the current jobs list.</param>
        public void JobExpired (JobController jobCompleted)
        {
            currentNumOfJobs -= 1;
            currentJobs.Remove(jobCompleted);
            if (currentNumOfJobs < 0)
            {
                currentNumOfJobs = 0;
            }
        }

        
        /// <summary>
        /// Will be called  at the start of the minigame to run the updatejobboard cooroutine.
        /// </summary>
        public void InitiateJobManager ()
        {
            delayTime = beginDelay;
            StartCoroutine(UpdateJobBoard());
        }


        /// <summary>
        /// Raise the limit on the max items needed for jobs, now jobs can have more than one item to complete
        /// </summary>
        public void RaiseItemMax ()
        {
            maxItemsNeeded++;
        }


        /// <summary>
        /// Now pick a random job and add it to the board.
        /// </summary>
        /// <param name="newJob">The jobcontroller for this new job. We need it to fill out the right info.</param>        
        protected void CreateNewJob(JobController newJob)
        {
            //Choose faction
            int jobIndex = Random.Range(0, availableJobs.Count);
            //Choose number of items needed
            int itemsNeeded = Random.Range(minItemsNeeded, maxItemsNeeded + 1);
            //now make that job and give it the info it needs.
            newJob.SetJobInfo(availableJobs[jobIndex], itemsNeeded);
        }


        protected void OnJobLoaded(GameObject jobGo, JobController job)
        {
            jobGo.SetActive(false);
            jobGo.SetActive(true);
            currentNumOfJobs += 1;
            currentJobs.Add(job);
            CreateNewJob(job);
        }


        /// <summary>
        /// A cooroutine that checks if we can add jobs to the board. Make sure this is started and stopped properly.
        /// </summary>
        protected IEnumerator UpdateJobBoard()
        {
            float elapsedTime = 0f;

            while (true)
            {
                while (elapsedTime < delayTime)
                {
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                delayTime = updateJobsAt;
                elapsedTime = 0f;
                if (currentNumOfJobs < maxNumOfJobs)
                {
                    AddressablePoolManager.RequestAsync<JobController>(addressableJobPrefab.RuntimeKey, transform, OnJobLoaded);
                }
            }
        }


        protected void LaunchBoat ()
        {
            AddressablePoolManager.RequestAsync<TweenAnchoredPosition>(addressableBoatPrefab.RuntimeKey, boatPath, OnBoatLoaded);
        }


        protected void OnBoatLoaded (GameObject boatGo, TweenAnchoredPosition tween)
        {
            boats.Add(boatGo);
            tween.StartTween(true, TweenPlaybackMode.Default, delegate { OnTweenFinished(boatGo.GetComponent<Image>()); });
        }


        protected void OnTweenFinished (Image image)
        {
            if (boats.Contains(image.gameObject))
            {
                boats.Remove(image.gameObject);
            }
            AddressablePoolManager.Release(image.gameObject);
        }
    }
}