# Storage

## Status

Current status is proposed.

## Context

Requirement to develop a scraping product to retrieve data from https://news.ycombinator.com.

## Decision

Use MongoDB to bring more scalability to the product.

## Consequences

Moving to relational databases is very complicated. But this kind of product prioritizes raw data in great amounts.

Product default exporting setting must have a refactor as well as increasing the product complexity and adding dependencies that need monitoring and probably involves more teams.