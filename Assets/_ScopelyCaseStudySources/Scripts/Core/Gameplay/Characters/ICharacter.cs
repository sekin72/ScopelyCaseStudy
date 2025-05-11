using System.Collections;
using System.Collections.Generic;
using CerberusFramework.Core.MVC;
using ScopelyCaseStudy.Core.Gameplay.Effects;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ScopelyCaseStudy
{
    public interface ICharacter : IController
    {
        public bool IsAlive();
        public void TakeDamage(float damage);
        public void GetModified(Effect effect);
    }
}
