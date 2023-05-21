// Copyright 2022 Niantic, Inc. All Rights Reserved.

using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.AR.HitTest;
using Niantic.ARDK.AR.Networking;
using Niantic.ARDK.AR.Networking.ARNetworkingEventArgs;
using Niantic.ARDK.Extensions;
using Niantic.ARDK.Networking;
using Niantic.ARDK.Networking.MultipeerNetworkingEventArgs;
using Niantic.ARDK.Utilities;
using Niantic.ARDK.Utilities.Input.Legacy;

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

using Niantic.LightshipHub.Templates;

namespace Niantic.ARDKExamples.Pong
{
    /// Controls the game logic and creation of objects
    public class GameController: MonoBehaviour
    {
        [Header("General")]
        /// Reference to AR Camera, used for hit test
        [SerializeField]
        private Camera _camera = null;

        [SerializeField]
        private FeaturePreloadManager preloadManager = null;

        [Header("UI")]
        [SerializeField]
        private Button joinButton = null;

        [Header("UI")]
        public GameObject ScanAnimUI;
        public GameObject JoinGroup;
        public GameObject RedrawButton;
        public GameObject InstructionText;

        [SerializeField]
        private GameObject startGameButton = null;

        public Text scoreText;

        /// Prefabs to be instantiated when the game starts
        [Header("Gameplay Prefabs")]
        [SerializeField]
        private GameObject playingFieldPrefab = null;

        [SerializeField]
        private GameObject playerPrefab = null;
        [SerializeField]
        private GameObject opponentPrefab = null;

        /// References to game objects after instantiation
        private GameObject _player;
        private GameObject _playingField;
        private GameObject _opponent;

        [Header ("Score")]
        //host does not mean phone 1, and guest does not mean phone 2.
        //it just means whatever is on your phone, is the host.
        internal int playerPoints;
        internal int opponentPoints;
		internal int hostScore = 0;
		internal int guestScore = 0;
		public int WinScoreRequirement = 2;
        public GameObject hostWins;
        public GameObject GuestWins;
        public TextMeshProUGUI myNumber;

        

        /// Cache your location every frame
        private Vector3 _location;

        private bool _objectsSpawned;

        private IARNetworking _arNetworking;

        public MessagingManager messagingManager;

        private IPeer _host;
        private IPeer _self;
        private bool _isHost;

        //[Header ("Networks")]
        //public string myID;
        //public string imposterID;
        //bool registerBool = false;
        //int playerIndex = 0;
        //public string[] playerlist = new string[5];

        //determine winner
        public bool holdingGun = false;

        public bool _isGameStarted;
        private bool _isSynced;

        //private bool isTouchingButton = false;
        private bool fieldplaced = false;

        //disable function when pressing buttons
        private bool isUIButtonPressed = false;

        [Header("BGM")]
        public AudioClip bgm;
        private AudioSource audioSource;

        private void Start()
        {
            //run the networking session
            startGameButton.SetActive(false);
            ARNetworkingFactory.ARNetworkingInitialized += OnAnyARNetworkingSessionInitialized;
            if (preloadManager.AreAllFeaturesDownloaded())
            OnPreloadFinished(true);
            else
            preloadManager.ProgressUpdated += PreloadProgressUpdated;

            audioSource = GetComponent<AudioSource>();
        }

        private void PreloadProgressUpdated(FeaturePreloadManager.PreloadProgressUpdatedArgs args)
        {
            if (args.PreloadAttemptFinished)
            {
            preloadManager.ProgressUpdated -= PreloadProgressUpdated;
            OnPreloadFinished(args.FailedPreloads.Count == 0);
            }
        }

        private void OnPreloadFinished(bool success)
        {
            //allow to join only preload has finished
            if (success)
            joinButton.interactable = true;
            else
            Debug.LogError("Failed to download resources needed to run AR Multiplayer");
        }

        // When all players are ready, create the game. Only the host will have the option to call this
        public void StartGame()
        {
            //if objects are not spawned yet, then call the InstantiateObjects function at a location as found in the function
            if (!_objectsSpawned)
            InstantiateObjects(_location);

            //game started then disable start button
            startGameButton.SetActive(false);
            _isGameStarted = true;

            //tell the guest's phone to enable _isGameStarted = true
            messagingManager.StartGame(_isGameStarted);

			if (_isHost)
			{
                audioSource.PlayOneShot(bgm);

                // Start the repeat loop
                InvokeRepeating("PlayAudioClip", bgm.length, bgm.length);
            }

            

            //create the imposter role
            // Get a random index
            //int randomIndex = Random.Range(0, playerlist.Length);

            //// Access the value at the random index
            //string randomValue = playerlist[randomIndex];

            //// register the imposter ID
            //imposterID = randomValue;

            //if (myID == imposterID)
            //{
            //             Debug.Log("Host is the imposter!");
            //}
        }

        private void PlayAudioClip()
        {
            // Play the audio clip
            audioSource.PlayOneShot(bgm);
        }

        // Instantiate game objects
        //this is important as it ensures both players have the same gameobjects.
        internal void InstantiateObjects(Vector3 position)
        {
            //only if playing field is null
            if (_playingField != null)
            {
                Debug.Log("Relocating the playing field!");
                _playingField.transform.position = position;

                //this is used to place the position of the player depending on host a not
                var offset = _isHost ? new Vector3(0, 0, -2) : new Vector3(0, 0, 2);

                // Instantiate the player and opponent avatars at opposite sides of the field
                _player.transform.position = position + offset;
                //place the opponent on the opposite by switching the offset to negative
                offset.z *= -1;

                _opponent.transform.position = position + offset;
                //_opponent.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                if (_isHost)
				    messagingManager.SpawnGameObjects(position);

			    return;
		    }

            scoreText.text = string.Format("P1: {0}\nP2: {1}", hostScore, guestScore);

            // Instantiate the playing field at floor level
            Debug.Log("Instantiating the playing field!");
            _playingField = Instantiate(playingFieldPrefab, position + new Vector3(0,-0.2f,0), Quaternion.identity);

            // Determine the starting location for the local player based on whether or not it is host
            var startingOffset = _isHost ? new Vector3(0, 0, -2) : new Vector3(0, 0, 2);

            //instantiate the prefabs
            _player = Instantiate(playerPrefab, position + startingOffset, Quaternion.identity);
            //_player = Instantiate(playerPrefab);

            //put the player inside the camera
            GameObject parentObject = Camera.main.gameObject;
            _player.transform.parent = parentObject.transform;
            _player.name = "Player 1 Body";

			//generate a number for Host only
			//if (_isHost)
			//{
                GeneratePlayerNumber(_player);
			//}

            startingOffset.z *= -1;

            //Enter score into _opponent body
            _opponent = Instantiate(opponentPrefab, position + startingOffset, Quaternion.Euler(0f, 0f, 0f));
            //_opponent = Instantiate(opponentPrefab);
            _opponent.name = "Player 2 Body";

            //generate a number for opponent
            //if (_isHost)
            //{
                GeneratePlayerNumber(_opponent);
            //}
            
            _objectsSpawned = true;

            if (!_isHost)
            return;

            messagingManager.SpawnGameObjects(position);
        }

        //regenerate number with button press
        public void ChangeNumber()
		{
            GeneratePlayerNumber(_player);
        }

        private void GeneratePlayerNumber(GameObject player)
        {
            PlayerInfo playerInfo = _player.GetComponent<PlayerInfo>();

            if (player != _player)
            {
                playerInfo = _opponent.GetComponent<PlayerInfo>();
            }

            var playerNumber = Random.Range(0, 10);

            // Check if Player 2 got the same number as Player 1
            if (playerNumber == playerInfo.score)
            {
                Debug.Log("Player 2 got the same number as Player 1. Regenerating...");
                GeneratePlayerNumber(player);
            }
			else
            {
                //register the player number
                playerInfo.EnterScore(playerNumber);

                //convert int (playerNumber) to float
                float playerNumberfloat = (float)playerNumber;

                //send to guest and update their player and opponent numbers
                if (player == _player)
                {
                    //if i'm the player, send as opponent for guest player
                    messagingManager.BroadcastNonHostOpponentNumber(playerNumberfloat);
                }
				else
				{
                    //if i'm the guest, send as host for guest player
                    messagingManager.BroadcastNonHostPlayerNumber(playerNumberfloat);
                }
                
            }

			//display player number on screen
			//if (player == _player)
			//{
			//	myNumber.text = "your number is: " + playerNumber.ToString();
			//}
			//else
			//{
                //update the myNumber Text
                //myNumber.text = "your number is: " + playerNumber.ToString();
                //for non host to update the mypoints
                //messagingManager.BroadcastMyPoints(playerNumber);
			//}
        }

        //set numbers for nonhost
        internal void SetPlayerNumberForNonHost(float number)
        {
            int intNumber = (int)number;

            PlayerInfo playerInfo = _player.GetComponent<PlayerInfo>();

            //register the player number
            playerInfo.EnterScore(intNumber);
        }

        internal void SetOpponentNumberForNonHost(float number)
        {
            int intNumber = (int)number;

            PlayerInfo playerInfo = _opponent.GetComponent<PlayerInfo>();

            //register the player number
            playerInfo.EnterScore(intNumber);
        }

        private void PullTrigger()
        {
            //as long as you 
            playerPoints = _player.GetComponent<PlayerInfo>().score;
            opponentPoints = _opponent.GetComponent<PlayerInfo>().score;

            if (_isHost)
            {
                if (playerPoints > opponentPoints)
                {
                    //fire animation
                    _player.GetComponentInChildren<Gun>().FireAnim();
                    //prevent doubleshot
                    //ableToFire = false;
                    //StartCoroutine(cooldown());
                    //increase host score by 1 point
                    Debug.Log("Host wins 1 point!");

                    //true = scored 1 point. False = reduce 1 point.
                    HostScored(true);
                }
                else
                {
                    _player.GetComponentInChildren<Gun>().FlagOut();

                    //prevent doubleshot
                    //ableToFire = false;
                    //StartCoroutine(cooldown());

                    //reduce host score by 1 point
                    Debug.Log("Guest wins 1 points!");

                    //true = scored 1 point. False = reduce 1 point.
                    HostScored(false);
                }
            }
            else
            {
                if (playerPoints > opponentPoints)
                {
                    //increase guest score by 1 point
                    Debug.Log("Guest wins 1 point!");

                    //fire animation
                    _player.GetComponentInChildren<Gun>().FireAnim();
                    //prevent doubleshot
                    //ableToFire = false;
                    //StartCoroutine(cooldown());

                    //true = scored 1 point. False = reduce 1 point.
                    GuestScored(true);
                }
                else
                {
                    //increase guest score by 2
                    Debug.Log("Host wins 2 points!");

                    //flagout
                    _player.GetComponentInChildren<Gun>().FlagOut();

                    //prevent doubleshot
                    //ableToFire = false;
                    //StartCoroutine(cooldown());

                    //true = scored 1 point. False = reduce 1 point.
                    GuestScored(false);
                }
            }
        }

        //make the opponent player drop the gun
        public void dropPlayerGun()
        {
            //true = drop gun
            //attach this to button
            messagingManager.DropGun(true);
        }

        //make the opponent player drop the gun
        public void dropOpponentGun()
        {
            _opponent.GetComponentInChildren<Gun>().PutDownGun();
        }

        //make the opponent player fires the gun and drop the gun
        public void opponentFires()
		{
            _opponent.GetComponentInChildren<Gun>().fireOpponentAnim();
        }

        //make the opponent player fires the gun and drop the gun
        public void opponentFlags()
        {
            _opponent.GetComponentInChildren<Gun>().FlagOut();
        }

        //      IEnumerator cooldown()
        //{
        //          yield return new WaitForSeconds(3);
        //          ableToFire = true;
        //}

        // Only the host should call this method
        private void HostScored(bool scored)
        {
            // Determine the host scored or penaltied
            if (scored)
            {
                //update the host score
                Debug.Log("Point scored for Host");
                hostScore += 1;
            }
            else
            {
                Debug.Log("Point reduced for Host");
                hostScore -= 1;

                //do not allow score to reduce below 0
                if (hostScore < 0)
                {
                    hostScore = 0;
                }
            }

            scoreText.text = string.Format("P1: {0}\nP2: {1}", hostScore, guestScore);

            //update the Guest score
            //true = add marks. False = reduce marks.
            messagingManager.HostScoring(scored);

            //let animation play for 3 seconds before restarting everything on the table.
            //StartCoroutine(ResetGunFromHandToTable());

            //check win conditions
            if (WinScoreRequirement == hostScore)
			{
                _isGameStarted = false;
                //tell the guest's phone to disable _isGameStarted = false

                //turn guest _isGameStarted to false - end game.
                messagingManager.StartGame(_isGameStarted);
                Debug.Log("Host Won!");

                HostWin();
            }
			else
			{
                //regenerate both players number
                StartCoroutine(RegeneratePlayerNumber());
			}

        }

        public void HostWin()
		{
            //show the win text
            hostWins.SetActive(true);

            //true = show host wins
            messagingManager.GameOver(true);
        }

        

        // Only the guest should call this method
        private void GuestScored(bool scored)
        {
            //true = add score.
            // Determine the host scored or penaltied
            if (scored)
            {
                Debug.Log("Point scored for Guest");
                guestScore += 1;
            }
            //false = reduce score
            else
            {
                Debug.Log("Point reduced for Guest");
                guestScore -= 1;

                //do not allow score to reduce below 0
                if (guestScore < 0)
                {
                    guestScore = 0;
                }
            }

            //scoreText.text = string.Format("Score: {0} - {1}", hostScore, guestScore);
            scoreText.text = string.Format("P1: {0}\nP2: {1}", hostScore, guestScore);


            messagingManager.GuestScoring(scored);

            //let animation play for 3 seconds before restarting everything on the table.
            //StartCoroutine(ResetGunFromHandToTable());

            //holdingGun = false;

            //check win conditions
            if (WinScoreRequirement == guestScore)
            {
                _isGameStarted = false;
                Debug.Log("Guest Won!");

                //tell the guest's phone to disable _isGameStarted = false
                messagingManager.StartGame(_isGameStarted);

                //show the win text
                GuestWin();
            }
            else
            {
                //regenerate both players number
                StartCoroutine(RegeneratePlayerNumber());
            }
        }

        public void GuestWin()
        {
            //show the win text
            GuestWins.SetActive(true);

            //false = show guest wins
            messagingManager.GameOver(false);
        }

        //      IEnumerator ResetGunFromHandToTable()
        //{
        //yield return new WaitForSeconds(3);

        //player's own gun disappears.
        //_player.GetComponentInChildren<Gun>().GunDisappear();

        //Gun is put back on table. gun disppear equals true
        //table table = UnityEngine.Object.FindObjectOfType<table>();
        //table.GunAppear();

        //make nonhost opponent's gun disppear
        //messagingManager.NotHoldingGun(false);

        //make nonhost table's gun disppear
        //messagingManager.GunOnTable(true);

        //holdingGun = false;
        //}

        internal IEnumerator RegeneratePlayerNumber()
		{
            yield return new WaitForSeconds(1);
            //generate a number for host
            GeneratePlayerNumber(_player);
            //generate a number for guest
            GeneratePlayerNumber(_opponent);
        }


        // Every frame, detect if you have hit the ball
        private void Update()
        {
            // Only allow tap on the screen when the game has started and is running
            if (_isSynced && _isGameStarted)
            {
                if (PlatformAgnosticInput.touchCount <= 0)
                    return;

                var touch = PlatformAgnosticInput.GetTouch(0);
                if (Input.GetMouseButtonDown(0) || touch.phase == TouchPhase.Began)
                {
                    //if (!fieldplaced)
                    //{
                    //    // On touch, it will retrieve the distance in front of you to be used in the FindFieldLocation() on line 229
                    //    var startGameDistance = Vector2.Distance(touch.position, new Vector2(startGameButton.transform.position.x, startGameButton.transform.position.y));

                    //    if (startGameDistance > 80)
                    //        FindFieldLocation(touch);

                    //    fieldplaced = true;
                    //    return;
                    //}
                }
                else if (Input.GetMouseButtonUp(0) || touch.phase == TouchPhase.Ended)
                {
                    // If holding gun and UI button is not pressed
                    if (holdingGun && !isUIButtonPressed)
                    {
                        
                        PullTrigger();
                    }
                }

                // Temporary disabled click on start to get score
                //holdingGun = true;

                //if (fieldplaced)
                //{
                _player.transform.position = Camera.main.transform.position;
                _player.transform.rotation = Camera.main.transform.rotation;

                //update number
                myNumber.text = "your number is: " + _player.GetComponent<PlayerInfo>().score.ToString();
                //}

                
            }
        }

        // Call this function to disable function using the isUIButtonPressed
        public void OnUIButtonPressed()
        {
            isUIButtonPressed = true;
            Debug.Log("isUIButtonPressed = " + isUIButtonPressed);

            //after pressed, quickly enable again.
            StartCoroutine(refreshbutton());
        }

        IEnumerator refreshbutton()
		{
            yield return new WaitForSeconds(1);
            isUIButtonPressed = false;

        }

        //find a place to place field
        private void FindFieldLocation(Touch touch)
        {
            var currentFrame = _arNetworking.ARSession.CurrentFrame;
            if (currentFrame == null)
            return;

            var results =
            currentFrame.HitTest
            (
                _camera.pixelWidth,
                _camera.pixelHeight,
                touch.position,
                ARHitTestResultType.ExistingPlaneUsingExtent
            );

            if (results.Count <= 0)
            {
            Debug.Log("Unable to place the field at the chosen location. Can't find a valid surface");
            return;
            }

            // Get the closest result
            //instantiate the table
            var result = results[0];
            var hitPosition = result.WorldTransform.ToPosition();
            InstantiateObjects(hitPosition);
        }
    
        //=========important=======
        // Every updated frame, get our location from the frame data and move the local player's avatar
        private void OnFrameUpdated(FrameUpdatedArgs args)
        {
            //get player location based on camera
            _location = MatrixUtils.PositionFromMatrix(args.Frame.Camera.Transform);
      
            // do not run frame if there is no player
            if (_player == null)
            return;

            //_player.transform.position = _location;
        }

        private void OnPeerStateReceived(PeerStateReceivedArgs args)
        {
            if (_self.Identifier == args.Peer.Identifier)
            UpdateOwnState(args);
            else
            UpdatePeerState(args);
        }

        private void UpdatePeerState(PeerStateReceivedArgs args)
        {
            if (args.State == PeerState.Stable)
            {
                _isSynced = true;

                //disable the UI
                ScanAnimUI.SetActive(false);

                //disable the join group
                JoinGroup.SetActive(false);

                //display the how to play instructions
                InstructionText.SetActive(true);

                if (_isHost)
                    startGameButton.SetActive(true);
            }

            //populate the list
            //if (args.State.ToString() == "Stable")
            //{
            //    playerlist[playerIndex] = args.Peer.Identifier.ToString();
            //    playerIndex++;
            //}
        }

        private void UpdateOwnState(PeerStateReceivedArgs args)
        {
            string message = args.State.ToString();
            scoreText.text = message;
            Debug.Log("We reached state " + message);


            //create the list
            //if (!registerBool)
            //{
            //    //register every player's ID
            //    myID = args.Peer.Identifier.ToString();
            //    registerBool = true;
            //    Debug.Log("my ID is : " + myID);
            //}

			//populate the list
			//if (message == "Stable")
			//{
   //             playerlist[playerIndex] = args.Peer.Identifier.ToString();
   //             playerIndex++;
			//}
        }

		// Upon receiving a peer's location data, take its location and move its avatar
		private void OnPeerPoseReceived(PeerPoseReceivedArgs args)
		{
			if (_opponent == null)
				return;

			var peerLocation = MatrixUtils.PositionFromMatrix(args.Pose);

            _opponent.transform.position = peerLocation;

        }

        private void OnDidConnect(ConnectedArgs args)
        {
            _self = args.Self;
            _host = args.Host;
            _isHost = args.IsHost;

        }

        private void OnAnyARNetworkingSessionInitialized(AnyARNetworkingInitializedArgs args)
        {
            _arNetworking = args.ARNetworking;
            _arNetworking.PeerPoseReceived += OnPeerPoseReceived;
            _arNetworking.PeerStateReceived += OnPeerStateReceived;
            _arNetworking.ARSession.FrameUpdated += OnFrameUpdated;
            _arNetworking.Networking.Connected += OnDidConnect;

            messagingManager = new MessagingManager();
            messagingManager.InitializeMessagingManager(args.ARNetworking.Networking, this);
        }

        private void OnDestroy()
        {
            ARNetworkingFactory.ARNetworkingInitialized -= OnAnyARNetworkingSessionInitialized;

            if (_arNetworking != null)
            {
            _arNetworking.PeerPoseReceived -= OnPeerPoseReceived;
            _arNetworking.PeerStateReceived -= OnPeerStateReceived;
            _arNetworking.ARSession.FrameUpdated -= OnFrameUpdated;
            _arNetworking.Networking.Connected -= OnDidConnect;
            }

			if (messagingManager != null)
			{
				messagingManager.Destroy();
				messagingManager = null;
			}
		}
	}
}
