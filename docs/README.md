# Documentation

Each page here is one of four kinds ([Diátaxis](https://diataxis.fr/)), so you can tell at a glance what to expect.

| Kind | Use it to | Pages |
|---|---|---|
| Explanation | understand why it is built this way | [Architecture diagram](architecture.svg) ([editable source](architecture.drawio)), [decision records](decisions/) |
| How-to | do a specific task | [Turn the site off and on](runbooks/turn-site-off-and-on.md) |
| Reference | look something up | (planned) names, regions, parameters |
| Tutorial | learn by following along | (planned) |

## Decision records

Short notes, one per decision, in the style of arc42 section 9. Each says what was decided, why, and what it costs.

| # | Decision |
|---|---|
| [0001](decisions/0001-container-apps-over-kubernetes.md) | Azure Container Apps instead of Kubernetes |
| [0002](decisions/0002-public-github-repo.md) | A public GitHub repository, with the resume sources kept out |
| [0003](decisions/0003-oidc-instead-of-stored-credentials.md) | Sign in to Azure with OIDC, not a stored password |
| [0004](decisions/0004-build-once-promote.md) | Build the image once and promote the same image to prod |
| [0005](decisions/0005-one-bicep-template.md) | One Bicep template with a parameter file per environment |
| [0006](decisions/0006-dns-only-cloudflare.md) | Cloudflare DNS only (no proxy), with an Azure managed certificate |
| [0007](decisions/0007-regions.md) | Dev in Central US, prod in West US 3 |
| [0008](decisions/0008-documentation-standards.md) | Documentation standards: Diátaxis, C4, and a slice of arc42 (explains what they are) |

## Keeping this public-safe

No subscription, tenant or client IDs, no secret values, and no employer material belong in this repository. Secret names are fine; values never are.
