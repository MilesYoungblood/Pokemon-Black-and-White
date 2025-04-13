/*
using Scripts;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(CharacterAnimator))]
    public class CharacterEditor : UnityEditor.Editor
    {
        private SpriteRenderer _spriteRenderer;

        private void OnEnable()
        {
            var characterAnimator = (CharacterAnimator)target;
            if (!_spriteRenderer || _spriteRenderer.gameObject != characterAnimator.gameObject)
            {
                _spriteRenderer = characterAnimator.GetComponent<SpriteRenderer>();
            }

            CharacterAnimator.OnValidation += ChangeDirection;
        }

        private void OnDisable()
        {
            CharacterAnimator.OnValidation -= ChangeDirection;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            if (serializedObject.ApplyModifiedProperties())
            {
                ChangeDirection();
            }
        }

        private void ChangeDirection()
        {
            if (_spriteRenderer && target)
            {
                ((CharacterAnimator)target).ChangeDirectionEditor(_spriteRenderer);
            }
        }
    }
}
*/