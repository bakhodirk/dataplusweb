using System.ComponentModel.DataAnnotations;

namespace DataPlus.Web.UI.Test;

[TestClass]
public class AccessorTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullInstance()
    {
        AccessorHelper.GetTypeAccessor<ContextTest>(null!);
    }

    [TestMethod]
    public void ValidAccessor()
    {
        var context = new ContextTest();
        var accessor = AccessorHelper.GetTypeAccessor(context);

        var property1 = accessor.Members.First(e => e.Name == nameof(ContextTest.Property1));
        Assert.IsNotNull(property1);
        Assert.AreEqual(1, property1.GetValue(context));

        var property2 = accessor.Members.First(e => e.Name == nameof(ContextTest.Property2));
        Assert.IsNotNull(property2);
        Assert.AreEqual(2, property2.GetValue(context));

        var method1 = accessor.Actions.First(e => e.Name == nameof(ContextTest.Method1));
        Assert.IsNotNull(method1);

        var method2 = accessor.Actions.First(e => e.Name == nameof(ContextTest.Method2));
        Assert.IsNotNull(method2);

        var method3 = accessor.Actions.First(e => e.Name == nameof(ContextTest.Method3));
        Assert.IsNotNull(method3);
        Assert.AreEqual(1, method3.Invoke(context, null!));

        var method4 = accessor.Actions.First(e => e.Name == nameof(ContextTest.Method4));
        Assert.IsNotNull(method4);
        Assert.AreEqual(2, method4.Invoke(context, new ContextTextActionValueProvider(2)));

        var method5 = accessor.Actions.First(e => e.Name == nameof(ContextTest.Method5));
        Assert.IsNotNull(method5);
    }

    public class ContextTest
    {
        public int Property1 { get; set; } = 1;

        public int Property2 { get; } = 2;

        public void Method1() { }

        public void Method2(int arg1) { }

        public int Method3() => 1;

        public int Method4(int arg1) => arg1;

        [Action]
        public void Method5() { }

    }

    public class ContextTextActionValueProvider(params object[] _values) : IActionArgumentValueProvider
    {
        public bool TryGetValue(int index, Type type, out object? value)
        {
            if (index < _values.Length)
            {
                value = _values[index];
                return true;
            }
            value = null;   
            return false;
        }
    }
}