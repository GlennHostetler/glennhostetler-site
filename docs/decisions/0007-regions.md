# 0007: Dev in Central US, prod in West US 3

Status: accepted

## Context
Prod should be near the person most likely to look at it (Denver). A subscription also allows only one Container Apps environment per region in some configurations, and my first sandbox environment had already used one region.

## Decision
Dev runs in Central US and prod in West US 3. Each environment is its own resource group.

## Consequences
- Prod is reasonably close to Colorado.
- Dev and prod are in different regions, so the image is copied between registries across regions on promotion. For one small image this is negligible.
- A region-specific capacity or quota problem affects one environment, not both.
