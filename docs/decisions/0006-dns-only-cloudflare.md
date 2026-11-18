# 0006: Cloudflare DNS only (no proxy), with an Azure managed certificate

Status: accepted

## Context
The domain is registered at Cloudflare. Azure issues a free managed certificate for the apex domain, validated over HTTP, which needs to reach the Container Apps environment directly.

## Decision
- An `A` record for the apex points at the environment's static IP, set to "DNS only" (proxy off).
- A `TXT` record named `asuid` holds the environment's verification code.
- Let Azure issue and renew the certificate.

## Consequences
- The free certificate works and renews, but only while those two records stay in place. Removing them to hide the site would break renewal.
- With the proxy off, Cloudflare's CDN and DDoS features are not in use. Acceptable for this site.
- If a `CAA` record is ever added, it must allow `digicert.com`.
