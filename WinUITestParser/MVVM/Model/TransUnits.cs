using System.Collections.Generic;

namespace WinUITestParser.MVVM.Model;

public class TransUnit
{
    public string Id { get; set; }

    public string Datatype { get; set; }

    public string Source { get; set; }

    public string? Target { get; set; }

    public List<TransUnitNote> Notes { get; set; }

    public List<TransUnitContextGroup> ContextGroups { get; set; }

    public int LineCount { get; set; }
    public int LocalLineNumber { get; set; }
    public int GlobalLineNumber { get; set; }
    public string Xml { get; set; }

    public TransUnit()
    {
        Notes = new();
        ContextGroups = new();
    }

    /// <summary>
    /// Копирование объекта
    /// </summary>
    /// <param name="unit">Исходный объект</param>
    public TransUnit(TransUnit unit)
    {
        Id = unit.Id;
        Datatype = unit.Datatype;
        Source = unit.Source;
        Target = unit.Target;

        Notes = new();
        foreach (var note in unit.Notes)
        {
            Notes.Add(new()
            {
                Priority = note.Priority,
                From = note.From,
                Text = note.Text,
            });
        }

        ContextGroups = new();
        foreach (var contextGroup in unit.ContextGroups)
        {
            var newContextGroups = new TransUnitContextGroup();
            newContextGroups.Purpose = contextGroup.Purpose;
            foreach (var contexts in contextGroup.Contexts)
            {
                newContextGroups.Contexts.Add(new()
                {
                    ContextType = contexts.ContextType,
                    Text = contexts.Text,
                });
            }

            ContextGroups.Add(newContextGroups);
        }
    }


    // flags
    public bool IsSomeDifferent { get; set; }

    public bool IsNotNow { get; set; }

    public bool IsNew { get; set; }
}

public class TransUnitNote
{
    public string Priority { get; set; }

    public string From { get; set; }

    public string Text { get; set; }


    public int LocalLineNumber { get; set; }
    public int GlobalLineNumber { get; set; }
    public string Xml { get; set; }


    // flags
    public bool IsNotNow { get; set; }

    public bool IsNew { get; set; }
}

public class TransUnitContextGroup
{
    public string Purpose { get; set; }

    public List<ContextItem> Contexts { get; set; }

    public int LineCount { get; set; }
    public int LocalLineNumber { get; set; }
    public int GlobalLineNumber { get; set; }
    public string Xml { get; set; }

    public TransUnitContextGroup()
    {
        Contexts = new();
    }


    // flags
    public bool IsNotNow { get; set; }

    public bool IsNew { get; set; }
}

public class ContextItem
{
    public string ContextType { get; set; }

    public string Text { get; set; }

    public int LocalLineNumber { get; set; }
    public int GlobalLineNumber { get; set; }
    public string Xml { get; set; }


    // flags
    public bool IsNew { get; set; }
}
