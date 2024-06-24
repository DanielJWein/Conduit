namespace Conduit.Net.Events;

/// <summary>
/// Represents a control packet event args structure.
/// </summary>
/// <remarks> Creates a new ControlEventArgs </remarks>
/// <param name="data"> The data for the control event </param>
public class ControlEventArgs( byte[ ] data ) : EventArgs {

    /// <summary>
    /// The data of the control event
    /// </summary>
    public byte[ ] Data { get; } = data;
}
