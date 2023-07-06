using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddinMD4
{
    public class CircuitBreaker
    {
        private readonly int failureThreshold;
        private readonly TimeSpan timeout;
        private readonly object lockObj = new object();

        private int failureCount;
        private DateTime lastFailureTime;
        private CircuitBreakerState state;

        public CircuitBreaker(int failureThreshold, TimeSpan timeout)
        {
            this.failureThreshold = failureThreshold;
            this.timeout = timeout;
            this.state = CircuitBreakerState.Closed;
        }

        public bool IsClosed => state == CircuitBreakerState.Closed;

        public bool IsOpen => state == CircuitBreakerState.Open;

        public void Execute(Action action)
        {
            if (state == CircuitBreakerState.Open && DateTime.Now - lastFailureTime >= timeout)
            {
                lock (lockObj)
                {
                    if (state == CircuitBreakerState.Open && DateTime.Now - lastFailureTime >= timeout)
                    {
                        state = CircuitBreakerState.HalfOpen;
                    }
                }
            }

            if (state == CircuitBreakerState.Closed || state == CircuitBreakerState.HalfOpen)
            {
                try
                {
                    action();
                    Reset();
                }
                catch
                {
                    RecordFailure();
                    throw;
                }
            }
            else if (state == CircuitBreakerState.Open)
            {
                throw new CircuitBreakerOpenException("Circuit breaker is open.");
            }
        }

        private void Reset()
        {
            lock (lockObj)
            {
                failureCount = 0;
                state = CircuitBreakerState.Closed;
            }
        }

        private void RecordFailure()
        {
            lock (lockObj)
            {
                failureCount++;
                lastFailureTime = DateTime.Now;

                if (failureCount >= failureThreshold)
                {
                    state = CircuitBreakerState.Open;
                }
            }
        }
    }

    public enum CircuitBreakerState
    {
        Closed,
        Open,
        HalfOpen
    }

    public class CircuitBreakerOpenException : Exception
    {
        public CircuitBreakerOpenException(string message) : base(message)
        {
        }
    }

}
