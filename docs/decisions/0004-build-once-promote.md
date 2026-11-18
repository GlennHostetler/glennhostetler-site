# 0004: Build the image once and promote the same image to prod

Status: accepted

## Context
Rebuilding for prod can produce a different image than the one that was tested in dev (new base-image patch, different dependency resolution).

## Decision
The `dev` job builds the image in the dev Azure Container Registry, tagged with the commit ID. After a reviewer approves, the `prod` job copies that exact image to the prod registry with `az acr import` and deploys it. Each environment has its own registry and managed identity.

## Consequences
- What ran in dev is byte-for-byte what runs in prod.
- Tags are the commit ID, so any running version maps back to source.
- Two registries (Basic tier) cost slightly more than one shared registry, in exchange for isolation between environments.
- The pipeline must be able to read the dev registry and write to the prod registry.
