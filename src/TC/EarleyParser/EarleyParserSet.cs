using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace TC.EarleyParser
{
    public class EarleyParserSet
    {
        public EarleyParserSet(int index)
        {
            Index = index;
            States = new List<EarleyParserState>();
        }

        public int Index { get; }

        public IList<EarleyParserState> States { get; }

        public EarleyParserState AddState(string left, string right, int origin)
        {
            var newState = new EarleyParserState(left, right, origin);
            var existingState = States.FirstOrDefault(x => x.Equals(newState));

            if (existingState == null)
                States.Add(newState);
            else
                newState = null;

            return newState;
        }

        public int Size => States.Count;

        public bool IsComplete(string startState) =>
            States.Any(x => x.Left == Constants.GrammarPreStartSymbol && x.Right == $"{startState}.");
    }
}