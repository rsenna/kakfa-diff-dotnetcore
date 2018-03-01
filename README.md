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

It is also a not-bad example of the advantages of NancyFX over other .NET REST libraries. Self-hosting, simpler code,
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


