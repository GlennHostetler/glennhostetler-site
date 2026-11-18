# 0002: A public GitHub repository, with the resume sources kept out

Status: accepted

## Context
I wanted GitHub (not Azure DevOps Git) and a protected prod deployment that waits for my approval. On a private repository, required reviewers on environments need a paid Enterprise plan. On a public repository they are free. I also did not want the old repository's history or my resume's working files public.

## Decision
- Start a new, public repository for the site, instead of reusing the old private one.
- Keep the old repository and the resume sources private. Only the finished resume PDF and docx are copied into the site.
- Store the three Azure IDs as secrets (not variables) so GitHub masks them in public logs.

## Consequences
- Approval gates on `prod` work without paying for them.
- Anyone can read the pipeline and the Bicep, which is a feature: it is the portfolio.
- I have to keep secrets, IDs and employer material out of every commit. The repository is the last line of defense, so I review diffs before merging.
- The commit history is short, which is honest for a small site.
