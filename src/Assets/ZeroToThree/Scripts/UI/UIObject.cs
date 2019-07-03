using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.ZeroToThree.Scripts.UI
{
    public class UIObject : PoolingObject
    {
        public new RectTransform transform { get { return base.transform as RectTransform; } }

        public event EventHandler<UIClickEventArgs> Click;

        public List<UIAction> Actions { get; private set; }

        public UIObject()
        {
            this.Actions = new List<UIAction>();
        }

        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            var delta = Time.deltaTime;
            var actions = this.Actions;
            var action = actions.FirstOrDefault();

            if (action != null)
            {
                if (action.Act(this, delta) == true)
                {
                    actions.Remove(action);
                }

            }

        }

        protected virtual UIObject QueryChildren(UIObject child, Vector2 worldPosition)
        {
            return child.Query(worldPosition);
        }

        protected virtual UIObject QueryChildren(Vector2 worldPosition)
        {
            var children = this.Children.ToArray();

            foreach (var child in children.Reverse())
            {
                var hover = this.QueryChildren(child, worldPosition);

                if (hover != null)
                {
                    return hover;
                }

            }

            return null;
        }

        public virtual UIObject Query(Vector2 worldPosition)
        {
            if (this.gameObject.activeSelf == false)
            {
                return null;
            }

            var queried = this.QueryChildren(worldPosition);

            if (queried != null)
            {
                return queried;
            }

            return this.QueryOwn(worldPosition);
        }

        protected virtual UIObject QueryOwn(Vector2 worldPosition)
        {
            var transform = this.transform;

            var size = transform.sizeDelta;
            var position = transform.position;
            position.x -= size.x / 2.0F;
            position.y -= size.y / 2.0F;

            if (new Rect(position, size).Contains(worldPosition) == true)
            {
                return this;
            }

            return null;
        }

        public void PerformClick(UIClickEventArgs e)
        {
            this.OnClick(e);
        }

        protected virtual void OnClick(UIClickEventArgs e)
        {
            this.Click?.Invoke(this, e);
        }

        public IEnumerable<UIObject> Children
        {
            get
            {
                foreach (var obj in this.transform.OfType<RectTransform>())
                {
                     var child = obj.GetComponent<UIObject>();

                    if (child != null)
                    {
                        yield return child;
                    }

                }

            }

        }

    }

}
