using System.Collections.Generic;
using System.Linq;

namespace TC.EarleyParser
{
    /// <summary>
    ///     <para>
    ///         Scanning: If a is the next symbol in the input stream, for every state in S(k) of the form(X → α • a β, j),
    ///         add(X → α a • β, j) to S(k+1).
    ///     </para>
    ///     <para>
    ///         Prediction: For every state in S(k) of the form (X → α • Y β, j) (where j is the origin position as above),
    ///         add (Y → • γ, k) to S(k) for every production in the grammar with Y on the left-hand side (Y → γ).
    ///     </para>
    ///     <para>
    ///         Completion: For every state in S(k) of the form(X → γ •, j), find states in S(j) of the form(Y → α • X β, i)
    ///         and add(Y → α X • β, i) to S(k).
    ///     </para>
    /// </summary>
    public class EarleyParser
    {
        public EarleyParser(Grammar grammar)
        {
            Grammar = grammar;
        }

        public Grammar Grammar { get; set; }

        public string[] Compute(string input)
        {
            var output = new List<string>();
            var s = new List<EarleyParserSet>();

            output.Add(input.Insert(0, "."));
            output.Add(new string('-', 100));
            s.Add(new EarleyParserSet(0));
            s.First().AddState(Constants.GrammarPreStartSymbol, Grammar.S, 0);
            s = Complete('\0', 0, Predict('\0', 0, Scan('\0', 0, s, output), output), output);
            output.Add(string.Empty);

            for (var i = 1; i <= input.Length; i++)
            {
                var a = input[i - 1];
                output.Add(input.Insert(i, "."));
                output.Add(new string('-', 100));
                s.Add(new EarleyParserSet(i));
                s = Complete(a, i, Predict(a, i, Scan(a, i, s, output), output), output);
                output.Add(string.Empty);
            }

            var accepted = s.LastOrDefault()?.IsComplete(Grammar.S) ?? false;
            output.Add(accepted ? "Accepted!" : "Rejected!");

            return output.ToArray();
        }

        public List<EarleyParserSet> Scan(char a, int i, List<EarleyParserSet> S, List<string> output)
        {
            if (i == 0)
                return S;

            var currentSet = S.Last();
            var prevSet = S.Skip(i - 1).First();
            foreach (var state in prevSet.States)
            {
                var stateIndex = prevSet.States.IndexOf(state);
                if (state.DotsNext == a)
                {
                    var newState = currentSet.AddState(state.Left, state.DotToRight, state.Origin);
                    if (newState != null)
                        output.Add($"({currentSet.Size}) {newState} # scan from S({i - 1})({stateIndex})");
                }
            }

            return S;
        }

        public List<EarleyParserSet> Predict(char a, int i, List<EarleyParserSet> S, List<string> output)
        {
            var currentSet = S.Last();
            var hasNewPredictions = true;

            while (hasNewPredictions)
            {
                hasNewPredictions = false;
                var currentStates = currentSet.States.ToList();
                foreach (var state in currentStates)
                {
                    var stateIndex = currentStates.IndexOf(state);
                    foreach (var production in Grammar.P.Where(x => x.Left == state.DotsNext.ToString()))
                    {
                        var newState = currentSet.AddState(production.Left, $".{production.Right}", i);
                        if (newState != null)
                        {
                            hasNewPredictions = true;
                            output.Add($"({currentSet.Size}) {newState} # predict from({stateIndex})");
                        }
                    }
                }
            }

            return S;
        }

        public List<EarleyParserSet> Complete(char a, int i, List<EarleyParserSet> S, List<string> output)
        {
            var currentSet = S.Last();
            var hasNewCompletedStates = true;

            while (hasNewCompletedStates)
            {
                hasNewCompletedStates = false;
                var currentStates = currentSet.States.ToList();
                foreach (var completeState in currentStates.Where(x => x.IsComplete))
                {
                    var completeStateIndex = currentStates.IndexOf(completeState);
                    foreach (var prevSet in S.Take(i - 1))
                    {
                        var prevSetIndex = S.IndexOf(prevSet);
                        foreach (var prevState in prevSet.States.Where(x => x.DotsNext.ToString() == completeState.Left))
                        {
                            var prevStateIndex = prevSet.States.IndexOf(prevState);
                            var newState = currentSet.AddState(prevState.Left, prevState.DotToRight, prevState.Origin);
                            if (newState != null)
                            {
                                hasNewCompletedStates = true;
                                output.Add($"({currentSet.Size}) {newState} # complete from({completeStateIndex}) and " +
                                           $"S({prevSetIndex})({prevStateIndex})");
                            }
                        }
                    }
                }
            }

            return S;
        }
    }
}