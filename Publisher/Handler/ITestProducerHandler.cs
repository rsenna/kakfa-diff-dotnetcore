﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kafka.Diff.Publisher.Handler
{
    public interface ITestProducerHandler
    {
        Task Test(ICollection<string> items);
    }
}
