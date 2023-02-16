using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using Niantic.ARDK.AR;
using Niantic.ARDK.Extensions;
using Niantic.ARDK.Utilities;
using Niantic.ARDK.Utilities.Input.Legacy;
using Niantic.ARDK.AR.HitTest;

using Niantic.ARDK.AR.WayspotAnchors;
using Niantic.ARDK.LocationService;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.AR.Configuration;

using UnityEngine;

public class ARGameLogic : MonoBehaviour
{
    private string LocalSaveKey = "my_wayspots";

    private IARSession session;
    public Camera camera;
    public GameObject objectPrefab;

    private bool InitialLocalizationFired = false;
    private WayspotAnchorService wayspotAnchorService;

    private Dictionary<System.Guid, GameObject> anchors = new Dictionary<System.Guid, GameObject>();

    void Start()
    {
      session = ARSessionFactory.Create();
      ARSessionFactory.SessionInitialized+=OnSessionInitialized;
    }

    void OnSessionInitialized(AnyARSessionInitializedArgs args){
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
    void OnSessionRan(ARSessionRanArgs args){
        
      var wayspotAnchorsConfiguration = WayspotAnchorsConfigurationFactory.Create();
      
      var locationService = LocationServiceFactory.Create(session.RuntimeEnvironment);
      locationService.Start();

      wayspotAnchorService = new WayspotAnchorService(session, locationService, wayspotAnchorsConfiguration);
      // Uncomment following line to clear local data. Run once and comment again.
      // ClearLocalReferences();
    }
    
    void Update()
    {
      if(wayspotAnchorService==null || wayspotAnchorService.LocalizationState != LocalizationState.Localized){
        return;
      }
      if(wayspotAnchorService.LocalizationState==LocalizationState.Localized && !InitialLocalizationFired){
        LoadLocalReference();
        InitialLocalizationFired = true;
      }

      if (PlatformAgnosticInput.touchCount <= 0) return;
      var touch = PlatformAgnosticInput.GetTouch(0);
      if (touch.phase == TouchPhase.Began) OnTapScreen(touch);
    }

    void OnTapScreen(Touch touch){

        var currentFrame = session.CurrentFrame;

        if (currentFrame == null) return;

        var hitTestResults = currentFrame.HitTest (
            camera.pixelWidth, 
            camera.pixelHeight, 
            touch.position, 
            ARHitTestResultType.EstimatedVerticalPlane
        );

        if (hitTestResults.Count <= 0) return;

        Matrix4x4 poseMatrix = hitTestResults[0].WorldTransform;//.ToPosition();

        AddAnchor(poseMatrix);
    }

    private void AddAnchor(Matrix4x4 poseData)
    {
      var anchors = wayspotAnchorService.CreateWayspotAnchors(poseData);
      OnWayspotAnchorsAdded(anchors);
    }


    private void OnWayspotAnchorsAdded(IWayspotAnchor[] wayspotAnchors)
    {
      foreach (var wayspotAnchor in wayspotAnchors)
      {
        if (anchors.ContainsKey(wayspotAnchor.ID)) continue;
        var id = wayspotAnchor.ID;
        var anchor = Instantiate(objectPrefab);
        anchor.SetActive(false);
        anchor.name = $"Anchor {id}";
        anchors.Add(id, anchor);

        wayspotAnchor.TransformUpdated += OnUpdateAnchorPose;

      }
      if(InitialLocalizationFired) SaveLocalReference();
    }

    private void OnUpdateAnchorPose(WayspotAnchorResolvedArgs wayspotAnchorResolvedArgs)
    {
      var anchor = anchors[wayspotAnchorResolvedArgs.ID].transform;
      anchor.position = wayspotAnchorResolvedArgs.Position;
      anchor.rotation = wayspotAnchorResolvedArgs.Rotation;
      anchor.gameObject.SetActive(true);
    }


    private void SaveLocalReference(){

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

    [Serializable]
    private class MyStoredAnchorsData
    {
      public string[] Payloads = Array.Empty<string>();
    }


    // HELPER FUNCTION TO CLEAR THE LOCAL WAYSPOT ANCHOR CACHE
    private void ClearLocalReferences()
    {
      if (PlayerPrefs.HasKey(LocalSaveKey))
      {
        wayspotAnchorService.DestroyWayspotAnchors(anchors.Keys.ToArray());
        PlayerPrefs.DeleteKey(LocalSaveKey); 
      }
    }
}