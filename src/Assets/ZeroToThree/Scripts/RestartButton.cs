using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.ZeroToThree.Scripts
{
    public class RestartButton : MonoBehaviour, IPointerClickHandler
    {
        public GameManager GameManager;

        public void OnPointerClick(PointerEventData e)
        {
            this.GameManager.Restart();
        }

    }

}
