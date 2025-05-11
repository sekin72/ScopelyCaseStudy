using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CerberusFramework.Core.Managers.Pool;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.Characters.Components;
using ScopelyCaseStudy.Core.Gameplay.Effects;
using UnityEngine;
using VContainer;

namespace ScopelyCaseStudy.Core.Gameplay.Weapons
{
    public class RangedWeapon : MonoBehaviour, IWeapon
    {
        private List<BulletGameobjectPair> _firedBullets;

        private PoolManager _poolManager;
        private GameSession _gameSession;
        public AttackComponent AttackComponent { get; private set; }

        private BulletData _originalBulletData;
        private BulletData _bulletData;

        [Inject]
        public void Inject(PoolManager poolManager, GameSession gameSession)
        {
            _poolManager = poolManager;
            _gameSession = gameSession;
        }

        public void Initialize(AttackComponent attackComponent, WeaponConfig weaponConfig)
        {
            AttackComponent = attackComponent;

            _firedBullets = new List<BulletGameobjectPair>();

            var rangedWeaponConfig = weaponConfig as RangedWeaponConfig;
            _originalBulletData = new BulletData(
                weaponConfig.Damage,
                weaponConfig.Range,
                rangedWeaponConfig.BulletLifetime,
                rangedWeaponConfig.BulletTravelSpeed,
                rangedWeaponConfig.AdditionalEffects,
                rangedWeaponConfig.BulletColor);
            _bulletData = _originalBulletData;
        }

        public void Dispose()
        {
            while (_firedBullets.Count > 0)
            {
                var bullet = _firedBullets[0];
                bullet.Bullet.Dispose();
            }
        }

        private void DisposeBullet(Bullet bullet)
        {
            var bulletGameObjectPair = _firedBullets.FirstOrDefault(x => x.Bullet == bullet);
            if (bulletGameObjectPair.LoadedGameObject == null)
            {
                return;
            }

            bullet.BulletDisposed -= DisposeBullet;
            _poolManager.SafeReleaseObject(PoolKeys.Bullet, bulletGameObjectPair.LoadedGameObject);
            _firedBullets.Remove(bulletGameObjectPair);
        }

        public UniTask AttackTarget(ICharacter target, CancellationToken cancellationToken)
        {
            var bullet = _poolManager.GetGameObject(PoolKeys.Bullet).GetComponent<Bullet>();

            bullet.transform.position = transform.position;

            bullet.BulletDisposed += DisposeBullet;
            bullet.Initialize(
                _gameSession,
                AttackComponent.AttachedCharacter.View as CharacterView,
                target.View.transform.position - transform.position,
                _bulletData,
                DisposeBullet);

            _firedBullets.Add(new BulletGameobjectPair
            {
                Bullet = bullet,
                LoadedGameObject = bullet.gameObject
            });

            return UniTask.CompletedTask;
        }

        public void LateTick()
        {
            for (int i = 0; i < _firedBullets.Count; i++)
            {
                var bullet = _firedBullets[i];
                bullet.Bullet.LateTick();
            }
        }

        public void GetModified(Effect effect)
        {
            switch (effect.EffectType)
            {
                default:
                    break;
            }
        }

        private struct BulletGameobjectPair
        {
            public Bullet Bullet;
            public GameObject LoadedGameObject;
        }
    }
}