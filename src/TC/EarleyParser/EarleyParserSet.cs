using System.Collections.Generic;
using System.Linq;

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

        public EarleyParserState AddState(string ruleLeft, string ruleRight, int positionFrom, int positionTo,
            string operation)
        {
            return AddState(ruleLeft, ruleRight, positionFrom, positionTo, null, operation);
        }

        public EarleyParserState AddState(string ruleLeft, string ruleRight, int positionFrom, int positionTo,
            EarleyParserState[] backPointer, string operation)
        {
            var newState = new EarleyParserState(ruleLeft, ruleRight, positionFrom, positionTo, backPointer, operation);
            var existingState = States.FirstOrDefault(x => x.Equals(newState));

            if (existingState == null)
                States.Add(newState);
            else
            {
                newState.Dispose();
                newState = null;
            }

            return newState;
        }

        public bool IsComplete(string startState) =>
            States.Any(x => x.IsComplete && x.RuleLeft == $"{startState}'");
    }
}