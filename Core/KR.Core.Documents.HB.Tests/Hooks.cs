
using Moq;
using TechTalk.SpecFlow;

namespace KR.Core.Documents.HB.Tests;

[Binding]
public sealed class Hooks
{


    [BeforeTestRun]
	public static void Initilize()
	{
           
    }

	[AfterTestRun]
	public static void Dispose()
	{
          
    }

      
}


