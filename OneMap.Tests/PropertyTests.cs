using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ReactiveUI;

namespace OneMap.Tests
{
    public class PropertyTests
    {

        public class Parent : ReactiveObject
        {

        }


        public class Child : ReactiveObject
        {
            private bool _canDoSomething;


            public bool CanDoSomething
            {
                get { return _canDoSomething; }
                set { this.RaiseAndSetIfChanged(ref _canDoSomething, value); }
            }
        }
    }
}
