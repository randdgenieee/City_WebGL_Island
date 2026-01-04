using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CIG
{
    public class RemoteConfig
    {
        private const string OverriddenPropertiesPrefix = "property_";

        public Dictionary<string, string> OverriddenGameProperties
        {
            get;
        }

        public Dictionary<string, string> Properties
        {
            get;
        }

        public bool LastSyncSuccess
        {
            get;
            private set;
        }

        public RemoteConfig()
        {
            Properties = new Dictionary<string, string>();
            OverriddenGameProperties = new Dictionary<string, string>();
            LastSyncSuccess = false;
        }


        private bool ActivateFetchedProperties()
        {
            UnityEngine.Debug.Log("[Firebase Remote Config] Activating Fetched Config.");
            Properties.Clear();
            OverriddenGameProperties.Clear();
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("[Firebase Remote Config] Failed to activate fetched config. Error: " + ex.Message + "\r\nStacktrace:\r\n" + ex.StackTrace);
            }
            return false;
        }
    }
}
