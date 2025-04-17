using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Source
{
    [Serializable]
    public struct Message
    {
        [SerializeField] [TextArea] private string[] pages;

        public string[] Pages => pages;

        public Message(Message message)
        {
            pages = new string[message.pages.Length];
            for (var i = 0; i < pages.Length; ++i)
            {
                pages[i] = message.pages[i];
            }
        }

        public Message(string page)
        {
            pages = new[] { page };
        }

        public Message(IEnumerable<string> pages)
        {
            this.pages = pages.ToArray();
        }
    }
}
