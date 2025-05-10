using System.Collections.Generic;

namespace CerberusFramework.Core.UI.Popups.Helpers
{
    public static class PopupHelpers
    {
        private static readonly Queue<PopupQueueItem> AlwaysAtEnd = new Queue<PopupQueueItem>(32);
        private static readonly Queue<PopupQueueItem> Possible = new Queue<PopupQueueItem>(32);
        private static readonly Queue<PopupQueueItem> ImPossible = new Queue<PopupQueueItem>(32);

        public static void ReorderPopupsForDisplayedScreenIndex(Queue<PopupQueueItem> popupQueue)
        {
            try
            {
                if (popupQueue.Count <= 1)
                {
                    return;
                }

                var array = popupQueue.ToArray();

                foreach (var queueItem in array)
                {
                    if (queueItem.ScreenAnchor == CFScreenAnchors.Any)
                    {
                        if (queueItem.QueuePosition == QueuePosition.MoveToEndAlways)
                        {
                            AlwaysAtEnd.Enqueue(queueItem);
                        }
                        else
                        {
                            Possible.Enqueue(queueItem);
                        }
                    }
                    else
                    {
                        ImPossible.Enqueue(queueItem);
                    }
                }

                if (Possible.Count == 0 && AlwaysAtEnd.Count == 0)
                {
                    return;
                }

                popupQueue.Clear();
                while (Possible.Count > 0)
                {
                    popupQueue.Enqueue(Possible.Dequeue());
                }

                while (AlwaysAtEnd.Count > 0)
                {
                    popupQueue.Enqueue(AlwaysAtEnd.Dequeue());
                }

                while (ImPossible.Count > 0)
                {
                    popupQueue.Enqueue(ImPossible.Dequeue());
                }
            }
            finally
            {
                AlwaysAtEnd.Clear();
                Possible.Clear();
                ImPossible.Clear();
            }
        }
    }
}