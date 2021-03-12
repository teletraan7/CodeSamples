using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using HFN.Common;
using HFN.Common.AddressableAssets;

namespace HFN.Citizin
{
    public class PathFinder : ExtendedMonoBehaviour
    {
        [SerializeField, Tooltip("The maximum number trains allowed to run at one time.")]
        protected int maxNumberOfTrains = 0;
        [SerializeField]
        protected int numberOfTrainsOnMap = 0;
        [SerializeField]
        protected int totalNumberOfTrains = 0;
        [SerializeField]
        protected int trainsAtStation = 0;
        [SerializeField]
        protected List<Node> shortestRoute = new List<Node>();
        [Header("Addressables")]
        [SerializeField]
        protected AssetReferenceGameObject addressableTrainPrefab = new AssetReferenceGameObject("");
        [SerializeField]
        protected bool cooldownOver = true;
        [SerializeField]
        private Transform trainParent = null;

        protected TrainController spawnedTrain = null;
        protected MapTile startTile = null;
        protected MapTile endTile = null;
        //a list of tiles trains are currently being sent to
        protected List<MapTile> listOfTargetDestinations = new List<MapTile>();
        protected List<Node> allNodes = new List<Node>();
        protected List<TrainController> trainsRunning = new List<TrainController>();
        protected List<TrainController> allTrains = new List<TrainController>();
        //The data trie of tiles being made as the player places tracks on the map.
        protected List<Node> nodeTrie = new List<Node>();

        public int NumberOfTrainsOnMap
        {
            get { return numberOfTrainsOnMap; }
            set { numberOfTrainsOnMap = value; }
        }

        public int TotalNumberOfTrains
        {
            get { return totalNumberOfTrains; }
            set { totalNumberOfTrains = value; }
        }

        public int TrainsAtStation
        {
            get { return trainsAtStation; }
            set { trainsAtStation = value; }
        }

        public List<TrainController> TrainsRunning
        {
            get { return trainsRunning; }
        }

        public TrainController AddTrainsRunning
        {
            set
            {
                trainsRunning.Add(value);
            }
        }

        public TrainController SubtractTrainsRunning
        {
            set
            {
                trainsRunning.Remove(value);
            }           
        }

        private static PathFinder instance = null;

        public static PathFinder Instance
        {
            get { return instance ? instance : instance = FindObjectOfType<PathFinder>(); }
        }

        /// <summary>
        /// When the game ends set all off the path finder data back to defaults
        /// </summary>
        public void ResetPathFinderData ()
        {
            TotalNumberOfTrains = 2;
            NumberOfTrainsOnMap = 0;
            TrainsAtStation = 2;
            allNodes.Clear();
            TrainsRunning.Clear();
            listOfTargetDestinations.Clear();
            shortestRoute.Clear();
            nodeTrie.Clear();
            cooldownOver = true;
            for (int i = 0; i < allTrains.Count; i++)
            {
                allTrains[i].Reset();
            }
            allTrains.Clear();
        }


        /// <summary>
        /// When the game begins set the initial start node 
        /// </summary>
        /// <param name="startTile">The station tile</param>
        public void StartNodeTrie (MapTile startTile)
        {
            Node newNode = new Node(startTile, new List<Node>());
            nodeTrie.Add(newNode);
            allNodes.Add(newNode);
        }


        /// <summary>
        /// When a track is placed create a node, add it to the trie, and update the nodes its's connected too
        /// </summary>
        /// <param name="tile">The tile a track is being placed on</param>
        /// <param name="adjacentTiles">The list of maptiles with tracks adjacent to tile</param>
        public void CreateNodeTrieBranch (MapTile tile, List<MapTile> adjacentTiles)
        {
            List<Node> adjacentNodes = new List<Node>();

            //go through the adjacent tiles
            for (int i = 0; i < adjacentTiles.Count; i++)
            {
                for (int j = 0; j < allNodes.Count; j++)
                {
                    //if their is a node that has matching coordinates as the adjacent tile then add that node to adjacentNodes
                    if (allNodes[j].ID == adjacentTiles[i].TileCoordinates)
                    {
                        adjacentNodes.Add(allNodes[j]);
                    }
                }
            }

            Node newNode = new Node(tile, adjacentNodes);
            allNodes.Add(newNode);

            //go throught the adjacent nodes
            for (int k = 0; k < adjacentNodes.Count; k++)
            {
                if (!adjacentNodes[k].children.Contains(newNode))
                {
                    //update those nodes to add this new node to their list of childern (adjacent nodes)
                    adjacentNodes[k].children.Add(newNode);
                }
            }
        }


        /// <summary>
        /// Set the start and end points for this path.
        /// </summary>
        /// <param name="start">A reference to the start tile.</param>
        /// <param name="end">A reference to the destination tile.</param>
        /// <param name="tiles">A list of all of the tiles with tracks on them.</param>
        public void RoutePoints(MapTile start, MapTile end)
        {
            cooldownOver = TrainCooldown.Instance.cooledDown;
            if (NumberOfTrainsOnMap < TotalNumberOfTrains && cooldownOver && !listOfTargetDestinations.Contains(end))
            {
                FindShortestPath(new List<Node>(), nodeTrie[0], end.TileCoordinates);
                startTile = start;
                endTile = end;
                TrainCooldown.Instance.BeginCooldown();
                SpawnTrain();
            }
        }


        /// <summary>
        /// Get the max number of trains for the script that called this
        /// </summary>
        /// <returns>Return the max number of trains</returns>
        public int GetMaxNumberOfTrains ()
        {
            return maxNumberOfTrains;
        }


        /// <summary>
        /// A new train has been purchased so update the amount of available trains
        /// </summary>
        public void AddNewTrain ()
        {
            TotalNumberOfTrains++;
        }


        /// <summary>
        /// Once a target tile is reached free it up from the list of current tiles trains are being sent to
        /// </summary>
        /// <param name="destination">The tile we are freeing from the list</param>
        public void RemoveDestinationFromList (MapTile destination)
        {
            listOfTargetDestinations.Remove(destination);
        }


        /// <summary>
        /// Find the quickest path to the destination.
        /// </summary>
        /// <param name="route">The current route the train is checking.</param>
        /// <param name="currentNode">The current node of the trie the pathfinder is on and checking.</param>
        /// <param name="targetId">The node ID of the target the pathfinder is trying to reach.</param>
        protected void FindShortestPath (List<Node> route, Node currentNode, Vector2 targetId)
        {
            //set the current node
            Node current = currentNode;

            if (shortestRoute.Count != 0 && route.Count > shortestRoute.Count)
            {
                //break this route if it goes over an already existing shortest route length
                return;
            }
            //if the current node id isnt the target
            else if (current.ID != targetId)
            {
                if (current.children.Count > 1)
                {
                    for (int i = 0; i < current.children.Count; i++)
                    {
                        if (!route.Contains(current.children[i]))
                        {
                            Node newCurrent = current.children[i];
                            List<Node> routeUpdate = new List<Node>(route);
                            routeUpdate.Add(current);
                            FindShortestPath(routeUpdate, newCurrent, targetId);
                        }
                    }
                }
                //if their is only one child then streamline the process
                else if (current.children.Count == 1)
                {
                    if (!route.Contains(current.children[0]))
                    {
                        Node newCurrent = current.children[0];
                        List<Node> routeUpdate = new List<Node>(route);
                        routeUpdate.Add(current);
                        FindShortestPath(routeUpdate, newCurrent, targetId);
                    }
                }
            }
            //if this node id is the target
            else if (current.ID == targetId)
            {
                route.Add(current);
                if (shortestRoute.Count == 0 || route.Count < shortestRoute.Count)
                {
                    shortestRoute = route;
                }
                return;
            }
        }
        

        /// <summary>
        /// Spawn the train and send it on its path
        /// </summary>
        protected void SpawnTrain ()
        {
            AddressablePoolManager.RequestAsync<TrainController>(addressableTrainPrefab.RuntimeKey, trainParent, OnTrainLoaded);
        }


        protected void OnTrainLoaded (GameObject trainG0, TrainController train)
        {
            trainG0.transform.position = new Vector3(0,0,-100);
            List<MapTile> trainPath = new List<MapTile>();

            for (int i = 0; i < shortestRoute.Count; i++)
            {
                trainPath.Add(shortestRoute[i].nodeTile);
            }
            
            train.GetSplines(trainPath, startTile, endTile);
            listOfTargetDestinations.Add(endTile);
            NumberOfTrainsOnMap++;
            AddTrainsRunning = train;
            allTrains.Add(train);
            TrainsAtStation = TotalNumberOfTrains - NumberOfTrainsOnMap;
            TrainPurchaser.Instance.AdjustTrainCount(TrainsAtStation);
            shortestRoute.Clear();
        }
    }
}