# State Specification

**Version:** 1.0

**Status:** Draft (Stored in project)

**Audience:** Backend systems, UI runtime engines, AI agents, and tooling that consume or generate page metadata.

**Scope:** Defines the structure, semantics, and lifecycle of state in the Dynamic UI Meta‑Language, including page state, wizard state, and application‑wide state.

## 1. Overview

State represents client‑side data used by pages, wizards, and the application runtime. It is the primary mechanism for:

storing user input

caching backend results

maintaining wizard progress

holding global application data

State is semantic, declarative, and AI‑processable.

## 2. Design Goals

Clear separation of scopes — page, wizard, and application state behave differently.

Predictable lifecycle — creation, update, reset, destruction.

Strict binding rules — state paths must be explicit and valid.

Declarative updates — only actions may mutate state.

Offline‑capable — application state may persist across sessions.

AI‑friendly — no implicit behavior, no hidden defaults.

## 3. State Scopes

### 3.1. Page State (state)

Local to a single page instance.

Created when the page loads.

Destroyed when the page is closed or navigated away.

Ideal for form fields, temporary input, and page‑local calculations.

Example:

state.barcode
state.amount
state.material.locked

### 3.2. Wizard State (wizard)

Shared across all steps of a wizard.

Created when the wizard starts.

Destroyed when the wizard completes or is cancelled.

Ideal for multi‑step flows and accumulated data.

Example:

wizard.materialType
wizard.step1.amount
wizard.summary.total

### 3.3. Application State (app)

Global, persistent, available everywhere.

Lives across pages and navigation.

May persist offline.

Ideal for user profile, settings, cached lists, tenant info.

Example:

app.user.id
app.settings.language
app.cached.entryTypes

## 4. State Structure

State is a hierarchical object with arbitrary nesting.

```json
{
  "state": {
    "barcode": "",
    "amount": 0,
    "material": {
      "locked": false
    }
  },
  "wizard": {
    "materialType": 2,
    "step1": { "amount": 10 }
  },
  "app": {
    "user": { "id": 123, "name": "Alice" },
    "settings": { "language": "en-US" },
    "cached": { "entryTypes": [] }
  }
}
```

## 5. State Access Rules

### 5.1. Read Access

State may be read by:

field bindings

action payloads

action effects

sources of type state

computed values (future extension)

### 5.2. Write Access

State may be written only by actions:

state.update

state.reset

RPC success effects

wizard completion effects

Bindings never write directly to state except through twoWay field bindings.

## 6. Binding Model for State

Bindings reference state using explicit paths:

state.<path>
wizard.<path>
app.<path>

Examples:

```json
{
  "source": {
    "binding": "state.barcode",
    "mode": "twoWay"
  }
}
```

```json
{
  "source": {
    "binding": "wizard.materialType",
    "mode": "twoWay"
  }
}
```

```json
{
  "source": {
    "binding": "app.settings.language",
    "mode": "twoWay"
  }
}
```

## 7. State Lifecycle

### 7.1. Page State Lifecycle

Create: when page loads

Update: via twoWay bindings or actions

Reset: via state.reset

Destroy: when navigating away

### 7.2. Wizard State Lifecycle

Create: when wizard starts

Update: via bindings or actions

Reset: via wizard completion or cancellation

Destroy: when wizard ends

### 7.3. Application State Lifecycle

Create: at app startup

Update: via actions or RPC results

Persist: optional offline storage

Destroy: only on logout or app reset

## 8. State Update Actions

### 8.1. state.update

Updates one or more state paths.

```json
{
  "action": "state.update",
  "targets": {
    "state.material.locked": false,
    "wizard.amount": 10,
    "app.settings.language": "de-DE"
  }
}
```

### 8.2. state.reset

Resets one or more state paths to their default values.

```json
{
  "action": "state.reset",
  "targets": ["barcode", "entryType", "locked", "amount"]
}
```

## 9. Interaction with Sources

### 9.1. State Source

```json
{
  "id": "userProfile",
  "type": "state",
  "parameters": {
    "path": "app.user"
  }
}
```

### 9.2. RPC Result → State

```json
{
  "action": "state.update",
  "targets": {
    "app.cached.entryTypes": "@source.entryTypes"
  }
}
```

## 10. Interaction with Actions

### 10.1. Payload Binding

```json
{
  "payload": {
    "barcode": "@state.barcode",
    "entryType": "@wizard.materialType",
    "userId": "@app.user.id"
  }
}
```

### 10.2. Error Handling

```json
{
  "action": "state.update",
  "targets": {
    "state.validation.barcode": "@rpcError.fieldErrors.barcode"
  }
}
```

## 11. AI Agent Constraints

State paths must be explicit.

No implicit creation of nested objects.

No dynamic expressions.

No computed paths.

No mutation outside actions.

Wizard state must not be used outside wizard context.

Application state must not be mutated by twoWay bindings unless explicitly allowed.

## 12. Examples

### 12.1. Page State Example

```json
{
  "binding": "state.amount",
  "mode": "twoWay"
}
```

### 12.2. Wizard State Example

```json
{
  "binding": "wizard.materialType",
  "mode": "twoWay"
}
```

### 12.3. Application State Example

```json
{
  "binding": "app.settings.language",
  "mode": "twoWay"
}
```

---
