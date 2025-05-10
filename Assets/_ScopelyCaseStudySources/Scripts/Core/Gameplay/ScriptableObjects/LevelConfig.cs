using UnityEngine;
using UnityEngine.UI;

namespace ScopelyCaseStudy.Core.Gameplay.Config
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "ScopelyCaseStudy/LevelConfig", order = 1)]
    public class LevelConfig : ScriptableObject
    {
        public RawImage BGImage;
        public RawImage FGImage;

    }
}
