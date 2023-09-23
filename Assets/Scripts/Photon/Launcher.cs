using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;
    [SerializeField] TMP_InputField roomNameInputField;
    // Start is called before the first frame update
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;

    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
   
    [SerializeField] GameObject playerListItemPrefab;

    [SerializeField] GameObject startGameButton;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();


    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("----------Joined Master Clint");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("----------Joined Lobby");

        MenuManager.instance.OpenMenu("Title");
    }

    public void CreateRoom()
    {

        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);

        MenuManager.instance.OpenMenu("loading");

    }

    public override void OnJoinedRoom()
    {

        Debug.Log(PhotonNetwork.CurrentRoom.Name);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        MenuManager.instance.OpenMenu("RoomMenu");
        PhotonNetwork.NickName = "Player" + Random.Range(0,1000).ToString("0000");

        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }


        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < PhotonNetwork.PlayerList.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);

    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }




    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room creation failed" + message;
        MenuManager.instance.OpenMenu("ErrorMenu");


    }
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.OpenMenu("loading");


    }
    public override void OnLeftRoom()
    {
        MenuManager.instance.OpenMenu("Title");
    }



    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
       foreach(Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }



        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);


        }

    }


    public void JoinRoom(RoomInfo _info)
    {
        PhotonNetwork.JoinRoom(_info.Name);
        MenuManager.instance.OpenMenu("loading");


        


    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);

    }

}
