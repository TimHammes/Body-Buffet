using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
  public GameObject playerPrefab;
  public GameObject agentPrefab;
  public GameObject scorePrefab;
  public RollerAgent agent;


  public PhotonLobby lobby;
  public int maxSwimmersOnScreen = 15;
  public int numSwimmers = 0;
  private List<GameObject> _swimmers = new List<GameObject>();
  private int swimmersOnScreen;
  public int swimmers;

  public float spawntime = 1.0f;
  public float spawnDelay = 1.0f;


  private GameObject[] _sharks;
  private int sharks;


  private void Awake()
  {
    
  }
  void Start()
  {


    if (playerPrefab == null || sharks == 2)
    {

    }
    else
    {
      PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 1f, 0f), Quaternion.identity, 0);
      Debug.Log(this.playerPrefab.name + playerPrefab.GetInstanceID() + " has entered the game.");

    }


    if (!PhotonNetwork.IsMasterClient)
    {
      return;
    }
    else
    {
      if (agentPrefab == null)
      {

      }
      else
      {
        InvokeRepeating("SpawnSwimmer", spawntime, spawnDelay);
      }
    }
  }

  public void SpawnSwimmer()
  {
    //if (swimmers < maxSwimmersOnScreen)
    //{
    //    for (var i = 0; i < maxSwimmersOnScreen; i++)
    //    {
    //        Debug.Log("Creating Agent now in SpawnSwimmer Function!");
    //        //CreateAgent(this.agentPrefab);
    //    }
    //}
    //else
    //{
    //    return;
    //}
    if (swimmers < maxSwimmersOnScreen)
    {
      CreateAgent(this.agentPrefab);
      CreateAgent(this.agentPrefab);
      CreateAgent(this.agentPrefab);
    }
    else
    {
      return;
    }
  }

  public void CreateAgent(GameObject prefab)
  {
    //Debug.Log("Creating Agent now in CreateAgent Function!");
    GameObject newAgent = PhotonNetwork.Instantiate(this.agentPrefab.name, new Vector3(30f, 6f, Random.Range(-2f, 15f)), Quaternion.identity, 0);

    _swimmers.Add(newAgent);

  }

  void Update()
  {
    _sharks = GameObject.FindGameObjectsWithTag("Shark");
    sharks = _sharks.Length;
    //Debug.Log(_sharks.Length);
    if (sharks > 2)
    {
      Destroy(_sharks[1]);
    };

    swimmers = _swimmers.Count;
    List<GameObject> itemsToRemove = new List<GameObject>();

    foreach (var swim in _swimmers)
    {
      if (swim == null)
      {
        itemsToRemove.Add(swim);
      }
    }

    foreach (var dead in itemsToRemove)
    {
      _swimmers.Remove(dead);
      CreateAgent(this.agentPrefab);
    }

    lobby = GameObject.FindGameObjectWithTag("lobby").GetComponent<PhotonLobby>();

    if (Input.GetKeyDown(KeyCode.Space))
    {
      //SceneManager.LoadScene(0);
      lobby.OnCancelButtonClicked();
  
    }
  }
}
