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
            
            //set a constant game version
            const string gameVersion = "1";

            //maximum players allowed per room
            public byte maxPlayersPerRoom = 4;

            //name of the room to join or create
            public string roomNameToJoin = "test";

            //UI elements to activate/deactivate
            public GameObject mConnectionProgress;
            public GameObject mBtnJoinRoom;
            public GameObject mInpPlayerName;
            public GameObject mCreateRoom;
            public GameObject mRoomList;
            public GameObject mBtnChooseRoom;

            //the room with the most amount of players
            RoomInfo biggestRoom;

            bool isConnecting = false;

            void Awake()
            {
                // this makes sure we can use PhotonNetwork.LoadLevel() on 
                // the master client and all clients in the same 
                // room sync their level automatically
                PhotonNetwork.AutomaticallySyncScene = true;
                //finding the object that has the audio script 
                audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
            }


            // Start is called before the first frame update
            void Start()
            {
                //deactivate UI that is not needed
                mConnectionProgress.SetActive(false);
                mRoomList.SetActive(false);
                mCreateRoom.SetActive(false);
            }

            public void Connect()
            {
                //play button audio
                audioManager.source.PlayOneShot(audioManager.join);

                //deactive UI elements 
                mBtnJoinRoom.SetActive(false);
                mBtnChooseRoom.SetActive(false);
                mInpPlayerName.SetActive(false);
                mRoomList.SetActive(false);

                //Activate loading text
                mConnectionProgress.SetActive(true);

                // we check if we are connected or not, we join if we are, 
                // else we initiate the connection to the room.
                if (PhotonNetwork.IsConnected)
                {
                    //join the room with the most amount of players 
                    if (biggestRoom != null)
                    {
                        PhotonNetwork.JoinRoom(biggestRoom.Name);
                        Debug.Log("Joined:" + biggestRoom.Name);
                    }
                    else
                    {
                        //if biggest room is not set create a room
                        JoinOrCreateNewRoom(roomNameToJoin);
                    }
                }
                else
                {
                    // Connect to Photon Online room.
                    isConnecting = PhotonNetwork.ConnectUsingSettings();
                    PhotonNetwork.GameVersion = gameVersion;
                }
            }

            //called when connected to the Photon Master room
            public override void OnConnectedToMaster()
            {
                if (isConnecting)
                {
                    Debug.Log("OnConnectedToMaster() was called by PUN");
                }
            }

            //called when the room list is updated
            public override void OnRoomListUpdate(List<RoomInfo> roomList)
            {
                //set the biggest room to the one with the most amount of players
                biggestRoom = GetRoomWithMostPlayers(roomList);
            }

            //filter through the list to return the room with the most players 
            public RoomInfo GetRoomWithMostPlayers(List<RoomInfo> roomList)
            {
                //initialising variables 
                RoomInfo targetRoom = null;
                int maxPlayerCount = 0;

                //Iterate through all rooms
                foreach (RoomInfo room in roomList)
                {
                    //if room is 1 player short of max players
                    if (room.PlayerCount == maxPlayersPerRoom - 1)
                    {
                        //return this room
                        return room;
                    }
                    //if room is not full and the room has more players then the room with the previos highest player count
                    else if (room.PlayerCount < room.MaxPlayers && room.PlayerCount > maxPlayerCount)
                    {
                        //change target room to this room
                        targetRoom = room;
                        //update the maxPlayerCount
                        maxPlayerCount = room.PlayerCount;
                    }
                }
                //return the room with the most players 
                return targetRoom;
            }

            //called when disconnected from Photon
            public override void OnDisconnected(DisconnectCause cause)
            {
                Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
                isConnecting = false;
            }

            //called when failed to join room
            public override void OnJoinRandomFailed(short returnCode, string message)
            {
                Debug.Log("OnJoinRandomFailed() was called by PUN. " +
                    "No random room available" +
                    ", so we create one by Calling: " +
                    "PhotonNetwork.CreateRoom");

                // Failed to join a random room.
                // This may happen if no room exists or 
                // they are all full. In either case, we create a new room.
                PhotonNetwork.CreateRoom(roomNameToJoin,
                    new RoomOptions
                    {
                        MaxPlayers = maxPlayersPerRoom,
                    }) ;
            }

            //join or create a specific room
            public void JoinOrCreateNewRoom(string name)
            {
                Debug.Log(name);
                PhotonNetwork.JoinOrCreateRoom(name,
                    new RoomOptions
                    {
                        MaxPlayers = maxPlayersPerRoom,
                    },null);
            }

            //called when you join a room
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
                //if we are not in the room list 
                if (mInpPlayerName.activeSelf)
                {
                    //load the previous scene 
                    SceneManager.LoadScene(Level.PreviousLevel);
                }
                //if we are in create room screen
                else if (mCreateRoom.activeSelf)
                {
                    mCreateRoom.SetActive(false);
                    mRoomList.SetActive(true);
                }
                else
                {
                    //deactivate room list and reactivate all other UI elements 
                    mRoomList.SetActive(false);
                    mBtnChooseRoom.SetActive(true);
                    mBtnJoinRoom.SetActive(true);
                    mInpPlayerName.SetActive(true);
                }
            }

            //button that leads to the room list screen
            public void ChooseRoomBtn()
            {
                //play button audio
                audioManager.source.PlayOneShot(audioManager.join);
                //activate room list and deactivate all other UI elements 
                mRoomList.SetActive(true);
                mBtnJoinRoom.SetActive(false);
                mBtnChooseRoom.SetActive(false);
                mInpPlayerName.SetActive(false);
            }

            //button that leads to the create room screen
            public void ToCreateRoomScreenBtn()
            {
                //play button audio
                audioManager.source.PlayOneShot(audioManager.join);
                //activate create room screen and deactivate room list
                mCreateRoom.SetActive(true);
                mRoomList.SetActive(false);
            }

            //create room screen
            public void OnCreateRoomBtnPressed()
            {
                //play button audio
                audioManager.source.PlayOneShot(audioManager.join);
                JoinOrCreateNewRoom(roomNameToJoin);
            }
        }
    }
}
