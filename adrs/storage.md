# Storage

## Status

Current status is accepted.

## Context

Requirement to develop a scraping program to retrieve data from https://news.ycombinator.com.

## Decision

Use a simple CSV export for the product. Per requirements, the program is expected to retrieve a maximum of 30 entries at a time. As such, the current state of the product could be think of as a validation stage or MVP.

## Consequences

CSV is a portable format, so integrating this output to any workflow is quite straight forward.
Using this script as part of a pipeline can result in reusable and downloadable artifacts, which is benefited from having a file with exported data.
Actual program data structures are quite simple, which doesn't justify the extra infraestructure plus runtime overhead.

On the other hand, may the product require more features or complex data structures, it would be a must to do heavy refactoring of code. Simple scripting would not make the cut and a full fledged project must be built.