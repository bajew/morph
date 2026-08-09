# Morph DSL Proposal

**Version:** 0.2 (Updated - Navigation-only arrows)

**Status:** Experimental - Under Discussion

**Purpose:** Define a simplified, human-readable Domain Specific Language for Morph metadata as an alternative to JSON.

---

## Overview

This document proposes a clean, indentation-based DSL that replaces verbose JSON with concise, readable syntax while maintaining all the semantic power of the current specification.

### Design Principles

1. **Readability first** - Humans should easily understand and write it
2. **Indentation-based structure** - Like YAML, but simpler
3. **Type inference** - Minimize explicit type declarations
4. **Declarative style** - Describe what, not how
5. **Backwards compatible** - Must compile to existing JSON format

---

## Syntax Conventions

| Concept | Syntax | Example |
|---------|--------|---------|
| **Definition** | `keyword Name:` | `page Login:` |
| **Nested block** | Indentation (2 spaces) | Fields under page |
| **List item** | `-` prefix | Menu items, fields |
| **Binding** | `@path` or `@bind:path` | `@state.name` |
| **Assignment** | `=` | `value = "text"` |
| **Navigation** | `-> target` | `-> page Dashboard` |
| **RPC call** | `call Method()` | `call RefreshData()` |
| **Comment** | `#` | `# This is a comment` |
| **String** | Optional quotes | `title Dashboard` or `title "Dashboard"` |

---

## 1. Pages

### JSON (Current)

```json
{
  "id": "page.dashboard",
  "type": "page",
  "title": "Dashboard",
  "sources": ["users", "orders"],
  "fields": [
    {
      "id": "welcomeLabel",
      "type": "label",
      "value": "@state.greeting"
    },
    {
      "id": "searchBox",
      "type": "text",
      "hint": "Search...",
      "value": "@bind:state.searchQuery"
    }
  ],
  "actions": [
    {
      "id": "refresh",
      "type": "rpc",
      "method": "RefreshDashboard",
      "onSuccess": [
        { "action": "ui.notify", "message": "Updated!" }
      ]
    }
  ]
}
```

### DSL (Proposed)

```dsl
page Dashboard:
  title "Main Dashboard"
  
  sources:
    - users
    - orders
  
  fields:
    - label welcomeLabel: @state.greeting
    
    - text searchBox:
        hint "Search..."
        value @bind:state.searchQuery
  
  actions:
    - rpc refresh calls RefreshDashboard():
        onSuccess:
          - notify "Updated!"
```

---

## 2. Menus

### JSON (Current)

```json
{
  "id": "mainMenu",
  "type": "menu",
  "items": [
    {
      "id": "dashboard",
      "title": "Dashboard",
      "navigate": "page.dashboard",
      "tags": ["highlight"]
    },
    {
      "id": "users",
      "title": "Users",
      "navigate": "page.users"
    },
    {
      "id": "setupWizard",
      "title": "Setup Wizard",
      "navigate": "wizard.setup",
      "visibleWhen": "@state.user.isAdmin"
    }
  ]
}
```

### DSL (Proposed)

```dsl
menu MainMenu:
  - Dashboard -> page Dashboard       [highlight]
  - Users     -> page Users
  - Orders    -> page Orders
  - Setup Wizard -> wizard SetupWizard
      if @state.user.isAdmin
  
# Alternative verbose syntax:
menu MainMenu (verbose):
  items:
    - id dashboard
      title "Dashboard"
      navigate page Dashboard
      tags [highlight]
    
    - id users
      title "Users"
      navigate page Users
    
    - id setupWizard
      title "Setup Wizard"
      navigate wizard SetupWizard
      visibleWhen @state.user.isAdmin
```

---

## 3. Wizards

### JSON (Current)

```json
{
  "id": "wizard.setup",
  "title": "Setup Wizard",
  "steps": [
    {
      "id": "step1",
      "title": "Enter Details",
      "fields": [
        {
          "id": "name",
          "type": "text",
          "value": "@bind:wizard.steps.step1.name"
        },
        {
          "id": "email",
          "type": "text",
          "value": "@bind:wizard.steps.step1.email"
        }
      ],
      "actions": [
        {
          "id": "next",
          "type": "navigate",
          "parameters": { "target": "step2" }
        }
      ]
    },
    {
      "id": "step2",
      "title": "Review",
      "fields": [
        {
          "id": "summary",
          "type": "label",
          "value": "Name: @wizard.steps.step1.name"
        }
      ]
    }
  ],
  "completion": [
    {
      "action": "rpc",
      "method": "CompleteSetup",
      "parameters": {
        "name": "@wizard.steps.step1.name",
        "email": "@wizard.steps.step1.email"
      },
      "onSuccess": [
        { "action": "navigate", "parameters": { "target": "page.dashboard" } }
      ]
    }
  ]
}
```

### DSL (Proposed)

```dsl
wizard SetupWizard:
  title "Setup Wizard"
  
  step Step1 "Enter Details":
    fields:
      - text name: @bind:wizard.name
      - text email: @bind:wizard.email
    
    actions:
      - navigate next -> step Step2
  
  step Step2 "Review":
    fields:
      - label summary: "Name: @wizard.name"
      - label emailLabel: "Email: @wizard.email"
    
    actions:
      - navigate back -> step Step1
      - rpc finish calls CompleteSetup():
          params:
            name: @wizard.name
            email: @wizard.email
          onSuccess:
            - navigate -> page Dashboard

# Alternative compact syntax:
wizard QuickSetup (compact):
  title "Quick Setup"
  
  step1 "Details":
    text name: @bind:wizard.name
    text email: @bind:wizard.email
  
  step2 "Review":
    label summary: "Configuring: @wizard.name"
    
  onComplete calls CompleteSetup():
    params:
      name: @wizard.name
      email: @wizard.email
    success: navigate -> page Dashboard
```

---

## 4. Actions

### JSON (Current)

```json
{
  "id": "bookEntry",
  "type": "rpc",
  "title": "Book Entry",
  "method": "BookEntry",
  "parameters": {
    "barcode": "@state.barcode",
    "entryType": "@state.entryType",
    "amount": "@state.amount"
  },
  "onSuccess": [
    { "action": "state.reset", "targets": ["barcode", "entryType", "amount"] },
    { "action": "ui.notify", "message": "Entry booked successfully." }
  ],
  "onFailure": [
    { "action": "ui.notify", "message": "@rpcError.message" }
  ]
}
```

### DSL (Proposed)

```dsl
# Standalone action definition
action BookEntry:
  type rpc
  calls BookEntry()
  
  params:
    barcode: @state.barcode
    entryType: @state.entryType
    amount: @state.amount
  
  onSuccess:
    - reset [barcode, entryType, amount]
    - notify "Entry booked successfully."
  
  onFailure:
    - notify @rpcError.message

# Inline action (within page/wizard)
actions:
  - rpc book calls BookEntry():
      params:
        barcode: @state.barcode
        entryType: @state.entryType
      onSuccess:
        - reset [barcode, entryType]
        - notify "Booked!"
      onFailure:
        - notify @rpcError.message

# State update action  
action UpdateSettings:
  type state.update
  
  targets:
    app.settings.language: "de-DE"
    app.theme: "dark"

# Navigation action (uses -> for navigation)  
action GoToDashboard:
  type navigate
  target page Dashboard
  params filter: "active"

# Or inline navigation in actions block
actions:
  - navigate home -> page Dashboard
  - navigate settings -> page Settings if @app.user.isAdmin
```

---

## 5. Sources

### JSON (Current)

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

{
  "id": "userProfile",
  "type": "state",
  "parameters": {
    "path": "app.user"
  }
}

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

### DSL (Proposed)

```dsl
# RPC source with caching
source EntryTypes:
  type rpc
  calls GetEntryTypes()
  cache app.cached.entryTypes

# State source
source UserProfile:
  type state
  path app.user

# Static source
source MaterialTypes:
  type static
  data:
    - id: 1, title: "Machines"
    - id: 2, title: "Tools"
    - id: 3, title: "Misc"

# Compact syntax
sources:
  rpc EntryTypes calls GetEntryTypes() cache app.cached.entryTypes
  state UserProfile path app.user
  static MaterialTypes:
    - {id: 1, title: "Machines"}
    - {id: 2, title: "Tools"}
    - {id: 3, title: "Misc"}
```

---

## 6. Fields (Detailed)

### JSON (Current)

```json
{
  "fields": [
    {
      "id": "barcode",
      "type": "text",
      "hint": "Enter barcode",
      "value": "@bind:state.barcode",
      "tags": ["required"]
    },
    {
      "id": "entryType",
      "type": "select",
      "value": "@bind:state.entryType",
      "options": {
        "source": "entryTypes",
        "valueField": "id",
        "displayField": "title"
      }
    },
    {
      "id": "locked",
      "type": "boolean",
      "value": "@bind:state.material.locked"
    },
    {
      "id": "amount",
      "type": "number",
      "value": "@bind:state.amount",
      "min": 0,
      "max": 1000
    }
  ]
}
```

### DSL (Proposed)

```dsl
fields:
  # Text field with binding and tag
  - text barcode [required]:
      hint "Enter barcode"
      value @bind:state.barcode
  
  # Select/dropdown with options source
  - select entryType:
      value @bind:state.entryType
      options from entryTypes (id, title)
  
  # Boolean/checkbox
  - boolean locked: @bind:state.material.locked
  
  # Number field with constraints
  - number amount:
      value @bind:state.amount
      min 0
      max 1000

# Compact inline syntax
fields (compact):
  text barcode [required]: "Enter barcode" @bind:state.barcode
  select entryType from entryTypes (id, title): @bind:state.entryType
  boolean locked: @bind:state.material.locked
  number amount [0-1000]: @bind:state.amount
```

---

## 7. State Definitions

### JSON (Current)

```json
{
  "defaults": {
    "state.barcode": "",
    "state.entryType": null,
    "state.material.locked": false,
    "app.settings.language": "en-US",
    "app.user.id": null
  }
}
```

### DSL (Proposed)

```dsl
# State defaults initialization
defaults:
  state:
    barcode: ""
    entryType: null
    material.locked: false
  
  app:
    settings.language: "en-US"
    user.id: null

# Compact syntax
defaults (compact):
  state.barcode = ""
  state.entryType = null
  state.material.locked = false
  app.settings.language = "en-US"
```

---

## 8. Decorators/Tags

### JSON (Current)

```json
{
  "tags": ["maintenanceDue", "critical", "bold"]
}
```

### DSL (Proposed)

```dsl
# Tags on any element
page CriticalPage [maintenanceDue, critical]:
  title "Critical Page"
  
  fields:
    - label warning [bold, highlight]: "Attention Required!"
    
    - text inputField [invalid]:
        value @bind:state.input

# Alternative syntax with explicit tags block
page AnotherPage:
  title "Another Page"
  tags:
    - maintenanceDue
    - critical
```

---

## 9. Complete Example: Inventory Management System

### Full DSL Document

```dsl
# ============================================================================
# Morph DSL Example: Inventory Management System
# ============================================================================

# --- SOURCES -----------------------------------------------------------------

sources:
  rpc Tools calls GetTools() cache app.cached.tools
  rpc EntryTypes calls GetEntryTypes() cache app.cached.entryTypes
  state CurrentUser path app.user
  static Statuses:
    - {id: "active", title: "Active"}
    - {id: "inactive", title: "Inactive"}


# --- MAIN MENU ---------------------------------------------------------------

menu MainMenu:
  - Dashboard -> page Dashboard      [highlight]
  - Inventory -> page Inventory
  - Add Tool -> wizard AddToolWizard
  - Settings -> page Settings        if @app.user.isAdmin
  
  
# --- DASHBOARD PAGE ----------------------------------------------------------

page Dashboard:
  title "Dashboard"
  sources [Tools, CurrentUser]
  
  fields:
    - label welcome: "Welcome, @CurrentUser.name!"
    
    - text searchBox:
        hint "Search tools..."
        value @bind:state.searchQuery
    
    - table toolsTable:
        source Tools
        columns:
          - name -> "Name" [bold]
          - status -> "Status"
          - lastMaintenance -> "Last Maintenance"
        tags from tool.tags


# --- INVENTORY PAGE ----------------------------------------------------------

page Inventory:
  title "Tool Inventory"
  sources [Tools, EntryTypes, Statuses]
  
  fields:
    - label title: "Inventory Management"
    
    # Filter section
    - select entryTypeFilter:
        hint "Filter by type"
        value @bind:state.filter.entryType
        options from EntryTypes (id, title)
    
    - select statusFilter:
        hint "Filter by status"  
        value @bind:state.filter.status
        options from Statuses (id, title)
    
    # Search
    - text searchInput [required]:
        hint "Search inventory..."
        value @bind:state.searchQuery
    
    # Results table
    - toolTable inventoryGrid:
        source Tools
        filters:
          entryType: @state.filter.entryType
          status: @state.filter.status
          search: @state.searchQuery
        columns:
          - barcode -> "Barcode" [bold]
          - name -> "Name"
          - entryType -> "Type"
          - status -> "Status"
          - amount -> "Amount" [right-align]
        actions:
          - edit calls EditTool()
          - delete calls DeleteTool()
    
    # Pagination
    - label totalItems: "@state.results.total items found"

  actions:
    - rpc refresh calls RefreshInventory():
        params:
          search: @state.searchQuery
          entryType: @state.filter.entryType
        onSuccess:
          - notify "Inventory refreshed"
    
    - navigate export -> page ExportReport


# --- ADD TOOL WIZARD ---------------------------------------------------------

wizard AddToolWizard:
  title "Add New Tool"
  
  step Step1 "Basic Information":
    fields:
      - text barcode [required]:
          hint "Scan or enter barcode"
          value @bind:wizard.barcode
      
      - text name [required]:
          hint "Tool name"
          value @bind:wizard.name
      
      - select entryType [required]:
          hint "Select type"
          value @bind:wizard.entryType
          options from EntryTypes (id, title)
    
    validation:
      barcode: notEmpty
      name: notEmpty
      entryType: notEmpty
    
    actions:
      - navigate next -> step Step2 if valid
  
  
  step Step2 "Details":
    fields:
      - number amount [required]:
          hint "Initial quantity"
          value @bind:wizard.amount
          min 1
      
      - text location:
          hint "Storage location (optional)"
          value @bind:wizard.location
      
      - boolean active:
          label "Active?"
          value @bind:wizard.active
          default true
    
    actions:
      - navigate back -> step Step1
      - navigate next -> step Step3 if valid
  
  
  step Step3 "Review":
    fields:
      - label reviewTitle: "Review Entry"
      
      - label barcodeDisplay: "Barcode: @wizard.barcode" [bold]
      - label nameDisplay: "Name: @wizard.name" [bold]
      - label typeDisplay: "Type: @wizard.entryType"
      - label amountDisplay: "Amount: @wizard.amount"
      - label locationDisplay: "Location: @wizard.location" if @wizard.location
      
      - summaryCard reviewSummary:
          fields [barcode, name, entryType, amount, location]
    
    actions:
      - navigate back -> step Step2
      - rpc submit calls AddTool():
          params:
            barcode: @wizard.barcode
            name: @wizard.name
            entryType: @wizard.entryType
            amount: @wizard.amount
            location: @wizard.location
            active: @wizard.active
          onSuccess:
            - notify "Tool added successfully!"
            - reset wizard
            - navigate -> page Inventory
          onFailure:
            - notify @rpcError.message


# --- ACTIONS -----------------------------------------------------------------

actions:
  rpc EditTool calls OpenEditPage():
    params toolId: @state.selectedTool.id
    onSuccess:
      - navigate -> page ToolEditor
    
  rpc DeleteTool calls DeleteToolById():
    params toolId: @state.selectedTool.id
    confirm "Are you sure?"
    onSuccess:
      - notify "Tool deleted"
      - refresh source Tools
    
  rpc RefreshInventory calls GetTools():
    params:
      search: @state.searchQuery
      entryType: @state.filter.entryType
      status: @state.filter.status


# --- STATE DEFAULTS ----------------------------------------------------------

defaults:
  state.searchQuery = ""
  state.filter.entryType = null
  state.filter.status = null
  
  wizard.barcode = ""
  wizard.name = ""
  wizard.amount = 1
  wizard.active = true


# --- BINDING EXAMPLES --------------------------------------------------------

# One-way binding (read-only)
label statusLabel: @state.tool.status

# Two-way binding (editable)  
text nameInput: @bind:state.tool.name

# Source binding for dropdowns
select typeSelector from EntryTypes (id, title): @bind:state.entryType

# Conditional visibility
button adminButton if @app.user.isAdmin:
  action -> AdminAction

# Dynamic tags from data
table toolsTable tags from tool.tags:
  columns [...]
```

---

## 10. Syntax Reference Summary

### Keywords

| Keyword | Purpose | Example |
|---------|---------|---------|
| `page` | Define a page | `page Dashboard:` |
| `menu` | Define a menu | `menu MainMenu:` |
| `wizard` | Define a wizard | `wizard SetupWizard:` |
| `step` | Wizard step | `step Step1 "Title":` |
| `source` / `sources` | Data sources | `rpc Tools calls GetTools()` |
| `fields` | UI fields container | `fields:` |
| `actions` | Action definitions | `actions:` |
| `defaults` | State defaults | `defaults:` |

### Field Types

| Type | DSL Syntax | JSON Equivalent |
|------|------------|-----------------|
| Label | `label id: value` | `{type: "label"}` |
| Text input | `text id: value` | `{type: "text"}` |
| Number | `number id: value` | `{type: "number"}` |
| Boolean | `boolean id: value` | `{type: "boolean"}` |
| Select/Dropdown | `select id from source (val, disp)` | `{type: "select", options: {...}}` |
| Table | `table id:` block | `{type: "table"}` |

### Action Types

| Type | DSL Syntax | Example |
|------|------------|---------|
| RPC | `rpc name calls Method()` | `rpc save calls SaveData()` |
| Navigate | `navigate name -> target` | `navigate home -> page Home` |
| State Update | `update targets {...}` | `update state.x = 1` |
| Reset | `reset [paths]` | `reset [field1, field2]` |
| Notify | `notify "message"` | `notify "Done!"` |

### Binding Syntax

| Type | DSL | JSON |
|------|-----|------|
| One-way | `@path` | `"@path"` |
| Two-way | `@bind:path` | `"@bind:path"` |
| Source | `from source (val, disp)` | `{source: "...", valueField: "...", displayField: "..."}` |

### Tags/Decorators

| Syntax | Example |
|--------|---------|
| Inline tags | `element [tag1, tag2]` |
| Block tags | `tags: - tag1 - tag2` |
| Dynamic tags | `tags from data.tags` |

---

## 11. Comparison: JSON vs DSL

### Complexity Reduction

| Aspect | JSON Lines | DSL Lines | Reduction |
|--------|------------|-----------|-----------|
| Simple page | ~30 lines | ~15 lines | 50% |
| Wizard (3 steps) | ~80 lines | ~40 lines | 50% |
| Menu (5 items) | ~40 lines | ~8 lines | 80% |
| Action with effects | ~20 lines | ~10 lines | 50% |

### Readability Improvements

**JSON:** Nested objects, quotes everywhere, verbose structure  
**DSL:** Flat hierarchy, minimal punctuation, semantic keywords

---

## 12. Implementation Considerations

### Parser Requirements

1. **Lexer**: Tokenize keywords, identifiers, bindings, operators
2. **Parser**: Build AST from indentation-based structure
3. **Validator**: Check against Morph schema
4. **Code Generator**: Output JSON for runtime consumption

### Tooling Support

- Syntax highlighting in editors (VS Code, etc.)
- Auto-completion for keywords and paths
- Real-time validation
- Format/prettify commands
- JSON ↔ DSL conversion utilities

### Migration Path

1. Phase 1: DSL parser + validator (read-only)
2. Phase 2: IDE support + auto-generation
3. Phase 3: Bidirectional sync (DSL ↔ JSON)
4. Phase 4: Native DSL runtime (optional future)

---

## 13. Open Questions

1. **Should we support both JSON and DSL?** (Recommended: yes, for transition)
2. **What about complex nested structures?** (Need more examples)
3. **How to handle escaping in strings?** (Define escape rules)
4. **Should we add type annotations?** (Probably not - infer from context)
5. **Error messages format?** (Line:column with helpful suggestions)

---

## 14. Next Steps

- [x] Create DSL syntax examples for all constructs
- [ ] Define formal grammar (EBNF or similar)
- [ ] Build prototype parser
- [ ] Create VS Code extension
- [ ] Write conversion tool (JSON ↔ DSL)
- [ ] Update Morph specs to include DSL syntax

---

## Appendix A: Quick Reference Card

```
# MORPH DSL QUICK REFERENCE

PAGE DEFINITION
page Name:
  title "Title"
  sources [source1, source2]
  fields: [...]
  actions: [...]

MENU DEFINITION  
menu Name:
  - Item -> page Target    [tags]
  - Item -> wizard Target  if condition

WIZARD DEFINITION
wizard Name:
  title "Title"
  step StepName "Step Title":
    fields: [...]
    validation: [...]
    actions: [...]
  onComplete calls Method():
    params: {...}
    success: navigate -> page Target
    failure: notify "Error"

FIELDS
- label id: value           [tags]
- text id: @bind:path       hint "text" [tags]
- number id: @bind:path     min N max M
- boolean id: @bind:path    default true/false
- select id from source (val, disp): @bind:path

ACTIONS
- rpc name calls Method():
    params: {...}
    onSuccess: [actions]
    onFailure: [actions]
- navigate name -> target   params {...}
- update state.path = value
- reset [path1, path2]
- notify "message"

SOURCES
rpc Name calls Method() cache path
state Name path path
static Name: [{...}, {...}]

BINDINGS
@path              # One-way read
@bind:path         # Two-way bind
from source (v,d)  # Dropdown options

TAGS
element [tag1, tag2]           # Inline
tags: - tag1 - tag2            # Block
tags from data.tags            # Dynamic

CONDITIONALS
if @condition          # Visibility guard
valid                  # Wizard step validation
confirm "message"      # Action confirmation

NAVIGATION (uses ->)
-> page Target        # Navigate to page
-> wizard Target      # Navigate to wizard
-> step StepName      # Navigate to wizard step
```

---

## Appendix B: Key Syntax Changes from v0.1

### Arrow Operator (`->`) Usage

**Version 0.2 Change:** The `->` operator is now used **only for navigation**.

| Before (v0.1) | After (v0.2) | Reason |
|---------------|--------------|--------|
| `rpc save -> SaveData()` | `rpc save calls SaveData()` | Clearer semantics |
| `source Tools -> GetTools()` | `source Tools calls GetTools()` | Consistent with actions |
| `- Dashboard -> page Dashboard` | `- Dashboard -> page Dashboard` | ✅ Unchanged (navigation) |
| `navigate home -> page Home` | `navigate home -> page Home` | ✅ Unchanged (navigation) |

### Rationale

Using `->` exclusively for navigation makes the DSL more intuitive:
- **Navigation** = direction/destination (`->`)
- **RPC calls** = function invocation (`calls Method()`)
- **State updates** = assignment (`=`)

This separation reduces ambiguity and improves readability.

---

**End of DSL Proposal Document (v0.2)**
