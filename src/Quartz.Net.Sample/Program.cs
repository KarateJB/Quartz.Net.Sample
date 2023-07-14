using Spectre.Console;
using System.Threading.Tasks;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

string IPAddress = AnsiConsole.Prompt(new TextPrompt<string>("Enter IPAddress to connect :"));
string Port = AnsiConsole.Prompt(new TextPrompt<string>("Enter Port to connect :"));
