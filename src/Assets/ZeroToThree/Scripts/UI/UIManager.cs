using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        public Camera Camera;
        public UIWindow MainWindow;
        public UIScreenMain Main;
        public UIScreenGame Game;
        private UIScreen Current;

        private bool QuitSure;
        private Coroutine QuitCoroutine;

        public UIDialogYesNo YesNoDialogPref;
        private ObjectPool<UIDialogYesNo> YesNoDialogPool;
        public UIDialogGameOver GameOverDialog;

        [Header("Status")]
        public Vector2 MousePosition;
        private bool PrevMouseDown;
        public bool MouseDown;
        public UIObject HoveringObject;
        public UIObject DownObject;

        public new RectTransform transform { get { return base.transform as RectTransform; } }
        public List<UIWindow> Windows { get; private set; }

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Application.wantsToQuit += this.OnApplicationWantsToQuit;
            this.QuitSure = false;
            this.QuitCoroutine = null;

            Instance = this;

            this.YesNoDialogPool = new ObjectPool<UIDialogYesNo>(this.YesNoDialogPref);
            this.YesNoDialogPool.Growed += this.OnYesNoDialogPoolGrowed;

            this.Windows = new List<UIWindow>() { this.MainWindow };

            this.ShowScreen(this.Main);
        }

        public void QuitDialogStart()
        {
            this.QuitCoroutine = this.StartCoroutine(this.QuitDialogRoutine());

        }

        private IEnumerator QuitDialogRoutine()
        {
            var dialog = this.PopupYesNoDialog("Quit\nApplication");
            dialog.ListenDetermine((sender, e)=>
            {
                if (e.Result == YesNoResult.Yes)
                {
                    this.QuitSure = true;
                    ApplicationUtils.Quit();
                }

            });

            yield return dialog.WaitForClose();

            this.QuitCoroutine = null;
        }

        private bool OnApplicationWantsToQuit()
        {
            if (ApplicationUtils.IsPlayingInEditor() == true)
            {
                return true;
            }

            if (this.QuitSure == true)
            {
                return true;
            }

            if (this.QuitCoroutine == null)
            {
                this.QuitDialogStart();
            }

            return false;
        }

        private void OnYesNoDialogPoolGrowed(object sender, PoolGrowEventArgs<UIDialogYesNo> e)
        {
            var dialog = e.Obj;
            dialog.Closed += this.OnYesNoDialogClosed;
        }

        private void OnYesNoDialogClosed(object sender, UIEventArgs e)
        {
            this.YesNoDialogPool.Free(e.Source as UIDialogYesNo);
        }

        private void Update()
        {
            this.UpdateInput();

            this.MainWindow.transform.sizeDelta = this.transform.sizeDelta;

            this.UpdateHover();

            this.UpdateDown();
        }

        private void UpdateDown()
        {
            var hovering = this.HoveringObject;
            var downing = this.DownObject;

            if (this.PrevMouseDown != this.MouseDown)
            {

                if (this.MouseDown == true)
                {
                    if (hovering != null)
                    {
                        this.DownObject = hovering;
                    }

                }
                else
                {
                    if (downing != null && hovering == downing)
                    {
                        downing.PerformClick();
                    }

                }

            }

        }

        private void UpdateInput()
        {
            this.MousePosition = this.Camera.ScreenToWorldPoint(Input.mousePosition);
            this.PrevMouseDown = this.MouseDown;
            this.MouseDown = Input.GetMouseButton(0);
        }

        private void UpdateHover()
        {
            var windows = this.Windows;
            var topWindow = windows[windows.Count - 1];

            var nextHover = topWindow.Query(this.MousePosition);
            this.HoveringObject = nextHover;
        }

        public UIDialogYesNo PopupYesNoDialog(string message)
        {
            var dialog = this.YesNoDialogPool.Obtain();
            dialog.Message.Text.text = message;
            this.PopupWindow(dialog);

            return dialog;
        }

        public void PopupWindow(UIWindow window)
        {
            window.transform.SetParent(UIManager.Instance.transform, false);
            window.transform.SetAsLastSibling();
            window.Closed += this.OnWindowClosed;
            window.Open();

            this.Windows.Add(window);
        }

        private void OnWindowClosed(object sender, UIEventArgs e)
        {
            var window = e.Source as UIWindow;
            window.Closed -= this.OnWindowClosed;

            this.Windows.Remove(window);
        }

        public T ShowScreen<T>(T screen) where T : UIScreen
        {
            var prev = this.Current;

            if (prev != null)
            {
                prev.OnClose();
            }

            this.Current = screen;

            if (screen != null)
            {
                screen.OnOpen();
            }

            return screen;
        }

    }

}
