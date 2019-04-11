using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.ZeroToThree.Scripts
{
    public class LabelButton : MonoBehaviour, IPointerClickHandler
    {
        public event EventHandler Click;

        protected virtual void OnClick(EventArgs e)
        {
            this.Click?.Invoke(this, e);
        }

        public void OnPointerClick(PointerEventData e)
        {
            this.OnClick(new EventArgs());

            Debug.Log(this.gameObject.name);
        }

    }

}
