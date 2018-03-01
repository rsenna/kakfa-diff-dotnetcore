# kakfa-diff-dotnetcore
Simple diff processing application in .NET Core, using Kafka (with Docker Compose), LocalDB, NancyFX.

Instructions:

- Install DotNet Core!
- Install Docker
- Run `docker-compose up` and wait until the Kafka instance is up
- `dotnet run -p .\Subscriber --service` to load the subscriber service
- `dotnet run -p .\Publisher --service ` to load the publisher service

WebServices are self hosted and were created using NancyFX - no IIS (or Apache) required.

### Endpoints

Post left side Base64 string:

    POST http://localhost:12345/v1/diff/{Guid}/left

Post right side Base64 string

    POST http://localhost:12345/v1/diff/{Guid}/right

Get diff result

    GET http://localhost:12340/v1/diff/{Guid}

The diff algorithm is arbitrary (and kind of bogus). The scope of this exercise is not to implement a proper diff 
algorithm (if you are looking for a good one in C#, take a look at
[http://www.mathertel.de/Diff/](http://www.mathertel.de/Diff/)).

Purpose was just show the benefits of having a CQRS-like system over a distributed event log (provided here by Kafka, but it could also be
Azure Even Hub, for instance).

Quick explanation: POSTS are processed and sent to a Kafka Topic, by the Publisher module. Later, the Subscriber 
module will load those messages, and "compose" the diff. You can send as much "left" or "right" messages you want in 
a same ID: the diff will eventually be done using the most current versions of each side.

The result diff will be stored in a [LiteDB](https://github.com/mbdavid/LiteDB) instance.

Be aware that a Kafka topic works "kind of" a message queue, with two main differences:
1) Kafka does guarantee absolute ordering, in a way that conventional MQ systems are not able to;
2) Kafka topics are persistent and durable: it can be configured to NEVER consume any messages, without penalty (Kafka 
was actually build with this particular usecase in mind).

Both features are being used in this implementation. That means that the LiteDB instance can even be lost: if the 
Kafka topic is intact, we will be able to reconstruct the final state of the database. 

Kafka topic are also elastic: we can add other nodes if needed. That means we can scale our application horizontally 
if needed.

This is also a not-bad example of the advantages of NancyFX over other .NET REST libraries. Self-hosting, simpler code,
 ease of testing are just some of them. NancyFX Testing is also quite nice, but unfortunatelly I'm not using it here 
 (yet).

Example using curl (note: using PowerShell syntax for escaping double-quotes):

```
curl -i -X POST http://localhost:12345/v1/diff/ad393807-5e94-456d-bec7-d58775454216/left -H "Content-Type: application/json" -d "{ \`"Data\`": \`"AQIDBAUGBwgJCgsM\`" }"
curl -i -X POST http://localhost:12345/v1/diff/ad393807-5e94-456d-bec7-d58775454216/right -H "Content-Type: application/json" -d "{ \`"Data\`": \`"AQIDBwYFBAgJCgsN\`" }"
curl -i -X GET http://localhost:12340/v1/diff/ad393807-5e94-456d-bec7-d58775454216
```

You can also run
`dotnet run -p .\Test.Integration` to run the integration test fixture: it will execute the whole application,
but bypassing the NancyFX http layer.


### Future improvements

This aims to be simple template for a micro-service application in .NET Core, but it is not yet quite there. Some 
things are missing, or need to be improved:

- Not enough unit testing
- Proper integration, E2E testing
- A proper worker implementation. Right now I'm using a naive, infinite, TPL loop on
`Kafka.Diff.Subscriber.Nancy.DiffController` to listen to messages arriving in the topic.
- Better identification of command vs query modules.
- Use F# instead of C# for tests - syntax is simply better, and we have many cool libraries in C# for that, like 
[FsUnit](https://github.com/fsprojects/FsUnit) and [Foq](https://github.com/fsprojects/Foq).
- Avoid shared-module microservice anti-pattern (we have `Kafka.Diff.Common` right now).


