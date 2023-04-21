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
    public List<Transform> Transforms;
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
            anchorsObjectData.Transforms = prefabs.Select(a => a.transform).ToList();

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

            var anchors = anchorsObjectData.Payloads.Select(a => WayspotAnchorPayload.Deserialize(a));
            var combined = anchors.Zip(anchorsObjectData.Prefabs, (a, b) => new AnchorObject(a, b));
            var transformed = combined.Zip(anchorsObjectData.Transforms, (a,b) => new AnchorObject(a.Payload, a.Prefab, b));

            return combined.ToArray();
        }
    }
}
