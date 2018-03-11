﻿using Nancy;

namespace Kafka.Diff.Publisher.Nancy
{
    /// <summary>
    /// Response returned by <see cref="SubmitController"/> actions.
    /// </summary>
    public class SubmitResponse
    {
        public BaseBody Body { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public abstract class BaseBody
        {
            public string Message { get; set; }
        }

        public class Success : BaseBody { }

        public class Error : BaseBody
        {
            public string Source { get; set; }
            public string StackTrace { get; set; }
        }
    }
}
