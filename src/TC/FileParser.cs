using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TC.EarleyParser;
using TC.PDA;

namespace TC
{
    public static class FileParser
    {
        public static PDA.PDA ParsePDAConfigFile(string file)
        {
            var lines = File.ReadAllLines(file);

            var q = lines[0].ToList().Where(x => !char.IsWhiteSpace(x)).ToArray();
            var sigma = lines[1].ToList().Where(x => !char.IsWhiteSpace(x)).ToArray();
            var gamma = lines[2].ToList().Where(x => !char.IsWhiteSpace(x)).ToArray();
            var q0 = char.Parse(lines[3].Trim());
            var z = char.Parse(lines[4].Trim());
            var f = lines[5].ToList().Where(x => !char.IsWhiteSpace(x)).ToArray();

            var delta = new List<Transition>();
            foreach (var line in lines.Skip(6))
            {
                var items = line.Split(' ').Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim()).ToArray();
                var stateSymbol = char.Parse(items[0]);
                var inputEntry = char.Parse(items[1]);
                var topOfStack = char.Parse(items[2]);
                var nextState = char.Parse(items[3]);
                var pushToStack = items[4];
                var output = items[5];
                delta.Add(new Transition(stateSymbol, inputEntry, topOfStack, nextState, pushToStack, output));
            }

            return new PDA.PDA(q, sigma, gamma, delta.ToArray(), q0, z, f);
        }

        public static EarleyParser.EarleyParser ParseEarleyParserConfigFile(string file)
        {
            var lines = File.ReadAllLines(file);

            var n = lines[0].Split(' ').Select(x => x.Trim()).ToList();
            var sigma = lines[1].Split(' ').Select(x => x.Trim()).ToList();
            var s = lines[2].Trim();

            var p = new List<Production>();
            foreach (var line in lines.Skip(3))
            {
                var items = line.Split(new[] { "->" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
                var left = items[0];
                var right = items[1];
                p.Add(new Production(left, right));
            }

            var g = new Grammar(n, sigma, s, p);

            return new EarleyParser.EarleyParser(g);
        }
    }
}