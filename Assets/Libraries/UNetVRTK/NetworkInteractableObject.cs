using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using VRTK;

namespace UNetVRTK
{
    [RequireComponent(typeof(VRTK_InteractableObject)), RequireComponent(typeof(NetworkTransform))]
    public class NetworkInteractableObject : NetworkBehaviour
    {
        private bool isKinematicOnStart;

        [HideInInspector]
        [SyncVar(hook = "OnIsGrabbedUpdate")]
        public bool isGrabbed;

        // Transient variables used when authority is not received and we need to visually put the object back (it never moved on the server)
        private Vector3 grabPosition;
        private Quaternion grabRotation;

        private VRTK_InteractableObject interactibleObject;
        private NetworkIdentity networkIdentity;

        public NetworkIdentity[] additionalItemsToOwn;

        public override void OnStartClient()
        {
            base.OnStartClient();
            OnIsGrabbedUpdate(isGrabbed);
        }

        void Awake()
        {
            interactibleObject = GetComponent<VRTK_InteractableObject>();
            networkIdentity = GetComponent<NetworkIdentity>();
            isKinematicOnStart = interactibleObject.isKinematic;
        }
        
        public void OnEnable()
        {
            interactibleObject.InteractableObjectGrabbed += HandleGrab;
            interactibleObject.InteractableObjectUngrabbed += HandleUngrab;
        }
        
        public void OnDisable()
        {
            interactibleObject.InteractableObjectGrabbed -= HandleGrab;
            interactibleObject.InteractableObjectUngrabbed -= HandleUngrab;
        }
        
        private void HandleGrab(object sender, InteractableObjectEventArgs e)
        {
            grabPosition = transform.position;
            grabRotation = transform.rotation;

            // We can't request authority directly because we need authority to send a [Command]
            VRTKNetworkBridge.localPlayerVRTKBridge.CmdAssignAuthority(networkIdentity);
            foreach (NetworkIdentity identity in additionalItemsToOwn)
            {
                VRTKNetworkBridge.localPlayerVRTKBridge.CmdAssignAuthority(identity);
            }

            // This needs to be async becuase hasAuthority has not yet been set from the network
            StartCoroutine(AsyncHandleGrab(sender, DateTime.Now.AddMilliseconds(500)));
        }

        private IEnumerator AsyncHandleGrab(object sender, DateTime until)
        {
            yield return new WaitUntil(() => hasAuthority || DateTime.Now > until);
            if (hasAuthority)
            {
                CmdGrabChange(true);
            }
            else
            {
                ((VRTK_InteractableObject)sender).ForceStopInteracting();
                transform.position = grabPosition;
                transform.rotation = grabRotation;
            }
        }

        private void HandleUngrab(object sender, InteractableObjectEventArgs e)
        {
            if (hasAuthority)
            {
                // "drop" before we relinquish authority
                CmdGrabChange(false);
                VRTKNetworkBridge.localPlayerVRTKBridge.CmdRemoveAuthority(networkIdentity);
                foreach (NetworkIdentity identity in additionalItemsToOwn)
                {
                    VRTKNetworkBridge.localPlayerVRTKBridge.CmdRemoveAuthority(identity);
                }
            }
        }

        /// <summary>
        /// Tell all other clients that this object is now "grabbed" (to set kinematic) or "ungrabbed" (to remove kinematic)
        /// </summary>
        [Command]
        private void CmdGrabChange(bool isGrabbed)
        {
            this.isGrabbed = isGrabbed;
        }

        /// <summary>
        /// When an object is grabbed by VRTK, it child's the object so movement is nice and smooth. 
        /// When the network assigns authority and it begins to move, gravity messes up with the prediction 
        /// becuase the other clients are unaware of the object/controller relationship.  Making the object 
        /// kinematic solves this problem.
        /// 
        /// Place any other "OnGrab/OnUngrab" code here
        /// </summary>
        /// <param name="isGrabbed">isGrabbed = kinematic</param>
        public void OnIsGrabbedUpdate(bool isGrabbed)
        {
            interactibleObject.isKinematic = isGrabbed ? true : isKinematicOnStart;
        }
    }
}
