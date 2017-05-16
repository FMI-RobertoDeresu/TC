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

        public List<string> Parse(string input)
        {
            var output = Enumerable.Range(0, input.Length + 1).Select(x => new List<string>()).ToList();
            var s = new List<EarleyParserSet> { new EarleyParserSet(0) };
            var initialState = s[0].AddState($"{Grammar.S}'", $".{Grammar.S}", 0, 0, "stare initiala");
            output[0].Add(initialState.ToString());

            for (var i = 0; i <= input.Length; i++)
            {
                var pos = i == input.Length ? '\0' : input[i];
                output[i].Insert(0, $"S({i}): {input.Insert(i, ".")}");

                if (i == s.Count)
                    break;

                for (var si = 0; si < (s[i]?.States.Count ?? 0); si++)
                {
                    var state = s[i].States[si];
                    if (!state.IsComplete && state.DotsNext == pos)
                        Scanner(pos, state, s, output);
                    else if (!state.IsComplete && state.DotsNext != pos)
                        Predictor(pos, state, s, output);
                    else if (state.IsComplete)
                        Completer(state, s, output);
                }

                output[i].Add(string.Empty);
            }

            var accepted = s.Count == input.Length + 1 && (s.LastOrDefault()?.IsComplete(Grammar.S) ?? false);
            output.Last().Add(accepted ? "Acceptat!\n\n" : "Respins!\n\n");

            //if (accepted)
            //    BuidTrees(s, output);

            return output.SelectMany(x => x).ToList();
        }

        private void Scanner(char pos, EarleyParserState state, List<EarleyParserSet> s, List<List<string>> output)
        {
            if (!state.IsComplete && state.DotsNext == pos)
            {
                var j = state.PositionFrom;
                var i = state.PositionTo;
                if (s.Count == i + 1)
                    s.Add(new EarleyParserSet(i + 1));
                var newState = s[i + 1].AddState(state.RuleLeft, state.DotToRight, j, i + 1, "scanare");
                if (newState != null)
                    output[i + 1].Add($"{newState}");
            }
        }

        private void Predictor(char pos, EarleyParserState state, List<EarleyParserSet> s, List<List<string>> output)
        {
            if (!state.IsComplete && state.DotsNext != pos)
            {
                var i = state.PositionTo;
                foreach (var production in Grammar.NonTermProds(state.DotsNext.ToString()))
                {
                    var newState = s[i].AddState(production.Left, $".{production.Right}", i, i, "productie");
                    if (newState != null)
                        output[i].Add($"{newState}");
                }

                if (Grammar.NonTermCanBeLambda(state.DotsNext.ToString()))
                {
                    var newState = s[i].AddState(state.RuleLeft, state.DotToRight, i, i, "productie *lambda");
                    if (newState != null)
                        output[i].Add($"{newState}");
                }
            }
        }

        private void Completer(EarleyParserState state, List<EarleyParserSet> s, List<List<string>> output)
        {
            if (state.IsComplete)
            {
                var j = state.PositionFrom;
                var i = state.PositionTo;
                var incStates = s[j].States.Where(x => x.DotsNext.ToString() == state.RuleLeft && x.PositionTo == j).ToList();
                foreach (var incState in incStates)
                {
                    var backPointer = incState.DotPosition == 0 ? new[] { state } : new[] { incState, state };
                    var k = incState.PositionFrom;
                    var newState = s[i].AddState(incState.RuleLeft, incState.DotToRight, k, i, backPointer, "completare");
                    if (newState != null)
                        output[i].Add($"{newState}");
                }
            }
        }

        private void BuidTrees(List<EarleyParserSet> s, List<List<string>> output)
        {
            var treeRoots = s.Last().States.Where(x => x.IsComplete && x.RuleLeft == $"{Grammar.S}'").ToList();

            output.Add(new List<string>());
            foreach (var treeRoot in treeRoots)
            {
                var nodes = treeRoot.TreeBackPointers().Where(x => x.IsComplete)
                    .Union(new[] { treeRoot }).OrderBy(x => x.Id).ToList();
                output.Last().Add($"Tree {treeRoots.IndexOf(treeRoot)}:");
                nodes.ForEach(x => output.Last().Add($"{x}"));
                output.Last().Add(string.Empty);
            }
        }
    }
}