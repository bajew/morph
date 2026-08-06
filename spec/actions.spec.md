# Actions Specification

**Version:** 2.0

**Status:** Updated Draft (Regenerated with new standard)

**Audience:** Backend systems, UI runtime engines, AI agents, and tooling that consume or generate page metadata.

**Scope:** Defines the structure, semantics, and processing rules for actions in the Dynamic UI Meta‑Language, including RPC error handling, binding scopes, and cross‑spec consistency.

## 1. Overview

Actions define behavior, not layout. They describe what happens when the user triggers an event such as pressing a button, completing a wizard step, or submitting a form.

Actions are:

Semantic — no UI hints (button, toolbar, swipe). The runtime decides.

Declarative — no embedded logic.

AI‑processable — predictable and explicit.

Platform‑neutral — work across web, mobile, desktop.

Actions appear in:

Single pages

Wizard steps

Completion flows

Error handling flows

## 2. Design Goals

Unified action model — RPC, navigation, and state updates share a common structure.

Explicit effects — success and failure behavior is declarative.

Predictable lifecycle — actions behave consistently across pages and wizards.

AI‑friendly — strict structure, no hidden behavior.

Offline‑capable — actions may use cached data or fallback behavior.

Consistent error model — RPC errors follow a strict, typed structure.

## 3. Action Model

An action describes a behavior triggered by the user or system.

```json
Action {
  "id": "string",
  "title": LocalizedString,
  "type": "rpc" | "navigate" | "state.update" | "state.reset" | "ui.notify",
  "method": "string"?,
  "parameters": object?,
  "onSuccess": ActionEffect[]?,
  "onFailure": ActionEffect[]?
}
```

### 3.1. Required Fields

Field

Type

Required

Description

id

string

yes

Unique identifier.

title

LocalizedString

yes

Display label.

type

string

yes

Action type.

## 4. Action Types

### 4.1. RPC Action

Calls a backend RPC method.

```json
{
  "id": "bookEntry",
  "type": "rpc",
  "title": { "key": "action.book", "default": "Book" },
  "method": "BookEntry",
  "parameters": {
    "barcode": "@state.barcode",
    "entryType": "@wizard.materialType",
    "userId": "@app.user.id"
  }
}
```

RPC Fields

Field

Type

Required

Description

method

string

yes

RPC method name.

parameters

object

no

Parameters passed to the RPC method.

### 4.2. Navigation Action

Navigates to another page.

```json
{
  "id": "goToDashboard",
  "type": "navigate",
  "title": { "key": "action.dashboard", "default": "Dashboard" },
  "parameters": {
    "target": "page.dashboard",
    "parameters": { "filter": "active" }
  }
}
```

Navigation Parameters

Field

Type

Required

Description

target

string

yes

Page identifier.

parameters

object

no

Optional navigation parameters.

### 4.3. State Update Action

Updates state values.

```json
{
  "id": "updateMaterial",
  "type": "state.update",
  "title": { "key": "action.updateMaterial", "default": "Update Material" },
  "parameters": {
    "targets": {
      "state.material.locked": false,
      "wizard.amount": 10,
      "app.settings.language": "de-DE"
    }
  }
}
```

State Update Parameters

Field

Type

Required

Description

targets

object

yes

Key/value pairs of state paths and new values.

### 4.4. State Reset Action

Resets state values.

```json
{
  "id": "resetForm",
  "type": "state.reset",
  "title": { "key": "action.reset", "default": "Reset" },
  "parameters": {
    "targets": ["barcode", "entryType", "locked", "amount"]
  }
}
```

Reset Parameters

Field

Type

Required

Description

targets

string[]

yes

List of state paths to reset.

### 4.5. UI Notification Action

Displays a message.

```json
{
  "id": "showSuccess",
  "type": "ui.notify",
  "title": { "key": "action.notify", "default": "Notify" },
  "parameters": {
    "message": { "key": "msg.success", "default": "Operation completed." }
  }
}
```

Notification Parameters

Field

Type

Required

Description

message

LocalizedString

yes

Notification message.

## 5. RPC Error Model

RPC actions expose a standard error object to failure effects.

```json
RpcError {
  "code": "string",
  "message": "string",
  "details": object?,
  "fieldErrors": { "<fieldId>": "string" }?
}
```

### 5.1. Error Binding Namespace

Failure effects may reference:

@rpcError.code

@rpcError.message

@rpcError.details

@rpcError.fieldErrors.<fieldId>

### 5.2. Example Failure Effect

```json
"onFailure": [
  {
    "action": "ui.notify",
    "message": "@rpcError.message"
  },
  {
    "action": "state.update",
    "targets": {
      "state.validation.barcode": "@rpcError.fieldErrors.barcode"
    }
  }
]
```

## 6. Action Effects

Effects describe what happens after an action succeeds or fails.

```json
ActionEffect {
  "action": "state.update" | "state.reset" | "ui.notify" | "navigate",
  "targets": object?,
  "message": LocalizedString?,
  "parameters": object?
}
```

### 6.1. Success Effects

Triggered when an action completes successfully.

Example:

```json
"onSuccess": [
  { "action": "state.reset", "targets": ["barcode", "entryType"] },
  { "action": "ui.notify", "message": { "key": "msg.success", "default": "Entry booked." } }
]
```

### 6.2. Failure Effects

Triggered when an action fails.

Example:

```json
"onFailure": [
  { "action": "ui.notify", "message": "@rpcError.message" }
]
```

## 7. Action Lifecycle

### 7.1. Initialization

Actions are defined in:

Page actions array

Wizard step actions array

Wizard completion action

### 7.2. Execution

Triggered by:

User interaction

Wizard transitions

System events

### 7.3. Success / Failure

RPC actions evaluate backend response.

Navigation actions always succeed.

State actions always succeed.

Notification actions always succeed.

### 7.4. Cleanup

Wizard completion may reset wizard state.

Page navigation may reset page state.

## 8. Interaction with Bindings

Actions may reference state, wizard, application, source, computed, and error data using one-way `@<path>` expressions. Actions and effects must not use `@bind:<path>`, because they do not accept user input.

### 8.1. Payload Binding

```json
{
  "method": "BookEntry",
  "parameters": {
    "barcode": "@state.barcode",
    "entryType": "@wizard.materialType",
    "userId": "@app.user.id"
  }
}
```

### 8.2. Effect Binding

```json
{
  "action": "state.update",
  "targets": {
    "wizard.amount": 10
  }
}
```

## 9. Localization in Actions

Actions may contain localized strings:

Titles

Notification messages

Navigation parameters

Example:

```json
{
  "title": { "key": "action.book", "default": "Book" }
}
```

## 10. Examples

### 10.1. RPC Action with Success/Failure

```json
{
  "id": "bookEntry",
  "title": { "key": "action.book", "default": "Book" },
  "type": "rpc",
  "method": "BookEntry",
  "parameters": {
    "barcode": "@state.barcode",
    "entryType": "@state.entryType",
    "locked": "@state.locked",
    "amount": "@state.amount"
  },
  "onSuccess": [
    { "action": "state.reset", "targets": ["barcode", "entryType", "locked", "amount"] },
    { "action": "ui.notify", "message": { "key": "msg.success", "default": "Entry booked successfully." } }
  ],
  "onFailure": [
    { "action": "ui.notify", "message": "@rpcError.message" }
  ]
}
```

### 10.2. Navigation Action

```json
{
  "id": "goToDashboard",
  "title": { "key": "action.dashboard", "default": "Dashboard" },
  "type": "navigate",
  "parameters": {
    "target": "page.dashboard"
  }
}
```

### 10.3. State Update Action

```json
{
  "id": "updateSettings",
  "title": { "key": "action.updateSettings", "default": "Update Settings" },
  "type": "state.update",
  "parameters": {
    "targets": {
      "app.settings.language": "de-DE"
    }
  }
}
```

### 10.4. Notification Action

```json
{
  "id": "showError",
  "title": { "key": "action.error", "default": "Error" },
  "type": "ui.notify",
  "parameters": {
    "message": { "key": "msg.error", "default": "An error occurred." }
  }
}
```

---
