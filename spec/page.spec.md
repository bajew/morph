# Page Specification (Consolidated)

**Version:** 1.2 (Consolidated)

**Status:** Updated - Removed duplicate binding syntax, added cross-references

**Purpose:** Defines the structure, semantics, and processing rules for Morph pages. Pages are declarative UI containers composed of fields, sources, and actions.

---

## 1. Overview

A page in Morph is a declarative metadata object that describes:
- what fields appear on the page
- how those fields behave
- what backend sources the page loads
- what actions the page can execute

Pages do not define layout, styling, or rendering. The UI runtime interprets page metadata and renders it according to platform conventions.

---

## 2. Page Structure

A page is defined as:

```json
{
  "id": "page.example",
  "type": "page",
  "title": "Example Page",
  "sources": [ ... ],
  "fields": [ ... ],
  "actions": [ ... ]
}
```

| Component | Description |
|-----------|-------------|
| **Fields** | Declarative UI elements such as labels, text inputs, numbers, tables, etc. |
| **Sources** | Backend data providers that populate the page. |
| **Actions** | Operations triggered by user interaction (e.g., RPC calls). |

---

## 3. Properties

| Property | Required | Description |
|----------|----------|-------------|
| `id` | yes | Unique identifier for the page. |
| `type` | yes | Always `"page"`. |
| `title` | yes | Human-readable title. |
| `sources` | no | Array of source identifiers. |
| `fields` | yes | Array of field definitions. |
| `actions` | no | Array of action definitions. |

---

## 4. Fields

Fields define the UI elements of a page.

### 4.1. Field Structure

```json
{
  "id": "fieldId",
  "type": "fieldType",
  "...typeSpecificProperties"
}
```

### 4.2. Common Field Properties

| Property | Required | Description |
|----------|----------|-------------|
| `id` | yes | Unique within the page. |
| `type` | yes | Field type (label, text, etc.). |
| `value` | optional | Static or bound value. |
| `hint` | optional | Static or bound hint text (input guidance). |
| `tags` | optional | Semantic decorators. See `decorators.spec.md`. |

### 4.3. Binding Syntax

**See `bindings.spec.md` for complete binding syntax and resolution rules.**

Brief summary:
- **One-way:** `@<path>` - reads a value (e.g., `@state.tool.name`)
- **Two-way:** `@bind:<path>` - reads and writes user changes back (e.g., `@bind:state.barcode`)

Bindings may appear in any string property including `value`, `hint`, options, visibility, etc.

**Examples:**

Static label:
```json
{ "id": "lbl", "type": "label", "value": "Enter tool name:" }
```

Dynamic label (one-way):
```json
{ "id": "lbl", "type": "label", "value": "@state.tool.name" }
```

Text field with two-way binding:
```json
{ "id": "name", "type": "text", "hint": "Tool name", "value": "@bind:state.name" }
```

### 4.4. Field Types

#### 4.4.1. label

Displays static or bound text.

**Properties:**
- `value` — required for display (static string or binding expression)

**Examples:**
```json
{ "id": "lbl", "type": "label", "value": "Enter tool name:" }
{ "id": "lbl", "type": "label", "value": "@state.tool.name" }
```

#### 4.4.2. text

Accepts user input.

**Properties:**
- `hint` — optional guidance text (static or bound)
- `value` — optional default or bound value

**Examples:**
```json
{ "id": "name", "type": "text", "hint": "Tool name" }
{ "id": "name", "type": "text", "hint": "@state.tool.placeholder" }
{ "id": "name", "type": "text", "value": "@bind:state.name" }
```

**Note:** Additional field types (select, number, boolean, table, etc.) are defined in future specifications. See `table.spec.md` for table fields.

---

## 5. Sources

Pages may reference multiple sources by identifier:

```json
"sources": ["source.tools", "source.archive"]
```

Each source is resolved before fields are rendered. The UI runtime decides how to display source data.

**See `sources.spec.md` for complete source type definitions (RPC, state, static).**

---

## 6. Actions

Actions define operations triggered by user interaction on the page.

### 6.1. Action Structure

```json
{
  "id": "submit",
  "type": "rpc",
  "method": "AddTool",
  "parameters": {
    "name": "@state.name"
  }
}
```

### 6.2. Action Types

| Type | Description |
|------|-------------|
| `rpc` | Calls a backend RPC method. |
| `navigate` | Navigates to another page or wizard. |
| `state.update` | Updates state values. |
| `state.reset` | Resets state values. |
| `ui.notify` | Displays a notification message. |

**See `actions.spec.md` for complete action type definitions, error handling, and effects.**

### 6.3. Parameter Binding

Parameters use one-way `@<path>` expressions to reference:
- State (page, wizard, application)
- Computed values
- Source data

**Example:**
```json
"parameters": { "id": "@state.toolId" }
```

---

## 7. Example Page

Complete example with all components:

```json
{
  "id": "page.addTool",
  "type": "page",
  "title": "Add Tool",
  "sources": [],
  "fields": [
    {
      "id": "lbl",
      "type": "label",
      "value": "Enter tool name:"
    },
    {
      "id": "name",
      "type": "text",
      "hint": "Tool name",
      "value": "@bind:state.name"
    }
  ],
  "actions": [
    {
      "id": "submit",
      "type": "rpc",
      "method": "AddTool",
      "parameters": {
        "name": "@state.name"
      },
      "onSuccess": [
        { "action": "ui.notify", "message": "Tool added successfully." }
      ]
    }
  ]
}
```

---

## 8. Cross-References

| Topic | See Specification |
|-------|------------------|
| **Binding Syntax** | `bindings.spec.md` - Complete binding rules and resolution |
| **State Scopes** | `state-model.spec.md` - Page, wizard, and application state |
| **Source Types** | `sources.spec.md` - RPC, state, and static sources |
| **Action Types** | `actions.spec.md` - All action types and effects |
| **Tags/Decorators** | `decorators.spec.md` - Semantic tag system |
| **Tables** | `table.spec.md` (future) - Table field type definition |

---
