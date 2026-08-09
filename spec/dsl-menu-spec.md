# Morph Menu DSL Specification
# Version: 0.2
# Status: Draft
# Scope: Menu constructs only

## 1. Overview
The Morph Menu DSL defines navigational structures within a Morph application.
A menu consists of a named block containing a list of menu items.
Each menu item has:
- a label
- a navigation target
- an optional taglist

Menus may reference pages, wizards, or other menus.

This specification describes only menu constructs.

---

## 2. Top-Level Constructs

### 2.1 Menu Declaration
A menu is declared using the following syntax:

```
menu <Identifier>:
    <MenuItem>*
```

Rules:
- `menu` is a reserved keyword.
- `<Identifier>` must be a valid name (letters, digits, underscores; no spaces).
- A colon `:` starts the menu item block.
- Menu items must be indented consistently.

---

## 3. Menu Items

### 3.1 Menu Item Syntax

```
- <Label> -> <Target> [<TagList>]
```

Rules:
- A menu item begins with a hyphen `-`.
- `<Label>` is either an identifier or a quoted string.
- `->` indicates navigation.
- `<Target>` specifies the navigation destination.
- `[<TagList>]` is optional.

---

## 4. Labels

### 4.1 Label Definition

```
<Label> ::= <Identifier> | <QuotedString>
```

Rules:
- Identifiers: `[A-Za-z_][A-Za-z0-9_]*`
- Quoted strings: `"Any text except unescaped quotes"`

Examples:
- `Dashboard`
- `"User Management"`

---

## 5. Navigation Targets

### 5.1 Allowed Target Types

```
<Target> ::= page <Identifier>
           | wizard <Identifier>
           | menu <Identifier>
```

Rules:
- `page`, `wizard`, and `menu` are reserved keywords.
- `<Identifier>` must be a valid name.
- No other target types are allowed in version 0.2.

---

## 6. Taglists

### 6.1 Taglist Syntax

```
[ tag1, tag2, tag3 ]
```

Rules:
- Taglist is optional.
- Tags are comma-separated.
- Tags must be identifiers.
- Whitespace around commas is allowed but not required.
- Taglist must be enclosed in square brackets.

### 6.2 Tag Definition

```
<Tag> ::= <Identifier>
```

Examples:
- `[icon-tools]`
- `[icon-tools, access-admin]`
- `[access-user, access-admin, highlight]`

### 6.3 Tag Semantics
Tags have no predefined meaning in version 0.2.
They are metadata for:
- UI rendering
- permissions
- styling
- grouping
- analytics
- future extensions

The DSL does not interpret tag semantics at this stage.

---

## 7. Constraints

### 7.1 Menu Constraints
- Menu names must be unique within a file.
- Menu item labels must be unique within a menu.
- Navigation targets must reference existing names (validated at a later stage).
- Only menu items may appear inside a menu block.

### 7.2 Syntax Constraints
- Indentation must be consistent within a menu block.
- No additional constructs (actions, links, sources) are allowed in version 0.2.

---

## 8. Grammar (EBNF)

```
MenuFile      = (AppDecl | MenuDecl)* ;

AppDecl       = "app", Identifier, Version ;

MenuDecl      = "menu", Identifier, ":", MenuItem* ;

MenuItem      = "-", Label, "->", Target, [ TagList ] ;

Label         = Identifier | QuotedString ;

Target        = PageTarget | WizardTarget | MenuTarget ;

PageTarget    = "page", Identifier ;
WizardTarget  = "wizard", Identifier ;
MenuTarget    = "menu", Identifier ;

TagList       = "[", Tag, { ",", Tag }, "]" ;
Tag           = Identifier ;

Identifier    = Letter, { Letter | Digit | "_" } ;
QuotedString  = '"', { Character - '"' }, '"' ;
Version       = Identifier ;
```

---

## 9. Example (Menus Only, With Taglists)

```
app MorphDemo 1.0

menu MainMenu:
  - Dashboard -> page Dashboard [icon-dashboard]
  - Tools     -> menu ToolMenu [icon-tools, access-admin, access-user]
  - Setup     -> wizard SetupWizard [icon-setup]

menu ToolMenu:
  - Diagnostics -> page Diagnostics [icon-diagnostics]
  - Metrics     -> page Metrics [icon-metrics, highlight]
  - Back        -> menu MainMenu [icon-back]
```

---

## 10. Version Notes
- Version 0.2 adds taglists to menu items.
- Future versions may introduce:
  - tag validation rules
  - tag namespaces
  - tag-based visibility
  - tag-based styling
  - dynamic tags
