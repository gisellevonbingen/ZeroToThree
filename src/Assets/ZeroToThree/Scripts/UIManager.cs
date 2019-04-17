using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.ZeroToThree.Scripts
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        public Camera Camera;
        public UIWindow MainWindow;
        public UIScreenMain Main;
        public UIScreenGame Game;
        private UIScreen Current;

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

            Instance = this;
            this.Windows = new List<UIWindow>() { this.MainWindow };

            this.ShowScreen(this.Main);
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
