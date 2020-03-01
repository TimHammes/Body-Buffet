using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun.UtilityScripts;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
  public static PhotonLobby lobby;   //singleton of class
  public GameObject offlineButton;
  public GameObject joinButton;
  public AudioSource audioSource;
  //public GameObject cancelButton;

  Vector3 cornerposition = new Vector3(675, -285, 0);
  Vector3 centerposition = new Vector3(100, 0, 0);

  string regionCode = "usw";
  string gameVersion = "1";


  private void Awake()
  {
    //lobby = this; //creates/initializes the singleton, lives within the Main menu scene

    if (lobby && lobby != this)
      Destroy(gameObject);
    else
      lobby = this;


    //PhotonNetwork.AutomaticallySyncScene = true;  //this enables use of PhotonNetwork.LoadLevel() on master client and all clients in the same room sync their level automatically
    DontDestroyOnLoad(lobby);
  }

  void Start()
  {
    //PhotonNetwork.ConnectToRegion(regionCode);
    //PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = "";
    PhotonNetwork.GameVersion = gameVersion;
    PhotonNetwork.ConnectUsingSettings(); //Connects to Master photon server.
    offlineButton.SetActive(true);

  }



  public override void OnConnectedToMaster()      //callback function
  {
    Debug.Log("Player has connected to the Photon master server");
    PhotonNetwork.AutomaticallySyncScene = true;
    offlineButton.SetActive(false);
    joinButton.SetActive(true);
  }

  public override void OnDisconnected(DisconnectCause cause)
  {
    Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
    //cancelButton.transform.localPosition = centerposition;
  }


  public void OnBattleButtonClicked()
  {
    Debug.Log("Join Button was clicked");
    joinButton.SetActive(false);
    //cancelButton.SetActive(true);
    PhotonNetwork.JoinRoom("bodyBuffet");
  }

  public override void OnJoinRoomFailed(short returnCode, string message)    //callback function
  {
    Debug.Log("Tried to join game but failed. There was no open games available");
    CreateRoom();
  }

  public override void OnJoinedRoom()
  {
    Debug.Log("Welcome to the bodyBuffet room");

    
    PhotonNetwork.LocalPlayer.SetScore(0);

    if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
    {
      Debug.Log("We load the 'shark-feed'");
      PhotonNetwork.LoadLevel("shark-feed");

      

    }
    else
    {
      LoadArena();
    }
    //LoadArena();
  }



  void LoadArena()
  {
    if (!PhotonNetwork.IsMasterClient)
    {
      //Debug.LogError("PhotonNetwork: Trying to load a level but we are not the master client");
    }
    //Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

    
    PhotonNetwork.LoadLevel("shark-feed");

  }

  void CreateRoom()
  {

    //Debug.Log("Trying to create a new Room");
    string roomName = "bodyBuffet";
    RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 2 };
    PhotonNetwork.CreateRoom(roomName, roomOps);
  }
  public override void OnCreateRoomFailed(short returnCode, string message)    //callback function
  {
    //Debug.Log("Tried to create a new room but failed, there must already be a room with the same name");

    //cancelButton.transform.localPosition = centerposition;
    CreateRoom();//retrys to create room
  }

  public override void OnCreatedRoom()
  {
    //Debug.Log("the bodyBuffet room has been created");
  }


  public void OnCancelButtonClicked()
  {
    //Debug.Log("Tried to create a new room but failed, there must already be a room with the same name");
 
    PhotonNetwork.LoadLevel("MenuScene");
    PhotonNetwork.LeaveRoom();
    
   

  }

  public override void OnLeftRoom()
  {
    Debug.Log("left room");


    
    
  }

  private void Update()
  {

    if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("GameOver"))
    {

      GameObject endText = GameObject.Find("GameOver");
      Vector3 buttonPosition = new Vector3(endText.transform.position.x - 200, endText.transform.position.y - 50, endText.transform.position.z + 5);

        
      joinButton.SetActive(false);
      offlineButton.SetActive(false);
    }

    if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("shark-feed"))
    {
   
      joinButton.SetActive(false);
      offlineButton.SetActive(false);
    }

    if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MenuScene"))
    {


    
      

     

    }

    audioSource = gameObject.GetComponent<AudioSource>();
    if (Input.GetKeyDown(KeyCode.Space))
    {
      
      PhotonNetwork.LoadLevel("MenuScene");
      PhotonNetwork.LeaveRoom();
      audioSource.Stop();
      audioSource.Play();

    }





  }

}
