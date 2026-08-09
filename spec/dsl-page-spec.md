# Morph Page DSL Specification
# Version: 0.2
# Status: Draft
# Scope: Minimal page constructs (labels + entries + selects + buttons)

## 1. Overview
The Morph Page DSL defines simple user interface pages.
A page consists of:
- a page declaration
- an optional title
- a list of UI elements (label, entry, select, button)
- optional taglists at both page and element level

The DSL mirrors the structure of menu items:
```
- <ElementType> -> <ElementDefinition> [<TagList>]
```

---

## 2. Page Declaration

### 2.1 Syntax

```
page <Identifier>: [<TagList>]
    <PageBody>
```

Rules:
- `page` is a reserved keyword.
- `<Identifier>` must be a valid name (letters, digits, underscores; no spaces).
- A colon `:` starts the page block.
- Taglist is optional.
- Page body must be indented consistently.

Examples:
```
page UserSettings:
page UserSettings: [access-user, translationkey-usersettings]
```

---

## 3. Page Body

The page body may contain:
- an optional title declaration
- zero or more element items

### 3.1 Title (optional)

```
title "<Text>"
```

Rules:
- Title is optional.
- Title text must be a quoted string.

Example:
```
title "User Settings"
```

---

## 4. Elements

Elements follow the same structural pattern as menu items:

```
- <ElementType> -> <ElementDefinition> [<TagList>]
```

### 4.1 Element Types

Supported element types in version 0.2:

```
label
entry
select
button
```

---

## 5. Element Definitions

Element definitions use the unified `@kind:path` syntax.

### 5.1 Label Definition

```
@text:<Identifier>
```

Example:
```
- label -> @text:UserName [decoration-info]
```

---

### 5.2 Entry Definition (input field)

```
@bind:<Path>
```

Where `<Path>` is typically a state or model reference.

Example:
```
- entry -> @bind:state.UserName [type-text, required]
```

---

### 5.3 Select Definition (combobox)

```
@source:<Identifier>
```

Example:
```
- select -> @source:CountryNames [type-combobox]
```

---

### 5.4 Button Definition (action trigger)

```
@action:<Identifier>
```

Example:
```
- button -> @action:SaveUser [decoration-primary]
```

More examples:
```
- button -> @action:DeleteUser [danger, access-admin]
- button -> @action:RefreshData [icon-refresh]
```

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
- Whitespace around commas is allowed.
- Taglist must be enclosed in square brackets.

### 6.2 Tag Definition

```
<Tag> ::= <Identifier>
```

Examples:
- `[type-text]`
- `[required]`
- `[decoration-info]`
- `[decoration-primary, icon-save]`

### 6.3 Tag Semantics
Tags have no predefined meaning in version 0.2.
They are metadata for:
- permissions
- styling
- translation keys
- validation
- analytics
- future extensions

---

## 7. Constraints

### 7.1 Page Constraints
- Page names must be unique within a file.
- Element identifiers inside `@text`, `@bind`, `@source`, and `@action` must be valid names.
- Title may appear at most once.
- Only `label`, `entry`, `select`, and `button` elements may appear inside a page block.

### 7.2 Syntax Constraints
- Indentation must be consistent within a page block.
- No sections, layout constructs, tables, forms, or actions beyond `@action:<Identifier>` are allowed in version 0.2.

---

## 8. Grammar (EBNF)

```
PageFile      = (AppDecl | PageDecl)* ;

AppDecl       = "app", Identifier, Version ;

PageDecl      = "page", Identifier, ":", [ TagList ], PageBody ;

PageBody      = ( TitleDecl | ElementItem )* ;

TitleDecl     = "title", QuotedString ;

ElementItem   = "-", ElementType, "->", ElementDef, [ TagList ] ;

ElementType   = "label" | "entry" | "select" | "button" ;

ElementDef    = TextDef | BindDef | SourceDef | ActionDef ;

TextDef       = "@text:", Identifier ;
BindDef       = "@bind:", Path ;
SourceDef     = "@source:", Identifier ;
ActionDef     = "@action:", Identifier ;

Path          = Identifier, { ".", Identifier } ;

TagList       = "[", Tag, { ",", Tag }, "]" ;
Tag           = Identifier ;

Identifier    = Letter, { Letter | Digit | "_" } ;
QuotedString  = '"', { Character - '"' }, '"' ;
Version       = Identifier ;
```

---

## 9. Example (Pages Only)

```
app MorphDemo 1.0

page UserSettings: [access-user]
  title "User Settings"

  - label -> @text:UserName [decoration-info]
  - entry -> @bind:state.UserName [type-text, required]

  - label -> @text:Email
  - entry -> @bind:state.Email [type-text, required]

  - label -> @text:Country
  - select -> @source:CountryNames [type-combobox]

  - label -> @text:IsActive
  - entry -> @bind:state.IsActive [type-switch]

  - button -> @action:SaveUser [decoration-primary]
  - button -> @action:DeleteUser [danger, access-admin]
```

---

## 10. Version Notes
- Version 0.2 adds **buttons**.
- Tables/datagrids will be introduced in version 0.3 or later.
