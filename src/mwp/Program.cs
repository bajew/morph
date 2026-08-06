using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MorphMWP
{
    // ----------------- Domain -----------------
    public class Tool
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool NeedsMaintenance { get; set; }
        public List<string> Tags { get; set; } = new();
    }

    // ----------------- RPC Layer -----------------
    public interface IToolService
    {
        List<Tool> GetTools();
        void AddTool(string name);
        void DeleteTool(int id);
        void CheckMaintenance(int id);
        List<Tool> GetArchive();
    }

    public class FakeToolService : IToolService
    {
        private readonly List<Tool> _tools = new();
        private readonly List<Tool> _archive = new();
        private int _idCounter = 1;

        public List<Tool> GetTools() => _tools;

        public void AddTool(string name)
        {
            _tools.Add(new Tool
            {
                Id = _idCounter++,
                Name = name,
                NeedsMaintenance = false
            });
        }

        public void DeleteTool(int id)
        {
            var tool = _tools.FirstOrDefault(t => t.Id == id);
            if (tool != null)
            {
                _tools.Remove(tool);
                tool.Tags.Add("archived");
                _archive.Add(tool);
            }
        }

        public void CheckMaintenance(int id)
        {
            var tool = _tools.FirstOrDefault(t => t.Id == id);
            if (tool != null)
            {
                tool.NeedsMaintenance = !tool.NeedsMaintenance;
                tool.Tags.Add(tool.NeedsMaintenance ? "maintenanceDue" : "maintenanceOk");
            }
        }

        public List<Tool> GetArchive() => _archive;
    }

    // ----------------- Metadata Models -----------------
    public class AppMetadata
    {
        public string StartMenu { get; set; }
    }

    public class MenuMetadata
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public List<MenuItemMetadata> Items { get; set; } = new();
    }

    public class MenuItemMetadata
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Navigate { get; set; } // menu or page id
        public List<string> Tags { get; set; } = new();
        public string VisibleWhen { get; set; }
    }

    public class PageMetadata
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public List<string> Sources { get; set; } = new();
        public List<FieldMetadata> Fields { get; set; } = new();
        public List<ActionMetadata> Actions { get; set; } = new();
    }

    public class FieldMetadata
    {
        public string Id { get; set; }
        public string Type { get; set; } // "label", "text"
        public string Value { get; set; }
        public string Hint { get; set; }
        public List<string> Tags { get; set; } = new();
    }

    public class ActionMetadata
    {
        public string Id { get; set; }
        public string Type { get; set; } // "rpc"
        public string Title { get; set; }
        public string Method { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new();
    }

    public class SourceMetadata
    {
        public string Id { get; set; }
        public string Type { get; set; } // "rpc", "state", or "static"
        public SourceParameters Parameters { get; set; }
    }

    public class SourceParameters
    {
        public string Method { get; set; }
    }

    // ----------------- Metadata Loader -----------------
    public static class MetadataLoader
    {
        public static T LoadSingle<T>(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public static Dictionary<string, T> LoadMany<T>(string folder, string prefix)
        {
            var dict = new Dictionary<string, T>();
            foreach (var file in Directory.GetFiles(folder, $"{prefix}*.json"))
            {
                var json = File.ReadAllText(file);
                var obj = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var idProp = typeof(T).GetProperty("Id");
                var id = (string)idProp.GetValue(obj);
                dict[id] = obj;
            }
            return dict;
        }
    }

    // ----------------- Engine -----------------
    class Program
    {
        static Dictionary<string, MenuMetadata> Menus;
        static Dictionary<string, PageMetadata> Pages;
        static Dictionary<string, SourceMetadata> Sources;
        static AppMetadata App;
        static IToolService ToolService = new FakeToolService();

        static void Main()
        {
            var metadataFolder = Path.Combine(Directory.GetCurrentDirectory(), "metadata");

            App = MetadataLoader.LoadSingle<AppMetadata>(Path.Combine(metadataFolder, "app.json"));
            Menus = MetadataLoader.LoadMany<MenuMetadata>(metadataFolder, "menu.");
            Pages = MetadataLoader.LoadMany<PageMetadata>(metadataFolder, "page.");
            Sources = MetadataLoader.LoadMany<SourceMetadata>(metadataFolder, "source.");

            var currentMenuId = App.StartMenu;

            while (true)
            {
                RenderMenu(currentMenuId);
            }
        }

        static void RenderMenu(string menuId)
        {
            Console.Clear();

            if (!Menus.TryGetValue(menuId, out var menu))
            {
                Console.WriteLine($"Menu '{menuId}' not found.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"=== {menu.Title} ===\n");

            for (int i = 0; i < menu.Items.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {menu.Items[i].Title}");
            }

            Console.WriteLine("\n0. Back to Main Menu");
            Console.Write("\nSelect: ");

            var input = Console.ReadLine();
            if (!int.TryParse(input, out int choice))
            {
                return;
            }

            if (choice == 0)
            {
                // always go back to main
                if (menuId != App.StartMenu)
                    RenderMenu(App.StartMenu);
                return;
            }

            if (choice < 1 || choice > menu.Items.Count)
            {
                return;
            }

            var item = menu.Items[choice - 1];
            if (Menus.ContainsKey(item.Navigate))
            {
                RenderMenu(item.Navigate);
            }
            else if (Pages.ContainsKey(item.Navigate))
            {
                RenderPage(item.Navigate);
            }
        }

        static void RenderPage(string pageId)
        {
            Console.Clear();

            if (!Pages.TryGetValue(pageId, out var page))
            {
                Console.WriteLine($"Page '{pageId}' not found.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"=== {page.Title} ===\n");

            // Load sources and display tools
            foreach (var sourceId in page.Sources)
            {
                if (!Sources.TryGetValue(sourceId, out var source))
                    continue;

                var data = ExecuteSource(source);
                if (data != null && data.Count > 0)
                {
                    foreach (var tool in data)
                    {
                        var tags = (tool.Tags != null && tool.Tags.Count > 0)
                            ? $" ({string.Join(", ", tool.Tags)})"
                            : "";
                        Console.WriteLine($"[{tool.Id}] {tool.Name}{tags}");
                    }
                    Console.WriteLine();
                }
            }

            // Field runtime: labels + text prompts
            var pageState = new Dictionary<string, string>();

            foreach (var field in page.Fields)
            {
                if (field.Type == "label")
                {
                    Console.WriteLine(ResolveValue(field.Value, pageState));
                }
                else if (field.Type == "text")
                {
                    while (true)
                    {
                        Console.Write($"{ResolveValue(field.Hint, pageState) ?? field.Id}: ");
                        var value = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            SetTwoWayValue(field.Value, value, pageState);
                            break;
                        }
                        Console.WriteLine("Invalid input, please try again.");
                    }
                }
            }

            // Execute first action (submit) if present
            var submit = page.Actions.FirstOrDefault();
            if (submit != null)
            {
                ExecuteAction(submit, pageState);
                Console.WriteLine("\nAction executed. Press ENTER to return to Main Menu...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("\nPress ENTER to return to Main Menu...");
                Console.ReadLine();
            }

            RenderMenu(App.StartMenu);
        }

        static List<Tool> ExecuteSource(SourceMetadata source)
        {
            return source.Parameters?.Method switch
            {
                "GetTools" => ToolService.GetTools(),
                "GetArchive" => ToolService.GetArchive(),
                _ => new List<Tool>()
            };
        }

        static void ExecuteAction(ActionMetadata action, Dictionary<string, string> pageState)
        {
            var boundParams = new Dictionary<string, string>();
            foreach (var kv in action.Parameters)
            {
                boundParams[kv.Key] = ResolveValue(kv.Value, pageState);
            }

            switch (action.Method)
            {
                case "AddTool":
                    if (boundParams.TryGetValue("name", out var name))
                        ToolService.AddTool(name);
                    break;

                case "DeleteTool":
                    if (boundParams.TryGetValue("id", out var idStr) && int.TryParse(idStr, out int id))
                        ToolService.DeleteTool(id);
                    break;

                case "CheckMaintenance":
                    if (boundParams.TryGetValue("id", out var midStr) && int.TryParse(midStr, out int mid))
                        ToolService.CheckMaintenance(mid);
                    break;
            }
        }

        static string ResolveValue(string value, Dictionary<string, string> pageState)
        {
            if (string.IsNullOrEmpty(value) || !value.StartsWith("@"))
                return value;

            var path = value[1..];
            return path.StartsWith("state.") && pageState.TryGetValue(path, out var resolved)
                ? resolved
                : string.Empty;
        }

        static void SetTwoWayValue(string binding, string value, Dictionary<string, string> pageState)
        {
            const string prefix = "@bind:state.";
            if (binding?.StartsWith(prefix) == true)
                pageState[$"state.{binding[prefix.Length..]}"] = value;
        }
    }
}
