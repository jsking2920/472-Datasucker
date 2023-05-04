// Copyright 2022 Niantic, Inc. All Rights Reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Niantic.ARDK.AR.WayspotAnchors;

using UnityEngine;

public class AnchorsObjectData
{
    public List<string> Payloads;
    public List<string> Prefabs;
    public List<Vector3> Positions;
    public List<Quaternion> Rotations;
    public List<Vector3> Scales;
}

namespace Niantic.ARDKExamples.WayspotAnchors
{
    public static class WayspotAnchorDataUtility
    {
        private const string DataKey = "wayspot_anchor_payloads";
        private static string _anchorJson;

        public static void InitAnchorJson(TextAsset textAsset)
        {
            _anchorJson = textAsset.text;
        }

        public static void SaveLocalPayloads(WayspotAnchorPayload[] wayspotAnchorPayloads, GameObject[] prefabs)
        {
            AnchorsObjectData anchorsObjectData = new AnchorsObjectData();
            anchorsObjectData.Payloads = wayspotAnchorPayloads.Select(a => a.Serialize()).ToList();
            anchorsObjectData.Prefabs = prefabs.Select(a => a.name).ToList();
            anchorsObjectData.Positions = prefabs.Select(a => a.transform.position).ToList();
            anchorsObjectData.Rotations = prefabs.Select(a => a.transform.rotation).ToList();
            anchorsObjectData.Scales = prefabs.Select(a => a.transform.localScale).ToList();

            string wayspotAnchorsJson = JsonUtility.ToJson(anchorsObjectData);

            string path = Application.persistentDataPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllText(Path.Combine(path, "anchors.json"), wayspotAnchorsJson);
        }

        public static AnchorObject[] LoadLocalPayloads()
        {
            var anchorsObjectData = JsonUtility.FromJson<AnchorsObjectData>(_anchorJson);
            List<AnchorObject> loaded = new List<AnchorObject>();

            for (int i = 0; i < anchorsObjectData.Payloads.Count; i++)
            {
                loaded.Add(new AnchorObject(WayspotAnchorPayload.Deserialize(anchorsObjectData.Payloads[i]), anchorsObjectData.Prefabs[i], anchorsObjectData.Positions[i], anchorsObjectData.Rotations[i], anchorsObjectData.Scales[i]));
            }

            return loaded.ToArray();
        }
    }
}
