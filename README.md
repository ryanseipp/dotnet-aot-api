# Dotnet AOT API Template

[![build](https://github.com/ryanseipp/dotnet-aot-api/actions/workflows/build.yml/badge.svg)](https://github.com/ryanseipp/dotnet-aot-api/actions/workflows/build.yml)
[![CodeQL](https://github.com/ryanseipp/dotnet-aot-api/actions/workflows/codeql.yml/badge.svg)](https://github.com/ryanseipp/dotnet-aot-api/actions/workflows/codeql.yml)

A starting point for building ASP.NET Core Web APIs with minimal APIs and Native AOT compilation.

### Features

- [x] Minimal APIs
- [x] DB access via Npgsql
- [x] Observability via OpenTelemetry
- [x] Simple Todo domain as example
- [x] Source Generator for wiring up endpoint classes
- [ ] Full-fledged authentication and user management
- [ ] Useful Grafana dashboards

### TODO

- User authentication and management is still WIP. Ideally support for email confirmation and 2FA will be added.
- Complete Todo domain and request handlers.
- Build useful Grafana dashboards for determining key performance metrics of the API.

### Observability

![Preview of the configured Grafana dashboard with metrics from the running
API](/o11y/dashboard-preview.png)
