using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SentenceGenerator {
    class Generator {
        private Dictionary<string, Node> nodes;
        public static readonly string[] PUNCTUATION = { ".", "!", "?", ",", ":", ";" };
        public static readonly string[] TERMINATORS = { ".", "!", "?" };
        public static readonly string[] IGNORE = { "\"", "(", ")", "-", "—", "[", "]", "{", "}", "*", "_" };
        private Random rng = new Random();

        public void Train(string input, bool verbose = false) {
            if (verbose) {
                Console.WriteLine("Building model...");
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            nodes = new Dictionary<string, Node>();
            var split = Split(input);
            Node previous = null;

            foreach (var word in split) {
                if (!nodes.ContainsKey(word)) {
                    nodes.Add(word, new Node(word));
                }

                var node = nodes[word];

                if (previous != null) {
                    previous.AddEdge(node);
                }

                previous = node;
            }

            stopwatch.Stop();

            if (verbose) {
                Console.WriteLine($"Model built in {stopwatch.Elapsed.TotalSeconds} seconds");
                Console.WriteLine($"{split.Length} words, {nodes.Count} nodes");
                Console.WriteLine();
            }
        }

        public string Generate(int softLimit = 10, int hardLimit = 100) {
            var previous = nodes.Values.ElementAt(rng.Next(0, nodes.Values.Count));
            var result = new List<Node> { previous };

            for (var i = 0; i < hardLimit; i++) {
                var next = previous.Next(rng);

                if (next == null) {
                    break;
                }

                result.Add(next);

                if (i >= softLimit && next.IsTerminator) {
                    break;
                }

                previous = next;
            }

            return Join(result);
        }

        private static string[] Split(string input) {
            foreach (var p in PUNCTUATION) {
                input = input.Replace(p, " " + p + " ");
            }

            foreach (var i in IGNORE) {
                input = input.Replace(i, " ");
            }

            var result =
                from token in Regex.Split(input, @"\s+")
                where !string.IsNullOrEmpty(token) && !IGNORE.Contains(token)
                select token.Trim();

            return result.ToArray();
        }

        private static string Join(List<Node> nodes) {
            var result = new StringBuilder();
            Node previous = null;

            foreach (var node in nodes) {
                var capitalize = previous == null || previous.IsTerminator;
                var space = previous != null && !node.IsPunctuation;
                var chars = node.Word.ToCharArray();

                if (space) {
                    result.Append(" ");
                }

                if (capitalize) {
                    chars[0] = char.ToUpper(chars[0]);
                }

                result.Append(chars);
                previous = node;
            }

            return result.ToString();
        }
    }
}
