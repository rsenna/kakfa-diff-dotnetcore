# kakfa-diff-dotnetcore
Simple diff processing application in .NET Core, using Kafka (with Docker Compose), LocalDB, NancyFX.

### Install dependencies

Since I am currently a Windows user, and since this is (by far) the most difficult OS to set this up, I will provide more detailed instructions to run this setup on that platform.

If you are a Docker veteran, you can skip most of it...

If you are on some other platform, and you are a .NET Core and/or Docker newbie, ask me and I'll be glad to help (but I've already provided some tips at the end of this document).

- Install .NET Core!<br>

  Get instructions [here](https://github.com/dotnet/core/blob/master/release-notes/download-archives/2.0.3.md).<br>
  Windows users can download it directly [here](https://download.microsoft.com/download/D/7/2/D725E47F-A4F1-4285-8935-A91AE2FCC06A/dotnet-sdk-2.0.3-win-x64.exe).

- Install Docker.<br>

  If you're on Windows, things can get a little bit difficult, but don't panic! :-)<br>
  First of all, make sure that [virtualization is enabled](https://docs.docker.com/docker-for-windows/troubleshoot/#virtualization-must-be-enabled) in your BIOS.

  Then you'll have 2 options:

  - Install **Docker for Windows**:

    If you're using Microsoft Windows 10 Professional/Enterprise, this is the preferred way.

    Download:<br>
    https://download.docker.com/win/stable/Docker%20for%20Windows%20Installer.exe

    Remember, you **must** also have Hyper V enabled to use Docker for Windows!<br>
    If you see this message:

    > Hyper-V and Containers features are not enabled.<br>
    > Do you want to enable them for Docker to be able to work properly?<br>
    > Your computer will restart automatically.

    Or

    > Containers feature is not enabled.<br>
    > Do you want to enable it for Docker to be able to work properly?<br>
    > Your computer will restart automatically.

    Then you **must** answer "OK" and allow your computer to be restarted...

  - Install **Docker Toolbox**:

    If you are using another Windows version (Windows 7, Windows 8, Windows 10 Home), then your option is to use Docker Toolbox:

    https://www.docker.com/products/docker-toolbox<br>
    https://docs.docker.com/toolbox/toolbox_install_windows/<br>

    Docker Toolbox uses VirtualBox instead of Microsoft's Hyper-V, so it should work even with Windows Home.

### Run the Application

Note: first be user you are on the application's root folder - there you'll find the `docker-compose.yml` file.

- Use docker compose to start Kafka and Zookeeper containers:

      docker-compose up

  Wait until the Kafka instance is up before proceeding...

- Start the subscriber service (in an elevated prompt) with:

      dotnet run -p .\Subscriber --service

- Start the publisher service (also in an elevated prompt) with:

      dotnet run -p .\Publisher --service

- After you have used the application, you can shutdown the docker containers with:

      docker-compose down

WebServices are self hosted and were created using [NancyFX](http://nancyfx.org/) - no IIS (or Apache) required.

### Endpoints

Post left side Base64 string:

    POST http://localhost:12345/v1/diff/{Guid}/left

Post right side Base64 string

    POST http://localhost:12345/v1/diff/{Guid}/right

Get diff result

    GET http://localhost:12340/v1/diff/{Guid}

### About the application

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

    curl -i -X POST http://localhost:12345/v1/diff/ad393807-5e94-456d-bec7-d58775454216/left -H "Content-Type: application/json" -d "{ \`"Data\`": \`"AQIDBAUGBwgJCgsM\`" }"
    curl -i -X POST http://localhost:12345/v1/diff/ad393807-5e94-456d-bec7-d58775454216/right -H "Content-Type: application/json" -d "{ \`"Data\`": \`"AQIDBwYFBAgJCgsN\`" }"
    curl -i -X GET http://localhost:12340/v1/diff/ad393807-5e94-456d-bec7-d58775454216

You can also run

    dotnet run -p .\Test.Integration

to execute the integration test fixture - it will execute the whole application, but bypassing the NancyFX http layer.

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

### Notes on other OSes

If you're are on Linux, things will be considerably easier to install and run.<br>
.NET Core can be a little bit less straightforward though, but only because you'll need to register .NET Core custom repository into your package manager (`apt`, `yum`/`dnf`, `pacman` and so on).

With OSX things are kind of similar regarding Docker setup (it will also run over a VM).<br>
You can get some instructions about how to install Docker on a Mac [here](https://docs.docker.com/docker-for-mac/install/#download-docker-for-mac).
