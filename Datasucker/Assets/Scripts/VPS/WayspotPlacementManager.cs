// Copyright 2022 Niantic, Inc. All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Linq;

using Niantic.ARDK.AR;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.AR.HitTest;
using Niantic.ARDK.AR.WayspotAnchors;
using Niantic.ARDK.LocationService;
using Niantic.ARDK.Utilities;
using Niantic.ARDK.Utilities.Input.Legacy;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AnchorObject 
{
    public WayspotAnchorPayload Payload;
    public string Prefab;
    public Transform Transform;

    public AnchorObject(WayspotAnchorPayload payload, string prefab, Transform transform = null)
    {
        Payload = payload;
        Prefab = prefab;
        if (transform != null)
            Transform = transform;
    }
}

namespace Niantic.ARDKExamples.WayspotAnchors
{
    public class WayspotPlacementManager : MonoBehaviour
    {
        private GameObject _anchorPrefab;

        [Tooltip("Camera used to place the anchors via raycasting")]
        [SerializeField]
        private Camera _camera;

        [Tooltip("Text used to display the current status of the demo")]
        [SerializeField]
        private Text _statusLog;

        [Tooltip("Text used to show the current localization state")]
        [SerializeField]
        private Text _localizationStatus;

        private WayspotAnchorService _wayspotAnchorService;
        private IARSession _arSession;
        private LocalizationState _localizationState;

        private readonly Dictionary<Guid, GameObject> _wayspotAnchorGameObjects =
        new Dictionary<Guid, GameObject>();

        private IWayspotAnchorsConfiguration _config;

        public UnityEvent onLocalized;

        private bool _localizedReady = false;

        [SerializeField]
        private TextAsset _anchorJsonTextAsset;

        [SerializeField]
        private PrefabContainer _prefabContainer;

        [SerializeField]
        private bool CanPlace;

        private GameObject _latest;

        private void Awake()
        {
            // This is necessary for setting the user id associated with the current user. 
            // We strongly recommend generating and using User IDs. Accurate user information allows
            //  Niantic to support you in maintaining data privacy best practices and allows you to
            //  understand usage patterns of features among your users.  
            // ARDK has no strict format or length requirements for User IDs, although the User ID string
            //  must be a UTF8 string. We recommend avoiding using an ID that maps back directly to the
            //  user. So, for example, don�t use email addresses, or login IDs. Instead, you should
            //  generate a unique ID for each user. We recommend generating a GUID.
            // When the user logs out, clear ARDK's user id with ArdkGlobalConfig.ClearUserIdOnLogout

            //  Sample code:
            //  // GetCurrentUserId() is your code that gets a user ID string from your login service
            //  var userId = GetCurrentUserId(); 
            //  ArdkGlobalConfig.SetUserIdOnLogin(userId);

            _statusLog.text = "Initializing Session.";
        }

        public void SetPrefab(GameObject prefab)
        {
            _anchorPrefab = prefab;
        }


        private void OnEnable()
        {
            ARSessionFactory.SessionInitialized += HandleSessionInitialized;
            WayspotAnchorDataUtility.InitAnchorJson(_anchorJsonTextAsset);
            _prefabContainer.Init();
        }
        private void OnDisable()
        {
            ARSessionFactory.SessionInitialized -= HandleSessionInitialized;
        }

        private void OnDestroy()
        {
            if (_wayspotAnchorService != null)
            {
                _wayspotAnchorService.LocalizationStateUpdated -= OnLocalizationStateUpdated;
                _wayspotAnchorService.Dispose();
            }
        }

        private void Update()
        {
            if (_wayspotAnchorService == null)
            {
                _localizedReady = false;
                return;
            }
            //Get the pose where you tap on the screen
            var touchSuccess = TryGetTouchInput(out Matrix4x4 localPose);
            if (CanPlace && touchSuccess)
            {
                if (_wayspotAnchorService.LocalizationState == LocalizationState.Localized)
                {
                    PlaceAnchor(localPose); //Create the Wayspot Anchor and place the GameObject
                }
                else
                {
                    _statusLog.text = "Must localize before placing anchor.";
                }
            }
            
            _localizationStatus.text = _wayspotAnchorService.LocalizationState.ToString();
            _localizationState = _wayspotAnchorService.LocalizationState;
            if (!_localizedReady && _localizationState == LocalizationState.Localized)
            {
                _localizedReady = true;
                onLocalized.Invoke();
            }
        }

        /// Saves all of the existing wayspot anchors
        public void SaveWayspotAnchors()
        {
            if (_wayspotAnchorGameObjects.Count > 0)
            {
                var wayspotAnchors = _wayspotAnchorService.GetAllWayspotAnchors();

                var saveableAnchors = wayspotAnchors.Where(a =>
                    a.Status == WayspotAnchorStatusCode.Limited || a.Status == WayspotAnchorStatusCode.Success);
                var payloads = saveableAnchors.Select(a => a.Payload);
                var prefabs = saveableAnchors.Select(a => _wayspotAnchorGameObjects[a.ID]);

                WayspotAnchorDataUtility.SaveLocalPayloads(payloads.ToArray(), prefabs.ToArray());
            }
            else
            {
                WayspotAnchorDataUtility.SaveLocalPayloads(Array.Empty<WayspotAnchorPayload>(), Array.Empty<GameObject>());
            }

            _statusLog.text = $"Saved {_wayspotAnchorGameObjects.Count} Wayspot Anchors.";
        }

        /// Loads all of the saved wayspot anchors
        public void LoadWayspotAnchors()
        {
            if (_wayspotAnchorService.LocalizationState != LocalizationState.Localized)
            {
                _statusLog.text = "Must localize before loading anchors.";
                return;
            }
            var wayspots = WayspotAnchorDataUtility.LoadLocalPayloads();
            if (wayspots.Length > 0)
            {
                foreach (var wayspot in wayspots)
                {
                    var anchors = _wayspotAnchorService.RestoreWayspotAnchors(wayspot.Payload);
                    var prefabObject = _prefabContainer.Prefabs[wayspot.Prefab];
                    _statusLog.text = "0";
                    if (anchors.Length == 0)
                    {
                        _statusLog.text = "0 length anchors list";
                        return; // error raised in CreateWayspotAnchors
                    }

                    CreateWayspotAnchorGameObject(anchors[0], wayspot.Transform.position, wayspot.Transform.rotation, false, prefabObject);
                }
                
                _statusLog.text = $"Loaded {_wayspotAnchorGameObjects.Count} anchors.";
            }
            else
            {
                _statusLog.text = "No anchors to load.";
            }
        }

        /// Clears all of the active wayspot anchors
        public void ClearAnchorGameObjects()
        {
            if (_wayspotAnchorGameObjects.Count == 0)
            {
                _statusLog.text = "No anchors to clear.";
                return;
            }

            foreach (var anchor in _wayspotAnchorGameObjects)
            {
                Destroy(anchor.Value);
            }

            _wayspotAnchorService.DestroyWayspotAnchors(_wayspotAnchorGameObjects.Keys.ToArray());
            _wayspotAnchorGameObjects.Clear();
            _statusLog.text = "Cleared Wayspot Anchors.";
        }

        /// Pauses the AR Session
        public void PauseARSession()
        {
            if (_arSession.State == ARSessionState.Running)
            {
                _arSession.Pause();
                _statusLog.text = $"AR Session Paused.";
            }
            else
            {
                _statusLog.text = $"Cannot pause AR Session.";
            }
        }

        /// Resumes the AR Session
        public void ResumeARSession()
        {
            if (_arSession.State == ARSessionState.Paused)
            {
                _arSession.Run(_arSession.Configuration);
                _statusLog.text = $"AR Session Resumed.";
            }
            else
            {
                _statusLog.text = $"Cannot resume AR Session.";
            }
        }

        /// Restarts Wayspot Anchor Service
        public void RestartWayspotAnchorService()
        {
            _wayspotAnchorService.Restart();
            _localizedReady = false;
        }

        private void HandleSessionInitialized(AnyARSessionInitializedArgs args)
        {
            _statusLog.text = "Running Session...";
            _arSession = args.Session;
            _arSession.Ran += HandleSessionRan;
        }

        private void HandleSessionRan(ARSessionRanArgs args)
        {
            _arSession.Ran -= HandleSessionRan;
            _wayspotAnchorService = CreateWayspotAnchorService();
            _wayspotAnchorService.LocalizationStateUpdated += OnLocalizationStateUpdated;
            _statusLog.text = "Session running.";
        }

        private void OnLocalizationStateUpdated(LocalizationStateUpdatedArgs args)
        {
            _localizationStatus.text =
                $"Localization Status: {args.State} " +
                (args.State == LocalizationState.Failed ? $"(Reason: {args.FailureReason})" : "");
        }
        private WayspotAnchorService CreateWayspotAnchorService()
        {
            var locationService = LocationServiceFactory.Create(_arSession.RuntimeEnvironment);
            locationService.Start();

            if (_config == null)
            {
                _config = WayspotAnchorsConfigurationFactory.Create();
            }

            var wayspotAnchorService = new WayspotAnchorService(_arSession, locationService, _config);

            return wayspotAnchorService;
        }

        private void PlaceAnchor(Matrix4x4 localPose)
        {
            var anchors = _wayspotAnchorService.CreateWayspotAnchors(localPose);
            if (anchors.Length == 0)
            {
                return; // error raised in CreateWayspotAnchors
            }

            var position = localPose.ToPosition();
            var rotation = localPose.ToRotation();
            CreateWayspotAnchorGameObject(anchors[0], position, rotation, true);

            _statusLog.text = "Anchor placed.";
        }

        private GameObject CreateWayspotAnchorGameObject(IWayspotAnchor anchor, Vector3 position, Quaternion rotation,
            bool startActive, GameObject gameObject = null)
        {
            anchor.TransformUpdated += HandleWayspotAnchorTrackingUpdated;
            var id = anchor.ID;
            GameObject go;
            if (gameObject != null) 
            {
                go = Instantiate(gameObject, position, rotation);
            }
            else {
                go = Instantiate(_anchorPrefab.gameObject, position, rotation);
            }

            go.SetActive(startActive);
            _wayspotAnchorGameObjects.Add(id, go);
            _latest = go;

            return go;
        }

        private void HandleWayspotAnchorTrackingUpdated(WayspotAnchorResolvedArgs wayspotAnchorResolvedArgs)
        {
            var anchor = _wayspotAnchorGameObjects[wayspotAnchorResolvedArgs.ID].transform;
            anchor.position = wayspotAnchorResolvedArgs.Position;
            anchor.rotation = wayspotAnchorResolvedArgs.Rotation;
            anchor.gameObject.SetActive(true);
        }

        private bool TryGetTouchInput(out Matrix4x4 localPose)
        {
            if (_arSession == null || PlatformAgnosticInput.touchCount <= 0)
            {
                localPose = Matrix4x4.zero;
                return false;
            }

            var touch = PlatformAgnosticInput.GetTouch(0);
            if (touch.IsTouchOverUIObject())
            {
                localPose = Matrix4x4.zero;
                return false;
            }

            if (touch.phase != TouchPhase.Began)
            {
                localPose = Matrix4x4.zero;
                return false;
            }

            var currentFrame = _arSession.CurrentFrame;
            if (currentFrame == null)
            {
                localPose = Matrix4x4.zero;
                return false;
            }

            var results = currentFrame.HitTest
            (
                _camera.pixelWidth,
                _camera.pixelHeight,
                touch.position,
                ARHitTestResultType.ExistingPlane
            );

            int count = results.Count;
            if (count <= 0)
            {
                localPose = Matrix4x4.zero;
                return false;
            }

            var result = results[0];
            localPose = result.WorldTransform;
            return true;
        }

        public void SetConfig(IWayspotAnchorsConfiguration config) 
        {
            _config = config;
        }

        public void UpdatePrefabTransform(Transform delta) 
        {
            if (_latest == null)
            {
                return;
            }

            _latest.transform.position += transform.position;
            _latest.transform.rotation *= transform.rotation;
        }
    }
}
