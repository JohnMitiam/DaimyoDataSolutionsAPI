using DbUp;
using DbUp.ScriptProviders;
using System.Text;

var connectionString = args.FirstOrDefault()
        ?? "";

var path = Path.Combine(Directory.GetCurrentDirectory(), "DaimyoDataSolutionsAPI\\DaimyoDataSolutionsAPI.Database.DbUpMySQL\\Scripts");

var options = new FileSystemScriptOptions
{
    // true = scan into subdirectories, false = top directory only
    IncludeSubDirectories = true,

    // Patterns to search the file system for. Set to "*.sql" by default.
    Extensions = new[] { "*.sql" },

    // Type of text encoding to use when reading the files. Defaults to "Encoding.UTF8".
    Encoding = Encoding.UTF8
};

var upgrader =
    DeployChanges.To
    .MySqlDatabase(connectionString)
    .WithScriptsFromFileSystem(path, options)
    .LogToConsole()
    .Build();

var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error);
    Console.ResetColor();
#if DEBUG
    Console.ReadLine();
#endif
    return -1;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Success!");
Console.ResetColor();
return 0;