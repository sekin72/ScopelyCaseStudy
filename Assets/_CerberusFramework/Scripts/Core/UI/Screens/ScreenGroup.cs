using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using CerberusFramework.Core.MVC;
using CerberusFramework.Core.UI.Components;
using CerberusFramework.Core.UI.Screens.Default;
using CerberusFramework.Utilities.Extensions;
using UnityEngine;
using VContainer;

namespace CerberusFramework.Core.UI.Screens
{
    public class ScreenGroup : View, IGradualLifecycle
    {
        public ICFScreen CurrentScreen { get; private set; }

        [SerializeField] private CFButton[] Buttons;
        [SerializeField] protected CFScreenView[] ScreenViews;
        [SerializeField] private int _mainScreenIndex = 0;

        protected IObjectResolver Resolver;

        protected ICFScreen[] Screens;
        private int _currentScreenIndex;

        private float _intervalX;

        protected CancellationTokenSource DisposeTokenSource;

        [Inject]
        public void Inject(IObjectResolver resolver)
        {
            Resolver = resolver;
            _intervalX = Screen.width;
        }

        public override async UniTask Initialize(CancellationToken cancellationToken)
        {
            DisposeTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            await SetScreens(cancellationToken);

            CurrentScreen = Screens[_mainScreenIndex];
            ForceMoveByButton(_mainScreenIndex);

            for (var i = 0; i < Buttons.Length; i++)
            {
                var index = i;
                Buttons[i].onClick.AddListener(() => ForceMoveByButton(index));
            }
        }

        public override void Activate()
        {
            for (var i = 0; i < ScreenViews.Length; i++)
            {
                Screens[i].ActivateController();
            }
        }

        public async UniTask ActivateGradual(CancellationToken cancellationToken)
        {
            var tasks = new List<UniTask>();
            for (var i = 0; i < ScreenViews.Length; i++)
            {
                tasks.Add(Screens[i].ActivateGradual(cancellationToken));
            }

            await UniTask.WhenAll(tasks);
        }

        public override void Deactivate()
        {
            for (var i = 0; i < ScreenViews.Length; i++)
            {
                Screens[i].DeactivateController();
            }
        }

        public async UniTask DeactivateGradual(CancellationToken cancellationToken)
        {
            var tasks = new List<UniTask>();
            for (var i = 0; i < ScreenViews.Length; i++)
            {
                tasks.Add(Screens[i].DeactivateGradual(cancellationToken));
            }

            await UniTask.WhenAll(tasks);
        }

        public override void Dispose()
        {
            Screens.DoForAll(x => x.DisposeController());
            foreach (var screenButton in Buttons)
            {
                screenButton.onClick.RemoveAllListeners();
            }

            DisposeTokenSource?.Cancel();
            DisposeTokenSource?.Dispose();
        }

        protected virtual async UniTask SetScreens(CancellationToken cancellationToken)
        {
            Screens = new ICFScreen[ScreenViews.Length];
            for (var i = 0; i < ScreenViews.Length; i++)
            {
                var screen = new DefaultScreen();
                var screenData = new DefaultScreenData();
                var screenView = ScreenViews[i] as DefaultScreenView;
                Screens[i] = screen;

                await screen.InitializeController(screenData, screenView, cancellationToken);
            }
        }

        private void ForceMoveByButton(int index)
        {
            if (index == _currentScreenIndex)
            {
                return;
            }

            ChangeScreen(index);
        }

        private void ChangeScreen(int index)
        {
            CurrentScreen.DeactivateController();

            _currentScreenIndex = index;
            CurrentScreen = Screens[index];

            var position = transform.position;
            position.x = (-index * _intervalX) + (_intervalX / 2);
            transform.position = position;

            CurrentScreen.ActivateController();
        }
    }
}