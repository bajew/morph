# Morph Page DSL Specification
# Version: 0.3
# Status: Draft
# Scope: Pages + labels + entries + selects + buttons + tables + rowactions + separator

## 1. Overview
The Morph Page DSL defines declarative user interface pages.
A page consists of:
- a page declaration
- an optional title
- a list of UI elements (label, entry, select, button, table)
- optional taglists at both page and element level

Tables are defined inside pages using a **flat structure** and a **mandatory separator** (`---`) to avoid nesting ambiguity.

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
- `<Identifier>` must be a valid name.
- Taglist is optional.
- Page body must be indented consistently.

---

## 3. Page Body

May contain:
- optional title
- element items
- table definitions

### 3.1 Title

```
title "<Text>"
```

---

## 4. Elements

Elements follow:

```
- <ElementType> -> @<kind>:<path> [<TagList>]
```

### 4.1 Element Types

```
label
entry
select
button
table
```

---

## 5. Element Definitions

### 5.1 Label

```
@text:<Identifier>
```

### 5.2 Entry (input)

```
@bind:<Path>
```

### 5.3 Select (combobox)

```
@source:<Identifier>
```

### 5.4 Button (page-level action)

```
@action:<Identifier>
```

---

## 6. Table Definition

Tables are defined **inside pages**, using a flat structure and a mandatory separator.

### 6.1 Syntax

```
table <Identifier> @source:<Source>:
    <TableBody>
    ---
```

### 6.2 Table Body

A table body consists of **flat element items**:

```
- <ColumnLabel> -> @bind:<Path> [tags]
- <ColumnLabel> -> @source:<Identifier> [tags]
- <ActionLabel> -> @rowaction:<Identifier> [tags]
```

### 6.3 Column Definitions

Columns use the same menu-style syntax:

```
- Name -> @bind:UserName
- Email -> @bind:Email
- Role -> @bind:Role
```

### 6.4 Row Actions

Row actions are defined **at the same level as columns**, using:

```
@rowaction:<Identifier>
```

Example:

```
- Edit -> @rowaction:EditUser
- Delete -> @rowaction:DeleteUser [danger]
```

### 6.5 Row Action Semantics

- Row actions always operate on the **selected row**.
- The DSL does not specify how the UI renders them.
- The UI may choose:
  - inline row buttons
  - toolbar buttons
  - context menus
  - swipe actions
  - long-press menus
- The DSL stays UI-agnostic.

### 6.6 Separator

The separator `---` **must** appear after the last table item.

It marks the end of the table definition and prevents nesting ambiguity.

Example:

```
table Users @source:Users:
  - Id -> @bind:Id [hidden]
  - Name -> @bind:UserName
  - Email -> @bind:Email
  - Role -> @bind:Role

  - Edit -> @rowaction:EditUser
  - Delete -> @rowaction:DeleteUser [danger]
  ---
```

---

## 7. Taglists

```
[ tag1, tag2, tag3 ]
```

Tags are metadata only.

---

## 8. Constraints

- No nested blocks inside tables beyond the table declaration.
- The separator `---` is mandatory after table body.
- No UI-specific semantics (tap, click, command, gesture).
- Row actions must use `@rowaction:` and must not appear inside columns.
- Page-level actions must use `@action:`.

---

## 9. Grammar (EBNF)

```
PageFile      = (AppDecl | PageDecl)* ;

AppDecl       = "app", Identifier, Version ;

PageDecl      = "page", Identifier, ":", [ TagList ], PageBody ;

PageBody      = ( TitleDecl | ElementItem | TableDecl )* ;

TitleDecl     = "title", QuotedString ;

ElementItem   = "-", ElementType, "->", ElementDef, [ TagList ] ;

ElementType   = "label" | "entry" | "select" | "button" ;

ElementDef    = TextDef | BindDef | SourceDef | ActionDef ;

TableDecl     = "table", Identifier, "@source:", Identifier, ":", TableBody, Separator ;

TableBody     = ( TableItem )* ;

TableItem     = "-", TableLabel, "->", TableDef, [ TagList ] ;

TableLabel    = Identifier ;

TableDef      = BindDef | SourceDef | RowActionDef ;

RowActionDef  = "@rowaction:", Identifier ;

Separator     = "---" ;

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

## 10. Example

```
app MorphDemo 1.0

page UserManagement:
  title "User Management"

  table Users @source:Users:
    - Id -> @bind:Id [hidden]
    - Name -> @bind:UserName
    - Email -> @bind:Email
    - Role -> @bind:Role

    - Edit -> @rowaction:EditUser
    - Delete -> @rowaction:DeleteUser [danger]
    ---

  - button -> @action:AddUser [decoration-primary]
  - button -> @action:DeleteSelected [danger]
```

---

## 11. Version Notes
- Version 0.3 adds:
  - tables inside pages
  - flat column + rowaction definitions
  - mandatory `---` separator
- No UI-specific semantics.
- No nested blocks beyond the table declaration.
