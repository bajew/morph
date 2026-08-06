# Wizard Specification

**Version:** 1.0

**Status:** Draft

**Scope:** Defines the structure, lifecycle, and state layering model for wizards in the meta‑language runtime.

## 1. Overview

A wizard is a multi‑step workflow with shared state, per‑step state, validation, transitions, and completion semantics. Wizard behavior is declarative and driven entirely by metadata.

## 2. Wizard State Model

Wizard state consists of three layers:

### 2.1. Global Wizard State

Shared across all steps.

wizard.materialType
wizard.totalAmount
wizard.summary

Stored in a DynamicStateContainer.

### 2.2. Step State

Each step has its own DynamicStateContainer.

wizard.steps.step1.amount
wizard.steps.step2.location

Steps are created dynamically from metadata.

### 2.3. Computed Wizard State

Derived values based on:

global wizard state

step state

app state

sources

Stored in a DynamicStateContainer.

## 3. Wizard Structure

```json
Wizard {
  "id": "string",
  "title": LocalizedString,
  "steps": Step[],
  "completion": Action[]
}
```

### 3.1. Step Structure

```json
Step {
  "id": "string",
  "title": LocalizedString,
  "fields": Field[],
  "actions": Action[],
  "validation": ValidationRule[]?
}
```

## 4. Wizard Lifecycle

### 4.1. Initialization

Create global wizard state

Create step containers

Initialize defaults

Set CurrentStepId to first step

### 4.2. Step Execution

Render fields for current step

Bind fields to step state

Execute step actions

Validate step

### 4.3. Step Transition

GoToStep(stepId)

Updates CurrentStepId and triggers UI refresh.

### 4.4. Completion

Triggered when final step is valid.

Execute completion actions

Reset wizard state

Navigate to target page

### 4.5. Cancellation

CancelWizard()

Resets:

global wizard state

all step states

computed values

## 5. Wizard Bindings

Bindings may reference:

wizard.<path>
wizard.steps.<stepId>.<path>
wizard.computed.<id>

Examples:

wizard.materialType
wizard.steps.step1.amount
wizard.computed.total

## 6. Wizard Actions

Actions may update wizard state:

"targets": {
  "wizard.materialType": 2,
  "wizard.steps.step1.amount": 10,
  "wizard.steps.step2.location": "A1"
}

## 7. Validation

Each step may define validation rules. Validation determines whether the user may proceed.

## 8. Example

wizard.materialType = 2
wizard.steps.step1.amount = 10
wizard.steps.step2.location = "A1"
wizard.computed.total = 10

---
