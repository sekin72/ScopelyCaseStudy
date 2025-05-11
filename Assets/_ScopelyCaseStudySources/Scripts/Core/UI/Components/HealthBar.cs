using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ScopelyCaseStudy
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Transform _bar;

        protected Tween Tween;

        public void Dispose()
        {
            Tween?.Kill();
        }

        public virtual void ForceSetHealth(float currentHealth, float maxHealth)
        {
            Tween?.Kill();
            _bar.localScale = new Vector3(currentHealth / maxHealth, 1, 1);
        }

        public virtual void UpdateHealth(float currentHealth, float maxHealth)
        {
            Tween?.Kill();
            Tween = DOVirtual.Float(_bar.localScale.x, currentHealth / maxHealth, 0.5f, x => _bar.localScale = new Vector3(x, 1, 1));
        }
    }
}
