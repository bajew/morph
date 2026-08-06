page.spec.md

Version: 1.1 Status: Updated Purpose: Defines the structure, semantics, and processing rules for Morph pages. Pages are declarative UI containers composed of fields, sources, and actions.

1. Overview

A page in Morph is a declarative metadata object that describes:

what fields appear on the page

how those fields behave

what backend sources the page loads

what actions the page can execute

Pages do not define layout, styling, or rendering. The UI runtime interprets page metadata and renders it according to platform conventions.

2. Page Structure

A page is defined as:

json

{
  "id": "page.example",
  "type": "page",
  "title": "Example Page",
  "sources": [ ... ],
  "fields": [ ... ],
  "actions": [ ... ]
}


Fields

Declarative UI elements such as labels, text inputs, numbers, tables, etc.

Sources

Backend data providers that populate the page.

Actions

Operations triggered by user interaction (e.g., RPC calls).

3. Properties

Property

Required

Description

id

yes

Unique identifier for the page.

type

yes

Always "page".

title

yes

Human-readable title.

sources

no

Array of source identifiers.

fields

yes

Array of field definitions.

actions

no

Array of action definitions.

4. Fields

Fields define the UI elements of a page.

4.1 Field Structure

json

{
  "id": "fieldId",
  "type": "fieldType",
  "...typeSpecificProperties"
}


4.2 Common Field Properties

Property

Required

Description

id

yes

Unique within the page.

type

yes

Field type (label, text, etc.).

value

optional

Static or bound value.

hint

optional

Static or bound hint text (input guidance).

tags

optional

Semantic decorators.

4.3 Binding Syntax (@bind:)

Morph supports Blazor-inspired binding syntax:

Code

@bind:<binding-path>


Bindings may appear in any string property, including:

value

hint

future properties such as options, visibility, etc.

Binding Resolution Rules

If a property contains @bind:<path>, the engine resolves the binding.

The resolved value overrides any static value.

Bound fields may update automatically when state changes (future behavior).

Text fields may support two-way binding (future behavior).

Examples

Static label

json

{ "id": "lbl", "type": "label", "value": "Enter tool name:" }


Dynamic label

json

{ "id": "lbl", "type": "label", "value": "@bind:state.tool.name" }


Static text field

json

{ "id": "name", "type": "text", "hint": "Tool name" }


Dynamic hint

json

{ "id": "name", "type": "text", "hint": "@bind:state.tool.placeholder" }


Dynamic value (future two‑way binding)

json

{ "id": "name", "type": "text", "value": "@bind:state.tool.name" }


4.4 Field Types

4.4.1 label

Displays static or bound text.

Properties:

value — required for display

Example:

json

{
  "id": "lbl",
  "type": "label",
  "value": "Enter tool name:"
}


Dynamic example:

json

{
  "id": "lbl",
  "type": "label",
  "value": "@bind:state.tool.name"
}


4.4.2 text

Accepts user input.

Properties:

hint — optional guidance text

value — optional default or bound value

Example:

json

{
  "id": "name",
  "type": "text",
  "hint": "Tool name"
}


Dynamic examples:

json

{
  "id": "name",
  "type": "text",
  "hint": "@bind:state.tool.placeholder"
}


json

{
  "id": "name",
  "type": "text",
  "value": "@bind:state.tool.name"
}


5. Sources

Pages may reference multiple sources.

json

"sources": ["source.tools", "source.archive"]


Each source is resolved before fields are rendered. The UI runtime decides how to display source data.

6. Actions

Actions define operations triggered by user interaction.

6.1 Action Structure

json

{
  "id": "submit",
  "type": "rpc",
  "rpcMethod": "AddTool",
  "parameters": {
    "name": "@bind:field.name"
  }
}


6.2 Action Types

Type

Description

rpc

Calls a backend RPC method.

6.3 Parameter Binding

Parameters may use @bind: syntax to reference:

field values

state

computed values

source data

Example:

json

"parameters": {
  "id": "@bind:field.toolId"
}


7. Example Page

json

{
  "id": "page.addTool",
  "type": "page",
  "title": "Add Tool",
  "sources": [],
  "fields": [
    {
      "id": "lbl",
      "type": "label",
      "value": "Enter tool name:"
    },
    {
      "id": "name",
      "type": "text",
      "hint": "Tool name"
    }
  ],
  "actions": [
    {
      "id": "submit",
      "type": "rpc",
      "rpcMethod": "AddTool",
      "parameters": {
        "name": "@bind:field.name"
      }
    }
  ]
}


8. Future Extensions

additional field types (number, checkbox, dropdown, table)

computed values

conditional visibility

validation rules

two-way binding

dynamic navigation

wizard pages

Your fully updated page.spec.md is now recreated and ready in Creations.