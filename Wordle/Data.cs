using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordle {
	class Data {
        public static Dictionary<int,List<Word>> GetWordFrequencies(int N) {
            var lines = File.ReadAllLines("WordFrequency.txt");
            var words = new Dictionary<int, List<Word>>();
            foreach (var l in lines) {
                var a = l.Split(' ');
                var s = a[0];
                if (!Valid(s)) continue;
                int c = s.Count();
                if (!words.ContainsKey(c)) words.Add(c,new List<Word>());
                words[c].Add(new Word(s.ToUpper(), int.Parse(a[1])));
            }
            return words;
        }

        public static List<Word> GetWords() {
            var lines = File.ReadAllLines("Words.txt");
            var words = new List<Word>();
            foreach (var s in lines) {
                words.Add(new Word(s.ToUpper(), 0));
            }
            return words;
        }

        private static bool Valid(string s) {
            foreach(char c in s) {
                if (c < 'a' || c > 'z') return false;
			}
            return true;
		}
    }
}
