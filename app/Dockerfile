# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
RUN apt-get update && apt-get install -y tree
ARG BUILD_CONFIGURATION=Debug
ARG BUILD_INSTANT=unknown
ARG BUILD_HASH=unknown
# Skip git-hook tooling during image build (see app.csproj InstallHusky target).
ENV CI=true
ENV HUSKY=0
WORKDIR /src
RUN echo "Building $BUILD_HASH at: $BUILD_INSTANT"

RUN pwd
COPY ./msb ./msb
WORKDIR /src/app
COPY app/app.csproj .
COPY Directory.Packages.props .
ARG BUILD_MODE=local
RUN --mount=type=bind,source=.nuget,target=/nuget if [ "$BUILD_MODE" = "local" ]; then dotnet nuget add source /nuget --name LocalSource && dotnet restore "app.csproj" && dotnet nuget remove source LocalSource; fi
RUN if [ "$BUILD_MODE" = "ci" ]; then dotnet restore "app.csproj"; fi
COPY ./app .
COPY ./.editorconfig .
RUN tree -a -L 9 /src
RUN dotnet build "./app.csproj" -o /build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./app.csproj" -c $BUILD_CONFIGURATION -o /publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

USER root
# Copy only the CRT file into the system certificate store and update the CA bundle.
# If the CRT is not present, update-ca-certificates will simply run without adding extras.
# Copy PEM into system certs directory as .crt so update-ca-certificates picks it up
COPY ../dc/os/.tls/root-ca.pem /usr/local/share/ca-certificates/root-ca.crt
RUN chmod 644 /usr/local/share/ca-certificates/root-ca.crt && update-ca-certificates --fresh
RUN apt-get update && apt-get install -y --no-install-recommends curl ca-certificates && rm -rf /var/lib/apt/lists/*

USER app
COPY --from=publish /publish .
RUN ls -ahlt
ENTRYPOINT ["dotnet", "No1.FaraBank.Api.dll"]