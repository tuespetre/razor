// <auto-generated/>
#pragma warning disable 1591
namespace Test
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
    public class TestComponent : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.RenderTree.RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            builder.OpenComponent<Test.MyComponent>(0);
            builder.AddAttribute(1, "OnClick", Microsoft.AspNetCore.Components.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<Microsoft.AspNetCore.Components.UIMouseEventArgs>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<Microsoft.AspNetCore.Components.UIMouseEventArgs>(this, EventCallback.Factory.Create<UIMouseEventArgs>(this, Increment))));
            builder.CloseComponent();
        }
        #pragma warning restore 1998
#line 3 "x:\dir\subdir\Test\TestComponent.cshtml"
            
    private int counter;
    private void Increment() {
        counter++;
    }

#line default
#line hidden
    }
}
#pragma warning restore 1591
