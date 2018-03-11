using System;
using System.Text;
using System.Threading;
using Autofac;
using FluentAssertions;
using Kafka.Diff.Common;
using Kafka.Diff.Publisher.Autofac;
using Kafka.Diff.Publisher.Handler.Impl;
using Kafka.Diff.Publisher.Nancy;
using Kafka.Diff.Subscriber.Autofac;
using Kafka.Diff.Subscriber.Handler;
using Kafka.Diff.Subscriber.Nancy;
using Nancy;
using Nancy.Testing;
using Xunit;

namespace Kafka.Diff.Test.Integration
{
    public class PublisherSubscriberFixture
    {
        private static readonly Guid Guid1 = Guid.Parse("7f15150a-7e57-cafe-babe-0123456789ab");

        private static readonly PublisherRequest PostBody1 = new PublisherRequest
        {
            Data = GetBase64("One morning, when Gregor Samsa woke from troubled dreams, he found himself transformed in his bed into a horrible vermin. He lay on his armour-like back, and if he lifted his head a little he could see his brown belly, slightly domed and divided by arches into stiff sections. The bedding was hardly able to cover it and seemed ready to slide off any moment. His many legs, pitifully thin compared with the size of the rest of him, waved about helplessly as he looked. \"What's happened to me?\" he thought. It wasn't a dream. His room, a proper human room although a little too small, lay peacefully between its four familiar walls. A collection of textile samples lay spread out on the table - Samsa was a travelling salesman - and above it there hung a picture that he had recently cut out of an illustrated magazine and housed in a nice, gilded frame. It showed a lady fitted out with a fur hat and fur boa who sat upright, raising a heavy fur muff that covered the whole of her lower arm towards the viewer.")
        };

        private static readonly PublisherRequest PostBody2 = new PublisherRequest
        {
            Data = GetBase64("One morning, when Gregor Samsa woke from troubled dreams, he found himself transformed in his bed into a canadian rabbit. He lay on his fluffy-blue back, and if he lifted his head a little he could see his brown belly, slightly round and patched with tiny little blue circles. The bedding was hardly able to cover it and seemed ready to slide off any moment. His two legs, cute and furry but apparently capable of running quite fast, were shakin a little as he looked. \"What's happened to me?\" he thought. It wasn't a dream. His room, a proper human room although a little too small, lay peacefully between its four familiar walls. A collection of textile samples lay spread out on the table - Samsa was a travelling salesman - and above it there hung a picture that he had recently cut out of an illustrated magazine and housed in a nice, gilded frame. It showed a lady fitted out with a fur hat and fur boa who sat upright, raising a heavy fur muff that covered the whole of her lower arm towards the viewer.")
        };

        private readonly SubmitController _submitController;
        private readonly DiffController _diffController;

        private static string GetBase64(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            var base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        public PublisherSubscriberFixture()
        {
            var publisherContainerBuilder = new ContainerBuilder();
            publisherContainerBuilder.RegisterModule<PublisherAutofacModule>();
            publisherContainerBuilder.RegisterType<SubmitController>().SingleInstance();
            var publisherContainer = publisherContainerBuilder.Build();

            _submitController = publisherContainer.Resolve<SubmitController>();

            var subscriberContainerBuilder = new ContainerBuilder();
            subscriberContainerBuilder.RegisterModule<SubscriberAutofacModule>();
            subscriberContainerBuilder.RegisterType<DiffController>().SingleInstance();
            var subscriberContainer = subscriberContainerBuilder.Build();

            _diffController = subscriberContainer.Resolve<DiffController>();
            _diffController.StartWorker();
        }

        [Fact]
        public void ShouldGetDiff()
        {
            var responsePost1 = _submitController.Post(Guid1, SubmitKey.Left, PostBody1).Result;
            responsePost1.StatusCode.Should().Be(HttpStatusCode.Accepted);

            var responsePost2 = _submitController.Post(Guid1, SubmitKey.Right, PostBody2).Result;
            responsePost2.StatusCode.Should().Be(HttpStatusCode.Accepted);

            Thread.Sleep(2000);

            var responseDiff = _diffController.GetDiff(Guid1);
            responseDiff.Body.Should().BeOfType<DiffRecord>();

            var diffRecord = (DiffRecord) responseDiff.Body;
            diffRecord.IsComplete.Should().BeTrue();
        }
    }
}
