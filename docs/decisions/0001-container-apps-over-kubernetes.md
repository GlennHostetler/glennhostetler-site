# 0001: Azure Container Apps instead of Kubernetes

Status: accepted

## Context
The first version of this site ran in a Kubernetes cluster with Helm and an Nginx ingress. It taught me a lot, but a small always-on cluster needs patching, monitoring and a monthly bill that is far out of proportion to a handful of static pages.

## Decision
Run the same container on Azure Container Apps, which is built on Kubernetes but hides the cluster. Scale from 0 to 2 replicas, 0.25 vCPU and 0.5 GiB each.

## Consequences
- Scale to zero means near-zero cost when nobody visits.
- No cluster to patch, and HTTPS, revisions and logging come with the service.
- The first request after idle is slower (a cold start). Acceptable for a personal site.
- Less control than Kubernetes (no custom ingress, no Helm). That is a fair trade here.
- My Kubernetes knowledge is working knowledge, and this decision does not change that.
