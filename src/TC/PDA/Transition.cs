namespace TC.PDA
{
    public class Transition
    {
        public Transition(char stateSymbol, char inputEntry, char topOfStack, char nextState, 
            string pushToStack, string output)
        {
            StateSymbol = stateSymbol;
            InputEntry = inputEntry;
            TopOfStack = topOfStack;
            NextState = nextState;
            PushToStack = pushToStack;
            Output = output;
        }

        public char StateSymbol { get; }

        public char InputEntry { get; }

        public char TopOfStack { get; }

        public char NextState { get; }

        public string PushToStack { get; }

        public string Output { get; }

        public override string ToString()
        {
            return $"{StateSymbol} {InputEntry} {TopOfStack} {NextState} {PushToStack} {Output}";
        }
    }
}