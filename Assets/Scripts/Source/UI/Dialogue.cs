using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Source.UI
{
    [Serializable]
    public struct Dialogue
    {
        [SerializeField, TextArea] private string[] pages;

        public string[] Pages => pages;

        public Dialogue(Dialogue dialogue)
        {
            pages = new string[dialogue.pages.Length];
            for (var i = 0; i < pages.Length; ++i)
            {
                pages[i] = dialogue.pages[i];
            }
        }

        public Dialogue(string page)
        {
            pages = new[] { page };
        }

        public Dialogue(IEnumerable<string> pages)
        {
            this.pages = pages.ToArray();
        }
    }
}
