using ConsoleTables;
using RecipeBook.Models;

namespace RecipeBook.Services
{
    class ConsoleMenuService
    {
        public void ShowMenu(string title, CommandList options, string selectionMessage = "Enter your choice: ", bool clear = true)
        {
            bool running = true;
            if (!options.Any(cmd => cmd.Name.Contains("Exit", StringComparison.OrdinalIgnoreCase)) && !options.Any(cmd => cmd.Name.Contains("Back", StringComparison.OrdinalIgnoreCase)))
                options.Add("Go Back", (_) => { return false; });

            while (running)
            {
                if (clear) Console.Clear();
                Console.WriteLine($"=== {title} ===\n");
                int index = 1;
                foreach (var option in options)
                {
                    Console.WriteLine($"{option.Trigger}. {option.Name}");
                    index++;
                }

                Console.Write(selectionMessage);
                string? input = Console.ReadLine();
                var verb = input?.Split(" ")[0];
                var args = input?.Split(" ").Skip(1).ToArray() ?? [];
                running = options.Run(input, args);
            }
        }

        public async Task ListEntitiesAsync<T>(IAsyncEnumerable<Paged<T>> pages, CommandList listOptions) where T : IPageable<T>
        {
            Console.Clear();
            Console.WriteLine($"=== {typeof(T).Name} List ===\n");
            string[] headers = T.GetTableHeaders();
            var table = new ConsoleTable(headers);

            await foreach (var page in pages)
            {
                foreach (T entity in page.Entities)
                {
                    table.AddRow(entity.ToTableRow());
                }
                Console.WriteLine(table.ToString());
                Console.WriteLine($"\nPage {page.Pagination.Page} of {page.Pagination.PageTotal}\n");
                ShowMenu("Options", listOptions, "Enter your choice: ", false);
            }
        }
    }
}
