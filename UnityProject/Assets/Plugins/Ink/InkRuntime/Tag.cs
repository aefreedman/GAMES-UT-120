﻿namespace Ink.Runtime
{
    internal class Tag : Object
    {
        public string text { get; private set; }

        public Tag(string tagText)
        {
            this.text = tagText;
        }

        public override string ToString()
        {
            return "# " + text;
        }
    }
}