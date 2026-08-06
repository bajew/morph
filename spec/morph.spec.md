# Morph Specification

**Version:** 1.1

**Status:** Master Specification (Updated)

**Purpose:** Defines the vision, architecture, completed specifications, and remaining open points for the Morph meta‑language engine and runtime.

## 1. Vision

Morph is a declarative meta‑language and runtime designed to build dynamic applications from metadata rather than handcrafted UI code. It aims to:

eliminate repetitive UI development

unify state, actions, sources, and navigation

support dynamic schemas and workflows

provide a consistent, predictable runtime

run across MAUI, web, and desktop environments

Morph is the engine that interprets metadata and produces a fully functional application.

## 2. Architectural Overview

Morph consists of four major layers:

### 2.1. Meta‑Language Layer

Defines the declarative structures:

pages

fields

actions

sources

wizards

state

bindings

tags

### 2.2. ViewModel Engine Layer

Implements:

dynamic state containers

wizard state layering

binding resolution

action execution

source loading

computed values

tag propagation

### 2.3. UI Runtime Layer

Responsible for:

rendering metadata

mapping field types to components

handling events

managing page lifecycle

managing wizard lifecycle

interpreting tags into visual styles

### 2.4. Tooling Layer

Includes:

schema validation

metadata editor

AI generation helpers

preview mode

debugging tools

## 3. Completed Specifications

Morph currently includes the following specifications:

✔ dynamic-state-model.spec.md

Defines the dynamic, hierarchical, observable state engine.

✔ state.spec.md

Defines state scopes, access rules, lifecycle, and state update actions.

✔ sources.spec.md

Defines RPC, state, and static data sources, including caching and fallback behavior.

✔ page.spec.md

Defines page structure, fields, bindings, sources, and page actions.

✔ wizard.spec.md

Defines wizard state layering, lifecycle, and semantics.

✔ actions.spec.md

Defines action types, RPC error model, and success/failure effects.

✔ bindings.spec.md

Defines binding resolution rules, path grammar, and scope resolution.

✔ decorators.spec.md

Defines the unified semantic tag system used across all UI elements.

✔ menu.spec.md

Defines declarative menus, navigation entries, actions, visibility, and tags.

✔ morph.spec.md

Master specification (this document).

## 4. Open Specifications

The following specifications are required for a complete Morph engine.

### 4.1. UI Runtime Specs

table.spec.md — table structure, columns, row actions, tags

layout.spec.md — semantic layout rules

event-model.spec.md — event → action mapping

page-lifecycle.spec.md — initialization, cleanup, source loading

wizard-lifecycle.spec.md — transitions, validation, completion

styling.spec.md — mapping tags → visuals

### 4.2. Action & Behavior Specs

action-executor.spec.md — payload binding, RPC pipeline, effects

navigation.spec.md — page transitions, parameters, guards

effects.spec.md — success/failure effect semantics

### 4.3. Data & Sources Specs

source-provider.spec.md — RPC, static, state sources

rpc.spec.md — RPC structure, error mapping

caching.spec.md — caching rules for sources

### 4.4. Computed & Validation Specs

computed-values.spec.md — dependency tracking, reactive updates

validation.spec.md — field, step, wizard validation

### 4.5. Error Model Specs

error-model.spec.md — binding errors, source errors, navigation errors

### 4.6. Security Specs

auth.spec.md — authentication

permissions.spec.md — authorization

### 4.7. Performance Specs

runtime-performance.spec.md — caching, batching, offline mode

### 4.8. Tooling Specs

schema.spec.md — metadata schema

editor.spec.md — metadata editor

ai-assist.spec.md — AI generation helpers

## 5. Open Points (Detailed)

The following areas require further definition before Morph can run end‑to‑end.

### 5.1. Action Executor

Defines how actions mutate state, call RPC, trigger effects, and navigate.

### 5.2. Source Provider

Defines how RPC/state/static sources load, refresh, and cache.

### 5.3. Computed Values

Defines reactive dependency tracking and derived values.

### 5.4. Table Specification

Defines columns, bindings, row actions, tags, selection, sorting, filtering.

### 5.5. Validation Model

Defines field validation, step validation, wizard validation, and RPC error mapping.

### 5.6. Navigation Model

Defines page transitions, parameters, guards, and wizard transitions.

### 5.7. Error Model

Defines binding errors, source errors, navigation errors, and component errors.

### 5.8. Security Model

Defines authentication and authorization.

### 5.9. Performance Model

Defines caching, batching, and offline mode.

### 5.10. Tooling Model

Defines schema validation, editor, AI helpers, and preview mode.

## 6. Roadmap

A suggested order for completing Morph:

Phase 1 — Behavior & Data

action-executor.spec.md

source-provider.spec.md

computed-values.spec.md

Phase 2 — UI Runtime

table.spec.md

layout.spec.md

event-model.spec.md

page-lifecycle.spec.md

wizard-lifecycle.spec.md

Phase 3 — Validation & Navigation

validation.spec.md

navigation.spec.md

error-model.spec.md

Phase 4 — Tooling & Performance

schema.spec.md

runtime-performance.spec.md

auth.spec.md

## 7. Glossary

Metadata — Declarative JSON describing UI, state, actions, and behavior.State Container — Dynamic hierarchical dictionary with MVVM notifications.Wizard — Multi-step workflow with layered state.Source — Data provider (RPC, static, state).Action — Declarative behavior triggered by events.Binding — Path-based reference to state or sources.Tag — Semantic annotation interpreted by the UI runtime.Computed Value — Derived state updated reactively.

---
