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

        public List<string> Compute(string input)
        {
            var output = new List<string>();
            var s = new List<EarleyParserSet> { new EarleyParserSet(0) };
            output.Add($"S(0): {input.Insert(0, ".")}");

            s.First().AddState($"{Grammar.S}'", $".{Grammar.S}", 0);
            output.Add($"(1) {s.First().States.First()} # start rule");

            s = SPC('\0', 0, s, output);
            output.Add(string.Empty);

            var cs = s;
            for (var i = 1; i <= input.Length; i++)
            {             
                s.Add(new EarleyParserSet(i));
                output.Add($"S({i}): {input.Insert(i, ".")}");

                s = SPC(input[i - 1], i, s, output);
                output.Add(string.Empty);
            }

            var accepted = s.LastOrDefault()?.IsComplete(Grammar.S) ?? false;
            output.Add(accepted ? "Accepted!" : "Rejected!");

            return output;
        }

        public List<EarleyParserSet> SPC(char a, int i, List<EarleyParserSet> S, List<string> output)
        {
            var hasChanges = true;
            while (hasChanges)
            {
                var l = S[i].States.Count;
                S = Complete(i, Predict(i, Scan(a, i, S, output), output), output);
                hasChanges = l != S[i].States.Count;
            }

            return S;
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
                        output.Add($"({currentSet.Size}) {newState} # scan from S({i - 1})({stateIndex + 1})");
                }
            }

            return S;
        }

        public List<EarleyParserSet> Predict(int i, List<EarleyParserSet> S, List<string> output)
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
                            output.Add($"({currentSet.Size}) {newState} # predict from ({stateIndex + 1})");
                        }
                    }
                }
            }

            return S;
        }

        public List<EarleyParserSet> Complete(int i, List<EarleyParserSet> S, List<string> output)
        {
            var currentSet = S.Last();
            var hasNewCompletedStates = true;

            while (hasNewCompletedStates)
            {
                hasNewCompletedStates = false;
                var currentStates = currentSet.States.ToList();
                var completeStates = currentStates.Where(x => x.IsComplete).ToList();
                foreach (var completeState in completeStates)
                {
                    var completeStateIndex = currentStates.IndexOf(completeState);
                    foreach (var prevSet in S.Where(x => x.Index == completeState.Origin).ToArray().Reverse())
                    {
                        var prevSetIndex = S.IndexOf(prevSet);
                        foreach (var prevState in prevSet.States.Where(x => x.DotsNext.ToString() == completeState.Left).Reverse())
                        {
                            var prevStateIndex = prevSet.States.IndexOf(prevState);
                            var newState = currentSet.AddState(prevState.Left, prevState.DotToRight, prevState.Origin);
                            if (newState != null)
                            {
                                hasNewCompletedStates = true;
                                output.Add($"({currentSet.Size}) {newState} # complete from ({completeStateIndex + 1}) and " +
                                           $"S({prevSetIndex})({prevStateIndex + 1})");
                            }
                        }
                    }
                }
            }

            return S;
        }
    }
}