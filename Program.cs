using System;
using System.IO;

namespace SentenceGenerator {
    public class Program {
        public static void Main(string[] args) {
            var input = File.ReadAllText(args[0]);
            var sentences = args.Length > 1 ? int.Parse(args[1]) : 10;
            var generator = new Generator();
            generator.Train(input, true);

            for (var i = 0; i < sentences; i++) {
                Console.WriteLine(generator.Generate());
            }
        }
    }
}
