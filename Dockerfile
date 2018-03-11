FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

# copy solution and projects, and then restore packages
COPY ./Kafka.Diff.sln .
COPY ./Common/Common.csproj ./Common/
COPY ./Publisher/Publisher.csproj ./Publisher/
COPY ./Publisher.Test.Unit/Publisher.Test.Unit.csproj ./Publisher.Test.Unit/
COPY ./Subscriber/Subscriber.csproj ./Subscriber/
COPY ./Subscriber.Test.Unit/Subscriber.Test.Unit.csproj ./Subscriber.Test.Unit/
COPY ./Test.Integration/Test.Integration.csproj ./Test.Integration/
RUN dotnet restore

# copy everything else and make binaries
COPY . ./
RUN dotnet publish -c Release -o out

# build runtime image
FROM microsoft/aspnetcore:2.0
WORKDIR /deploy

COPY --from=build-env /app/Publisher/out ./Publisher/
COPY --from=build-env /app/Publisher.Test.Unit/out ./Publisher.Test.Unit
COPY --from=build-env /app/Subscriber/out ./Subscriber
COPY --from=build-env /app/Subscriber.Test.Unit/out ./Subscriber.Test.Unit
COPY --from=build-env /app/Test.Integration/out ./Test.Integration

RUN echo "/usr/sbin/apache2" >> /etc/bash.bashrc
RUN echo "/path/to/mongodb" >> /etc/bash.bashrc
ENTRYPOINT ["/bin/bash"]
