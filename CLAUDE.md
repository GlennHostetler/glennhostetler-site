# CLAUDE.md

Guidance for AI coding assistants working in this repository. This file is public, so keep it free of IDs, secrets, and anything private.

## What this repo is

Source for glennhostetler.com (a personal career and services site) plus the infrastructure and pipeline that host it on Azure. It is also a learning project, so explanations matter as much as results: say *why*, and point out differences between GitHub and Azure DevOps when they come up.

## Layout

| Path | What it is |
|---|---|
| `GlennSite/` | ASP.NET Core (.NET 10) app that only serves static files from `wwwroot/` |
| `GlennSite/wwwroot/` | Hand-written HTML, CSS, JS. No framework, no build step |
| `GlennSite/Dockerfile` | Multi-stage, non-root, port 8080. Build context is `GlennSite/` only |
| `tests/GlennSite.Tests/` | xUnit integration tests that start the site in memory |
| `infra/` | Bicep: `main.bicep` plus `dev.bicepparam` and `prod.bicepparam` |
| `.github/workflows/deploy.yml` | Dev to prod pipeline (OIDC login, build once, promote, approval before prod) |
| `.github/workflows/ci.yml` | Runs on every PR: tests, and Bicep compile and lint |
| `docs/` | Diátaxis-style docs: `decisions/` (ADRs), `runbooks/`, the architecture diagram |

Start with `docs/README.md`. Decision records explain why things are the way they are; read the relevant one before changing the design.

## Commands

```bash
dotnet test GlennSite.sln                      # integration tests
az bicep build --file infra/main.bicep --stdout > /dev/null   # compile the template
az bicep lint --file infra/main.bicep
cd GlennSite && dotnet run                      # run the site locally
```

## Working rules

- **`main` is protected.** Never push to it. Branch from the latest remote main, open a pull request, squash-merge:
  `git fetch origin` then `git switch -c <branch> origin/main`. Do not branch from a stale local branch.
- **Deploys:** the deploy workflow runs only when `GlennSite/**`, `infra/**`, or `deploy.yml` change on `main`. Docs- or test-only changes do not deploy. Dev deploys automatically; prod waits for a human approval. Approving a prod deploy re-applies the Bicep, including public ingress.
- **Public repository.** Never commit subscription, tenant, or client IDs, secret values, resume source files, or employer material. Secret *names* are fine. The three Azure IDs live in GitHub environment secrets.
- **Infrastructure changes go through Bicep,** not manual `az` edits. Keep `bindCustomDomain = true` in `prod.bicepparam` or the next deploy unbinds the domain.
- **Commit and PR text:** do not add `Co-Authored-By` or "Generated with" attribution lines. On Windows PowerShell, write multi-line commit messages and PR bodies to a UTF-8 (no BOM) file and use `git commit -F` and `gh pr create --body-file`; inline here-strings have broken before.
- **Docs:** each page is one Diátaxis kind (tutorial, how-to, reference, explanation). A significant decision gets a new numbered record in `docs/decisions/`; do not rewrite old records, supersede them.
- **Pace:** the owner prefers small steps they can run and watch, with self-contained instructions.

## Things that have caused trouble

- Windows `obj` folders break `az acr build`; the Dockerfile removes `bin` and `obj` before restore. Keep that.
- The Cloudflare `A` record must be "DNS only" (proxy off) for the Azure managed certificate to validate and renew. See ADR 0006.
- OIDC federated credentials use the ID-based subject format. Recreating the repo changes its ID and breaks sign-in. See ADR 0003.
- GitHub required status checks only work if the workflow has no `paths` filter. `ci.yml` deliberately has none.

## Where things stand

See the "Where it is now" status board in `GlennSite/wwwroot/how-this-site-works.html` and the runbook `docs/runbooks/turn-site-off-and-on.md`. Planned next: require the `test` and `bicep` checks in the ruleset, security scanning (Trivy, CodeQL), accessibility and link checks, a Playwright smoke test of the deployed site, a C4 context and container view, Application Insights with a KQL dashboard.
