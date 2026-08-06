Dynamic State Container Specification

Version: 1.1**Status: Draft (Extended with full implementation example)Scope: Defines the dynamic, hierarchical, observable state engine used by pages, wizards, and application-level state in the meta‑language runtime.

1. Overview

The Dynamic State Container is a flexible, schema‑free state engine designed for metadata‑driven applications. It supports:

dynamic page models

hierarchical paths (state.material.locked)

MVVM change propagation

wizard and application state layering

computed values

state.update and state.reset actions

It behaves like a dynamic JSON object with MVVM notifications.

2. Core Concepts

2.1 Dynamic Storage

State is stored in nested dictionaries:

Dictionary<string, object?>

This allows arbitrary schemas defined at runtime.

2.2 Hierarchical Paths

Paths use dot notation:

"material.locked"
"step1.amount"
"user.id"

Each segment represents a nested dictionary.

2.3 Observable State

The container implements:

INotifyPropertyChanged

deep change propagation

state change events for computed values

3. API Surface

3.1 Get

Retrieve a value by hierarchical path.

object? Get(string path)
T? Get<T>(string path)

3.2 Set

Assign a value by hierarchical path.

void Set(string path, object? value)
void Set<T>(string path, T value)

Automatically creates missing nested dictionaries. Raises PropertyChanged(path).

3.3 Reset

Reset a value or remove a path.

void Reset(string path)

If defaults exist, resets to default. Otherwise removes the path.

3.4 Defaults

Initialize default values for reset operations.

void InitializeDefaults(Dictionary<string, object?> defaults)

3.5 Change Events

event EventHandler<StateChangedEventArgs> StateChanged;

Used by computed values and wizard transitions.

4. Internal Mechanics

4.1 ResolvePath

Traverses nested dictionaries to retrieve a value. Returns null if any segment is missing.

4.2 SetInternal

Creates nested dictionaries as needed. Assigns the final value.

4.3 RemovePath

Removes the final segment of a path. Does not remove parent dictionaries.

5. Full Implementation Example

Below is a complete example implementation of the Dynamic State Container.

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
            if (current is Dictionary<string, object?> dict)
            {
                if (!dict.TryGetValue(part, out var next))
                    return null;

                current = next;
            }
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
            {
                dict = nested;
            }
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

6. Integration

6.1 Page State

Each page receives its own DynamicStateContainer.

6.2 Wizard State

Wizard state uses layered containers:

global wizard state

per‑step state containers

computed wizard values

6.3 Application State

App state persists across pages and sessions.

6.4 Bindings

Bindings reference state using:

state.<path>
wizard.<path>
app.<path>

The binding resolver delegates to the appropriate container.

7. Example Usage

State.Set("material.locked", true);
Wizard.Steps["step1"].Set("amount", 10);
App.Set("user.id", 42);

End of Specification