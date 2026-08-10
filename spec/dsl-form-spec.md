# Morph Form DSL — Formal Specification  
Version 1.0  
Author: Dmitri  

---

## **1. Overview**

Morph is a declarative DSL for defining forms, data sources, and workflows.  
The language is intentionally minimal and pure:

- Forms define **structure only**  
- Sources define **data origin + semantics**  
- Bindings define **behavior + workflows**  

No functions, no lifecycle keywords, no UI constructs, no procedural syntax.

---

## **2. Grammar Summary**

### **2.1 Form Definition**

```
form <FormName>:
  <FieldName> [source:<SourceName>]
  <FieldName>
  ...
```

Rules:

- A form contains a **flat list of fields**.  
- Fields may optionally reference a **source**.  
- No functions, no lifecycle, no UI constructs.  
- No grouping, no sections, no relationships.  
- Field names are identifiers.  
- Form names are identifiers.

---

### **2.2 Source Definition**

```
source <SourceName> -> rpc <RpcMethod> [<taglist>]
```

Rules:

- A source has a **name**.  
- A source binds to an RPC method.  
- A source may have a **taglist**, always at the end.  
- Taglist is semantic only (e.g., `choices`, `table`, `cache`).  
- Taglist is comma‑separated inside brackets.  
- No UI constructs allowed in taglist.

Examples:

```
source Countries -> rpc LoadCountries [choices]
source Orders    -> rpc LoadOrders    [table, cache]
```

---

### **2.3 Submit Binding**

```
submit <FormName> -> rpc <RpcMethod>
```

Rules:

- Defines the RPC method called when the form is submitted.  
- No functions, no parentheses.  
- No lifecycle keywords.  
- No UI constructs.

Example:

```
submit UserForm -> rpc SaveUser
```

---

### **2.4 Workflow Binding (Wizard Behavior)**

```
submit <FormName> -> rpc <RpcMethod> -> <NextFormName>
```

Rules:

- Defines a transition from one form to another.  
- Enables multi‑form workflows (wizards).  
- No explicit wizard syntax — workflows emerge from bindings.  
- Supports linear, branching, and looping workflows.

Examples:

```
submit UserForm       -> rpc SaveUser       -> AddressForm
submit AddressForm    -> rpc SaveAddress    -> PaymentForm
submit PaymentForm    -> rpc ProcessPayment -> ConfirmationForm
```

Branching:

```
submit UserForm -> rpc SaveUser ->
  AdminForm   [when Role = "Admin"]
  ProfileForm [when Role = "User"]
```

Looping:

```
submit OrderForm -> rpc SaveOrder -> OrderForm
```

---

## **3. Taglist Specification**

Taglist is a semantic annotation on sources.

### **3.1 Syntax**

```
[<tag>, <tag>, ...]
```

### **3.2 Rules**

- Always appears at the end of the source binding line.  
- Tags are semantic only.  
- Tags must not express UI constructs.  
- Tags must not express procedural behavior.  
- Tags may express data semantics (e.g., `choices`, `table`, `tree`, `cache`, `state`).

Examples:

```
[choices]
[table]
[choices, cache]
[state]
```

---

## **4. Semantics**

### **4.1 Forms**

Forms define:

- field names  
- optional source references  

Forms do **not** define:

- submit behavior  
- reset behavior  
- load behavior  
- navigation  
- validation  
- UI widgets  
- relationships  
- grouping  
- lifecycle  

All behavior is defined externally via bindings.

---

### **4.2 Sources**

Sources define:

- where data comes from  
- how data is loaded  
- semantic meaning via tags  

Sources do **not** define:

- UI widgets  
- relationships  
- procedural logic  

---

### **4.3 Submit Bindings**

Submit bindings define:

- RPC method invoked on submit  
- optional next form (wizard step)  

Submit bindings do **not** define:

- UI widgets  
- lifecycle keywords  
- procedural logic  

---

### **4.4 Workflows**

Workflows emerge from:

- submit bindings  
- transitions  
- optional conditions  

Morph infers:

- wizard steps  
- navigation  
- progress indicators  
- validation order  
- back/next behavior  

No wizard syntax is required.

---

## **5. Complete Example**

### **5.1 Forms**

```
form UserForm:
  Name
  Email
  Role
  Country [source:Countries]

form AddressForm:
  Street
  City
  Zip
  Country [source:Countries]

form PaymentForm:
  CardNumber
  Expiry
  CVC

form ConfirmationForm:
  Message
```

### **5.2 Sources**

```
source Countries -> rpc LoadCountries [choices]
```

### **5.3 Workflow**

```
submit UserForm       -> rpc SaveUser       -> AddressForm
submit AddressForm    -> rpc SaveAddress    -> PaymentForm
submit PaymentForm    -> rpc ProcessPayment -> ConfirmationForm
```

This defines a 4‑step wizard.

---

## **6. Reserved Keywords**

- `form`
- `source`
- `submit`
- `rpc`
- `when`
- taglist brackets `[...]`

---

## **7. Design Principles**

- Purity  
- Declarativity  
- UI‑agnostic  
- Behavior‑agnostic  
- Minimal syntax  
- Explicit bindings  
- No functions  
- No lifecycle keywords  
- No procedural constructs  

---

## **8. Future Extensions (Optional)**

- Validation tags  
- Navigation tags  
- Static sources  
- Workflow metadata  
- Multi‑form validation rules  

---
