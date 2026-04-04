namespace MacroMission.Application.Common.Messaging;

/// <summary>Marker for commands that return no value (Result).</summary>
public interface ICommand;

/// <summary>Marker for commands that return a value (Result&lt;TResponse&gt;).</summary>
public interface ICommand<TResponse>;
