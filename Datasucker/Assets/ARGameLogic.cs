using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using UnityEngine;

using Niantic.ARDK.AR;
using Niantic.ARDK.AR.Configuration;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.Extensions;
using Niantic.ARDK.Utilities.Input.Legacy;
using Niantic.ARDK.AR.HitTest;
using Niantic.ARDK.AR.WayspotAnchors;
using Niantic.ARDK.LocationService;

public class ARGameLogic : MonoBehaviour
{

    private string LocalSaveKey = "my_wayspots";
    private IARSession session;
    public Camera camera;
    public GameObject objectPrefab;
    private WayspotAnchorService wayspotAnchorService;
    private bool InitialLocalizationFired = false;
    private Dictionary<System.Guid, GameObject> anchors = new Dictionary<System.Guid, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        session = ARSessionFactory.Create();
        ARSessionFactory.SessionInitialized += OnSessionInitialized;
    }

    void OnSessionInitialized(AnyARSessionInitializedArgs args)
    {
        // Session Initialized
        var configuration = ARWorldTrackingConfigurationFactory.Create();

        configuration.WorldAlignment = WorldAlignment.Gravity;
        configuration.PlaneDetection = PlaneDetection.Vertical;

        configuration.IsLightEstimationEnabled = false;
        configuration.IsAutoFocusEnabled = false;
        configuration.IsDepthEnabled = false;
        configuration.IsSharedExperienceEnabled = false;

        session.Ran += OnSessionRan;
        session.Run(configuration);
    }

    void OnSessionStarted(ARSessionRanArgs args)
    {
        var wayspotAnchorsConfiguration = WayspotAnchorsConfigurationFactory.Create();

        var locationService = LocationServiceFactory.Create(session.RuntimeEnvironment);

        locationService.Start();

        wayspotAnchorService = new WayspotAnchorService(session, locationService, wayspotAnchorsConfiguration);
    }

    void OnSessionRan(ARSessionRanArgs args)
    {
        // Session Ran
    }

    // Update is called once per frame
    void Update()
    {

        if (wayspotAnchorService == null || wayspotAnchorService.LocalizationState != LocalizationState.Localized)
        {
            return;
        }

        if (wayspotAnchorService.LocalizationState == LocalizationState.Localized && !InitialLocalizationFired)
        {
            LoadLocalReference();
            InitialLocalizationFired = true;
            // Localization event, fired once
        }

        if (PlatformAgnosticInput.touchCount <= 0) return;
        var touch = PlatformAgnosticInput.GetTouch(0);
       
        if (touch.phase == TouchPhase.Began)
        {
            OnTapScreen(touch);
        }
    }

    void OnTapScreen(Touch touch)
    {
        // Touch event
        var currentFrame = session.CurrentFrame;

        if (currentFrame == null) return;

        var hitTestResults = currentFrame.HitTest (
            camera.pixelWidth,
            camera.pixelHeight,
            touch.position,
            ARHitTestResultType.EstimatedVerticalPlane
        );

        if (hitTestResults.Count <= 0) return;

        Matrix4x4 poseMatrix = hitTestResults[0].WorldTransform;

        AddAnchor(poseMatrix);
    }

    private void AddAnchor(Matrix4x4 poseData)
    {
        wayspotAnchorService.CreateWayspotAnchors(OnWayspotAnchorsAdded, poseData);
    }

    private void OnWayspotAnchorsAdded(IWayspotAnchor[] wayspotAnchors)
    {
        // Process Anchors
        foreach (var wayspotAnchor in wayspotAnchors)
        {
            if (anchors.ContainsKey(wayspotAnchor.ID)) continue;
            var id = wayspotAnchor.ID;
            var anchor = Instantiate(objectPrefab);
            anchor.SetActive(false);
            anchor.name = $"Anchor {id}";
            anchors.Add(id, anchor);

            wayspotAnchor.TrackingStateUpdated += OnUpdateAnchorPose;
        }

        if (InitialLocalizationFired) SaveLocalReference();
    }

    private void SaveLocalReference()
    {
        IWayspotAnchor[] wayspotAnchors = wayspotAnchorService.GetAllWayspotAnchors();

        MyStoredAnchorsData storedAnchorsData = new MyStoredAnchorsData();

        storedAnchorsData.Payloads = wayspotAnchors.Select(a => a.Payload.Serialize()).ToArray();

        string jsonData = JsonUtility.ToJson(storedAnchorsData);
        PlayerPrefs.SetString(LocalSaveKey, jsonData);
    }

    public void LoadLocalReference()
    {
        if (PlayerPrefs.HasKey(LocalSaveKey))
        {
            List<WayspotAnchorPayload> payloads = new List<WayspotAnchorPayload>();

            string json = PlayerPrefs.GetString(LocalSaveKey);
            MyStoredAnchorsData storedData = JsonUtility.FromJson<MyStoredAnchorsData>(json);

            foreach (var wayspotAnchorPayload in storedData.Payloads)
            {
                var payload = WayspotAnchorPayload.Deserialize(wayspotAnchorPayload);
                payloads.Add(payload);
            }

            if (payloads.Count > 0)
            {
                var wayspotAnchors = wayspotAnchorService.RestoreWayspotAnchors(payloads.ToArray());
                OnWayspotAnchorsAdded(wayspotAnchors);
            }
        }
    }

    private void OnUpdateAnchorPose(WayspotAnchorResolvedArgs wayspotAnchorResolvedArgs)
    {
        // Position Anchor Event
        var anchor = anchors[wayspotAnchorResolvedArgs.ID].transform;
        anchor.rotation = wayspotAnchorResolvedArgs.Rotation;
        anchor.position = wayspotAnchorResolvedArgs.Position;
        anchor.gameObject.SetActive(true);
    }

    [Serializable]
    private class MyStoredAnchorsData
    {
        public string[] Payloads = Array.Empty<string>();
    }
}
