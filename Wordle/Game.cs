using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordle {
	class Game {
        int N;

        List<Word> allWords;
        List<Word> words;
		Dictionary<char, int> characterCount = new Dictionary<char, int>();

		char[] greenLetters;
		List<char>[] yellowLetters;
		List<char> greyLetters;

		public Game(List<Word> words) {
            N = words[0].word.Length;

			greenLetters = new char[N];
			yellowLetters = new List<char>[N];
			for (int i = 0; i < N; i++) {
				yellowLetters[i] = new List<char>();
			}
			greyLetters = new List<char>();

            this.words = allWords = words;
		}

        public string NextWord() {
            GetCounts();
            ScoreWords();
            if (words.Count == 0) return "";
            words.Sort();
            return words.First().word;
        }

        public List<Word> GetBestWords() {
            int x = 20;
            return words.Count > x ? words.GetRange(0, x) : words;
        }

        public List<Word> GetUsefulWords() {
            Game helperGame = new Game(allWords);
            List<char> allSame = words.First().word.ToList();
            foreach(var w in words) {
                foreach(char c in allSame.ToList()) {
                    if (!w.word.Contains(c)) allSame.Remove(c);
				}
			}
            SortedSet<char> allowed = new SortedSet<char>();
            foreach (var w in words) {
                allowed.UnionWith(w.word.ToList().Where(x => !allSame.Contains(x)));
            }

            helperGame.GetCounts();
            helperGame.ScoreWords(allowed);
            helperGame.words.Sort();
            return helperGame.GetBestWords();
        }

        void ScoreAllWords() {
            foreach (var w in allWords) {
                int score = 0;
                foreach (var c in w.word) {
                    if (!characterCount.ContainsKey(c)) continue;
                    score += characterCount[c] / w.word.Count(x => x == c);
                }
                w.score = score;
            }
        }

        private bool IsWordGreenMatch(ref string word) {
            for (int i = 0; i < N; i++) {
                if (greenLetters[i] != 0 && word[i] != greenLetters[i]) {
                    return false;
                }
                if (word[i] == greenLetters[i]) {
                    word = word.Substring(0, i) + '-' + word.Substring(i + 1);
                }
            }
            return true;
        }
        private bool IsWordYellowMatch(string word) {
            for (int i = 0; i < N; i++) {
                foreach (char letter in yellowLetters[i]) {
                    if (word[i] == letter || !word.Contains(letter)) {
                        return false;
                    }
                }
            }
            return true;
        }
        private bool IsWordGreyMatch(string word) {
            foreach (char letter in greyLetters) {
                if (word.Contains(letter)) {
                    return false;
                }
            }
            return true;
        }

        void ScoreWords(IEnumerable<char> allowed = null) {
            foreach (var w in words) {
                int score = 0;
                for(int i = 0; i < N; i++) {
                    char c = w.word[i];
                    if (c == greenLetters[i]) continue;
                    if (allowed != null && !allowed.Contains(c)) continue;
                    score += characterCount[c] / w.word.Count(x => x == c);
                }
                w.score = score;
            }
        }

        void GetCounts() {
            var d = new Dictionary<char, int>();
            foreach (var w in words) {
                for (int i = 0; i < N; i++) {
                    char c = w.word[i];
                    if (c == greenLetters[i]) continue;
                    if (!d.ContainsKey(c)) d[c] = 0;
                    d[c]++;
                }
            }
            characterCount = d;
        }

        public void SetWord(string word, char[] colors) {
            SetColorLetters(word, colors);
            FilterWords();
        }

        void SetColorLetters(string word, char[] colors) {
            for (int i = 0; i < N; i++) {
                var letter = word[i];
                if (colors[i] == 'g') {
                    if (greenLetters[i] != letter) {
                        greenLetters[i] = letter;
                        for (int j = 0; j < N; j++) {
                            yellowLetters[j].Remove(letter);
                        }
                        greyLetters.Remove(letter);
                    }
                }
                if (colors[i] == 'y') {
                    yellowLetters[i].Add(letter);
                }
                if (colors[i] == 'r') {
                    greyLetters.Add(letter);
                }
            }
        }

        void FilterWords() {
            var newWords = new List<Word>();
            foreach (var word in words) {
                var w = word.word;
                if (!IsWordGreenMatch(ref w)) continue;
                if (!IsWordYellowMatch(w)) continue;
                if (!IsWordGreyMatch(w)) continue;
                newWords.Add(word);
            }
            words = newWords;
        }

        public char[] GetColors(string word) {
            var colors = new char[N];
            for(int i = 0; i < N; i++) {
                char c = word[i];
                if (greenLetters[i] == c) colors[i] = 'g';
                else if (yellowLetters[i].Contains(c)) colors[i] = 'y';
                else colors[i] = 'r';
			}
            return colors;
		}
    }
}
