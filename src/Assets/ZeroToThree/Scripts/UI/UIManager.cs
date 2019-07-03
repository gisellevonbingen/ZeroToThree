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

        public static int MouseButtons { get; } = 2;

        [Header("Status")]
        public Vector2 MousePosition;
        public bool[] PrevMouseDowns;
        public bool[] MouseDowns;
        public UIObject HoveringObject;
        public UIObject[] DownObjects;

        public new RectTransform transform { get { return base.transform as RectTransform; } }
        public List<UIWindow> Windows { get; private set; }

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Application.wantsToQuit += this.OnApplicationWantsToQuit;
            this.QuitSure = false;
            this.QuitCoroutine = null;

            this.YesNoDialogPool = new ObjectPool<UIDialogYesNo>(this.YesNoDialogPref);
            this.YesNoDialogPool.Growed += this.OnYesNoDialogPoolGrowed;

            this.MousePosition = new Vector2();
            this.PrevMouseDowns = new bool[MouseButtons];
            this.MouseDowns = new bool[MouseButtons];
            this.HoveringObject = null;
            this.DownObjects = new UIObject[MouseButtons];

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
            dialog.ListenDetermine((sender, e) =>
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
            this.YesNoDialogPool.Free(sender as UIDialogYesNo);
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

            for (int i = 0; i < MouseButtons; i++)
            {
                var downing = this.DownObjects[i];

                if (this.PrevMouseDowns[i] != this.MouseDowns[i])
                {
                    if (this.MouseDowns[i] == true)
                    {
                        if (hovering != null)
                        {
                            this.DownObjects[i] = hovering;
                        }

                    }
                    else
                    {
                        if (downing != null && hovering == downing)
                        {
                            downing.PerformClick(new UIClickEventArgs(this.MousePosition, i));
                        }

                    }

                }

            }


        }

        private void UpdateInput()
        {
            this.MousePosition = this.Camera.ScreenToWorldPoint(Input.mousePosition);

            for (int i = 0; i < MouseButtons; i++)
            {
                this.PrevMouseDowns[i] = this.MouseDowns[i];
                this.MouseDowns[i] = Input.GetMouseButton(i);
            }

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
            window.transform.SetParent(this.transform, false);
            window.transform.SetAsLastSibling();
            window.Closed += this.OnWindowClosed;
            window.Open();

            this.Windows.Add(window);
        }

        private void OnWindowClosed(object sender, UIEventArgs e)
        {
            var window = sender as UIWindow;
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
