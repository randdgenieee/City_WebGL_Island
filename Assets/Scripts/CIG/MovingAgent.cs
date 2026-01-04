using System;
using System.Collections;
using UnityEngine;

namespace CIG
{
    public abstract class MovingAgent : MonoBehaviour
    {
        public delegate void RemovedEventHandler(MovingAgent gridAgent);
        public RectTransform propellor;
        [Serializable]
        public class MovingAgentSprite
        {
            [Serializable]
            public class ChildSprite
            {
                [SerializeField]
                private Sprite[] _sprites;

                public Sprite[] Sprites => _sprites;
            }

            [SerializeField]
            private Sprite[] _sprites;

            [SerializeField]
            private ChildSprite[] _childSprites;

            [SerializeField]
            private bool _flipped;

            public Sprite[] Sprites => _sprites;

            public ChildSprite[] ChildSprites => _childSprites;

            public bool IsFlipped => _flipped;

            public MovingAgentSprite(Sprite[] sprites, ChildSprite[] childSprites, bool flipped)
            {
                _sprites = sprites;
                _childSprites = childSprites;
                _flipped = flipped;
            }
        }

        private const float RemoveAnimationTime = 2f;

        [SerializeField]
        protected MovingAgentSprite[] _sprites;

        protected Timing _timing;

        private double? _lastPositionUpdateTime;

        protected Vector3 _position;

        protected float _angle;

        private IEnumerator _removeRoutine;

        public Vector3 Position
        {
            get
            {
                UpdatePositionAndAngle();
                return _position;
            }
        }

        public float Angle
        {
            get
            {
                UpdatePositionAndAngle();
                return _angle;
            }
        }

        public float Speed
        {
            get;
            protected set;
        }

        protected virtual bool CanUpdate => _removeRoutine == null;

        public event RemovedEventHandler RemovedEvent;

        protected void FireRemovedEvent()
        {
            if (this.RemovedEvent != null)
            {
                this.RemovedEvent(this);
            }
        }

        private void OnDisable()
        {
            if (_removeRoutine != null)
            {
                UnityEngine.Object.Destroy(base.gameObject);
            }
        }

        private void Update()
        {
            if (CanUpdate)
            {
                UpdateWorldPosition();
            }
        }

        public void Remove()
        {
            if (base.gameObject.activeInHierarchy)
            {
                if (_removeRoutine == null)
                {
                    StartCoroutine(_removeRoutine = RemoveRoutine());
                }
            }
            else
            {
                RemoveNow();
            }
        }

        protected void InitializeMovingAgent(Timing timing)
        {
            _timing = timing;
            UpdatePositionAndAngle();
            UpdateWorldPosition();
        }

        protected abstract void SetInitialPosition();

        protected abstract void UpdatePositionAndAngle(double deltaTime);

        protected abstract void SetSprites(MovingAgentSprite sprites);

        protected abstract void SetAlpha(float alpha);

        protected void RemoveNow()
        {
            UnityEngine.Object.Destroy(base.gameObject);
            FireRemovedEvent();
        }

        private void UpdatePositionAndAngle()
        {
            double gameTime = _timing.GameTime;
            if (_lastPositionUpdateTime.HasValue)
            {
                double num = gameTime - _lastPositionUpdateTime.Value;
                if (num > 0.0)
                {
                    UpdatePositionAndAngle(num);
                }
            }
            else
            {
                SetInitialPosition();
            }
            _lastPositionUpdateTime = gameTime;
        }

        private void UpdateWorldPosition()
        {
            base.transform.localPosition = Position;
            UpdateSprite();
        }

        private void UpdateSprite()
        {
            float num = 90f / ((float)_sprites.Length / 4f);
            int num2 = (int)(Angle / num);
            num2 %= _sprites.Length;
            MovingAgentSprite sprites = _sprites[num2];
            SetSprites(sprites);

            //switch (num2)
            //{
            //    case 0:
            //        propellor.anchoredPosition = new Vector2(-65f, -17.9f);
            //        break;
            //    case 8:
            //        propellor.anchoredPosition = new Vector2(-97.6f, 1.8f);
            //        break;

            //}
        }

        private IEnumerator RemoveRoutine()
        {
            float startTime = Time.time;
            float endTime = startTime + 2f;
            while (Time.time < endTime)
            {
                SetAlpha(1f - (Time.time - startTime) / 2f);
                yield return null;
            }
            _removeRoutine = null;
            RemoveNow();
        }
    }
}
