namespace Conduit.Net.Events;

/// <summary>
/// Represents a control packet event args structure.
/// </summary>
/// <remarks> Creates a new ControlEventArgs </remarks>
/// <param name="Data"> The data for the control event </param>
public class ControlEventArgs( byte[ ] Data ) : EventArgs {

    /// <summary>
    /// The data of the control event
    /// </summary>
    public byte[ ] data { get; } = Data;
}
