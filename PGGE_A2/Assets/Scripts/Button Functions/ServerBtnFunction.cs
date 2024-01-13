using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerBtnFunction : MonoBehaviour
{
    public string ServerName;

    public void OnButtonPressed()
    {
        ServerList.Instance.JoinRoomByName(ServerName);
    }
}
