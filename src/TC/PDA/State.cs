using System.Collections.Generic;
using System.Linq;

namespace TC.PDA
{
    public class State
    {
        /// <summary>
        ///     Create start state
        /// </summary>
        public State(char stateStartSymbol, char stackStartSymbol)
        {
            Symbol = stateStartSymbol;
            Stack = new Stack<char>(new[] { stackStartSymbol });
            Parent = null;
        }

        /// <summary>
        ///     Only for "non start" states
        /// </summary>
        private State(char stateSymbol, string stack, char inputEntry, string output, State parent)
        {
            Symbol = stateSymbol;
            Stack = new Stack<char>(stack.Reverse().ToList());
            InputEntry = inputEntry;
            Output = output;
            Parent = parent;
        }

        private State Parent { get; }

        private Stack<char> Stack { get; }

        public char Symbol { get; }

        public char InputEntry { get; }

        public string Output { get; }

        public char TopOfStack => Stack.Peek();

        public State NextState(Transition transition)
        {
            var currentStateStackStr = string.Join("", Stack.ToArray().Skip(1));
            var pushToStackReverseStr = string.Join("", transition.PushToStack.Where(x => x != Constants.Lambda));
            var nextStackStr = pushToStackReverseStr + currentStateStackStr;
            var next = new State(transition.NextState, nextStackStr, transition.InputEntry, transition.Output, this);
            return next;
        }

        public bool IsFinalState(IEnumerable<char> finalStates)
        {
            return finalStates.Any(x => x == Symbol);
        }

        public static IList<State> GetPath(State state)
        {
            if (state == null)
                return new List<State>();

            var parentPath = GetPath(state.Parent);
            parentPath.Add(state);

            return parentPath;
        }

        public override string ToString()
        {
            return $"{Parent?.Symbol} -> {Symbol} / {string.Join("", Stack.ToArray())}";
        }
    }
}