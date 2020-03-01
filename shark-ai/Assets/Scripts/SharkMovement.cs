using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class SharkMovement : MonoBehaviourPun
{


  Transform sharkTransform;
  Vector3 sharkVelocity;
  Rigidbody sharkBody;

  public float sharkSpeed;
  public Player Owner { get; private set; }

  void Start()
  {
    sharkBody = GetComponent<Rigidbody>();
    sharkSpeed = 15.0f;
    InitializeOwner(Owner);
  }

  public void InitializeOwner(Player owner)
  {
    PhotonView fishView = this.GetComponent<PhotonView>();
    Owner = owner;
    Owner = PhotonNetwork.LocalPlayer;
  

    Debug.Log("shark has been initialized" + Owner);
  }

  void FixedUpdate()
  {

    if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
    {
      return;
    }
    else
    {
      var x = Input.GetAxis("Horizontal") * Time.deltaTime * 7.0f;
      var z = Input.GetAxis("Vertical") * Time.deltaTime * 7.0f;

      sharkVelocity.x = x;
      sharkVelocity.z = z;
      sharkBody.AddForce(sharkVelocity * sharkSpeed);
      transform.LookAt(transform.position + sharkBody.velocity);

      if (transform.position.z < -19f)
      {
        Vector3 boundary = new Vector3(transform.position.x, transform.position.y, -19f);
        transform.position = boundary;
      }
      else if (transform.position.z > 19f)
      {
        Vector3 boundary = new Vector3(transform.position.x, transform.position.y, 19f);
        transform.position = boundary;
      }

      if (transform.position.x > 19f)
      {
        Vector3 boundary = new Vector3(19f, transform.position.y, transform.position.z);
        transform.position = boundary;
      }
      else if (transform.position.x < -19f)
      {
        Vector3 boundary = new Vector3(-19f, transform.position.y, transform.position.z);
        transform.position = boundary;
      }

    }




  }



}
