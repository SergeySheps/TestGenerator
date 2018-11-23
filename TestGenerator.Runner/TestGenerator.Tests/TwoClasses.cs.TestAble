using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;

namespace ClassLibrary1lab_spp
{
    public class TraceResult
    {
        public ConcurrentDictionary<int, ThreadResult> ThreadResults { get; }

        public TraceResult()
        {
            ThreadResults = new ConcurrentDictionary<int, ThreadResult>();
        }

        public void StartMethodTrace(MethodBase methodBase)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            if (ThreadResults.ContainsKey(threadId))
            {
                ThreadResults[threadId].StartMethodTrace(methodBase);
            }
            else
            {
                ThreadResult threadResult = new ThreadResult();
                ThreadResults.GetOrAdd(threadId, threadResult);
                threadResult.StartMethodTrace(methodBase);
            }
        }

        public void StopMethodTrace()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            ThreadResults[threadId]?.StopMethodTrace();
        }
    }

    public class SecondClass
    {
        public ConcurrentDictionary<int, ThreadResult> ThreadResults { get; }

        public TraceResult()
        {
            ThreadResults = new ConcurrentDictionary<int, ThreadResult>();
        }

        public void StartMethodTrace123(MethodBase methodBase)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            if (ThreadResults.ContainsKey(threadId))
            {
                ThreadResults[threadId].StartMethodTrace(methodBase);
            }
            else
            {
                ThreadResult threadResult = new ThreadResult();
                ThreadResults.GetOrAdd(threadId, threadResult);
                threadResult.StartMethodTrace(methodBase);
            }
        }

        public void StopMethodTrace123()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            ThreadResults[threadId]?.StopMethodTrace();
        }
    }
}
