using System;
using UnityEditor.U2D.Sprites;
using Object = UnityEngine.Object;

namespace Scripts.Utility
{
    public static class SpriteUtility
    {
        public static void InitSpriteFactory(Object obj)
        {
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(obj);
            if (dataProvider == null)
            {
                throw new Exception($"Failed to get {nameof(ISpriteEditorDataProvider)}");
            }

            dataProvider.InitSpriteEditorDataProvider();
            dataProvider.Apply();
        }
    }
}
