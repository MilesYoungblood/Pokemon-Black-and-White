using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Scripts.Source
{
    [RequireComponent(typeof(Animator))]
    public class BattleUnit : MonoBehaviour
    {
        [SerializeField] private UI.PokemonHUD pokemonHud;

        private Dictionary<VolatileStatusCondition, int> _volatileStatusConditions;

        public bool IsPlayerUnit => name == "Player Unit";

        public UI.PokemonHUD PokemonHud => pokemonHud;

        public IBattler Battler { get; private set; }

        public Move CurrentMove { get; }

        private Dictionary<Stat, int> _statMods;

        private int _volatileStatusCounter;

        [SerializeField] private SpriteRenderer spriteRenderer;

        [SerializeField] private Animator animator;

        private Vector3 _originalPosition;

        private Vector3 _originalScale;

        public int this[Stat stat]
        {
            get => _statMods[stat];
            set => _statMods[stat] = value;
        }

        public Pokemon Pokemon => Battler.ActivePokemon;

        public string PokemonPrefixName => $"{Battler.Prefix}{Pokemon}";

        private void Awake()
        {
            _originalPosition = transform.localPosition;
            _originalScale = transform.localScale;

            ResetPosition();
        }

        public IEnumerator Init()
        {
            _statMods = new Dictionary<Stat, int>
            {
                [Stat.Attack] = 0,
                [Stat.Defense] = 0,
                [Stat.SpAttack] = 0,
                [Stat.SpDefense] = 0,
                [Stat.Speed] = 0,
                [Stat.Accuracy] = 0,
                [Stat.Evasiveness] = 0
            };
            _volatileStatusConditions = new Dictionary<VolatileStatusCondition, int>
            {
                [VolatileStatusCondition.Flinch] = -1,
                [VolatileStatusCondition.Confusion] = -1,
                [VolatileStatusCondition.Infatuation] = -1
            };

            //_pokemonAnimator.Init(IsPlayerUnit ? Pokemon.Base.BackSpriteSheet : Pokemon.Base.FrontSpriteSheet);

            ResetPosition();
            if (!Pokemon.Asset.Levitates)
            {
                // Calculate the current bottom Y position of the sprite in local space
                var currentBottomYLocal = transform.localPosition.y - spriteRenderer.sprite.bounds.min.y * transform.localScale.y;

                // Desired bottom Y position in local space (you can set this to your desired value)
                var desiredBottomYLocal = IsPlayerUnit ? 250.0f : 0.0f;

                // Calculate the amount to shift the sprite down in local space
                var shiftAmountLocal = currentBottomYLocal - desiredBottomYLocal;

                // Shift the sprite down by adjusting the local position
                transform.DOLocalMoveY(shiftAmountLocal - _originalPosition.y, 0.0f);
            }

            pokemonHud.Init(Pokemon);

            PlayEnterAnimation();

            yield return Pokemon.PlayCryToCompletion();
        }

        public IEnumerator Init(IBattler battler)
        {
            Battler = battler;
            yield return Init();
        }

        private void ResetPosition()
        {
            transform.localPosition = new Vector3(IsPlayerUnit ? -650.0f : 550.0f, _originalPosition.y);
        }

        private void OnDisable()
        {
            ResetPosition();
            Battler = null;
            spriteRenderer.sprite = Pokedex.Instance.FakeSprite;
        }

        public float GetStatModCalc(Stat stat)
        {
            return stat switch
            {
                Stat.Attack or Stat.Defense or Stat.SpAttack or Stat.SpDefense or Stat.Speed => _statMods[stat] < 0
                    ? 2.0f / (8 + _statMods[stat])
                    : (2.0f + _statMods[stat]) / 2.0f,
                Stat.Accuracy or Stat.Evasiveness => _statMods[stat] switch
                {
                    -6 => 33.0f / 100.0f,
                    -5 => 36.0f / 100.0f,
                    -4 => 43.0f / 100.0f,
                    -3 => 50.0f / 100.0f,
                    -2 => 60.0f / 100.0f,
                    -1 => 75.0f / 100.0f,
                    0 => 100.0f / 100.0f,
                    1 => 133.0f / 100.0f,
                    2 => 166.0f / 100.0f,
                    3 => 200.0f / 100.0f,
                    4 => 250.0f / 100.0f,
                    5 => 266.0f / 100.0f,
                    6 => 300.0f / 100.0f,
                    _ => throw new ArgumentOutOfRangeException()
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public bool ApplyStatEffect(Stat stat, int amount)
        {
            var previousMod = _statMods[stat];
            _statMods[stat] = Mathf.Clamp(_statMods[stat] + amount, -6, 6);

            return _statMods[stat] != previousMod;
        }

        public bool HasVolatileStatusCondition(VolatileStatusCondition volatileStatusCondition)
        {
            return _volatileStatusConditions[volatileStatusCondition] >= 0;
        }

        public int GetVolatileStatusCount(VolatileStatusCondition volatileStatusCondition)
        {
            return _volatileStatusConditions[volatileStatusCondition];
        }

        public void UpdateVolatileStatusCounters()
        {
            try
            {
                foreach (var condition in _volatileStatusConditions.Keys)
                {
                    _volatileStatusConditions[condition] = Mathf.Max(-1, _volatileStatusConditions[condition] - 1);
                }
            }
            catch (Exception e)
            {
                print(e);
            }
        }

        private void PlayEnterAnimation()
        {
            // restore the image to its original alpha
            spriteRenderer.DOFade(1.0f, 0.0f);
            transform.DOLocalMoveX(_originalPosition.x, 1.0f);
        }

        public void PlayExitAnimation()
        {
            transform.DOLocalMoveX(IsPlayerUnit ? -600.0f : 550.0f, 1.0f);
        }

        public void PlayAttackAnimation()
        {
            DOTween.Sequence()
                .Append(transform.DOLocalMoveX(_originalPosition.x + (IsPlayerUnit ? 50.0f : -50.0f), 0.25f))
                .Append(transform.DOLocalMoveX(_originalPosition.x, 0.25f));
        }

        public void PlayHitAnimation()
        {
            var previousColor = spriteRenderer.color;
            DOTween.Sequence()
                .Append(spriteRenderer.DOColor(Color.gray, 0.1f))
                .Append(spriteRenderer.DOColor(previousColor, 0.1f));
        }

        public void PlayFaintAnimation()
        {
            transform.DOLocalMoveY(_originalPosition.y - 150.0f, 0.5f);
            spriteRenderer.DOFade(0.0f, 0.5f).onComplete = Reset;

            return;

            void Reset()
            {
                // reposition the image to its original offset
                transform.localPosition = new Vector3(_originalPosition.x, _originalPosition.y);
            }
        }

        public IEnumerator PlayCaptureAnimation()
        {
            yield return DOTween.Sequence()
                .Append(spriteRenderer.DOFade(0.0f, 0.5f))
                .Join(transform.DOLocalMoveY(_originalPosition.y + 50.0f, 0.5f))
                .Join(transform.DOScale(new Vector3(0.3f, 0.3f, 1.0f), 0.5f))
                .WaitForCompletion();
        }

        public IEnumerator PlayEscapeAnimation()
        {
            yield return DOTween.Sequence()
                .Append(spriteRenderer.DOFade(1.0f, 0.5f))
                .Join(transform.DOLocalMoveY(_originalPosition.y, 0.5f))
                .Join(transform.DOScale(_originalScale, 0.5f))
                .WaitForCompletion();
        }
    }
}
