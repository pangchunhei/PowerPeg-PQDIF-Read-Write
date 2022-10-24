using pqdif_io;

// See https://aka.ms/new-console-template for more information

MainFunction t = new MainFunction();

Console.WriteLine("Running PQDif to CSV application....");

string fileName = "t1.pqd";
string path = Path.Combine(Environment.CurrentDirectory, @"TestCase\", fileName);
await t.importPQDifFile(path);

Console.WriteLine("Finished Output, application closed");
