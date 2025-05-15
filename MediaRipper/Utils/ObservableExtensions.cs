using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MediaRipper.Utils;

public static class ObservableExtensions
{
    /// <summary>
    /// Maps the observable collection to a target collection but also listing for future collection changes.
    /// </summary>
    /// <param name="sources">The source collection.</param>
    /// <param name="targets">The target collection to map the items to.</param>
    /// <param name="transformer">The transformer method to convert an source item into a target item.</param>
    /// <typeparam name="TSource">The source item.</typeparam>
    /// <typeparam name="TTarget">The target item.</typeparam>
    /// <returns>Returns the handler in case you want to remove it.</returns>
    public static NotifyCollectionChangedEventHandler MapAndObserve<TSource, TTarget>(this ObservableCollection<TSource> sources,
        ObservableCollection<TTarget> targets, Func<TSource, TTarget> transformer)
    {
        // Maps all current elements.
        targets.Clear();
        foreach (var source in sources)
        {
            var target = transformer(source);
            targets.Add(target);
        }
        
        // Add an event listener for future changes.
        NotifyCollectionChangedEventHandler handler = (_, args) =>
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (args.NewItems is not null)
                    {
                        var index = args.NewStartingIndex;
                        foreach (TSource source in args.NewItems)
                        {
                            var target = transformer(source);
                            targets.Insert(index++, target);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (args.OldItems is not null)
                    {
                        for (var i = 0; i < args.OldItems.Count; i++)
                        {
                            targets.RemoveAt(args.OldStartingIndex);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException();
                case NotifyCollectionChangedAction.Reset:
                    targets.Clear();
                    break;
            }
        };

        sources.CollectionChanged += handler;
        return handler;
    }
}