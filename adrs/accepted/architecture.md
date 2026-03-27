# Application Architecture

## Status

Current status is *accepted*.

## Context

Requirement to develop a scraping product to retrieve data from https://news.ycombinator.com.

## Decision

Monolithic. Organize project structure to accommodate for multiple files and stop using a single file script.

## Consequences

Full project refactor needed.

Now tests can be added to the project, plus more complex architectures may be implemented in case there are more complex use scenarios.

A simple structure prioritizing development speed was chosen for this first version of the project due to scope and complexity.