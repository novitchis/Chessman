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

        public static RankOrFieldViewModel[] GetRanks(SideColor color)
        {
            var result = new [] {
                new RankOrFieldViewModel('8'),
                new RankOrFieldViewModel('7'),
                new RankOrFieldViewModel('6'),
                new RankOrFieldViewModel('5'),
                new RankOrFieldViewModel('4'),
                new RankOrFieldViewModel('3'),
                new RankOrFieldViewModel('2'),
                new RankOrFieldViewModel('1'),
            };

            if (color == SideColor.Black)
                result = result.Reverse().ToArray();

            LoadIndexes(result);

            return result;
        }

        public static RankOrFieldViewModel[] GetFields(SideColor color)
        {
            var result = new[] {
                new RankOrFieldViewModel('a'),
                new RankOrFieldViewModel('b', true),
                new RankOrFieldViewModel('c'),
                new RankOrFieldViewModel('d', true),
                new RankOrFieldViewModel('e'),
                new RankOrFieldViewModel('f', true),
                new RankOrFieldViewModel('g'),
                new RankOrFieldViewModel('h', true),
            };

            if (color == SideColor.Black)
                result = result.Reverse().ToArray();

            LoadIndexes(result);

            return result;
        }

        private static void LoadIndexes(RankOrFieldViewModel[] viewmodels)
        {
            for (int index = 0; index < viewmodels.Length; index++)
                viewmodels[index].IsOddIndex = index % 2 == 0;
        }
    }
}
