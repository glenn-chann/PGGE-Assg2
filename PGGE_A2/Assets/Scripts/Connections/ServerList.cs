using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using PGGE.Multiplayer;

public class ServerList : MonoBehaviourPunCallbacks
{
    public static ServerList Instance;

    public ConnectionController controller;

    public Transform serverListParent;
    public GameObject serverBtnPrefab;

    private List<RoomInfo> cachedServerList = new List<RoomInfo>();

    public void ChangeServerToCreateName(string name)
    {
        controller.serverNameToJoin = name;
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
   
    public override void OnRoomListUpdate(List<RoomInfo> serverList)
    {
        //if this is the first time we call it 
        if (cachedServerList.Count <= 0) 
        {
            //set cachedserverlists to the server list
            cachedServerList = serverList;
        }
        else
        {
            foreach (var server in serverList)
            {
                for(int i = 0; i <cachedServerList.Count; i++)
                {
                    //if the server is the same as the server that was returned as changed 
                    if (cachedServerList[i].Name == server.Name)
                    {
                        List<RoomInfo> newList = cachedServerList;

                        if (server.RemovedFromList)
                        {
                            newList.Remove(newList[i]);
                        }
                        else
                        {
                            newList[i] = server;
                        }

                        cachedServerList = newList;
                    }
                }
            }
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        foreach (Transform ServerBtn in serverListParent)
        {
            Destroy(ServerBtn.gameObject);
        }

        foreach (var server in cachedServerList)
        {
            GameObject serverButton = Instantiate(serverBtnPrefab, serverListParent);

            serverButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = server.Name;
            serverButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = server.PlayerCount + "/4";

            serverButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = server.PlayerCount == server.MaxPlayers ? Color.red : Color.green;

            serverButton.GetComponent<ServerBtnFunction>().ServerName = server.Name;
        }
    }

    public void JoinRoomByName(string name)
    {
        controller.serverNameToJoin = name;
        controller.JoinOrCreateNewRoom(name);
    }
}


