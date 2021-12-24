using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSetup : MonoBehaviourPunCallbacks
{

    private void Start()
    {
        if (photonView.IsMine)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

}
