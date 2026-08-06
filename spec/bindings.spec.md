# Bindings Specification

**Version:** 1.0

**Status:** Draft

**Audience:** Backend systems, UI runtime engines, AI agents, and tooling that consume or generate page metadata.

**Scope:** Defines the structure, semantics, and processing rules for data bindings in the Dynamic UI Meta‑Language.

## 1. Overview

Bindings define how data flows between UI fields, client state, and data sources. They are the connective tissue of the Dynamic UI Meta‑Language.

Bindings are:

Semantic — no layout or rendering hints.

Declarative — no embedded logic.

AI‑processable — predictable and explicit.

Platform‑neutral — work across web, mobile, desktop.

Bindings appear in:

Fields

Wizard steps

Action payloads

Action effects

Computed values (optional future extension)

## 2. Design Goals

Explicit targets — no implicit binding resolution.

Explicit direction — the binding prefix determines whether a value is read or read and written.

Predictable lifecycle — initialization, update, reset.

Uniform syntax — same binding model for fields and actions.

AI‑friendly — strict structure, no hidden behavior.

## 3. Binding Model

A binding connects a UI element to a value path. Its prefix specifies its direction.

```text
One-way: @<path>
Two-way: @bind:<path>
```

`@<path>` reads a value from the binding context. `@bind:<path>` reads a value and writes user changes back to that path.

### 3.1. Binding Targets

Bindings may reference:

State Binding

state.<path>, wizard.<path>, or app.<path>

Examples:

@state.barcode

@state.material.locked

@state.entryType

@wizard.materialType

@app.user.id

Source Binding

source.<id>

Examples:

@source.entryTypes

@source.userProfile

Computed Binding (future extension)

computed.<id>

Examples:

@computed.totalPrice

@computed.fullName

## 4. Binding Modes

One-way (`@<path>`)

Data flows from state/source to UI.

UI cannot modify the underlying data.

Used for read‑only fields.

Two-way (`@bind:<path>`)

Data flows both ways:

UI reads from state.

UI writes back to state.

Never writes directly to sources.

## 5. Binding Lifecycle

Bindings follow a predictable lifecycle.

### 5.1. Initialization

On page load or wizard step entry:

State values are applied to fields.

Source values populate lists or computed fields.

### 5.2. Update

Triggered by:

User input (for two-way bindings)

RPC responses (via action effects)

State updates (state.update)

### 5.3. Reset

Triggered by:

state.reset action effect

Wizard step transitions

## 6. Binding Usage in Fields

Fields use bindings to connect UI controls to data.

Example:

```json
{
  "id": "barcode",
  "type": "text",
  "value": "@bind:state.barcode"
}
```

### 6.1. Binding to Object Properties

```json
"@bind:state.material.locked"
```

### 6.2. Binding to Lists

Used for dropdowns, multiselects, lists.

```json
"@source.entryTypes"
```

## 7. Options Binding (Dropdowns & Lists)

Dropdowns require additional metadata to map list objects to UI.

```json
OptionsSource {
  "source": "entryTypes",
  "valueField": "id",
  "displayField": "title"
}
```

### 7.1. Value Field

Determines what is stored in state.

Example: id

### 7.2. Display Field

Determines what is shown to the user.

May be a raw string.

May be a localized string.

Example item:

```json
{
  "id": 2,
  "title": { "key": "material.tools", "default": "Tools" }
}
```

## 8. Binding Usage in Actions

Bindings are used inside action payloads and effects.

### 8.1. Payload Binding

```json
{
  "method": "BookEntry",
  "payload": {
    "barcode": "@state.barcode",
    "entryType": "@state.entryType",
    "locked": "@state.locked",
    "amount": "@state.amount"
  }
}
```

### 8.2. Effect Binding

```json
{
  "action": "state.update",
  "targets": {
    "material.locked": false
  }
}
```

## 9. Localization in Bindings

Bindings themselves are not localized. Localization applies to display fields.

Example:

```json
{
  "displayField": "title"
}
```

Where each item contains:

```json
{
  "title": { "key": "material.tools", "default": "Tools" }
}
```

The UI runtime resolves localized strings.

## 10. Examples

### 10.1. Binding to a List of Objects

```json
{
  "id": "entryType",
  "type": "select",
  "value": "@bind:state.entryType",
  "options": {
    "source": "entryTypes",
    "valueField": "id",
    "displayField": "title"
  }
}
```

### 10.2. Binding to a Property of an Object

```json
{
  "id": "locked",
  "type": "boolean",
  "value": "@bind:state.material.locked"
}
```

---
