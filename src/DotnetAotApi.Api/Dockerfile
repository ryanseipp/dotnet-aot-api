FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine3.18 as build
ARG TARGETARCH
WORKDIR /source

RUN apk add --upgrade --no-cache \
    build-base \
    clang \
    zlib-dev

COPY DotnetAotApi.Api.csproj .
RUN dotnet restore -r linux-musl-${TARGETARCH}

COPY . .
RUN dotnet publish --no-restore -a ${TARGETARCH} -o /app
# RUN rm /app/*.dbg /app/*.Development.json

FROM alpine:3.18 as run

ENV \
    APP_UID=1654 \
    ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true

RUN apk add --upgrade --no-cache \
    ca-certificates-bundle \
    libgcc \
    libssl3 \
    zlib

# Create a non-root user and group
RUN addgroup \
    --gid=$APP_UID \
    app \
    && adduser \
    --uid=$APP_UID \
    --ingroup=app \
    --disabled-password \
    app

WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENTRYPOINT [ "./DotnetAotApi.Api" ]
