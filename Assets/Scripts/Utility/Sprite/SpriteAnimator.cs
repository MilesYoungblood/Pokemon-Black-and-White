using System;
using System.Collections.Generic;
using UnityEngine;
using Sprite = UnityEngine.Sprite;

namespace Scripts.Utility
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimator : MonoBehaviour
    {
        private const int MaxFrameRate = 500;
        [SerializeField] private Sprite[] frames;

        [SerializeField] [Range(1, MaxFrameRate)]
        private int frameRate = 60;

        [SerializeField] [Min(0)] private int currentFrame;

        [SerializeField] private bool loop;

        [SerializeField] private bool reverse;

        private readonly Dictionary<int, Action> _frameEvents = new();

        private SpriteRenderer _spriteRenderer;

        private float _timer;

        public Sprite[] Frames
        {
            get => frames;
            set => frames = value;
        }

        public int FrameRate
        {
            get => frameRate;
            set => frameRate = Mathf.Clamp(value, 1, MaxFrameRate);
        }

        public int CurrentFrame
        {
            get => currentFrame;
            private set
            {
                currentFrame = value;
                OnValidate();
                _spriteRenderer.sprite = Frames[value];

                OnUpdate?.Invoke();
                if (_frameEvents.TryGetValue(CurrentFrame, out var action)) action?.Invoke();
            }
        }

        public bool Loop
        {
            get => loop;
            set => loop = value;
        }

        public bool Reverse
        {
            get => reverse;
            set => reverse = value;
        }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Update()
        {
            _timer += Time.deltaTime;
            if (_timer <= 1.0f / FrameRate) return;

            _timer -= FrameRate;

            if (Reverse)
            {
                if (CurrentFrame == 0)
                {
                    CurrentFrame = Loop ? Frames.Length - 1 : 0;
                    enabled = Loop;
                }
                else
                {
                    CurrentFrame--;
                }
            }
            else
            {
                if (CurrentFrame == Frames.Length - 1)
                {
                    CurrentFrame = Loop ? 0 : Frames.Length - 1;
                    enabled = Loop;
                }
                else
                {
                    CurrentFrame++;
                }
            }
        }

        private void OnValidate()
        {
            CurrentFrame = Mathf.Clamp(CurrentFrame, 0, Mathf.Max(Frames.Length - 1, 0));
        }

        public event Action OnUpdate;

        public void Restart()
        {
            _timer = 0.0f;
            CurrentFrame = 0;
        }

        public void AddFrameEvent(int n, Action action)
        {
            if (n >= 0 && n < Frames.Length) _frameEvents[n] = action;
        }

        public bool RemoveFrameEvent(int n)
        {
            return _frameEvents.Remove(n);
        }
    }
}