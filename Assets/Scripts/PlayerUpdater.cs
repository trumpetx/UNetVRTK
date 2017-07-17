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

        private void OnEnable()
        {
            VRTK.VRTK_SDKManager.instance.LoadedSetupChanged += OnLoadedSetupChange;
        }

        private void OnDisable()
        {
            VRTK.VRTK_SDKManager.instance.LoadedSetupChanged -= OnLoadedSetupChange;
        }

        private void Start()
        {
            if (!isLocalPlayer)
            {
                Debug.Log("Setting up network player");
                leftControllerRender.GetComponent<MeshRenderer>().material.color = Color.black;
                rightControllerRender.GetComponent<MeshRenderer>().material.color = Color.black;
                headRender.GetComponent<MeshRenderer>().material.color = Color.gray;
            }
        }

        override public void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            Debug.Log("Setting up local player");
            VRTK.VRTK_SDKManager sdkManager = VRTK.VRTK_SDKManager.instance;

            if (sdkManager.loadedSetup != null)
            {
                VRTK.VRTK_SDKManager.instance.loadedSetup.actualBoundaries.transform.SetPositionAndRotation(transform.position, transform.rotation);
                OnLoadedSetupChange(sdkManager);
            }

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

        private void OnLoadedSetupChange(VRTK.VRTK_SDKManager sdkManager)
        {
            // Set up the network tracking objets
            leftController = sdkManager.loadedSetup.actualLeftController.transform;
            rightController = sdkManager.loadedSetup.actualRightController.transform;
            head = sdkManager.loadedSetup.actualHeadset.transform;
        }

        private void OnLoadedSetupChange(VRTK.VRTK_SDKManager sdkManager, VRTK.VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            if (isLocalPlayer)
            {
                VRTK.VRTK_SDKManager.instance.loadedSetup.actualBoundaries.transform.SetPositionAndRotation(transform.position, transform.rotation);
                OnLoadedSetupChange(sdkManager);
            }
        }
        private readonly object syncLock = new object();

        void Update()
        {
            if (isLocalPlayer && leftController != null && rightController != null)
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
