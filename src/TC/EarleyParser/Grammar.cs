using System.Collections.Generic;
using System.Linq;

namespace TC.EarleyParser
{
    /// <summary>
    ///     <para>A context-free grammar G is defined by the 4-tuple:</para>
    ///     <para>G = (N, Sigma, S, P) where</para>
    ///     <para>
    ///         N is a finite set; each element n in N is called a nonterminal character or a variable.
    ///         Each variable represents a different type of phrase or clause in the sentence.
    ///         Variables are also sometimes called syntactic categories.
    ///         Each variable defines a sub-language of the language defined by G.
    ///     </para>
    ///     <para>
    ///         Σ is a finite set of terminals, disjoint from V, which make up the actual content of the sentence.
    ///         The set of terminals is the alphabet of the language defined by the grammar G.
    ///     </para>
    ///     <para>
    ///         S is the start variable(or start symbol), used to represent the whole sentence(or program).
    ///         It must be an element of V.
    ///     </para>
    ///     <para>
    ///         P is a finite relation from V to (V U Sigma )*, where the asterisk represents the Kleene star operation.
    ///         The members of R are called the rules or productions of the grammar.
    ///     </para>
    /// </summary>
    public class Grammar
    {
        public Grammar(IList<string> n, IList<string> sigma, string s, IList<Production> p)
        {
            N = n;
            Sigma = sigma;
            S = s;
            P = p;
        }

        public IList<string> N { get; set; }

        public IList<string> Sigma { get; set; }

        public string S { get; set; }

        public IList<Production> P { get; set; }

        public IList<Production> NonTermProds(string n)
        {
            return P.Where(x => x.Left == n).ToList();
        }

        public bool NonTermCanBeLambda(string n)
        {
            if (P.Any(x => x.Left == n && x.Right == Constants.Lambda.ToString()))
                return true;

            return NonTermProds(n)
                .Where(x => !x.Right.Contains(n) && x.Right.All(c => N.Contains(c.ToString())))
                .Aggregate(false, (a, b) => a || b.Right
                                                .Aggregate(true, (c, d) => c && NonTermCanBeLambda(d.ToString())));
        }
    }
}