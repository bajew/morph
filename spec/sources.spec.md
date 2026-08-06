# Sources Specification

**Version:** 1.0

**Status:** Draft

**Audience:** Backend systems, UI runtime engines, AI agents, and tooling that consume or generate page metadata.

**Scope:** Defines the structure, semantics, and processing rules for data sources in the Dynamic UI Meta‑Language.

## 1. Overview

Sources provide data to pages, fields, wizard steps, and actions. They are the primary mechanism for retrieving, caching, and exposing structured information to the UI.

Sources are:

Semantic — no layout or rendering hints.

Declarative — no embedded logic.

AI‑processable — predictable and explicit.

Platform‑neutral — work across web, mobile, desktop.

Sources appear in:

Page definitions

Wizard steps

Dropdowns and lists

Action payloads

Application‑wide cached data

## 2. Design Goals

Unified model — RPC, state, and static sources share a common structure.

Explicit behavior — initialization, refresh, caching, and error handling are declarative.

Predictable lifecycle — sources behave consistently across pages and wizards.

AI‑friendly — strict structure, no hidden behavior.

Offline‑capable — sources may use cached data when RPC calls fail.

## 3. Source Model

A source provides data to the UI.

```json
Source {
  "id": "string",
  "type": "rpc" | "state" | "static",
  "parameters": { ... },
  "data": array | object?,
  "cache": CacheConfig?,
  "onError": ErrorHandling?
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

type

string

yes

Source type.

parameters

object

no

Configuration for the source.

data

array or object

required for static sources

Embedded data returned by a static source. Ignored for all other source types.

## 4. Source Types

### 4.1. RPC Source

Fetches data from the backend.

```json
{
  "id": "entryTypes",
  "type": "rpc",
  "parameters": {
    "method": "GetEntryTypes",
    "arguments": {}
  }
}
```

RPC Parameters

Field

Type

Required

Description

method

string

yes

RPC method name.

arguments

object

no

RPC arguments.

RPC Behavior

Loaded on page initialization unless marked lazy.

May populate state via action effects.

May be cached.

May define fallback behavior.

### 4.2. State Source

Reads data from page, wizard, or application state.

```json
{
  "id": "userProfile",
  "type": "state",
  "parameters": {
    "path": "app.user"
  }
}
```

State Parameters

Field

Type

Required

Description

path

string

yes

Path to state (state, wizard, app).

State Behavior

Always available.

Read‑only.

Ideal for global data (user, settings, cached lists).

### 4.3. Static Source

Provides fixed data.

```json
{
  "id": "materialTypes",
  "type": "static",
  "data": [
    { "id": 1, "title": "Machines" },
    { "id": 2, "title": "Tools" },
    { "id": 3, "title": "Misc" }
  ]
}
```

Static Data

Field

Type

Required

Description

data

array

yes

Static data returned by the source.

Static Behavior

Always available.

Ideal for fixed dropdowns.

Uses plain string fields.

## 5. Cache Configuration

Sources may define caching behavior.

```json
CacheConfig {
  "scope": "state" | "wizard" | "app",
  "path": "app.cached.entryTypes"
}
```

### 5.1. Cache Scopes

Scope

Lifetime

Description

state

Page

Destroyed when page closes.

wizard

Wizard

Destroyed when wizard completes.

app

Global

Persistent across navigation.

### 5.2. Cache Behavior

Cached data replaces RPC calls when available.

Cached data may be updated via action effects.

## 6. Error Handling

Sources may define fallback behavior.

```json
ErrorHandling {
  "fallback": "app.cached.entryTypes"
}
```

### 6.1. Error Behavior

RPC errors may trigger fallback.

Fallback must reference a valid state path.

UI runtime may show notifications via actions.

## 7. Source Lifecycle

### 7.1. Initialization

RPC sources load automatically unless marked lazy.

State and static sources are instantly available.

### 7.2. Refresh

Triggered by:

Actions

User interactions

Wizard transitions

### 7.3. Destruction

Page sources destroyed on navigation.

Wizard sources destroyed on completion.

Application sources persist.

## 8. Interaction with Bindings

Fields may bind directly to sources.

### 8.1. Field Binding

```json
{
  "binding": "source.entryTypes",
  "mode": "oneWay"
}
```

### 8.2. Dropdown Options

```json
{
  "options": {
    "source": "entryTypes",
    "valueField": "id",
    "displayField": "title"
  }
}
```

### 8.3. Action Payloads

```json
{
  "payload": {
    "types": "@source.entryTypes"
  }
}
```

## 9. Display Strings in Sources

Sources may contain plain string display fields.

Example:

```json
{
  "id": 2,
  "title": "Tools"
}
```

The UI runtime displays the string value.

## 10. Examples

### 10.1. RPC Source

```json
{
  "id": "entryTypes",
  "type": "rpc",
  "parameters": {
    "method": "GetEntryTypes"
  },
  "cache": {
    "scope": "app",
    "path": "app.cached.entryTypes"
  }
}
```

### 10.2. State Source

```json
{
  "id": "userProfile",
  "type": "state",
  "parameters": {
    "path": "app.user"
  }
}
```

### 10.3. Static Source

```json
{
  "id": "materialTypes",
  "type": "static",
  "data": [
    { "id": 1, "title": "Machines" },
    { "id": 2, "title": "Tools" },
    { "id": 3, "title": "Misc" }
  ]
}
```

---
