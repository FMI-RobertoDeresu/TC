using System;
using System.Linq;
using System.Text;

namespace TC.PDA
{
    public class Configuration
    {
        public Configuration(char[] q, char[] sigma, char[] gamma, Transition[] delta, char q0, char z, char[] f)
        {
            Validate(q, sigma, gamma, delta, q0, z, f);

            Q = q;
            Sigma = sigma;
            Gamma = gamma;
            Delta = delta;
            Q0 = q0;
            Z = z;
            F = f;
        }

        private char[] Q { get; }

        public char[] Sigma { get; }

        private char[] Gamma { get; }

        public Transition[] Delta { get; }

        public char Q0 { get; }

        public char Z { get; }

        public char[] F { get; }

        private void Validate(char[] q, char[] sigma, char[] gamma, Transition[] delta, char q0, char z, char[] f)
        {
            if (delta.Any(t => !q.Any(qq => qq == t.StateSymbol)))
                throw new ArgumentException($"Transition state not found in set of states!", nameof(delta));

            if (delta.Any(t => !q.Any(qq => qq == t.NextState)))
                throw new ArgumentException($"Transition next state not found in set of states!", nameof(delta));

            if (delta.Any(t => !sigma.Any(s => s == t.InputEntry) && t.InputEntry != Constants.Lambda))
                throw new ArgumentException($"Transition input entry not found in input alphabet!", nameof(delta));

            if (delta.Any(t => !gamma.Any(g => g == t.TopOfStack)))
                throw new ArgumentException($"Transition top stack symbol not found in stack alphabet!", nameof(delta));

            if (delta.Any(t => t.PushToStack.Any(p => !gamma.Any(g => p == g)) && 
                               t.PushToStack != Constants.Lambda.ToString()))
                throw new ArgumentException(
                    $"Transition push to stack has symbols that are not found in stack alphabet!", nameof(delta));

            if (!q.Any(qq => qq == q0))
                throw new ArgumentException($"Start state symbol not found in set of states!", nameof(q0));

            if (!gamma.Any(g => g == z))
                throw new ArgumentException($"Initial stack symbol not found in stack alphabet!", nameof(z));

            if (f.Any(ff => !q.Any(qq => qq == ff)))
                throw new ArgumentException(
                    $"Set of accepting states has states thare are not found in set of states!", nameof(f));
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(string.Join(" ", Q));
            stringBuilder.AppendLine(string.Join(" ", Sigma));
            stringBuilder.AppendLine(string.Join(" ", Gamma));
            stringBuilder.AppendLine(Q0.ToString());
            stringBuilder.AppendLine(Z.ToString());
            stringBuilder.AppendLine(string.Join(" ", F));
            Delta.ToList().ForEach(x => stringBuilder.AppendLine(x.ToString()));

            return stringBuilder.ToString();
        }
    }
}