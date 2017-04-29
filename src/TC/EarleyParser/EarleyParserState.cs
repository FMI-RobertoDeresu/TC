using System;
using System.Linq;

namespace TC.EarleyParser
{
    public class EarleyParserState : IEquatable<EarleyParserState>, IDisposable
    {
        public EarleyParserState(string ruleLeft, string ruleRight, int positionFrom, int positionTo,
            EarleyParserState[] backPointers, string operation)
        {
            Count++;
            Id = Count;
            RuleLeft = ruleLeft;
            RuleRight = ruleRight;
            PositionFrom = positionFrom;
            PositionTo = positionTo;
            BackPointers = backPointers;
            Operation = operation;
        }

        private static int Count { get; set; }

        public int Id { get; set; }

        public string RuleLeft { get; }

        public string RuleRight { get; }

        public int PositionFrom { get; set; }

        public int PositionTo { get; set; }

        public EarleyParserState[] BackPointers { get; set; }

        public string Operation { get; set; }

        //utils
        public int DotPosition => RuleRight.IndexOf('.');

        public bool IsComplete => RuleRight.Length == DotPosition + 1;

        public char DotsNext => !IsComplete ? RuleRight[DotPosition + 1] : '\0';

        public string DotToRight => !IsComplete ? RuleRight.Insert(DotPosition + 2, ".").Remove(DotPosition, 1) : RuleRight;

        public void Dispose()
        {
            Count--;
        }

        public bool Equals(EarleyParserState other)
        {
            return RuleLeft == other.RuleLeft && RuleRight == other.RuleRight &&
                   PositionFrom == other.PositionFrom && PositionTo == other.PositionTo;
        }

        public EarleyParserState[] TreeBackPointers()
        {
            if (BackPointers == null)
                return new EarleyParserState[0];

            return BackPointers.SelectMany(x => x.TreeBackPointers()).Union(BackPointers).ToArray();
        }

        public override string ToString()
        {
            var str = $"({Id}) {RuleLeft} -> {RuleRight} [{PositionFrom},{PositionTo}] ({Operation})";
            if (BackPointers != null && BackPointers.Length > 0)
                str += $" ({string.Join(", ", BackPointers.Select(x => x.Id))})";
            return str;
        }
    }
}