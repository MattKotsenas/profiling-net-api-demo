# Profiling a .NET 8 app running in a linux container with CLI tools

This repository contains a demo application. It is a .NET 8 API with these endpoints:

- `/blocking-threads`
- `/high-cpu`
- `/memory-leak`
- `/json`

Each endpoint contains a different performance issue.

This repository is used to demonstrate how to profile an app using the .NET CLI diagnostic tools (`dotnet-dump`,
`dotnet-trace`, `dotnet-counters`, `dotnet-gcdump`) and Visual Studio.

More info about it on the blog post:
- https://www.mytechramblings.com/posts/profiling-a-net-app-with-dotnet-cli-diagnostic-tools/

## Creating the Docker image

```powershell
dotnet publish --os linux --arch x64 -t:PublishContainer -p:ContainerImageTag=8.0
```

## Profiling a Kubernetes pod

See [k8s-pod-profiling.md](./docs/k8s-pod-profiling.md).
