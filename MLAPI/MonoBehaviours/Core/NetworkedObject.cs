﻿using MLAPI.NetworkingManagerComponents;
using System.Collections.Generic;
using UnityEngine;

namespace MLAPI
{
    public class NetworkedObject : MonoBehaviour
    {
        [HideInInspector]
        public uint NetworkId;
        [HideInInspector]
        public int OwnerClientId = -2;
        [HideInInspector]
        public int SpawnablePrefabIndex;
        [HideInInspector]
        public bool isPlayerObject = false;
        public bool ServerOnly = false;
        [HideInInspector]
        public bool isPooledObject = false;
        [HideInInspector]
        public ushort PoolId;
        public bool isLocalPlayer
        {
            get
            {
                return isPlayerObject && (OwnerClientId == NetworkingManager.singleton.MyClientId || (OwnerClientId == -1 && NetworkingManager.singleton.isHost));
            }
        }

        public bool isOwner
        {
            get
            {
                return !isPlayerObject && (OwnerClientId == NetworkingManager.singleton.MyClientId || (OwnerClientId == -1 && NetworkingManager.singleton.isHost));
            }
        }

        private void OnDestroy()
        {
            SpawnManager.OnDestroyObject(NetworkId, false);
        }

        internal bool isSpawned = false;

        public void Spawn()
        {
            SpawnManager.OnSpawnObject(this);
        }

        public void SpawnWithOwnership(int clientId)
        {
            SpawnManager.OnSpawnObject(this, clientId);
        }

        public void RemoveOwnership()
        {
            SpawnManager.RemoveOwnership(NetworkId);
        }

        public void ChangeOwnership(int newOwnerClientId)
        {
            SpawnManager.ChangeOwnership(NetworkId, newOwnerClientId);
        }

        internal void InvokeBehaviourOnLostOwnership()
        {
            NetworkedBehaviour[] netBehaviours = GetComponentsInChildren<NetworkedBehaviour>();
            for (int i = 0; i < netBehaviours.Length; i++)
            {
                //We check if we are it's networkedObject owner incase a networkedObject exists as a child of our networkedObject.
                if (netBehaviours[i].networkedObject == this)
                {
                    netBehaviours[i].OnLostOwnership();
                }
            }
        }

        internal void InvokeBehaviourOnGainedOwnership()
        {
            NetworkedBehaviour[] netBehaviours = GetComponentsInChildren<NetworkedBehaviour>();
            for (int i = 0; i < netBehaviours.Length; i++)
            {
                //We check if we are it's networkedObject owner incase a networkedObject exists as a child of our networkedObject.
                if (netBehaviours[i].networkedObject == this)
                {
                    netBehaviours[i].OnGainedOwnership();
                }
            }
        }

        internal void InvokeBehaviourNetworkSpawn()
        {
            NetworkedBehaviour[] netBehaviours = GetComponentsInChildren<NetworkedBehaviour>();
            for (int i = 0; i < netBehaviours.Length; i++)
            {
                //We check if we are it's networkedObject owner incase a networkedObject exists as a child of our networkedObject.
                if(netBehaviours[i].networkedObject == this && !netBehaviours[i].networkedStartInvoked)
                {
                    netBehaviours[i].NetworkStart();
                    netBehaviours[i].SyncVarInit();
                }
            }
        }

        internal static List<NetworkedBehaviour> networkedBehaviours = new List<NetworkedBehaviour>();
        internal static void InvokeSyncvarUpdate()
        {
            for (int i = 0; i < networkedBehaviours.Count; i++)
            {
                networkedBehaviours[i].SyncvarUpdate();
            }
        }

        internal ushort GetOrderIndex(NetworkedBehaviour instance)
        {
            NetworkedBehaviour[] behaviours = GetComponentsInChildren<NetworkedBehaviour>();
            for (ushort i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i].networkedObject == this && behaviours[i] == instance)
                    return i;
            }
            return 0;
        }

        internal NetworkedBehaviour GetBehaviourAtOrderIndex(ushort index)
        {
            NetworkedBehaviour[] behaviours = GetComponentsInChildren<NetworkedBehaviour>();
            return behaviours[index];
        }
    }
}
