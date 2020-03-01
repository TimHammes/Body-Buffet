using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;

public class Score : MonoBehaviourPunCallbacks
{
  private Dictionary<int, GameObject> playerListEntries;
  public PhotonLobby lobby;

  public GameObject PlayerOverviewEntryPrefab;

  private void Awake()
  {
    PhotonNetwork.AutomaticallySyncScene = true;
  }
  private void Start()
  {

    
    playerListEntries = new Dictionary<int, GameObject>();


  }
  public override void OnPlayerLeftRoom(Player otherPlayer)
  {
    Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
    playerListEntries.Remove(otherPlayer.ActorNumber);
  }


  private void FixedUpdate()
  {


    foreach (Player p in PhotonNetwork.PlayerList)
    {
      if (!playerListEntries.ContainsKey(p.ActorNumber))
      {
        GameObject entry = Instantiate(PlayerOverviewEntryPrefab);
        entry.transform.SetParent(gameObject.transform);
        entry.transform.localScale = Vector3.one;
        entry.GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("Score: {0}", p.GetScore());

        playerListEntries.Add(p.ActorNumber, entry);
      }
      foreach (Player pl in PhotonNetwork.PlayerList)
      {
        GameObject entry;
        if (playerListEntries.TryGetValue(pl.ActorNumber, out entry))
        {
          int Score = pl.GetScore();
          entry.GetComponent<TMPro.TextMeshProUGUI>().text = string.Format("Score: {0}", Score);
          if(Score >= 50)
          {
            
            PhotonNetwork.LoadLevel("GameOver");
          }
        }
      }

     
    }

  }
}