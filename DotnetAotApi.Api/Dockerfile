FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine3.18 as build
ARG TARGETARCH
WORKDIR /source

RUN apk add --upgrade --no-cache \
    build-base \
    clang \
    zlib-dev

COPY DotnetAotApi.sln .
COPY DotnetAotApi.Api/DotnetAotApi.Api.csproj DotnetAotApi.Api/
COPY DotnetAotApi.Api.Generators/DotnetAotApi.Api.Generators.csproj DotnetAotApi.Api.Generators/
RUN dotnet restore -r linux-musl-${TARGETARCH} --locked-mode

COPY DotnetAotApi.Api DotnetAotApi.Api/
COPY DotnetAotApi.Api.Generators DotnetAotApi.Api.Generators/
RUN dotnet publish DotnetAotApi.Api -a ${TARGETARCH} -o /app
RUN rm /app/*.dbg /app/*.pdb /app/appsettings.Development.json

FROM node:lts-alpine3.18 as build-web
ENV PNPM_HOME="/pnpm"
ENV PATH="$PNPM_HOME:$PATH"
RUN corepack enable
WORKDIR /source

COPY DotnetAotApi.Web/package.json .
COPY DotnetAotApi.Web/pnpm-lock.yaml .

RUN pnpm i --frozen-lockfile

COPY DotnetAotApi.Web .
RUN pnpm run build

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
COPY --from=build-web /source/dist/ /app/wwwroot/
USER $APP_UID
ENTRYPOINT [ "./DotnetAotApi.Api" ]
