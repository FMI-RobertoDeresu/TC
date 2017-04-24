using System;

namespace TC.EarleyParser
{
    public class EarleyParserState : IEquatable<EarleyParserState>
    {
        public EarleyParserState(string left, string right, int origin)
        {
            Left = left;
            Right = right;
            Origin = origin;
        }

        public string Left { get; }

        public string Right { get; }

        public int Origin { get; }

        public int DotPosition => Right.IndexOf('.');

        public bool IsComplete => Right.Length == DotPosition + 1;

        public char DotsNext => !IsComplete ? Right[DotPosition + 1] : '\0';

        public string DotToRight => !IsComplete ? Right.Insert(DotPosition + 1, ".").Remove(DotPosition, 1) : Right;

        public bool Equals(EarleyParserState other)
        {
            return Left == other.Left && Right == other.Right;
        }

        public override string ToString()
        {
            return $"{Left} -> {Right} ({Origin})";
        }
    }
}