using System;
using System.Linq;
using ConsoleTables;
using ProjectA.Services.Exceptions;

namespace ProjectA.Services
{
    public class CommandService
    {
        private readonly RepositoryService _repositoryService;

        public CommandService(RepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        private static void DisplayInformation()
        {
            Console.WriteLine("Choose an option using the following pattern:");
            Console.WriteLine("\tlist\t\t\t\t\t- List all the documents");
            Console.WriteLine(
                "\tadd [entity id] ([snapshot folder id])\t- Add a document with out without snapshot folder id set");
            Console.WriteLine("\tdelete [entity id]\t\t\t- Delete a document");
            Console.WriteLine("Or enter QUIT to exit...");
        }

        public void Run()
        {
            DisplayInformation();
            var input = Console.ReadLine();

            while (input?.ToLower() != "quit")
            {
                // buffer input
                var command = input;
                input = string.Empty;

                if (!string.IsNullOrEmpty(command))
                    try
                    {
                        if (command.ToLower().StartsWith("list"))
                        {
                        }
                        else if (command.ToLower().StartsWith("add"))
                        {
                            var args = command!.Split(" ");
                            if (args.Length is > 3 or < 2)
                                throw new InvalidCommandException(command);

                            _repositoryService.AddDocument(int.Parse(args[1]),
                                args.Length == 2 ? 0 : int.Parse(args[2]));
                        }
                        else if (command.ToLower().StartsWith("delete"))
                        {
                            var args = command!.Split(" ");
                            if (args.Length is not 2) throw new InvalidCommandException(command);

                            _repositoryService.DeleteDocument(int.Parse(args[1]));
                        }
                        else
                        {
                            throw new InvalidCommandException(command);
                        }

                        PrintDocuments();
                    }
                    catch (RepositoryException repositoryException)
                    {
                        Console.WriteLine(repositoryException.Message);
                    }
                    catch (InvalidCommandException commandException)
                    {
                        Console.WriteLine($"Unknown command pattern: {commandException.Message}");
                    }
                
                DisplayInformation();
                input = Console.ReadLine();
            }
        }

        private void PrintDocuments()
        {
            // list documents
            var table = new ConsoleTable("Id", "Snapshot Folder", "Cur. Version");
            foreach (var document in _repositoryService.List())
                table.AddRow(document.EntityId,
                    document.SnapshotFolderId,
                    document.Versions.Any() ? document.Versions.Last() : null);
            table.Write();
        }

        private class InvalidCommandException : Exception
        {
            public InvalidCommandException(string message) : base($"Unknown command: {message}")
            {
            }
        }
    }
}