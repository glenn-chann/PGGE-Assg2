using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using PGGE.Multiplayer;

public class RoomList : MonoBehaviourPunCallbacks
{
    public static RoomList Instance;

    public ConnectionController controller;

    public Transform roomListParent;
    public GameObject roomBtnPrefab;

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    public void ChangeRoomToCreateName(string name)
    {
        controller.roomNameToJoin = name;
    }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        //if we are in a room, leave
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }
   
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //if this is the first time we call it 
        if (cachedRoomList.Count <= 0) 
        {
            //set cachedserverlists to the room list
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
        UpdateUI();
    }

    void UpdateUI()
    {
        foreach (Transform RoomBtn in roomListParent)
        {
            Destroy(RoomBtn.gameObject);
        }

        foreach (var room in cachedRoomList)
        {
            GameObject roomButton = Instantiate(roomBtnPrefab, roomListParent);

            roomButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;
            roomButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.PlayerCount + "/4";

            roomButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = room.PlayerCount == room.MaxPlayers ? Color.red : Color.green;

            roomButton.GetComponent<RoomBtnFunction>().RoomName = room.Name;
        }
    }

    public void JoinRoomByName(string name)
    {
        controller.roomNameToJoin = name;
        controller.JoinOrCreateNewRoom(name);
    }
}


