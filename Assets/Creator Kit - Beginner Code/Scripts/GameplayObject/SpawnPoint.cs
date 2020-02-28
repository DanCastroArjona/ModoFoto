using CreatorKitCodeInternal;
using UnityEngine;

namespace CreatorKitCode 
{
    /// <summary>
    /// When the player walk through the trigger on the same GameObject as that script, the GameObject position become
    /// the new place where the player will respawn when their health reach 0.
    /// </summary>
    public class SpawnPoint : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            //no need to check as only the player can trigger object in that layer, if it create a null ref it SHOULD
            //error as that mean this other object shouldn't be in that layer
            CharacterControl chr = other.GetComponent<CharacterControl>();
            chr.SetNewRespawn(this);
        }

        public void Activated()
        {
            enabled = false;
        }

        public void Deactivated()
        {
            enabled = true;
        }
    }
}