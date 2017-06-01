using UnityEngine;
using UnityEngine.Networking;

namespace WizardArena
{
    public class Fireball : NetworkBehaviour
    {
        void Start()
        {
            GetComponent<Rigidbody>().velocity = transform.up * 12;
            Destroy(gameObject, 2);
        }

        void OnCollisionEnter(Collision collision)
        {

        }
    }
}