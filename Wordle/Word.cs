using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordle {
	class Word : IComparable<Word> {
		public string word;
		public int score;
		public int frequency;
		public Word (string word, int frequency) {
			this.word = word;
			this.frequency = frequency;
		}

		public int CompareTo(Word other) {
			int v = score + (int)(Math.Log(frequency)*20);
			int v2 = other.score + (int)(Math.Log(other.frequency)*20);
			return v2.CompareTo(v);
		}

		public override string ToString() {
			return word;
		}

		public string ToStringLong() {
			return word + " " + score + " " + frequency;
		}
	}
}
