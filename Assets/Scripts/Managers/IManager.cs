using System;
using System.Collections;

namespace Managers
{
    public interface IManager
    {
        void Init();
        IEnumerator Dispose();
    }
}
