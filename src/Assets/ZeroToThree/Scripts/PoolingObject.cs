using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ZeroToThree.Scripts
{
    public abstract class PoolingObject : MonoBehaviour
    {
        public virtual void OnObtain()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void OnFree()
        {
            this.gameObject.SetActive(false);
        }

    }

}
