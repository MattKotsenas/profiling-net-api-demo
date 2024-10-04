# Kubernetes performance profiling

## Overview

There are several .NET tools available to diagnose performance issues of various types. See
[.NET diagnostic tools](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/tools-overview) for a full list.

The ones you're mostly likely to use are:
- counters: a lightweight tool to get summary information to determine which deeper-level investigation tool to use
- trace: gathers diagnostic events over time to diagnose CPU, HTTP, or GC issues
- dump: creates a dump of all memory allocated by the process to diagnose memory leaks and similar issues

## Process outline

For ad-hoc performance analysis, the easiest way to use the tools is to follow this basic process:
1. Identify a pod with an issue
2. Log in to the affected pod using `kubectl exec`
3. Download and install the required tool
4. Copy the trace to your local machine for debugging

To download a tool, use the format `https://aka.ms/dotnet-{tool}/{os}-{arch}` where:
- tool: dotnet tool, e.g. 'dump', 'trace', 'counters', etc.
- os: Operating system, either 'win' or 'linux'
- arch: CPU architecture, e.g. 'x64', 'arm64'

Here's an example using a Mariner 2.0 image:

```powershell
curl -L https://aka.ms/dotnet-trace/linx-x64 -o dotnet-trace && chmod +x ./dotnet-trace
```

## Demo

Here's an example end-to-end flow using [kind](https://kind.sigs.k8s.io/). This is designed to allow people to play
around with the tools locally _before_ there's an important issue to fix.

All paths are relative to the repo root (_not_ this document).

### 1. Create the cluster

Here's a sample repo that has a web app with easily identifiable performance issues and k8s resources to play with.

```powershell
git clone https://github.com/mattkotsenas/profiling-net-api-demo
cd profiling-net-api-demo
kind create cluster --config ./cluster.yaml
kubectl cluster-info --context kind-profiling-cluster
```

This will create a new 1 node k8s cluster on your local machine and set the `kubectl` context to point to it.

### 2. Create a sample app to monitor / profile

Create a docker image called `profiling-api` that contains the web app with performance issues.

```powershell
dotnet publish --os linux --arch x64 -t:PublishContainer -p:ContainerImageTag=8.0
kind load docker-image --name profiling-cluster profiling-api:8.0
```

### 3. Deploy sample app to sample cluster

App the Pod and Service definitions to your cluster.

```powershell
kubectl apply -f ./pod.yaml
kubectl apply -f ./service.yaml
```

These create and expose the sample app to your local machine. You can validate the app works by running

```powershell
curl http://localhost:30080/swagger/index.html
```

and getting a 200 OK response.

### 4. Exec into the pod and download the tools

The simplest way to gather a trace is to work in the pod directly, as it avoids all namespace and permissions issues.

```powershell
kubectl exec pod/sample-app -it -- /bin/bash
cd /tmp # Pods may have read-only file systems, so work out of /tmp
# Use 'linux-arm64' for ARM64 pods (run `uname -p` to check)
curl -L https://aka.ms/dotnet-trace/linux-x64 -o dotnet-trace
chmod +x ./dotnet-trace
```

### 5. Run the tool

In parallel:
1. Navigate to http://localhost:30080/swagger/index.html on your local machine and use the Swagger UI to run one of
   the endpoints that has a performance problem such as `/high-cpu` or `/memory-leak`. Click "Try it out" and then
   "Execute" to trigger the endpoint
2. Create a trace inside the pod by running `dotnet-trace`

```powershell
./dotnet-trace collect -p 1
```

Note the trace's file path (e.g. `/tmp/dotnet_20240907_022946.nettrace`).

### 6. View the trace on your local machine

If the pod has `tar` available, you can use `kubectl cp`. However the images built by the dotnet SDK do not. We can
workaround this by writing the file as a base64 encoded string to stdout, and then converting it back to binary and
writing it to a file.

```powershell
kubectl exec pod/sample-app -- base64 /tmp/dotnet_20240907_022946.nettrace `
    | Out-String `
    | %{ [System.Convert]::FromBase64String($_) } `
    | Set-Content .\dotnet_20240907_022946.nettrace -AsByteStream
```

Open the trace in Visual Studio (drag and drop), or use [PerfView](https://github.com/microsoft/perfview). See
[Measure app performance in Visual Studio](https://learn.microsoft.com/en-us/visualstudio/profiling/?view=vs-2022) for
docs on using Visual Studio to interpret traces.

### 7. Clean up
Delete the trace files from your machine and run `kind delete cluster --name profiling-cluster` to delete the kind k8s
cluster.
