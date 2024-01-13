using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace PGGE
{
    namespace Multiplayer
    {
        public class ConnectionController : MonoBehaviourPunCallbacks
        {
            AudioManager audioManager;

            const string gameVersion = "1";

            public byte maxPlayersPerRoom = 3;

            public string serverNameToJoin = "test";

            public GameObject mConnectionProgress;
            public GameObject mBtnJoinRoom;
            public GameObject mInpPlayerName;
            public GameObject mCreateServer;
            public GameObject mServerList;
            public GameObject mBtnChooseRoom;

            RoomInfo biggestRoom;

            bool isConnecting = false;

            void Awake()
            {
                // this makes sure we can use PhotonNetwork.LoadLevel() on 
                // the master client and all clients in the same 
                // room sync their level automatically
                PhotonNetwork.AutomaticallySyncScene = true;
                audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
            }


            // Start is called before the first frame update
            void Start()
            {
                mConnectionProgress.SetActive(false);
                mServerList.SetActive(false);
                mCreateServer.SetActive(false);
            }

            public void Connect()
            {
                //play button audio
                audioManager.source.PlayOneShot(audioManager.join);
                mBtnJoinRoom.SetActive(false);
                mBtnChooseRoom.SetActive(false);
                mInpPlayerName.SetActive(false);
                mServerList.SetActive(false);
                mConnectionProgress.SetActive(true);

                // we check if we are connected or not, we join if we are, 
                // else we initiate the connection to the server.
                if (PhotonNetwork.IsConnected)
                {
                    // Attempt joining a random Room. 
                    // If it fails, we'll get notified in 
                    // OnJoinRandomFailed() and we'll create one.
                    PhotonNetwork.JoinRoom(biggestRoom.Name);
                }
                else
                {
                    // Connect to Photon Online Server.
                    isConnecting = PhotonNetwork.ConnectUsingSettings();
                    PhotonNetwork.GameVersion = gameVersion;
                }
            }


            public override void OnConnectedToMaster()
            {
                if (isConnecting)
                {
                    Debug.Log("OnConnectedToMaster() was called by PUN");
                    PhotonNetwork.JoinRoom(biggestRoom.Name);
                }
            }

            public override void OnRoomListUpdate(List<RoomInfo> roomList)
            {
                // Room list has been updated
                // Iterate through the rooms and filter them based on your criteria

                biggestRoom = GetRoomWithMostPlayers(roomList);
            }

            public RoomInfo GetRoomWithMostPlayers(List<RoomInfo> roomList)
            {
                RoomInfo targetRoom = null;
                int maxPlayerCount = 0;

                foreach (RoomInfo room in roomList)
                {
                    if (room.PlayerCount < room.MaxPlayers && room.PlayerCount > maxPlayerCount)
                    {
                        targetRoom = room;
                        maxPlayerCount = room.PlayerCount;
                    }
                }

                return targetRoom;
            }

            public override void OnDisconnected(DisconnectCause cause)
            {
                Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
                isConnecting = false;
            }

            public override void OnJoinRandomFailed(short returnCode, string message)
            {
                Debug.Log("OnJoinRandomFailed() was called by PUN. " +
                    "No random room available" +
                    ", so we create one by Calling: " +
                    "PhotonNetwork.CreateRoom");

                // Failed to join a random room.
                // This may happen if no room exists or 
                // they are all full. In either case, we create a new room.
                PhotonNetwork.CreateRoom(serverNameToJoin,
                    new RoomOptions
                    {
                        MaxPlayers = maxPlayersPerRoom,
                    }) ;
            }

            public void JoinOrCreateNewRoom(string name)
            {
                PhotonNetwork.JoinOrCreateRoom(name,
                    new RoomOptions
                    {
                        MaxPlayers = maxPlayersPerRoom,
                    },null);
            }


            public override void OnJoinedRoom()
            {
                Debug.Log("OnJoinedRoom() called by PUN. Client is in a room.");
                Debug.Log(PhotonNetwork.CurrentRoom.Name);
                if (PhotonNetwork.IsMasterClient)
                {
                    Debug.Log("We load the default room for multiplayer");
                    PhotonNetwork.LoadLevel("MultiplayerMap00");
                }
            }

            //function for the back button
            public void OnBack()
            {
                //play button audio
                audioManager.source.PlayOneShot(audioManager.back);
                //if we are not in the server list 
                if (mInpPlayerName.activeSelf)
                {
                    //load the previous scene 
                    SceneManager.LoadScene(Level.PreviousLevel);
                }
                //if we are in create server screen
                else if (mCreateServer.activeSelf)
                {
                    mCreateServer.SetActive(false);
                    mServerList.SetActive(true);
                }
                else
                {
                    //deactivate server list and reactivate all other UI elements 
                    mServerList.SetActive(false);
                    mBtnChooseRoom.SetActive(true);
                    mBtnJoinRoom.SetActive(true);
                    mInpPlayerName.SetActive(true);
                }
            }

            public void ChooseServerBtn()
            {
                //play button audio
                audioManager.source.PlayOneShot(audioManager.join);
                //activate server list and deactivate all other UI elements 
                mServerList.SetActive(true);
                mBtnJoinRoom.SetActive(false);
                mBtnChooseRoom.SetActive(false);
                mInpPlayerName.SetActive(false);
            }

            public void ToCreateServerScreenBtn()
            {
                //play button audio
                audioManager.source.PlayOneShot(audioManager.join);
                //activate create server screen and deactivate server list
                mCreateServer.SetActive(true);
                mServerList.SetActive(false);
                Debug.Log("Pressed");
            }

            public void OnCreateServerBtnPressed()
            {
                //play button audio
                audioManager.source.PlayOneShot(audioManager.join);
                JoinOrCreateNewRoom(serverNameToJoin);
            }
        }
    }
}
