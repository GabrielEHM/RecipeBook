using ConsoleTables;
using RecipeBook.Models;

namespace RecipeBook.Services
{
    class ConsoleMenuService
    {
        public bool ShowMenu(string title, CommandList options, string selectionMessage = "Enter your choice: ", bool clear = true)
        {
            if (!options.Any(cmd => cmd.Name.Contains("Exit", StringComparison.OrdinalIgnoreCase)) && !options.Any(cmd => cmd.Name.Contains("Back", StringComparison.OrdinalIgnoreCase)))
                options.Add("Go Back", (_) => { return false; });

            if (clear) Console.Clear();
            Console.WriteLine($"=== {title} ===\n");
            int index = 1;
            foreach (var option in options)
            {
                Console.WriteLine($"{option.Trigger}: {option.Name}");
                index++;
            }

            Console.Write(selectionMessage);
            string? input = Console.ReadLine();
            var verb = input?.Split(" ")[0];
            var args = input?.Split(" ").Skip(1).ToArray() ?? [];
            return options.Run(verb, args);
        }

        public bool ListEntities<T>(Paged<T> page, Service<T> service) where T : IPageable<T>
        {

            Console.Clear();
            Console.WriteLine($"=== {typeof(T).Name} List ===\n");
            string[] headers = T.GetTableHeaders();
            var table = new ConsoleTable(headers);

            foreach (T entity in page.Entities)
            {
                table.AddRow(entity.ToTableRow());
            }
            Console.WriteLine(table.ToString());
            Console.WriteLine($"\nPage {page.Pagination.Page} of {page.Pagination.PageTotal}\n");
            var listOptions = new CommandList
                {
                    { "Add a new ingredient", (_) => service.Add() , "add"},
                    { "Update ingredient <id>", (args) => service.Add(id: args[0]) , "add"},
                    { "Detail <id>", (args) => service.GetById(id: args[0]), "detail" },
                    { "Delete <id1> [<id2> <...> <idn>]", (args) => service.Delete(ids: args), "delete" }
                };

            if (page.Pagination.PageTotal > 1)
                listOptions.Add(new Command("Go to Page <id> [<page_size>]", (args) => { return service.ListAll(page: int.Parse(args[0]), pageSize: args.Length > 1 ? int.Parse(args[1]) : page.Pagination.PageSize); }, "goto"));
            if (page.Pagination.Page < page.Pagination.PageTotal)
                listOptions.Add(new Command("Next Page", (args) => { return service.ListAll(page: page.Pagination.Page + 1, pageSize: page.Pagination.PageSize); }, "next"));
            if (page.Pagination.Page > 1)
                listOptions.Add(new Command("Previous Page", (args) => { return service.ListAll(page: page.Pagination.Page - 1, pageSize: page.Pagination.PageSize); }, "prev"));

            return ShowMenu("Options", listOptions, "Enter your choice: ", false);
        }
    }
}
