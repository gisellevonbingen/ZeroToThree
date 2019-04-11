using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        public UIScreenMain Main;
        public UIScreenGame Game;
        private UIScreen Current;

        private void Awake()
        {
            Application.targetFrameRate = 60;

            Instance = this;

            this.ShowScreen(this.Main);
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
                screen.transform.SetParent(this.transform, false);
                screen.OnOpen();
            }

            return screen;
        }

    }

}
