# 0005: One Bicep template with a parameter file per environment

Status: accepted

## Context
Dev and prod should be the same shape, so that dev is a meaningful test of prod.

## Decision
`infra/main.bicep` defines every resource: registry, Log Analytics, managed identity and role assignment, Container Apps environment and app, and the custom domain with its managed certificate. `infra/dev.bicepparam` and `infra/prod.bicepparam` supply the differences (environment name, and the domain for prod only). The image and port come from environment variables set by the pipeline.

The custom domain needs two steps, because the certificate can only be requested after the app already has the hostname, and the bound hostname needs the certificate. The `bindCustomDomain` parameter handles this: false adds the hostname unbound, true binds it. It must stay true afterwards, or the next deploy unbinds the domain.

## Consequences
- The environments can be rebuilt or deleted from the repository.
- Re-running the pipeline puts everything back as declared. That is the recovery path for prod ingress that was turned off by hand (see the runbook).
- `what-if` output is noisy for some Azure resources, so it is a hint, not a gate.
