using System;
using System.Collections.Generic;
using System.IO;

namespace PolyAxisGraphs.FileGenerator;

internal enum FileType { txt, csv }
internal class FileGenerator
{
    private FileType Type;
    private Series X;
    private List<Series> Y;
    private Uri ID;
    internal FileGenerator(Series xseries, List<Series> yseries, FileType type, Uri InitialDirectory)
    {
        X = xseries;
        Y = yseries;
        Type = type;
        ID = InitialDirectory;
    }

    internal string GenerateData()
    {
        bool cont = true;
        while(cont)
        {
            cont = X.AddNextX();
            if(cont) foreach(var y in Y) y.AddNextY();
        }
        var path = FindNextFileName();
        if(path is not null) SaveFile(path);
        return path is not null ? path : "could not generate file. maximum file limit (1000) reached.";
    }

    private void SaveFile(string path)
    {
        char separator = Type == FileType.txt ? ' ' : ';';
        using(StreamWriter writer = new(path, append: true))
        {
            string line = "";
            line += X.Name;
            foreach(var y in Y) line += separator + y.Name;
            writer.WriteLine(line);
            for(int i = 0; i < X.Values.Count; i++)
            {
                line = "";
                line += X.Values[i].ToString();
                foreach(var y in Y) line += separator + y.Values[i].ToString();
                writer.WriteLine(line);
            }
        }
    }
    private string? FindNextFileName()
    {
        for(int i = 0; i < 1000; i++)
        {
            string path = Type == FileType.txt ? string.Format("{1}FileGenerator{0}.txt", i, ID.AbsolutePath) : string.Format("{1}FileGenerator{0}.csv", i, ID.AbsolutePath);
            if(!File.Exists(path)) return path;
        }
        return null;
    }
}