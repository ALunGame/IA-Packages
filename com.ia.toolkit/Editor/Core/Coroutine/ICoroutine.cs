using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAToolkit.Core
{
    public interface IYield
    {
        bool Result(ICoroutine coroutine);
    }

    public interface ICoroutine
    {
        bool IsRunning { get; }
        IYield Current { get; }

        bool MoveNext();
        void Stop();
    }
}
