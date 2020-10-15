namespace SUS.MvcFramework.ViewEngine
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public class SusViewEngine : IViewEngine
    {
        private const string AtSign = "@";

        public string GetHtml(string templateCode, object viewModel)
        {
            var csharpCode = GenerateCSharpFromTamplate(templateCode, viewModel);
            var executableObject = GenerateExecutableObject(csharpCode, viewModel);
            var html = executableObject.ExecuteTemplate(viewModel);

            return html;
        }

        private string GenerateCSharpFromTamplate(string templateCode, object viewModel)
        {
            var typeOfModel = "object";

            if (viewModel != null)
            {
                if (viewModel.GetType().IsGenericType)
                {
                    var modelName = viewModel.GetType().FullName;
                    var arguments = viewModel.GetType().GenericTypeArguments;
                    typeOfModel = modelName.Substring(0,modelName.IndexOf("`")) + "<" + string.Join(",", arguments.Select(x => x.FullName))  + ">";
                }
                else
                {
                    typeOfModel = viewModel.GetType().FullName;
                }
            }

            var code = @"
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using SUS.MvcFramework.ViewEngine;

namespace ViewNamespace
{
    public class ViewClass : IView
    {
        public string ExecuteTemplate(object viewModel)
        {
            var Model = viewModel as " + typeOfModel + @";
            var html = new StringBuilder();

            " + GetMethodBody(templateCode) + @"

            return html.ToString();
        }
    }
}
";
            return code;
        }

        private string GetMethodBody(string templateCode)
        {
            var regex = new Regex(@"[^\""\s&\'\<]+");
            var csharpCode = new StringBuilder();
            var sr = new StringReader(templateCode);

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.TrimStart().StartsWith(AtSign))
                {
                    var index = line.IndexOf(AtSign);
                    line = line.Substring(index + 1);
                    csharpCode.AppendLine(line);
                }
                else if (line.TrimStart().StartsWith("{") || line.TrimStart().StartsWith("}"))
                {
                    csharpCode.AppendLine(line);
                }
                else
                {
                    csharpCode.Append($"html.AppendLine(@\"");

                    while (line.Contains(AtSign))
                    {
                        var atSignLocation = line.IndexOf(AtSign);
                        var htmlBeforeAtSign = line.Substring(0, atSignLocation);
                        csharpCode.Append(htmlBeforeAtSign.Replace("\"", "\"\"") + "\" + ");
                        var lineAfterAtSign = line.Substring(atSignLocation + 1);
                        var code = regex.Match(lineAfterAtSign).Value;
                        csharpCode.Append(code + " + @\"");
                        line = lineAfterAtSign.Substring(code.Length);
                    }

                    csharpCode.AppendLine(line.Replace("\"", "\"\"") + "\");");
                }
            }

            return csharpCode.ToString();
        }

        private IView GenerateExecutableObject(string charpCode, object viewModel)
        {
            var compileResult = CSharpCompilation.Create("ViewAssembly")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IView).Assembly.Location));

            if (viewModel != null)
            {
                if (viewModel.GetType().IsGenericType)
                {
                    var genericArguments = viewModel.GetType().GetGenericArguments();

                    foreach (var genericArgument in genericArguments)
                    {
                        compileResult = compileResult.AddReferences(MetadataReference.CreateFromFile(genericArgument.Assembly.Location));
                    }
                }

                compileResult = compileResult.AddReferences(MetadataReference.CreateFromFile(viewModel.GetType().Assembly.Location));
            }

            var libraries = Assembly.Load(new AssemblyName("netstandard")).GetReferencedAssemblies();

            foreach (var library in libraries)
            {
                compileResult = compileResult.AddReferences(MetadataReference.CreateFromFile(Assembly.Load(library).Location));
            }

            compileResult = compileResult.AddSyntaxTrees(SyntaxFactory.ParseSyntaxTree(charpCode));

            using var memoryStream = new MemoryStream();
            var result = compileResult.Emit(memoryStream);

            if (!result.Success)
            {
                return new ErrorView(result.Diagnostics
                    .Where(x => x.Severity == DiagnosticSeverity.Error)
                    .Select(x => x.GetMessage()), charpCode);
            }

            memoryStream.Seek(0, SeekOrigin.Begin);

            var byteAssembly = memoryStream.ToArray();
            var assembly = Assembly.Load(byteAssembly);
            var viewType = assembly.GetType("ViewNamespace.ViewClass");
            var instance = Activator.CreateInstance(viewType);
            return instance as IView;
        }
    }
}
