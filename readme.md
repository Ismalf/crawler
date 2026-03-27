# README

## ADR

For ADR documentation, the following [template](https://github.com/joelparkerhenderson/architecture-decision-record/tree/main/locales/en/templates/decision-record-template-by-michael-nygard) was used.

Please be sure to read ADRs to get full context of product development.

## Key Design Choice Summary

### Monolithic Architecture

A single deployable file, no added complexity. Perfect for the product scope.

### Single file executable (CLI).

Single executable file accompanied by settings, perfect for cli usage and devops pipelines integration.

### File export as storage

Product scope didn't require complex storing solutions. Requirements are met with no overhead for development and operations teams.

## Key Tradeoffs

### Scalability

For more demanding scrapping or more data retrieved from web sites file exports are not enough and would require non relational databases.

### Extensibility

New features like multiple targets and advanced scraping would require more complex features forcing a more flexible architecture oriented toward services.

### Maintainability

More modifications to the code and more added features can increase easily program complexity, making maintenance hard and error prone.

## Branching Strategy

### Trunk based development

Small team in charge of the development, reducing overhead of managing branches and releases. Every new feature can be enabled and disabled accordingly.

## Roadmap

To ensure proper product evolution and tracking, the following roadmap was built.

### 0.1.0 

Be able to download and show HTML content of the provided URL. URL must be passed as a parameter for the script.

- Status: Delivered.
- Related ADRs: [Language](adrs/language.md) & [Architecture](adrs/architecture.md)

### 0.2.0 

Be able to parse the HTML content and iterate over its structure to find relevant features.
Implement feature flags for trunk based development.

- Status: Delivered.

### 0.3.0 

Be able to extract relevant data from the website.

- Status: Delivered.

### 0.4.0 

Implement the two described filters

- Status: Delivered.

### 0.5.0 

Save data to a database to have metrics of usage and recovered data.

- Status: Delivered.
- Related ADR: [Storage](adrs/storage.md)

### 0.6.0

Implement automatic testing

Status: Delivered.

### 0.7.0

Full documentation on product and MVP release

Status: Delivered.

## Development container

A Development container configuration file was added for ease of exploration and execution for this project.

Tested on VS Code.

