namespace TC.EarleyParser
{
    /// <summary>
    ///     <para>
    ///         A production rule in R is formalized mathematically as a pair (alpha ,beta) in R, where alpha in V is a
    ///         nonterminal and beta in (V U Sigma )* is a string of variables and/or terminals; rather than using ordered pair
    ///         notation, production rules are usually written using an arrow operator with α as its left hand side and β as
    ///         its right hand side: alpha -> beta.
    ///     </para>
    ///     <para>
    ///         It is allowed for β to be the empty string, and in this case it is customary to denote it by ε. The form
    ///         alpha -> ε  is called an ε-production.
    ///     </para>
    /// </summary>
    public class Production
    {
        private string _right;

        public Production(string left, string right)
        {
            Left = left;
            Right = right;
        }

        public string Left { get; set; }

        public string Right
        {
            get { return _right.Replace(Constants.Lambda.ToString(), ""); }
            private set { _right = value; }
        }

        public bool IsLambdaProduction => _right == Constants.Lambda.ToString();

        public override string ToString()
        {
            return $"{Left} -> {Right}";
        }
    }
}