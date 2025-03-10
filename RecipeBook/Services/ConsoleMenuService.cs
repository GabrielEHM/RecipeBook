﻿using ConsoleTables;
using Humanizer;
using RecipeBook.Models;
using System.Diagnostics.Metrics;
using System.Diagnostics;

namespace RecipeBook.Services
{
    class ConsoleMenuService
    {
        public static bool ShowMenu(string title, CommandList options, string selectionMessage = "What do you wish to do? ", bool clear = true)
        {
            if (!options.Any(cmd => cmd.Name.Contains("Exit", StringComparison.OrdinalIgnoreCase)) && !options.Any(cmd => cmd.Name.Contains("Back", StringComparison.OrdinalIgnoreCase)))
                options.Add("Go Back", (_) => { return false; }, "back");

            if (clear) Console.Clear();
            Console.WriteLine($"=== {title} ===\n");
            int index = 1;
            foreach (var option in options)
            {
                Console.WriteLine($"{option.Trigger}: {option.Name}");
                index++;
            }

            Console.WriteLine();
            if (!selectionMessage.EndsWith(' '))
            {
                selectionMessage += "";
            }
            Console.Write(selectionMessage);
            string? input = Console.ReadLine();
            var verb = input?.Split(' ')[0];
            var args = input?.Split(' ').Skip(1).ToArray() ?? [];
            return options.Run(verb, args);
        }

        public static bool ListEntities<T>(Paged<T> page, Service<T> service) where T : IPageable<T>
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
                    { "Update ingredient <id>", (args) => service.Add(id: args[0]) , "update"},
                    { "Detail <id>", (args) => service.GetById(id: args[0]), "detail" },
                    { "Delete <id1> [<id2> <...> <idn>]", (args) => service.Delete(ids: args), "delete" }
                };

            if (page.Pagination.PageTotal > 1)
                listOptions.Add(new Command("Go to Page <page_number> [<page_size>]", (args) =>
                {
                    if (args.Length == 0) return CommandList.InvalidChoice("You need to provide a page number to jump to");
                    var pageNumber = int.Parse(args[0]);
                    var pageSize = args.Length > 1 ? int.Parse(args[1]) : page.Pagination.PageSize;
                    if (pageNumber < 1 || pageNumber > page.Pagination.PageTotal) return CommandList.InvalidChoice("The page number is out of range");
                    return service.ListAll(page: pageNumber, pageSize);
                }, "goto"));
            if (page.Pagination.Page < page.Pagination.PageTotal)
                listOptions.Add(new Command("Next Page", (args) => { return service.ListAll(page: page.Pagination.Page + 1, pageSize: page.Pagination.PageSize); }, "next"));
            if (page.Pagination.Page > 1)
                listOptions.Add(new Command("Previous Page", (args) => { return service.ListAll(page: page.Pagination.Page - 1, pageSize: page.Pagination.PageSize); }, "prev"));

            return ShowMenu("Options", listOptions, "Enter your choice: ", false);
        }

        public static bool Confirm(string message)
        {
            Console.WriteLine(message);
            Console.Write("Are you sure? (Y/N): ");
            var response = Console.ReadLine();
            return response is not null && (response.Equals("y", StringComparison.OrdinalIgnoreCase) || response.Equals("yes", StringComparison.OrdinalIgnoreCase));
        }

        private static void ShowMessage(string message, bool error = false)
        {
            if (!message.EndsWith('.'))
            {
                message += ".";
            }
            Console.WriteLine($"{(error ? "Error: " : "")}{message}");
        }

        public static void ShowMessage(params string[] messages)
        {
            foreach (var message in messages)
            {
                ShowMessage(message, false);
            }
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();
        }

        public static void ShowError(params string[] messages)
        {
            foreach (var message in messages)
            {
                ShowMessage(message, true);
            }
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();
        }
    }
}
