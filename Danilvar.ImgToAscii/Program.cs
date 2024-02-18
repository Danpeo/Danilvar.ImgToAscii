using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

while (true)
{
    Console.WriteLine("=====>>>>IMAGE TO ASCII<<<<=====");
    Console.WriteLine("ENTER ANY KEY TO USE CONVERTER OR PRESS 'Q' TO EXIT: ");
    string? key = Console.ReadLine();

    if (key != null && !key.Equals("q", StringComparison.CurrentCultureIgnoreCase))
    {
        Console.WriteLine("ENTER FILE PATH OR URL: ");
        string? filePathOrUrl = Console.ReadLine()?.Trim('"');

        if (File.Exists(filePathOrUrl) || Uri.TryCreate(filePathOrUrl, UriKind.Absolute, out _))
        {
            Console.WriteLine("ENTER WIDTH(PRESS 'ENTER' TO SKIP!): ");
            string? width = Console.ReadLine();
            Console.WriteLine("ENTER HEIGHT(PRESS 'ENTER' TO SKIP!): ");
            string? height = Console.ReadLine();
            if (string.IsNullOrEmpty(width))
                width = "100";
            if (string.IsNullOrEmpty(height))
                height = "100";

            await GenerateAsciiFromImage(filePathOrUrl, Convert.ToInt32(width), Convert.ToInt32(height));
        }
        else
        {
            Console.WriteLine("Invalid file path or URL.");
        }
    }
    else
    {
        Environment.Exit(69);
    }
}

static async Task GenerateAsciiFromImage(string filePathOrUrl, int width, int height)
{
    try
    {
        using var httpClient = new HttpClient();
        using HttpResponseMessage response = await httpClient.GetAsync(filePathOrUrl);
        if (response.IsSuccessStatusCode)
        {
            await using var stream = await response.Content.ReadAsStreamAsync();
            using var image = Image.Load<Rgba32>(stream);
            ascii(image);
        }
        else
        {
            Console.WriteLine($"Failed to download image from {filePathOrUrl}.");
        }
    }
    catch (NotSupportedException e)
    {
        using var image = Image.Load<Rgba32>(filePathOrUrl);
        ascii(image);
    }

    return;

    void ascii(Image<Rgba32> img)
    {
        img.Mutate(i => i.Resize(width, height));
        string ascii = ConvertToAscii(img);
        Console.WriteLine(ascii);
    }
}

static string ConvertToAscii(Image<Rgba32> image)
{
    char[] asciiChars = [' ', '.', ':', '-', '=', '+', '*', '#', '%', '@'];

    string asciiArt = "";

    for (int h = 0; h < image.Height; h++)
    {
        for (int w = 0; w < image.Width; w++)
        {
            Rgba32 pixelColor = image[w, h];
            int grayValue = (int)(0.299 * pixelColor.R + 0.587 * pixelColor.G + 0.114 * pixelColor.B);
            int asciiIndex = grayValue * (asciiChars.Length - 1) / 255;
            asciiArt += asciiChars[asciiIndex];
        }

        asciiArt += "\n";
    }

    return asciiArt;
}