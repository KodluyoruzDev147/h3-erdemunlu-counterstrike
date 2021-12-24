using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class SoldierController : MonoBehaviourPunCallbacks
{

    public float defaultMoveMultiplier = 5, defaultRotateMultiplier = 1250;
    public float fastMoveMultiplier = 10f;
    public float soldierAngle;
    public Camera camera;
    private Animator animator;


    private void Start()
    {
        if (photonView.IsMine)
        {
            animator = GetComponent<Animator>();
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
                                                                                         
                UnityEngine.Debug.Log("Object tag: " + objectHit.tag);                   
                // Do something with the object that was hit by the raycast.
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
}
