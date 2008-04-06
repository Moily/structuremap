using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StructureMap.Graph
{
    /// <summary>
    /// Models the machine-level overrides for default instances per plugin type.
    /// </summary>
    [Serializable]
    public class MachineOverride
    {
        private Dictionary<string, InstanceDefault> _defaults;
        private string _machineName;
        private Profile _profile = new Profile(string.Empty);

        public MachineOverride(string machineName, Profile profile)
            : this(machineName)
        {
            if (profile != null)
            {
                _profile = profile;
            }
        }

        public MachineOverride(string machineName) : base()
        {
            _machineName = machineName;
            _defaults = new Dictionary<string, InstanceDefault>();
        }

        public string MachineName
        {
            get { return _machineName; }
        }

        /// <summary>
        /// Finds the default key for a plugin type
        /// </summary>
        [IndexerName("DefaultKey")]
        public string this[string pluginTypeName]
        {
            get
            {
                if (_profile.HasOverride(pluginTypeName))
                {
                    return _profile[pluginTypeName];
                }
                else
                {
                    return _defaults[pluginTypeName].DefaultKey;
                }
            }
        }

        /// <summary>
        /// If the MachineOverride has a Profile, returns the profile name
        /// </summary>
        public string ProfileName
        {
            get { return _profile == null ? string.Empty : _profile.ProfileName; }
        }


        public InstanceDefault[] Defaults
        {
            get
            {
                InstanceDefault[] profileDefaults = _profile.Defaults;
                Hashtable defaultHash = new Hashtable();
                foreach (InstanceDefault instance in profileDefaults)
                {
                    defaultHash.Add(instance.PluginTypeName, instance);
                }

                foreach (InstanceDefault instance in _defaults.Values)
                {
                    if (!defaultHash.ContainsKey(instance.PluginTypeName))
                    {
                        defaultHash.Add(instance.PluginTypeName, instance);
                    }
                }

                InstanceDefault[] returnValue = new InstanceDefault[defaultHash.Count];
                defaultHash.Values.CopyTo(returnValue, 0);
                Array.Sort(returnValue);

                return returnValue;
            }
        }


        public InstanceDefault[] InnerDefaults
        {
            get
            {
                InstanceDefault[] returnValue = new InstanceDefault[_defaults.Count];
                _defaults.Values.CopyTo(returnValue, 0);
                return returnValue;
            }
        }

        /// <summary>
        /// Registers an override for the default instance of a certain plugin type.
        /// </summary>
        /// <param name="pluginTypeName"></param>
        /// <param name="defaultKey"></param>
        public void AddMachineOverride(string pluginTypeName, string defaultKey)
        {
            InstanceDefault instanceDefault = new InstanceDefault(pluginTypeName, defaultKey);
            _defaults.Add(pluginTypeName, instanceDefault);
        }

        /// <summary>
        /// Determines if the MachineOverride instance has an overriden default for the plugin type
        /// </summary>
        /// <param name="pluginTypeName"></param>
        /// <returns></returns>
        public bool HasOverride(string pluginTypeName)
        {
            return (_defaults.ContainsKey(pluginTypeName) || _profile.HasOverride(pluginTypeName));
        }
    }
}