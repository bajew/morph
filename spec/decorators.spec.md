Decorators Specification

Version: 1.0**Status: DraftPurpose: Defines the unified, semantic, tag-based decorator system used across all Morph UI elements (tables, rows, fields, cards, wizard steps, etc.). Decorators provide semantic meaning without embedding logic or visual styling into metadata.

1. Overview

Decorators in Morph are semantic tags applied to UI elements. They:

come from backend or metadata

express meaning, not visual instructions

are interpreted by the UI runtime

work across all UI element types

avoid conditions or logic in metadata

avoid UI-specific styling in metadata

Decorators unify styling, state indication, and semantic annotation under one concept: tags.

2. Core Principles

2.1 Semantic, Not Visual

Tags express meaning such as:

maintenanceDue

critical

bold

selected

invalid

The UI runtime decides how to render them.

2.2 Backend-Driven

Backend may emit tags based on domain logic:

"tags": ["maintenanceDue", "critical"]

2.3 Metadata-Driven

Metadata may define static tags:

"tags": ["bold"]

2.4 UI Runtime Interpretation

The UI runtime maps tags to actual visuals:

maintenanceDue → orange background
critical → red border
bold → bold text

2.5 Universal Applicability

Tags apply to:

table rows

table cells

list items

fields

cards

wizard steps

pages

3. Tag Structure

Tags are simple strings:

"tags": ["tagName1", "tagName2", ...]

Tags must be:

lowercase or camelCase

semantic identifiers

platform-neutral

Examples:

maintenanceDue

expired

warning

critical

bold

highlight

selected

hover

invalid

4. Tag Sources

Tags may come from:

4.1 Backend

Backend returns tags as part of data objects:

{
  "id": 42,
  "name": "Hammer",
  "tags": ["maintenanceDue", "bold"]
}

4.2 Metadata

Metadata may define static tags:

"tags": ["highlight"]

4.3 UI Runtime

UI runtime may add UI-only tags:

hover

active

selected

5. Tag Resolution

When rendering an element, Morph merges tags from:

backend

metadata

UI runtime

Example merged result:

["maintenanceDue", "bold", "selected"]

6. Tag Application Rules

Tags are applied to any element that supports them.

6.1 Tables

Rows may have tags:

"tags": ["maintenanceDue", "critical"]

Cells may inherit row tags or define their own.

6.2 Fields

Fields may have tags:

"tags": ["invalid"]

6.3 Wizard Steps

Wizard steps may have tags:

"tags": ["complete"]

6.4 Cards / Lists / Pages

Any element may define tags.

7. Tag Interpretation (UI Runtime)

The UI runtime decides how to render tags.

Example mapping:

maintenanceDue → orange background
critical → red border
bold → bold text
invalid → red underline
selected → blue highlight

This mapping is platform-specific.

8. Tag Conflicts

If multiple tags apply:

UI runtime resolves conflicts

metadata may define optional priorities later (not in v1.0)

Example:

["critical", "bold"]

UI runtime decides which visual takes precedence.

9. Tag Examples

9.1 Backend-driven domain tags

"tags": ["maintenanceDue", "expired"]

9.2 Metadata-driven formatting tags

"tags": ["bold"]

9.3 UI-driven interaction tags

"tags": ["hover", "selected"]

10. Example: Tools Maintenance Table

Backend:

{
  "id": 42,
  "name": "Hammer",
  "nextMaintenance": "2026-08-01",
  "tags": ["maintenanceDue", "bold"]
}

Metadata:

{
  "id": "toolsTable",
  "type": "table",
  "source": "tools",
  "tags": true
}

UI Runtime:

maintenanceDue → orange row
bold → bold text

11. Future Extensions

Potential future additions:

tag priorities

tag groups

tag inheritance rules

tag-based animations

tag-based transitions

End of Specification