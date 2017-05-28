using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class RankOrFieldViewModel
    {
        public char Value { get; set; }

        public bool IsOddIndex { get; set; }

        public RankOrFieldViewModel(char value, bool isOddIndex = false)
        {
            Value = value;
            IsOddIndex = isOddIndex;
        }

        public static RankOrFieldViewModel[] GetRanksAsWhite()
        {
            return new [] {
                new RankOrFieldViewModel('8', true),
                new RankOrFieldViewModel('7'),
                new RankOrFieldViewModel('6', true),
                new RankOrFieldViewModel('5'),
                new RankOrFieldViewModel('4', true),
                new RankOrFieldViewModel('3'),
                new RankOrFieldViewModel('2', true),
                new RankOrFieldViewModel('1'),
            };
        }

        public static RankOrFieldViewModel[] GetFieldsAsWhite()
        {
            return new[] {
                new RankOrFieldViewModel('a'),
                new RankOrFieldViewModel('b', true),
                new RankOrFieldViewModel('c'),
                new RankOrFieldViewModel('d', true),
                new RankOrFieldViewModel('e'),
                new RankOrFieldViewModel('f', true),
                new RankOrFieldViewModel('g'),
                new RankOrFieldViewModel('h', true),
            };
        }
    }
}
