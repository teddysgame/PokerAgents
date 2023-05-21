// Copyright 2022 Niantic, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Niantic.ARDK.AR;
using Niantic.ARDK.AR.Configuration;
using Niantic.ARDK.AR.Networking;
using Niantic.ARDK.AR.HitTest;
using Niantic.ARDK.Networking;
using Niantic.ARDK.Networking.MultipeerNetworkingEventArgs;
using Niantic.ARDK.Utilities;
using Niantic.ARDK.Utilities.BinarySerialization;
using Niantic.ARDK.Utilities.Input.Legacy;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

public class P2PGameLogic : MonoBehaviour
{

    // A reference to the Camera component, set in the Editor.
    public Camera _camera;

    // A reference to the car prefab, set in the Editor.
    public GameObject _carPrefab;

    // A reference to the local car, created on the first tap after connecting to the session.
    private GameObject _localCar;

    // A constant representing a position event tag.
    private const uint POSITION_EVENT = 1;

    // The world tracking configuration, created during OnEnable().
    private IARWorldTrackingConfiguration _arWorldTrackingConfiguration;

    // The multipeer networking, created during OnEnable().
    private IMultipeerNetworking _multipeerNetworking;

    // The AR session, created during OnEnable().
    private IARSession _arSession;

    // The AR networking session, created during OnEnable().
    private IARNetworking _arNetworkingSession;

    // The session identifier.
    private readonly byte[] _sessionID = Encoding.UTF8.GetBytes("SampleSessionID");

    // A dictionary list of the players and their car GameObjects.
    private Dictionary<System.Guid, GameObject> _players = new Dictionary<System.Guid, GameObject>();

    private void OnEnable()
    {
        // First, create and configure the world tracking configuration...
        _arWorldTrackingConfiguration = ARWorldTrackingConfigurationFactory.Create();
        _arWorldTrackingConfiguration.WorldAlignment = WorldAlignment.Gravity;
        _arWorldTrackingConfiguration.IsLightEstimationEnabled = true;
        _arWorldTrackingConfiguration.PlaneDetection = PlaneDetection.Horizontal;
        _arWorldTrackingConfiguration.IsAutoFocusEnabled = true;
        _arWorldTrackingConfiguration.IsDepthEnabled = false;
        _arWorldTrackingConfiguration.IsSharedExperienceEnabled = true;

        // ...next, create the multipeer networking...
        _multipeerNetworking = MultipeerNetworkingFactory.Create();

        // ...then, create the AR session, passing in the stage identifier...
        _arSession = ARSessionFactory.Create(_multipeerNetworking.StageIdentifier);

        // ...next, create the AR networking session, passing in the AR session and the multipeer networking...
        _arNetworkingSession = ARNetworkingFactory.Create(_arSession, _multipeerNetworking);

        // ...finally, subscribe callback methods
        _multipeerNetworking.Connected += OnConnected;
        _multipeerNetworking.ConnectionFailed += OnConnectionFailed;
        _multipeerNetworking.Disconnected += OnDisconnected;
        _multipeerNetworking.PeerAdded += OnPeerAdded;
        _multipeerNetworking.PeerRemoved += OnPeerRemoved;
        _multipeerNetworking.PeerDataReceived += OnPeerDataReceived;
    }

    private void OnDisable() 
    {
        // Leave the networking session...
        LeaveNetworkingSession();

        // ...unsubscribe callback methods...
        _multipeerNetworking.Connected -= OnConnected;
        _multipeerNetworking.ConnectionFailed -= OnConnectionFailed;
        _multipeerNetworking.Disconnected -= OnDisconnected;
        _multipeerNetworking.PeerAdded -= OnPeerAdded;
        _multipeerNetworking.PeerRemoved -= OnPeerRemoved;
        _multipeerNetworking.PeerDataReceived -= OnPeerDataReceived;

        // ...and dispose of the objects.
        _arWorldTrackingConfiguration.Dispose();
        _multipeerNetworking.Dispose();
        _arSession.Dispose();
        _arNetworkingSession.Dispose();
    }

    private void OnApplicationQuit()
    {
        // Leave the networking session.
        LeaveNetworkingSession();
    }

    void Start()
    {
        // Run the AR networking session and join with the session ID...
        _arNetworkingSession.ARSession.Run(_arWorldTrackingConfiguration);
        _arNetworkingSession.Networking.Join(_sessionID);
    }

    void Update()
    {
        // If there are no touches detected, then return...
        if (PlatformAgnosticInput.touchCount <= 0) return;

        // ...otherwise, get the first touch...
        Touch touch = PlatformAgnosticInput.GetTouch(0);

        // ...and as long is it began on this frame...
        if (touch.phase == TouchPhase.Began)
        {
            // ...store the current frame and return if it's null for some reason...
            IARFrame currentFrame = _arNetworkingSession.ARSession.CurrentFrame;
            if (currentFrame == null) return;

            // ...then, perform a hit test and return if there's no result for some reason...
            ReadOnlyCollection<IARHitTestResult> hitTestResults = currentFrame.HitTest(_camera.pixelWidth,
                                                                                       _camera.pixelHeight,
                                                                                       touch.position,
                                                                                       ARHitTestResultType.EstimatedHorizontalPlane);
            if (hitTestResults.Count <= 0) return;

            // ...next, store the tapped position...
            Vector3 tappedPosition = hitTestResults[0].WorldTransform.ToPosition();

            // ...then, if there has not yet been a local car created...
            if (_localCar == null)
            {
                // ..create a local car at the touched position.
                _localCar = CreateCar(tappedPosition);
            }
            // ...but if a local car has already been created...
            else
            {
                // ...then set the local car's position.
                //_localCar.GetComponent<CarScript>().SetDestination(tappedPosition);
            }

            // ...and finally, broadcast this car's position to all other peers...
            BroadcastCarPosition(tappedPosition);
        }
    }

    // Creates a car at a given position.
    private GameObject CreateCar(Vector3 position)
    {
        GameObject createdCar = Instantiate(_carPrefab, _carPrefab.transform);
        createdCar.transform.position = position;
        return createdCar;
    }

    // Broadcasts this car's position at a given position.
    private void BroadcastCarPosition(Vector3 position)
    {
        // First, create a new memory stream object...
        MemoryStream memoryStream = new MemoryStream();

        // ...then, serialize the position into the memory stream...
        GlobalSerializer.Serialize(memoryStream, position);

        // ...next, store the memory stream as a byte array...
        byte[] data = memoryStream.ToArray();

        // ...and finally, send the byte array to all peers on the network.
        _arNetworkingSession.Networking.SendDataToPeers(tag: POSITION_EVENT,
                                                        data: data,
                                                        peers: _arNetworkingSession.Networking.OtherPeers,
                                                        TransportType.ReliableUnordered);
    }

    // Leaves the multiplayer session, if applicable.
    private void LeaveNetworkingSession()
    {
        if (_multipeerNetworking.IsConnected) _multipeerNetworking.Leave();
    }

    // Subscribes to the _multipeerNetworking.Connected event handler during OnEnable().
    private void OnConnected(ConnectedArgs args)
    {
        Debug.Log("Lightship: ---P2PGameLogic.OnConnected()---");
        Debug.Log("Lightship: args.IsHost = " + args.IsHost);
    }

    // Subscribes to the _multipeerNetworking.ConnectionFailed event handler during OnEnable().
    private void OnConnectionFailed(ConnectionFailedArgs args)
    {
        Debug.Log("Lightship: ---P2PGameLogic.OnConnectionFailed()---");
        Debug.Log("Lightship: args.ErrorCode = " + args.ErrorCode);
    }

    // Subscribes to the _multipeerNetworking.Disconnected event handler during OnEnable().
    private void OnDisconnected(DisconnectedArgs args)
    {
        Debug.Log("Lightship: ---P2PGameLogic.OnDisconnected()---");
    }

    // Subscribes to the _multipeerNetworking.PeerAdded event handler during OnEnable().
    private void OnPeerAdded(PeerAddedArgs args)
    {
        Debug.Log("Lightship: ---P2PGameLogic.OnPeerAdded()---");
        Debug.Log("Lightship: args.Peer = " + args.Peer);

        // Broadcast this car's position when a peer joins the session.
        if (_localCar != null)
        {
            BroadcastCarPosition(_localCar.transform.position);
        }
    }

    // Subscribes to the _multipeerNetworking.PeerRemoved event handler during OnEnable().
    private void OnPeerRemoved(PeerRemovedArgs args)
    {
        Debug.Log("Lightship: ---P2PGameLogic.OnPeerRemoved()---");
        Debug.Log("Lightship: args.Peer = " + args.Peer);

        // If the dictionary of players contains the peer that's been removed...
        if (_players.ContainsKey(args.Peer.Identifier))
        {
            // ...get a reference to that player's car...
            GameObject peerCar;
            if (_players.TryGetValue(args.Peer.Identifier, out peerCar))
            {
                // ...destroy the car...
                Destroy(peerCar);
            }
            // ...and remove that player from the dictionary.
            _players.Remove(args.Peer.Identifier);
        }
    }

    // Subscribes to the _multipeerNetworking.PeerDataReceived event handler during OnEnable().
    private void OnPeerDataReceived(PeerDataReceivedArgs args)
    {
        Debug.Log("Lightship: ---P2PGameLogic.OnPeerDataReceived()---");
        Debug.Log("Lightship: args.Peer = " + args.Peer);
        Debug.Log("Lightship: args.Tag = " + args.Tag);
        Debug.Log("Lightship: args.DataLength = " + args.DataLength);
        Debug.Log("Lightship: args.TransportType = " + args.TransportType);

        // First, copy the argument data into a memory stream...
        MemoryStream memoryStream = new MemoryStream(args.CopyData());

        // ...then, take note of the player identifier...
        System.Guid playerIdentifier = args.Peer.Identifier;

        switch (args.Tag)
        {
            // In the case of a position event...
            case POSITION_EVENT:
                
                // ...deserialize the position from the memory stream...
                Vector3 position = (Vector3) GlobalSerializer.Deserialize(memoryStream);

                // ...and if the dictionary already contains the player...
                if (_players.ContainsKey(playerIdentifier))
                {
                    // ...then set the player's car's destination.
                    //_players[playerIdentifier].GetComponent<CarScript>().SetDestination(position);
                }
                // ...but if the dictionary does not contain the player...
                else
                {
                    // ...then create a car for the remote player...
                    GameObject remoteCar = CreateCar(position);

                    // ...and add it to the dictionary.
                    _players.Add(playerIdentifier, remoteCar);
                }
                break;
        }
    }
}