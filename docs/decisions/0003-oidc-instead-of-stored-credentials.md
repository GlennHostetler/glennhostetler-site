# 0003: Sign in to Azure with OIDC, not a stored password

Status: accepted

## Context
The pipeline needs to deploy to Azure. A service principal secret would work, but it expires, can leak, and would have to be rotated.

## Decision
Use workload identity federation. GitHub issues a short-lived OIDC token per job, and an Entra ID app registration trusts it for this repository and only for the `dev` and `prod` environments (one federated credential each). No client secret exists.

The federated credential subject uses the ID-based format (`repo:OWNER@OWNERID/REPO@REPOID:environment:ENV`), which is what GitHub issued for this repository.

The running app uses a user-assigned managed identity with the `AcrPull` role on its own registry, and nothing else.

## Consequences
- Nothing to rotate or leak.
- Only deployments from the `dev` and `prod` environments of this repository can sign in. A fork or another branch cannot.
- Renaming or recreating the repository changes its ID and breaks the subject, so the credentials must be updated if that ever happens.
