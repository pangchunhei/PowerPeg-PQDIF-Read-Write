using pqdif_io;

// See https://aka.ms/new-console-template for more information

string pqdifPath;
MainFunction t;

Console.WriteLine("Running PQDif to CSV application....");
Console.WriteLine("Please input the absolute file path for the PQDif file, if want to use default press 'Enter'");
string inputPqdifPath = Console.ReadLine();

if (string.IsNullOrEmpty(inputPqdifPath))
{
    inputPqdifPath = "t1.pqd";
    pqdifPath = Path.Combine(Environment.CurrentDirectory, @"TestCase\", inputPqdifPath);
}
else
{
    pqdifPath = inputPqdifPath;
}

Console.WriteLine("Please input the absolute file path and name for the output csv file, if want to use default (bin folder) press 'Enter'");
string inputCSVPath = Console.ReadLine();

if (string.IsNullOrEmpty(inputCSVPath))
{
    t = new MainFunction(pqdifPath);
}
else
{
    t = new MainFunction(pqdifPath, inputCSVPath);
}

await t.importPQDifFile();

Console.WriteLine("Finished Output, application closed");
