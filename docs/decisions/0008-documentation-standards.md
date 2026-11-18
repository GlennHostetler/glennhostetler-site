# 0008: Documentation standards: Diátaxis, C4, and a slice of arc42

Status: accepted

## What these are

Three well-known, free frameworks that each answer a different documentation question. They are complementary, not competing.

| Framework | By | It answers | In one sentence |
|---|---|---|---|
| [Diátaxis](https://diataxis.fr/) | Daniele Procida | How do I write and organise pages so readers find what they need? | Every page is exactly one of four kinds: **tutorial** (learn by doing), **how-to guide** (accomplish a task), **reference** (look something up), or **explanation** (understand why). |
| [C4 model](https://c4model.com/) | Simon Brown | How do I draw the structure of a system? | Diagrams at four zoom levels: **Context** (the system and the people and systems around it), **Containers** (the deployable pieces), **Components** (inside one container), **Code**. Plus deployment views. |
| [arc42](https://arc42.org/) | Gernot Starke and Peter Hruschka | What topics must architecture documentation cover? | A twelve-section template, from goals and constraints through building blocks, deployment, decisions, quality and risks. |

Architecture decision records (ADRs) are the practice behind the `decisions/` folder: one short note per significant decision, popularised by Michael Nygard. arc42 includes them as section 9.

## Context

This is a small static site with one pipeline. I wanted documentation that is findable and honest without being heavier than the system. I also wanted to practise the vocabulary that architecture teams use, so I can talk about it precisely.

## Decision

- **Diátaxis: adopt the page types.** `docs/README.md` sorts every page into one of the four kinds. How-to pages (runbooks) are the first thing to add, because they are the most useful when something is wrong.
- **C4: adopt for diagrams, lightly.** The Azure-icon diagram (`architecture.drawio` and `.svg`) is a deployment view. A small Context view and Container view will be added as Mermaid text, which GitHub renders and which diffs cleanly.
- **arc42: use only the decision-records idea (section 9).** The other eleven sections are not adopted. Quality goals and risks may be added later if they have something real to say.
- **ADR format:** short, numbered files with Status, Context, Decision and Consequences. Records are not rewritten when things change; a new record supersedes the old one.

## Alternatives considered

- **Full arc42:** twelve sections would mostly be empty or repeat the code for a site this size. Documentation that is mostly placeholders is worse than none, because it looks unmaintained.
- **No framework:** one long README. Fast, but it mixes how-to steps with explanation, so neither is easy to find.
- **Wiki or documentation site:** more tooling to host and keep current than the content justifies. Markdown in the repository is reviewed in pull requests alongside the code.

## Consequences

- Pages have an obvious home, and a new page starts with the question "which of the four kinds is this?".
- Docs go through the same pull request process as code.
- Some Diátaxis kinds (reference and tutorial) are empty for now. The index says so instead of hiding it.
- The three names are jargon. This record is the explanation, and the index links to it.
