using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chessman.Extensions
{
    public static class DebounceExtension
    {
        /// <summary>
        /// When called multiple times will ignore initial calls and execute the action only for the last request
        /// </summary>
        /// <param name="func"></param>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static Action Debounce(this Action func, int milliseconds = 300)
        {
            var last = 0;
            return () =>
            {
                var current = Interlocked.Increment(ref last);
                Task.Delay(milliseconds).ContinueWith(task =>
                {
                    if (current == last) func();
                });
            };
        }
    }
}
