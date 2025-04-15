using System;
using System.Collections.Generic;
using Scripts.Utility.Math;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Source
{
    [DisallowMultipleComponent]
    public sealed class SummaryScreen : MonoBehaviour
    {
        [SerializeField] private PokemonInfoPage pokemonInfoPage;

        [SerializeField] private MovesPage movesPage;

        [SerializeField] private Animator pokemonAnimator;

        [SerializeField] private TextMeshProUGUI nickname;

        [SerializeField] private TextMeshProUGUI level;

        private SummaryScreenInput _summaryScreenInput;

        private const int PageCount = 2;

        private Player _player;

        private Pokemon[] _pokemon;

        private HashSet<string> _pages;

        private Action _onCancel;

        private int _page;

        private int _selectedPokemon;

        private int Page
        {
            get => _page;
            set
            {
                _page = Mathf.Clamp(value, 0, _pages.Count - 1);

                // set active page and disable inactive pages
                for (var i = 0; i < PageCount; ++i)
                {
                    var page = transform.GetChild(i);
                    if (_pages.Contains(page.name))
                    {
                        page.gameObject.SetActive(i == Page);
                    }
                    else
                    {
                        page.gameObject.SetActive(false);
                    }
                }
            }
        }

        private int SelectedPokemon
        {
            get => _selectedPokemon;
            set
            {
                _selectedPokemon = Mathf.Clamp(value, 0, _pokemon.Length - 1);
                var pokemon = _pokemon[SelectedPokemon];

                // initialize pages pages
                {
                    var previous = pokemonInfoPage.gameObject.activeSelf;
                    pokemonInfoPage.gameObject.SetActive(true);
                    pokemonInfoPage.Init(_player, pokemon);
                    pokemonInfoPage.gameObject.SetActive(previous);
                }
                {
                    var previous = movesPage.gameObject.activeSelf;
                    movesPage.gameObject.SetActive(true);
                    movesPage.Pokemon = pokemon;
                    movesPage.gameObject.SetActive(previous);
                }

                //pokemonAnimator.Init(pokemon.Base.FrontSpriteSheet ?? new[] { Pokedex.Instance.FakeSprite });

                nickname.text = pokemon.ToString();

                level.text = pokemon.Level.ToString();

                //AudioManager.Instance.StopSound();
                pokemon.PlayCry();
            }
        }

        private void Awake()
        {
            _summaryScreenInput = new SummaryScreenInput();
            _summaryScreenInput.Default.Select.performed += Select;
            _summaryScreenInput.Default.Cancel.performed += Cancel;
        }

        private void OnDestroy()
        {
            _summaryScreenInput.Default.Select.performed -= Select;
            _summaryScreenInput.Default.Cancel.performed -= Cancel;
        }

        private void OnEnable()
        {
            _summaryScreenInput.Default.Enable();
        }

        private void OnDisable()
        {
            _summaryScreenInput.Default.Disable();
        }

        public void Init(int initialPokemon, Player player, Pokemon[] pokemon, HashSet<string> pageIds, Action onCancel)
        {
            _player = player;
            _pokemon = new Pokemon[pokemon.Length];
            for (var i = 0; i < _pokemon.Length; ++i)
            {
                _pokemon[i] = new Pokemon(pokemon[i]);
            }

            _pages = pageIds;
            _onCancel = onCancel;

            SelectedPokemon = initialPokemon;
            Page = 0;
        }

        public void Select(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }

            var input = context.ReadValue<Vector2>().Round().ClampToAxis();

            if (input == Vector2.up)
            {
                if (SelectedPokemon - 1 >= 0)
                {
                    --SelectedPokemon;
                }
            }
            else if (input == Vector2.down)
            {
                if (SelectedPokemon + 1 < _pokemon.Length)
                {
                    ++SelectedPokemon;
                }
            }
            else if (input == Vector2.left)
            {
                --Page;
            }
            else if (input == Vector2.right)
            {
                ++Page;
            }
        }

        public void Cancel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _onCancel?.Invoke();
            }
        }
    }
}
