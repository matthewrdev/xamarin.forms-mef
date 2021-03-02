using System;
using System.ComponentModel.Composition;
using XamMef;

namespace XamMef
{
    [Export(typeof(IMyService))]
    public class MyService : IMyService
    {
    }
}