using System;

using PAC.Maths;

/// <summary>
/// A fraction a / b (where a and b are integers) kept in simplest form.
/// </summary>
/// <remarks>
/// Note that small values can have large <see cref="numerator"/>s and <see cref="denominator"/>s, which could cause overflow in e.g. <see cref="operator +(Rational, Rational)"/>.
/// </remarks>
public readonly struct Rational : IEquatable<Rational>, IEquatable<int>
{
    #region Fields
    /// <summary>
    /// The top number of the fraction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This will always be coprime to the <see cref="denominator"/> and its sign will always be the sign of the <see cref="Rational"/>.
    /// </para>
    /// <para>
    /// If the <see cref="denominator"/> is 0, this will be 0 (and this <see cref="Rational"/> represents an undefined value).
    /// </para>
    /// </remarks>
    /// <seealso cref="denominator"/>
    public readonly int numerator;
    /// <summary>
    /// The bottom number of the fraction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This will always be non-negative and coprime to the <see cref="numerator"/>.
    /// </para>
    /// <para>
    /// If the <see cref="numerator"/> is 0 and this is not 0, this will be 1.
    /// </para>
    /// </remarks>
    /// <see cref="numerator"/>
    public readonly int denominator;
    #endregion

    #region Properties
    /// <summary>
    /// Whether the <see cref="denominator"/> is 1.
    /// </summary>
    public readonly bool isInteger => denominator == 1;
    /// <summary>
    /// Whether the <see cref="Rational"/> is <see cref="Undefined"/>.
    /// </summary>
    /// <remarks>
    /// Note that two <see cref="Undefined"/> values are not considered ==, so use this method to check if a <see cref="Rational"/> is <see cref="Undefined"/>.
    /// </remarks>
    public readonly bool isUndefined => denominator == 0;

    /// <summary>
    /// The sign of the <see cref="Rational"/>.
    /// </summary>
    /// <returns>
    /// <list type="bullet">
    /// <item>
    /// 1 if the value is positive
    /// </item>
    /// <item>
    /// 0 if the value is zero or <see cref="Undefined"/>
    /// </item>
    /// <item>
    /// -1 if the value is negative
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <para>
    /// This will match the sign of the <see cref="numerator"/>.
    /// </para>
    /// </remarks>
    public readonly int sign => Math.Sign(numerator);
    #endregion

    #region Predefined Instances
    /// <summary>
    /// Represents any value with denominator 0, but is stored as 0/0.
    /// </summary>
    /// <remarks>
    /// Note that this will not be considered == to itself. Use <see cref="isUndefined"/> instead to determine if a <see cref="Rational"/> is <see cref="Undefined"/>.
    /// </remarks>
    public static readonly Rational Undefined = new Rational(0, 0);

    /// <summary>
    /// The value 1/2.
    /// </summary>
    public static readonly Rational Half = new Rational(1, 2);

    /// <summary>
    /// The most negative value that can be represented.
    /// </summary>
    /// <seealso cref="MaxValue"/>
    public static readonly Rational MinValue = new Rational(int.MinValue + 1, 1);
    /// <summary>
    /// The most positive value that can be represented.
    /// </summary>
    /// <seealso cref="MinValue"/>
    public static readonly Rational MaxValue = new Rational(int.MaxValue, 1);
    /// <summary>
    /// The smallest positive value that can be represented.
    /// </summary>
    public static readonly Rational Epsilon = new Rational(1, int.MaxValue);
    #endregion

    #region Constructors
    /// <remarks>
    /// Will simplify the fraction.
    /// </remarks>
    /// <param name="numerator">See <see cref="numerator"/>.</param>
    /// <param name="denominator">See <see cref="denominator"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="numerator"/> or <paramref name="denominator"/> are <see cref="int.MinValue"/>.</exception>
    public Rational(int numerator, int denominator)
    {
        if (numerator == int.MinValue)
        {
            throw new ArgumentOutOfRangeException(nameof(numerator), $"{nameof(numerator)} cannot be int.MinValue.");
        }
        if (denominator == int.MinValue)
        {
            throw new ArgumentOutOfRangeException(nameof(denominator), $"{nameof(denominator)} cannot be int.MinValue as the denominator will be made positive, but -int.MinValue cannot be " +
                $"represented as an int.");
        }

        // Make denominator non-negative
        if (denominator < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        if (denominator == 0)
        {
            this.numerator = 0;
            this.denominator = 0;
        }
        else if (numerator == 0)
        {
            this.numerator = 0;
            this.denominator = 1;
        }
        else
        {
            int gcd = MathExtensions.Gcd(numerator, denominator);
            this.numerator = numerator / gcd;
            this.denominator = denominator / gcd;
        }
    }
    #endregion

    #region Conversion
    /// <summary>
    /// Converts the integer into a <see cref="Rational"/> with <see cref="denominator"/> 1.
    /// </summary>
    public static implicit operator Rational(int integer) => new Rational(integer, 1);

    /// <summary>
    /// Rounds the <see cref="Rational"/> towards zero.
    /// </summary>
    /// <exception cref="DivideByZeroException"><paramref name="rational"/> is <see cref="Undefined"/>.</exception>
    public static explicit operator int(Rational rational) => rational.numerator / rational.denominator;
    /// <summary>
    /// Converts the <see cref="Rational"/> to a <see cref="float"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="Undefined"/> will be converted to <see cref="float.NaN"/>.
    /// </remarks>
    public static implicit operator float(Rational rational) => rational.isUndefined ? float.NaN : rational.numerator / (float)rational.denominator;
    #endregion

    #region Comparison
    /// <summary>
    /// Whether the two <see cref="Rational"/>s represent the same fraction.
    /// </summary>
    /// <remarks>
    /// Two <see cref="Undefined"/> values are not considered ==. Use <see cref="isUndefined"/> to check if a <see cref="Rational"/> is <see cref="Undefined"/>.
    /// </remarks>
    public static bool operator ==(Rational a, Rational b) => !a.isUndefined && !b.isUndefined && a.numerator == b.numerator && a.denominator == b.denominator;
    /// <summary>
    /// See <see cref="operator ==(Rational, Rational)"/>.
    /// </summary>
    public static bool operator !=(Rational a, Rational b) => !(a == b);
    /// <summary>
    /// See <see cref="operator ==(Rational, Rational)"/>.
    /// </summary>
    public bool Equals(Rational other) => this == other;
    /// <summary>
    /// See <see cref="implicit operator Rational(int)"/> and <see cref="operator ==(Rational, Rational)"/>.
    /// </summary>
    public bool Equals(int other) => this == other;
    /// <summary>
    /// See <see cref="Equals(Rational)"/>.
    /// </summary>
    public override bool Equals(object obj) => obj is Rational other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(numerator, denominator);

    /// <summary>
    /// Whether <paramref name="a"/> &lt; <paramref name="b"/>.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="false"/> if either of <paramref name="a"/> or <paramref name="b"/> are <see cref="Undefined"/>.
    /// </remarks>
    public static bool operator <(Rational a, Rational b) => (a.isUndefined || b.isUndefined) ? false : a.numerator * b.denominator < b.numerator * a.denominator;
    /// <summary>
    /// Whether <paramref name="a"/> &lt;= <paramref name="b"/>.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="false"/> if either of <paramref name="a"/> or <paramref name="b"/> are <see cref="Undefined"/>.
    /// </remarks>
    public static bool operator <=(Rational a, Rational b) => (a.isUndefined || b.isUndefined) ? false : a.numerator * b.denominator <= b.numerator * a.denominator;
    /// <summary>
    /// Whether <paramref name="a"/> &gt; <paramref name="b"/>.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="false"/> if either of <paramref name="a"/> or <paramref name="b"/> are <see cref="Undefined"/>.
    /// </remarks>
    public static bool operator >(Rational a, Rational b) => (a.isUndefined || b.isUndefined) ? false : a.numerator * b.denominator > b.numerator * a.denominator;
    /// <summary>
    /// Whether <paramref name="a"/> &gt;= <paramref name="b"/>.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="false"/> if either of <paramref name="a"/> or <paramref name="b"/> are <see cref="Undefined"/>.
    /// </remarks>
    public static bool operator >=(Rational a, Rational b) => (a.isUndefined || b.isUndefined) ? false : a.numerator * b.denominator >= b.numerator * a.denominator;
    #endregion

    #region Operations
    /// <remarks>
    /// If either of <paramref name="a"/> and <paramref name="b"/> are <see cref="Undefined"/>, it will return <see cref="Undefined"/>.
    /// </remarks>
    public static Rational operator +(Rational a, Rational b) => new Rational(a.numerator * b.denominator + b.numerator * a.denominator, a.denominator * b.denominator);
    /// <remarks>
    /// If either of <paramref name="a"/> and <paramref name="b"/> are <see cref="Undefined"/>, it will return <see cref="Undefined"/>.
    /// </remarks>
    public static Rational operator -(Rational a, Rational b) => a + (-b);
    /// <remarks>
    /// If <paramref name="a"/> is <see cref="Undefined"/>, it will return <see cref="Undefined"/>.
    /// </remarks>
    public static Rational operator -(Rational a) => new Rational(-a.numerator, a.denominator);
    /// <remarks>
    /// If either of <paramref name="a"/> and <paramref name="b"/> are <see cref="Undefined"/>, it will return <see cref="Undefined"/>.
    /// </remarks>
    public static Rational operator *(Rational a, Rational b) => new Rational(a.numerator * b.numerator, a.denominator * b.denominator);
    /// <remarks>
    /// If either of <paramref name="a"/> and <paramref name="b"/> are <see cref="Undefined"/>, or if <paramref name="b"/> is 0, it will return <see cref="Undefined"/>.
    /// </remarks>
    public static Rational operator /(Rational a, Rational b) => b.isUndefined ? Undefined : new Rational(a.numerator * b.denominator, a.denominator * b.numerator);
    /// <summary>
    /// Returns the smallest non-negative <see cref="Rational"/> r such that there is an integer q such that <paramref name="a"/> = q * <paramref name="b"/> + r.
    /// </summary>
    /// <remarks>
    /// If either of <paramref name="a"/> and <paramref name="b"/> are <see cref="Undefined"/>, it will return <see cref="Undefined"/>.
    /// </remarks>
    public static Rational operator %(Rational a, Rational b)
        => (a.isUndefined || b.isUndefined) ? Undefined : new Rational((a.numerator * b.denominator).Mod(b.numerator * a.denominator), a.denominator * b.denominator);

    /// <summary>
    /// Returns the absolute value of <paramref name="a"/>.
    /// </summary>
    /// <remarks>
    /// If <paramref name="a"/> is <see cref="Undefined"/>, it will return <see cref="Undefined"/>.
    /// </remarks>
    public static Rational Abs(Rational a) => new Rational(Math.Abs(a.numerator), a.denominator);
    #endregion

    public override string ToString() => denominator switch
    {
        0 => "Undefined",
        1 => numerator.ToString(),
        _ => $"{numerator} / {denominator}"
    };
}