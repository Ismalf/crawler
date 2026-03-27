# README

## ADR

For ADR documentation, the following [template](https://github.com/joelparkerhenderson/architecture-decision-record/tree/main/locales/en/templates/decision-record-template-by-michael-nygard) was used.

Please be sure to read ADRs to get full context of product development.

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

Status: Pending.