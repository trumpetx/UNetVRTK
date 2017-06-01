using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace WizardArena
{
    public class PlayerUpdater : NetworkBehaviour
    {
        [SyncVar]
        public int playerLayer;

        private Transform leftController;
        private Transform rightController;
        private Transform head;

        public Transform leftControllerRender;
        public Transform rightControllerRender;
        public Transform headRender;

        void Start()
        {
            if (isLocalPlayer)
            {
                Debug.Log("Setting up local player");
                VRTK.VRTK_SDKManager sdkManager = VRTK.VRTK_SDKManager.instance;

                // Move the player to their starting location
                sdkManager.actualBoundaries.transform.position = transform.position;
                sdkManager.actualBoundaries.transform.rotation = transform.rotation;

                // Set up the network tracking objets
                leftController = sdkManager.actualLeftController.transform;
                rightController = sdkManager.actualRightController.transform;
                head = sdkManager.actualHeadset.transform;

                // Local color
                leftControllerRender.GetComponent<MeshRenderer>().material.color = Color.red;
                rightControllerRender.GetComponent<MeshRenderer>().material.color = Color.red;
                headRender.GetComponent<MeshRenderer>().material.color = Color.blue;
                leftControllerRender.GetComponent<MeshRenderer>().GetComponent<Collider>().enabled = false;
                rightControllerRender.GetComponent<MeshRenderer>().GetComponent<Collider>().enabled = false;
                headRender.GetComponent<MeshRenderer>().GetComponent<Collider>().enabled = false;

                // Set player's layer (Controls bounding box)
                sdkManager.GetComponentInChildren<VRTK.VRTK_BasicTeleport>().GetComponent<VRTK.VRTK_PolicyList>().identifiers.Add(LayerMask.LayerToName(playerLayer));
            }
            else
            {
                Debug.Log("Setting up network player");
                leftControllerRender.GetComponent<MeshRenderer>().material.color = Color.black;
                rightControllerRender.GetComponent<MeshRenderer>().material.color = Color.black;
                headRender.GetComponent<MeshRenderer>().material.color = Color.gray;
            }
        }

        void Update()
        {
            if (isLocalPlayer)
            {
                leftControllerRender.position = leftController.position;
                rightControllerRender.position = rightController.position;
                headRender.position = head.position;
                leftControllerRender.rotation = leftController.rotation;
                rightControllerRender.rotation = rightController.rotation;
                headRender.rotation = head.rotation;
            }
        }
    }
}