using UnityEngine;
using UnityEngine.Networking;

namespace WizardArena {
    public class NetworkManagerCustom : NetworkManager
    {
        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            Transform startPosition = GetStartPosition().transform;
            var player = Instantiate(playerPrefab, startPosition.position, startPosition.rotation);
            player.GetComponent<PlayerUpdater>().playerLayer = startPosition.gameObject.layer;
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }
        public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController playerController)
        {
            base.OnServerRemovePlayer(conn, playerController);
        }
    }
}