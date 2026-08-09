# State Model Specification

**Version:** 1.2 (Consolidated)

**Status:** Draft (Merged from state.spec.md + dynamic-state-model.spec.md)

**Audience:** Backend systems, UI runtime engines, AI agents, and tooling that consume or generate page metadata.

**Scope:** Defines the structure, semantics, lifecycle, and implementation of state in the Morph meta-language, including page state, wizard state, application-wide state, and the dynamic state container engine.

---

## 1. Overview

State represents client-side data used by pages, wizards, and the application runtime. It is implemented through a **Dynamic State Container** - a flexible, schema-free state engine with MVVM notifications.

State is the primary mechanism for:
- storing user input
- caching backend results
- maintaining wizard progress
- holding global application data

The Dynamic State Container behaves like a dynamic JSON object with MVVM change propagation.

---

## 2. Design Goals

- **Clear separation of scopes** — page, wizard, and application state behave differently
- **Predictable lifecycle** — creation, update, reset, destruction
- **Strict binding rules** — state paths must be explicit and valid
- **Declarative updates** — only actions may mutate state (except twoWay bindings)
- **Offline-capable** — application state may persist across sessions
- **AI-friendly** — no implicit behavior, no hidden defaults

---

## 3. State Scopes

### 3.1. Page State (`state.`)

**Lifetime:** Local to a single page instance (created on load, destroyed on navigation)

**Use Case:** Form fields, temporary input, page-local calculations

**Examples:**
```
state.barcode
state.amount
state.material.locked
```

### 3.2. Wizard State (`wizard.`)

**Lifetime:** Shared across all steps of a wizard (created on start, destroyed on completion/cancellation)

**Use Case:** Multi-step flows, accumulated data, step-specific values

**Examples:**
```
wizard.materialType
wizard.steps.step1.amount
wizard.summary.total
```

### 3.3. Application State (`app.`)

**Lifetime:** Global, persistent across pages and sessions (lives until logout or app reset)

**Use Case:** User profile, settings, cached lists, tenant info

**Examples:**
```
app.user.id
app.settings.language
app.cached.entryTypes
```

---

## 4. State Structure

State is a hierarchical object with arbitrary nesting using dot notation:

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

---

## 5. Dynamic State Container API

The state engine is implemented as a `DynamicStateContainer` class with the following interface:

### 5.1. Get - Retrieve Values

Retrieve a value by hierarchical path.

**Signature:**
```csharp
object? Get(string path)
T? Get<T>(string path)
```

**Example:**
```csharp
var barcode = State.Get<string>("barcode");
var userId = App.Get<int>("user.id");
```

### 5.2. Set - Assign Values

Assign a value by hierarchical path. Automatically creates missing nested dictionaries and raises `PropertyChanged(path)`.

**Signature:**
```csharp
void Set(string path, object? value)
void Set<T>(string path, T value)
```

**Example:**
```csharp
State.Set("material.locked", true);
Wizard.Steps["step1"].Set("amount", 10);
App.Set("user.id", 42);
```

### 5.3. Reset - Restore Defaults or Remove

Reset a value to its default (if defined) or remove the path.

**Signature:**
```csharp
void Reset(string path)
```

**Behavior:**
- If defaults exist: resets to default value
- Otherwise: removes the path from state

### 5.4. InitializeDefaults - Set Default Values

Initialize default values for reset operations.

**Signature:**
```csharp
void InitializeDefaults(Dictionary<string, object?> defaults)
```

### 5.5. Change Events

**Event:** `StateChanged`
```csharp
event EventHandler<StateChangedEventArgs> StateChanged;
```

Used by computed values and wizard transitions to react to state changes.

---

## 6. Implementation Reference

Below is the complete reference implementation of the Dynamic State Container:

```csharp
public class DynamicStateContainer : ObservableObject
{
    private readonly Dictionary<string, object?> _root = new();
    private readonly Dictionary<string, object?> _defaults = new();

    public object? Get(string path)
    {
        return ResolvePath(_root, path.Split('.'));
    }

    public T? Get<T>(string path)
    {
        return (T?)Get(path);
    }

    public void Set(string path, object? value)
    {
        SetInternal(_root, path.Split('.'), value);
        OnPropertyChanged(path);
        StateChanged?.Invoke(this, new StateChangedEventArgs(path, value));
    }

    public void Reset(string path)
    {
        if (_defaults.TryGetValue(path, out var defaultValue))
        {
            Set(path, defaultValue);
        }
        else
        {
            RemovePath(_root, path.Split('.'));
            OnPropertyChanged(path);
        }
    }

    public void InitializeDefaults(Dictionary<string, object?> defaults)
    {
        foreach (var kv in defaults)
        {
            _defaults[kv.Key] = kv.Value;
            Set(kv.Key, kv.Value);
        }
    }

    private static object? ResolvePath(Dictionary<string, object?> root, string[] parts)
    {
        object? current = root;
        foreach (var part in parts)
        {
            if (current is Dictionary<string, object?> dict && dict.TryGetValue(part, out var next))
                current = next;
            else return null;
        }
        return current;
    }

    private static void SetInternal(Dictionary<string, object?> root, string[] parts, object? value)
    {
        var dict = root;
        for (int i = 0; i < parts.Length - 1; i++)
        {
            var part = parts[i];
            if (!dict.TryGetValue(part, out var next) || next is not Dictionary<string, object?> nested)
            {
                nested = new Dictionary<string, object?>();
                dict[part] = nested;
            }
            dict = nested;
        }
        dict[parts[^1]] = value;
    }

    private static void RemovePath(Dictionary<string, object?> root, string[] parts)
    {
        var dict = root;
        for (int i = 0; i < parts.Length - 1; i++)
        {
            if (dict.TryGetValue(parts[i], out var next) && next is Dictionary<string, object?> nested)
                dict = nested;
            else return;
        }
        dict.Remove(parts[^1]);
    }

    public event EventHandler<StateChangedEventArgs>? StateChanged;
}

public class StateChangedEventArgs : EventArgs
{
    public string Path { get; }
    public object? Value { get; }
    public StateChangedEventArgs(string path, object? value)
    {
        Path = path;
        Value = value;
    }
}
```

---

## 7. State Access Rules

### 7.1. Read Access

State may be read by:
- Field bindings (one-way and two-way)
- Action payloads
- Action effects
- Sources of type `state`
- Computed values (future extension)

### 7.2. Write Access

State may be written **only** by:
- Actions (`state.update`, `state.reset`)
- RPC success/failure effects
- Wizard completion effects
- Two-way field bindings (user input only)

**Important:** Bindings never write directly to state except through twoWay field bindings for user input.

---

## 8. State Lifecycle

### 8.1. Page State Lifecycle

| Phase | Trigger | Behavior |
|-------|---------|----------|
| **Create** | Page loads | New `DynamicStateContainer` instance |
| **Update** | User input or actions | Via twoWay bindings or state.update action |
| **Reset** | state.reset action | Restore defaults or remove paths |
| **Destroy** | Navigation away | Container disposed, state lost |

### 8.2. Wizard State Lifecycle

| Phase | Trigger | Behavior |
|-------|---------|----------|
| **Create** | Wizard starts | Global wizard state + per-step containers |
| **Update** | Bindings or actions | Via bindings across steps |
| **Reset** | Completion/cancellation | All wizard state cleared |
| **Destroy** | Wizard ends | Containers disposed |

### 8.3. Application State Lifecycle

| Phase | Trigger | Behavior |
|-------|---------|----------|
| **Create** | App startup | Global persistent container |
| **Update** | Actions or RPC results | Via state.update or effects |
| **Persist** | Optional | Offline storage support |
| **Destroy** | Logout or app reset | Only on explicit reset |

---

## 9. State Update Actions

### 9.1. state.update Action

Updates one or more state paths across any scope.

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

### 9.2. state.reset Action

Resets one or more state paths to their default values.

```json
{
  "action": "state.reset",
  "targets": ["barcode", "entryType", "locked", "amount"]
}
```

---

## 10. Interaction with Sources

### 10.1. State Source Definition

Reads data from page, wizard, or application state:

```json
{
  "id": "userProfile",
  "type": "state",
  "parameters": {
    "path": "app.user"
  }
}
```

### 10.2. RPC Result → State Update

RPC actions can populate state via success effects:

```json
{
  "onSuccess": [
    {
      "action": "state.update",
      "targets": {
        "app.cached.entryTypes": "@source.entryTypes"
      }
    }
  ]
}
```

---

## 11. Interaction with Actions

### 11.1. Payload Binding

Actions reference state using one-way `@<path>` expressions:

```json
{
  "payload": {
    "barcode": "@state.barcode",
    "entryType": "@wizard.materialType",
    "userId": "@app.user.id"
  }
}
```

### 11.2. Error Handling

RPC errors can be stored in state:

```json
{
  "onFailure": [
    {
      "action": "state.update",
      "targets": {
        "state.validation.barcode": "@rpcError.fieldErrors.barcode"
      }
    }
  ]
}
```

---

## 12. AI Agent Constraints

- State paths must be explicit (no implicit nested object creation)
- No dynamic expressions or computed paths
- No mutation outside actions (except twoWay bindings for user input)
- Wizard state must not be used outside wizard context
- Application state must not be mutated by twoWay bindings unless explicitly allowed

---

## 13. Examples

### 13.1. Page State Example

```json
{
  "id": "barcode",
  "type": "text",
  "value": "@bind:state.barcode"
}
```

### 13.2. Wizard State Example

```json
{
  "id": "materialType",
  "type": "select",
  "value": "@bind:wizard.materialType"
}
```

### 13.3. Application State Example

```json
{
  "id": "languageSelector",
  "type": "select",
  "value": "@bind:app.settings.language"
}
```

---

## 14. Cross-References

- **Bindings:** See `bindings.spec.md` for binding syntax and resolution rules
- **Sources:** See `sources.spec.md` for state source type details
- **Actions:** See `actions.spec.md` for action types and effects
- **Pages:** See `page.spec.md` for page-level state usage
- **Wizards:** See `wizard.spec.md` for wizard-specific state layering

---
