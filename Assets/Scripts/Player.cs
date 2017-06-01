using UnityEngine;
using UnityEngine.Networking;

namespace WizardArena
{
    public class Player : NetworkBehaviour
    {
        public GameObject wand;

        public override void OnStartLocalPlayer()
        {
            CmdSpawnWand();
        }

        [Command]
        void CmdSpawnWand()
        {
            Debug.Log("Wand spawned at: " + transform.position.ToString());
            NetworkServer.Spawn(Instantiate(
                wand,
                transform.position + transform.up * .25f,
                Quaternion.identity));
        }

    }
}