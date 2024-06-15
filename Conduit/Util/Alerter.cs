namespace Conduit.Util;

/// <summary>
/// Represents a value that raises an event when changed.
/// </summary>
/// <typeparam name="T"> The type of the object. </typeparam>
/// <remarks> Creates a new Alerter </remarks>
/// <param name="defaultValue"> </param>
public class Alerter<T>( T defaultValue = default ) where T : IEquatable<T> {

    /// <summary>
    /// The backing field for this Alerter.
    /// </summary>
    private T backingfield = defaultValue;

    /// <summary>
    /// The value for this alerter.
    /// </summary>
    public T Value {
        get => backingfield;
        set {
            if ( !backingfield?.Equals( value ) ?? true ) {
                backingfield = value;
                ValueChanged?.Invoke( this, null );
            }
        }
    }

    /// <summary>
    /// Raised when this Alerter is changed.
    /// </summary>
    public event EventHandler ValueChanged;

    /// <summary>
    /// Exposes the backing field as an implicit cast
    /// </summary>
    /// <param name="a"> </param>
    public static implicit operator T( Alerter<T> a ) => a.backingfield;
}
