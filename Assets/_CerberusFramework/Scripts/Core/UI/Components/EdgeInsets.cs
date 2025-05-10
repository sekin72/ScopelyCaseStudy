using System;

namespace CerberusFramework.Core.UI.Components
{
    [Serializable]
    public class EdgeInsets
    {
        public int Top;
        public int Right;
        public int Bottom;
        public int Left;

        public EdgeInsets(int top, int right, int bottom, int left)
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }
    }
}