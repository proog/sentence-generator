using System;
using System.Collections.Generic;
using System.Linq;

namespace SentenceGenerator {
    class Node {
        public readonly string Word;
        public readonly Dictionary<Node, int> Edges;
        public bool IsTerminator => Generator.TERMINATORS.Contains(Word);
        public bool IsPunctuation => Generator.PUNCTUATION.Contains(Word);

        public Node(string word) {
            this.Word = word;
            this.Edges = new Dictionary<Node, int>();
        }

        public void AddEdge(Node other) {
            if (!Edges.ContainsKey(other)) {
                Edges.Add(other, 0);
            }

            Edges[other] += 1;
        }

        public Node Next(Random rng) {
            var max = Edges.Values.Sum();
            var random = rng.Next(0, max);
            var count = 0;

            foreach (var entry in Edges) {
                count += entry.Value;

                if (random < count) {
                    return entry.Key;
                }
            }

            return null;
        }
    }
}
