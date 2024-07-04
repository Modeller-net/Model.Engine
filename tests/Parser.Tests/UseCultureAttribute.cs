using System.Globalization;
using System.Reflection;
using Xunit.Sdk;

namespace Modeller.ParserTests;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class UseCultureAttribute(string culture, string uiCulture) : BeforeAfterTestAttribute
{
	private readonly CultureInfo _culture = new(culture);
	private readonly CultureInfo _uiCulture = new(uiCulture);
	private CultureInfo? _originalCulture;
	private CultureInfo? _originalUICulture;

	public string Culture { get; } = culture;

	public string UiCulture { get; } = uiCulture;

	public UseCultureAttribute(string culture)
		: this(culture, culture)
	{
	}

	public override void Before(MethodInfo methodUnderTest)
	{
		_originalCulture = Thread.CurrentThread.CurrentCulture;
		_originalUICulture = Thread.CurrentThread.CurrentUICulture;

		Thread.CurrentThread.CurrentCulture = _culture;
		Thread.CurrentThread.CurrentUICulture = _uiCulture;
	}

	public override void After(MethodInfo methodUnderTest)
	{
		Thread.CurrentThread.CurrentCulture = _originalCulture!;
		Thread.CurrentThread.CurrentUICulture = _originalUICulture!;
	}
}
