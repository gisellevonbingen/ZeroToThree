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

        public event EventHandler<UITouchButtonEventArgs> TouchButtonClick;
        public event EventHandler<UITouchButtonEventArgs> TouchButtonDown;
        public event EventHandler<UITouchButtonEventArgs> TouchButtonUp;
        public event EventHandler<UITouchEventArgs> TouchHover;
        public event EventHandler<UITouchEventArgs> TouchLeave;

        public List<UIAction> Actions { get; private set; }

        private bool Awaked { get; set; }

        public UIObject()
        {
            this.Actions = new List<UIAction>();
            this.Awaked = false;
        }

        public override void OnFree()
        {
            base.OnFree();

            this.Actions.Clear();
        }

        public void Awake()
        {
            if (this.Awaked == false)
            {
                this.Awaked = true;

                foreach (var child in this.Children)
                {
                    child.Awake();
                }

                this.OnAwake();
            }

        }

        public void Start()
        {
            this.OnStart();
        }

        public void Update()
        {
            this.OnUpdate();
        }

        protected virtual void OnAwake()
        {

        }

        protected virtual void OnStart()
        {

        }

        protected virtual void OnUpdate()
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

        public void PerformTouchButtonClick(UITouchButtonEventArgs e)
        {
            this.OnTouchButtonClick(e);
        }

        protected virtual void OnTouchButtonClick(UITouchButtonEventArgs e)
        {
            this.TouchButtonClick?.Invoke(this, e);
        }

        public void PerformTouchButtonDown(UITouchButtonEventArgs e)
        {
            this.OnTouchButtonDown(e);
        }

        protected virtual void OnTouchButtonDown(UITouchButtonEventArgs e)
        {
            this.TouchButtonDown?.Invoke(this, e);
        }

        public void PerformTouchButtonUp(UITouchButtonEventArgs e)
        {
            this.OnTouchButtonUp(e);
        }

        protected virtual void OnTouchButtonUp(UITouchButtonEventArgs e)
        {
            this.TouchButtonUp?.Invoke(this, e);
        }

        public void PerformTouchHover(UITouchEventArgs e)
        {
            this.OnTouchHover(e);
        }

        protected virtual void OnTouchHover(UITouchEventArgs e)
        {
            this.TouchHover?.Invoke(this, e);
        }

        public void PerformTouchLeave(UITouchEventArgs e)
        {
            this.OnTouchLeave(e);
        }

        protected virtual void OnTouchLeave(UITouchEventArgs e)
        {
            this.TouchLeave?.Invoke(this, e);
        }

        public IEnumerable<UIObject> Children
        {
            get
            {
                var tr = this.transform;
                var childCount = tr.childCount;

                for (int i = 0; i < childCount; i++)
                {
                    var child = tr.GetChild(i).GetComponent<UIObject>();

                    if (child != null)
                    {
                        yield return child;
                    }

                }

            }

        }

    }

}
