using System.Collections.Generic;
using System.Linq;

namespace TC.PDA
{
    /// <summary>
    ///     We use standard formal language notation: Gamma* denotes the set of strings over alphabet Gamma and lambda
    ///     denotes the empty string.
    ///     <para>A PDA is formally defined as a 7-tuple:</para>
    ///     <para>M=(Q, Sigma , Gamma , delta , q0, Z, F)}</para>
    ///     <para>where:</para>
    ///     <para>- Q is a finite set of states</para>
    ///     <para>- Sigma  is a finite set which is called the input alphabet</para>
    ///     <para>- Gamma  is a finite set which is called the stack alphabet</para>
    ///     <para>- delta  is a finite subset of Q x(Sigma U { lambda}) x Gamma x Q x Gamma*, the transition relation</para>
    ///     <para>- q0 in Q is the start state symbol</para>
    ///     <para>- Z in Gamma  is the initial stack symbol</para>
    ///     <para>- F subset of Q is the set of accepting states</para>
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class PDA
    {
        public PDA(char[] q, char[] sigma, char[] gamma, Transition[] delta, char q0, char z, char[] f)
        {
            Configuration = new Configuration(q, sigma, gamma, delta, q0, z, f);
        }

        private Configuration Configuration { get; }

        public IEnumerable<string> Compute(string input)
        {
            if (input.Any(entry => !Configuration.Sigma.Any(s => s == entry)))
                return new List<string> { "Some input entries not found in input alphabet" };

            var initialState = new State(Configuration.Q0, Configuration.Z);
            var activeStates = new List<State> { initialState };
            activeStates.AddRange(ProcessInputEntry(Constants.Lambda, activeStates));

            foreach (var inputEntry in input)
            {
                var generatedStates = ProcessInputEntry(inputEntry, activeStates);
                activeStates = generatedStates;

                generatedStates = ProcessInputEntry(Constants.Lambda, activeStates);
                activeStates.AddRange(generatedStates);
            }

            var acceptedPaths = activeStates
                .Where(state => state.IsFinalState(Configuration.F))
                .Select(State.GetPath)
                .ToList();

            return acceptedPaths
                .Select(path => path
                    .Select(state => state.Output)
                    .Where(output => output != Constants.Lambda.ToString())
                    .Aggregate("", (a, b) => a + b));
        }

        private List<State> ProcessInputEntry(char inputEntry, IEnumerable<State> activeStates)
        {
            var nextStates = new List<State>();

            foreach (var activeState in activeStates)
            {
                var activeStateValidTransitions = Configuration.Delta
                    .Where(x => x.InputEntry == inputEntry &&
                                x.StateSymbol == activeState.Symbol &&
                                x.TopOfStack == activeState.TopOfStack)
                    .ToList();

                foreach (var transition in activeStateValidTransitions)
                    nextStates.Add(activeState.NextState(transition));
            }

            if (nextStates.Any() && inputEntry == Constants.Lambda)
                nextStates.AddRange(ProcessInputEntry(inputEntry, nextStates));

            return nextStates;
        }

        public override string ToString()
        {
            return Configuration.ToString();
        }
    }
}