using System.Collections.Generic;
using System.Linq;

namespace System
{
    public struct Option<T>
    {
        #region Constants

        public static Option<T> None => default(Option<T>);

        #endregion

        #region Members

        private readonly T inner;

        public bool HasValue { get; }

        #endregion

        #region Constructors

        public Option(T obj)
        {
            inner = obj;
            HasValue = obj != null;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Unwraps the Option to a List that contains the Element if it's not None.
        /// This should be used with foreach.
        /// </summary>
        /// <returns>A List that contains the Element if it's not None.</returns>
        public IEnumerable<T> Unwrap()
        {
            if (HasValue)
                yield return inner;
        }

        public T Unwrap(T defaultValue)
        {
            return HasValue ? inner : defaultValue;
        }

        public T Unwrap(Func<T> defaultValueFunction)
        {
            return HasValue ? inner : defaultValueFunction();
        }

        public R Match<R>(Func<T, R> notEmpty, R empty)
        {
            return HasValue ? notEmpty(inner) : empty;
        }

        public R Match<R>(Func<T, R> notEmpty, Func<R> empty)
        {
            return HasValue ? notEmpty(inner) : empty();
        }

        public R Match<R>(R notEmpty, R empty)
        {
            return HasValue ? notEmpty : empty;
        }

        public Option<T> Match(Action<T> notEmpty, Action empty)
        {
            if (HasValue)
                notEmpty(inner);
            else
                empty();

            return this;
        }

        public Option<R> Map<R>(Func<T, R> func)
        {
            return HasValue ? func(inner) : Option<R>.None;
        }

        public Option<R> Map<R>(Func<T, Option<R>> func)
        {
            return HasValue ? func(inner) : Option<R>.None;
        }

        public Option<T> Map(Action<T> action)
        {
            if (HasValue)
                action(inner);

            return this;
        }

        public Option<T> Filter(Predicate<T> predicate)
        {
            return HasValue && predicate(inner) ? this : None;
        }

        public Option<R> OfType<R>()
            where R : T
        {
            return Filter(x => x is R).Map(x => (R)x);
        }

        #endregion

        #region Casts

        public static implicit operator Option<T>(T obj)
        {
            return new Option<T>(obj);
        }

        #endregion

        #region Bind and Unwrap Operators

        public static Option<T> operator &(Option<T> a, Func<T, T> func)
        {
            return a.HasValue ? func(a.inner) : None;
        }

        public static Option<T> operator &(Option<T> a, Func<T, Option<T>> func)
        {
            return a.HasValue ? func(a.inner) : None;
        }

        public static Option<T> operator &(Option<T> a, Action<T> action)
        {
            if (a.HasValue)
                action(a.inner);

            return a;
        }

        public static T operator |(Option<T> a, T defaultValue)
        {
            return a.HasValue ? a.inner : defaultValue;
        }

        public static T operator |(Option<T> a, Func<T> defaultValueFunction)
        {
            return a.HasValue ? a.inner : defaultValueFunction();
        }

        #endregion

        #region Operators

        public static bool operator >(Option<T> a, Option<T> b)
        {
            return a.HasValue && b.HasValue && (dynamic)a.inner > (dynamic)b.inner;
        }

        public static bool operator <(Option<T> a, Option<T> b)
        {
            return a.HasValue && b.HasValue && (dynamic)a.inner > (dynamic)b.inner;
        }

        public static bool operator >=(Option<T> a, Option<T> b)
        {
            return a.HasValue && b.HasValue && (dynamic)a.inner >= (dynamic)b.inner;
        }

        public static bool operator <=(Option<T> a, Option<T> b)
        {
            return a.HasValue && b.HasValue && (dynamic)a.inner >= (dynamic)b.inner;
        }

        public static bool operator ==(Option<T> a, Option<T> b)
        {
            return a.HasValue
                    ? (b.HasValue && (dynamic)a.inner == (dynamic)b.inner)
                    : !b.HasValue;
        }

        public static bool operator !=(Option<T> a, Option<T> b)
        {
            return !(a == b);
        }

        public static Option<T> operator +(Option<T> a, Option<T> b)
        {
            return a.HasValue && b.HasValue ? (dynamic)a.inner + (dynamic)b.inner : None;
        }

        public static Option<T> operator -(Option<T> a, Option<T> b)
        {
            return a.HasValue && b.HasValue ? (dynamic)a.inner - (dynamic)b.inner : None;
        }

        public static Option<T> operator *(Option<T> a, Option<T> b)
        {
            return a.HasValue && b.HasValue ? (dynamic)a.inner * (dynamic)b.inner : None;
        }

        public static Option<T> operator /(Option<T> a, Option<T> b)
        {
            return a.HasValue && b.HasValue ? (dynamic)a.inner / (dynamic)b.inner : None;
        }

        public static Option<T> operator %(Option<T> a, Option<T> b)
        {
            return a.HasValue && b.HasValue ? (dynamic)a.inner % (dynamic)b.inner : None;
        }

        public static Option<T> operator ^(Option<T> a, Option<T> b)
        {
            return a.HasValue && b.HasValue ? (dynamic)a.inner ^ (dynamic)b.inner : None;
        }

        public static Option<T> operator <<(Option<T> a, int b)
        {
            return a.HasValue ? (dynamic)a.inner << b : None;
        }

        public static Option<T> operator >>(Option<T> a, int b)
        {
            return a.HasValue ? (dynamic)a.inner >> b : None;
        }

        public static Option<T> operator +(Option<T> a)
        {
            return a.HasValue ? +(dynamic)a.inner : None;
        }

        public static Option<T> operator -(Option<T> a)
        {
            return a.HasValue ? -(dynamic)a.inner : None;
        }

        public static Option<T> operator !(Option<T> a)
        {
            return a.HasValue ? !(dynamic)a.inner : None;
        }

        public static Option<T> operator ~(Option<T> a)
        {
            return a.HasValue ? ~(dynamic)a.inner : None;
        }

        public static Option<T> operator ++(Option<T> a)
        {
            if (a.HasValue)
            {
                var inner = (dynamic)a.inner;
                return ++inner;
            }
            return None;
        }

        public static Option<T> operator --(Option<T> a)
        {
            if (a.HasValue)
            {
                var inner = (dynamic)a.inner;
                return --inner;
            }
            return None;
        }

        #endregion

        #region Overridden Methods

        public override bool Equals(object obj)
        {
            if (obj is T)
            {
                var t = (T)obj;
                return HasValue ? Equals(inner, t) : false;
            }

            if (obj is Option<T>)
            {
                var opt = (Option<T>)obj;
                return HasValue
                    ? (opt.HasValue && Equals(inner, opt.inner))
                    : !opt.HasValue;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HasValue ? inner.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return HasValue ? inner.ToString() : "None";
        }

        #endregion
    }

    public static class OptionExtensions
    {
        public static Option<T> ToOption<T>(this T obj)
        {
            return obj;
        }

        public static Option<T> ToOption<T>(this T? obj)
            where T : struct
        {
            return obj.HasValue ? obj.Value : Option<T>.None;
        }

        public static Option<string> FilterOutEmpty(this Option<string> option)
        {
            return option.Filter(x => !string.IsNullOrEmpty(x));
        }

        public static Option<string> FilterOutWhiteSpace(this Option<string> option)
        {
            return option.Filter(x => !string.IsNullOrWhiteSpace(x));
        }

        public static IEnumerable<T> UnwrapList<T>(this Option<IEnumerable<T>> option)
        {
            return option.Unwrap(() => new T[0]);
        }

        public static IEnumerable<T> FilterOutNone<T>(this IEnumerable<Option<T>> enumerable)
        {
            return enumerable.Where(x => x.HasValue).Select(x => x.Unwrap(() => default(T)));
        }

        public static Option<T> FirstOrOption<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.FirstOrDefault();
        }

        public static Option<T> FirstOrOption<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            return enumerable.FirstOrDefault(predicate);
        }
    }
}
