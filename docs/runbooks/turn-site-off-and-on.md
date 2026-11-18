# How to turn the site off and on

Use this to take `glennhostetler.com` offline, or bring it back. Everything below is about prod. Dev is not affected.

## Which way to turn it off

| Option | Hides the domain | Hides the Azure address | Easy to undo | Side effects |
|---|---|---|---|---|
| A. Disable ingress | yes | yes | re-run the pipeline | none known |
| B. Remove the DNS records | yes | no | re-add the records | breaks certificate renewal while removed |

Option A is the one used so far. Prefer it.

## Turn it off (option A)

```powershell
az containerapp ingress disable --name ca-glennsite-prod --resource-group rg-glennsite-prod
```

Check that it is off. This should fail to connect or time out:

```powershell
curl.exe -I --max-time 15 https://glennhostetler.com
```

## Turn it back on

The reliable way is to let the pipeline re-apply the Bicep, which declares ingress, the domain and the certificate binding.

1. In GitHub, open **Actions**, choose **deploy**, then **Run workflow** on `main`.
2. Wait for the `dev` job to go green.
3. Open the waiting `prod` job and **Review deployments**, then approve it.
4. Wait for `prod` to finish. Its smoke test must pass.

Then verify the domain is bound and serving:

```powershell
az containerapp hostname list --name ca-glennsite-prod --resource-group rg-glennsite-prod --output table
```

The hostname should show `SniEnabled`. Then:

```powershell
curl.exe -I https://glennhostetler.com
```

Expect `HTTP/2 200`.

## Things to know

- **Approving any prod deployment turns the site back on.** Every merge to `main` that touches `GlennSite/`, `infra/` or the workflow file starts the pipeline and waits at the prod approval. If the site should stay off, leave that approval pending, or reject it.
- Disabling ingress was not tested to see whether Azure keeps the custom-domain binding. That is why the supported way back is the pipeline, not a one-line command.
- The site scales to zero, so turning it off saves almost nothing. It only hides it.
- If a problem needs the site down quickly, option A is the fastest. Dev stays up at its own Azure address.

## If the domain does not come back

1. Check the hostname list above. If it is missing or `Disabled`, re-run the pipeline once more. `bindCustomDomain` must be `true` in `infra/prod.bicepparam`.
2. Check DNS. The `A` record must point at the environment's static IP, set to DNS only, and the `asuid` `TXT` record must exist (see decision 0006).
3. Check the certificate: `az containerapp env certificate list` for the prod environment should show `Succeeded`.
