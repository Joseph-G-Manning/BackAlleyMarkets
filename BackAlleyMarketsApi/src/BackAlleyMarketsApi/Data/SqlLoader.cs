using System.Reflection;

public static class SqlLoader
{
    public static string Load(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Embedded names look like "YourProject.Sql.PostItem.sql"
        var resourceName =
            assembly
                .GetManifestResourceNames()
                .SingleOrDefault(n =>
                    n.EndsWith("." + fileName, StringComparison.OrdinalIgnoreCase)
                )
            ?? throw new FileNotFoundException(
                $"Embedded SQL resource '{fileName}' was not found. "
                    + "Check that the file is in the Sql folder and included as an EmbeddedResource."
            );

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
