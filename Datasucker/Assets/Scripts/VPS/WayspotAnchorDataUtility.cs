// Copyright 2022 Niantic, Inc. All Rights Reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Niantic.ARDK.AR.WayspotAnchors;

using UnityEngine;

namespace Niantic.ARDKExamples.WayspotAnchors
{
    public static class WayspotAnchorDataUtility
    {
        private const string DataKey = "wayspot_anchor_payloads";
        private static string AnchorJson;


        public static void InitAnchorJson(string json)
        {
            AnchorJson = json;
        }

        public static void SaveLocalPayloads(WayspotAnchorPayload[] wayspotAnchorPayloads)
        {
            var wayspotAnchorsData = new WayspotAnchorsData();
            wayspotAnchorsData.Payloads = wayspotAnchorPayloads.Select(a => a.Serialize()).ToArray();
            string wayspotAnchorsJson = JsonUtility.ToJson(wayspotAnchorsData);
            PlayerPrefs.SetString(DataKey, wayspotAnchorsJson);

            string path = Application.persistentDataPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllText(Path.Combine(path, "anchors.json"), wayspotAnchorsJson);
        }

        public static WayspotAnchorPayload[] LoadLocalPayloads()
        {
            var payloads = new List<WayspotAnchorPayload>();
            var wayspotAnchorsData = JsonUtility.FromJson<WayspotAnchorsData>(AnchorJson);
            foreach (var wayspotAnchorPayload in wayspotAnchorsData.Payloads)
            {
                var payload = WayspotAnchorPayload.Deserialize(wayspotAnchorPayload);
                payloads.Add(payload);
            }

            return payloads.ToArray();
        }

        [Serializable]
        private class WayspotAnchorsData
        {
            /// The payloads to save via JsonUtility
            public string[] Payloads = Array.Empty<string>();
        }
    }
}
