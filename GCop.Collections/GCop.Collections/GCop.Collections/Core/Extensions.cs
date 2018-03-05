namespace GCop.Collections.Core
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    public static class Extensions
    {
        /// <summary>
        /// Determines if none of the items in this list meet a given criteria.
        /// </summary>
        public static bool None<T>(this IEnumerable<T> list, Func<T, bool> criteria)
        {
            return !list.Any(criteria);
        }

        /// <summary>
        /// Determines if this is null or an empty list.
        /// </summary>
        public static bool None<T>(this IEnumerable<T> list)
        {
            if (list == null) return true;
            return !list.Any();
        }

        public static TimeSpan Minutes(this int number) => TimeSpan.FromMinutes(number);

        public static DateTime ToLocal(this DateTime utcValue) => utcValue.ToLocal(LocalTime.CurrentTimeZone());

        public static DateTime ToLocal(this DateTime utcValue, TimeZoneInfo timeZone)
        {
            return new DateTime(utcValue.Ticks, DateTimeKind.Local).Add(timeZone.GetUtcOffset(utcValue));
        }

        public static DateTime ToUniversal(this DateTime localValue)
        {
            return localValue.ToUniversal(sourceTimezone: LocalTime.CurrentTimeZone());
        }

        public static DateTime ToUniversal(this DateTime localValue, TimeZoneInfo sourceTimezone)
        {
            return new DateTime(localValue.Ticks, DateTimeKind.Utc).Subtract(sourceTimezone.BaseUtcOffset);
        }

        public static void AppendLineIf(this StringBuilder builder, string text, bool condition)
        {
            if (condition) builder.AppendLine(text);
        }

        public static bool HasValue(this string text) => !string.IsNullOrEmpty(text);

        static string ToLogText(object item)
        {
            try
            {
                if (item is DictionaryEntry)
                    return ((DictionaryEntry)item).Get(x => x.Key + ": " + x.Value);
                return item.ToString();
            }
            catch
            {
                return "?";
            }
        }

        /// <summary>
        /// Gets a specified member of this object. If this is null, null will be returned. Otherwise the specified expression will be returned.
        /// </summary>
        [System.Diagnostics.DebuggerStepThrough]
        public static K Get<T, K>(this T item, Func<T, K> selector)
        {
            if (object.ReferenceEquals(item, null))
                return default(K);
            else
            {
                try
                {
                    return selector(item);
                }
                catch (NullReferenceException)
                {
                    return default(K);
                }
            }
        }

        public static bool IsEmpty(this string text) => string.IsNullOrEmpty(text);

        public static string WithPrefix(this string text, string prefix)
        {
            if (text.IsEmpty()) return string.Empty;
            else return prefix + text;
        }

        public static string Or(this string text, string defaultValue)
        {
            if (string.IsNullOrEmpty(text)) return defaultValue;
            else return text;
        }

        public static string[] ToLines(this string text)
        {
            if (text == null) return new string[0];

            return text.Split('\n').Select(l => l.Trim('\r')).ToArray();
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> list, Func<T, bool> criteria)
        {
            return list.Where(i => !criteria(i));
        }

        public static string ToString<T>(this IEnumerable<T> list, string seperator)
        {
            return ToString(list, seperator, seperator);
        }

        public static string ToString<T>(this IEnumerable<T> list, string seperator, string lastSeperator)
        {
            var result = new StringBuilder();

            var items = list.Cast<object>().ToArray();

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];

                if (item == null) result.Append("{NULL}");
                else result.Append(item.ToString());

                if (i < items.Length - 2)
                    result.Append(seperator);

                if (i == items.Length - 2)
                    result.Append(lastSeperator);
            }

            return result.ToString();
        }

        public static string ToFullMessage(this Exception ex)
        {
            return ToFullMessage(ex, additionalMessage: null, includeStackTrace: false, includeSource: false, includeData: false);
        }

        public static string ToFullMessage(this Exception error, string additionalMessage, bool includeStackTrace, bool includeSource, bool includeData)
        {
            if (error == null) throw new NullReferenceException("This exception object is null");

            var r = new StringBuilder();
            r.AppendLineIf(additionalMessage, additionalMessage.HasValue());
            var err = error;
            while (err != null)
            {
                r.AppendLine(err.Message);
                if (includeData && err.Data != null)
                {
                    r.AppendLine("\r\nException Data:\r\n{");
                    foreach (var i in err.Data)
                        r.AppendLine(ToLogText(i).WithPrefix("    "));

                    r.AppendLine("}");
                }

                if (err is ReflectionTypeLoadException)
                {
                    foreach (var loaderEx in (err as ReflectionTypeLoadException).LoaderExceptions)
                        r.AppendLine("Type load exception: " + loaderEx.ToFullMessage());
                }

                try
                {
                    //r.AppendLineIf((err as HttpUnhandledException)?.GetHtmlErrorMessage().TrimBefore("Server Error"));
                }
                catch
                {
                    // No logging is needed
                }

                err = err.InnerException;

                if (err is TargetInvocationException)
                    err = err.InnerException;

                if (err != null)
                {
                    r.AppendLine();
                    if (includeStackTrace)
                        r.AppendLine("###############################################");
                    r.Append("Base issue: ");
                }
            }

            if (includeStackTrace && error.StackTrace.HasValue())
            {
                var stackLines = error.StackTrace.Or("").Trim().ToLines();
                stackLines = stackLines.Except(l => l.Trim().StartsWith("at System.Data.")).ToArray();
                r.AppendLine(stackLines.ToString("\r\n\r\n").WithPrefix("\r\n--------------------------------------\r\nSTACK TRACE:\r\n\r\n"));
            }

            return r.ToString();
        }

        public static void RemoveNulls<T>(this IList<T> list) => list.RemoveWhere(i => i == null);

        public static void RemoveWhere<T>(this IList<T> list, Func<T, bool> selector)
        {
            lock (list)
            {
                var itemsToRemove = list.Where(selector).ToList();
                list.Remove(itemsToRemove);
            }
        }

        public static void Remove<T>(this IList<T> list, IEnumerable<T> itemsToRemove)
        {
            if (itemsToRemove != null)
                foreach (var item in itemsToRemove)
                    if (list.Contains(item)) list.Remove(item);
        }

        public static bool TrueForAtLeastOnce<T>(this IEnumerable<T> collection, Predicate<T> predicate)
        {
            foreach (var item in collection)
            {
                if (predicate(item)) return true;
            }
            return false;
        }

        public static IEnumerable<string> Trim(this IEnumerable<string> list)
        {
            if (list == null) return Enumerable.Empty<string>();

            return list.Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => v.Trim()).Where(v => v.HasValue()).ToArray();
        }

        public static bool StartsWith(this string input, string other, bool caseSensitive)
        {
            if (other.IsEmpty()) return false;

            if (caseSensitive) return input.StartsWith(other);
            else return input.StartsWith(other, StringComparison.OrdinalIgnoreCase);
        }

        public static string Remove(this string text, string substringToRemove)
        {
            if (text.IsEmpty()) return text;

            return text.Replace(substringToRemove, string.Empty);
        }

        /// <summary>
        /// Adds the specified key/value pair to this list.
        /// </summary>
        public static KeyValuePair<TKey, TValue> Add<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> list, TKey key, TValue value)
        {
            var result = new KeyValuePair<TKey, TValue>(key, value);
            list.Add(result);

            return result;
        }

        public static bool IsKind(this SyntaxNode node, params SyntaxKind[] kinds)
        {
            foreach (var kind in kinds)
                if (Microsoft.CodeAnalysis.CSharpExtensions.IsKind(node, kind)) return true;
            return false;
        }

        public static TAncestor GetSingleAncestor<TAncestor>(this SyntaxNode node) where TAncestor : SyntaxNode => node.Ancestors().OfType<TAncestor>().FirstOrDefault();

        public static string GetNodeSourceText(this SyntaxNode node)
        {
            if (node.IsKind(SyntaxKind.MethodDeclaration, SyntaxKind.ClassDeclaration)) return string.Empty;
            SyntaxNode parent = node.GetSingleAncestor<MethodDeclarationSyntax>();
            if (parent == null)
            {
                parent = node.GetSingleAncestor<ClassDeclarationSyntax>();
            }
            return parent?.NormalizeWhitespace()?.ToString();
        }

        public static T As<T>(this SyntaxNode node) where T : SyntaxNode => node as T;

        public static TSymbol As<TSymbol>(this ISymbol symbol) => symbol is TSymbol ? (TSymbol)symbol : default(TSymbol);

        public static bool HasAttribute(this ISymbol symbol, string name)
        {
            if (symbol == null) return false;

            var result = false;

            if (symbol is IPropertySymbol)
            {
                result = symbol.As<IPropertySymbol>().Type.GetAttributes().Any(it => it.AttributeClass.Name == name);
                if (!result)
                    result = symbol.As<IPropertySymbol>().GetAttributes().Any(it => it.AttributeClass.Name == name);
            }
            else if (symbol is IParameterSymbol)
                result = symbol.As<IParameterSymbol>().Type.GetAttributes().Any(it => it.AttributeClass.Name == name);

            else if (symbol is IFieldSymbol)
                result = symbol.As<IFieldSymbol>().Type.GetAttributes().Any(it => it.AttributeClass.Name == name);

            else if (symbol is ILocalSymbol)
                result = symbol.As<ILocalSymbol>().Type.GetAttributes().Any(it => it.AttributeClass.Name == name);

            else if (symbol is INamedTypeSymbol)
                result = symbol.As<INamedTypeSymbol>().GetAttributes().Any(it => it.AttributeClass.Name == name);

            else if (symbol is ITypeSymbol)
                result = symbol.As<ITypeSymbol>().GetAttributes().Any(it => it.AttributeClass.Name == name);

            return result;
        }

        public static bool IsAnyKind(this SyntaxNode node, params SyntaxKind[] kinds)
        {
            foreach (var kind in kinds)
            {
                if (node.IsKind(kind)) return true;
            }
            return false;
        }

        public static int GetLineNumberToReport(this SyntaxNode self) => self.GetLocation().GetLineNumberToReport();

        private static int GetLineNumberToReport(this Location self) => self.GetLineSpan().StartLinePosition.Line + 1;

        public static string GetName(this MethodDeclarationSyntax node)
        {
            if (node == null) return null;
            return node.Identifier.ValueText;
        }

        public static bool IsInherited<T>(this ISymbol symbol) => IsInherited(symbol, typeof(T).Name, typeof(T).FullName);

        public static bool IsInherited(this ISymbol symbol, string typeName, string typeFullName = "")
        {
            if (symbol == null) return false;

            var result = false;

            if (symbol is IPropertySymbol)
                result = symbol.As<IPropertySymbol>()?.Type.BaseType?.ToString() == typeFullName ||
                    IsInheritedFromIEntity(symbol, typeName);

            else if (symbol is IParameterSymbol)
                result = symbol.As<IParameterSymbol>()?.Type.BaseType?.ToString() == typeFullName || IsInheritedFromIEntity(symbol, typeName);

            else if (symbol is IFieldSymbol)
                result = symbol.As<IFieldSymbol>()?.Type.BaseType?.ToString() == typeFullName || IsInheritedFromIEntity(symbol, typeName);

            else if (symbol is ILocalSymbol)
                result = symbol.As<ILocalSymbol>()?.Type.BaseType?.ToString() == typeFullName || IsInheritedFromIEntity(symbol, typeName);

            else if (symbol is ITypeParameterSymbol)
                result = symbol.As<ITypeParameterSymbol>()?.BaseType?.ToString() == typeFullName ||
                    IsInheritedFromIEntity(symbol, typeName);

            else if (symbol is INamedTypeSymbol)
                result = symbol.As<INamedTypeSymbol>()?.BaseType?.ToString() == typeFullName || IsInheritedFromIEntity(symbol, typeName);

            else if (symbol is ITypeSymbol)
                result = symbol.As<ITypeSymbol>()?.BaseType?.ToString() == typeFullName || IsInheritedFromIEntity(symbol, typeName);

            return result;
        }

        private static bool IsInheritedFromIEntity(ISymbol symbol, string typeName)
        {
            if (symbol is ITypeParameterSymbol)
                return (symbol as ITypeParameterSymbol).AllInterfaces.Any(it => it.Name == typeName);
            if (symbol is INamedTypeSymbol)
                return (symbol as INamedTypeSymbol).AllInterfaces.Any(it => it.Name == typeName);
            if (symbol is ITypeSymbol)
                return (symbol as ITypeSymbol).AllInterfaces.Any(it => it.Name == typeName);
            return false;
        }

        public static IEnumerable<ReturnStatementSyntax> GetReturnStatements(this SyntaxNode node)
        {
            var returns = node?.DescendantNodes().OfType<ReturnStatementSyntax>().ToList();
            return returns.Where(@return => @return.AncestorsAndSelf().None(it => it.IsKind(SyntaxKind.SimpleLambdaExpression)));
        }

        public static string Normalize(this ITypeSymbol returnType)
        {
            if (returnType == null) return "T";
            var sepratedParts = returnType.ToDisplayParts();
            if (sepratedParts.Length < 2) return "T";
            return sepratedParts[sepratedParts.Length - 2].ToString();
        }

        public static bool IsSingleCharacter(this string value) => value.IsEmpty() ? false : value.Length == 1;

        public static SyntaxNode GetParent<T>(this SyntaxNode node) => GetParent(node, typeof(T));

        public static SyntaxNode GetParent(this SyntaxNode node, Type parentType)
        {
            var parent = node;

            while (parent != null)
            {
                parent = parent?.Parent;
                if (parent == null) break;
                if (parent.GetType().Equals(parentType))
                    return parent;
            }

            return parent;
        }

        public static string GetIdentifier(this SyntaxNode node) => GetIdentifierSyntax(node)?.Identifier.ValueText;

        public static IdentifierNameSyntax GetIdentifierSyntax(this SyntaxNode node)
        {
            if (node == null) return null;
            if (node is IdentifierNameSyntax) return (IdentifierNameSyntax)node;

            return node.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
        }

        public static bool Contains(this IEnumerable<string> list, string instance, bool caseSensitive)
        {
            if (caseSensitive || instance.IsEmpty())
                return list.Contains(instance);
            else return list.Any(i => i.HasValue() && i.Equals(instance, StringComparison.OrdinalIgnoreCase));
        }

        public static bool Lacks<T>(this IEnumerable<T> list, T item) => !list.Contains(item);

        public static bool HasMany<T>(this IEnumerable<T> collection)
        {
            using (var en = collection.GetEnumerator())
                return en.MoveNext() && en.MoveNext();
        }

        public static bool IsAnyOf(this string text, params string[] items)
        {
            if (text == null) return items.Any(x => x == null);

            return items.Contains(text);
        }
    }

    public class LocalTime
    {
        /// <summary>
        /// By default provides the current server's timezone.
        /// You can override this to provide user-specific time-zones or based on any other system setting.
        /// </summary>
        public static Func<TimeZoneInfo> CurrentTimeZone = () => TimeZoneInfo.Local;

        /// <summary>
        /// If set, it will provide the "Now" value.
        /// Note: This has lower priority than thread-level overrides.
        /// </summary>
        static Func<DateTime> GlobalNowGetter;

        /// <summary>        
        /// <para>Gets the local current date/time of the application.</para>
        /// <para>By default it equals to System.DateTime.Now.</para>
        /// <para>To override its value, you should wrap the calling code inside "using (LocalTime.SetNow(some date)) { ... }"</para>
        /// <para>&#160;</para>
        /// <para> Examples:</para>
        /// <para>—————————————————————————————————</para>
        /// <para>var now = LocalTime.Now // which is identical to DateTime.Now</para>
        /// <para>—————————————————————————————————</para>
        /// <para>using (LocalTime.Set(DateTime.Parse("15/01/2000 06:13")))</para>
        /// <para>{</para>
        /// <para> var date = LocalTime.Now; // that sets date to 15th Jan 200 at 6:13.</para>
        /// <para>}</para>
        /// </summary>
        public static DateTime Now
        {
            get
            {
                var setting = ProcessContext<OverriddenApplicationDate>.Current;

                var nowGetter = setting?.NowGetter ?? GlobalNowGetter;

                if (nowGetter != null) return nowGetter();

                return DateTime.UtcNow.ToLocal();
            }
        }

        /// <summary>
        /// Gets the current Universal Time.
        /// </summary>
        public static DateTime UtcNow
        {
            get
            {
                var setting = ProcessContext<OverriddenApplicationDate>.Current;

                var nowGetter = setting?.NowGetter ?? GlobalNowGetter;

                if (nowGetter != null) return nowGetter().ToUniversal();

                return DateTime.UtcNow;
            }
        }

        /// <summary>
        /// <para>Gets the local current date of the application (no time).</para>
        /// <para>By default it equals to System.DateTime.Today.</para>
        /// <para>To override its value, you should wrap the calling code inside "using (LocalTime.SetNow(some date)) { ... }"</para>
        /// <para>&#160;</para>
        /// <para> Examples:</para>
        /// <para>—————————————————————————————————</para>
        /// <para>var now = LocalTime.Today // which is identical to DateTime.Today</para>
        /// <para>—————————————————————————————————</para>
        /// <para>using (LocalTime.Set(DateTime.Parse("15/01/2000 06:13")))</para>
        /// <para>{</para>
        /// <para> var date = LocalTime.Today; // that sets date to 15th Jan 200.</para>
        /// <para>}</para>
        /// </summary>
        public static DateTime Today => Now.Date;

        /// <summary>
        /// Gets the current Universal Time's date part (without time).
        /// </summary>
        public static DateTime UtcToday => UtcNow.Date;

        /// <summary>
        /// <para>Sets the current time of the application.</para>
        /// <para>&#160;</para>
        /// <para> Examples:</para>
        /// <para>—————————————————————————————————</para>
        /// <para>using (LocalTime.Set(DateTime.Parse("15/01/2000 06:13")))</para>
        /// <para>{</para>
        /// <para><tab> </tab>//Here any call for LocalTime.Now/Today will return 15th of Jan 2000 (at 6:30).</para>
        /// <para>}</para>
        /// </summary>
        public static IDisposable Set(DateTime overriddenNow) => Set(() => overriddenNow);

        /// <summary>
        /// <para>Sets the current time function of the application.</para>
        /// </summary>
        public static IDisposable Set(Func<DateTime> overriddenNow)
        {
            return new ProcessContext<OverriddenApplicationDate>(new OverriddenApplicationDate(overriddenNow));
        }

        /// <summary>
        /// Sets the current time function of the application.
        /// Note: This has lower priority than thread-level time setting.
        /// </summary>
        public static void RedefineNow(Func<DateTime> overriddenNow)
        {
            GlobalNowGetter = overriddenNow;
        }

        public static bool IsRedefined
        {
            get
            {
                return (ProcessContext<OverriddenApplicationDate>.Current?.NowGetter ?? GlobalNowGetter) != null;
            }
        }

        /// <summary>
        /// <para>Freezes the time to the current system time.</para>
        /// <para>&#160;</para>
        /// <para> Examples:</para>
        /// <para>—————————————————————————————————</para>
        /// <para>using (LocalTime.Stop())</para>
        /// <para>{</para>
        /// <para> // Freezes the time to Datetime.Now.</para>
        /// <para>}</para>
        /// </summary>
        public static IDisposable Stop() => Set(DateTime.Now);

        /// <summary>
        /// Adds the specified time to the current LocalTime.
        /// </summary>
        public static void Add(TimeSpan time)
        {
            var setting = ProcessContext<OverriddenApplicationDate>.Current;

            if (setting == null)
                throw new InvalidOperationException("The current thread is not running inside a LocalTime.");

            var currentGetter = setting.NowGetter;
            setting.NowGetter = () => currentGetter.Invoke().Add(time);
        }

        public static void AddSeconds(double seconds) => Add(TimeSpan.FromSeconds(seconds));

        public static void AddMinutes(double minutes) => Add(TimeSpan.FromMinutes(minutes));

        public static void AddHours(double hours) => Add(TimeSpan.FromHours(hours));

        public static void AddDays(double days) => Add(TimeSpan.FromDays(days));
    }

    class OverriddenApplicationDate
    {
        public Func<DateTime> NowGetter { get; internal set; }

        /// <summary>
        /// Creates a new OverriddenApplicationDate instance.
        /// </summary>
        public OverriddenApplicationDate(Func<DateTime> time)
        {
            NowGetter = time;
        }
    }

    /// <summary>
    /// Provides process context data sharing mechanism to pass arguments and data around execution in a shared pipeline.
    /// It supports context nesting.
    /// </summary>
    public class ProcessContext<T> : IDisposable
    {
        static readonly object SyncLock = new object();
        static Func<T> DefaultDataExpression = delegate { return default(T); };

        /// <summary>
        /// Creates a new Process Context.
        /// </summary>
        public ProcessContext(T data) : this(null, data) { }

        /// <summary>
        /// Creates a new Process Context with the specified key and data.
        /// </summary>
        public ProcessContext(string key, T data)
        {
            Data = data;
            Key = key;
            GetContexts(key).Add(this);
        }

        static string GetProcessContextKey(string key)
        {
            return "ProcessContext:" + typeof(T).FullName + "|K:" + key;
        }

        /// <summary>
        /// Sets the default data expression, when no context data is available.
        /// </summary>
        public static void SetDefaultDataExpression(Func<T> expression)
        {
            if (expression == null)
                expression = delegate { return default(T); };

            DefaultDataExpression = expression;
        }

        /// <summary>
        /// Gets or sets the Data of this ProcessContext.
        /// </summary>
        public T Data { get; private set; }

        /// <summary>
        /// Gets or sets the key of this ProcessContext.
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// A number of nested process context objects in the currenly executing thread.
        /// </summary>
        public static List<ProcessContext<T>> GetContexts(string key)
        {
            lock (SyncLock)
            {
                var contextKey = GetProcessContextKey(key);

                if (CallContext.GetData(contextKey) == null)
                {
                    var result = new List<ProcessContext<T>>();

                    CallContext.SetData(contextKey, result);
                    return result;
                }
                else
                {
                    // Already exists:
                    return CallContext.GetData(contextKey) as List<ProcessContext<T>>;
                }
            }
        }

        /// <summary>
        /// Gets the data of the current context with default key (null).
        /// </summary>
        public static T Current => GetCurrent(null);

        /// <summary>
        /// Gets the data of the current context with the specified key.
        /// </summary>
        public static T GetCurrent(string key)
        {
            if (GetContexts(key).Count == 0) return DefaultDataExpression();
            return GetContexts(key).Last().Data;
        }

        /// <summary>
        /// Disposes the current process context and switches the actual context to the containing process context.
        /// </summary>
        public void Dispose()
        {
            try { GetContexts(Key).Remove(this); }
            catch { }
        }
    }

    /// <summary>
    /// Provides a facade for easiper creation of a Process Context.
    /// </summary>
    public static class ProcessContext
    {
        /// <summary>
        /// Create a process context for the specified object.
        /// To access the context object, you can use ProcessContext&lt;Your Type&gt;.Current.
        /// </summary>
        public static ProcessContext<T> Create<T>(T contextObject)
        {
            return new ProcessContext<T>(contextObject);
        }

        /// <summary>
        /// Create a process context for the specified object with the specified key.
        /// To access the context object, you can use ProcessContext&lt;Your Type&gt;.GetCurrent(key).
        /// </summary>
        public static ProcessContext<T> Create<T>(string key, T contextObject)
        {
            return new ProcessContext<T>(key, contextObject);
        }
    }

    /// <summary>
    /// Provides a way to set contextual data that flows with the call and 
    /// async context of a test or invocation.
    /// </summary>
    public static class CallContext
    {
        static ConcurrentDictionary<string, AsyncLocal<object>> state = new ConcurrentDictionary<string, AsyncLocal<object>>();

        /// <summary>
        /// Stores a given object and associates it with the specified name.
        /// </summary>
        /// <param name="name">The name with which to associate the new item in the call context.</param>
        /// <param name="data">The object to store in the call context.</param>
        public static void SetData(string name, object data) =>
            state.GetOrAdd(name, _ => new AsyncLocal<object>()).Value = data;

        /// <summary>
        /// Retrieves an object with the specified name from the <see cref="CallContext"/>.
        /// </summary>
        /// <param name="name">The name of the item in the call context.</param>
        /// <returns>The object in the call context associated with the specified name, or <see langword="null"/> if not found.</returns>
        public static object GetData(string name) =>
            state.TryGetValue(name, out AsyncLocal<object> data) ? data.Value : null;
    }
}
