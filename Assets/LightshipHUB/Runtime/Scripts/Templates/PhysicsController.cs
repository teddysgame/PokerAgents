// Copyright 2022 Niantic, Inc. All Rights Reserved.
using UnityEngine;

namespace Niantic.ARDKExamples.Pong
{
    public class PhysicsController : MonoBehaviour
    {
        //User preferences
        [Header("Object physics in runtime")]
        [Tooltip("Size of the object")]
        [SerializeField]
        [Range(0.01f, 5)]
        private float scale = 1;

        [Tooltip("Initial speed when spawning")]
        [SerializeField]
        [Range(0.5f, 5)]
        private float launchForce = 1;

        [SerializeField]
        [Range(0, 1)]
        private float bounciness;

        [Tooltip("Having zero friction should seem like standing on ice")]
        [SerializeField]
        [Range(0, 50)]
        private float friction = 0.6f;

        [Tooltip("Spin the object")]
        [SerializeField]
        [Range(0, 50)]
        private byte spiningForce;

        [Tooltip("Adds drag force")]
        [SerializeField]
        [Range(0, 5)]
        private float slowliness;

        [SerializeField]
        private bool rockSolid, zeroGravity;

        public Rigidbody rb;

        //public GameObject setactivefalse;

        //network
        // Cache the floor level, so the ball is reset properly
        private Vector3 _initialPosition;

        // Flags for whether the game has started and if the local player is the host
        //private bool _isGameStarted;
        private bool _isHost;

        /// Reference to the messaging manager
        private MessagingManager messagingManager;

        //for displaying card physics
        // Flags for whether the game has started and if the local player is the host
        private bool _isGameStarted;
        private Vector3 _pos;

        private void Start()
        {
            //rb = gameObject.GetComponent<Rigidbody>();

            ApplyUserPreferencesToTransform(gameObject);
            ApplyUserPreferencesToRigidbody(rb);
            ApplyUserPreferencesToCollider(gameObject);
        }

		//private void Update()
		//{
		//	if (!_isGameStarted)
		//		return;

		//	_pos = gameObject.transform.position;

		//	transform.position = _pos;

		//	messagingManager.BroadcastCardPosition(_pos);
		//}

		// Set up the initial conditions
		internal void GameStart(bool isHost, MessagingManager messagingManager1)
        {
            _isHost = isHost;
            //_isGameStarted = true;
            _initialPosition = transform.position;
            Debug.Log(_isHost);

            if (!_isHost)
                return;

            messagingManager = messagingManager1;
            //_velocity = new Vector3(_initialVelocity, 0, _initialVelocity);
        }

        private void ApplyUserPreferencesToTransform(GameObject obj)
        {
            Vector3 newScale = new Vector3(scale, scale, scale);
            obj.transform.localScale = newScale;
        }

        private void ApplyUserPreferencesToRigidbody(Rigidbody rig)
        {
            //if (rockSolid)
                //rig.constraints = RigidbodyConstraints.FreezeRotationX;
                //rig.constraints = RigidbodyConstraints.FreezeRotationZ;
            if (zeroGravity) rig.useGravity = false;
            rig.drag = slowliness;
            if (slowliness <= 1) rig.angularDrag = slowliness;
            //rig.AddTorque(transform.up * spiningForce / 2, ForceMode.Impulse);
        }

        private void ApplyUserPreferencesToCollider(GameObject obj)
        {
            if (obj == null) return;

            Collider col = obj.GetComponent<MeshCollider>();

            if (col != null)
            {
                col.material = new PhysicMaterial();
                col.material.bounciness = bounciness;
                AdjustBouncinessCombine(col);
                col.material.dynamicFriction = friction;
                col.material.staticFriction = friction;
            }

            foreach (Transform child in obj.transform)
            {
                if (null == child) continue;
                ApplyUserPreferencesToCollider(child.gameObject);
            }
        }

        private void AdjustBouncinessCombine(Collider coll)
        {
            if (bounciness >= .8) coll.material.bounceCombine = PhysicMaterialCombine.Maximum;
            if (bounciness >= .5 && bounciness < .8) coll.material.bounceCombine = PhysicMaterialCombine.Multiply;
            if (bounciness < .5) coll.material.bounceCombine = PhysicMaterialCombine.Average;
        }

        //remove spin card temporarily
        //public void Launch(float speed, float movementValue)
        public void Launch(float speed, GameObject player, GameObject card)
        {
            //cancel off movement value if too little
   //         if (movementValue < 70f && movementValue > -70f)
			//{
   //             movementValue = 0;
   //         }

            //Debug.Log(player);

            rb = card.gameObject.GetComponent<Rigidbody>();

            //place card on playerbody
            //ResetRigidbody(player, card);
            //gameObject.SetActive(true);

            // Calculate the spin direction based on the float value
            //float spinDirection = Mathf.Sign(movementValue);

            //speed is derived from the length of 2 points.
            //overall is reduced by 50%
            //rb.AddRelativeForce(Camera.main.transform.forward * speed/2 * launchForce, ForceMode.Impulse);

            //the code below allows the card to fly slightly left or right

            //launch only from your camera if you are the player
            //if (transform.CompareTag("Player2"))
            //{
            //             //Debug.Log("Player 2 launch");
            //             rb.AddRelativeForce((oppogameObject.transform.forward * speed / 2 + Camera.main.transform.right * movementValue / 100 / 1) * launchForce, ForceMode.Impulse);
            //         }
            //else
            //{
            //             //Debug.Log("Player 1 launch");
            //             rb.AddRelativeForce((Camera.main.transform.forward * speed / 2 + Camera.main.transform.right * movementValue / 100 / 1) * launchForce, ForceMode.Impulse);

            //rb.AddRelativeForce((Camera.main.transform.forward * speed / 2 + Camera.main.transform.right * movementValue / 100 / 1) * launchForce, ForceMode.Impulse);
            rb.AddRelativeForce(Vector3.forward * speed/10 * launchForce, ForceMode.Impulse);

            //float spinForce = Mathf.Abs(movementValue / 100) * 10;
            //Vector3 spinAxis = Vector3.up * Mathf.Sign(movementValue);
            //rb.AddTorque(spinAxis * spinForce, ForceMode.Impulse);


            //float defaultLaunchForce = 300.0f;
            //rb.AddForce(Camera.main.transform.forward * defaultLaunchForce);

            //Debug.Log(spinDirection);

            // Calculate the spin speed based on the velocity magnitude
            //float velocityMagnitude = rb.velocity.magnitude;
            //float maxVelocityMagnitude = 10.0f; // Adjust the maximum velocity magnitude as needed
            //float maxSpinSpeed = movementValue/10; // Adjust the maximum spin speed as needed
            //float spinSpeed = Mathf.Lerp(0f, maxSpinSpeed, 1f / (velocityMagnitude + 1f));

            //spinCard(spinDirection, movementValue);
        }

        //public void LaunchOpponent(float speed)
        //{
        //    //rb = GameObject.Find("Player 2")
        //    ResetRigidbody();
        //    gameObject.SetActive(true);

        //    rb.AddRelativeForce((Camera.main.transform.forward * speed / 2) * launchForce, ForceMode.Impulse);
        //}

        //      private void spinCard(float spinDirection, float movementValue)
        //{
        //          //rb = gameObject.GetComponent<Rigidbody>();

        //          //Debug.Log(movementValue/100/4);

        //          // Add spin to the object based on the spin direction and spin speed
        //          rb.AddTorque(Vector3.up * movementValue/100/4, ForceMode.Impulse);
        //      }


        public GameObject ResetRigidbody(GameObject player, GameObject card)
        {
            //setactivefalse.SetActive(true);

            rb = card.GetComponent<Rigidbody>();
            rb.position = Vector3.zero;
            //rb.rotation = Quaternion.identity;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero; // Add this line
            //transform.position = Camera.main.transform.position + Camera.main.transform.forward;
            card.transform.position = player.transform.position + new Vector3(0,0,0);
            //transform.rotation = Quaternion.identity;
            card.transform.rotation = player.transform.rotation;

            return card;
        }

        //to turn off game object when game starts
        public void SetActiveFalseGO()
		{
            //setactivefalse.SetActive(false);
		}
    }
}