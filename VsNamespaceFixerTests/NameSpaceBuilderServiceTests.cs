using System;
using System.IO;
using NamespaceFixer;
using NamespaceFixer.NamespaceBuilder;
using Xunit;

namespace VsNamespaceFixerTests
{
    public class NameSpaceBuilderServiceTests
    {
        [Fact]
        public void ShouldGetNamespaceIgnoringFolders()
        {
            var options = new Options();
            options.NamespaceFormat = "{projectName}{fileToProjectPath}";
            options.FoldersToIgnore = "source;teste;opa";
            var service = new CsNamespaceBuilderService(options);

            var solutionFile = new FileInfo(@"D:\projetos\ncr-d-connect\NCR Ifood.sln");
            var projectFile = new FileInfo(@"D:\projetos\ncr-d-connect\Ncr.Ifood\Ncr.Ifood.csproj");

            var name = service.GetNamespace(@"D:\projetos\ncr-d-connect\Ncr.Ifood\Source\IntegradorDePedidos.cs", solutionFile, projectFile);

            Assert.DoesNotContain("source", name);
            Assert.False(name.EndsWith('.'));
        }
    }

    internal class Options : INamespaceAdjusterOptions
    {
        public string NamespaceFormat { get; set;  }
        public string FileExtensionsToIgnore { get; }
        public string FoldersToIgnore { get; set; }
    }
}
