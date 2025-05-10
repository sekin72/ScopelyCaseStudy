using System.Collections.Generic;
using UnityEngine;

namespace CerberusFramework.Config
{
    [CreateAssetMenu(fileName = "SoundsList", menuName = "CerberusFramework/SoundsList", order = 1)]
    public class SoundsList : ScriptableObject
    {
        public List<AudioClip> AudioClips;
    }
}