# Application Architecture

## Status

Current status is *deprecated*.

## Context

Requirement to develop a scraping product to retrieve data from https://news.ycombinator.com.

## Decision

Monolithic. Use .Net 10 scripting capabilities to create a single file script.

## Consequences

Single file script strips away all the boiler plate code and unnecessary complexity. Due to the product scope, a single file is all that's needed to achieve a successful result.

This approach reduces development time and speeds up ROI for the product.
Many solutions requiring complex architectures, such as hexagonal, demand more time, abstractions and boiler plate to prepare the product for future scenarios that may not occur.

May the chance come for a more complex solution and list of requirements, core logic can be organized in different layers growing into a full hexagonal architecture.

This is the simplest form that can scale, and despite needing heavy refactoring, there is no inherited complexity because the single script holds core logic that can be split and organized accordingly.