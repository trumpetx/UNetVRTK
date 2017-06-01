using UnityEngine;
using UnityEngine.Networking;

namespace UNetVRTK
{
    /// <summary>
    /// Place this component on the "Player" prefab object with a Network Identity
    /// </summary>
    [RequireComponent(typeof(NetworkIdentity))]
    public class VRTKNetworkBridge : NetworkBehaviour
    {
        public static VRTKNetworkBridge localPlayerVRTKBridge;
        public override void OnStartLocalPlayer()
        {
            localPlayerVRTKBridge = this;
        }

        /// <summary>
        /// Currently, UNET does not allow us to request client authority from an unassigned object (even if it has no authority yet), 
        /// thus we use this bridge attached to the player's prefab to request on the behalf of other objects.
        /// </summary>
        /// <param name="identity">NetworkIdentity to get authority from</param>
        [Command]
        public void CmdAssignAuthority(NetworkIdentity identity)
        {
            bool assigned = false;
            // Don't take authority from someone else, if we already have authority there's no need to run this
            if (identity.clientAuthorityOwner == null)
            {
                assigned = identity.AssignClientAuthority(connectionToClient);
            }
            Debug.Log((assigned ? "Assigned" : "Unable to assign") + " authority to client");
        }

        /// <summary>
        /// Releasing authority would work fine from our object; however the Server (Host "client") does not have connectionToClient on the 
        /// object and only has it populated on the "player" object to which this is attached.  So, we use the bridge once again.
        /// </summary>
        /// <param name="identity">NetworkIdentity to remove authority from</param>
        [Command]
        public void CmdRemoveAuthority(NetworkIdentity identity)
        {
            bool removed = false;
            if (identity.clientAuthorityOwner == connectionToClient)
            {
                removed = identity.RemoveClientAuthority(connectionToClient);
            }
            Debug.Log((removed ? "Removed" : "Unable to remove") + " authority from client");
        }
    }
}