// See https://aka.ms/new-console-template for more information

using Microsoft.CodeAnalysis;
using RoslynPlayground;

const string rootClassContent = @"
public class RootClass
{
    public void DoSomething()
    {
        var s = new A();
        s.Try();
    }

    internal A A {get;set;}
}
";

const string aClassContent = @"
public class A
{
    public void Try()
    {
        // do nothing
    }
}
";

var workspace = new AdhocWorkspace();
var project = workspace.AddProject("Playground", LanguageNames.CSharp);
project = project.AddDocument("RootClass.cs", rootClassContent).Project;
project = project.AddDocument("A.cs", aClassContent).Project;

project = await Processor.Process(project);
