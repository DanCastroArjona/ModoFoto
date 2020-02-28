using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CreatorKitCode 
{
    /// <summary>
    ///This allow to define a list of VFX prefab with a name.
    ///An editor script take care of generating a C# file containing an enum with the given name to index the Entries array
    ///so you can do in code something like VFXManager.SpawnFX(MyEffectName) 
    /// </summary>
    [CreateAssetMenu(fileName = "VFXDatabase", menuName = "Beginner Code/VFXDatabase", order = -800)]
    public class VFXDatabase : ScriptableObject
    {
        [System.Serializable]
        public class VFXDBEntry
        {
            public string Name;
            public GameObject Prefab;
            public int PoolSize = 6;
        }

        public VFXDBEntry[] Entries;
    }
}