
using UnityEngine;
using VRTK;
using UnityEngine.Networking;

namespace WizardArena
{
    public class Wand : NetworkBehaviour
    {
        public GameObject projectilePrefab;

        private void Start()
        {
            GetComponent<VRTK_InteractableObject>().InteractableObjectUsed += new InteractableObjectEventHandler((sender, args) => CmdFire());
        }

        [Command]
        void CmdFire()
        {
            var projectile = Instantiate(
                projectilePrefab,
                transform.position + transform.up * .25f,
                transform.rotation);

            NetworkServer.Spawn(projectile);
        }
    }
}