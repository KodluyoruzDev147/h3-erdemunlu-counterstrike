using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class SoldierController : MonoBehaviourPunCallbacks
{

    public float defaultMoveMultiplier = 5, defaultRotateMultiplier = 1250;
    public float fastMoveMultiplier = 10f;
    public float soldierAngle;
    public Camera camera;
    private Animator animator;
    public TextMeshProUGUI playerHealthText;
    public GameObject playerHealthTextBackground;
    public Vector3 startPosition;


    private void Start()
    {
        if (photonView.IsMine)
        {
            animator = GetComponent<Animator>();
            playerHealthText.enabled = false;
            playerHealthTextBackground.SetActive(false);
            startPosition = gameObject.transform.position;
            playerHealthText.text = $"Health 100%";
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                Move(1);
            }
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                Move(-1);
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                Move(2);
            }
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                Move(-2);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Hit();
            }
            else
            {
                SetAnimationState(true, false, false);
            }
            Rotate();
        }
    }

    public void Move(int direction)
    {
        if (photonView.IsMine)
        {
            SetAnimationState(false, true, false);
            switch (direction)
            {
                case 1:
                    gameObject.transform.Translate(Vector3.forward * defaultMoveMultiplier * Time.deltaTime);
                    break;
                case -1:
                    gameObject.transform.Translate(Vector3.back * defaultMoveMultiplier * Time.deltaTime);
                    break;
                case 2:
                    gameObject.transform.Translate(Vector3.right * defaultMoveMultiplier * Time.deltaTime);
                    break;
                case -2:
                    gameObject.transform.Translate(Vector3.left * defaultMoveMultiplier * Time.deltaTime);
                    break;
                default:
                    break;                                                                                            
            }                                                                                                         
        }                                                                                                             
    }                                                                                                                 
    public void Rotate()                                                                                              
    {                                                                                                                 
        if (photonView.IsMine)
        {
            soldierAngle += Input.GetAxis("Mouse X") * defaultRotateMultiplier * Time.deltaTime;
            transform.localRotation = Quaternion.AngleAxis(soldierAngle, Vector3.up);
        }
    }

    public void Hit()
    {
        if (photonView.IsMine)
        {
            SetAnimationState(false, false, true);

            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);                      
                                                                                         
            if (Physics.Raycast(ray, out hit))                                           
            {                                                                            
                Transform objectHit = hit.transform;
                if (objectHit.tag != "Player") return;
                int randomDamage = (int)Random.Range(5, 15);
                objectHit.gameObject.GetComponent<PhotonView>().RPC(nameof(DamageEnemy),RpcTarget.AllBuffered, randomDamage);
            }
        }
    }

    public void SetAnimationState(bool isIdle, bool isRun, bool isShoot)
    {
        if (photonView.IsMine)
        {
            animator.SetBool("idle", isIdle);
            animator.SetBool("isRun", isRun);
            animator.SetBool("isShoot", isShoot);
        }
    }


    [PunRPC]
    public void DamageEnemy(int damage, PhotonMessageInfo info)
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            if(player.ActorNumber == info.photonView.Owner.ActorNumber)
            {
                int playerHealth = (int)player.CustomProperties[Constants.PLAYER_HEALTH];
                if (playerHealth < damage)
                {
                    Reset();
                    break;
                }
                else playerHealth -= damage;
                player.CustomProperties[Constants.PLAYER_HEALTH] = playerHealth;
                playerHealthText.text = $"Health {playerHealth}%";
                
            }
        }
    }



    public void Reset()
    {
        ExitGames.Client.Photon.Hashtable healthProp = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_HEALTH, Constants.SOLDIER_HEALTH } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(healthProp);
        playerHealthText.text = $"Health {Constants.SOLDIER_HEALTH}%";
        if (photonView.IsMine) gameObject.transform.position = startPosition;
    }
}
