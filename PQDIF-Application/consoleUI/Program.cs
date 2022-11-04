using pqdif_io;

// See https://aka.ms/new-console-template for more information

Console.WriteLine("Running PQDif to CSV application....");

string confirm;
do
{
    Console.WriteLine("Please confirm the PQDIF files are in (Y/N):");
    Console.WriteLine(MainFunction.getDefaultPQDIFFolderPath());
    confirm = Console.ReadLine();
} while (!(confirm.Equals("Y") || confirm.Equals("y")));

await MainFunction.batchProcessing();

Console.WriteLine("Finished Output, CSVs are in:");
Console.WriteLine(MainFunction.getDefaultCSVExportFolder());
Console.WriteLine("Application closing");