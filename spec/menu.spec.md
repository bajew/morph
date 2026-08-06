Menu Specification

Version: 1.0Status: Draft (Migrated into Morph Creations)Purpose: Defines the structure, semantics, and processing rules for application menus in the Morph meta‑language. Menus provide navigational and contextual entry points into pages, wizards, and actions.

1. Overview

Menus in Morph are declarative metadata structures that describe navigational elements. They:

define menu items

reference pages, wizards, or actions

support semantic tags

support visibility rules

support grouping

Menus do not define UI styling or layout. The UI runtime interprets menu metadata and renders it according to platform conventions.

2. Menu Structure

A menu is defined as:

{
  "id": "mainMenu",
  "type": "menu",
  "items": [ ... ],
  "tags": [ ... ]
}

Fields

id — unique identifier

type — always menu

items — array of menu items

tags — semantic tags applied to the menu

3. Menu Item Structure

Menu items represent navigational entries.

{
  "id": "tools",
  "title": "Tools",
  "navigate": "toolsPage",
  "tags": ["highlight"]
}

Fields

id — unique identifier

title — display text

navigate — page or wizard identifier

action — optional action instead of navigation

tags — semantic tags

visibleWhen — optional binding controlling visibility

4. Navigation

Menu items may navigate to:

pages

wizards

Example:

"navigate": "inventoryPage"

5. Actions

Menu items may trigger actions instead of navigation:

"action": "refreshInventory"

Actions follow the Morph action model.

6. Visibility Rules

Menu items may be conditionally visible:

"visibleWhen": "state.user.isAdmin"

Visibility uses the binding system defined in bindings.spec.md.

7. Tags

Menu items and menus may define semantic tags:

"tags": ["bold", "maintenanceDue"]

Tags follow the unified decorator model defined in decorators.spec.md.

8. Grouping

Menus may define groups:

{
  "id": "adminGroup",
  "title": "Administration",
  "items": [ ... ]
}

Groups are purely semantic; UI runtime decides how to render them.

9. Example Menu

{
  "id": "mainMenu",
  "type": "menu",
  "items": [
    {
      "id": "tools",
      "title": "Tools",
      "navigate": "toolsPage",
      "tags": ["bold"]
    },
    {
      "id": "maintenance",
      "title": "Maintenance",
      "navigate": "maintenanceWizard",
      "tags": ["maintenanceDue"]
    }
  ]
}

10. Future Extensions

Potential future additions:

menu-level actions

dynamic menu generation

role-based menu filtering

multi-level nested menus

End of Specification