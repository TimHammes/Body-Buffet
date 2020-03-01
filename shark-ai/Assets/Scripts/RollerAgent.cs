using UnityEngine;
using System.Collections;
using MLAgents;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class RollerAgent : Agent
{
    public Transform target;
    public Transform target2;
    public Transform target3;
    public Transform Avoid;
    public GameObject shark1;
    public Rigidbody sharkBody1;
    public GameManager gManager;
    public AudioSource randomSound;
    public AudioClip[] audioSources;

    public float speed = 0.2f;
    public float timeBetweenRewards = 10f;
    public float timeSinceReward = 0;
    public float bleadOutTime = 50.0f;
    public float timeSinceBite = 0;
    public bool isEast = false;


    private bool isDry = false;
    private bool isReward = false;
    public bool isBit = false;
    private bool onShore = false;
    private bool swimming = false;

    Rigidbody rBody;
    Animator m_Animator;
    PhotonView photonView;

    //Player player;

    public ParticleSystem blood;
    public ParticleSystem.EmissionModule em;

    private void Awake()
    {
        target = GameObject.Find("WaterArea").transform;
        target2 = GameObject.Find("EastBeach").transform;
        target3 = GameObject.Find("Vanish").transform;

        shark1 = GameObject.FindGameObjectWithTag("Shark");
        sharkBody1 = shark1.GetComponent<Rigidbody>();
        Avoid = shark1.transform;
        rBody = GetComponent<Rigidbody>();

        //if there is a swimmer child of the ball
        m_Animator = GetComponentInChildren<Animator>();
    }
    void Start()
    {
        //player = PhotonNetwork.LocalPlayer;
        blood = GetComponentInChildren<ParticleSystem>();
        em = blood.emission;
        em.enabled = false;
    }

    public override void AgentReset()
    {
        if (this.transform.position.y < 0)
        {
            //if agent falls, zero its momentum
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.position = new Vector3(25, 0.5f, 0);
        }
    }

    public override void CollectObservations()
    {
        //target and agent positions
        AddVectorObs(target.position);
        AddVectorObs(target2.position);
        AddVectorObs(target3.position);
        AddVectorObs(Avoid.position);
        AddVectorObs(this.transform.position);

        //Agent velocity
        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.z);

        //Shark velocity
        AddVectorObs(sharkBody1.velocity.x);
        AddVectorObs(sharkBody1.velocity.z);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);

        float distanceToWater = Vector3.Distance(this.transform.position, target.position);
        float distanceToHalfEast = Vector3.Distance(this.transform.position, target2.position);
        float distanceToEast = Vector3.Distance(this.transform.position, target2.position);
        float distanceToWest = Vector3.Distance(this.transform.position, target3.position);
        float distanceToHalfWest = Vector3.Distance(this.transform.position, target3.position);
        float distancetoAvoid = Vector3.Distance(this.transform.position, Avoid.position);
        SharkMovement sharkScript = shark1.GetComponent<SharkMovement>();

        //Reached target
        {
            //if (distanceToWater < 20.00f)
            //{

            //    SetReward(.05f);
            //    //Done();
            //}

            if (distancetoAvoid < 5.0f)
            {

                SetReward(-1f);
            }

            //if (distanceToHalfEast < 25.0f)
            //{
            //    SetReward(0.1f);
            //}

            //if (distanceToEast < 15.0f)
            //{
            //    if (isEast == false)
            //    {
            //        //sharkScript.Owner.AddScore(-1);
            //        isEast = true;
            //    }
            //    SetReward(0.5f);
            //}

            //if (distanceToHalfWest < 25.0f)
            //{

            //    SetReward(0.1f);
            //}
            if (distanceToWest < 15.0f)
            {
                if (isEast == true)
                {
                    sharkScript.Owner.AddScore(-1);
                    isEast = false;
                }
                SetReward(0.5f);
            }

            if (this.transform.position.y < 0)
            {
                isReward = true;
                Done();
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Shark")
        {

            PhotonView sharkView = other.gameObject.GetComponent<PhotonView>();
            if (!isBit)
            {
                isBit = true;
                randomSound.clip = audioSources[UnityEngine.Random.Range(0, audioSources.Length)];
                randomSound.Play();
                em.enabled = true;
                blood.Play();
                rBody.isKinematic = true;

                SharkMovement shark = other.gameObject.GetComponent<SharkMovement>();
                if (sharkView.IsMine)
                {
                    shark.Owner.AddScore(1);
                }

                //if there is a swimmer as a child of the ball
                m_Animator.SetBool("BittenAndDying", true);
            }
        }

        if (other.gameObject.tag == "Shore")
        {
            if (!onShore)
            {
                Vector3 newPosition = new Vector3(this.transform.position.x - 2, this.transform.position.y + 4, this.transform.position.z);
                this.transform.position = newPosition;
                swimming = false;
                onShore = true;
                m_Animator.SetBool("Swimming", false);
                m_Animator.SetBool("OnShore", true);

            }
        }

        if (other.gameObject.tag == "vanish")
        {
            PhotonView newView = other.gameObject.GetComponent<PhotonView>();
            SharkMovement sharkScript = shark1.GetComponent<SharkMovement>();

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                player.AddScore(-1);
            }

            photonView = PhotonView.Get(this);
            photonView.RPC("GoodSwimmer", RpcTarget.All);

        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Shore")
        {
            if (!onShore)
            {
                swimming = false;
                onShore = true;
                m_Animator.SetBool("Swimming", false);
                m_Animator.SetBool("OnShore", true);


            }
        }

        if (other.gameObject.tag == "Water")
        {
            if (!swimming)
            {
                onShore = false;
                swimming = true;
                m_Animator.SetBool("OnShore", false);
                m_Animator.SetBool("Swimming", true);
            }
        }
    }

    [PunRPC]
    public void DeadSwimmer()
    {
        Destroy(gameObject);
    }

    [PunRPC]
    public void GoodSwimmer()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if (isBit)
        {
            timeSinceBite += Time.deltaTime;
            if (timeSinceBite > (bleadOutTime * 0.9f))
            {
                //if there is a swimmer as a child of the ball
                m_Animator.SetBool("HasDied", true);

                if (timeSinceBite > bleadOutTime)
                {
                    //this.gameObject.SetActive(false);
                    //Score.localScore += 5;
                    //player.AddScore(5);

                    photonView = PhotonView.Get(this);
                    photonView.RPC("DeadSwimmer", RpcTarget.All);


                    //DeadSwimmer();
                }
            }
        }
    }
}
