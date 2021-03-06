﻿namespace NamespaceFixer.Extensions
{
    using System.IO;

    public static class FileExtensions
    {
        public static string NameWithoutExtension(this FileInfo fileInfo)
        {
            var extension = fileInfo.Extension;

            return fileInfo.Name.Substring(0, fileInfo.Name.Length - extension.Length);
        }
    }
}