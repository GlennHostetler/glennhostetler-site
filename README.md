# glennhostetler-site

Source for [Glenn Hostetler's](https://www.linkedin.com/in/glennhostetler) personal career and services website, and the infrastructure and pipeline that run it on Azure.

It is also a learning project: the site is hosted the way I'd want a small production workload hosted, using the practices I care about (infrastructure as code, identity instead of secrets, build once and promote, approvals before production). The site's own "How this site is built" page describes the plan and current status.

## How it works

```
merge to main -> sign in to Azure with OIDC (no stored passwords)
              -> build the image in Azure Container Registry (tagged with the commit ID)
              -> deploy to dev with Bicep -> smoke test
              -> wait for approval
              -> promote the same image to prod -> deploy with Bicep -> smoke test
```

| Path | What it is |
|------|------------|
| `GlennSite/` | ASP.NET Core (.NET 10) app that serves the static site from `wwwroot/` |
| `GlennSite/wwwroot/` | Hand-written HTML, CSS, and JS (no front-end framework) |
| `GlennSite/Dockerfile` | Multi-stage build (SDK image to build, ASP.NET runtime image to run), non-root, port 8080 |
| `infra/` | Bicep for the registry, logging, managed identity, Container Apps environment, and app; one parameter file per environment |
| `.github/workflows/deploy.yml` | The dev-to-prod pipeline |

## Run locally

```bash
cd GlennSite
dotnet run
```

## Run in a container

```bash
cd GlennSite
docker build -t glennsite:dev .
docker run -d --name site -p 8080:8080 glennsite:dev
```

Then browse to http://localhost:8080 (plain HTTP).

## Design choices

- **Azure Container Apps instead of Kubernetes:** a small site should not pay for an idle cluster, and it scales to zero.
- **OIDC federated credentials:** the pipeline holds no Azure password or key. Azure trusts tokens from this repository and these environments only.
- **Managed identity for image pulls:** the app pulls from the registry with its own identity and an `AcrPull` role.
- **Build once, promote:** prod runs the exact image that ran in dev.
- **Least privilege:** the pipeline identity is scoped to the two resource groups, not the subscription.
