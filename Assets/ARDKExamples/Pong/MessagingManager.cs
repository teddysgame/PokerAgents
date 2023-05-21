// Copyright 2022 Niantic, Inc. All Rights Reserved.

using System;
using System.IO;

using Niantic.ARDK.Networking;
using Niantic.ARDK.Networking.MultipeerNetworkingEventArgs;
using Niantic.ARDK.Utilities.BinarySerialization;
using Niantic.ARDK.Utilities.BinarySerialization.ItemSerializers;

using UnityEngine;

namespace Niantic.ARDKExamples.Pong
{
    /// <summary>
    /// A manager that handles outgoing and incoming messages
    /// </summary>
    public class MessagingManager
    {
        // Reference to the networking object
        private IMultipeerNetworking _networking;

        // References to the local game controller and ball
        private GameController _controller;
        //private BallBehaviour _ball;

        private readonly MemoryStream _builderMemoryStream = new MemoryStream(24);

        // Enums for the various message types
        private enum _MessageType:
            uint
        {
            StartGameMessage,

            HostScoredMessage,
            GuestScoredMessage,

            GameOverMessage,

            NonHostPlayerMessage,
            NonHostOpponentMessage,

            GunOnTableMessage,
            HoldingGunMessage,

            //HostPositionMessage,
            //HostRotationMessage,
            //CardPositionMessage,
            //CardRotationMessage,
            SpawnGameObjectsMessage,
            LaunchSpeed
        }

        // Initialize manager with relevant data and references
        internal void InitializeMessagingManager
        (
            IMultipeerNetworking networking,
            GameController controller
        )
        {
            _networking = networking;
            _controller = controller;

            _networking.PeerDataReceived += OnDidReceiveDataFromPeer;
        }

		// Signal to host that a non-host has hit the ball, host should handle logic
		//internal void BallHitByPlayer(IPeer host, Vector3 direction)
		//{
		//    _networking.SendDataToPeer
		//    (
		//    (uint)_MessageType.BallHitMessage,
		//    SerializeVector3(direction),
		//    host,
		//    TransportType.UnreliableUnordered
		//    );
		//}

		// Signal to non-hosts that a Host has fired the gun
		//true = fired. False = null.
		internal void GameOver(bool ended)
		{
            //remember that byte is the number of array it can send.
			var message = new byte[1];

            //0 = Host Win
            if (ended)
				message[0] = 0;

			//1 = Guest Win
			else
				message[0] = 1;

			_networking.BroadcastData
			(
			(uint)_MessageType.GameOverMessage,
			message,
			TransportType.ReliableUnordered
			);
		}

		// Signal to non-hosts that a Host has scored
		//true = add marks. False = reduce marks.
		internal void HostScoring(bool scored)
        {
            var message = new byte[1];

            if (scored)
                //0 = added
                message[0] = 0;
            //1 = reduced
            else
                message[0] = 1;

            _networking.BroadcastData
            (
            (uint)_MessageType.HostScoredMessage,
            message,
            TransportType.ReliableUnordered
            );
        }

        // Signal to non-hosts that a Host has scored
        internal void StartGame(bool start)
        {
            var message = new byte[1];

            if (start)
                //0 = true. GameStart
                message[0] = 0;
            else
                //1 = false. GameEnded
                message[0] = 1;

            _networking.BroadcastData
            (
            (uint)_MessageType.StartGameMessage,
            message,
            TransportType.ReliableUnordered
            );
        }

		// Signal to non-hosts that a Host has scored
		//internal void GunOnTable(bool appear)
		//{
		//    var message = new byte[1];

		//    if (appear)
		//        //0 = true. Disappear
		//        message[0] = 0;
		//    else
		//        //1 = false. Appear
		//        message[0] = 1;

		//    _networking.BroadcastData
		//    (
		//    (uint)_MessageType.GunOnTableMessage,
		//    message,
		//    TransportType.ReliableUnordered
		//    );
		//}

		// Signal to non-hosts that a Host has drop gun
		internal void DropGun(bool appear)
		{
			var message = new byte[1];

			if (appear)
				//0 = true. Not holding
				message[0] = 0;
			//else
			//	//1 = false. Holding
			//	message[0] = 1;

			_networking.BroadcastData
			(
			(uint)_MessageType.HoldingGunMessage,
			message,
			TransportType.ReliableUnordered
			);
		}

		// Signal to non-hosts that a Host has scored
		internal void GuestScoring(bool scored)
        {
            var message = new byte[1];

            if (scored)
                //0 = scored
                message[0] = 0;
            //1 = reduced
            else
                message[0] = 1;

            _networking.BroadcastData
            (
            (uint)_MessageType.GuestScoredMessage,
            message,
            TransportType.ReliableUnordered
            );
        }

        //internal void BroadcastMyPoints(float points)
        //{
        //    //broadcast = I did this! now you know what I do.
        //    _networking.BroadcastData
        //    (
        //    (uint)_MessageType.MyPointsMessage,
        //    SerializeFloat(points),
        //    TransportType.UnreliableUnordered
        //    );
        //}

        internal void BroadcastNonHostPlayerNumber(float score)
        {
            //broadcast = I did this! now you know what I do.
            _networking.BroadcastData
            (
            (uint)_MessageType.NonHostPlayerMessage,
            SerializeFloat(score),
            TransportType.UnreliableUnordered
            );
        }

        internal void BroadcastNonHostOpponentNumber(float position)
        {
            //broadcast = I did this! now you know what I do.
            _networking.BroadcastData
            (
            (uint)_MessageType.NonHostOpponentMessage,
            SerializeFloat(position),
            TransportType.UnreliableUnordered
            );
        }


        internal void BroadcastLaunchToOpponent(IPeer player, float launchSpeed)
        {
            //SendDataToPeer = everyone do what I'm doing.
            _networking.SendDataToPeer
            (
                (uint)_MessageType.LaunchSpeed,
                SerializeFloat(launchSpeed),
                player,
                TransportType.UnreliableUnordered
            );
        }

        // Spawn game objects with a position and rotation
        internal void SpawnGameObjects(Vector3 position)
        {
            _networking.BroadcastData
            (
            (uint)_MessageType.SpawnGameObjectsMessage,
            SerializeVector3(position),
            TransportType.ReliableUnordered
            );
        }

        //to receive messages from other peers
        private void OnDidReceiveDataFromPeer(PeerDataReceivedArgs args)
        {
            //Debug.Log("Received message with tag: " + tag);

            var data = args.CopyData();
            switch ((_MessageType)args.Tag)
            {
                //for starting game
                case _MessageType.StartGameMessage:

                    //0 = true. Game Started
                    if (data[0] == 0)
                    {
                        Debug.Log("Game Started!");
                        _controller._isGameStarted = true;

                        //upon game start pressed by the host, guest will trigger the following
                        //enable the redraw button for everyone
                        _controller.RedrawButton.SetActive(true);

                        //display the how to play instructions
                        _controller.InstructionText.SetActive(false);
                    }
					else
					{
                        Debug.Log("Game Ended!");
                        _controller._isGameStarted = false;
                    }

                    break;

				//for Win Menu
				case _MessageType.GameOverMessage:
					//0 = Host Wins
					if (data[0] == 0)
					{
						Debug.Log("Host Wins!");
                        _controller.hostWins.SetActive(true);
					}
					//1 = Guest Wins
					else
					{
                        Debug.Log("Guest Wins!");
                        _controller.GuestWins.SetActive(true);
                    }

					break;

				//for holding gun
				case _MessageType.HoldingGunMessage:
					//0 = true. not holding
					if (data[0] == 0)
					{
						Debug.Log("Gun dropped!");
						_controller.dropOpponentGun();
					}
					//else
					//{
					//	Debug.Log("holding gun");
					//	_controller._opponent.GetComponent<Gun>().GunAppear();
					//}

					break;

				//for host scoring
				case _MessageType.HostScoredMessage:

                    //hostscore = _controller.HostScore;

                    //0 = host added score
                    if (data[0] == 0)
                    {
                        Debug.Log("Point scored for Host!");
                        _controller.hostScore += 1;

                        //opponent on the guest's phone fires and resets.
                        _controller.opponentFires();
                    }

                    //1 = host reduced
                    else
                    {
                        Debug.Log("Point reduced for Host!");
                        _controller.hostScore -= 1;

                        //opponent on the guest's phone fires and resets.
                        _controller.opponentFlags();

                        ////prevent from going negative
                        ///checked - needs to be called if not will go negative since the checking is not in the messenger
                        if (_controller.hostScore < 0)
						{
                            _controller.hostScore = 0;
						}
					}

                    _controller.scoreText.text =
                    string.Format
                    (
                        "P1: {0}\nP2: {1}",
                        _controller.hostScore,
                        _controller.guestScore
                    );

                    break;

                //for guest scoring
                case _MessageType.GuestScoredMessage:

                    //var guestscore = _controller.GuestScore;
                    //0 = guest scored
                    if (data[0] == 0)
                    {
                        Debug.Log("Point scored for Host!");
                        _controller.guestScore += 1;

                        //opponent on the guest's phone fires and resets.
                        _controller.opponentFires();
                    }
                    //1 = guest reduced
                    else
                    {
                        Debug.Log("Point reduced for Host!");
                        _controller.guestScore -= 1;

                        //opponent on the guest's phone fires and resets.
                        _controller.opponentFlags();

                        //prevent from going negative
                        if (_controller.guestScore < 0)
						{
                            _controller.guestScore = 0;

						}
					}

                    //_controller.scoreText.text =
                    //string.Format
                    //(
                    //    "Score: {0} - {1}",
                    //    _controller.hostScore,
                    //    _controller.guestScore
                    //);

                    _controller.scoreText.text =
                    string.Format
                    (
                        "P1: {0}\nP2: {1}",
                        _controller.hostScore,
                        _controller.guestScore
                    );

                    //check if got win - duplicate. already called in GameController
                    //if (_controller.WinScoreRequirement == _controller.GuestScore)
                    //{
                    //    _controller._isGameStarted = false;
                    //    Debug.Log("Guest Won!");
                    //}
                    //else
                    //{
                    //    //regenerate both players number
                    //    _controller.StartCoroutine(_controller.RegeneratePlayerNumber());
                    //}

                    break;

                case _MessageType.SpawnGameObjectsMessage:
                    Debug.Log("Creating game objects");
                    _controller.InstantiateObjects(DeserializeVector3(data));
                    break;

                //case _MessageType.MyPointsMessage:
                //    Debug.Log("Set Non-Host MyPoints");

                //    _controller.myNumber.text = DeserializeFloat(data).ToString();
                //    break;

                case _MessageType.NonHostPlayerMessage:
				    Debug.Log("Set Non-Host player number");

                    _controller.SetPlayerNumberForNonHost(DeserializeFloat(data));
				    break;

                case _MessageType.NonHostOpponentMessage:
                    Debug.Log("Set Non-Host player number");

                    _controller.SetOpponentNumberForNonHost(DeserializeFloat(data));
                    break;

                default:
            throw new ArgumentException("Received unknown tag from message");
            }
    }

        // Helper to serialize a Vector3 into a byte[] to be passed over the network
        private byte[] SerializeVector3(Vector3 vector)
        {
            _builderMemoryStream.Position = 0;
            _builderMemoryStream.SetLength(0);

            using (var binarySerializer = new BinarySerializer(_builderMemoryStream))
            Vector3Serializer.Instance.Serialize(binarySerializer, vector);

            return _builderMemoryStream.ToArray();
        }


        // Helper to deserialize a byte[] received from the network into a Vector3
        private Vector3 DeserializeVector3(byte[] data)
        {
          using(var readingStream = new MemoryStream(data))
            using (var binaryDeserializer = new BinaryDeserializer(readingStream))
              return Vector3Serializer.Instance.Deserialize(binaryDeserializer);
        }

        // Helper to serialize a quaterneon into a byte[] to be passed over the network
        //private byte[] SerializeQuaternion(Quaternion quaternion)
        //{
        //    _builderMemoryStream.Position = 0;
        //    _builderMemoryStream.SetLength(0);

        //    using (var binarySerializer = new BinarySerializer(_builderMemoryStream))
        //    QuaternionSerializer.Instance.Serialize(binarySerializer, quaternion);

        //    return _builderMemoryStream.ToArray();
        //}

        // Helper to deserialize a byte[] received from the network into a quaterneon
        //private Quaternion DeserializeQuaterneon(byte[] data)
        //{
        //    using (var readingStream = new MemoryStream(data))
        //    using (var binaryDeserializer = new BinaryDeserializer(readingStream))
        //        return QuaternionSerializer.Instance.Deserialize(binaryDeserializer);
        //}

        // Helper to serialize a float into a byte[] to be passed over the network
        private byte[] SerializeFloat(float floatnumber)
        {
            _builderMemoryStream.Position = 0;
            _builderMemoryStream.SetLength(0);

            using (var binarySerializer = new BinarySerializer(_builderMemoryStream))
                FloatSerializer.Instance.Serialize(binarySerializer, floatnumber);

            return _builderMemoryStream.ToArray();
        }

		// Helper to deserialize a byte[] received from the network into a Float
		private float DeserializeFloat(byte[] launchSpeed)
		{
			using (var readingStream = new MemoryStream(launchSpeed))
			using (var binaryDeserializer = new BinaryDeserializer(readingStream))
				return FloatSerializer.Instance.Deserialize(binaryDeserializer);
        }

        // Remove callback from networking object on destruction
        internal void Destroy()
        {
            _networking.PeerDataReceived -= OnDidReceiveDataFromPeer;
        }
	}
}
