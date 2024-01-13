using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using PGGE.Multiplayer;

public class RoomList : MonoBehaviourPunCallbacks
{
    //Singleton pattern 
    public static RoomList Instance;

    //reference to connectioncontroller script 
    public ConnectionController controller;

    //UI elements 
    public Transform roomListParent;
    public GameObject roomBtnPrefab;

    //list to store cached room information
    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    //method to change room name to join 
    public void ChangeRoomToCreateName(string name)
    {
        controller.roomNameToJoin = name;
    }

    //singleton pattern 
    private void Awake()
    {
        Instance = this;
    }

    // Coroutine to connect to photon
    IEnumerator Start()
    {
        //if we are in a room, leave
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        //wait until disconnect then connect using settings
        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);
        PhotonNetwork.ConnectUsingSettings();
    }

    //called when connected to the photon master server
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        //join the lobby
        PhotonNetwork.JoinLobby();
    }
   
    //called when the room list is updated
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //if this is the first time we call it 
        if (cachedRoomList.Count <= 0) 
        {
            //set cachedroomlists to the room list
            cachedRoomList = roomList;
        }
        else
        {
            foreach (var room in roomList)
            {
                for(int i = 0; i <cachedRoomList.Count; i++)
                {
                    //if the room is the same as the room that was returned as changed 
                    if (cachedRoomList[i].Name == room.Name)
                    {
                        List<RoomInfo> newList = cachedRoomList;

                        //update the changes to the cachedRoomList
                        if (room.RemovedFromList)
                        {
                            newList.Remove(newList[i]);
                        }
                        else
                        {
                            newList[i] = room;
                        }

                        cachedRoomList = newList;
                    }
                }
            }
        }
        //update the server list UI
        UpdateUI();
    }

    //function to update the server list UI
    void UpdateUI()
    {
        //destroy all Room buttons
        foreach (Transform RoomBtn in roomListParent)
        {
            Destroy(RoomBtn.gameObject);
        }

        //create room buttons based on the cachedRoomList
        foreach (var room in cachedRoomList)
        {
            GameObject roomButton = Instantiate(roomBtnPrefab, roomListParent);

            //set name and player count
            roomButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;
            roomButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.PlayerCount + "/4";

            //changed color of player count based on if its full or not
            roomButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = room.PlayerCount == room.MaxPlayers ? Color.red : Color.green;

            //set the room name so the button works
            roomButton.GetComponent<RoomBtnFunction>().RoomName = room.Name;
        }
    }

    //function to join room using a room name
    public void JoinRoomByName(string name)
    {
        controller.roomNameToJoin = name;
        controller.JoinOrCreateNewRoom(name);
    }
}


