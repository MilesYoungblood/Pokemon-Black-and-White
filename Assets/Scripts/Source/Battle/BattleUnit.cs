using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public sealed class BattleUnit : MonoBehaviour
    {
        [SerializeField] private PokemonHUD pokemonHUD;

        [SerializeField] private SpriteRenderer spriteRenderer;

        [SerializeField] private Animator animator;

        private readonly StatModifier _attack = new(true);

        private readonly StatModifier _defense = new(true);

        private readonly StatModifier _spAttack = new(true);

        private readonly StatModifier _spDefense = new(true);

        private readonly StatModifier _speed = new(true);

        private readonly StatModifier _accuracy = new(false);

        private readonly StatModifier _evasiveness = new(false);

        private Dictionary<VolatileStatusCondition, int> _volatileStatusConditions;

        private int _volatileStatusCounter;

        private Vector3 _originalPosition;

        private Vector3 _originalScale;

        public StatModifier this[Stat stat]
        {
            get
            {
                return stat switch
                {
                    Stat.HP => throw new ArgumentException("HP doesn't have a stat modifier.", nameof(stat)),
                    Stat.Attack => _attack,
                    Stat.Defense => _defense,
                    Stat.SpAttack => _spAttack,
                    Stat.SpDefense => _spDefense,
                    Stat.Speed => _speed,
                    Stat.Accuracy => _accuracy,
                    Stat.Evasiveness => _evasiveness,
                    _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
                };
            }
        }

        public bool IsPlayerUnit => name is "Player Unit";

        public IBattler Battler { get; private set; }

        public Move CurrentMove { get; }

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
            _attack.Init();
            _defense.Init();
            _spAttack.Init();
            _spDefense.Init();
            _speed.Init();
            _accuracy.Init();
            _evasiveness.Init();

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

            pokemonHUD.Pokemon = Pokemon;

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

        public IEnumerator TakeDamage(
            BattleSystem battleSystem,
            BattleDialogueBox battleDialogueBox,
            int damage,
            List<string> messages)
        {
            Pokemon.HP -= damage;
            PlayHitAnimation();
            yield return pokemonHUD.UpdateHP();
            foreach (var message in messages)
            {
                yield return battleDialogueBox.TypeDialogue(message);
            }

            if (!Pokemon.CanFight)
            {
                yield return battleSystem.Faint(this);
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

        private void PlayHitAnimation()
        {
            var previousColor = spriteRenderer.color;
            DOTween.Sequence()
                .Append(spriteRenderer.DOColor(Color.gray, 0.1f))
                .Append(spriteRenderer.DOColor(previousColor, 0.1f));
        }

        public void PlayFaintAnimation()
        {
            transform.DOLocalMoveY(_originalPosition.y - 150.0f, 0.5f);
            spriteRenderer.DOFade(0.0f, 0.5f).onComplete = HandleReset;

            return;

            void HandleReset()
            {
                // reposition the image to its original offset
                transform.localPosition = _originalPosition;
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
