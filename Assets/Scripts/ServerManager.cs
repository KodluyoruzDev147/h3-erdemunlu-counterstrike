using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class ServerManager : MonoBehaviourPunCallbacks
{
    public static Dictionary<int, GameObject> allPlayers;
    public Transform[] StartPositions;
    public GameObject Soldier;

    private void Start()
    {
        // Server Settings.
        PhotonNetwork.SendRate = 100;
        PhotonNetwork.SerializationRate = 100;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings(); // Server connections.

    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinOrCreateRoom("1", new RoomOptions { MaxPlayers = 20, IsOpen = true, IsVisible = true }, TypedLobby.Default);
    }


    public override void OnJoinedRoom() // In here actor numbers given to the players by Photon. Before here all players actor number is -1.
    {
        if (allPlayers == null) allPlayers = new Dictionary<int, GameObject>();

        
        GameObject newPlayer = PhotonNetwork.Instantiate(Soldier.name, StartPositions[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.position, Quaternion.identity, 0, null);
        allPlayers.Add(PhotonNetwork.LocalPlayer.ActorNumber, newPlayer);

        ExitGames.Client.Photon.Hashtable initialProps0 = new ExitGames.Client.Photon.Hashtable() { { Constants.SOLDIER_HEALTH, PlayerSetup.PlayerHealth } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps0);

        ExitGames.Client.Photon.Hashtable initialProps1 = new ExitGames.Client.Photon.Hashtable() { { Constants.SOLDIER_MAGAZINE, PlayerSetup.PlayerMagazine } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps1);

    }

    public enum PlayerSetup
    {
        PlayerHealth = 100,
        PlayerMagazine = 80,
    }
}
